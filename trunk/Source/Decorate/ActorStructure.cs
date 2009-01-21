
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
	public sealed class ActorStructure
	{
		#region ================== Constants
		
		#endregion
		
		#region ================== Variables
		
		// Declaration
		private string classname;
		private string inheritclass;
		private string replaceclass;
		private int doomednum = -1;
		
		// Flags
		private Dictionary<string, bool> flags;
		
		// Properties
		// We only parse the properties we know about
		// because this format doesn't allow parsing in a generic way
		// (properties can have zero, one or two values and there is
		// nothing that tells you if it is a value or another property)
		private int radius;
		private int height;
		
		// States
		private List<StateStructure> states;
		
		#endregion
		
		#region ================== Properties
		
		public Dictionary<string, bool> Flags { get { return flags; } }
		public string Name { get { return classname; } }
		public int Radius { get { return radius; } }
		public int Height { get { return height; } }
		public int DoomEdNum { get { return doomednum; } }
		
		#endregion
		
		#region ================== Constructor / Disposer
		
		// Constructor
		internal ActorStructure(DecorateParser parser)
		{
			// Initialize
			flags = new Dictionary<string, bool>();
			states = new List<StateStructure>();
			
			// First next token is the class name
			parser.SkipWhitespace(true);
			classname = parser.ReadToken();
			if(string.IsNullOrEmpty(classname))
			{
				parser.ReportError("Expected actor class name");
				return;
			}
			
			// Parse tokens before entering the actor scope
			while(parser.SkipWhitespace(true))
			{
				string token = parser.ReadToken();
				if(!string.IsNullOrEmpty(token))
				{
					token = token.ToLowerInvariant();
					if(token == ":")
					{
						// The next token must be the class to inherit from
						parser.SkipWhitespace(true);
						inheritclass = parser.ReadToken();
						if(string.IsNullOrEmpty(inheritclass) || parser.IsSpecialToken(inheritclass))
						{
							parser.ReportError("Expected class name to inherit from");
							return;
						}
					}
					else if(token == "replaces")
					{
						// The next token must be the class to replace
						parser.SkipWhitespace(true);
						replaceclass = parser.ReadToken();
						if(string.IsNullOrEmpty(replaceclass) || parser.IsSpecialToken(replaceclass))
						{
							parser.ReportError("Expected class name to replace");
							return;
						}
					}
					else if(token == "{")
					{
						// Actor scope begins here,
						// break out of this parse loop
						break;
					}
					else
					{
						// Check if numeric
						if(!int.TryParse(token, out doomednum))
						{
							// Not numeric!
							parser.ReportError("Expected numeric editor thing number or start of actor scope");
							return;
						}
					}
				}
				else
				{
					parser.ReportError("Unexpected end of structure");
					return;
				}
			}
			
			// Now parse the contents of actor structure
			while(parser.SkipWhitespace(true))
			{
				string token = parser.ReadToken();
				token = token.ToLowerInvariant();
				if((token == "+") || (token == "-"))
				{
					// Next token is a flag (option) to set or remove
					bool flagvalue = (token == "+");
					parser.SkipWhitespace(true);
					string flagname = parser.ReadToken();
					if(!string.IsNullOrEmpty(flagname))
					{
						// Add the flag with its value
						flagname = flagname.ToLowerInvariant();
						flags.Add(flagname, flagvalue);
					}
					else
					{
						parser.ReportError("Expected flag name");
						return;
					}
				}
				else if(token == "states")
				{
					// Now parse actor states until we reach the end of the states structure
					while(parser.SkipWhitespace(true))
					{
						string statename = parser.ReadToken();
						if(!string.IsNullOrEmpty(statename))
						{
							// Start of scope?
							if(statename == "{")
							{
								// This is fine
							}
							// End of scope?
							else if(statename == "}")
							{
								// Done with the states,
								// break out of this parse loop
								break;
							}
							else
							{
								// Next token must be a :
								parser.SkipWhitespace(true);
								string labeltoken = parser.ReadToken();
								if(!string.IsNullOrEmpty(statename))
								{
									if(labeltoken == ":")
									{
										// Parse actor state
										StateStructure st = new StateStructure(parser, statename);
										states.Add(st);
										if(parser.HasError) return;
									}
									else
									{
										parser.ReportError("Expected state label colon");
										return;
									}
								}
								else
								{
									parser.ReportError("Unexpected end of structure");
									return;
								}
							}
						}
						else
						{
							parser.ReportError("Unexpected end of structure");
							return;
						}
					}
				}
				else if(token == "}")
				{
					// Actor scope ends here,
					// break out of this parse loop
					break;
				}
				else
				{
					// This must be a property
					
					// Is this a known property?
					if((token == "radius") || (token == "height"))
					{
						// Next token is the property value to set
						parser.SkipWhitespace(true);
						string value = parser.ReadToken();
						if(!string.IsNullOrEmpty(value))
						{
							// Try parsing as integer value
							int intvalue;
							int.TryParse(value, out intvalue);
							
							// Set the property
							if(token == "radius")
								radius = intvalue;
							else if(token == "height")
								height = intvalue;
						}
						else
						{
							// Can't find the property value!
							parser.ReportError("Expected a value for property '" + token + "'");
							return;
						}
					}
				}
			}
		}
		
		#endregion
		
		#region ================== Methods
		
		#endregion
	}
}
