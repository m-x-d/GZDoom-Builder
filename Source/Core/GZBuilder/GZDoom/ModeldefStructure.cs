#region ================== Namespaces

using System;
using System.IO;
using System.Globalization;
using SlimDX;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.Geometry;

#endregion

namespace CodeImp.DoomBuilder.GZBuilder.GZDoom 
{
	internal sealed class ModeldefStructure 
	{
		private const int MAX_MODELS = 4; //maximum models per modeldef entry, zero-based
		private bool parsingfinished;

		internal ModelData ModelData;
		internal bool ParsingFinished { get { return parsingfinished; } }

		internal bool Parse(ModeldefParser parser, string classname)
		{

#region ================== Vars

			string[] textureNames = new string[MAX_MODELS];
			string[] modelNames = new string[MAX_MODELS];
			string[] frameNames = new string[MAX_MODELS];
			int[] frameIndices = new int[MAX_MODELS];
			bool[] modelsUsed = new bool[MAX_MODELS];
			string path = "";
			Vector3 scale = new Vector3(1, 1, 1);
			Vector3 offset = new Vector3();
			float angleOffset = 0;
			float pitchOffset = 0;
			float rollOffset = 0;
			bool inheritactorpitch = false; 
			bool inheritactorroll = false;

			string token;

#endregion

			//read modeldef structure contents
			parsingfinished = false;
			while(!parsingfinished && parser.SkipWhitespace(true)) 
			{
				token = parser.ReadToken();
				if(!string.IsNullOrEmpty(token)) 
				{
					token = parser.StripTokenQuotes(token).ToLowerInvariant(); //ANYTHING can be quoted...
					switch(token)
					{

#region ================== Path

						case "path":
							parser.SkipWhitespace(true);
							path = parser.StripTokenQuotes(parser.ReadToken(false)).Replace("/", "\\"); // Don't skip newline
							if(string.IsNullOrEmpty(path))
							{
								parser.ReportError("Expected model path");
								return false;
							}
							break;

#endregion

#region ================== Model

						case "model":
							parser.SkipWhitespace(true);

							//model index
							int index;
							token = parser.StripTokenQuotes(parser.ReadToken());
							if(!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out index)) 
							{
								// Not numeric!
								parser.ReportError("Expected model index, but got \"" + token + "\"");
								return false;
							}

							if(index >= MAX_MODELS) 
							{
								parser.ReportError("GZDoom doesn't allow more than " + MAX_MODELS + " models per MODELDEF entry");
								return false;
							}

							parser.SkipWhitespace(true);

							//model path
							token = parser.StripTokenQuotes(parser.ReadToken(false)).ToLowerInvariant(); // Don't skip newline
							if(string.IsNullOrEmpty(token)) 
							{
								parser.ReportError("Expected model name");
								return false;
							} 

							//check invalid path chars
							if(!parser.CheckInvalidPathChars(token)) return false;

							//check extension
							string modelext = Path.GetExtension(token);
							if(string.IsNullOrEmpty(modelext)) 
							{
								parser.ReportError("Model \"" + token + "\" won't be loaded. Models without extension are not supported by GZDoom");
								return false;
							}

							if(modelext != ".md3" && modelext != ".md2") 
							{
								parser.ReportError("Model \"" + token + "\" won't be loaded. Only MD2 and MD3 models are supported");
								return false;
							}

							//GZDoom allows models with identical modelIndex, it uses the last one encountered
							modelNames[index] = Path.Combine(path, token);
							break;

#endregion

#region ================== Skin

						case "skin":
							parser.SkipWhitespace(true);

							//skin index
							int skinIndex;
							token = parser.StripTokenQuotes(parser.ReadToken());
							if(!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out skinIndex)) 
							{
								// Not numeric!
								parser.ReportError("Expected skin index, but got \"" + token + "\"");
								return false;
							}

							if(skinIndex >= MAX_MODELS) 
							{
								parser.ReportError("GZDoom doesn't allow more than " + MAX_MODELS + " skins per MODELDEF entry");
								return false;
							}

							parser.SkipWhitespace(true);

							//skin path
							token = parser.StripTokenQuotes(parser.ReadToken(false)).ToLowerInvariant(); // Don't skip newline
							if(string.IsNullOrEmpty(token)) 
							{
								parser.ReportError("Expected skin path");
								return false;
							} 

							//check invalid path chars
							if(!parser.CheckInvalidPathChars(token)) return false;

							//check extension
							string texext = Path.GetExtension(token);
							if(Array.IndexOf(ModelData.SUPPORTED_TEXTURE_EXTENSIONS, texext) == -1) 
							{
								parser.ReportError("Image format \"" + texext + "\" is not supported");
								return false;
							} 

							//GZDoom allows skins with identical modelIndex, it uses the last one encountered
							textureNames[skinIndex] = Path.Combine(path, token);
							break;

#endregion

#region ================== Scale

						case "scale":
							parser.SkipWhitespace(true);
							token = parser.StripTokenQuotes(parser.ReadToken());
							if(!parser.ReadSignedFloat(token, ref scale.Y)) 
							{
								// Not numeric!
								parser.ReportError("Expected Scale X value, but got \"" + token + "\"");
								return false;
							}

							parser.SkipWhitespace(true);
							token = parser.StripTokenQuotes(parser.ReadToken());
							if(!parser.ReadSignedFloat(token, ref scale.X)) 
							{
								// Not numeric!
								parser.ReportError("Expected Scale Y value, but got \"" + token + "\"");
								return false;
							}

							parser.SkipWhitespace(true);
							token = parser.StripTokenQuotes(parser.ReadToken());
							if(!parser.ReadSignedFloat(token, ref scale.Z)) 
							{
								// Not numeric!
								parser.ReportError("Expected Scale Z value, but got \"" + token + "\"");
								return false;
							}
							break;

#endregion

#region ================== Offset

						case "offset":
							parser.SkipWhitespace(true);
							token = parser.StripTokenQuotes(parser.ReadToken());
							if(!parser.ReadSignedFloat(token, ref offset.X)) 
							{
								// Not numeric!
								parser.ReportError("Expected Offset X value, but got \"" + token + "\"");
								return false;
							}

							parser.SkipWhitespace(true);
							token = parser.StripTokenQuotes(parser.ReadToken());
							if(!parser.ReadSignedFloat(token, ref offset.Y)) 
							{
								// Not numeric!
								parser.ReportError("Expected Offset Y value, but got \"" + token + "\"");
								return false;
							}

							parser.SkipWhitespace(true);
							token = parser.StripTokenQuotes(parser.ReadToken());
							if(!parser.ReadSignedFloat(token, ref offset.Z)) 
							{
								// Not numeric!
								parser.ReportError("Expected Offset Z value, but got \"" + token + "\"");
								return false;
							}
							break;

#endregion

#region ================== ZOffset

						case "zoffset":
							parser.SkipWhitespace(true);
							token = parser.StripTokenQuotes(parser.ReadToken());
							if(!parser.ReadSignedFloat(token, ref offset.Z)) 
							{
								// Not numeric!
								parser.ReportError("Expected ZOffset value, but got \"" + token + "\"");
								return false;
							}
							break;

#endregion

#region ================== AngleOffset

						case "angleoffset":
							parser.SkipWhitespace(true);
							token = parser.StripTokenQuotes(parser.ReadToken());
							if(!parser.ReadSignedFloat(token, ref angleOffset)) 
							{
								// Not numeric!
								parser.ReportError("Expected AngleOffset value, but got \"" + token + "\"");
								return false;
							}
							break;

#endregion

#region ================== PitchOffset

						case "pitchoffset":
							parser.SkipWhitespace(true);
							token = parser.StripTokenQuotes(parser.ReadToken());
							if(!parser.ReadSignedFloat(token, ref pitchOffset)) 
							{
								// Not numeric!
								parser.ReportError("Expected PitchOffset value, but got \"" + token + "\"");
								return false;
							}
							break;

#endregion

#region ================== RollOffset

						case "rolloffset":
							parser.SkipWhitespace(true);
							token = parser.StripTokenQuotes(parser.ReadToken());
							if(!parser.ReadSignedFloat(token, ref rollOffset)) 
							{
								// Not numeric!
								parser.ReportError("Expected RollOffset value, but got \"" + token + "\"");
								return false;
							}
							break;

#endregion

#region ================== InheritActorPitch

						case "inheritactorpitch":
							inheritactorpitch = true;
							break;

#endregion

#region ================== InheritActorRoll

						case "inheritactorroll":
							inheritactorroll = true;
							break;

#endregion

#region ================== Frame / FrameIndex

						case "frameindex":
						case "frame":
							//parsed all required fields. if got more than one model - find which one(s) should be displayed 
							if(modelNames.GetLength(0) > 1) 
							{
								string spriteLump = null;
								string spriteFrame = null;

								//step back
								parser.DataStream.Seek(-token.Length - 1, SeekOrigin.Current);

								//here we check which models are used in first encountered lump and frame
								while(parser.SkipWhitespace(true)) 
								{
									token = parser.StripTokenQuotes(parser.ReadToken()).ToLowerInvariant();
									if(token == "frameindex" || token == "frame") 
									{
										bool frameIndex = (token == "frameindex");
										parser.SkipWhitespace(true);

										//should be sprite lump
										token = parser.StripTokenQuotes(parser.ReadToken()).ToLowerInvariant();
										if(string.IsNullOrEmpty(spriteLump)) 
										{
											spriteLump = token;
										} 
										else if(spriteLump != token) //got another lump
										{ 
											for(int i = 0; i < modelsUsed.Length; i++) 
											{
												if(!modelsUsed[i]) 
												{
													modelNames[i] = null;
													textureNames[i] = null;
												}
											}
											break;
										}

										parser.SkipWhitespace(true);

										//should be sprite frame
										token = parser.StripTokenQuotes(parser.ReadToken()).ToLowerInvariant();
										if(string.IsNullOrEmpty(spriteFrame)) 
										{
											spriteFrame = token;
										} 
										else if(spriteFrame != token) //got another frame
										{
											for(int i = 0; i < modelsUsed.Length; i++) 
											{
												if(!modelsUsed[i]) 
												{
													modelNames[i] = null;
													textureNames[i] = null;
												}
											}
											break;
										}

										parser.SkipWhitespace(true);

										//should be model index
										token = parser.StripTokenQuotes(parser.ReadToken());

										int modelIndex;
										if(!int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out modelIndex)) 
										{
											// Not numeric!
											parser.ReportError("Expected model index, but got \"" + token + "\"");
											return false;
										}

										if(modelIndex >= MAX_MODELS) 
										{
											parser.ReportError("GZDoom doesn't allow more than " + MAX_MODELS + " models per MODELDEF entry");
											return false;
										}

										if(modelNames[modelIndex] == null) 
										{
											parser.ReportError("Model index doesn't correspond to any defined model");
											return false;
										}

										modelsUsed[modelIndex] = true;

										parser.SkipWhitespace(true);

										// Should be frame name or index
										token = parser.StripTokenQuotes(parser.ReadToken());
										if(frameIndex)
										{
											int frame = 0;
											if(!parser.ReadSignedInt(token, ref frame))
											{
												// Not numeric!
												parser.ReportError("Expected model frame index, but got \"" + token + "\"");
												return false;
											}

											// Skip the model if frame index is -1
											if(frame == -1) modelsUsed[modelIndex] = false;
											else frameIndices[modelIndex] = frame;
										}
										else
										{
											if(string.IsNullOrEmpty(token))
											{
												// Missing!
												parser.ReportError("Expected model frame name");
												return false;
											}

											frameNames[modelIndex] = token.ToLowerInvariant();
										}
									} 
									else 
									{
										//must be "}", step back
										parser.DataStream.Seek(-token.Length - 1, SeekOrigin.Current);
										break;
									}
								}
							}

							parsingfinished = true;
							break;

#endregion
					}
				}
			}

			// Find closing brace, then quit
			while(parser.SkipWhitespace(true)) 
			{
				token = parser.ReadToken();
				if(string.IsNullOrEmpty(token) || token == "}") break;
			}

			// Bail out when got errors or no models are used
			if(Array.IndexOf(modelsUsed, true) == -1)
			{
				parser.ReportError("No models are used by \"" + classname + "\"");
				return false;
			}
			
			// Classname is set in ModeldefParser
			ModelData = new ModelData { InheritActorPitch = inheritactorpitch, InheritActorRoll = inheritactorroll };
			Matrix moffset = Matrix.Translation(offset.Y, -offset.X, offset.Z); // Things are complicated in GZDoom...
			Matrix mrotation = Matrix.RotationY(-Angle2D.DegToRad(rollOffset)) * Matrix.RotationX(-Angle2D.DegToRad(pitchOffset)) * Matrix.RotationZ(Angle2D.DegToRad(angleOffset));
			ModelData.SetTransform(mrotation, moffset, scale);

			for(int i = 0; i < modelNames.Length; i++) 
			{
				if(!string.IsNullOrEmpty(modelNames[i]) && modelsUsed[i]) 
				{
					ModelData.TextureNames.Add(string.IsNullOrEmpty(textureNames[i]) ? string.Empty : textureNames[i].ToLowerInvariant());
					ModelData.ModelNames.Add(modelNames[i].ToLowerInvariant());
					ModelData.FrameNames.Add(frameNames[i]);
					ModelData.FrameIndices.Add(frameIndices[i]);
				}
			}

			if(ModelData.ModelNames.Count == 0)
			{
				parser.ReportError("\"" + classname + "\" has no models");
				return false;
			}

			return true;
		}
	}
}
