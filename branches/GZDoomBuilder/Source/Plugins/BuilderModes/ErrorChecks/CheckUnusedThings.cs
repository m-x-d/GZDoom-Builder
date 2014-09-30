#region ================== Namespaces

using System.Collections.Generic;
using System.Threading;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Map;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	[ErrorChecker("Check unused things", true, 50)]
	public class CheckUnusedThings : ErrorChecker
	{
		#region ================== Constants

		private const int PROGRESS_STEP = 10;

		#endregion

		#region ================== Constructor / Destructor

		public CheckUnusedThings()	
		{
			// Total progress is done when all things are checked
			SetTotalProgress(General.Map.Map.Things.Count / PROGRESS_STEP);
		}

		#endregion

		#region ================== Methods

		// This runs the check
		public override void Run() 
		{
			int progress = 0;
			int stepprogress = 0;
			bool udmf = General.Map.UDMF;

			// Go for all things
			foreach(Thing t in General.Map.Map.Things) 
			{
				List<string> messages = new List<string>();
				foreach(KeyValuePair<string, Dictionary<string, ThingFlagsCompare>> group in General.Map.Config.ThingFlagsCompare) 
				{
					if(group.Value.Count < 2) continue;
					bool haveflags = false;

					foreach(KeyValuePair<string, ThingFlagsCompare> flags in group.Value) 
					{
						if((udmf && t.Fields.ContainsKey(flags.Key) && (bool)t.Fields[flags.Key].Value) 
							|| t.IsFlagSet(flags.Key)) 
						{
							haveflags = true;
							break;
						}
					}

					if(!haveflags) messages.Add(GetDescription(group.Key));
				}

				if(messages.Count > 0)
				{
					string msg = " is not used " + string.Join(", ", messages.ToArray());
					SubmitResult(new ResultUnusedThing(t, msg));
				}

				// Handle thread interruption
				try { Thread.Sleep(0); } catch(ThreadInterruptedException) { return; }

				// We are making progress!
				if((++progress / PROGRESS_STEP) > stepprogress) 
				{
					stepprogress = (progress / PROGRESS_STEP);
					AddProgress(1);
				}
			}	
		}

		private string GetDescription(string group)
		{
			switch(group) 
			{
				case "skills": return "in any skill level";
				case "gamemodes": return "in any game mode";
				case "classes": return "by any class";
				default: return "by any class, skill or game mode";
			}
		}

		#endregion
	}
}
