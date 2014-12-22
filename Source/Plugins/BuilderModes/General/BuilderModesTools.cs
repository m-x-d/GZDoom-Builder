#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Drawing;
using CodeImp.DoomBuilder.GZBuilder.Tools;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.VisualModes;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	// A struct, which contains information about visual sides connected to start and end of given visual side
	internal class SortedVisualSide
	{
		internal readonly BaseVisualGeometrySidedef Side;
		internal readonly Vector2D Start;
		internal readonly Vector2D End;
		internal Rectangle Bounds;
		internal readonly Dictionary<SortedVisualSide, bool> NextSides;
		internal readonly Dictionary<SortedVisualSide, bool> PreviousSides;
		internal readonly int Index;
		private static int index;

		//Initial texture coordinates
		private readonly float OffsetX;
		private readonly float OffsetY;
		private readonly float ScaleX;
		private readonly float ScaleY;

		internal SortedVisualSide(BaseVisualGeometrySidedef side)
		{
			Side = side;
			Bounds = BuilderModesTools.GetSidedefPartSize(side);
			Index = index++;

			if (side.Sidedef.Line.Front == side.Sidedef)
			{
				Start = side.Sidedef.Line.Start.Position;
				End = side.Sidedef.Line.End.Position;
			}
			else
			{
				Start = side.Sidedef.Line.End.Position;
				End = side.Sidedef.Line.Start.Position;
			}

			switch (side.GeometryType)
			{
				case VisualGeometryType.WALL_UPPER:
					OffsetX = UDMFTools.GetFloat(side.Sidedef.Fields, "offsetx_top");
					OffsetY = UDMFTools.GetFloat(side.Sidedef.Fields, "offsety_top");
					ScaleX = UDMFTools.GetFloat(side.Sidedef.Fields, "scalex_top", 1.0f);
					ScaleY = UDMFTools.GetFloat(side.Sidedef.Fields, "scaley_top", 1.0f);
					break;

				case VisualGeometryType.WALL_MIDDLE:
					OffsetX = UDMFTools.GetFloat(side.Sidedef.Fields, "offsetx_mid");
					OffsetY = UDMFTools.GetFloat(side.Sidedef.Fields, "offsety_mid");
					ScaleX = UDMFTools.GetFloat(side.Sidedef.Fields, "scalex_mid", 1.0f);
					ScaleY = UDMFTools.GetFloat(side.Sidedef.Fields, "scaley_mid", 1.0f);
					break;

				case VisualGeometryType.WALL_MIDDLE_3D:
					Sidedef cs = side.GetControlLinedef().Front;
					OffsetX = UDMFTools.GetFloat(cs.Fields, "offsetx_mid");
					OffsetY = UDMFTools.GetFloat(cs.Fields, "offsety_mid");
					ScaleX = UDMFTools.GetFloat(cs.Fields, "scalex_mid", 1.0f);
					ScaleY = UDMFTools.GetFloat(cs.Fields, "scaley_mid", 1.0f);
					break;

				case VisualGeometryType.WALL_LOWER:
					OffsetX = UDMFTools.GetFloat(side.Sidedef.Fields, "offsetx_bottom");
					OffsetY = UDMFTools.GetFloat(side.Sidedef.Fields, "offsety_bottom");
					ScaleX = UDMFTools.GetFloat(side.Sidedef.Fields, "scalex_bottom", 1.0f);
					ScaleY = UDMFTools.GetFloat(side.Sidedef.Fields, "scaley_bottom", 1.0f);
					break;
			}

			NextSides = new Dictionary<SortedVisualSide, bool>();
			PreviousSides = new Dictionary<SortedVisualSide, bool>();
		}

		internal void OnTextureFit(FitTextureOptions options)
		{
			options.Bounds = Bounds;
			options.InitialOffsetX = OffsetX;
			options.InitialOffsetY = OffsetY;
			options.InitialScaleX = ScaleX;
			options.InitialScaleY = ScaleY;

			Side.OnTextureFit(options);
		}
	}
	
	internal static class BuilderModesTools
	{
		#region ================== Sidedef

		internal static Rectangle GetSidedefPartSize(BaseVisualGeometrySidedef side) 
		{
			if(side.GeometryType == VisualGeometryType.WALL_MIDDLE_3D) 
			{
				Rectangle rect = new Rectangle(0, 0, Math.Max(1, (int)Math.Round(side.Sidedef.Line.Length)), 0);
				Linedef cl = side.GetControlLinedef();
				
				if(cl.Front != null && cl.Front.Sector != null) 
				{
					// Use floor height for vavoom-type 3d floors, because FloorHeight should be > CeilHeight for this type of 3d floor.
					if (cl.Args[1] == 0)
					{
						rect.Y = -cl.Front.Sector.FloorHeight;
						rect.Height = cl.Front.Sector.FloorHeight - cl.Front.Sector.CeilHeight;
					}
					else
					{
						rect.Y = -cl.Front.Sector.CeilHeight;
						rect.Height = cl.Front.GetMiddleHeight();
					}
				} 
				else 
				{
					rect.Y = -side.Sidedef.Sector.CeilHeight;
					rect.Height = side.Sidedef.GetMiddleHeight();
				}

				return rect;
			}

			return GetSidedefPartSize(side.Sidedef, side.GeometryType);
		}

		public static Rectangle GetSidedefPartSize(Sidedef side, VisualGeometryType type) 
		{
			Rectangle rect = new Rectangle(0, 0, Math.Max(1, (int)Math.Round(side.Line.Length)), 0);

			switch(type) 
			{
				case VisualGeometryType.WALL_LOWER:
					if (side.LowRequired())
					{
						rect.Y = -side.Other.Sector.FloorHeight;
						rect.Height = side.GetLowHeight();
					}
					break;

				case VisualGeometryType.WALL_UPPER:
					if(side.HighRequired()) 
					{
						rect.Y = -side.Sector.CeilHeight;
						rect.Height = side.GetHighHeight();
					} 
					break;

				case VisualGeometryType.WALL_MIDDLE:
					if(side.MiddleRequired())
					{
						rect.Y = -side.Sector.CeilHeight;
					}
					else if (side.Other.Sector != null) // Double-sided
					{
						rect.Y = -Math.Min(side.Sector.CeilHeight, side.Other.Sector.CeilHeight);
					}
					rect.Height = side.GetMiddleHeight();
					break;

				default:
					throw new NotImplementedException("GetSidedefPartSize: got unsupported geometry type: '" + type + "'");
			}

			return rect;
		}

		public static List<SortedVisualSide> SortVisualSides(IEnumerable<BaseVisualGeometrySidedef> tosort)
		{
			List<SortedVisualSide> result = new List<SortedVisualSide>();

			// Sort by texture
			Dictionary<long, List<BaseVisualGeometrySidedef>> sidesbytexture = new Dictionary<long, List<BaseVisualGeometrySidedef>>();
			foreach (BaseVisualGeometrySidedef side in tosort)
			{
				long texturelong;
				if (side is VisualLower) texturelong = side.Sidedef.LongLowTexture;
				else if (side is VisualUpper) texturelong = side.Sidedef.LongHighTexture;
				else texturelong = side.Sidedef.LongMiddleTexture;

				if(texturelong == MapSet.EmptyLongName) continue; //not interested...

				if(!sidesbytexture.ContainsKey(texturelong)) sidesbytexture.Add(texturelong, new List<BaseVisualGeometrySidedef>());
				sidesbytexture[texturelong].Add(side);
			}

			// Connect sides
			foreach (KeyValuePair<long, List<BaseVisualGeometrySidedef>> pair in sidesbytexture)
			{
				IEnumerable<SortedVisualSide> group = ConnectSides(pair.Value);
				result.AddRange(group);
			}

			return result;
		}

		// Connect sides, left to right 
		// NextSides - sides connected to the right (Start) vertex, 
		// PreviousSides - sides connected to the left (End) vertex
		private static IEnumerable<SortedVisualSide> ConnectSides(List<BaseVisualGeometrySidedef> allsides)
		{
			List<SortedVisualSide> result = new List<SortedVisualSide>();
			List<SortedVisualSide> sides = new List<SortedVisualSide>(allsides.Count);
			foreach (BaseVisualGeometrySidedef side in allsides)
			{
				sides.Add(new SortedVisualSide(side));
			}

			foreach(SortedVisualSide curside in sides)
			{
				// Find sides connected to the end of curside
				foreach(SortedVisualSide nextside in sides) 
				{
					if(curside.Index == nextside.Index) continue;
					if(nextside.Start == curside.End && nextside.End != curside.Start) 
					{
						// Add both ways
						if(!nextside.PreviousSides.ContainsKey(curside)) nextside.PreviousSides.Add(curside, false);
						if(!curside.NextSides.ContainsKey(nextside)) curside.NextSides.Add(nextside, false);
					}
				}

				// Find sides connected to the start of curside
				foreach(SortedVisualSide prevside in sides) 
				{
					if(curside.Index == prevside.Index) continue;
					if(prevside.End == curside.Start && prevside.Start != curside.End) 
					{
						// Add both ways
						if(!prevside.NextSides.ContainsKey(curside)) prevside.NextSides.Add(curside, false);
						if(!curside.PreviousSides.ContainsKey(prevside)) curside.PreviousSides.Add(prevside, false);
					}
				}

				result.Add(curside);
			}

			// Try to find the left-most side
			SortedVisualSide start = result[0];
			foreach (SortedVisualSide side in result)
			{
				if (side.PreviousSides.Count == 0)
				{
					start = side;
					break;
				}
			}

			// Set horizontal offsets...
			ApplyHorizontalOffset(start, null, true, new Dictionary<int, bool>());

			return result;
		}

		private static void ApplyHorizontalOffset(SortedVisualSide side, SortedVisualSide prevside, bool forward, Dictionary<int, bool> processed) 
		{
			// Set offset
			if (!processed.ContainsKey(side.Index))
			{
				if (prevside != null)
				{
					if (forward)
						side.Bounds.X = prevside.Bounds.X + (int)Math.Round(prevside.Side.Sidedef.Line.Length);
					else
						side.Bounds.X = prevside.Bounds.X - (int)Math.Round(side.Side.Sidedef.Line.Length);
				}

				processed.Add(side.Index, false);
			}

			// Repeat for NextSides
			foreach (KeyValuePair<SortedVisualSide, bool> pair in side.NextSides)
			{
				if (!processed.ContainsKey(pair.Key.Index))
					ApplyHorizontalOffset(pair.Key, side, true, processed);
			}

			// Repeat for PreviousSides
			foreach(KeyValuePair<SortedVisualSide, bool> pair in side.PreviousSides) 
			{
				if(!processed.ContainsKey(pair.Key.Index)) 
					ApplyHorizontalOffset(pair.Key, side, false, processed);
			}
		}

		#endregion
	}
}
