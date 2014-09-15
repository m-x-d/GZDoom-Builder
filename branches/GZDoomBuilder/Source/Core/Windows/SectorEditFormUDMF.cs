using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.GZBuilder.Tools;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Controls;

namespace CodeImp.DoomBuilder.Windows
{
	internal partial class SectorEditFormUDMF : DelayedForm
	{
		#region ================== Events

		public event EventHandler OnValuesChanged; //mxd

		#endregion

		#region ================== Variables

		private ICollection<Sector> sectors;
		private Dictionary<Sector, SectorProperties> sectorprops; //mxd
		private bool blockupdate; //mxd
		private StepsList anglesteps; //mxd

		//mxd. Persistent settings
		private static bool linkCeilingScale;
		private static bool linkFloorScale;
		private static bool useFloorLineAngles;
		private static bool useCeilLineAngles;
		private static bool useFloorSlopeLineAngles;
		private static bool useCeilSlopeLineAngles;

		//mxd. Window setup stuff
		private static Point location = Point.Empty;
		private static int activetab;
	
		//mxd. Slope pivots
		private Vector3D globalceilslopepivot;
		private Vector3D globalfloorslopepivot;
		private Dictionary<Sector, Vector3D> ceilslopepivots;
		private Dictionary<Sector, Vector3D> floorslopepivots;

		#endregion

		#region ================== Structs

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

			//UDMF slopes. Angles are in degrees
			public readonly float FloorSlopeAngleXY;
			public readonly float FloorSlopeAngleZ;
			public readonly float FloorSlopeOffset;
			public readonly float CeilingSlopeAngleXY;
			public readonly float CeilingSlopeAngleZ;
			public readonly float CeilingSlopeOffset;

			public float VirtualFloorSlopeOffset;
			public float VirtualCeilingSlopeOffset;

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

				//UDMF slopes
				if (s.FloorSlope.GetLengthSq() > 0) {
					FloorSlopeAngleXY = (float)Math.Round(Angle2D.RadToDeg(s.FloorSlope.GetAngleXY()), 1);
					FloorSlopeAngleZ = (float)Math.Round(Angle2D.RadToDeg(s.FloorSlope.GetAngleZ()), 1);
				} else {
					FloorSlopeAngleXY = 0;
					FloorSlopeAngleZ = 0;
				}
				FloorSlopeOffset = float.IsNaN(s.FloorSlopeOffset) ? -s.FloorHeight : s.FloorSlopeOffset;
				VirtualFloorSlopeOffset = FloorSlopeOffset;

				if (s.CeilingSlope.GetLengthSq() > 0) {
					CeilingSlopeAngleXY = (float)Math.Round(Angle2D.RadToDeg(s.CeilingSlope.GetAngleXY()), 1);
					CeilingSlopeAngleZ = (float)Math.Round(Angle2D.RadToDeg(s.CeilingSlope.GetAngleZ()), 1);
				} else {
					CeilingSlopeAngleXY = 0;
					CeilingSlopeAngleZ = 0;
				}
				CeilingSlopeOffset = float.IsNaN(s.CeilingSlopeOffset) ? -s.CeilHeight : s.CeilingSlopeOffset;
				VirtualCeilingSlopeOffset = CeilingSlopeOffset;
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
				if(activetab > 0) tabs.SelectTab(activetab);
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
			effect.GeneralizedOptions = General.Map.Config.GenEffectOptions; //mxd
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

			floorslopecontrol.UseLineAngles = useFloorSlopeLineAngles;
			ceilingslopecontrol.UseLineAngles = useCeilSlopeLineAngles;
		}

		#endregion

		#region ================== Methods

		// This sets up the form to edit the given sectors
		public void Setup(ICollection<Sector> sectors) {
			blockupdate = true; //mxd

			// Keep this list
			this.sectors = sectors;
			if(sectors.Count > 1) this.Text = "Edit Sectors (" + sectors.Count + ")";
			sectorprops = new Dictionary<Sector, SectorProperties>(sectors.Count); //mxd

			//mxd. Make undo
			string undodesc = "sector";
			if(sectors.Count > 1) undodesc = sectors.Count + " sectors";
			General.Map.UndoRedo.CreateUndo("Edit " + undodesc);

			//mxd. Set default height offset
			heightoffset.Text = "0";

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
			SetupSlopes(sc, true);

			// Action
			tagSelector.Setup(UniversalType.SectorTag); //mxd
			tagSelector.SetTag(sc.Tag);//mxd

			// Custom fields
			fieldslist.SetValues(sc.Fields, true);

			anglesteps = new StepsList();

			//mxd. Slope pivots
			floorslopepivots = new Dictionary<Sector, Vector3D>(sectors.Count);
			ceilslopepivots = new Dictionary<Sector, Vector3D>(sectors.Count);

			////////////////////////////////////////////////////////////////////////
			// Now go for all sectors and change the options when a setting is different
			////////////////////////////////////////////////////////////////////////

			// Go for all sectors
			foreach(Sector s in sectors) {
				//mxd. Store initial properties
				SectorProperties sp = new SectorProperties(s);
				sectorprops.Add(s, sp);

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
				SetupSlopes(s, false);

				// Action
				if(s.Tag != sc.Tag) tagSelector.ClearTag(); //mxd

				// Custom fields
				s.Fields.BeforeFieldsChange(); //mxd
				fieldslist.SetValues(s.Fields, false);

				//mxd. Angle steps
				int angle;
				foreach(Sidedef side in s.Sidedefs){
					if (side.Line.Front != null && side.Index == side.Line.Front.Index)
						angle = General.ClampAngle(270 - side.Line.AngleDeg);
					else
						angle = General.ClampAngle(90 - side.Line.AngleDeg);

					if(!anglesteps.Contains(angle)) anglesteps.Add(angle);
				}

				//Slope pivots
				Vector3D floorpivot = new Vector3D(s.BBox.X + s.BBox.Width / 2, s.BBox.Y + s.BBox.Height / 2, s.FloorHeight);
				Vector3D ceilpivot = new Vector3D(s.BBox.X + s.BBox.Width / 2, s.BBox.Y + s.BBox.Height / 2, s.CeilHeight);

				globalfloorslopepivot += floorpivot;
				globalceilslopepivot += ceilpivot;

				floorslopepivots.Add(s, floorpivot);
				ceilslopepivots.Add(s, ceilpivot);
			}

			globalfloorslopepivot /= sectors.Count;
			globalceilslopepivot /= sectors.Count;

			//mxd. Update slope controls
			#if DEBUG
			ceilingslopecontrol.UpdateControls();
			floorslopecontrol.UpdateControls();
			#endif

			// Show sector height
			UpdateSectorHeight();

			//mxd. Angle steps
			anglesteps.Sort();
			if(useCeilLineAngles) ceilRotation.StepValues = anglesteps;
			if(useFloorLineAngles) floorRotation.StepValues = anglesteps;
			if(useCeilSlopeLineAngles) ceilingslopecontrol.StepValues = anglesteps;
			if(useFloorSlopeLineAngles) floorslopecontrol.StepValues = anglesteps;

			blockupdate = false; //mxd
		}

		// This updates the sector height field
		private void UpdateSectorHeight() {
			int delta = int.MinValue;

			// Check all selected sectors
			foreach(Sector s in sectors) {
				if(delta == int.MinValue) {
					// First sector in list
					delta = s.CeilHeight - s.FloorHeight;
				} else if(delta != (s.CeilHeight - s.FloorHeight)) {
					// We can't show heights because the delta
					// heights for the sectors is different
					delta = int.MinValue;
					break;
				}
			}

			if(delta != int.MinValue) {
				sectorheight.Text = delta.ToString();
				sectorheight.Visible = true;
				sectorheightlabel.Visible = true;
			} else {
				sectorheight.Visible = false;
				sectorheightlabel.Visible = false;
			}
		}

		//mxd
		private void UpdateCeilingHeight() 
		{
			int offset = heightoffset.GetResult(0);

			//restore values
			if(string.IsNullOrEmpty(ceilingheight.Text)) 
			{
				foreach(Sector s in sectors)
					s.CeilHeight = sectorprops[s].CeilHeight + offset;
				
			} 
			else //update values
			{
				foreach(Sector s in sectors)
					s.CeilHeight = ceilingheight.GetResult(sectorprops[s].CeilHeight) + offset;
			}
		}

		//mxd
		private void UpdateFloorHeight() 
		{
			int offset = heightoffset.GetResult(0);

			//restore values
			if(string.IsNullOrEmpty(floorheight.Text)) 
			{
				foreach(Sector s in sectors)
					s.FloorHeight = sectorprops[s].FloorHeight + offset;
				
			} 
			else //update values
			{
				foreach(Sector s in sectors)
					s.FloorHeight = floorheight.GetResult(sectorprops[s].FloorHeight) + offset;
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
			int tagoffset = 0; //mxd
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
				s.Tag = General.Clamp(tagSelector.GetSmartTag(s.Tag, tagoffset++), General.Map.FormatInterface.MinTag, General.Map.FormatInterface.MaxTag); //mxd

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
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();

			// Store value linking
			linkCeilingScale = ceilScale.LinkValues;
			linkFloorScale = floorScale.LinkValues;
			useCeilLineAngles = cbUseCeilLineAngles.Checked;
			useFloorLineAngles = cbUseFloorLineAngles.Checked;
			useCeilSlopeLineAngles = ceilingslopecontrol.UseLineAngles;
			useFloorSlopeLineAngles = floorslopecontrol.UseLineAngles;

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
			activetab = tabs.SelectedIndex;
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
			ceilRotation.ButtonStepsWrapAround = cbUseCeilLineAngles.Checked;
			ceilRotation.StepValues = (cbUseCeilLineAngles.Checked ? anglesteps : null);
		}

		private void cbUseFloorLineAngles_CheckedChanged(object sender, EventArgs e) {
			floorRotation.ButtonStepsWrapAround = cbUseFloorLineAngles.Checked;
			floorRotation.StepValues = (cbUseFloorLineAngles.Checked ? anglesteps : null);
		}

		#endregion

		#region ================== Sector Realtime events (mxd)

		private void ceilingheight_WhenTextChanged(object sender, EventArgs e) 
		{
			if(blockupdate)	return;

			UpdateCeilingHeight();
			UpdateSectorHeight();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void floorheight_WhenTextChanged(object sender, EventArgs e) 
		{
			if(blockupdate)	return;

			UpdateFloorHeight();
			UpdateSectorHeight();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void heightoffset_WhenTextChanged(object sender, EventArgs e) 
		{
			if(blockupdate) return;

			UpdateFloorHeight();
			UpdateCeilingHeight();
			UpdateSectorHeight();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void brightness_WhenTextChanged(object sender, EventArgs e) {
			if(blockupdate)	return;

			//restore values
			if(string.IsNullOrEmpty(brightness.Text)) {
				foreach(Sector s in sectors)
					s.Brightness = sectorprops[s].Brightness;
			//update values
			} else {
				foreach(Sector s in sectors)
					s.Brightness = General.Clamp(brightness.GetResult(sectorprops[s].Brightness), General.Map.FormatInterface.MinBrightness, General.Map.FormatInterface.MaxBrightness);
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void ceilingtex_OnValueChanged(object sender, EventArgs e) {
			if(blockupdate)	return;

			//restore values
			if(string.IsNullOrEmpty(ceilingtex.TextureName)) {
				foreach(Sector s in sectors)
					s.SetCeilTexture(sectorprops[s].CeilTexture);
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
			if(blockupdate)	return;

			//restore values
			if(string.IsNullOrEmpty(floortex.TextureName)) {
				foreach(Sector s in sectors)
					s.SetFloorTexture(sectorprops[s].FloorTexture);
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
			if(blockupdate)	return;

			//restore values
			if(string.IsNullOrEmpty(floorRotation.Text)) {
				foreach(Sector s in sectors) {
					UDMFTools.SetFloat(s.Fields, "rotationfloor", sectorprops[s].FloorRotation, 0f);
					s.UpdateNeeded = true;
				}
			//update values
			} else {
				foreach(Sector s in sectors) {
					UDMFTools.SetFloat(s.Fields, "rotationfloor", floorRotation.GetResultFloat(sectorprops[s].FloorRotation), 0f);
					s.UpdateNeeded = true;
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void ceilRotation_WhenTextChanged(object sender, EventArgs e) {
			ceilAngleControl.Angle = (int)General.ClampAngle(360 - ceilRotation.GetResultFloat(0));
			if(blockupdate)	return;

			//restore values
			if(string.IsNullOrEmpty(ceilRotation.Text)) {
				foreach(Sector s in sectors) {
					UDMFTools.SetFloat(s.Fields, "rotationceiling", sectorprops[s].CeilRotation, 0f);
					s.UpdateNeeded = true;
				}
			//update values
			} else {
				foreach(Sector s in sectors) {
					UDMFTools.SetFloat(s.Fields, "rotationceiling", ceilRotation.GetResultFloat(sectorprops[s].CeilRotation), 0f);
					s.UpdateNeeded = true;
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void lightColor_OnValueChanged(object sender, EventArgs e) {
			if(blockupdate)	return;

			foreach(Sector s in sectors) {
				lightColor.ApplyTo(s.Fields, sectorprops[s].LightColor);
				s.UpdateNeeded = true;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void fadeColor_OnValueChanged(object sender, EventArgs e) {
			if(blockupdate)	return;

			foreach(Sector s in sectors) {
				fadeColor.ApplyTo(s.Fields, sectorprops[s].FadeColor);
				s.UpdateNeeded = true;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		#endregion

		#region ================== Ceiling/Floor realtime events (mxd)

		private void ceilOffsets_OnValuesChanged(object sender, EventArgs e) {
			if(blockupdate)	return;

			foreach(Sector s in sectors) {
				ceilOffsets.ApplyTo(s.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, sectorprops[s].CeilOffsetX, sectorprops[s].CeilOffsetY);
				s.UpdateNeeded = true;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void floorOffsets_OnValuesChanged(object sender, EventArgs e) {
			if(blockupdate)	return;

			foreach(Sector s in sectors) {
				floorOffsets.ApplyTo(s.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, sectorprops[s].FloorOffsetX, sectorprops[s].FloorOffsetY);
				s.UpdateNeeded = true;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void ceilScale_OnValuesChanged(object sender, EventArgs e) {
			if(blockupdate)	return;

			foreach(Sector s in sectors) {
				ceilScale.ApplyTo(s.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, sectorprops[s].CeilScaleX, sectorprops[s].CeilScaleY);
				s.UpdateNeeded = true;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void floorScale_OnValuesChanged(object sender, EventArgs e) {
			if(blockupdate)	return;

			foreach(Sector s in sectors) {
				floorScale.ApplyTo(s.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset, sectorprops[s].FloorScaleX, sectorprops[s].FloorScaleY);
				s.UpdateNeeded = true;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void ceilBrightness_WhenTextChanged(object sender, EventArgs e) {
			if(blockupdate)	return;

			//restore values
			if(string.IsNullOrEmpty(ceilBrightness.Text)) {
				foreach(Sector s in sectors) {
					UDMFTools.SetInteger(s.Fields, "lightceiling", sectorprops[s].CeilBrightness, 0);
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

					int value = General.Clamp(ceilBrightness.GetResult(sectorprops[s].CeilBrightness), (absolute ? 0 : -255), 255);
					UDMFTools.SetInteger(s.Fields, "lightceiling", value, 0);
					s.UpdateNeeded = true;
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void floorBrightness_WhenTextChanged(object sender, EventArgs e) {
			if(blockupdate)	return;

			//restore values
			if(string.IsNullOrEmpty(floorBrightness.Text)) {
				foreach(Sector s in sectors) {
					UDMFTools.SetInteger(s.Fields, "lightfloor", sectorprops[s].FloorBrightness, 0);
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

					int value = General.Clamp(floorBrightness.GetResult(sectorprops[s].FloorBrightness), (absolute ? 0 : -255), 255);
					UDMFTools.SetInteger(s.Fields, "lightfloor", value, 0);
					s.UpdateNeeded = true;
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void ceilLightAbsolute_CheckedChanged(object sender, EventArgs e) {
			if(blockupdate)	return;

			if(ceilLightAbsolute.Checked) {
				foreach(Sector s in sectors) {
					s.Fields["lightceilingabsolute"] = new UniValue(UniversalType.Boolean, true);
					s.UpdateNeeded = true;
				}
			} else if(ceilLightAbsolute.CheckState == CheckState.Indeterminate) {
				foreach(Sector s in sectors) {
					if(sectorprops[s].CeilLightAbsoulte) {
						s.Fields["lightceilingabsolute"] = new UniValue(UniversalType.Boolean, true);
						s.UpdateNeeded = true;
					}else if(s.Fields.ContainsKey("lightceilingabsolute")) {
						s.Fields.Remove("lightceilingabsolute");
						s.UpdateNeeded = true;
					}
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
			if(blockupdate)	return;

			if(floorLightAbsolute.Checked){
				foreach(Sector s in sectors) {
					s.Fields["lightfloorabsolute"] = new UniValue(UniversalType.Boolean, true);
					s.UpdateNeeded = true;
				}
			} else if(floorLightAbsolute.CheckState == CheckState.Indeterminate) {
				foreach(Sector s in sectors) {
					if(sectorprops[s].FloorLightAbsoulte) {
						s.Fields["lightfloorabsolute"] = new UniValue(UniversalType.Boolean, true);
						s.UpdateNeeded = true;
					} else if(s.Fields.ContainsKey("lightfloorabsolute")) {
						s.Fields.Remove("lightfloorabsolute");
						s.UpdateNeeded = true;
					}
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

		#region ================== Slope Utility (mxd)

		private void SetupSlopes(Sector s, bool first) {
			if(s.CeilingSlope.GetLengthSq() > 0) {
				float anglexy = (float)Math.Round(Angle2D.RadToDeg(s.CeilingSlope.GetAngleXY()), 1);
				float anglez = (float)(270 - Math.Round(Angle2D.RadToDeg(s.CeilingSlope.GetAngleZ()), 1));
				float offset = (float)Math.Round(GetSlopeOffset(s, s.CeilingSlopeOffset, ceilingslopecontrol.PivotMode, false), 1);
				if (anglexy == 180.0f) {
					anglexy = 0;
					anglez = -anglez;
				}
				ceilingslopecontrol.SetValues(anglexy, anglez, offset, first);
			} else {
				ceilingslopecontrol.SetValues(0f, 0f, s.CeilHeight, first);
			}

			if(s.FloorSlope.GetLengthSq() > 0) {
				float anglexy = (float)Math.Round(Angle2D.RadToDeg(s.FloorSlope.GetAngleXY()), 1);
				float anglez = (float)(Math.Round(Angle2D.RadToDeg(s.FloorSlope.GetAngleZ()) - 90, 1));
				float offset = (float)Math.Round(GetSlopeOffset(s, -s.FloorSlopeOffset, floorslopecontrol.PivotMode, true), 1);
				if (anglexy == 180.0f) {
					anglexy = 0;
					anglez = -anglez;
				}
				floorslopecontrol.SetValues(anglexy, anglez, offset, first);
			} else {
				floorslopecontrol.SetValues(0f, 0f, s.FloorHeight, first);
			}
		}

		private float GetSlopeOffset(Sector target, float offset, SlopePivotMode mode, bool floor) {
			float validoffset;

			if (mode == SlopePivotMode.ORIGIN) {
				if (float.IsNaN(offset)) {
					validoffset = (floor ? sectorprops[target].FloorSlopeOffset : sectorprops[target].CeilingSlopeOffset);
				} else {
					//use current value
					validoffset = offset;
				}
			} else {
				//use virtual value
				validoffset = (floor ? sectorprops[target].VirtualFloorSlopeOffset : sectorprops[target].VirtualCeilingSlopeOffset);
			}

			switch(mode) {
				case SlopePivotMode.GLOBAL: //rotate around the center of selection 
					//TODO: translate offset from virtual to real

					return validoffset;

				case SlopePivotMode.LOCAL: //rotate around sector's bounding box center
					//TODO: translate offset from virtual to real
					//Vector3D pivot = floor

					return validoffset;

				case SlopePivotMode.ORIGIN: //rotate around world origin (0, 0)
					return validoffset;

				default:
					throw new NotImplementedException("SectorEditFormUDMF.SetSlopeOffset: Got unknown SlopePivotMode (" + (int)mode + ")");
			}
		}

		#endregion

		#region ================== Slopes realtime events (mxd)

		private void ceilingslopecontrol_OnAnglesChanged(object sender, EventArgs e) 
		{
			if(blockupdate) return;
			float anglexy, anglez;

			//Set or restore values
			foreach(Sector s in sectors) 
			{
				anglexy = (float.IsNaN(ceilingslopecontrol.AngleXY) ? sectorprops[s].CeilingSlopeAngleXY : ceilingslopecontrol.AngleXY);
				anglez = (float.IsNaN(ceilingslopecontrol.AngleZ) ? sectorprops[s].CeilingSlopeAngleZ : ceilingslopecontrol.AngleZ + 90);
				if (anglexy == 0 && anglez == 90) {
					s.CeilingSlope = new Vector3D();
				} else {
					s.CeilingSlope = Vector3D.FromAngleXYZ(Angle2D.DegToRad(anglexy) + Angle2D.PI, Angle2D.DegToRad(-anglez));
				}
				s.CeilingSlopeOffset = GetSlopeOffset(s, ceilingslopecontrol.Offset, ceilingslopecontrol.PivotMode, false);
				s.UpdateNeeded = true;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void floorslopecontrol_OnAnglesChanged(object sender, EventArgs e) 
		{
			if(blockupdate) return;
			float anglexy, anglez;

			//Set or restore values
			foreach(Sector s in sectors)
			{
				anglexy = (float.IsNaN(floorslopecontrol.AngleXY) ? sectorprops[s].FloorSlopeAngleXY : floorslopecontrol.AngleXY);
				anglez = (float.IsNaN(floorslopecontrol.AngleZ) ? sectorprops[s].FloorSlopeAngleZ : floorslopecontrol.AngleZ + 90);
				if(anglexy == 0 && anglez == 90) {
					s.FloorSlope = new Vector3D();
				} else {
					s.FloorSlope = Vector3D.FromAngleXYZ(Angle2D.DegToRad(anglexy) + Angle2D.PI, Angle2D.DegToRad(anglez));
				}
				s.FloorSlopeOffset = GetSlopeOffset(s, -floorslopecontrol.Offset, floorslopecontrol.PivotMode, true);
				s.UpdateNeeded = true;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void ceilingslopecontrol_OnPivotModeChanged(object sender, EventArgs e) 
		{
			//TODO: update offsets to match current PivotMode


			//TODO: update Offset value


		}

		private void floorslopecontrol_OnPivotModeChanged(object sender, EventArgs e) 
		{
			//TODO: update offsets to match current PivotMode


			//TODO: update Offset value


		}

		private void ceilingslopecontrol_OnOffsetChanged(object sender, EventArgs e) 
		{
			if(blockupdate) return;
			foreach(Sector s in sectors) {
				s.CeilingSlopeOffset = GetSlopeOffset(s, ceilingslopecontrol.Offset, ceilingslopecontrol.PivotMode, false);
				s.UpdateNeeded = true;
			}
			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void floorslopecontrol_OnOffsetChanged(object sender, EventArgs e) 
		{
			if(blockupdate) return;
			foreach(Sector s in sectors) {
				s.FloorSlopeOffset = GetSlopeOffset(s, -floorslopecontrol.Offset, floorslopecontrol.PivotMode, true);
				s.UpdateNeeded = true;
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void ceilingslopecontrol_OnResetClicked(object sender, EventArgs e) 
		{
			ceilingslopecontrol.SetOffset(General.GetByIndex(sectors, 0).CeilHeight, true);
			
			foreach(Sector s in sectors) {
				s.CeilingSlope = new Vector3D();
				s.CeilingSlopeOffset = float.NaN;
				s.UpdateNeeded = true;
				ceilingslopecontrol.SetOffset(s.CeilHeight, false);
			}

			ceilingslopecontrol.UpdateOffset();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void floorslopecontrol_OnResetClicked(object sender, EventArgs e) 
		{
			floorslopecontrol.SetOffset(General.GetByIndex(sectors, 0).FloorHeight, true);
			
			foreach(Sector s in sectors) {
				s.FloorSlope = new Vector3D();
				s.FloorSlopeOffset = float.NaN;
				s.UpdateNeeded = true;
				floorslopecontrol.SetOffset(s.FloorHeight, false);
			}

			floorslopecontrol.UpdateOffset();

			General.Map.IsChanged = true;
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);
		}

		private void ceilingslopecontrol_OnUseLineAnglesChanged(object sender, EventArgs e) 
		{
			ceilingslopecontrol.StepValues = (ceilingslopecontrol.UseLineAngles ? anglesteps : null);
		}

		private void floorslopecontrol_OnUseLineAnglesChanged(object sender, EventArgs e) 
		{
			floorslopecontrol.StepValues = (floorslopecontrol.UseLineAngles ? anglesteps : null);
		}

		#endregion

	}
}
