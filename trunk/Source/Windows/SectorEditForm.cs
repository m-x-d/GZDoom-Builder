
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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.IO;
using System.IO;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Controls;

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
			sc = General.GetByIndex<Sector>(sectors, 0);

			// Effects
			effect.Value = sc.Effect;
			brightness.Text = sc.Brightness.ToString();

			// Floor/ceiling
			floorheight.Text = sc.FloorHeight.ToString();
			ceilingheight.Text = sc.CeilHeight.ToString();
			floortex.TextureName = sc.FloorTexture;
			ceilingtex.TextureName = sc.CeilTexture;

			// Action
			tag.Text = sc.Tag.ToString();

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

				// Action
				if(s.Tag.ToString() != tag.Text) tag.Text = "";

				// Custom fields
				fieldslist.SetValues(s.Fields, false);
			}
		}

		// OK clicked
		private void apply_Click(object sender, EventArgs e)
		{
			string undodesc = "sector";

			// Make undo
			if(sectors.Count > 1) undodesc = sectors.Count + " sectors";
			General.Map.UndoRedo.CreateUndo("Edit " + undodesc, UndoGroup.None, 0);

			// Go for all sectors
			foreach(Sector s in sectors)
			{
				// Effects
				if(!effect.Empty) s.Effect = effect.Value;
				s.Brightness = brightness.GetResult(s.Brightness);

				// Floor/Ceiling
				s.FloorHeight = floorheight.GetResult(s.FloorHeight);
				s.CeilHeight = ceilingheight.GetResult(s.CeilHeight);
				s.SetFloorTexture(floortex.GetResult(s.FloorTexture));
				s.SetCeilTexture(ceilingtex.GetResult(s.CeilTexture));

				// Action
				s.Tag = tag.GetResult(s.Tag);

				// Custom fields
				fieldslist.Apply(s.Fields);
			}

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

		// This finds a new (unused) tag
		private void newtag_Click(object sender, EventArgs e)
		{
			tag.Text = General.Map.Map.GetNewTag().ToString();
		}

		// Browse Effect clicked
		private void browseeffect_Click(object sender, EventArgs e)
		{
			effect.Value = EffectBrowserForm.BrowseEffect(this, effect.Value);
		}
	}
}