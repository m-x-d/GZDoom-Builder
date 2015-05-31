#region === Copyright (c) 2010 Pascal van der Heiden ===

using System;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal class Effect3DFloor : SectorEffect
	{
		// Linedef that is used to create this effect
		// The sector can be found by linedef.Front.Sector
		private Linedef linedef;
		
		// Floor and ceiling planes
		private SectorLevel floor;
		private SectorLevel ceiling;

		// Alpha transparency
		private int alpha;
		
		// Vavoom type?
		private bool vavoomtype;

		//mxd. Translucent 3d-floor?
		private bool renderinside;

		//mxd. Dirty hack to emulate GZDoom behaviour?
		private bool sloped3dfloor;

		//mxd. Ignore Bottom Height?
		private bool ignorebottomheight;

		// Properties
		public int Alpha { get { return alpha; } }
		public SectorLevel Floor { get { return floor; } }
		public SectorLevel Ceiling { get { return ceiling; } }
		public Linedef Linedef { get { return linedef; } }
		public bool VavoomType { get { return vavoomtype; } }
		public bool RenderInside { get { return renderinside; } } //mxd
		public bool IgnoreBottomHeight { get { return ignorebottomheight; } } //mxd
		public bool Sloped3dFloor { get { return sloped3dfloor; } } //mxd

		//mxd. 3D-Floor Flags
		[Flags]
		public enum Flags
		{
			None = 0,
			DisableLighting = 1,
			RestrictLighting = 2,
			Fog = 4,
			IgnoreBottomHeight = 8,
			UseUpperTexture = 16,
			UseLowerTexture = 32,
			RenderAdditive = 64
		}

		//mxd. 3D-Floor Types
		[Flags]
		public enum FloorTypes
		{
			VavoomStyle = 0,
			Solid = 1,
			Swimmable = 2,
			NonSolid = 3,
			RenderInside = 4,
			HiTagIsLineID = 8,
			InvertVisibilityRules = 16,
			InvertShootabilityRules = 32
		}
		
		// Constructor
		public Effect3DFloor(SectorData data, Linedef sourcelinedef) : base(data)
		{
			linedef = sourcelinedef;
			
			// New effect added: This sector needs an update!
			if(data.Mode.VisualSectorExists(data.Sector))
			{
				BaseVisualSector vs = (BaseVisualSector)data.Mode.GetVisualSector(data.Sector);
				vs.UpdateSectorGeometry(true);
			}
		}

		// This makes sure we are updated with the source linedef information
		public override void Update()
		{
			SectorData sd = data.Mode.GetSectorData(linedef.Front.Sector);
			if(!sd.Updated) sd.Update();
			sd.AddUpdateSector(data.Sector, true);

			if(floor == null)
			{
				floor = new SectorLevel(sd.Floor);
				data.AddSectorLevel(floor);
			}

			if(ceiling == null)
			{
				ceiling = new SectorLevel(sd.Ceiling);
				data.AddSectorLevel(ceiling);
			}

			// For non-vavoom types, we must switch the level types
			if(linedef.Args[1] != (int)FloorTypes.VavoomStyle)
			{
				vavoomtype = false;
				alpha = linedef.Args[3];
				sd.Ceiling.CopyProperties(floor);
				sd.Floor.CopyProperties(ceiling);
				floor.type = SectorLevelType.Floor;
				floor.plane = sd.Ceiling.plane.GetInverted();
				ceiling.type = SectorLevelType.Ceiling;
				ceiling.plane = sd.Floor.plane.GetInverted();

				//mxd. check for Swimmable/RenderInside setting
				renderinside = ( (((linedef.Args[1] & (int)FloorTypes.Swimmable) == (int)FloorTypes.Swimmable) && (linedef.Args[1] & (int)FloorTypes.NonSolid) != (int)FloorTypes.NonSolid) )
							|| ((linedef.Args[1] & (int)FloorTypes.RenderInside) == (int)FloorTypes.RenderInside);
				ignorebottomheight = ((linedef.Args[2] & (int)Flags.IgnoreBottomHeight) == (int)Flags.IgnoreBottomHeight);

				// A 3D floor's color is always that of the sector it is placed in
				floor.color = 0;
			}
			else
			{
				vavoomtype = true;
				floor.type = SectorLevelType.Ceiling;
				floor.plane = sd.Ceiling.plane;
				ceiling.type = SectorLevelType.Floor;
				ceiling.plane = sd.Floor.plane;
				alpha = 255;
				
				// A 3D floor's color is always that of the sector it is placed in
				ceiling.color = 0;
			}

			// Apply alpha
			floor.alpha = alpha;
			ceiling.alpha = alpha;

			//mxd. Check slopes, cause GZDoom can't handle sloped translucent 3d floors...
			sloped3dfloor = (alpha < 255 &&
							 (Angle2D.RadToDeg(ceiling.plane.Normal.GetAngleZ()) != 270 ||
							  Angle2D.RadToDeg(floor.plane.Normal.GetAngleZ()) != 90));
			
			// Do not adjust light? (works only for non-vavoom types)
			if(!vavoomtype)
			{
				bool disablelighting = ((linedef.Args[2] & (int) Flags.DisableLighting) == (int) Flags.DisableLighting); //mxd
				bool restrictlighting = alpha < 255 && ((linedef.Args[2] & (int) Flags.RestrictLighting) == (int) Flags.RestrictLighting); //mxd

				if(disablelighting || restrictlighting)
				{
					floor.brightnessbelow = -1;
					floor.restrictlighting = restrictlighting; //mxd
					floor.disablelighting = disablelighting; //mxd
					floor.colorbelow = PixelColor.FromInt(0);
					ceiling.color = 0;
					ceiling.brightnessbelow = -1;
					ceiling.disablelighting = true; //mxd
					ceiling.colorbelow = PixelColor.FromInt(0);
				}
			}
		}
	}
}
