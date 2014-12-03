using System;
using System.IO;

namespace CodeImp.DoomBuilder.GZBuilder.Data 
{
	public class EngineInfo 
	{
		public const string DEFAULT_ENGINE_NAME = "Engine with no name";
		
		public string TestProgramName;
		public string TestProgram;
		public string TestParameters;
		public bool CustomParameters;
		public int TestSkill;
		public bool TestShortPaths;

		public EngineInfo() 
		{
			TestProgramName = DEFAULT_ENGINE_NAME;
		}

		public EngineInfo(EngineInfo other) 
		{
			TestProgramName = other.TestProgramName;
			TestProgram = other.TestProgram;
			TestParameters = other.TestParameters;
			CustomParameters = other.CustomParameters;
			TestSkill = other.TestSkill;
			TestShortPaths = other.TestShortPaths;
		}

		public void CheckProgramName(bool forced) 
		{
			if ((forced || TestProgramName == DEFAULT_ENGINE_NAME) && !String.IsNullOrEmpty(TestProgram)) 
			{
				//get engine name from folder name
				TestProgramName = Path.GetFileNameWithoutExtension(TestProgram);
			}
		}
	}
}
