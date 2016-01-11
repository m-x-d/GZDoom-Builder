
#region ================== Copyright (c) 2007 Pascal vd Heiden

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.IO;
using CodeImp.DoomBuilder.Compilers;
using CodeImp.DoomBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.ZDoom
{
	public abstract class ZDTextParser
	{
		#region ================== Constants

		protected static readonly string RELATIVE_PATH_MARKER = ".." + Path.DirectorySeparatorChar;
		protected static readonly string CURRENT_FOLDER_PATH_MARKER = "." + Path.DirectorySeparatorChar;
		protected static readonly string ALT_RELATIVE_PATH_MARKER = ".." + Path.AltDirectorySeparatorChar;
		protected static readonly string ALT_CURRENT_FOLDER_PATH_MARKER = "." + Path.AltDirectorySeparatorChar;
		
		#endregion
		
		#region ================== Variables
		
		// Parsing
		protected string whitespace = "\n \t\r\u00A0\0"; //mxd. non-breaking space is also space :)
		protected string specialtokens = ":{}+-\n;";
		
		// Input data stream
		protected Stream datastream;
		protected BinaryReader datareader;
		protected string sourcename;
		
		// Error report
		private int errorline;
		private string errordesc;
		private string errorsource;
		private long prevstreamposition; //mxd. Text stream position storted before performing ReadToken.
		
		#endregion
		
		#region ================== Properties
		
		internal Stream DataStream { get { return datastream; } }
		internal BinaryReader DataReader { get { return datareader; } }
		public int ErrorLine { get { return errorline; } }
		public string ErrorDescription { get { return errordesc; } }
		public string ErrorSource { get { return errorsource; } }
		public bool HasError { get { return (errordesc != null); } }
		
		#endregion
		
		#region ================== Constructor / Disposer
		
		// Constructor
		protected ZDTextParser()
		{
			// Initialize
			errordesc = null;
		}
		
		#endregion

		#region ================== Parsing

		//mxd. This parses the given decorate stream. Returns false on errors
		public virtual bool Parse(Stream stream, string sourcefilename, bool clearerrors)
		{
			//mxd. Clear error status?
			if(clearerrors) ClearError();

			//mxd. Integrity checks
			if(stream == null)
			{
				ReportError("Unable to load \"" + sourcefilename + "\"");
				return false;
			}

			if(stream.Length == 0)
			{
				if(!string.IsNullOrEmpty(sourcename))
					LogWarning("Include file \"" + sourcefilename + "\" is empty");
				else
					LogWarning("File is empty");
				return true;
			}
			
			datastream = stream;
			datareader = new BinaryReader(stream, Encoding.ASCII);
			sourcename = sourcefilename;
			datastream.Seek(0, SeekOrigin.Begin);
			return true;
		}
		
		// This returns true if the given character is whitespace
		private bool IsWhitespace(char c)
		{
			return (whitespace.IndexOf(c) > -1);
		}

		// This returns true if the given character is a special token
		private bool IsSpecialToken(char c)
		{
			return (specialtokens.IndexOf(c) > -1);
		}

		// This returns true if the given character is a special token
		protected internal bool IsSpecialToken(string s)
		{
			if(s.Length > 0) return (specialtokens.IndexOf(s[0]) > -1);
			return false;
		}

		//mxd. This removes beginning and ending quotes from a token
		protected internal string StripTokenQuotes(string token) 
		{
			return StripQuotes(token);
		}
		
		// This removes beginning and ending quotes from a token
		internal static string StripQuotes(string token)
		{
			// Remove first character, if it is a quote
			if(!string.IsNullOrEmpty(token) && (token[0] == '"'))
				token = token.Substring(1);
			
			// Remove last character, if it is a quote
			if(!string.IsNullOrEmpty(token) && (token[token.Length - 1] == '"'))
				token = token.Substring(0, token.Length - 1);
			
			return token;
		}
		
		// This skips whitespace on the stream, placing the read
		// position right before the first non-whitespace character
		// Returns false when the end of the stream is reached
		protected internal bool SkipWhitespace(bool skipnewline)
		{
			int offset = skipnewline ? 0 : 1;
			char c;
			prevstreamposition = datastream.Position; //mxd
			
			do
			{
				if(datastream.Position == datastream.Length) return false;
				c = (char)datareader.ReadByte();

				// Check if this is comment
				if(c == '/')
				{
					if(datastream.Position == datastream.Length) return false;
					char c2 = (char)datareader.ReadByte();
					if(c2 == '/')
					{
						// Check if not a special comment with a token
						if(datastream.Position == datastream.Length) return false;
						char c3 = (char)datareader.ReadByte();
						if(c3 != '$')
						{
							// Skip entire line
							char c4 = ' ';
							while((c4 != '\n') && (datastream.Position < datastream.Length)) { c4 = (char)datareader.ReadByte(); }
							c = c4;
						}
						else
						{
							// Not a comment
							c = c3;
						}
					}
					else if(c2 == '*')
					{
						// Skip until */
						char c4, c3 = '\0';
						prevstreamposition = datastream.Position; //mxd
						do
						{
							if(datastream.Position == datastream.Length) //mxd
							{
								// ZDoom doesn't give even a warning message about this, so we shouldn't report error or fail parsing.
								General.ErrorLogger.Add(ErrorType.Warning, "DECORATE warning in '" + sourcename + "', line " + GetCurrentLineNumber() + ". Block comment is not closed.");
								return false;
							}

							c4 = c3;
							c3 = (char)datareader.ReadByte();
						}
						while((c4 != '*') || (c3 != '/'));
						c = ' ';
					}
					else
					{
						// Not a comment, rewind from reading c2
						datastream.Seek(-1, SeekOrigin.Current);
					}
				}
			}
			while(whitespace.IndexOf(c, offset) > -1);
			
			// Go one character back so we can read this non-whitespace character again
			datastream.Seek(-1, SeekOrigin.Current);
			return true;
		}
		
		// This reads a token (all sequential non-whitespace characters or a single character)
		// Returns null when the end of the stream has been reached
		protected internal string ReadToken() { return ReadToken(true); } //mxd. Added "multiline" param
		protected internal string ReadToken(bool multiline)
		{
			//mxd. Return empty string when the end of the stream has been reached
			if(datastream.Position == datastream.Length) return string.Empty;
			
			//mxd. Store starting position 
			prevstreamposition = datastream.Position;
			
			string token = "";
			bool quotedstring = false;
			
			// Start reading
			char c = (char)datareader.ReadByte();
			while(!IsWhitespace(c) || quotedstring || IsSpecialToken(c))
			{
				//mxd. Break at newline?
				if(!multiline && c == '\r')
				{
					// Go one character back so line number is correct
					datastream.Seek(-1, SeekOrigin.Current);
					return token;
				}
				
				// Special token?
				if(!quotedstring && IsSpecialToken(c))
				{
					// Not reading a token yet?
					if(token.Length == 0)
					{
						// This is our whole token
						token += c;
						break;
					}
					else
					{
						// This is a new token and shouldn't be read now
						// Go one character back so we can read this token again
						datastream.Seek(-1, SeekOrigin.Current);
						break;
					}
				}
				else
				{
					// Quote?
					if(c == '"')
					{
						// Quote to end the string?
						if(quotedstring) quotedstring = false;
						
						// First character is a quote?
						if(token.Length == 0) quotedstring = true;
						
						token += c;
					}
					// Potential comment?
					else if((c == '/') && !quotedstring)
					{
						// Check the next byte
						if(datastream.Position == datastream.Length) return token;
						char c2 = (char)datareader.ReadByte();
						if((c2 == '/') || (c2 == '*'))
						{
							// This is a comment start, so the token ends here
							// Go two characters back so we can read this comment again
							datastream.Seek(-2, SeekOrigin.Current);
							break;
						}
						else
						{
							// Not a comment
							// Go one character back so we can read this char again
							datastream.Seek(-1, SeekOrigin.Current);
							token += c;
						}
					}
					else
					{
						token += c;
					}
				}
				
				// Next character
				if(datastream.Position < datastream.Length)
					c = (char)datareader.Read();
				else
					break;
			}
			
			return token;
		}

		// This reads a token (all sequential non-whitespace characters or a single character) using custom set of special tokens
		// Returns null when the end of the stream has been reached (mxd)
		protected internal string ReadToken(string specialTokens) 
		{
			// Return null when the end of the stream has been reached
			if(datastream.Position == datastream.Length) return null;

			//mxd. Store starting position 
			prevstreamposition = datastream.Position;
			
			string token = "";
			bool quotedstring = false;

			// Start reading
			char c = (char)datareader.ReadByte();
			while(!IsWhitespace(c) || quotedstring || specialTokens.IndexOf(c) != -1) 
			{
				// Special token?
				if(!quotedstring && specialTokens.IndexOf(c) != -1) 
				{
					// Not reading a token yet?
					if(token.Length == 0) 
					{
						// This is our whole token
						token += c;
						break;
					} 

					// This is a new token and shouldn't be read now
					// Go one character back so we can read this token again
					datastream.Seek(-1, SeekOrigin.Current);
					break;
				} 
				else 
				{
					// Quote?
					if(c == '"') 
					{
						// Quote to end the string?
						if(quotedstring) quotedstring = false;

						// First character is a quote?
						if(token.Length == 0) quotedstring = true;

						token += c;
					}
					// Potential comment?
					else if((c == '/') && !quotedstring) 
					{
						// Check the next byte
						if(datastream.Position == datastream.Length) return token;
						char c2 = (char)datareader.ReadByte();
						if((c2 == '/') || (c2 == '*')) 
						{
							// This is a comment start, so the token ends here
							// Go two characters back so we can read this comment again
							datastream.Seek(-2, SeekOrigin.Current);
							break;
						} 
						else 
						{
							// Not a comment
							// Go one character back so we can read this char again
							datastream.Seek(-1, SeekOrigin.Current);
							token += c;
						}
					} 
					else 
					{
						token += c;
					}
				}

				// Next character
				if(datastream.Position < datastream.Length)
					c = (char)datareader.Read();
				else
					break;
			}

			return token;
		}

		// This reads the rest of the line
		// Returns null when the end of the stream has been reached
		protected internal string ReadLine()
		{
			string token = "";

			// Return null when the end of the stream has been reached
			if(datastream.Position == datastream.Length) return null;

			// Start reading
			char c = (char)datareader.ReadByte();
			while(c != '\n')
			{
				token += c;
				
				// Next character
				if(datastream.Position < datastream.Length)
					c = (char)datareader.Read();
				else
					break;
			}

			return token.Trim();
		}

		//mxd
		internal bool NextTokenIs(string expectedtoken) 
		{
			return NextTokenIs(expectedtoken, true);
		}

		//mxd
		internal bool NextTokenIs(string expectedtoken, bool reporterror) 
		{
			if(!SkipWhitespace(true)) return false;
			string token = ReadToken();

			if(string.Compare(token, expectedtoken, true) != 0)
			{
				if(reporterror) ReportError("Expected '" + expectedtoken + "', but got '" + token + "'");

				// Rewind so this structure can be read again
				DataStream.Seek(-token.Length - 1, SeekOrigin.Current);
				return false;
			}

			return true;
		}

		//mxd
		protected internal bool ReadSignedFloat(ref float value) { return ReadSignedFloat(StripTokenQuotes(ReadToken(false)), ref value); }
		protected internal bool ReadSignedFloat(string token, ref float value) 
		{
			int sign = 1;
			if(token == "-") 
			{
				sign = -1;
				token = StripTokenQuotes(ReadToken(false));
			}

			float val;
			bool success = float.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out val);
			if(success) value = val * sign;
			return success;
		}

		//mxd
		protected internal bool ReadSignedInt(ref int value) { return ReadSignedInt(StripTokenQuotes(ReadToken(false)), ref value); }
		protected internal bool ReadSignedInt(string token, ref int value) 
		{
			int sign = 1;
			if(token == "-") 
			{
				sign = -1;
				token = StripTokenQuotes(ReadToken(false));
			}

			int val;
			bool success = int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out val);
			if(success) value = val * sign;
			return success;
		}

		//mxd
		protected void SkipStructure() { SkipStructure(new HashSet<string>()); }
		protected void SkipStructure(HashSet<string> breakat)
		{
			string token;
			do
			{
				if(!SkipWhitespace(true)) break;
				token = ReadToken();
				if(string.IsNullOrEmpty(token)) break;
				if(breakat.Contains(token))
				{
					DataStream.Seek(-token.Length - 1, SeekOrigin.Current);
					return;
				}
			}
			while(token != "{");
			int scopelevel = 1;
			do
			{
				if(!SkipWhitespace(true)) break;
				token = ReadToken();
				if(string.IsNullOrEmpty(token)) break;
				if(token == "{") scopelevel++;
				if(token == "}") scopelevel--;
			}
			while(scopelevel > 0);
		}
		
		// This reports an error
		protected internal void ReportError(string message)
		{
			// Set error information
			errordesc = message;
			errorline = (datastream != null ? GetCurrentLineNumber() : CompilerError.NO_LINE_NUMBER); //mxd
			errorsource = sourcename;
		}

		//mxd. This adds a warning to the ErrorLogger
		protected internal void LogWarning(string message)
		{
			// Add a warning
			int errline = GetCurrentLineNumber();
			General.ErrorLogger.Add(ErrorType.Warning, GetLanguageType() + " warning in '" + sourcename
								+ (errline != CompilerError.NO_LINE_NUMBER ? "', line " + (errline + 1) : "'") + ". " 
								+ message + ".");
		}

		//mxd. This adds an error to the ErrorLogger
		public void LogError()
		{
			General.ErrorLogger.Add(ErrorType.Error, GetLanguageType() + " error in '" + errorsource
								+ (errorline != CompilerError.NO_LINE_NUMBER ? "', line " + (errorline + 1) : "'") + ". "
								+ errordesc + ".");
		}

		//mxd
		protected void ClearError()
		{
			errordesc = null;
			errorsource = null;
			errorline = CompilerError.NO_LINE_NUMBER;
		}

		//mxd 
		protected int GetCurrentLineNumber()
		{
			long pos = datastream.Position;
			long finishpos = Math.Min(prevstreamposition, pos);
			long readpos = 0;
			int linenumber = -1;

			// Find the line on which we found this error
			datastream.Seek(0, SeekOrigin.Begin);
			StreamReader textreader = new StreamReader(datastream, Encoding.ASCII);
			while(readpos < finishpos) 
			{
				string line = textreader.ReadLine();
				if(line == null) break;
				readpos += line.Length + 2;
				linenumber++;
			}

			// Return to original position
			datastream.Seek(pos, SeekOrigin.Begin);

			return Math.Max(linenumber, 0);
		}

		//mxd. This converts given path to be relative to "filename"
		protected string GetRootedPath(string filename, string includefilename)
		{
			// Construct root-relative path...
			filename = filename.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

			// Filename absolute? Try to convert it to resource-rooted
			if(Path.IsPathRooted(filename))
			{
				foreach(DataReader reader in General.Map.Data.Containers)
				{
					if(reader is DirectoryReader && filename.StartsWith(reader.Location.location, true, CultureInfo.InvariantCulture))
					{
						filename = filename.Substring(reader.Location.location.Length + 1, filename.Length - reader.Location.location.Length - 1);
						break;
					}
				}
			}
			
			string filepath = Path.GetDirectoryName(filename);
			string result = includefilename.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

			if(result.StartsWith(RELATIVE_PATH_MARKER) && !string.IsNullOrEmpty(filepath))
			{
				string[] parts = filepath.Split(Path.DirectorySeparatorChar);
				int index = parts.Length - 1;
				int pos;

				// Count & trim relative path markers
				while((pos = result.LastIndexOf(RELATIVE_PATH_MARKER, StringComparison.Ordinal)) != -1)
				{
					// includefilename references something above the root?
					if(index-- < 0)
					{
						ReportError("Unable to construct rooted path from '" + includefilename + "'");
						return string.Empty;
					}

					string start = result.Substring(0, pos);
					string end = result.Substring(pos + RELATIVE_PATH_MARKER.Length, result.Length - pos - RELATIVE_PATH_MARKER.Length);
					result = start + end;
				}

				// Construct absolute path relative to current filename
				while(index > -1)
				{
					result = parts[index--] + Path.DirectorySeparatorChar + result;
				}
			}
			else
			{
				// Trim "this folder" marker
				if(result.StartsWith(CURRENT_FOLDER_PATH_MARKER))
					result = result.TrimStart(CURRENT_FOLDER_PATH_MARKER.ToCharArray());

				// Treat as relative path
				if(!string.IsNullOrEmpty(filepath))
					result = Path.Combine(filepath, result);
			}

			return result;
		}

		//mxd. This replicates System.IO.Path.CheckInvalidPathChars() internal function
		public bool CheckInvalidPathChars(string path)
		{
			foreach(char c in path)
			{
				int num = c;
				switch(num)
				{
					case 34: case 60: case 62: case 124:
						ReportError("Unsupported character \"" + c + "\" in path \"" + path + "\"");
						return false;

					default:
						if(num >= 32) continue;
						goto case 34;
				}
			}

			return true;
		}

		//mxd. Language type
		protected abstract string GetLanguageType();

		#endregion
	}
}
