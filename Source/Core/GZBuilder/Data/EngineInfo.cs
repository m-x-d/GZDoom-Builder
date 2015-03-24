using System;
using System.IO;

namespace CodeImp.DoomBuilder.GZBuilder.Data 
{
	public class EngineInfo 
	{
		public const string DEFAULT_ENGINE_NAME = "Engine with no name";
		
		private string testprogramname;
		public string TestProgramName { get { return testprogramname; } set { testprogramname = value; CheckProgramName(); } }
		public string TestProgram;
		public string TestParameters;
		public bool CustomParameters;
		public int TestSkill;
		public bool TestShortPaths;

		public EngineInfo() 
		{
			testprogramname = DEFAULT_ENGINE_NAME;
		}

		public EngineInfo(EngineInfo other) 
		{
			testprogramname = other.TestProgramName;
			TestProgram = other.TestProgram;
			TestParameters = other.TestParameters;
			CustomParameters = other.CustomParameters;
			TestSkill = other.TestSkill;
			TestShortPaths = other.TestShortPaths;
		}

		private void CheckProgramName() 
		{
			if(testprogramname == DEFAULT_ENGINE_NAME && !String.IsNullOrEmpty(TestProgram)) 
			{
				//get engine name from path
				testprogramname = Path.GetFileNameWithoutExtension(TestProgram);
			}
		}
	}
}
