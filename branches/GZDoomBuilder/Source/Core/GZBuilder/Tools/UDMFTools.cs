﻿using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Types;

namespace CodeImp.DoomBuilder.GZBuilder.Tools
{
	public static class UDMFTools
	{
		//float
		public static void SetFloat(UniFields fields, string key, float value, float defaultValue) {
			if(fields == null) return;

			if(value != defaultValue) {
				if(!fields.ContainsKey(key))
					fields.Add(key, new UniValue(UniversalType.Float, value));
				else
					fields[key].Value = value;
			} else if(fields.ContainsKey(key)) { //don't save default value
				fields.Remove(key);
			}
		}

		public static float GetFloat(UniFields fields, string key, float defaultValue) {
			if(fields == null) return defaultValue;
			return fields.GetValue(key, defaultValue);
		}

		//int
		public static void SetInteger(UniFields fields, string key, int value, int defaultValue) {
			if(fields == null) return;

			if(value != defaultValue) {
				if(!fields.ContainsKey(key))
					fields.Add(key, new UniValue(UniversalType.Integer, value));
				else
					fields[key].Value = value;
			} else if(fields.ContainsKey(key)) { //don't save default value
				fields.Remove(key);
			}
		}

		public static int GetInteger(UniFields fields, string key, int defaultValue) {
			if(fields == null) return defaultValue;
			return fields.GetValue(key, defaultValue);
		}
	}
}
