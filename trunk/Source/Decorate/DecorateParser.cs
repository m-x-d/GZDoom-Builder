
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
		private StreamReader datareader;
		
		// Error report
		private int errorline;
		private string errordesc;
		
		#endregion
		
		#region ================== Properties
		
		internal Stream DataStream { get { return datastream; } }
		internal StreamReader DataReader { get { return datareader; } }
		public ICollection<ActorStructure> Actors { get { return actors; } }
		public int ErrorLine { get { return errorline; } }
		public string ErrorDescription { get { return errordesc; } }
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
		public bool Parse(Stream stream)
		{
			datastream = stream;
			datareader = new StreamReader(datastream, Encoding.ASCII);
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
					}
					else
					{
						// Unknown structure!
						ReportError("Unknown declaration type '" + objdeclaration + "'");
						break;
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
			int offset = skipnewline ? 1 : 0;
			int c;
			
			do
			{
				c = datareader.Read();
				if(c == -1) return false;
			}
			while(WHITESPACE.IndexOf(unchecked((char)c), offset) > -1);
			
			// Go one character back so we can read this non-whitespace character again
			datastream.Seek(-1, SeekOrigin.Current);
			return true;
		}
		
		// This reads a token (all sequential non-whitespace characters or a single character)
		// Returns null when the end of the stream has been reached
		internal string ReadToken()
		{
			string token = "";
			int c;
			
			// Return null when the end of the stream has been reached
			if(datareader.EndOfStream) return null;
			
			// Start reading
			c = datareader.Read();
			while((c != -1) && !IsWhitespace(unchecked((char)c)))
			{
				char cc = unchecked((char)c);
				
				// Special token?
				if(IsSpecialToken(cc))
				{
					// Not reading a token yet?
					if(token.Length == 0)
					{
						// This is our whole token
						token += cc;
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
					token += cc;
				}
				
				// Next character
				c = datareader.Read();
			}
			
			return token;
		}
		
		// This reports an error
		internal void ReportError(string message)
		{
			long position = datastream.Position;
			int linenumber = 1;
			
			// Find the line on which we found this error
			datastream.Seek(0, SeekOrigin.Begin);
			while(datastream.Position < position)
			{
				string line = datareader.ReadLine();
				if(line == null) break;
				linenumber++;
			}
			
			// Return to original position
			datastream.Seek(position, SeekOrigin.Begin);
			
			// Set error information
			errordesc = message;
			errorline = linenumber;
		}
		
		#endregion
	}
}
