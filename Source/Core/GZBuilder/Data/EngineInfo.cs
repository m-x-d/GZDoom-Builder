using System;
using System.Drawing;
using System.IO;

namespace CodeImp.DoomBuilder.GZBuilder.Data 
{
	public class EngineInfo : IDisposable
	{
		// Disposing
		private bool isdisposed;
		
		public const string DEFAULT_ENGINE_NAME = "Engine with no name";
		
		private string testprogramname;
		public string TestProgramName { get { return testprogramname; } set { testprogramname = value; CheckProgramName(); } }
		public string TestProgram;
		public string TestParameters;
		public bool CustomParameters;
		public int TestSkill;
		public bool TestShortPaths;
		private Bitmap icon;
		public Bitmap TestProgramIcon { get { return icon; } }

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
			icon = other.icon;
		}

		private void CheckProgramName() 
		{
			if(testprogramname == DEFAULT_ENGINE_NAME && !String.IsNullOrEmpty(TestProgram)) 
			{
				//get engine name from path
				testprogramname = Path.GetFileNameWithoutExtension(TestProgram);
			}

			// Update icon
			if(icon != null)
			{
				icon.Dispose();
				icon = null;
			}

			if(File.Exists(TestProgram))
			{
				Icon i = Icon.ExtractAssociatedIcon(TestProgram);
				if(i != null) icon = i.ToBitmap();
			}
			
			if(icon == null)
			{
				icon = new Bitmap(16, 16);
			}
		}

		public void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				icon.Dispose();

				// Done
				isdisposed = true;
			}
		}
	}
}
