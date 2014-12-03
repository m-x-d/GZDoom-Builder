using System;
using System.Collections.Generic;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Types;

namespace CodeImp.DoomBuilder.GZBuilder.Tools
{
	public static class UDMFTools
	{
		//float
		public static void SetFloat(UniFields fields, string key, float value) 
		{
			SetFloat(fields, key, value, 0f);
		}

		public static void SetFloat(UniFields fields, string key, float value, float defaultValue) 
		{
			if(fields == null) return;

			if(value != defaultValue) 
			{
				if(!fields.ContainsKey(key))
					fields.Add(key, new UniValue(UniversalType.Float, value));
				else
					fields[key].Value = value;
			} 
			else if(fields.ContainsKey(key)) //don't save default value
			{ 
				fields.Remove(key);
			}
		}

		public static float GetFloat(UniFields fields, string key) 
		{
			return GetFloat(fields, key, 0f);
		}

		public static float GetFloat(UniFields fields, string key, float defaultValue) 
		{
			if(fields == null) return defaultValue;
			return fields.GetValue(key, defaultValue);
		}

		//int
		public static void SetInteger(UniFields fields, string key, int value) 
		{
			SetInteger(fields, key, value, 0);
		}

		public static void SetInteger(UniFields fields, string key, int value, int defaultValue) 
		{
			if(fields == null) return;

			if(value != defaultValue) 
			{
				if(!fields.ContainsKey(key))
					fields.Add(key, new UniValue(UniversalType.Integer, value));
				else
					fields[key].Value = value;
			} 
			else if(fields.ContainsKey(key)) //don't save default value
			{ 
				fields.Remove(key);
			}
		}

		public static int GetInteger(UniFields fields, string key) 
		{
			return GetInteger(fields, key, 0);
		}

		public static int GetInteger(UniFields fields, string key, int defaultValue) 
		{
			if(fields == null) return defaultValue;
			return fields.GetValue(key, defaultValue);
		}

		public static void SetString(UniFields fields, string key, string value, string defaultValue) 
		{
			if(fields == null) return;

			if(value != defaultValue) 
			{
				if(!fields.ContainsKey(key))
					fields.Add(key, new UniValue(UniversalType.String, value));
				else
					fields[key].Value = value;
			} 
			else if(fields.ContainsKey(key)) //don't save default value
			{
				fields.Remove(key);
			}
		}

		public static void ClearFields(UniFields fields, string[] keys) 
		{
			if(fields == null) return;

			foreach(string key in keys)
			{
				if(fields.ContainsKey(key)) fields.Remove(key);
			}
		}

		public static void ClearField(UniFields fields, string key) 
		{
			if(fields == null || !fields.ContainsKey(key)) return;
			fields.Remove(key);
		}

		public static bool FieldsMatch(UniFields fields1, UniFields fields2) 
		{
			if (fields1.Keys.Count != fields2.Keys.Count) return false;
			foreach(KeyValuePair<string, UniValue> group in fields1) 
			{
				if (!fields2.ContainsKey(group.Key)) return false;
				if (fields2[group.Key].Type != fields1[group.Key].Type) return false;
				
				if (fields1[group.Key].Value is int) 
				{
					if ((int)fields1[group.Key].Value != (int)fields2[group.Key].Value) return false;
				}
				else if (fields1[group.Key].Value is float) 
				{
					if((float)fields1[group.Key].Value != (float)fields2[group.Key].Value) return false;
				}
				else if(fields1[group.Key].Value is bool) 
				{
					if((bool)fields1[group.Key].Value != (bool)fields2[group.Key].Value) return false;
				}
				else if (fields1[group.Key].Value is string) 
				{
					if ((string)fields1[group.Key].Value != (string)fields2[group.Key].Value) return false;
				} 
				else 
				{
					throw new Exception("Got unknown Custom Field type to compare: " + fields1[group.Key].Value.GetType());
				}
			}

			return true;
		}
	}
}
