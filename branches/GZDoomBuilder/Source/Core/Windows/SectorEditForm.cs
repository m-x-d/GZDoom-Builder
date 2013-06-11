
#region ================== Copyright (c) 2007 Pascal vd Heiden

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.Windows
{
	internal partial class SectorEditForm : DelayedForm
	{
		// Variables
		private ICollection<Sector> sectors;

		// Constructor
		public SectorEditForm()
		{
			// Initialize
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

            //mxd. Texture offsets panel setup
            if (!General.Map.UDMF) {
                panelTextureOffsets.Visible = false;
                panelHeights.Top = floortex.Top;
            }
			
			// Initialize custom fields editor
			fieldslist.Setup("sector");
		}
		
		// This sets up the form to edit the given sectors
		public void Setup(ICollection<Sector> sectors)
		{
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

            //mxd. Texture offsets
            if (General.Map.UDMF) {
                ceilOffsetX.Text = getUDMFTextureOffset(sc.Fields, "xpanningceiling").ToString();
                ceilOffsetY.Text = getUDMFTextureOffset(sc.Fields, "ypanningceiling").ToString();
                floorOffsetX.Text = getUDMFTextureOffset(sc.Fields, "xpanningfloor").ToString();
                floorOffsetY.Text = getUDMFTextureOffset(sc.Fields, "ypanningfloor").ToString();
            }

			// Action
			//tag.Text = sc.Tag.ToString();
			tagSelector.Setup(); //mxd
			tagSelector.SetTag(sc.Tag);//mxd

			// Custom fields
			fieldslist.SetValues(sc.Fields, true);
			
			////////////////////////////////////////////////////////////////////////
			// Now go for all sectors and change the options when a setting is different
			////////////////////////////////////////////////////////////////////////

			// Go for all sectors
			foreach(Sector s in sectors)
			{
				// Effects
				if(s.Effect != effect.Value) effect.Empty = true;
				if(s.Brightness.ToString() != brightness.Text) brightness.Text = "";

				// Floor/Ceiling
				if(s.FloorHeight.ToString() != floorheight.Text) floorheight.Text = "";
				if(s.CeilHeight.ToString() != ceilingheight.Text) ceilingheight.Text = "";
				if(s.FloorTexture != floortex.TextureName) floortex.TextureName = "";
				if(s.CeilTexture != ceilingtex.TextureName) ceilingtex.TextureName = "";

                //mxd. Texture offsets
                if (General.Map.UDMF) {
                    if (ceilOffsetX.Text != getUDMFTextureOffset(s.Fields, "xpanningceiling").ToString()) ceilOffsetX.Text = "";
                    if (ceilOffsetY.Text != getUDMFTextureOffset(s.Fields, "ypanningceiling").ToString()) ceilOffsetY.Text = "";
                    if (floorOffsetX.Text != getUDMFTextureOffset(s.Fields, "xpanningfloor").ToString()) floorOffsetX.Text = "";
                    if (floorOffsetY.Text != getUDMFTextureOffset(s.Fields, "ypanningfloor").ToString()) floorOffsetY.Text = "";
                }

				// Action
				//if(s.Tag.ToString() != tag.Text) tag.Text = "";
				if(s.Tag != sc.Tag)	tagSelector.ClearTag(); //mxd

				// Custom fields
				fieldslist.SetValues(s.Fields, false);
			}

			// Show sector height
			UpdateSectorHeight();
		}

		// This updates the sector height field
		private void UpdateSectorHeight()
		{
			bool showheight = true;
			int delta = 0;
			Sector first = null;
			
			// Check all selected sectors
			foreach(Sector s in sectors)
			{
				if(first == null)
				{
					// First sector in list
					delta = s.CeilHeight - s.FloorHeight;
					showheight = true;
					first = s;
				}
				else
				{
					if(delta != (s.CeilHeight - s.FloorHeight))
					{
						// We can't show heights because the delta
						// heights for the sectors is different
						showheight = false;
						break;
					}
				}
			}

			if(showheight)
			{
				int fh = floorheight.GetResult(first.FloorHeight);
				int ch = ceilingheight.GetResult(first.CeilHeight);
				int height = ch - fh;
				sectorheight.Text = height.ToString();
				sectorheight.Visible = true;
				sectorheightlabel.Visible = true;
			}
			else
			{
				sectorheight.Visible = false;
				sectorheightlabel.Visible = false;
			}
		}

        //mxd
        private float getUDMFTextureOffset(UniFields fields, string key) {
            if (fields != null && fields.ContainsKey(key))
                return (float)fields[key].Value;
            return 0;
        }

        //mxd
        private void setUDMFTextureOffset(UniFields fields, string key, float value) {
            if (fields == null) return;

            fields.BeforeFieldsChange();

			if(value != 0) {
				if(!fields.ContainsKey(key))
					fields.Add(key, new UniValue(UniversalType.Float, value));
				else
					fields[key].Value = value;
			} else if(fields.ContainsKey(key)) { //don't save default value
				fields.Remove(key);
			}
        }

		// OK clicked
		private void apply_Click(object sender, EventArgs e)
		{
			string undodesc = "sector";
			
			// Verify the tag
			tagSelector.ValidateTag(); //mxd
			if((tagSelector.GetTag(0) < General.Map.FormatInterface.MinTag) || (tagSelector.GetTag(0) > General.Map.FormatInterface.MaxTag))
			{
				General.ShowWarningMessage("Sector tag must be between " + General.Map.FormatInterface.MinTag + " and " + General.Map.FormatInterface.MaxTag + ".", MessageBoxButtons.OK);
				return;
			}

			// Verify the effect
			if((effect.Value < General.Map.FormatInterface.MinEffect) || (effect.Value > General.Map.FormatInterface.MaxEffect))
			{
				General.ShowWarningMessage("Sector effect must be between " + General.Map.FormatInterface.MinEffect + " and " + General.Map.FormatInterface.MaxEffect + ".", MessageBoxButtons.OK);
				return;
			}

			// Verify the brightness
            //mxd. We clamp it anyway, so...
			/*if((brightness.GetResult(0) < General.Map.FormatInterface.MinBrightness) || (brightness.GetResult(0) > General.Map.FormatInterface.MaxBrightness))
			{
				General.ShowWarningMessage("Sector brightness must be between " + General.Map.FormatInterface.MinBrightness + " and " + General.Map.FormatInterface.MaxBrightness + ".", MessageBoxButtons.OK);
				return;
			}*/
			
			// Make undo
			if(sectors.Count > 1) undodesc = sectors.Count + " sectors";
			General.Map.UndoRedo.CreateUndo("Edit " + undodesc);

			// Go for all sectors
			foreach(Sector s in sectors)
			{
				// Effects
				if(!effect.Empty) s.Effect = effect.Value;
				s.Brightness = General.Clamp(brightness.GetResult(s.Brightness), General.Map.FormatInterface.MinBrightness, General.Map.FormatInterface.MaxBrightness);

				// Floor/Ceiling
				s.FloorHeight = floorheight.GetResult(s.FloorHeight);
				s.CeilHeight = ceilingheight.GetResult(s.CeilHeight);
				s.SetFloorTexture(floortex.GetResult(s.FloorTexture));
				s.SetCeilTexture(ceilingtex.GetResult(s.CeilTexture));

				// Action
				//s.Tag = General.Clamp(tag.GetResult(s.Tag), General.Map.FormatInterface.MinTag, General.Map.FormatInterface.MaxTag);
				s.Tag = tagSelector.GetTag(s.Tag); //mxd

				// Custom fields
				fieldslist.Apply(s.Fields);

                //mxd. Texture offsets
                int min = General.Map.FormatInterface.MinTextureOffset;
                int max = General.Map.FormatInterface.MaxTextureOffset;

                if (General.Map.UDMF) {
                    if (ceilOffsetX.Text != "") setUDMFTextureOffset(s.Fields, "xpanningceiling", General.Clamp(ceilOffsetX.GetResult((int)getUDMFTextureOffset(s.Fields, "xpanningceiling")), min, max));
                    if (ceilOffsetY.Text != "") setUDMFTextureOffset(s.Fields, "ypanningceiling", General.Clamp(ceilOffsetY.GetResult((int)getUDMFTextureOffset(s.Fields, "ypanningceiling")), min, max));
                    if (floorOffsetX.Text != "") setUDMFTextureOffset(s.Fields, "xpanningfloor", General.Clamp(floorOffsetX.GetResult((int)getUDMFTextureOffset(s.Fields, "xpanningfloor")), min, max));
                    if (floorOffsetY.Text != "") setUDMFTextureOffset(s.Fields, "ypanningfloor", General.Clamp(floorOffsetY.GetResult((int)getUDMFTextureOffset(s.Fields, "ypanningfloor")), min, max));
                }
			}
			
			// Update the used textures
			General.Map.Data.UpdateUsedTextures();
			
			// Done
			General.Map.IsChanged = true;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		// Cancel clicked
		private void cancel_Click(object sender, EventArgs e)
		{
			// Be gone
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}


		// Browse Effect clicked
		private void browseeffect_Click(object sender, EventArgs e)
		{
			effect.Value = EffectBrowserForm.BrowseEffect(this, effect.Value);
		}

		// Ceiling height changes
		private void ceilingheight_TextChanged(object sender, EventArgs e)
		{
			UpdateSectorHeight();
		}

		// Floor height changes
		private void floorheight_TextChanged(object sender, EventArgs e)
		{
			UpdateSectorHeight();
		}

		//mxd
		private void tabcustom_MouseEnter(object sender, EventArgs e) {
			fieldslist.Focus();
		}

		// Help
		private void SectorEditForm_HelpRequested(object sender, HelpEventArgs hlpevent)
		{
			General.ShowHelp("w_sectoredit.html");
			hlpevent.Handled = true;
		}
	}
}