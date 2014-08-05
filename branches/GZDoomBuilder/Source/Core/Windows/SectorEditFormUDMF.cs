using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.GZBuilder.Tools;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Geometry;

namespace CodeImp.DoomBuilder.Windows
{
	internal partial class SectorEditFormUDMF : DelayedForm
	{
		#region ================== Events

		public event EventHandler OnValuesChanged; //mxd

		#endregion

		#region ================== Variables

		private ICollection<Sector> sectors;
		private List<SectorProperties> sectorProps; //mxd
		private bool blockUpdate; //mxd
		private StepsList angleSteps; //mxd

		//mxd. Persistent settings
		private static bool linkCeilingScale;
		private static bool linkFloorScale;
		private static bool useFloorLineAngles;
		private static bool useCeilLineAngles;

		//mxd. Window setup stuff
		private static Point location = Point.Empty;
		private static int activeTab;

		//mxd. Slope stuff
		private static readonly string[] floorslopekeys = { "floorplane_a", "floorplane_b", "floorplane_c", "floorplane_d" };
		private static readonly string[] ceilslopekeys = { "ceilingplane_a", "ceilingplane_b", "ceilingplane_c", "ceilingplane_d" };
		
		//mxd. Slope pivots
		//private Vector3D ceilslopenormal;
		//private Vector3D floorslopenormal;
		private Vector3D globalceilslopepivot;
		private Vector3D globalfloorslopepivot;
		private List<Vector3D> ceilslopepivots;
		private List<Vector3D> floorslopepivots;
		//private static SlopePivotMode ceilpivotmode = SlopePivotMode.LOCAL; //dbg?
		//private static SlopePivotMode floorpivotmode = SlopePivotMode.LOCAL;

		private struct SectorProperties //mxd
		{
			public readonly int Brightness;
			public readonly int FloorHeight;
			public readonly int CeilHeight;
			public readonly string FloorTexture;
			public readonly string CeilTexture;

			//UDMF stuff
			public readonly int LightColor;
			public readonly int FadeColor;
			//public float Desaturation;

			//UDMF Ceiling
			public readonly float CeilOffsetX;
			public readonly float CeilOffsetY;
			public readonly float CeilScaleX;
			public readonly float CeilScaleY;
			//public float CeilAlpha;
			public readonly float CeilRotation;
			public readonly int CeilBrightness;
			public readonly bool CeilLightAbsoulte;

			//UDMF Floor
			public readonly float FloorOffsetX;
			public readonly float FloorOffsetY;
			public readonly float FloorScaleX;
			public readonly float FloorScaleY;
			//public float FloorAlpha;
			public readonly float FloorRotation;
			public readonly int FloorBrightness;
			public readonly bool FloorLightAbsoulte;

			//UDMF floor slope
			public readonly float FloorSlopeX;
			public readonly float FloorSlopeY;
			public readonly float FloorSlopeZ;
			public readonly float FloorSlopeOffset;

			//UDMF ceiling slope
			public readonly float CeilSlopeX;
			public readonly float CeilSlopeY;
			public readonly float CeilSlopeZ;
			public readonly float CeilSlopeOffset;

			public SectorProperties(Sector s) {
				Brightness = s.Brightness;
				FloorHeight = s.FloorHeight;
				CeilHeight = s.CeilHeight;
				FloorTexture = s.FloorTexture;
				CeilTexture = s.CeilTexture;

				//UDMF stuff
				LightColor = UDMFTools.GetInteger(s.Fields, "lightcolor", 16777215);
				FadeColor = UDMFTools.GetInteger(s.Fields, "fadecolor", 0);

				//UDMF Ceiling
				CeilOffsetX = UDMFTools.GetFloat(s.Fields, "xpanningceiling", 0f);
				CeilOffsetY = UDMFTools.GetFloat(s.Fields, "ypanningceiling", 0f);
				CeilScaleX = UDMFTools.GetFloat(s.Fields, "xscaleceiling", 1.0f);
				CeilScaleY = UDMFTools.GetFloat(s.Fields, "yscaleceiling", 1.0f);
				//CeilAlpha = UDMFTools.GetFloat(s.Fields, "alphaceiling", 1.0f);
				CeilRotation = s.Fields.GetValue("rotationceiling", 0.0f);
				CeilBrightness = s.Fields.GetValue("lightceiling", 0);
				CeilLightAbsoulte = s.Fields.GetValue("lightceilingabsolute", false);

				//UDMF Floor
				FloorOffsetX = UDMFTools.GetFloat(s.Fields, "xpanningfloor", 0f);
				FloorOffsetY = UDMFTools.GetFloat(s.Fields, "ypanningfloor", 0f);
				FloorScaleX = UDMFTools.GetFloat(s.Fields, "xscalefloor", 1.0f);
				FloorScaleY = UDMFTools.GetFloat(s.Fields, "yscalefloor", 1.0f);
				//FloorAlpha = UDMFTools.GetFloat(s.Fields, "alphafloor", 1.0f);
				FloorRotation = s.Fields.GetValue("rotationfloor", 0.0f);
				FloorBrightness = s.Fields.GetValue("lightfloor", 0);
				FloorLightAbsoulte = s.Fields.GetValue("lightfloorabsolute", false);

				//UDMF Ceiling slope
				CeilSlopeX = UDMFTools.GetFloat(s.Fields, "ceilingplane_a", 0f);
				CeilSlopeY = UDMFTools.GetFloat(s.Fields, "ceilingplane_b", 0f);
				CeilSlopeZ = UDMFTools.GetFloat(s.Fields, "ceilingplane_c", 0f);
				CeilSlopeOffset = UDMFTools.GetFloat(s.Fields, "ceilingplane_d", 0f);

				//UDMF Floor slope
				FloorSlopeX = UDMFTools.GetFloat(s.Fields, "floorplane_a", 0f);
				FloorSlopeY = UDMFTools.GetFloat(s.Fields, "floorplane_b", 0f);
				FloorSlopeZ = UDMFTools.GetFloat(s.Fields, "floorplane_c", 0f);
				FloorSlopeOffset = UDMFTools.GetFloat(s.Fields, "floorplane_d", 0f);
			}
		}

		#endregion

		#region ================== Constructor

		public SectorEditFormUDMF() {
			InitializeComponent();

#if !DEBUG //TODO: implement this!
			tabs.TabPages.Remove(tabslopes);
#endif

			//mxd. Widow setup
			if(location != Point.Empty) {
				this.StartPosition = FormStartPosition.Manual;
				this.Location = location;
				if(activeTab > 0) tabs.SelectTab(activeTab);
			}

			// Fill flags list
			foreach(KeyValuePair<string, string> lf in General.Map.Config.SectorFlags)
				flags.Add(lf.Value, lf.Key);

			//mxd. Fill renderstyles
			foreach(KeyValuePair<string, string> lf in General.Map.Config.SectorRenderStyles) {
				floorRenderStyle.Items.Add(lf.Value);
				ceilRenderStyle.Items.Add(lf.Value);
			}

			// Fill effects list
			effect.AddInfo(General.Map.Config.SortedSectorEffects.ToArray());

			// Initialize custom fields editor
			fieldslist.Setup("sector");

			// Fill universal fields list
			fieldslist.ListFixedFields(General.Map.Config.SectorFields);

			// Initialize image selectors
			floortex.Initialize();
			ceilingtex.Initialize();

			// Set steps for brightness field
			brightness.StepValues = General.Map.Config.BrightnessLevels;

			// Value linking
			ceilScale.LinkValues = linkCeilingScale;
			floorScale.LinkValues = linkFloorScale;

			cbUseCeilLineAngles.Checked = useCeilLineAngles;
			cbUseFloorLineAngles.Checked = useFloorLineAngles;

			// Slopes stuff
			//ceilslopenormal = new Vector3D(0f, 0f, -1f);
			//floorslopenormal = new Vector3D(0f, 0f, 1f);
			globalceilslopepivot = new Vector3D();
			globalfloorslopepivot = new Vector3D();

			//TODO: set stored pivot mode
		}

		#endregion

		#region ================== Methods

		// This sets up the form to edit the given sectors
		public void Setup(ICollection<Sector> sectors) {
			blockUpdate = true; //mxd

			// Keep this list
			this.sectors = sectors;
			if(sectors.Count > 1) this.Text = "Edit Sectors (" + sectors.Count + ")";
			sectorProps = new List<SectorProperties>(); //mxd

			//mxd. Make undo
			string undodesc = "sector";
			if(sectors.Count > 1) undodesc = sectors.Count + " sectors";
			General.Map.UndoRedo.CreateUndo("Edit " + undodesc);

			////////////////////////////////////////////////////////////////////////
			// Set all options to the first sector properties
			////////////////////////////////////////////////////////////////////////

			// Get first sector
			Sector sc = General.GetByIndex(sectors, 0);

			// Flags
			foreach(CheckBox c in flags.Checkboxes)
				if(sc.Flags.ContainsKey(c.Tag.ToString())) c.Checked = sc.Flags[c.Tag.ToString()];

			// Effects
			effect.Value = sc.Effect;
			brightness.Text = sc.Brightness.ToString();

			// Floor/ceiling
			floorheight.Text = sc.FloorHeight.ToString();
			ceilingheight.Text = sc.CeilHeight.ToString();
			floortex.TextureName = sc.FloorTexture;
			ceilingtex.TextureName = sc.CeilTexture;

			//UDMF stuff
			//Texture offsets
			ceilOffsets.SetValuesFrom(sc.Fields, true);
			floorOffsets.SetValuesFrom(sc.Fields, true);

			//Texture scale
			ceilScale.SetValuesFrom(sc.Fields, true);
			floorScale.SetValuesFrom(sc.Fields, true);

			//Texture rotation
			float ceilAngle = sc.Fields.GetValue("rotationceiling", 0.0f);
			float floorAngle = sc.Fields.GetValue("rotationfloor", 0.0f);

			ceilRotation.Text = ceilAngle.ToString();
			floorRotation.Text = floorAngle.ToString();

			ceilAngleControl.Angle = General.ClampAngle(360 - (int)ceilAngle);
			floorAngleControl.Angle = General.ClampAngle(360 - (int)floorAngle);

			//Texture brightness
			ceilBrightness.Text = sc.Fields.GetValue("lightceiling", 0).ToString();
			floorBrightness.Text = sc.Fields.GetValue("lightfloor", 0).ToString();
			ceilLightAbsolute.Checked = sc.Fields.GetValue("lightceilingabsolute", false);
			floorLightAbsolute.Checked = sc.Fields.GetValue("lightfloorabsolute", false);

			//Alpha
			ceilAlpha.Text = General.Clamp(sc.Fields.GetValue("alphaceiling", 1.0f), 0f, 1f).ToString();
			floorAlpha.Text = General.Clamp(sc.Fields.GetValue("alphafloor", 1.0f), 0f, 1f).ToString();

			//Render style
			string[] rskeys = null;
			if (General.Map.Config.SectorRenderStyles.Count > 0) {
				rskeys = new string[General.Map.Config.SectorRenderStyles.Count];
				General.Map.Config.SectorRenderStyles.Keys.CopyTo(rskeys, 0);
				ceilRenderStyle.SelectedIndex = Array.IndexOf(rskeys, sc.Fields.GetValue("renderstyleceiling", "translucent"));
				floorRenderStyle.SelectedIndex = Array.IndexOf(rskeys, sc.Fields.GetValue("renderstylefloor", "translucent"));
			} else {
				ceilRenderStyle.Enabled = false;
				floorRenderStyle.Enabled = false;
			}

			//Misc
			soundSequence.Text = sc.Fields.GetValue("soundsequence", string.Empty);
			gravity.Text = sc.Fields.GetValue("gravity", 1.0f).ToString();
			desaturation.Text = General.Clamp(sc.Fields.GetValue("desaturation", 0.0f), 0f, 1f).ToString();

			//Sector colors
			fadeColor.SetValueFrom(sc.Fields);
			lightColor.SetValueFrom(sc.Fields);

			//Slopes
			ceilslopex.Text = sc.Fields.GetValue("ceilingplane_a", 0f).ToString();
			ceilslopey.Text = sc.Fields.GetValue("ceilingplane_b", 0f).ToString();
			ceilslopez.Text = sc.Fields.GetValue("ceilingplane_c", 0f).ToString();
			ceilslopeoffset.Text = sc.Fields.GetValue("ceilingplane_d", 0f).ToString();

			floorslopex.Text = sc.Fields.GetValue("floorplane_a", 0f).ToString();
			floorslopey.Text = sc.Fields.GetValue("floorplane_b", 0f).ToString();
			floorslopez.Text = sc.Fields.GetValue("floorplane_c", 0f).ToString();
			floorslopeoffset.Text = sc.Fields.GetValue("floorplane_d", 0f).ToString();

			// Action
			tagSelector.Setup(UniversalType.SectorTag); //mxd
			tagSelector.SetTag(sc.Tag);//mxd

			// Custom fields
			fieldslist.SetValues(sc.Fields, true);

			angleSteps = new StepsList();

			floorslopepivots = new List<Vector3D>(sectors.Count);
			ceilslopepivots = new List<Vector3D>(sectors.Count);

			////////////////////////////////////////////////////////////////////////
			// Now go for all sectors and change the options when a setting is different
			////////////////////////////////////////////////////////////////////////

			// Go for all sectors
			foreach(Sector s in sectors) {
				// Flags
				foreach(CheckBox c in flags.Checkboxes) {
					if(c.CheckState == CheckState.Indeterminate) continue; //mxd
					if(s.IsFlagSet(c.Tag.ToString()) != c.Checked) {
						c.ThreeState = true;
						c.CheckState = CheckState.Indeterminate;
					}
				}

				// Effects
				if(s.Effect != effect.Value) effect.Empty = true;
				if(s.Brightness.ToString() != brightness.Text) brightness.Text = "";

				// Floor/Ceiling
				if(s.FloorHeight.ToString() != floorheight.Text) floorheight.Text = "";
				if(s.CeilHeight.ToString() != ceilingheight.Text) ceilingheight.Text = "";
				if (s.FloorTexture != floortex.TextureName) {
					floortex.MultipleTextures = true; //mxd
					floortex.TextureName = "";
				}
				if (s.CeilTexture != ceilingtex.TextureName) {
					ceilingtex.MultipleTextures = true; //mxd
					ceilingtex.TextureName = "";
				}

				//mxd. UDMF stuff
				//Texture offsets
				ceilOffsets.SetValuesFrom(s.Fields, false);
				floorOffsets.SetValuesFrom(s.Fields, false);

				//Texture scale
				ceilScale.SetValuesFrom(s.Fields, false);
				floorScale.SetValuesFrom(s.Fields, false);

				//Texture rotation
				if(s.Fields.GetValue("rotationceiling", 0.0f).ToString() != ceilRotation.Text) {
					ceilRotation.Text = "";
					ceilAngleControl.Angle = 0;
				}
				if(s.Fields.GetValue("rotationfloor", 0.0f).ToString() != floorRotation.Text) {
					floorRotation.Text = "";
					floorAngleControl.Angle = 0;
				}

				//Texture brightness
				if(s.Fields.GetValue("lightceiling", 0).ToString() != ceilBrightness.Text) ceilBrightness.Text = "";
				if(s.Fields.GetValue("lightfloor", 0).ToString() != floorBrightness.Text) floorBrightness.Text = "";

				if(s.Fields.GetValue("lightceilingabsolute", false) != ceilLightAbsolute.Checked) {
					ceilLightAbsolute.ThreeState = true;
					ceilLightAbsolute.CheckState = CheckState.Indeterminate;
				}
				if(s.Fields.GetValue("lightfloorabsolute", false) != floorLightAbsolute.Checked) {
					floorLightAbsolute.ThreeState = true;
					floorLightAbsolute.CheckState = CheckState.Indeterminate;
				}

				//Alpha
				if(s.Fields.GetValue("alphaceiling", 1.0f).ToString() != ceilAlpha.Text) ceilAlpha.Text = "";
				if(s.Fields.GetValue("alphafloor", 1.0f).ToString() != floorAlpha.Text) floorAlpha.Text = "";

				//Render style
				if (rskeys != null) {
					if (ceilRenderStyle.SelectedIndex > -1 && ceilRenderStyle.SelectedIndex != Array.IndexOf(rskeys, s.Fields.GetValue("renderstyleceiling", "translucent")))
						ceilRenderStyle.SelectedIndex = -1;
					if (floorRenderStyle.SelectedIndex > -1 && floorRenderStyle.SelectedIndex != Array.IndexOf(rskeys, s.Fields.GetValue("renderstylefloor", "translucent")))
						floorRenderStyle.SelectedIndex = -1;
				}

				//Misc
				if(s.Fields.GetValue("soundsequence", string.Empty) != soundSequence.Text) soundSequence.Text = "";
				if(s.Fields.GetValue("gravity", 1.0f).ToString() != gravity.Text) gravity.Text = "";
				if(s.Fields.GetValue("desaturation", 0.0f).ToString() != desaturation.Text) desaturation.Text = "";

				//Sector colors
				fadeColor.SetValueFrom(s.Fields);
				lightColor.SetValueFrom(s.Fields);

				//Slopes
				if(s.Fields.GetValue("ceilingplane_a", 0f).ToString() != ceilslopex.Text) ceilslopex.Text = "";
				if(s.Fields.GetValue("ceilingplane_b", 0f).ToString() != ceilslopey.Text) ceilslopey.Text = "";
				if(s.Fields.GetValue("ceilingplane_c", 0f).ToString() != ceilslopez.Text) ceilslopez.Text = "";
				if(s.Fields.GetValue("ceilingplane_d", 0f).ToString() != ceilslopeoffset.Text) ceilslopeoffset.Text = "";

				if(s.Fields.GetValue("floorplane_a", 0f).ToString() != floorslopex.Text) floorslopex.Text = "";
				if(s.Fields.GetValue("floorplane_b", 0f).ToString() != floorslopey.Text) floorslopey.Text = "";
				if(s.Fields.GetValue("floorplane_c", 0f).ToString() != floorslopez.Text) floorslopez.Text = "";
				if(s.Fields.GetValue("floorplane_d", 0f).ToString() != floorslopeoffset.Text) floorslopeoffset.Text = "";

				// Action
				if(s.Tag != sc.Tag) tagSelector.ClearTag(); //mxd

				// Custom fields
				s.Fields.BeforeFieldsChange(); //mxd
				fieldslist.SetValues(s.Fields, false);

				//mxd. Store initial properties
				sectorProps.Add(new SectorProperties(s));

				//mxd. Angle steps
				int angle;
				foreach(Sidedef side in s.Sidedefs){
					if (side.Line.Front != null && side.Index == side.Line.Front.Index)
						angle = General.ClampAngle(270 - side.Line.AngleDeg);
					else
						angle = General.ClampAngle(90 - side.Line.AngleDeg);

					if(!angleSteps.Contains(angle)) angleSteps.Add(angle);
				}

				//Slope pivots
				Vector3D ceilpivot = new Vector3D(s.BBox.X + s.BBox.Width / 2, s.BBox.Y + s.BBox.Height / 2, s.CeilHeight);
				Vector3D floorpivot = new Vector3D(s.BBox.X + s.BBox.Width / 2, s.BBox.Y + s.BBox.Height / 2, s.FloorHeight);

				globalfloorslopepivot += floorpivot;
				globalceilslopepivot += ceilpivot;

				floorslopepivots.Add(floorpivot);
				ceilslopepivots.Add(ceilpivot);
			}

			globalfloorslopepivot /= sectors.Count;
			globalceilslopepivot /= sectors.Count;

			//mxd. Set ceiling slope controls
			if (!string.IsNullOrEmpty(ceilslopex.Text) && !string.IsNullOrEmpty(ceilslopey.Text) && !string.IsNullOrEmpty(ceilslopez.Text)) {
				Vector3D v = new Vector3D(ceilslopex.GetResultFloat(0f), ceilslopey.GetResultFloat(0f), ceilslopez.GetResultFloat(0f));
				if (v.x != 0 || v.y != 0 || v.z != 0) {
					ceilslopeangle.Value = (int) Math.Round(Angle2D.RadToDeg(v.GetAngleXY()));
					ceilsloperoll.Value = (int) Math.Round(Angle2D.RadToDeg(v.GetAngleZ()));
					ceilslopeanglelabel.Text = ceilslopeangle.Value + "\u00B0";
					ceilsloperolllabel.Text = ceilsloperoll.Value + "\u00B0";
				}
			}

			//mxd. Set floor slope controls
			if(!string.IsNullOrEmpty(floorslopex.Text) && !string.IsNullOrEmpty(floorslopey.Text) && !string.IsNullOrEmpty(floorslopez.Text)) {
				Vector3D v = new Vector3D(floorslopex.GetResultFloat(0f), floorslopey.GetResultFloat(0f), floorslopez.GetResultFloat(0f));
				if(v.x != 0 || v.y != 0 || v.z != 0) {
					/*ceilslopeangle.Value = General.ClampAngle((int)Math.Round(Angle2D.RadToDeg(v.GetAngleXY())));
					ceilsloperoll.Value = (int)Math.Round(Angle2D.RadToDeg(v.GetAngleZ()));
					ceilslopeanglelabel.Text = ceilslopeangle.Value + "\u00B0";
					ceilsloperolllabel.Text = ceilsloperoll.Value + "\u00B0";*/
					floorsloperotation.Text = ((int)Math.Round(Angle2D.RadToDeg(v.GetAngleXY()))).ToString();
					floorslopeangle.Text = ((int)Math.Round(Angle2D.RadToDeg(v.GetAngleZ()))).ToString();
				}
			}

			// Show sector height
			UpdateSectorHeight();

			//mxd. Angle steps
			angleSteps.Sort();
			if(useCeilLineAngles) ceilRotation.StepValues = angleSteps;
			if(useFloorLineAngles) floorRotation.StepValues = angleSteps;

			blockUpdate = false; //mxd
		}

		// This updates the sector height field
		private void UpdateSectorHeight() {
			int delta = 0;
			int index = -1; //mxd
			int i = 0; //mxd

			// Check all selected sectors
			foreach(Sector s in sectors) {
				if(index == -1) {
					// First sector in list
					delta = s.CeilHeight - s.FloorHeight;
					index = i; //mxd
				} else if(delta != (s.CeilHeight - s.FloorHeight)) {
					// We can't show heights because the delta
					// heights for the sectors is different
					index = -1;
					break;
				}

				i++;
			}

			if(index > -1) {
				int fh = floorheight.GetResult(sectorProps[index].FloorHeight); //mxd
				int ch = ceilingheight.GetResult(sectorProps[index].CeilHeight); //mxd
				int height = ch - fh;
				sectorheight.Text = height.ToString();
				sectorheight.Visible = true;
				sectorheightlabel.Visible = true;
			} else {
				sectorheight.Visible = false;
				sectorheightlabel.Visible = false;
			}
		}

		#endregion

		#region ================== Events

		private void apply_Click(object sender, EventArgs e) {
			// Verify the tag
			tagSelector.ValidateTag(); //mxd
			if((tagSelector.GetTag(0) < General.Map.FormatInterface.MinTag) || (tagSelector.GetTag(0) > General.Map.FormatInterface.MaxTag)) {
				General.ShowWarningMessage("Sector tag must be between " + General.Map.FormatInterface.MinTag + " and " + General.Map.FormatInterface.MaxTag + ".", MessageBoxButtons.OK);
				return;
			}

			// Verify the effect
			if((effect.Value < General.Map.FormatInterface.MinEffect) || (effect.Value > General.Map.FormatInterface.MaxEffect)) {
				General.ShowWarningMessage("Sector effect must be between " + General.Map.FormatInterface.MinEffect + " and " + General.Map.FormatInterface.MaxEffect + ".", MessageBoxButtons.OK);
				return;
			}

			//mxd
			string[] rskeys = null;
			if(General.Map.Config.SectorRenderStyles.Count > 0) {
				rskeys = new string[General.Map.Config.SectorRenderStyles.Count];
				General.Map.Config.SectorRenderStyles.Keys.CopyTo(rskeys, 0);
			}

			// Go for all sectors
			foreach(Sector s in sectors) {
				// Apply all flags
				foreach(CheckBox c in flags.Checkboxes) {
					if(c.CheckState == CheckState.Checked) s.SetFlag(c.Tag.ToString(), true);
					else if(c.CheckState == CheckState.Unchecked) s.SetFlag(c.Tag.ToString(), false);
				}

				// Effects
				if(!effect.Empty) s.Effect = effect.Value;
				s.Brightness = General.Clamp(brightness.GetResult(s.Brightness), General.Map.FormatInterface.MinBrightness, General.Map.FormatInterface.MaxBrightness);

				// Action
				s.Tag = tagSelector.GetTag(s.Tag); //mxd

				//Fields
				fieldslist.Apply(s.Fields);

				//alpha
				if(!string.IsNullOrEmpty(ceilAlpha.Text)) {
					float ceilAlphaVal = General.Clamp(ceilAlpha.GetResultFloat(s.Fields.GetValue("alphaceiling", 1.0f)), 0f, 1f);
					UDMFTools.SetFloat(s.Fields, "alphaceiling", ceilAlphaVal, 1.0f);
				}
				
				if(!string.IsNullOrEmpty(floorAlpha.Text)){
					float floorAlphaVal = General.Clamp(floorAlpha.GetResultFloat(s.Fields.GetValue("alphafloor", 1.0f)), 0f, 1f);
					UDMFTools.SetFloat(s.Fields, "alphafloor", floorAlphaVal, 1.0f);
				}

				//renderstyle
				if (rskeys != null) {
					if (ceilRenderStyle.SelectedIndex > -1) {
						UDMFTools.SetString(s.Fields, "renderstyleceiling", rskeys[ceilRenderStyle.SelectedIndex], "translucent");
					}
					if (floorRenderStyle.SelectedIndex > -1) {
						UDMFTools.SetString(s.Fields, "renderstylefloor", rskeys[floorRenderStyle.SelectedIndex], "translucent");
					}
				}

				//misc
				if(soundSequence.Text != "") s.Fields["soundsequence"] = new UniValue(UniversalType.String, soundSequence.Text);
				if(gravity.Text != "")
					UDMFTools.SetFloat(s.Fields, "gravity", gravity.GetResultFloat(s.Fields.GetValue("gravity", 1.0f)), 1.0f);
				if(desaturation.Text != "") {
					float val = General.Clamp(desaturation.GetResultFloat(s.Fields.GetValue("desaturation", 0f)), 0f, 1f);
					UDMFTools.SetFloat(s.Fields, "desaturation", val, 0f);
				}

				//clear slope props if all values are 0
				if(UDMFTools.GetFloat(s.Fields, ceilslopekeys[0]) == 0 && UDMFTools.GetFloat(s.Fields, ceilslopekeys[1]) == 0
					&& UDMFTools.GetFloat(s.Fields, ceilslopekeys[2]) == 0) {
					UDMFTools.ClearFields(s.Fields, ceilslopekeys);
					s.UpdateNeeded = true;
				}

				if(UDMFTools.GetFloat(s.Fields, floorslopekeys[0]) == 0 && UDMFTools.GetFloat(s.Fields, floorslopekeys[1]) == 0
					&& UDMFTools.GetFloat(s.Fields, floorslopekeys[2]) == 0) {
					UDMFTools.ClearFields(s.Fields, floorslopekeys);
					s.UpdateNeeded = true;
				}
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();

			// Store value linking
			linkCeilingScale = ceilScale.LinkValues;
			linkFloorScale = floorScale.LinkValues;
			useCeilLineAngles = cbUseCeilLineAngles.Checked;
			useFloorLineAngles = cbUseFloorLineAngles.Checked;

			// Done
			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty); //mxd
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void cancel_Click(object sender, EventArgs e) {
			//mxd. Let's pretend nothing of this really happened...
			General.Map.UndoRedo.WithdrawUndo();
			
			// Be gone
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void browseeffect_Click(object sender, EventArgs e) {
			effect.Value = EffectBrowserForm.BrowseEffect(this, effect.Value);
		}

		//mxd
		private void SectorEditFormUDMF_FormClosing(object sender, FormClosingEventArgs e) {
			location = this.Location;
			activeTab = tabs.SelectedIndex;
		}

		private void SectorEditFormUDMF_HelpRequested(object sender, HelpEventArgs hlpevent) {
			General.ShowHelp("w_sectoredit.html");
			hlpevent.Handled = true;
		}

		private void tabcustom_MouseEnter(object sender, EventArgs e) {
			fieldslist.Focus();
		}

		private void ceilAngleControl_AngleChanged() {
			ceilRotation.Text = (General.ClampAngle(360 - ceilAngleControl.Angle)).ToString();
		}

		private void floorAngleControl_AngleChanged() {
			floorRotation.Text = (General.ClampAngle(360 - floorAngleControl.Angle)).ToString();
		}

		private void cbUseCeilLineAngles_CheckedChanged(object sender, EventArgs e) {
			ceilRotation.StepValues = (cbUseCeilLineAngles.Checked ? angleSteps : null);
		}

		private void cbUseFloorLineAngles_CheckedChanged(object sender, EventArgs e) {
			floorRotation.StepValues = (cbUseFloorLineAngles.Checked ? angleSteps : null);
		}

		#endregion

		#region mxd. Sector Realtime events

		private void ceilingheight_WhenTextChanged(object sender, EventArgs e) {
			if(blockUpdate)	return;
			int i = 0;

			//restore values
			if(string.IsNullOrEmpty(ceilingheight.Text)) {
				foreach(Sector s in sectors)
					s.CeilHeight = sectorProps[i++].CeilHeight;
			//update values
			} else {
				foreach(Sector s in sectors)
					s.CeilHeight = ceilingheight.GetResult(sectorProps[i++].CeilHeight);
			}

			UpdateSectorHeight();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void floorheight_WhenTextChanged(object sender, EventArgs e) {
			if(blockUpdate)	return;
			int i = 0;

			//restore values
			if(string.IsNullOrEmpty(floorheight.Text)) {
				foreach(Sector s in sectors)
					s.FloorHeight = sectorProps[i++].FloorHeight;
			//update values
			} else {
				foreach(Sector s in sectors)
					s.FloorHeight = floorheight.GetResult(sectorProps[i++].FloorHeight);
			}

			UpdateSectorHeight();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void brightness_WhenTextChanged(object sender, EventArgs e) {
			if(blockUpdate)	return;
			int i = 0;

			//restore values
			if(string.IsNullOrEmpty(brightness.Text)) {
				foreach(Sector s in sectors)
					s.Brightness = sectorProps[i++].Brightness;
			//update values
			} else {
				//clamp value?
				int val = brightness.GetResult(0);
				int clampedVal = General.Clamp(val, 0, 255);
				if(val != clampedVal) {
					brightness.Text = clampedVal.ToString();
					return;
				}

				foreach(Sector s in sectors)
					s.Brightness = General.Clamp(brightness.GetResult(sectorProps[i++].Brightness), General.Map.FormatInterface.MinBrightness, General.Map.FormatInterface.MaxBrightness);
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void ceilingtex_OnValueChanged(object sender, EventArgs e) {
			if(blockUpdate)	return;

			//restore values
			if(string.IsNullOrEmpty(ceilingtex.TextureName)) {
				int i = 0;

				foreach(Sector s in sectors)
					s.SetCeilTexture(sectorProps[i++].CeilTexture);
			//update values
			} else {
				foreach(Sector s in sectors)
					s.SetCeilTexture(ceilingtex.GetResult(s.CeilTexture));
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void floortex_OnValueChanged(object sender, EventArgs e) {
			if(blockUpdate)	return;

			//restore values
			if(string.IsNullOrEmpty(floortex.TextureName)) {
				int i = 0;

				foreach(Sector s in sectors)
					s.SetFloorTexture(sectorProps[i++].FloorTexture);
			//update values
			} else {
				foreach(Sector s in sectors)
					s.SetFloorTexture(floortex.GetResult(s.FloorTexture));
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void floorRotation_WhenTextChanged(object sender, EventArgs e) {
			floorAngleControl.Angle = (int)General.ClampAngle(360 - floorRotation.GetResultFloat(0));

			if(blockUpdate)	return;
			int i = 0;

			//restore values
			if(string.IsNullOrEmpty(floorRotation.Text)) {
				foreach(Sector s in sectors) {
					UDMFTools.SetFloat(s.Fields, "rotationfloor", sectorProps[i++].FloorRotation, 0f);
					s.UpdateNeeded = true;
				}
			//update values
			} else {
				foreach(Sector s in sectors) {
					UDMFTools.SetFloat(s.Fields, "rotationfloor", floorRotation.GetResultFloat(sectorProps[i++].FloorRotation), 0f);
					s.UpdateNeeded = true;
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void ceilRotation_WhenTextChanged(object sender, EventArgs e) {
			ceilAngleControl.Angle = (int)General.ClampAngle(360 - ceilRotation.GetResultFloat(0));

			if(blockUpdate)	return;
			int i = 0;

			//restore values
			if(string.IsNullOrEmpty(ceilRotation.Text)) {
				foreach(Sector s in sectors) {
					UDMFTools.SetFloat(s.Fields, "rotationceiling", sectorProps[i++].CeilRotation, 0f);
					s.UpdateNeeded = true;
				}
			//update values
			} else {
				foreach(Sector s in sectors) {
					UDMFTools.SetFloat(s.Fields, "rotationceiling", ceilRotation.GetResultFloat(sectorProps[i++].CeilRotation), 0f);
					s.UpdateNeeded = true;
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void lightColor_OnValueChanged(object sender, EventArgs e) {
			if(blockUpdate)	return;
			int i = 0;

			foreach(Sector s in sectors) {
				lightColor.ApplyTo(s.Fields, sectorProps[i++].LightColor);
				s.UpdateNeeded = true;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void fadeColor_OnValueChanged(object sender, EventArgs e) {
			if(blockUpdate)	return;
			int i = 0;

			foreach(Sector s in sectors) {
				fadeColor.ApplyTo(s.Fields, sectorProps[i++].FadeColor);
				s.UpdateNeeded = true;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		#endregion

		#region mxd. Ceiling/Floor realtime events

		private void ceilOffsets_OnValuesChanged(object sender, EventArgs e) {
			if(blockUpdate)	return;
			int i = 0;

			foreach(Sector s in sectors) {
				ceilOffsets.ApplyTo(s.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, sectorProps[i].CeilOffsetX, sectorProps[i++].CeilOffsetY);
				s.UpdateNeeded = true;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void floorOffsets_OnValuesChanged(object sender, EventArgs e) {
			if(blockUpdate)	return;
			int i = 0;

			foreach(Sector s in sectors) {
				floorOffsets.ApplyTo(s.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, sectorProps[i].FloorOffsetX, sectorProps[i++].FloorOffsetY);
				s.UpdateNeeded = true;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void ceilScale_OnValuesChanged(object sender, EventArgs e) {
			if(blockUpdate)	return;

			int i = 0;

			foreach(Sector s in sectors) {
				ceilScale.ApplyTo(s.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, sectorProps[i].CeilScaleX, sectorProps[i++].CeilScaleY);
				s.UpdateNeeded = true;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void floorScale_OnValuesChanged(object sender, EventArgs e) {
			if(blockUpdate)	return;
			int i = 0;

			foreach(Sector s in sectors) {
				floorScale.ApplyTo(s.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, sectorProps[i].FloorScaleX, sectorProps[i++].FloorScaleY);
				s.UpdateNeeded = true;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void ceilBrightness_WhenTextChanged(object sender, EventArgs e) {
			if(blockUpdate)	return;
			int i = 0;

			//restore values
			if(string.IsNullOrEmpty(ceilBrightness.Text)) {
				foreach(Sector s in sectors) {
					UDMFTools.SetInteger(s.Fields, "lightceiling", sectorProps[i++].CeilBrightness, 0);
					s.UpdateNeeded = true;
				}
			//update values
			} else {
				foreach(Sector s in sectors) {
					bool absolute = false;
					if(ceilLightAbsolute.CheckState == CheckState.Indeterminate) {
						absolute = s.Fields.GetValue("lightceilingabsolute", false);
					} else if(ceilLightAbsolute.CheckState == CheckState.Checked) {
						absolute = true;
					}

					int value = General.Clamp(ceilBrightness.GetResult(sectorProps[i++].CeilBrightness), (absolute ? 0 : -255), 255);
					UDMFTools.SetInteger(s.Fields, "lightceiling", value, 0);
					s.UpdateNeeded = true;
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void floorBrightness_WhenTextChanged(object sender, EventArgs e) {
			if(blockUpdate)	return;
			int i = 0;

			//restore values
			if(string.IsNullOrEmpty(floorBrightness.Text)) {
				foreach(Sector s in sectors) {
					UDMFTools.SetInteger(s.Fields, "lightfloor", sectorProps[i++].FloorBrightness, 0);
					s.UpdateNeeded = true;
				}
			//update values
			} else {
				foreach(Sector s in sectors) {
					bool absolute = false;
					if(floorLightAbsolute.CheckState == CheckState.Indeterminate) {
						absolute = s.Fields.GetValue("lightfloorabsolute", false);
					} else if(floorLightAbsolute.CheckState == CheckState.Checked) {
						absolute = true;
					}

					int value = General.Clamp(floorBrightness.GetResult(sectorProps[i++].FloorBrightness), (absolute ? 0 : -255), 255);
					UDMFTools.SetInteger(s.Fields, "lightfloor", value, 0);
					s.UpdateNeeded = true;
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void ceilLightAbsolute_CheckedChanged(object sender, EventArgs e) {
			if(blockUpdate)	return;

			if(ceilLightAbsolute.Checked) {
				foreach(Sector s in sectors) {
					s.Fields["lightceilingabsolute"] = new UniValue(UniversalType.Boolean, true);
					s.UpdateNeeded = true;
				}
			} else if(ceilLightAbsolute.CheckState == CheckState.Indeterminate) {
				int i = 0;
				
				foreach(Sector s in sectors) {
					if(sectorProps[i].CeilLightAbsoulte) {
						s.Fields["lightceilingabsolute"] = new UniValue(UniversalType.Boolean, true);
						s.UpdateNeeded = true;
					}else if(s.Fields.ContainsKey("lightceilingabsolute")) {
						s.Fields.Remove("lightceilingabsolute");
						s.UpdateNeeded = true;
					}

					i++;
				}
			} else {
				foreach(Sector s in sectors) {
					if(s.Fields.ContainsKey("lightceilingabsolute")) {
						s.Fields.Remove("lightceilingabsolute");
						s.UpdateNeeded = true;
					}
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void floorLightAbsolute_CheckedChanged(object sender, EventArgs e) {
			if(blockUpdate)	return;

			if(floorLightAbsolute.Checked){
				foreach(Sector s in sectors) {
					s.Fields["lightfloorabsolute"] = new UniValue(UniversalType.Boolean, true);
					s.UpdateNeeded = true;
				}
			} else if(floorLightAbsolute.CheckState == CheckState.Indeterminate) {
				int i = 0;

				foreach(Sector s in sectors) {
					if(sectorProps[i].FloorLightAbsoulte) {
						s.Fields["lightfloorabsolute"] = new UniValue(UniversalType.Boolean, true);
						s.UpdateNeeded = true;
					} else if(s.Fields.ContainsKey("lightfloorabsolute")) {
						s.Fields.Remove("lightfloorabsolute");
						s.UpdateNeeded = true;
					}

					i++;
				}
			} else {
				foreach(Sector s in sectors) {
					if(s.Fields.ContainsKey("lightfloorabsolute")) {
						s.Fields.Remove("lightfloorabsolute");
						s.UpdateNeeded = true;
					}
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		#endregion

		#region mxd. Floor slope realtime events

		private void resetfloorslope_Click(object sender, EventArgs e) {
			foreach(Sector s in sectors) {
				UDMFTools.ClearFields(s.Fields, floorslopekeys);
				s.UpdateNeeded = true;
			}

			blockUpdate = true;
			floorslopex.Text = "0";
			floorslopey.Text = "0";
			floorslopez.Text = "0";
			floorslopeoffset.Text = "0";
			blockUpdate = false;

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void floorslopex_WhenTextChanged(object sender, EventArgs e) {
			/*if(blockUpdate) return;
			int i = 0;

			//restore values
			if(string.IsNullOrEmpty(floorslopex.Text)) {
				foreach(Sector s in sectors) {
					UDMFTools.SetFloat(s.Fields, "floorplane_a", sectorProps[i++].FloorSlopeX, float.MinValue);
					s.UpdateNeeded = true;
				}
			//update values
			} else {
				foreach(Sector s in sectors) {
					UDMFTools.SetFloat(s.Fields, "floorplane_a", floorslopex.GetResultFloat(sectorProps[i++].FloorSlopeX), float.MinValue);
					s.UpdateNeeded = true;
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);*/
		}

		private void floorslopey_WhenTextChanged(object sender, EventArgs e) {
			/*if(blockUpdate) return;
			int i = 0;

			//restore values
			if(string.IsNullOrEmpty(floorslopey.Text)) {
				foreach(Sector s in sectors) {
					UDMFTools.SetFloat(s.Fields, "floorplane_b", sectorProps[i++].FloorSlopeY, float.MinValue);
					s.UpdateNeeded = true;
				}
			//update values
			} else {
				foreach(Sector s in sectors) {
					UDMFTools.SetFloat(s.Fields, "floorplane_b", floorslopey.GetResultFloat(sectorProps[i++].FloorSlopeY), float.MinValue);
					s.UpdateNeeded = true;
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);*/
		}

		private void floorslopez_WhenTextChanged(object sender, EventArgs e) {
			/*if(blockUpdate) return;
			int i = 0;

			//restore values
			if(string.IsNullOrEmpty(floorslopez.Text)) {
				foreach(Sector s in sectors) {
					UDMFTools.SetFloat(s.Fields, "floorplane_c", sectorProps[i++].FloorSlopeZ, float.MinValue);
					s.UpdateNeeded = true;
				}
			//update values
			} else {
				foreach(Sector s in sectors) {
					UDMFTools.SetFloat(s.Fields, "floorplane_c", floorslopez.GetResultFloat(sectorProps[i++].FloorSlopeZ), float.MinValue);
					s.UpdateNeeded = true;
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);*/
		}

		private void floorslopeoffset_WhenTextChanged(object sender, EventArgs e) {
			if(blockUpdate) return;
			int i = 0;

			//restore values
			if(string.IsNullOrEmpty(floorslopeoffset.Text)) {
				foreach(Sector s in sectors) {
					UDMFTools.SetFloat(s.Fields, "floorplane_d", sectorProps[i++].FloorSlopeOffset, float.MinValue);
					s.UpdateNeeded = true;
				}
			//update values
			} else {
				foreach(Sector s in sectors) {
					UDMFTools.SetFloat(s.Fields, "floorplane_d", floorslopeoffset.GetResultFloat(sectorProps[i++].FloorSlopeOffset), float.MinValue);
					s.UpdateNeeded = true;
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void floorsloperotation_WhenTextChanged(object sender, EventArgs e) {
			if(blockUpdate) return;
			float anglexy = floorsloperotation.GetResultFloat(float.NaN);
			float anglez = floorslopeangle.GetResultFloat(float.NaN);
			if(float.IsNaN(anglexy) || float.IsNaN(anglez)) return;

			applySlopeTransform(Angle2D.DegToRad(anglexy), Angle2D.DegToRad(anglez), floorslopekeys);
		}

		private void floorslopeangle_WhenTextChanged(object sender, EventArgs e) {
			if(blockUpdate) return;
			float anglexy = floorsloperotation.GetResultFloat(float.NaN);
			float anglez = floorslopeangle.GetResultFloat(float.NaN);
			if(float.IsNaN(anglexy) || float.IsNaN(anglez)) return;

			applySlopeTransform(Angle2D.DegToRad(anglexy), Angle2D.DegToRad(anglez), floorslopekeys);
		}

		private void applySlopeTransform(float anglexy, float anglez, string[] keys) {
			Vector3D v = Vector3D.FromAngleXYZ(anglexy + Angle2D.PI, anglez);

			//restore or set values
			if(v.x == 0 && v.y == 0 && v.z == 0) {
				foreach(Sector s in sectors) {
					UDMFTools.ClearFields(s.Fields, keys);
					s.UpdateNeeded = true;
				}
			} else {
				foreach(Sector s in sectors) {
					UDMFTools.SetFloat(s.Fields, keys[0], v.x, float.MinValue);
					UDMFTools.SetFloat(s.Fields, keys[1], v.y, float.MinValue);
					UDMFTools.SetFloat(s.Fields, keys[2], v.z, float.MinValue);
					//TODO: set offset based on current SlopePivotMode
					s.UpdateNeeded = true;
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		#endregion

		#region mxd. Ceiling slope realtime events

		private void resetceilslope_Click(object sender, EventArgs e) {
			foreach(Sector s in sectors) {
				UDMFTools.ClearFields(s.Fields, ceilslopekeys);
				s.UpdateNeeded = true;
			}

			blockUpdate = true;
			ceilslopex.Text = "0";
			ceilslopey.Text = "0";
			ceilslopez.Text = "0";
			ceilslopeoffset.Text = "0";
			ceilslopeangle.Value = 0;
			ceilslopeanglelabel.Text = "0";
			ceilsloperoll.Value = 0;
			ceilsloperolllabel.Text = "0";
			blockUpdate = false;

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void ceilslopex_WhenTextChanged(object sender, EventArgs e) {
			if(blockUpdate) return;
			int i = 0;

			//restore values
			if(string.IsNullOrEmpty(ceilslopex.Text)) {
				foreach(Sector s in sectors) {
					UDMFTools.SetFloat(s.Fields, "ceilingplane_a", sectorProps[i++].CeilSlopeX, float.MinValue);
					s.UpdateNeeded = true;
				}
			//update values
			} else {
				foreach(Sector s in sectors) {
					UDMFTools.SetFloat(s.Fields, "ceilingplane_a", ceilslopex.GetResultFloat(sectorProps[i++].CeilSlopeX), float.MinValue);
					s.UpdateNeeded = true;
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void ceilslopey_WhenTextChanged(object sender, EventArgs e) {
			if(blockUpdate) return;
			int i = 0;

			//restore values
			if(string.IsNullOrEmpty(ceilslopey.Text)) {
				foreach(Sector s in sectors) {
					UDMFTools.SetFloat(s.Fields, "ceilingplane_b", sectorProps[i++].CeilSlopeY, float.MinValue);
					s.UpdateNeeded = true;
				}
			//update values
			} else {
				foreach(Sector s in sectors) {
					UDMFTools.SetFloat(s.Fields, "ceilingplane_b", ceilslopey.GetResultFloat(sectorProps[i++].CeilSlopeY), float.MinValue);
					s.UpdateNeeded = true;
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void ceilslopez_WhenTextChanged(object sender, EventArgs e) {
			if(blockUpdate) return;
			int i = 0;

			//restore values
			if(string.IsNullOrEmpty(ceilslopez.Text)) {
				foreach(Sector s in sectors) {
					UDMFTools.SetFloat(s.Fields, "ceilingplane_c", sectorProps[i++].CeilSlopeZ, float.MinValue);
					s.UpdateNeeded = true;
				}
			//update values
			} else {
				foreach(Sector s in sectors) {
					UDMFTools.SetFloat(s.Fields, "ceilingplane_c", ceilslopez.GetResultFloat(sectorProps[i++].CeilSlopeZ), float.MinValue);
					s.UpdateNeeded = true;
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void ceilslopeoffset_WhenTextChanged(object sender, EventArgs e) {
			if(blockUpdate) return;
			int i = 0;

			//restore values
			if(string.IsNullOrEmpty(ceilslopeoffset.Text)) {
				foreach(Sector s in sectors) {
					UDMFTools.SetFloat(s.Fields, "ceilingplane_d", sectorProps[i++].CeilSlopeOffset, float.MinValue);
					s.UpdateNeeded = true;
				}
			//update values
			} else {
				foreach(Sector s in sectors) {
					UDMFTools.SetFloat(s.Fields, "ceilingplane_d", ceilslopeoffset.GetResultFloat(sectorProps[i++].CeilSlopeOffset), float.MinValue);
					s.UpdateNeeded = true;
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		#endregion

		private void ceilslopeangle_Scroll(object sender, EventArgs e) {
			if(blockUpdate) return;

			//ceilslopeanglelabel.Text = ceilslopeangle.Value + "\u00B0";
			//applySlopeValues(Angle2D.DegToRad(ceilslopeangle.Value), Angle2D.DegToRad(ceilsloperoll.Value), ceilslopekeys);

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void ceilsloperoll_Scroll(object sender, EventArgs e) {
			if(blockUpdate) return;

			//ceilsloperolllabel.Text = ceilsloperoll.Value + "\u00B0";
			//applySlopeValues(Angle2D.DegToRad(ceilslopeangle.Value), Angle2D.DegToRad(ceilsloperoll.Value), ceilslopekeys);

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		//TODO: remove this
		/*private void applySlopeValues(float anglexy, float anglez, string[] keys) {
			Vector3D v = Vector3D.FromAngleXYZ(anglexy, anglez); //normalize?

			//restore or set values
			if (v.x == 0 && v.y == 0 && v.z == 0) {
				foreach(Sector s in sectors) {
					UDMFTools.ClearFields(s.Fields, keys);
					s.UpdateNeeded = true;
				}

				//update offset text
				blockUpdate = true;
				ceilslopeoffset.Text = "0";
				blockUpdate = false;

			} else {
				foreach (Sector s in sectors) {
					UDMFTools.SetFloat(s.Fields, keys[0], v.x, float.MinValue);
					UDMFTools.SetFloat(s.Fields, keys[1], v.y, float.MinValue);
					UDMFTools.SetFloat(s.Fields, keys[2], v.z, float.MinValue);
					//TODO: set offset based on current SlopePivotMode
					s.UpdateNeeded = true;
				}
			}

			//update xyz text
			blockUpdate = true;
			ceilslopex.Text = v.x.ToString();
			ceilslopey.Text = v.y.ToString();
			ceilslopez.Text = v.z.ToString();
			blockUpdate = false;
		}*/

	}
}
