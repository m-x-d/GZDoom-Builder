#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.VisualModes;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes.ErrorChecks
{
	[ErrorChecker("Check texture alignment", true, 1000)]
	public class CheckTextureAlignment : ErrorChecker
	{
		#region ================== Constants

		private const int PROGRESS_STEP = 100;

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public CheckTextureAlignment()
		{
			// Total progress is done when all lines are checked
			SetTotalProgress(General.Map.Map.Linedefs.Count / PROGRESS_STEP);
		}
		
		#endregion
		
		#region ================== Methods
		
		// This runs the check
		public override void Run()
		{
			Dictionary<int, Dictionary<int, bool>> donesides = new Dictionary<int, Dictionary<int, bool>>();
			int progress = 0;
			int stepprogress = 0;
			bool udmf = General.Map.UDMF;

			// Go for all the liendefs
			foreach(Linedef l in General.Map.Map.Linedefs) 
			{
				// Check if not already done
				if (donesides.ContainsKey(l.Index)) continue;

				// Check if we need to align any part of the front sidedef
				if (l.Front != null) 
				{
					CheckTopAlignment(l.Front, donesides, udmf);
					CheckMiddleAlignment(l.Front, donesides, udmf);
					CheckBottomAlignment(l.Front, donesides, udmf);
				}

				// Check if we need to align any part of the back sidedef
				if (l.Back != null) 
				{
					CheckTopAlignment(l.Back, donesides, udmf);
					CheckMiddleAlignment(l.Back, donesides, udmf);
					CheckBottomAlignment(l.Back, donesides, udmf);
				}

				// Handle thread interruption
				try { Thread.Sleep(0); } catch(ThreadInterruptedException) { return; }

				// We are making progress!
				if ((++progress / PROGRESS_STEP) > stepprogress) 
				{
					stepprogress = (progress / PROGRESS_STEP);
					AddProgress(1);
				}
			}
		}

		#endregion

		#region ================== UDMF alignment checks

		private void CheckTopAlignment(Sidedef sidedef, Dictionary<int, Dictionary<int, bool>> donesides, bool udmf) 
		{
			if (!sidedef.HighRequired() || sidedef.LongHighTexture == MapSet.EmptyLongName) return;
			if (!udmf)
			{
				CheckClassicTopAlignment(sidedef, donesides);
				return;
			}

		}

		private void CheckMiddleAlignment(Sidedef sidedef, Dictionary<int, Dictionary<int, bool>> donesides, bool udmf) 
		{
			if (!sidedef.MiddleRequired() || sidedef.LongMiddleTexture == MapSet.EmptyLongName) return;
			if (!udmf)
			{
				CheckClassicMiddleAlignment(sidedef, donesides);
				return;
			}

		}

		private void CheckBottomAlignment(Sidedef sidedef, Dictionary<int, Dictionary<int, bool>> donesides, bool udmf) 
		{
			if (!sidedef.LowRequired() || sidedef.LongLowTexture == MapSet.EmptyLongName) return;
			if (!udmf) 
			{
				CheckClassicBottomAlignment(sidedef, donesides);
				return;
			}

		}

		private void AddProcessedSides(Sidedef s1, Sidedef s2, Dictionary<int, Dictionary<int, bool>> donesides)
		{
			// Add them both ways
			if (!donesides.ContainsKey(s1.Index)) donesides.Add(s1.Index, new Dictionary<int, bool>());
			donesides[s1.Index].Add(s2.Index, false);

			if (!donesides.ContainsKey(s2.Index)) donesides.Add(s2.Index, new Dictionary<int, bool>());
			donesides[s2.Index].Add(s1.Index, false);
		}

		#endregion

		#region ==================  Classic alignment checks

		private void CheckClassicTopAlignment(Sidedef sidedef, Dictionary<int, Dictionary<int, bool>> donesides) 
		{
			int x = sidedef.OffsetX;
			int y = (int)Tools.GetSidedefTopOffsetY(sidedef, sidedef.OffsetY, 1.0f, false);
			CheckClassicAlignment(sidedef, x, y, BuilderModesTools.GetSidedefPartSize(sidedef, VisualGeometryType.WALL_UPPER), sidedef.HighTexture, donesides);
		}

		private void CheckClassicMiddleAlignment(Sidedef sidedef, Dictionary<int, Dictionary<int, bool>> donesides)
		{
			int x = sidedef.OffsetX;
			int y = (int)Tools.GetSidedefMiddleOffsetY(sidedef, sidedef.OffsetY, 1.0f, false);
			CheckClassicAlignment(sidedef, x, y, BuilderModesTools.GetSidedefPartSize(sidedef, VisualGeometryType.WALL_MIDDLE), sidedef.MiddleTexture, donesides);
		}

		private void CheckClassicBottomAlignment(Sidedef sidedef, Dictionary<int, Dictionary<int, bool>> donesides) 
		{
			int x = sidedef.OffsetX;
			int y = (int)Tools.GetSidedefBottomOffsetY(sidedef, sidedef.OffsetY, 1.0f, false);
			CheckClassicAlignment(sidedef, x, y, BuilderModesTools.GetSidedefPartSize(sidedef, VisualGeometryType.WALL_LOWER), sidedef.LowTexture, donesides);
		}

		private void CheckClassicAlignment(Sidedef sidedef, int offsetx, int offsety, Rectangle partsize, string texturename, Dictionary<int, Dictionary<int, bool>> donesides) 
		{
			ImageData texture = General.Map.Data.GetTextureImage(texturename);
			if (!texture.IsImageLoaded) return;

			float scalex = (General.Map.Config.ScaledTextureOffsets && !texture.WorldPanning) ? texture.Scale.x : 1.0f;
			float scaley = (General.Map.Config.ScaledTextureOffsets && !texture.WorldPanning) ? texture.Scale.y : 1.0f;

			// Move offsets to proper range
			offsetx %= texture.Width;
			if (offsetx < 0) offsetx += texture.Width;
			offsety %= texture.Height;
			if (offsety < 0) offsety += texture.Height;

			// Check if current line is aligned to other sides
			ICollection<Linedef> lines = (sidedef.IsFront ? sidedef.Line.Start.Linedefs : sidedef.Line.End.Linedefs);
			Vertex v = sidedef.IsFront ? sidedef.Line.Start : sidedef.Line.End;

			foreach(Linedef line in lines) 
			{
				if (line.Index == sidedef.Line.Index) continue;

				Sidedef target = null;
				if (line.Front != null && line.End == v) target = line.Front;
				else if (line.Back != null && line.Start == v) target = line.Back;

				// No target or laready processed?
				if (target == null || (donesides.ContainsKey(sidedef.Index) && donesides[sidedef.Index].ContainsKey(target.Index)))
					continue;

				// Get expected texture offsets
				int alignedY = GetExpectedOffsetY(sidedef, target, texturename, texture.Height, scaley, partsize);
				if (alignedY == int.MinValue) continue; // alignedY == int.MinValue means no textures on target and current source part match 
				
				alignedY %= texture.Height;
				if (alignedY < 0) alignedY += texture.Height;

				int alignedX = (target.OffsetX + (int)Math.Round(target.Line.Length / scalex)) % texture.Width;
				if (alignedX < 0) alignedX += texture.Width;

				// Submit result if target offsets don't match expected ones
				if (offsetx != alignedX || offsety != alignedY)
				{
#if DEBUG //TODO: remove this
					string msg = "Case 1: '"+texturename+"' source " + sidedef.Line.Index + " (" + (sidedef.IsFront ? "front" : "back") 
						+ "), target " + target.Line.Index + " (" + (target.IsFront ? "front" : "back") 
						+ "): expected: " + alignedX + ", " + alignedY
						+ "; actual [source]: " + offsetx + ", " + offsety;

					SubmitResult(new ResultTexturesMisaligned(sidedef, target, texturename, msg));
#else
					SubmitResult(new ResultTexturesMisaligned(sidedef, target, texturename));
#endif
					AddProcessedSides(sidedef, target, donesides);
				}
			}

			// Check if other sides are aligned to current side
			lines = (sidedef.IsFront ? sidedef.Line.End.Linedefs : sidedef.Line.Start.Linedefs);
			v = (sidedef.IsFront ? sidedef.Line.End : sidedef.Line.Start);

			foreach(Linedef line in lines) 
			{
				if (line.Index == sidedef.Line.Index) continue;

				Sidedef target = null;
				if (line.Front != null && line.Start == v) target = line.Front;
				else if (line.Back != null && line.End == v) target = line.Back;

				// No target or laready processed?
				if (target == null || (donesides.ContainsKey(sidedef.Index) && donesides[sidedef.Index].ContainsKey(target.Index)))
					continue;

				// Get expected texture offsets
				int alignedY = GetExpectedOffsetY(sidedef, target, texturename, texture.Height, scaley, partsize);
				if (alignedY == int.MinValue) continue; // alignedY == int.MinValue means no textures on target and current source part match 
				
				alignedY %= texture.Height;
				if (alignedY < 0) alignedY += texture.Height;

				int alignedX = (target.OffsetX - (int)Math.Round(sidedef.Line.Length / scalex)) % texture.Width;
				if (alignedX < 0) alignedX += texture.Width;

				// Submit result if target offsets don't match expected ones
				if (offsetx != alignedX || offsety != alignedY) 
				{
#if DEBUG //TODO: remove this
					string msg = "Case 2: '" + texturename + "' source " + sidedef.Line.Index + " (" + (sidedef.IsFront ? "front" : "back")
						+ "), target " + target.Line.Index + " (" + (target.IsFront ? "front" : "back")
						+ "): expected: " + alignedX + ", " + alignedY
						+ "; actual [source]: " + offsetx + ", " + offsety;
					
					SubmitResult(new ResultTexturesMisaligned(target, sidedef, texturename, msg));
#else
					SubmitResult(new ResultTexturesMisaligned(target, sidedef, texturename));
#endif
					AddProcessedSides(sidedef, target, donesides);
				}
			}
		}

		private int GetExpectedOffsetY(Sidedef source, Sidedef target, string texturename, int textureheight, float scaleY, Rectangle partsize) 
		{
			if (target.MiddleTexture == texturename
					&& partsize.IntersectsWith(BuilderModesTools.GetSidedefPartSize(target, VisualGeometryType.WALL_MIDDLE))) 
			{
				return ((int)Tools.GetSidedefMiddleOffsetY(target, target.OffsetY, 1.0f, false) + (int)Math.Round((target.Sector.CeilHeight - source.Sector.CeilHeight) / scaleY)) % textureheight;
			} 
			
			if (target.HighTexture == texturename
					&& partsize.IntersectsWith(BuilderModesTools.GetSidedefPartSize(target, VisualGeometryType.WALL_UPPER))) 
			{
				return ((int)Tools.GetSidedefTopOffsetY(target, target.OffsetY, 1.0f, false) + (int)Math.Round((target.Sector.CeilHeight - source.Sector.CeilHeight) / scaleY)) % textureheight;
			} 
			
			if (target.LowTexture == texturename
				  && partsize.IntersectsWith(BuilderModesTools.GetSidedefPartSize(target, VisualGeometryType.WALL_LOWER))) 
			{
				return ((int)Tools.GetSidedefBottomOffsetY(target, target.OffsetY, 1.0f, false) + (int)Math.Round((target.Sector.CeilHeight - source.Sector.CeilHeight) / scaleY)) % textureheight;
			}

			return int.MinValue;
		}

		#endregion
	}
}
