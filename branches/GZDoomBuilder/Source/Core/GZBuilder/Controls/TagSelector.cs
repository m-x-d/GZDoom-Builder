using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using System.Globalization;

namespace CodeImp.DoomBuilder.GZBuilder.Controls
{
	internal struct TagInfo
	{
		public string Label;
		public int Tag;
		public bool HasLabel;

		public TagInfo(int tag, string label) {
			if(string.IsNullOrEmpty(label)) {
				Label = "Tag " + tag;
				HasLabel = false;
			} else {
				Label = label;
				HasLabel = true;
			}

			Tag = tag;
		}

		public override string ToString() {
			if(HasLabel) return Label + " (tag " + Tag + ")";
			return Label;
		}
	}
	
	public partial class TagSelector : UserControl
	{
		private List<int> tags;
		private List<TagInfo> infos;
		private bool valid;
		private int tag;
		
		public TagSelector() {
			InitializeComponent();
		}

		public void Setup() {
			tags = new List<int>();
			infos = new List<TagInfo>();

			//collect used tags from sectors...
			foreach(Sector s in General.Map.Map.Sectors) {
				if(s.Tag == 0 || tags.Contains(s.Tag)) continue;
				tags.Add(s.Tag);
			}

			//...and linedefs...
			if(General.Map.FormatInterface.HasLinedefTag) {
				foreach(Linedef l in General.Map.Map.Linedefs) {
					if(l.Tag == 0 || tags.Contains(l.Tag)) continue;
					tags.Add(l.Tag);
				}
			}

			//...and things...
			if(General.Map.FormatInterface.HasThingTag) {
				foreach(Thing t in General.Map.Map.Things) {
					if(t.Tag == 0 || tags.Contains(t.Tag)) continue;
					tags.Add(t.Tag);
				}
			}

			//now sort them
			tags.Sort();

			//create tag infos
			foreach(int tag in tags){
				if(General.Map.Options.TagLabels.ContainsKey(tag)) //tag labels
					infos.Add(new TagInfo(tag, General.Map.Options.TagLabels[tag]));
				else
					infos.Add(new TagInfo(tag, string.Empty));
			}

			foreach(TagInfo info in infos)
				cbTagPicker.Items.Add(info);
		}

		public void SetTag(int newTag) {
			if(tags.Contains(newTag)) {
				cbTagPicker.SelectedIndex = tags.IndexOf(newTag);
			} else {
				cbTagPicker.Text = newTag.ToString();
			}
			tag = newTag;
			valid = true;
		}

		public void ClearTag() {
			cbTagPicker.SelectedIndex = -1;
			cbTagPicker.Text = string.Empty;
			valid = false;
		}

		public int GetTag(int original) {
			if(!valid) return original;
			return tag;
		}

		public void ValidateTag() {
			if(cbTagPicker.SelectedIndex != -1) {
				tag = infos[cbTagPicker.SelectedIndex].Tag;
				valid = true;
				return;
			}

			if(string.IsNullOrEmpty(cbTagPicker.Text)) {
				valid = false;
				return;
			}

			//check text
			string text = cbTagPicker.Text.Trim().ToLowerInvariant();

			if(!int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out tag)) {
				//maybe it's user-pasted label?
				foreach(TagInfo info in infos) {
					if(info.Label.ToLowerInvariant().Contains(text)) {
						tag = info.Tag;
						valid = true;
						return;
					}
				}
				
				tag = General.Map.Map.GetNewTag();

				//create new tag label
				if(General.Map.Options.TagLabels.ContainsKey(tag))
					General.Map.Options.TagLabels[tag] = cbTagPicker.Text.Trim();
				else
					General.Map.Options.TagLabels.Add(tag, cbTagPicker.Text.Trim());

				valid = true;
				return;
			}

			valid = true;
		}

		private void newTag_Click(object sender, EventArgs e) {
			//todo: check tag labels?
			tag = General.Map.Map.GetNewTag();
			cbTagPicker.Text = tag.ToString();
			valid = true;
		}
	}
}
