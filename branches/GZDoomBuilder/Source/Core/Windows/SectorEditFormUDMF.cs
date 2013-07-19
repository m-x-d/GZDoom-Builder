using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.GZBuilder.Tools;

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

		//Value linking
		private static bool linkCeilingScale;
		private static bool linkFloorScale;

		private struct SectorProperties //mxd
		{
			public int Brightness;
			public int FloorHeight;
			public int CeilHeight;
			public string FloorTexture;
			public string CeilTexture;

			//UDMF stuff
			public int LightColor;
			public int FadeColor;
			//public float Desaturation;

			//UDMF Ceiling
			public float CeilOffsetX;
			public float CeilOffsetY;
			public float CeilScaleX;
			public float CeilScaleY;
			//public float CeilAlpha;
			public float CeilRotation;
			public int CeilBrightness;
			public bool CeilLightAbsoulte;

			//UDMF Floor
			public float FloorOffsetX;
			public float FloorOffsetY;
			public float FloorScaleX;
			public float FloorScaleY;
			//public float FloorAlpha;
			public float FloorRotation;
			public int FloorBrightness;
			public bool FloorLightAbsoulte;

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
			}
		}

		#endregion

		#region ================== Constructor

		public SectorEditFormUDMF() {
			InitializeComponent();

			// Fill flags list
			foreach(KeyValuePair<string, string> lf in General.Map.Config.SectorFlags)
				flags.Add(lf.Value, lf.Key);

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

			// Initialize custom fields editor
			fieldslist.Setup("sector");

			// Value linking
			ceilScale.LinkValues = linkCeilingScale;
			floorScale.LinkValues = linkFloorScale;
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
			ceilRenderStyle.SelectedIndex = (sc.Fields.GetValue("renderstyleceiling", "") == "add" ? 1 : 0);
			floorRenderStyle.SelectedIndex = (sc.Fields.GetValue("renderstylefloor", "") == "add" ? 1 : 0);

			//Misc
			soundSequence.Text = sc.Fields.GetValue("soundsequence", string.Empty);
			gravity.Text = sc.Fields.GetValue("gravity", 1.0f).ToString();
			desaturation.Text = General.Clamp(sc.Fields.GetValue("desaturation", 0.0f), 0f, 1f).ToString();

			//Sector colors
			fadeColor.SetValueFrom(sc.Fields);
			lightColor.SetValueFrom(sc.Fields);

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
				// Flags
				foreach(CheckBox c in flags.Checkboxes) {
					if(s.Flags.ContainsKey(c.Tag.ToString())) {
						if(s.Flags[c.Tag.ToString()] != c.Checked) {
							c.ThreeState = true;
							c.CheckState = CheckState.Indeterminate;
						}
					}
				}

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
				if(s.Fields.GetValue("lightfloor", 0).ToString() != floorBrightness.Text)
					floorBrightness.Text = "";

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
				if(ceilRenderStyle.SelectedIndex != (s.Fields.GetValue("renderstyleceiling", "") == "add" ? 1 : 0)) ceilRenderStyle.SelectedIndex = -1;
				if(floorRenderStyle.SelectedIndex != (s.Fields.GetValue("renderstylefloor", "") == "add" ? 1 : 0)) floorRenderStyle.SelectedIndex = -1;

				//Misc
				if(s.Fields.GetValue("soundsequence", string.Empty) != soundSequence.Text) soundSequence.Text = "";
				if(s.Fields.GetValue("gravity", 1.0f).ToString() != gravity.Text) gravity.Text = "";
				if(s.Fields.GetValue("desaturation", 0.0f).ToString() != desaturation.Text) desaturation.Text = "";

				//Sector colors
				fadeColor.SetValueFrom(s.Fields);
				lightColor.SetValueFrom(s.Fields);

				// Action
				if(s.Tag != sc.Tag) tagSelector.ClearTag(); //mxd

				// Custom fields
				s.Fields.BeforeFieldsChange(); //mxd
				fieldslist.SetValues(s.Fields, false);

				//mxd. Store initial properties
				sectorProps.Add(new SectorProperties(s));
			}

			// Show sector height
			UpdateSectorHeight();

			blockUpdate = false; //mxd
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

				//alpha
				if(!string.IsNullOrEmpty(ceilAlpha.Text)) {
					float ceilAlphaVal = General.Clamp(ceilAlpha.GetResultFloat(s.Fields.GetValue("alphaceiling", 1.0f)), 0f, 1f);
					UDMFTools.SetFloat(s.Fields, "alphaceiling", ceilAlphaVal, 1.0f, false);
				}
				
				if(!string.IsNullOrEmpty(floorAlpha.Text)){
					float floorAlphaVal = General.Clamp(floorAlpha.GetResultFloat(s.Fields.GetValue("alphafloor", 1.0f)), 0f, 1f);
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

				//misc
				if(soundSequence.Text != "") s.Fields["soundsequence"] = new UniValue(UniversalType.String, soundSequence.Text);
				if(gravity.Text != "")
					UDMFTools.SetFloat(s.Fields, "gravity", gravity.GetResultFloat(s.Fields.GetValue("gravity", 1.0f)), 1.0f, false);
				if(desaturation.Text != "") {
					float val = General.Clamp(desaturation.GetResultFloat(s.Fields.GetValue("desaturation", 0f)), 0f, 1f);
					UDMFTools.SetFloat(s.Fields, "desaturation", val, 0f, false);
				}
			}

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();

			// Store value linking
			linkCeilingScale = ceilScale.LinkValues;
			linkFloorScale = floorScale.LinkValues;

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

		#endregion

		#region mxd. Sector Realtime events

		private void ceilingheight_WhenTextChanged(object sender, EventArgs e) {
			UpdateSectorHeight();

			if(blockUpdate)	return;

			//restore values
			if(string.IsNullOrEmpty(ceilingheight.Text)) {
				int i = 0;

				foreach(Sector s in sectors)
					s.CeilHeight = sectorProps[i++].CeilHeight;
			//update values
			} else {
				foreach(Sector s in sectors)
					s.CeilHeight = ceilingheight.GetResult(s.CeilHeight);
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void floorheight_WhenTextChanged(object sender, EventArgs e) {
			UpdateSectorHeight();

			if(blockUpdate)	return;

			//restore values
			if(string.IsNullOrEmpty(floorheight.Text)) {
				int i = 0;

				foreach(Sector s in sectors)
					s.FloorHeight = sectorProps[i++].FloorHeight;
				//update values
			} else {
				foreach(Sector s in sectors)
					s.FloorHeight = floorheight.GetResult(s.FloorHeight);
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void brightness_WhenTextChanged(object sender, EventArgs e) {
			if(blockUpdate)	return;

			//restore values
			if(string.IsNullOrEmpty(brightness.Text)) {
				int i = 0;

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
					s.Brightness = General.Clamp(brightness.GetResult(s.Brightness), General.Map.FormatInterface.MinBrightness, General.Map.FormatInterface.MaxBrightness);
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
			floorAngleControl.Angle = General.ClampAngle(360 - floorRotation.GetResult(0));

			if(blockUpdate)	return;
			int i = 0;

			//restore values
			if(string.IsNullOrEmpty(floorRotation.Text)) {
				foreach(Sector s in sectors) {
					UDMFTools.SetFloat(s.Fields, "rotationfloor", sectorProps[i++].FloorRotation, 0f, false);
					s.UpdateNeeded = true;
				}
			//update values
			} else {
				foreach(Sector s in sectors) {
					UDMFTools.SetFloat(s.Fields, "rotationfloor", floorRotation.GetResult((int)sectorProps[i++].FloorRotation), 0f, false);
					s.UpdateNeeded = true;
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void ceilRotation_WhenTextChanged(object sender, EventArgs e) {
			ceilAngleControl.Angle = General.ClampAngle(360 - ceilRotation.GetResult(0));

			if(blockUpdate)	return;
			int i = 0;

			//restore values
			if(string.IsNullOrEmpty(ceilRotation.Text)) {
				foreach(Sector s in sectors) {
					UDMFTools.SetFloat(s.Fields, "rotationceiling", sectorProps[i++].CeilRotation, 0f, false);
					s.UpdateNeeded = true;
				}
			//update values
			} else {
				foreach(Sector s in sectors) {
					UDMFTools.SetFloat(s.Fields, "rotationceiling", ceilRotation.GetResult((int)sectorProps[i++].CeilRotation), 0f, false);
					s.UpdateNeeded = true;
				}
			}

			General.Map.IsChanged = true;
			if(OnValuesChanged != null)	OnValuesChanged(this, EventArgs.Empty);
		}

		private void fieldslist_OnFieldValueChanged(string fieldname) {
			if(blockUpdate)	return;
			Sector sc = null;

			foreach(Sector s in sectors) {
				fieldslist.Apply(s.Fields);
				s.UpdateNeeded = true;
				if(sc == null) sc = s;
			}

			if(sc == null) return;

			//update interface... yaaaay...
			switch(fieldname) {
				case "xpanningfloor":
				case "ypanningfloor":
					floorOffsets.SetValuesFrom(sc.Fields, true);
					break;

				case "xpanningceiling":
				case "ypanningceiling":
					ceilOffsets.SetValuesFrom(sc.Fields, true);
					break;

				case "xscalefloor":
				case "yscalefloor":
					floorScale.SetValuesFrom(sc.Fields, true);
					break;

				case "xscaleceiling":
				case "yscaleceiling":
					ceilScale.SetValuesFrom(sc.Fields, true);
					break;

				case "rotationceiling":
					ceilRotation.Text = sc.Fields.GetValue("rotationceiling", 0f).ToString();
					break;

				case "rotationfloor":
					floorRotation.Text = sc.Fields.GetValue("rotationfloor", 0f).ToString();
					break;

				case "lightfloor":
					floorBrightness.Text = sc.Fields.GetValue("lightfloor", 0).ToString();
					break;

				case "lightceiling":
					ceilBrightness.Text = sc.Fields.GetValue("lightceiling", 0).ToString();
					break;

				case "lightfloorabsolute":
					floorLightAbsolute.Checked = sc.Fields.GetValue("lightfloorabsolute", false);
					break;

				case "lightceilingabsolute":
					ceilLightAbsolute.Checked = sc.Fields.GetValue("lightceilingabsolute", false);
					break;

				case "alphafloor":
					floorAlpha.Text = sc.Fields.GetValue("alphafloor", 1.0f).ToString();
					break;

				case "alphaceiling":
					floorAlpha.Text = sc.Fields.GetValue("alphaceiling", 1.0f).ToString();
					break;

				case "gravity":
					gravity.Text = sc.Fields.GetValue("gravity", 1.0f).ToString();
					break;

				case "desaturation":
					desaturation.Text = sc.Fields.GetValue("desaturation", 0f).ToString();
					break;

				case "lightcolor":
					lightColor.SetValueFrom(sc.Fields);
					break;

				case "fadecolor":
					fadeColor.SetValueFrom(sc.Fields);
					break;

				case "renderstylefloor":
					string rsf = sc.Fields.GetValue("renderstylefloor", string.Empty);

					if(string.IsNullOrEmpty(rsf) || rsf.ToLower() == "translucent") {
						floorRenderStyle.SelectedIndex = 0;
					} else {
						floorRenderStyle.SelectedIndex = 1;
					}
					break;

				case "renderstyleceiling":
					string rsc = sc.Fields.GetValue("renderstyleceiling", string.Empty);

					if(string.IsNullOrEmpty(rsc) || rsc.ToLower() == "translucent") {
						ceilRenderStyle.SelectedIndex = 0;
					} else {
						ceilRenderStyle.SelectedIndex = 1;
					}
					break;

				case "soundsequence":
					soundSequence.Text = sc.Fields.GetValue("soundsequence", string.Empty);
					break;
			}
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
					UDMFTools.SetInteger(s.Fields, "lightceiling", sectorProps[i++].CeilBrightness, 0, false);
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
					UDMFTools.SetInteger(s.Fields, "lightceiling", value, 0, false);
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
					UDMFTools.SetInteger(s.Fields, "lightfloor", sectorProps[i++].FloorBrightness, 0, false);
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
					UDMFTools.SetInteger(s.Fields, "lightfloor", value, 0, false);
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

	}
}
