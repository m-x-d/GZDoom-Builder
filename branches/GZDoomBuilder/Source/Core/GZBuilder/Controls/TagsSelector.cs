#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.GZBuilder.Controls
{
	public partial class TagsSelector : UserControl
	{
		#region ================== Structs

		private struct TagLinkData
		{
			public int Index;
			public int Tag;
		}

		#endregion

		#region ================== Variables

		private List<int> usedtags; //tags already used in the map
		private List<TagInfo> infos;
		private List<int> tags; //tags being edited
		private List<int> rangemodes; //0 - none, 1 - positive (>=), -1 - negative (<=)
		private List<int> offsetmodes; //0 - none, 1 - positive (++), -1 - negative (--)
		private UniversalType elementtype;
		private const string TAGS_SEPARATOR = ", ";
		private int curtagindex;
		private bool blockupdate;

		#endregion

		#region ================== Constructor

		public TagsSelector()
		{
			InitializeComponent();
		}

		#endregion

		#region ================== Setup

		public void Setup(UniversalType mapelementtype)
		{
			tags = new List<int>();
			usedtags = new List<int>();
			rangemodes = new List<int>();
			offsetmodes = new List<int>();
			infos = new List<TagInfo>();
			elementtype = mapelementtype;

			//collect used tags from appropriate element type...
			switch(elementtype)
			{
				case UniversalType.SectorTag:
					foreach(Sector s in General.Map.Map.Sectors)
					{
						foreach(int tag in s.Tags)
						{
							if(tag == 0 || usedtags.Contains(tag)) continue;
							usedtags.Add(tag);
						}
					}
					break;

				case UniversalType.LinedefTag:
					if(General.Map.FormatInterface.HasLinedefTag)
					{
						foreach(Linedef l in General.Map.Map.Linedefs)
						{
							foreach(int tag in l.Tags)
							{
								if(tag == 0 || usedtags.Contains(tag)) continue;
								usedtags.Add(tag);
							}
						}
					}
					break;

				default:
					throw new NotSupportedException(elementtype + " doesn't support 'moreids' property!");
			}

			//now sort them in descending order
			usedtags.Sort((a, b) => -1 * a.CompareTo(b));

			//create tag infos
			foreach(int tag in usedtags)
			{
				if(General.Map.Options.TagLabels.ContainsKey(tag)) //tag labels
					infos.Add(new TagInfo(tag, General.Map.Options.TagLabels[tag]));
				else
					infos.Add(new TagInfo(tag, string.Empty));
			}

			foreach(TagInfo info in infos) tagpicker.Items.Add(info);
			tagpicker.DropDownWidth = DoomBuilder.Geometry.Tools.GetDropDownWidth(tagpicker);
		}

		// Update collections and controls
		public void FinishSetup()
		{
			if(tags.Count == 0) tags.Add(0);

			// Initialize modifier modes
			for(int i = 0; i < tags.Count; i++)
			{
				rangemodes.Add(0);
				offsetmodes.Add(0);
			}

			// Update controls
			UpdateTagPicker(tags[0]);
			UpdateTagsList();
			removetag.Enabled = (tags.Count > 1);
		}

		public void SetValue(List<int> newtags, bool first)
		{
			if(first)
			{
				foreach(int tag in newtags) tags.Add(tag);
				return;
			}

			for(int i = 0; i < newtags.Count; i++)
			{
				if(i < tags.Count && newtags[i] != tags[i])
					tags[i] = int.MinValue;
				else if(i >= tags.Count)
					tags.Add(int.MinValue);
			}
		}

		#endregion

		#region ================== Apply

		public void ApplyTo(Linedef mo, int offset)
		{
			int[] oldtags = new int[mo.Tags.Count];
			mo.Tags.CopyTo(oldtags);
			mo.Tags.Clear();
			mo.Tags.AddRange(GetResultTags(oldtags, offset));
		}

		public void ApplyTo(Sector mo, int offset)
		{
			int[] oldtags = new int[mo.Tags.Count];
			mo.Tags.CopyTo(oldtags);
			mo.Tags.Clear();
			mo.Tags.AddRange(GetResultTags(oldtags, offset));
		}

		private IEnumerable<int> GetResultTags(int[] oldtags, int offset)
		{
			Dictionary<int, bool> newtags = new Dictionary<int, bool>();

			for(int i = 0; i < tags.Count; i++)
			{
				if(tags[i] == int.MinValue && oldtags.Length > i)
				{
					if(!newtags.ContainsKey(oldtags[i])) newtags.Add(oldtags[i], false);
				}
				else if(tags[i] != 0 && tags[i] != int.MinValue)
				{
					int tag;
					if(rangemodes[i] != 0)
						tag = tags[i] + offset * rangemodes[i];
					else if(offsetmodes[i] != 0 && oldtags.Length > i)
						tag = oldtags[i] + tags[i] * offsetmodes[i];
					else
						tag = tags[i];

					if(!newtags.ContainsKey(tag)) newtags.Add(tag, false);
				}
			}

			if(newtags.Count == 0) newtags.Add(0, false);
			return newtags.Keys;
		}

		#endregion

		#region ================== Methods

		private void UpdateTagsList()
		{
			string[] displaytags = new string[tags.Count];
			int displaytagslen = 0;
			tagslist.Links.Clear();

			// Gather tags into a single string collection
			for(int i = 0; i < tags.Count; i++)
			{
				displaytags[i] = (tags[i] == int.MinValue ? "???" : tags[i].ToString());

				// Add modify mode markers
				if(offsetmodes[i] == -1) displaytags[i] = "--" + displaytags[i];
				else if(offsetmodes[i] == 1) displaytags[i] = "++" + displaytags[i];
				else if(rangemodes[i] == -1) displaytags[i] = "<=" + displaytags[i];
				else if(rangemodes[i] == 1) displaytags[i] = ">=" + displaytags[i];

				// Add selection indictor
				if(curtagindex == i) displaytags[i] = "[" + displaytags[i] + "]";
				else displaytags[i] = " " + displaytags[i] + " ";

				int start = displaytagslen + i * TAGS_SEPARATOR.Length;
				tagslist.Links.Add(new LinkLabel.Link(start, displaytags[i].Length, new TagLinkData { Index = i, Tag = tags[i] }));
				displaytagslen += displaytags[i].Length;
			}

			// Create label text
			tagslist.Text = string.Join(TAGS_SEPARATOR, displaytags);

			// Update current tag label
			curtaglabel.Text = "Tag " + (curtagindex + 1) + ":";
		}

		private void UpdateTagPicker(int tag)
		{
			blockupdate = true;

			tagpicker.SelectedIndex = -1;

			if(tag == int.MinValue)
			{
				tagpicker.Text = string.Empty;
			}
			else if(rangemodes[curtagindex] != 0 && tag != 0)
			{
				tagpicker.Text = (rangemodes[curtagindex] == 1 ? ">=" : "<=") + tag;
			}
			else if(offsetmodes[curtagindex] != 0 && tag != 0)
			{
				tagpicker.Text = (offsetmodes[curtagindex] == 1 ? "++" : "--") + tag;
			}
			else
			{
				foreach(var item in tagpicker.Items)
					if(((TagInfo)item).Tag == tag) tagpicker.SelectedItem = item;

				if(tagpicker.SelectedIndex == -1) tagpicker.Text = tag.ToString();
			}

			blockupdate = false;
		}

		#endregion

		#region ================== Events

		private void newtag_Click(object sender, EventArgs e)
		{
			tagpicker.SelectedIndex = -1;
			tagpicker.Text = General.Map.Map.GetNewTag().ToString();
		}

		private void unusedtag_Click(object sender, EventArgs e)
		{
			tagpicker.SelectedIndex = -1;
			tagpicker.Text = General.Map.Map.GetNewTag(elementtype).ToString();
		}

		private void clear_Click(object sender, EventArgs e)
		{
			tagpicker.SelectedIndex = -1;
			tagpicker.Text = "0";
		}

		private void addtag_Click(object sender, EventArgs e)
		{
			// When an item has no tags, act like "New Tag" button
			if(tags.Count == 1 && tags[0] == 0)
			{
				newtag_Click(sender, e);
				return;
			}
			
			int nt = General.Map.Map.GetNewTag(tags);
			tags.Add(nt);
			rangemodes.Add(0);
			offsetmodes.Add(0);
			curtagindex = tags.Count - 1;

			// Update controls
			blockupdate = true;
			tagpicker.Text = nt.ToString();
			blockupdate = false;

			removetag.Enabled = true;
			UpdateTagsList();
		}

		private void removetag_Click(object sender, EventArgs e)
		{
			tags.RemoveAt(curtagindex);
			rangemodes.RemoveAt(curtagindex);
			offsetmodes.RemoveAt(curtagindex);
			if(curtagindex >= tags.Count) curtagindex = tags.Count - 1;

			// Update controls
			UpdateTagPicker(tags[curtagindex]);

			removetag.Enabled = (tags.Count > 1);
			UpdateTagsList();
		}

		private void clearalltags_Click(object sender, EventArgs e)
		{
			curtagindex = 0;
			
			// Clear collections
			tags.Clear();
			tags.Add(0);
			rangemodes.Clear();
			rangemodes.Add(0);
			offsetmodes.Clear();
			offsetmodes.Add(0);

			// Update controls
			blockupdate = true;
			tagpicker.Text = "0";
			blockupdate = false;

			removetag.Enabled = false;
			UpdateTagsList();
		}

		private void tagslist_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			TagLinkData data = (TagLinkData)e.Link.LinkData;
			curtagindex = data.Index;
			curtaglabel.Text = "Tag " + (curtagindex + 1) + ":";

			// Update interface
			UpdateTagPicker(data.Tag);
			UpdateTagsList();
		}

		private void tagpicker_TextChanged(object sender, EventArgs e)
		{
			if(blockupdate) return;

			if(tagpicker.SelectedItem != null)
			{
				TagInfo info = (TagInfo)tagpicker.SelectedItem;
				tags[curtagindex] = info.Tag;
				UpdateTagsList();
				return;
			}

			string text = tagpicker.Text.Trim();
			if(string.IsNullOrEmpty(text))
			{
				tags[curtagindex] = int.MinValue;
				UpdateTagsList();
				return;
			}

			//incremental?
			int rangemode = 0;
			int offsetmode = 0;
			if(text.Length > 2)
			{
				if(text.StartsWith(">=")) //range up
				{
					rangemode = 1;
					text = text.Substring(2, text.Length - 2);
				}
				else if(text.StartsWith("<=")) //range down
				{
					rangemode = -1;
					text = text.Substring(2, text.Length - 2);
				}
				else if(text.StartsWith("++")) //relative up
				{
					offsetmode = 1;
					text = text.Substring(2, text.Length - 2);
				}
				else if(text.StartsWith("--")) //relative down
				{
					offsetmode = -1;
					text = text.Substring(2, text.Length - 2);
				}
			} 

			int tag;
			if(int.TryParse(text, out tag))
			{
				// Validate entered tag
				if((tag < General.Map.FormatInterface.MinTag) || (tag > General.Map.FormatInterface.MaxTag))
				{
					General.ShowWarningMessage("Sector tag must be between " + General.Map.FormatInterface.MinTag + " and " + General.Map.FormatInterface.MaxTag + ".", MessageBoxButtons.OK);
					return;
				}

				tags[curtagindex] = tag;
				rangemodes[curtagindex] = rangemode;
				offsetmodes[curtagindex] = offsetmode;
				UpdateTagsList();
			}
		}

		#endregion
	}
}