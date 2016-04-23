#region ================== Namespaces

using System;
using System.Collections.Generic;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.ZDoom;
using CodeImp.DoomBuilder.GZBuilder.Data;
using SlimDX;

#endregion

namespace CodeImp.DoomBuilder.GZBuilder.GZDoom 
{
	internal class ModeldefParser : ZDTextParser
	{
		#region ================== Variables

		private readonly Dictionary<string, int> actorsbyclass;
		private Dictionary<string, ModelData> entries; //classname, entry

		#endregion

		#region ================== Properties

		internal override ScriptType ScriptType { get { return ScriptType.MODELDEF; } }
		internal Dictionary<string, ModelData> Entries { get { return entries; } }

		#endregion

		#region ================== Constructor

		internal ModeldefParser(Dictionary<string, int> actorsbyclass)
		{
			this.actorsbyclass = actorsbyclass;
			this.entries = new Dictionary<string, ModelData>(StringComparer.OrdinalIgnoreCase);
		}

		#endregion

		#region ================== Parsing

		// Should be called after all decorate actors are parsed 
		public override bool Parse(TextResourceData data, bool clearerrors)
		{
			// Already parsed?
			if(!base.AddTextResource(data))
			{
				if(clearerrors) ClearError();
				return true;
			}

			// Cannot process?
			if(!base.Parse(data, clearerrors)) return false;

			// Continue until at the end of the stream
			while(SkipWhitespace(true)) 
			{
				string token = ReadToken();
				if(string.IsNullOrEmpty(token) || token.ToLowerInvariant() != "model") continue;

				// Find classname
				SkipWhitespace(true);
				string classname = StripQuotes(ReadToken(ActorStructure.ACTOR_CLASS_SPECIAL_TOKENS));
				if(string.IsNullOrEmpty(classname))
				{
					ReportError("Expected actor class");
					return false;
				}

				// Check if actor exists
				bool haveplaceableactor = actorsbyclass.ContainsKey(classname);
				if(!haveplaceableactor && !General.Map.Data.Decorate.ActorsByClass.ContainsKey(classname))
					LogWarning("DECORATE class \"" + classname + "\" does not exist");
				
				// Now find opening brace
				if(!NextTokenIs("{")) return false;

				// Parse the structure
				ModeldefStructure mds = new ModeldefStructure();
				if(mds.Parse(this))
				{
					// Fetch Actor info
					if(haveplaceableactor)
					{
						ThingTypeInfo info = General.Map.Data.GetThingInfoEx(actorsbyclass[classname]);

						// Actor has a valid sprite?
						if(info != null && !string.IsNullOrEmpty(info.Sprite) && !info.Sprite.ToLowerInvariant().StartsWith(DataManager.INTERNAL_PREFIX))
						{
							string targetsprite = info.Sprite.Substring(0, 5);
							if(mds.Frames.ContainsKey(targetsprite))
							{
								// Create model data
								ModelData md = new ModelData { InheritActorPitch = mds.InheritActorPitch, InheritActorRoll = mds.InheritActorRoll };

								// Things are complicated in GZDoom...
								Matrix moffset = Matrix.Translation(mds.Offset.Y, -mds.Offset.X, mds.Offset.Z);
								Matrix mrotation = Matrix.RotationY(-Angle2D.DegToRad(mds.RollOffset)) * Matrix.RotationX(-Angle2D.DegToRad(mds.PitchOffset)) * Matrix.RotationZ(Angle2D.DegToRad(mds.AngleOffset));
								md.SetTransform(mrotation, moffset, mds.Scale);

								// Add models
								foreach(var fs in mds.Frames[targetsprite])
								{
									// Sanity checks
									if(string.IsNullOrEmpty(mds.ModelNames[fs.ModelIndex]))
									{
										LogWarning("Model definition \"" + classname + "\", frame \"" + fs.SpriteName + " " + fs.FrameName + "\" references undefiend model index " + fs.ModelIndex);
										continue;
									}
									
									// Texture name will be empty when skin path is embedded in the model
									string texturename = (!string.IsNullOrEmpty(mds.TextureNames[fs.ModelIndex]) ? mds.TextureNames[fs.ModelIndex].ToLowerInvariant() : string.Empty);

									md.TextureNames.Add(texturename);
									md.ModelNames.Add(mds.ModelNames[fs.ModelIndex].ToLowerInvariant());
									md.FrameNames.Add(fs.FrameName);
									md.FrameIndices.Add(fs.FrameIndex);
								}

								// More sanity checks...
								if(md.ModelNames.Count == 0)
								{
									LogWarning("Model definition \"" + classname + "\" has no defined models");
								}
								else
								{
									// Add to collection
									entries[classname] = md;
								}
							}
						}
					}
				}
							
				if(HasError)
				{
					LogError();
					ClearError();
				}
			}

			return true;
		}

		#endregion
	}
}
