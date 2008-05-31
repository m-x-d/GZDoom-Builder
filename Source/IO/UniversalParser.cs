
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
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections;
using System.Collections.Specialized;

#endregion

namespace CodeImp.DoomBuilder.IO
{
	public sealed class UniversalParser
	{
		#region ================== Constants
		
		// Path seperator
		public const string DEFAULT_SEPERATOR = ".";
		
		// Parse mode constants
		private const int PM_NOTHING = 0;
		private const int PM_ASSIGNMENT = 1;
		private const int PM_NUMBER = 2;
		private const int PM_STRING = 3;
		private const int PM_KEYWORD = 4;
		
		// Error strings
		private const string ERROR_KEYMISSING = "Missing key name in assignment or scope.";
		private const string ERROR_KEYSPACES = "Spaces not allowed in key names.";
		private const string ERROR_ASSIGNINVALID = "Invalid assignment. Missing a previous terminator symbol?";
		private const string ERROR_VALUEINVALID = "Invalid value in assignment. Missing a previous terminator symbol?";
		private const string ERROR_VALUETOOBIG = "Value too big.";
		private const string ERROR_KEYNOTUNQIUE = "Key is not unique within scope.";
		private const string ERROR_KEYWORDUNKNOWN = "Unknown keyword in assignment. Missing a previous terminator symbol?";
		
		#endregion
		
		#region ================== Variables
		
		// Error result
		private int cpErrorResult = 0;
		private string cpErrorDescription = "";
		private int cpErrorLine = 0;
		
		// Configuration root
		private ListDictionary root = null;
		
		#endregion
		
		#region ================== Properties
		
		// Properties
		public int ErrorResult { get { return cpErrorResult; } }
		public string ErrorDescription { get { return cpErrorDescription; } }
		public int ErrorLine { get { return cpErrorLine; } }
		public ListDictionary Root { get { return root; } set { root = value; } }
		
		#endregion
		
		#region ================== Constructor / Destructor
		
		// Constructor
		public UniversalParser()
		{
			// Standard new configuration
			NewConfiguration();
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}
		
		// Constructor to load a file immediately
		public UniversalParser(string filename)
		{
			// Load configuration from file
			LoadConfiguration(filename);

			// We have no destructor
			GC.SuppressFinalize(this);
		}
		
		#endregion
		
		#region ================== Private Methods

		// This is called by all the ReadSetting overloads to perform the read
		private bool CheckSetting(string setting, string pathseperator)
		{
			ListDictionary cs = null;

			// Split the path in an array
			string[] keys = setting.Split(pathseperator.ToCharArray());

			// Get the root item
			object item = root;

			// Go for each item
			for(int i = 0; i < keys.Length; i++)
			{
				// Check if the current item is of ConfigStruct type
				if(item is ListDictionary)
				{
					// Check if the key is valid
					if(ValidateKey(null, keys[i].Trim(), -1) == true)
					{
						// Cast to ConfigStruct
						cs = (ListDictionary)item;

						// Check if the requested item exists
						if(cs.Contains(keys[i]) == true)
						{
							// Set the item to the next item
							item = cs[keys[i]];
						}
						else
						{
							// Key not found
							return false;
						}
					}
					else
					{
						// Invalid key in path
						return false;
					}
				}
				else
				{
					// Unable to go any further
					return false;
				}
			}

			// Return result
			return true;
		}
		
		// This is called by all the ReadSetting overloads to perform the read
		private object ReadAnySetting(string setting, object defaultsetting, string pathseperator)
		{
			ListDictionary cs = null;
			
			// Split the path in an array
			string[] keys = setting.Split(pathseperator.ToCharArray());
			
			// Get the root item
			object item = root;
			
			// Go for each item
			for(int i = 0; i < keys.Length; i++)
			{
				// Check if the current item is of ConfigStruct type
				if(item is ListDictionary)
				{
					// Check if the key is valid
					if(ValidateKey(null, keys[i].Trim(), -1) == true)
					{
						// Cast to ConfigStruct
						cs = (ListDictionary)item;
						
						// Check if the requested item exists
						if(cs.Contains(keys[i]) == true)
						{
							// Set the item to the next item
							item = cs[keys[i]];
						}
						else
						{
							// Key not found
							// return default setting
							return defaultsetting;
						}
					}
					else
					{
						// Invalid key in path
						// return default setting
						return defaultsetting;
					}
				}
				else
				{
					// Unable to go any further
					// return default setting
					return defaultsetting;
				}
			}
			
			// Return the item
			return item;
		}
		
		
		// This returns a string added with escape characters
		private string EscapedString(string str)
		{
			// Replace the \ with \\ first!
			str = str.Replace("\\", "\\\\");
			str = str.Replace("\n", "\\n");
			str = str.Replace("\r", "\\r");
			str = str.Replace("\t", "\\t");
			str = str.Replace("\"", "\\\"");
			
			// Return result
			return str;
		}
		
		
		// This raises an error
		private void RaiseError(int line, string description)
		{
			// Raise error
			cpErrorResult = 1;
			cpErrorDescription = description;
			cpErrorLine = line;
		}
		
		
		// This validates a given key and sets
		// error properties if key is invalid and errorline > -1
		private bool ValidateKey(ListDictionary container, string key, int errorline)
		{
			bool validateresult;
			
			// Check if key is an empty string
			if(key == "")
			{
				// ERROR: Missing key name in statement
				if(errorline > -1) RaiseError(errorline, ERROR_KEYMISSING);
				validateresult = false;
			}
			else
			{
				// Check if there are spaces in the key
				if(key.IndexOfAny(" ".ToCharArray()) > -1)
				{
					// ERROR: Spaces not allowed in key names
					if(errorline > -1) RaiseError(errorline, ERROR_KEYSPACES);
					validateresult = false;
				}
				else
				{
					// Check if we can test existance
					if(container != null)
					{
						// Test if the key exists in this container
						if(container.Contains(key) == true)
						{
							// ERROR: Key is not unique within struct
							if(errorline > -1) RaiseError(errorline, ERROR_KEYNOTUNQIUE);
							validateresult = false;
						}
						else
						{
							// Key OK
							validateresult = true;
						}
					}
					else
					{
						// Key OK
						validateresult = true;
					}
				}
			}
			
			// Return result
			return validateresult;
		}
		
		
		// This validates a given keyword and sets
		// error properties if keyword is invalid and errorline > -1
		private bool ValidateKeyword(string keyword, int errorline)
		{
			bool validateresult;
			
			// Check if key is an empty string
			if(keyword == "")
			{
				// ERROR: Missing key name in statement
				if(errorline > -1) RaiseError(errorline, ERROR_ASSIGNINVALID);
				validateresult = false;
			}
			else
			{
				// Check if there are spaces in the key
				if(keyword.IndexOfAny(" ".ToCharArray()) > -1)
				{
					// ERROR: Spaces not allowed in key names
					if(errorline > -1) RaiseError(errorline, ERROR_ASSIGNINVALID);
					validateresult = false;
				}
				else
				{
					// Key OK
					validateresult = true;
				}
			}
			
			// Return result
			return validateresult;
		}
		
		
		// This parses a structure in the given data starting
		// from the given pos and line and updates pos and line.
		private ListDictionary InputStructure(ref string data, ref int pos, ref int line)
		{
			char c = '\0';					// current data character
			int pm = PM_NOTHING;			// current parse mode
			string key = "", val = "";		// current key and value beign built
			bool escape = false;			// escape sequence?
			bool endofstruct = false;		// true as soon as this level struct ends
			ListDictionary cs = new ListDictionary();
			
			// Go through all of the data until
			// the end or until the struct closes
			// or when an arror occurred
			while ((pos < data.Length) && (cpErrorResult == 0) && (endofstruct == false))
			{
				// Get current character
				c = data[pos];
				
				// ================ What parse mode are we at?
				if(pm == PM_NOTHING)
				{
					// Now check what character this is
					switch(c)
					{
						case '{': // Begin of new struct
							
							// Validate key
							if(ValidateKey(cs, key.Trim(), line))
							{
								// Next character
								pos++;
								
								// Parse this struct and add it
								cs.Add(key.Trim(), InputStructure(ref data, ref pos, ref line));
								
								// Check the last character
								pos--;
								
								// Reset the key
								key = "";
							}
							
							// Leave switch
							break;
							
						case '}': // End of this struct
							
							// Stop parsing in this struct
							endofstruct = true;
							
							// Leave the loop
							break;
							
						case '=': // Assignment
							
							// Validate key
							if(ValidateKey(cs, key.Trim(), line))
							{
								// Now parsing assignment
								pm = PM_ASSIGNMENT;
							}
							
							// Leave switch
							break;
							
						case ';': // Terminator
							
							// Validate key
							if(ValidateKey(cs, key.Trim(), line))
							{
								// Add the key with null as value
								cs.Add(key.Trim(), null);
							
								// Reset key and value
								key = "";
								val = "";
							}
							
							// Leave switch
							break;
							
						case '\n': // New line
							
							// Count the line
							line++;
							
							// Add this to the key as a space.
							// Spaces are not allowed, but it will be trimmed
							// when its the first or last character.
							key += " ";
							
							// Leave switch
							break;
							
						case '\\': // Possible comment
						case '/':
							
							// Check for the line comment //
							if(data.Substring(pos, 2) == "//")
							{
								// Find the next line
								int np = data.IndexOf("\n", pos);
								
								// Next line found?
								if(np > -1)
								{
									// Count the line
									line++;
									
									// Skip everything on this line
									pos = np;
								}
								else
								{
									// No end of line
									// Skip everything else
									pos = data.Length;
								}
							}
								// Check for the block comment /* */
							else if(data.Substring(pos, 2) == "/*")
							{
								// Find the next closing block comment
								int np = data.IndexOf("*/", pos);
								
								// Closing block comment found?
								if(np > -1)
								{
									// Count the lines in the block comment
									string blockdata = data.Substring(pos, np - pos + 2);
									line += (blockdata.Split("\n".ToCharArray()).Length - 1);
									
									// Skip everything in this block
									pos = np + 1;
								}
								else
								{
									// No end of line
									// Skip everything else
									pos = data.Length;
								}
							}
							
							// Leave switch
							break;
							
						default: // Everything else
							
							// Add character to key
							key += c.ToString(CultureInfo.InvariantCulture);
							
							// Leave switch
							break;
					}
				}
				// ================ Parsing an assignment
				else if(pm == PM_ASSIGNMENT)
				{
					// Check for string opening
					if(c == '\"')
					{
						// Now parsing string
						pm = PM_STRING;
					}
					// Check for numeric character
					else if("0123456789-.&".IndexOf(c.ToString(CultureInfo.InvariantCulture)) > -1)
					{
						// Now parsing number
						pm = PM_NUMBER;
						
						// Go one byte back, because this
						// byte is part of the number!
						pos--;
					}
					// Check for new line
					else if(c == '\n')
					{
						// Count the new line
						line++;
					}
					// Check if assignment ends
					else if(c == ';')
					{
						// End of assignment
						pm = PM_NOTHING;
						
						// Remove this if it causes problems
						key = "";
						val = "";
					}
					// Otherwise (if not whitespace) it will be a keyword
					else if((c != ' ') && (c != '\t'))
					{
						// Now parsing a keyword
						pm = PM_KEYWORD;
						
						// Go one byte back, because this
						// byte is part of the keyword!
						pos--;
					}
				}
				// ================ Parsing a number
				else if(pm == PM_NUMBER)
				{
					// Check if number ends here
					if(c == ';')
					{
						// Floating point?
						if(val.IndexOf(".") > -1)
						{
							float fval = 0;
							
							// Convert to float (remove the f first)
							try { fval = System.Convert.ToSingle(val.Trim(), CultureInfo.InvariantCulture); }
							catch(System.FormatException)
							{ 
								// ERROR: Invalid value in assignment
								RaiseError(line, ERROR_VALUEINVALID);
							}
							
							// Add it to struct
							cs.Add(key.Trim(), fval);
						}
						else
						{
							int ival = 0;
							long lval = 0;
							
							// Convert to int
							try
							{
								// Convert to value
								ival = System.Convert.ToInt32(val.Trim(), CultureInfo.InvariantCulture);
								
								// Add it to struct
								cs.Add(key.Trim(), ival);
							}
							catch(System.OverflowException)
							{
								// Too large for Int32, try Int64
								try
								{
									// Convert to value
									lval = System.Convert.ToInt64(val.Trim(), CultureInfo.InvariantCulture);
									
									// Add it to struct
									cs.Add(key.Trim(), lval);
								}
								catch(System.OverflowException)
								{
									// Too large for Int64, return error
									RaiseError(line, ERROR_VALUETOOBIG);
								}
								catch(System.FormatException)
								{ 
									// ERROR: Invalid value in assignment
									RaiseError(line, ERROR_VALUEINVALID);
								}
							}
							catch(System.FormatException)
							{ 
								// ERROR: Invalid value in assignment
								RaiseError(line, ERROR_VALUEINVALID);
							}
						}
						
						// Reset key and value
						key = "";
						val = "";
						
						// End of assignment
						pm = PM_NOTHING;
					}
					// Check for new line
					else if(c == '\n')
					{
						// Count the new line
						line++;
					}
					// Everything else is part of the value
					else
					{
						val += c.ToString(CultureInfo.InvariantCulture);
					}
				}
				// ================ Parsing a string
				else if(pm == PM_STRING)
				{
					// Check if in an escape sequence
					if(escape)
					{
						// What character?
						switch(c)
						{
							case '\\': val += "\\"; break;
							case 'n': val += "\n"; break;
							case '\"': val += "\""; break;
							case 'r': val += "\r"; break;
							case 't': val += "\t"; break;
							default:
								
								// Is it a number?
								if("0123456789".IndexOf(c.ToString(CultureInfo.InvariantCulture)) > -1)
								{
									int vv = 0;
									char vc = '0';
									
									// Convert the next 3 characters to a number
									string v = data.Substring(pos, 3);
									try { vv = System.Convert.ToInt32(v.Trim(), CultureInfo.InvariantCulture); }
									catch(System.FormatException)
									{ 
										// ERROR: Invalid value in assignment
										RaiseError(line, ERROR_VALUEINVALID);
									}
									
									// Convert the number to a char
									try { vc = System.Convert.ToChar(vv, CultureInfo.InvariantCulture); }
									catch(System.FormatException)
									{ 
										// ERROR: Invalid value in assignment
										RaiseError(line, ERROR_VALUEINVALID);
									}
									
									// Add the char
									val += vc.ToString(CultureInfo.InvariantCulture);
								}
								else
								{
									// Add the character as it is
									val += c.ToString(CultureInfo.InvariantCulture);
								}
								
								// Leave switch
								break;
						}
						
						// End of escape sequence
						escape = false;
					}
					else
					{
						// Check for sequence start
						if(c == '\\')
						{
							// Next character is of escape sequence
							escape = true;
						}
						// Check if string ends
						else if(c == '\"')
						{
							// Add string to struct
							cs.Add(key.Trim(), val);
							
							// End of assignment
							pm = PM_ASSIGNMENT;
							
							// Reset key and value
							key = "";
							val = "";
						}
						// Check for new line
						else if(c == '\n')
						{
							// Count the new line
							line++;
						}
						// Everything else is just part of string
						else
						{
							// Add to value
							val += c.ToString(CultureInfo.InvariantCulture);
						}
					}
				}
				// ================ Parsing a keyword
				else if(pm == PM_KEYWORD)
				{
					// Check if keyword ends
					if(c == ';')
					{
						// Validate the keyword
						if(ValidateKeyword(val.Trim(), line))
						{
							// Add to the struct depending on the keyword
							switch(val.Trim().ToLowerInvariant())
							{
								case "true":
									
									// Add boolean true
									cs.Add(key.Trim(), true);
									break;
									
								case "false":
									
									// Add boolean false
									cs.Add(key.Trim(), false);
									break;
									
								default:
									
									// Unknown keyword
									RaiseError(line, ERROR_KEYWORDUNKNOWN);
									break;
							}
							
							// End of assignment
							pm = PM_NOTHING;
							
							// Reset key and value
							key = "";
							val = "";
						}
					}
					// Check for new line
					else if(c == '\n')
					{
						// Count the new line
						line++;
					}
					// Everything else is just part of keyword
					else
					{
						// Add to value
						val += c.ToString(CultureInfo.InvariantCulture);
					}
				}
				
				// Next character
				pos++;
			}
			
			// Return the parsed result
			return cs;
		}
		
		
		// This will create a data structure from the given object
		private string OutputStructure(ListDictionary cs, int level, string newline, bool whitespace)
		{
			string leveltabs = "";
			string spacing = "";
			StringBuilder db = new StringBuilder("");
			
			// Check if this ConfigStruct is not empty
			if(cs.Count > 0)
			{
				// Create whitespace
				if(whitespace)
				{
					for(int i = 0; i < level; i++) leveltabs += "\t";
					spacing = " ";
				}
				
				// Get enumerator
				IDictionaryEnumerator de = cs.GetEnumerator();
				
				// Go for each item
				for(int i = 0; i < cs.Count; i++)
				{
					// Go to next item
					de.MoveNext();
					
					// Check if the value if of ConfigStruct type
					if(de.Value is ListDictionary)
					{
						// Output recursive structure
						if(whitespace) { db.Append(leveltabs); db.Append(newline); }
						db.Append(leveltabs); db.Append(de.Key); db.Append(newline);
						db.Append(leveltabs); db.Append("{"); db.Append(newline);
						db.Append(OutputStructure((ListDictionary)de.Value, level + 1, newline, whitespace));
						db.Append(leveltabs); db.Append("}"); db.Append(newline);
						if(whitespace) { db.Append(leveltabs); db.Append(newline); }
					}
					// Check if the value is of boolean type
					else if(de.Value is bool)
					{
						// Check value
						if((bool)de.Value == true)
						{
							// Output the keyword "true"
							db.Append(leveltabs); db.Append(de.Key.ToString()); db.Append(spacing);
							db.Append("="); db.Append(spacing); db.Append("true;"); db.Append(newline);
						}
						else
						{
							// Output the keyword "false"
							db.Append(leveltabs); db.Append(de.Key.ToString()); db.Append(spacing);
							db.Append("="); db.Append(spacing); db.Append("false;"); db.Append(newline);
						}
					}
					// Check if value is of float type
					else if(de.Value is float)
					{
						// Output the value with a postfixed f
						db.Append(leveltabs); db.Append(de.Key); db.Append(spacing); db.Append("=");
						db.Append(spacing); db.Append(String.Format(CultureInfo.InvariantCulture, "{0}", de.Value)); db.Append("f;"); db.Append(newline);
					}
					// Check if value is of other numeric type
					else if(de.Value.GetType().IsPrimitive)
					{
						// Output the value unquoted
						db.Append(leveltabs); db.Append(de.Key); db.Append(spacing); db.Append("=");
						db.Append(spacing); db.Append(String.Format(CultureInfo.InvariantCulture, "{0}", de.Value)); db.Append(";"); db.Append(newline);
					}
					else
					{
						// Output the value with quotes and escape characters
						db.Append(leveltabs); db.Append(de.Key); db.Append(spacing); db.Append("=");
						db.Append(spacing); db.Append("\""); db.Append(EscapedString(de.Value.ToString())); db.Append("\";"); db.Append(newline);
					}
				}
			}
			
			// Return the structure
			return db.ToString();
		}
		
		#endregion
		
		#region ================== Public Methods
		
		// This clears the last error
		public void ClearError()
		{
			// Clear error
			cpErrorResult = 0;
			cpErrorDescription = "";
			cpErrorLine = 0;
		}
		
		
		// This creates a new configuration
		public void NewConfiguration()
		{
			// Create new configuration
			root = new ListDictionary();
		}
		
		// This checks if a given setting exists (disregards type)
		public bool SettingExists(string setting) { return CheckSetting(setting, DEFAULT_SEPERATOR); }
		public bool SettingExists(string setting, string pathseperator) { return CheckSetting(setting, pathseperator); }
		
		// This can give a value of a key specified in a path form
		// also, this does not error when the setting does not exist,
		// but instead returns the given default value.
		public string ReadSetting(string setting, string defaultsetting) { object r = ReadAnySetting(setting, defaultsetting, DEFAULT_SEPERATOR); if(r != null) return r.ToString(); else return null; }
		public string ReadSetting(string setting, string defaultsetting, string pathseperator) { object r = ReadAnySetting(setting, defaultsetting, pathseperator); if(r != null) return r.ToString(); else return null; }
		public int ReadSetting(string setting, int defaultsetting) { return Convert.ToInt32(ReadAnySetting(setting, defaultsetting, DEFAULT_SEPERATOR), CultureInfo.InvariantCulture); }
		public int ReadSetting(string setting, int defaultsetting, string pathseperator) { return Convert.ToInt32(ReadAnySetting(setting, defaultsetting, pathseperator), CultureInfo.InvariantCulture); }
		public float ReadSetting(string setting, float defaultsetting) { return Convert.ToSingle(ReadAnySetting(setting, defaultsetting, DEFAULT_SEPERATOR), CultureInfo.InvariantCulture); }
		public float ReadSetting(string setting, float defaultsetting, string pathseperator) { return Convert.ToSingle(ReadAnySetting(setting, defaultsetting, pathseperator), CultureInfo.InvariantCulture); }
		public short ReadSetting(string setting, short defaultsetting) { return Convert.ToInt16(ReadAnySetting(setting, defaultsetting, DEFAULT_SEPERATOR), CultureInfo.InvariantCulture); }
		public short ReadSetting(string setting, short defaultsetting, string pathseperator) { return Convert.ToInt16(ReadAnySetting(setting, defaultsetting, pathseperator), CultureInfo.InvariantCulture); }
		public long ReadSetting(string setting, long defaultsetting) { return Convert.ToInt64(ReadAnySetting(setting, defaultsetting, DEFAULT_SEPERATOR), CultureInfo.InvariantCulture); }
		public long ReadSetting(string setting, long defaultsetting, string pathseperator) { return Convert.ToInt64(ReadAnySetting(setting, defaultsetting, pathseperator), CultureInfo.InvariantCulture); }
		public bool ReadSetting(string setting, bool defaultsetting) { return Convert.ToBoolean(ReadAnySetting(setting, defaultsetting, DEFAULT_SEPERATOR), CultureInfo.InvariantCulture); }
		public bool ReadSetting(string setting, bool defaultsetting, string pathseperator) { return Convert.ToBoolean(ReadAnySetting(setting, defaultsetting, pathseperator), CultureInfo.InvariantCulture); }
		public byte ReadSetting(string setting, byte defaultsetting) { return Convert.ToByte(ReadAnySetting(setting, defaultsetting, DEFAULT_SEPERATOR), CultureInfo.InvariantCulture); }
		public byte ReadSetting(string setting, byte defaultsetting, string pathseperator) { return Convert.ToByte(ReadAnySetting(setting, defaultsetting, pathseperator), CultureInfo.InvariantCulture); }
		public ListDictionary ReadSetting(string setting, ListDictionary defaultsetting) { return (ListDictionary)ReadAnySetting(setting, defaultsetting, DEFAULT_SEPERATOR); }
		public ListDictionary ReadSetting(string setting, ListDictionary defaultsetting, string pathseperator) { return (ListDictionary)ReadAnySetting(setting, defaultsetting, pathseperator); }

		public object ReadSettingObject(string setting, object defaultsetting) { return ReadAnySetting(setting, defaultsetting, DEFAULT_SEPERATOR); }
		
		
		// This writes a given setting to the configuration.
		// Wont change existing structs, but will add them as needed.
		// Returns true when written, false when failed.
		public bool WriteSetting(string setting, object settingvalue) { return WriteSetting(setting, settingvalue, DEFAULT_SEPERATOR); }
		public bool WriteSetting(string setting, object settingvalue, string pathseperator)
		{
			ListDictionary cs = null;
			
			// Split the path in an array
			string[] keys = setting.Split(pathseperator.ToCharArray());
			string finalkey = keys[keys.Length - 1];
			
			// Get the root item
			object item = root;
			
			// Go for each path item
			for(int i = 0; i < (keys.Length - 1); i++)
			{
				// Check if the key is valid
				if(ValidateKey(null, keys[i].Trim(), -1) == true)
				{
					// Cast to ConfigStruct
					cs = (ListDictionary)item;
					
					// Check if the requested item exists
					if(cs.Contains(keys[i]) == true)
					{
						// Check if the requested item is a ConfigStruct
						if(cs[keys[i]] is ListDictionary)
						{
							// Set the item to the next item
							item = cs[keys[i]];
						}
						else
						{
							// Cant proceed with path
							return false;
						}
					}
					else
					{
						// Key not found
						// Create it now
						ListDictionary ncs = new ListDictionary();
						cs.Add(keys[i], ncs);
						
						// Set the item to the next item
						item = cs[keys[i]];
					}
				}
				else
				{
					// Invalid key in path
					return false;
				}
			}
			
			// Cast to ConfigStruct
			cs = (ListDictionary)item;
			
			// Check if the key already exists
			if(cs.Contains(finalkey) == true)
			{
				// Update the value
				cs[finalkey] = settingvalue;
			}
			else
			{
				// Create the key/value pair
				cs.Add(finalkey, settingvalue);
			}
			
			// Return success
			return true;
		}
		
		
		// This removes a given setting from the configuration.
		public bool DeleteSetting(string setting) { return DeleteSetting(setting, DEFAULT_SEPERATOR); }
		public bool DeleteSetting(string setting, string pathseperator)
		{
			ListDictionary cs = null;
			
			// Split the path in an array
			string[] keys = setting.Split(pathseperator.ToCharArray());
			string finalkey = keys[keys.Length - 1];
			
			// Get the root item
			object item = root;
			
			// Go for each path item
			for(int i = 0; i < (keys.Length - 1); i++)
			{
				// Check if the key is valid
				if(ValidateKey(null, keys[i].Trim(), -1) == true)
				{
					// Cast to ConfigStruct
					cs = (ListDictionary)item;
					
					// Check if the requested item exists
					if(cs.Contains(keys[i]) == true)
					{
						// Check if the requested item is a ConfigStruct
						if(cs[keys[i]] is ListDictionary)
						{
							// Set the item to the next item
							item = cs[keys[i]];
						}
						else
						{
							// Cant proceed with path
							return false;
						}
					}
					else
					{
						// Key not found
						// Create it now
						ListDictionary ncs = new ListDictionary();
						cs.Add(keys[i], ncs);
						
						// Set the item to the next item
						item = cs[keys[i]];
					}
				}
				else
				{
					// Invalid key in path
					return false;
				}
			}
			
			// Cast to ConfigStruct
			cs = (ListDictionary)item;
			
			// Arrived at our destination
			// Delete the key if the key exists
			if(cs.Contains(finalkey) == true)
			{
				// Key exists, delete it
				cs.Remove(finalkey);
				
				// Return success
				return true;
			}
			else
			{
				// Key not found, return fail
				return false;
			}
		}
		
		
		// This will save the current configuration to the specified file
		public bool SaveConfiguration(string filename) { return SaveConfiguration(filename, "\r\n", true); }
		public bool SaveConfiguration(string filename, string newline) { return SaveConfiguration(filename, newline, true); }
		public bool SaveConfiguration(string filename, string newline, bool whitespace)
		{
			// Kill the file if it exists
			if(File.Exists(filename) == true) File.Delete(filename);
			
			// Open file stream for writing
			FileStream fstream = File.OpenWrite(filename);
			
			// Create output structure and write to file
			string data = OutputConfiguration(newline, whitespace);
			byte[] baData= Encoding.ASCII.GetBytes(data);
			fstream.Write(baData, 0, baData.Length);
			fstream.Flush();
			fstream.Close();
			
			// Return true when done, false when errors occurred
			if(cpErrorResult == 0) return true; else return false;
		}
		
		
		// This will output the current configuration as a string
		public string OutputConfiguration() { return OutputConfiguration("\r\n", true); }
		public string OutputConfiguration(string newline) { return OutputConfiguration(newline, true); }
		public string OutputConfiguration(string newline, bool whitespace)
		{
			// Simply return the configuration structure as string
			return OutputStructure(root, 0, newline, whitespace);
		}
		
		
		// This will load a configuration from file
		public bool LoadConfiguration(string filename)
		{
			// Check if the file is missing
			if(File.Exists(filename) == false)
			{
				throw(new FileNotFoundException("File not found \"" + filename + "\"", filename));
			}
			else
			{
				// Load the file contents
				FileStream fstream = File.OpenRead(filename);
				byte[] fbuffer = new byte[fstream.Length];
				fstream.Read(fbuffer, 0, fbuffer.Length);
				fstream.Close();
				
				// Convert byte array to string
				string data = Encoding.ASCII.GetString(fbuffer);
				
				// Load the configuration from this data
				return InputConfiguration(data);
			}
		}
		
		
		// This will load a configuration from string
		public bool InputConfiguration(string data)
		{
			// Remove returns and tabs because the
			// parser only uses newline for new lines.
			data = data.Replace("\r", "");
			data = data.Replace("\t", "");
			
			// Clear errors
			ClearError();
			
			// Parse the data to the root structure
			int pos = 0;
			int line = 1;
			root = InputStructure(ref data, ref pos, ref line);
			
			// Return true when done, false when errors occurred
			if(cpErrorResult == 0) return true; else return false;
		}
		
		#endregion
	}
}
