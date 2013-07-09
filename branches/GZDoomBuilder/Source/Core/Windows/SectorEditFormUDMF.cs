using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.GZBuilder.Tools;

namespace CodeImp.DoomBuilder.Windows
{
	public partial class SectorEditFormUDMF : DelayedForm, ISectorEditForm
	{
		#region ================== Events

		public event EventHandler OnValuesChanged; //mxd

		#endregion

		#region ================== Variables

		private ICollection<Sector> sectors;
		private List<CheckBox> flags;

		#endregion

		#region ================== Properties

		public ICollection<Sector> Selection { get { return sectors; } } //mxd

		#endregion

		#region ================== Constructor

		public SectorEditFormUDMF() {
			InitializeComponent();

			// Fill effects list
			effect.AddInfo(General.Map.Config.SortedSectorEffects.ToArray());

			// Fill universal fields list
			fieldslist.ListFixedFields(General.Map.Config.SectorFields);

			// Initialize image selectors
			floortex.Initialize();
			ceilingtex.Initialize();

			// Set steps for brightness field
			brightness.StepValues = General.Map.Config.BrightnessLevels;

			// Custom fields?
			if(!General.Map.FormatInterface.HasCustomFields)
				tabs.TabPages.Remove(tabcustom);

			flags = new List<CheckBox>() { cbDropactors, cbHidden, cbNofallingdamage, cbNorespawn, cbSilent, ceilLightAbsolute, floorLightAbsolute };

			// Initialize custom fields editor
			fieldslist.Setup("sector");
		}

		#endregion

		#region ================== Methods

		// This sets up the form to edit the given sectors
		public void Setup(ICollection<Sector> sectors) {
			Sector sc;

			// Keep this list
			this.sectors = sectors;
			if(sectors.Count > 1) this.Text = "Edit Sectors (" + sectors.Count + ")";

			////////////////////////////////////////////////////////////////////////
			// Set all options to the first sector properties
			////////////////////////////////////////////////////////////////////////

			// Get first sector
			sc = General.GetByIndex(sectors, 0);

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
			ceilOffsets.SetValuesFrom(sc.Fields);
			floorOffsets.SetValuesFrom(sc.Fields);

			//Texture scale
			ceilScale.SetValuesFrom(sc.Fields);
			floorScale.SetValuesFrom(sc.Fields);

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

			//Alpha
			ceilAlpha.Text = General.Clamp(sc.Fields.GetValue("alphaceiling", 1.0f), 0f, 1f).ToString();
			floorAlpha.Text = General.Clamp(sc.Fields.GetValue("alphafloor", 1.0f), 0f, 1f).ToString();

			//Render style
			ceilRenderStyle.SelectedIndex = (sc.Fields.GetValue("renderstyleceiling", "") == "add" ? 1 : 0);
			floorRenderStyle.SelectedIndex = (sc.Fields.GetValue("renderstylefloor", "") == "add" ? 1 : 0);

			//Misc
			soundSequence.Text = sc.Fields.GetValue("soundsequence", string.Empty);
			gravity.Text = sc.Fields.GetValue("gravity", 1.0f).ToString();
			desaturation.Text = General.Clamp(sc.Fields.GetValue("desaturation", 0.0f), 0f, 1f).ToString();

			//Sector colors
			fadeColor.SetValueFrom(sc.Fields);
			lightColor.SetValueFrom(sc.Fields);

			//Sector flags
			foreach(CheckBox cb in flags) {
				string key = cb.Tag.ToString();
				if(sc.Fields != null)
					cb.CheckState = (sc.Fields.GetValue(key, false) ? CheckState.Checked : CheckState.Unchecked);
			}

			// Action
			tagSelector.Setup(); //mxd
			tagSelector.SetTag(sc.Tag);//mxd

			// Custom fields
			fieldslist.SetValues(sc.Fields, true);

			////////////////////////////////////////////////////////////////////////
			// Now go for all sectors and change the options when a setting is different
			////////////////////////////////////////////////////////////////////////

			// Go for all sectors
			foreach(Sector s in sectors) {
				// Effects
				if(s.Effect != effect.Value) effect.Empty = true;
				if(s.Brightness.ToString() != brightness.Text) brightness.Text = "";

				// Floor/Ceiling
				if(s.FloorHeight.ToString() != floorheight.Text) floorheight.Text = "";
				if(s.CeilHeight.ToString() != ceilingheight.Text) ceilingheight.Text = "";
				if(s.FloorTexture != floortex.TextureName) floortex.TextureName = "";
				if(s.CeilTexture != ceilingtex.TextureName) ceilingtex.TextureName = "";

				//mxd. UDMF stuff
				//Texture offsets
				ceilOffsets.SetValuesFrom(s.Fields);
				floorOffsets.SetValuesFrom(s.Fields);

				//Texture scale
				ceilScale.SetValuesFrom(s.Fields);
				floorScale.SetValuesFrom(s.Fields);

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

				//Alpha
				if(s.Fields.GetValue("alphaceiling", 1.0f).ToString() != ceilAlpha.Text) ceilAlpha.Text = "";
				if(s.Fields.GetValue("alphafloor", 1.0f).ToString() != floorAlpha.Text) floorAlpha.Text = "";

				//Render style
				if(ceilRenderStyle.SelectedIndex != (s.Fields.GetValue("renderstyleceiling", "") == "add" ? 1 : 0)) ceilRenderStyle.SelectedIndex = -1;
				if(floorRenderStyle.SelectedIndex != (s.Fields.GetValue("renderstylefloor", "") == "add" ? 1 : 0)) floorRenderStyle.SelectedIndex = -1;

				//Misc
				if(s.Fields.GetValue("soundsequence", string.Empty) != soundSequence.Text) soundSequence.Text = "";
				if(s.Fields.GetValue("gravity", 1.0f).ToString() != gravity.Text) gravity.Text = "";
				if(s.Fields.GetValue("desaturation", 0.0f).ToString() != desaturation.Text) desaturation.Text = "";

				//Sector colors
				fadeColor.SetValueFrom(s.Fields);
				lightColor.SetValueFrom(s.Fields);

				//Sector flags
				foreach(CheckBox cb in flags) {
					if(cb.CheckState == CheckState.Indeterminate) continue;

					string key = cb.Tag.ToString();
					if(s.Fields != null) {
						CheckState state = (s.Fields.GetValue(key, false) ? CheckState.Checked : CheckState.Unchecked);
						if(cb.CheckState != state) {
							cb.ThreeState = true;
							cb.CheckState = CheckState.Indeterminate;
						}
					}
				}

				// Action
				if(s.Tag != sc.Tag) tagSelector.ClearTag(); //mxd

				// Custom fields
				fieldslist.SetValues(s.Fields, false);
			}

			// Show sector height
			UpdateSectorHeight();
		}

		// This updates the sector height field
		private void UpdateSectorHeight() {
			bool showheight = true;
			int delta = 0;
			Sector first = null;

			// Check all selected sectors
			foreach(Sector s in sectors) {
				if(first == null) {
					// First sector in list
					delta = s.CeilHeight - s.FloorHeight;
					showheight = true;
					first = s;
				} else {
					if(delta != (s.CeilHeight - s.FloorHeight)) {
						// We can't show heights because the delta
						// heights for the sectors is different
						showheight = false;
						break;
					}
				}
			}

			if(showheight) {
				int fh = floorheight.GetResult(first.FloorHeight);
				int ch = ceilingheight.GetResult(first.CeilHeight);
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
			string undodesc = "sector";

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

			// Make undo
			if(sectors.Count > 1) undodesc = sectors.Count + " sectors";
			General.Map.UndoRedo.CreateUndo("Edit " + undodesc);

			// Go for all sectors
			foreach(Sector s in sectors) {
				// Effects
				if(!effect.Empty) s.Effect = effect.Value;
				s.Brightness = General.Clamp(brightness.GetResult(s.Brightness), General.Map.FormatInterface.MinBrightness, General.Map.FormatInterface.MaxBrightness);

				// Floor/Ceiling
				s.FloorHeight = floorheight.GetResult(s.FloorHeight);
				s.CeilHeight = ceilingheight.GetResult(s.CeilHeight);
				s.SetFloorTexture(floortex.GetResult(s.FloorTexture));
				s.SetCeilTexture(ceilingtex.GetResult(s.CeilTexture));

				// Action
				s.Tag = tagSelector.GetTag(s.Tag); //mxd

				// Custom fields
				fieldslist.Apply(s.Fields);

				//mxd. UDMF stuff
				ceilOffsets.ApplyTo(s.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset);
				floorOffsets.ApplyTo(s.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset);

				ceilScale.ApplyTo(s.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset);
				floorScale.ApplyTo(s.Fields, General.Map.FormatInterface.MinTextureOffset, General.Map.FormatInterface.MaxTextureOffset);

				//rotation
				if(ceilRotation.Text != ""){
					float angle = General.ClampAngle(ceilRotation.GetResultFloat(0f));
					UDMFTools.SetFloat(s.Fields, "rotationceiling", angle, 0f, false);
				}

				if(floorRotation.Text != "") {
					float angle = General.ClampAngle(floorRotation.GetResultFloat(0f));
					UDMFTools.SetFloat(s.Fields, "rotationfloor", angle, 0f, false);
				}

				//alpha
				if(ceilAlpha.Text != "") {
					float ceilAlphaVal = General.Clamp(ceilAlpha.GetResultFloat(1.0f), 0f, 1f);
					UDMFTools.SetFloat(s.Fields, "alphaceiling", ceilAlphaVal, 1.0f, false);
				}
				
				if(floorAlpha.Text != ""){
					float floorAlphaVal = General.Clamp(floorAlpha.GetResultFloat(1.0f), 0f, 1f);
					UDMFTools.SetFloat(s.Fields, "alphafloor", floorAlphaVal, 1.0f, false);
				}

				//renderstyle
				if(ceilRenderStyle.SelectedIndex == 1) { //add
					s.Fields["renderstyleceiling"] = new UniValue(UniversalType.String, "add");
				} else if(s.Fields.ContainsKey("renderstyleceiling")) {
					s.Fields.Remove("renderstyleceiling");
				}

				if(floorRenderStyle.SelectedIndex == 1) { //add
					s.Fields["renderstylefloor"] = new UniValue(UniversalType.String, "add");
				} else if(s.Fields.ContainsKey("renderstylefloor")) {
					s.Fields.Remove("renderstylefloor");
				}

				//brightness
				if(!string.IsNullOrEmpty(ceilBrightness.Text)) {
					string key = ceilBrightness.Tag.ToString();
					bool absolute = (ceilLightAbsolute.CheckState == CheckState.Checked);
					int value = General.Clamp(ceilBrightness.GetResult(UDMFTools.GetInteger(s.Fields, key, 0)), (absolute ? 0 : -255), 255);
					UDMFTools.SetInteger(s.Fields, key, value, 0, false);
				}

				if(!string.IsNullOrEmpty(floorBrightness.Text)) {
					string key = floorBrightness.Tag.ToString();
					bool absolute = (floorLightAbsolute.CheckState == CheckState.Checked);
					int value = General.Clamp(floorBrightness.GetResult(UDMFTools.GetInteger(s.Fields, key, 0)), (absolute ? 0 : -255), 255);
					UDMFTools.SetInteger(s.Fields, key, value, 0, false);
				}

				//Sector colors
				fadeColor.ApplyTo(s.Fields);
				lightColor.ApplyTo(s.Fields);

				//misc
				if(soundSequence.Text != "") s.Fields["soundsequence"] = new UniValue(UniversalType.String, soundSequence.Text);
				if(gravity.Text != "") UDMFTools.SetFloat(s.Fields, "alphafloor", gravity.GetResultFloat(1.0f), 1.0f, false);
				if(desaturation.Text != "") {
					float val = General.Clamp(desaturation.GetResultFloat(0f), 0f, 1f);
					UDMFTools.SetFloat(s.Fields, "desaturation", val, 0f, false);
				}

				//flags
				foreach(CheckBox cb in flags) {
					if(cb.CheckState == CheckState.Indeterminate) continue;
					string key = cb.Tag.ToString();
					bool oldValue = s.Fields.GetValue(key, false);

					if(cb.CheckState == CheckState.Checked) {
						if(!oldValue) s.Fields[key] = new UniValue(UniversalType.Boolean, true);
					} else if(s.Fields.ContainsKey(key)) {
						s.Fields.Remove(key);
					}
				}
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();

			// Done
			General.Map.IsChanged = true;

			//dbg
			if(OnValuesChanged != null) OnValuesChanged(this, EventArgs.Empty);

			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void cancel_Click(object sender, EventArgs e) {
			// Be gone
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void browseeffect_Click(object sender, EventArgs e) {
			effect.Value = EffectBrowserForm.BrowseEffect(this, effect.Value);
		}

		private void ceilingheight_WhenTextChanged(object sender, EventArgs e) {
			UpdateSectorHeight();
		}

		private void floorheight_WhenTextChanged(object sender, EventArgs e) {
			UpdateSectorHeight();
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

		private void floorRotation_WhenTextChanged(object sender, EventArgs e) {
			floorAngleControl.Angle = General.ClampAngle(360 - floorRotation.GetResult(0));
		}

		private void ceilRotation_WhenTextChanged(object sender, EventArgs e) {
			ceilAngleControl.Angle = General.ClampAngle(360 - ceilRotation.GetResult(0));
		}

		#endregion
	}
}
