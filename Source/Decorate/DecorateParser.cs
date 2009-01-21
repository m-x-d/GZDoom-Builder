
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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Data;
using System.IO;
using System.Diagnostics;
using CodeImp.DoomBuilder.Compilers;

#endregion

namespace CodeImp.DoomBuilder.Decorate
{
	public sealed class DecorateParser
	{
		#region ================== Delegates

		public delegate void IncludeDelegate(DecorateParser parser, string includefile);
		
		public IncludeDelegate OnInclude;
		
		#endregion
		
		#region ================== Constants

		// Parsing
		private const string WHITESPACE = "\n \t\r";
		private const string SPECIALTOKEN = ":{}+-\n";
		
		#endregion
		
		#region ================== Variables
		
		// Objects
		private List<ActorStructure> actors;
		
		// Input data stream
		private Stream datastream;
		private BinaryReader datareader;
		private string sourcename;
		
		// Error report
		private int errorline;
		private string errordesc;
		private string errorsource;
		
		#endregion
		
		#region ================== Properties
		
		internal Stream DataStream { get { return datastream; } }
		internal BinaryReader DataReader { get { return datareader; } }
		public ICollection<ActorStructure> Actors { get { return actors; } }
		public int ErrorLine { get { return errorline; } }
		public string ErrorDescription { get { return errordesc; } }
		public string ErrorSource { get { return errorsource; } }
		public bool HasError { get { return (errordesc != null); } }
		
		#endregion
		
		#region ================== Constructor / Disposer
		
		// Constructor
		public DecorateParser()
		{
			// Initialize
			actors = new List<ActorStructure>();
			errordesc = null;
		}
		
		#endregion
		
		#region ================== Methods
		
		// This parses the given decorate stream
		// Returns false on errors
		public bool Parse(Stream stream, string sourcefilename)
		{
			Stream localstream = stream;
			string localsourcename = sourcefilename;
			BinaryReader localreader = new BinaryReader(localstream, Encoding.ASCII);
			datastream = localstream;
			datareader = localreader;
			sourcename = localsourcename;
			datastream.Seek(0, SeekOrigin.Begin);
			
			// Continue until at the end of the stream
			while(SkipWhitespace(true))
			{
				// Read a token
				string objdeclaration = ReadToken();
				if(objdeclaration != null)
				{
					objdeclaration = objdeclaration.ToLowerInvariant();
					if(objdeclaration == "actor")
					{
						// Read actor structure
						ActorStructure actor = new ActorStructure(this);
						if(this.HasError) break;
						General.WriteLogLine("Added actor '" + actor.Name + "' from '" + localsourcename + "'");
						actors.Add(actor);
					}
					else if(objdeclaration == "#include")
					{
						// Include a file
						SkipWhitespace(true);
						string filename = ReadToken();
						if(!string.IsNullOrEmpty(filename))
						{
							// Strip the quotes
							filename = filename.Replace("\"", "");

							// Callback to parse this file now
							if(OnInclude != null) OnInclude(this, filename);

							// Set our buffers back to continue parsing
							datastream = localstream;
							datareader = localreader;
							sourcename = localsourcename;
							if(HasError) break;
						}
						else
						{
							ReportError("Expected file name to include");
							break;
						}
					}
					else
					{
						// Unknown structure!
						// Best we can do now is just find the first { and the follow the scopes until the matching } is found
						string token2;
						do
						{
							SkipWhitespace(true);
							token2 = ReadToken();
						}
						while(token2 != "{");
						int scopelevel = 1;
						do
						{
							SkipWhitespace(true);
							token2 = ReadToken();
							if(token2 == "{") scopelevel++;
							if(token2 == "}") scopelevel--;
						}
						while(scopelevel > 0);
					}
				}
			}
			
			// Return true when no errors occurred
			return (errordesc == null);
		}
		
		// This returns true if the given character is whitespace
		internal bool IsWhitespace(char c)
		{
			return (WHITESPACE.IndexOf(c) > -1);
		}

		// This returns true if the given character is a special token
		internal bool IsSpecialToken(char c)
		{
			return (SPECIALTOKEN.IndexOf(c) > -1);
		}

		// This returns true if the given character is a special token
		internal bool IsSpecialToken(string s)
		{
			if(s.Length > 0)
				return (SPECIALTOKEN.IndexOf(s[0]) > -1);
			else
				return false;
		}
		
		// This skips whitespace on the stream, placing the read
		// position right before the first non-whitespace character
		// Returns false when the end of the stream is reached
		internal bool SkipWhitespace(bool skipnewline)
		{
			int offset = skipnewline ? 0 : 1;
			char c;
			
			do
			{
				if(datastream.Position == datastream.Length) return false;
				c = (char)datareader.ReadByte();

				// Check if this is comment
				if(c == '/')
				{
					char c2 = (char)datareader.ReadByte();
					if(c2 == '/')
					{
						// Skip entire line
						char c3;
						do { c3 = (char)datareader.ReadByte(); } while(c3 != '\n');
						c = ' ';
					}
					else if(c2 == '*')
					{
						// Skip until */
						char c4, c3 = '\0';
						do
						{
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
			while(WHITESPACE.IndexOf(c, offset) > -1);
			
			// Go one character back so we can read this non-whitespace character again
			datastream.Seek(-1, SeekOrigin.Current);
			return true;
		}
		
		// This reads a token (all sequential non-whitespace characters or a single character)
		// Returns null when the end of the stream has been reached
		internal string ReadToken()
		{
			string token = "";
			bool quotedstring = false;

			// Return null when the end of the stream has been reached
			if(datastream.Position == datastream.Length) return null;
			
			// Start reading
			char c = (char)datareader.ReadByte();
			while(!IsWhitespace(c) || quotedstring || IsSpecialToken(c))
			{
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
					// Quote to end the string?
					if(quotedstring && (c == '"')) quotedstring = false;
					
					// First character is a quote?
					if((token.Length == 0) && (c == '"')) quotedstring = true;
					
					token += c;
				}
				
				// Next character
				if(datastream.Position < datastream.Length)
					c = (char)datareader.Read();
				else
					break;
			}
			
			return token;
		}
		
		// This reports an error
		internal void ReportError(string message)
		{
			long position = datastream.Position;
			long readpos = 0;
			int linenumber = 1;
			
			// Find the line on which we found this error
			datastream.Seek(0, SeekOrigin.Begin);
			StreamReader textreader = new StreamReader(datastream, Encoding.ASCII);
			while(readpos < position)
			{
				string line = textreader.ReadLine();
				if(line == null) break;
				readpos += line.Length + 2;
				linenumber++;
			}
			
			// Return to original position
			datastream.Seek(position, SeekOrigin.Begin);
			
			// Set error information
			errordesc = message;
			errorline = linenumber;
			errorsource = sourcename;
		}
		
		#endregion
	}
}
