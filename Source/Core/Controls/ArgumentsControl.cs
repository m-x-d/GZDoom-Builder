#region ================== Namespaces

using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Types;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	public partial class ArgumentsControl : UserControl
	{
		#region ================== Native stuff

		[DllImport("user32.dll")]
		private static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);
		
		private const int WM_SETREDRAW = 11;

		#endregion

		#region ================== Enums

		private enum ArgZeroMode
		{
			DEFAULT,
			SCRIPT_NUMBER,
			SCRIPT_NAME,
		}

		#endregion

		#region ================== Variables

		private string arg0str;
		private bool havearg0str;
		private int action;
		private ArgumentInfo[] arginfo;
		private ArgZeroMode argzeromode;

		#endregion

		#region ================== Constructor

		public ArgumentsControl()
		{
			InitializeComponent();

            Reset();
		}

        #endregion

        #region ================== Setup

        public void Reset()
        {
            // Only when running (this.DesignMode won't do when not this, but one of parent controls is in design mode)
            if (LicenseManager.UsageMode != LicenseUsageMode.Designtime)
            {
                //mxd. Setup script numbers
                scriptnumbers.Location = new Point(arg0.Location.X, arg0.Location.Y + 2);
                scriptnumbers.Items.Clear();
                foreach (ScriptItem si in General.Map.NumberedScripts.Values)
                    scriptnumbers.Items.Add(new ColoredComboBoxItem(si, si.IsInclude ? SystemColors.HotTrack : SystemColors.WindowText));
                scriptnumbers.DropDownWidth = Tools.GetDropDownWidth(scriptnumbers);

                //mxd. Setup script names
                if (General.Map.UDMF)
                {
                    scriptnames.Items.Clear();
                    scriptnames.Location = scriptnumbers.Location;
                    foreach (ScriptItem nsi in General.Map.NamedScripts.Values)
                        scriptnames.Items.Add(new ColoredComboBoxItem(nsi, nsi.IsInclude ? SystemColors.HotTrack : SystemColors.WindowText));
                    scriptnames.DropDownWidth = Tools.GetDropDownWidth(scriptnames);
                }
                else
                {
                    scriptnames.Visible = false;
                    cbuseargstr.Visible = false;
                }
            }
        }

		public void SetValue(Linedef l, bool first)
		{
			SetValue(l.Fields, l.Args, first);
		}

		public void SetValue(Thing t, bool first)
		{
			SetValue(t.Fields, t.Args, first);
		}

		private void SetValue(UniFields fields, int[] args, bool first)
		{
			if(first)
			{
				if(General.Map.UDMF)
				{
					arg0str = fields.GetValue("arg0str", string.Empty);
					havearg0str = !string.IsNullOrEmpty(arg0str);
				}

				// Update arguments
				arg0.SetValue(args[0]);
				arg1.SetValue(args[1]);
				arg2.SetValue(args[2]);
				arg3.SetValue(args[3]);
				arg4.SetValue(args[4]);
			}
			else
			{
				if(General.Map.UDMF)
				{
					if(arg0str != fields.GetValue("arg0str", string.Empty))
					{
						havearg0str = true;
						arg0str = string.Empty;
					}
				}

				// Update arguments
				if(!string.IsNullOrEmpty(arg0.Text) && args[0] != arg0.GetResult(int.MinValue)) arg0.ClearValue();
				if(!string.IsNullOrEmpty(arg1.Text) && args[1] != arg1.GetResult(int.MinValue)) arg1.ClearValue();
				if(!string.IsNullOrEmpty(arg2.Text) && args[2] != arg2.GetResult(int.MinValue)) arg2.ClearValue();
				if(!string.IsNullOrEmpty(arg3.Text) && args[3] != arg3.GetResult(int.MinValue)) arg3.ClearValue();
				if(!string.IsNullOrEmpty(arg4.Text) && args[4] != arg4.GetResult(int.MinValue)) arg4.ClearValue();
			}
		}

		#endregion

		#region ================== Apply

		public void Apply(Linedef l, int step)
		{
			//mxd. Script name/number handling
			// We can't rely on control visibility here, because all controlls will be invisible if ArgumentsControl is invisible
			// (for example, when a different tab is selected)
			switch(argzeromode)
			{
				// Apply arg0str
				case ArgZeroMode.SCRIPT_NAME:
					if(!string.IsNullOrEmpty(scriptnames.Text))
						l.Fields["arg0str"] = new UniValue(UniversalType.String, scriptnames.Text);
					break;

				// Apply script number
				case ArgZeroMode.SCRIPT_NUMBER:
					if(!string.IsNullOrEmpty(scriptnumbers.Text))
					{
						if(scriptnumbers.SelectedItem != null)
							l.Args[0] = ((ScriptItem)((ColoredComboBoxItem)scriptnumbers.SelectedItem).Value).Index;
						else if(!int.TryParse(scriptnumbers.Text.Trim(), out l.Args[0]))
							l.Args[0] = 0;

						if(l.Fields.ContainsKey("arg0str")) l.Fields.Remove("arg0str");
					}
					break;

				// Apply classic arg
				case ArgZeroMode.DEFAULT:
					l.Args[0] = arg0.GetResult(l.Args[0], step);
					if(l.Fields.ContainsKey("arg0str")) l.Fields.Remove("arg0str");
					break;

				default: throw new NotImplementedException("Unknown ArgZeroMode");
			}

			// Apply the rest of args
			l.Args[1] = arg1.GetResult(l.Args[1], step);
			l.Args[2] = arg2.GetResult(l.Args[2], step);
			l.Args[3] = arg3.GetResult(l.Args[3], step);
			l.Args[4] = arg4.GetResult(l.Args[4], step);
		}

		public void Apply(Thing t, int step)
		{
			//mxd. Script name/number handling
			// We can't rely on control visibility here, because all controlls will be invisible if ArgumentsControl is invisible
			// (for example, when a different tab is selected)
			switch(argzeromode)
			{
				// Apply arg0str
				case ArgZeroMode.SCRIPT_NAME:
					if(!string.IsNullOrEmpty(scriptnames.Text))
						t.Fields["arg0str"] = new UniValue(UniversalType.String, scriptnames.Text);
					break;

				// Apply script number
				case ArgZeroMode.SCRIPT_NUMBER:
					if(!string.IsNullOrEmpty(scriptnumbers.Text))
					{
						if(scriptnumbers.SelectedItem != null)
							t.Args[0] = ((ScriptItem)((ColoredComboBoxItem)scriptnumbers.SelectedItem).Value).Index;
						else if(!int.TryParse(scriptnumbers.Text.Trim(), out t.Args[0]))
							t.Args[0] = 0;

						if(t.Fields.ContainsKey("arg0str")) t.Fields.Remove("arg0str");
					}
					break;

				// Apply classic arg
				case ArgZeroMode.DEFAULT:
					t.Args[0] = arg0.GetResult(t.Args[0], step);
					if(t.Fields.ContainsKey("arg0str")) t.Fields.Remove("arg0str");
					break;

				default: throw new NotImplementedException("Unknown ArgZeroMode");
			}

			// Apply the rest of args
			t.Args[1] = arg1.GetResult(t.Args[1], step);
			t.Args[2] = arg2.GetResult(t.Args[2], step);
			t.Args[3] = arg3.GetResult(t.Args[3], step);
			t.Args[4] = arg4.GetResult(t.Args[4], step);
		}

		#endregion

		#region ================== Update

		public void UpdateAction(int action, bool setuponly)
		{
			UpdateAction(action, setuponly, null);
		}

		public void UpdateAction(int action, bool setuponly, ThingTypeInfo info)
		{
			// Update arguments
			int showaction = 0;
			ArgumentInfo[] oldarginfo = (arginfo != null ? (ArgumentInfo[])arginfo.Clone() : null); //mxd

			// Only when action type is known
			if(General.Map.Config.LinedefActions.ContainsKey(action)) showaction = action;
			
			// Update argument infos
			if((showaction == 0) && (info != null)) arginfo = info.Args;
			else arginfo = General.Map.Config.LinedefActions[showaction].Args;

			// Don't update action args when thing type is changed
			if(info != null && showaction != 0 && this.action == showaction) return;

			//mxd. Don't update action args when old and new argument infos match
			if(arginfo != null && oldarginfo != null && ArgumentInfosMatch(arginfo, oldarginfo)) return;

			// Change the argument descriptions
			this.BeginUpdate();

			UpdateArgument(arg0, arg0label, arginfo[0]);
			UpdateArgument(arg1, arg1label, arginfo[1]);
			UpdateArgument(arg2, arg2label, arginfo[2]);
			UpdateArgument(arg3, arg3label, arginfo[3]);
			UpdateArgument(arg4, arg4label, arginfo[4]);

			if(!setuponly)
			{
				// Apply action's or thing's default arguments
				if(showaction != 0 || info != null)
				{
					arg0.SetDefaultValue();
					arg1.SetDefaultValue();
					arg2.SetDefaultValue();
					arg3.SetDefaultValue();
					arg4.SetDefaultValue();
				}
				else //or set them to 0
				{
					arg0.SetValue(0);
					arg1.SetValue(0);
					arg2.SetValue(0);
					arg3.SetValue(0);
					arg4.SetValue(0);
				}
			}

			// Store current action
			this.action = showaction;

			this.EndUpdate();
		}

		public void UpdateScriptControls()
		{
			// Update script-specific stuff
			if(Array.IndexOf(GZBuilder.GZGeneral.ACS_SPECIALS, action) != -1)
			{
				// Update script controls visibility
				bool shownamedscripts = (General.Map.UDMF && havearg0str);
				cbuseargstr.Visible = General.Map.UDMF;
				cbuseargstr.Checked = shownamedscripts;
				scriptnames.Visible = shownamedscripts;
				scriptnumbers.Visible = !shownamedscripts;

				// Update named script name
				if(shownamedscripts)
				{
					argzeromode = ArgZeroMode.SCRIPT_NAME;
					scriptnames.Text = arg0str;
					
					if(General.Map.NamedScripts.ContainsKey(arg0str))
						UpdateScriptArguments(General.Map.NamedScripts[arg0str]);
					else
						arg0label.Text = "Script Name:"; // Unknown script name
				}
				// Update numbered script name
				else
				{
					argzeromode = ArgZeroMode.SCRIPT_NUMBER;
					int a0 = arg0.GetResult(0);
					if(General.Map.NumberedScripts.ContainsKey(a0))
					{
						int i = 0;
						foreach(ScriptItem item in General.Map.NumberedScripts.Values)
						{
							if(item.Index == a0)
							{
								scriptnumbers.SelectedIndex = i;
								UpdateScriptArguments(item);
								break;
							}

							i++;
						}
					}
					else
					{
						// Unknown script number...
						scriptnumbers.Text = a0.ToString();
					}
				}
			}
			else
			{
				cbuseargstr.Visible = false;
				scriptnames.Visible = false;
				scriptnumbers.Visible = false;
				cbuseargstr.Checked = false;
				argzeromode = ArgZeroMode.DEFAULT;
			}

			arg0.Visible = (argzeromode == ArgZeroMode.DEFAULT);
		}

		private void UpdateArgument(ArgumentBox arg, Label label, ArgumentInfo info)
		{
			// Update labels
			label.Text = info.Title + ":";
			label.Enabled = info.Used;
			arg.ForeColor = (label.Enabled ? SystemColors.WindowText : SystemColors.GrayText);
			arg.Setup(info);

			// Update tooltip
			UpdateToolTip(label, info);
		}

		private void UpdateToolTip(Label label, ArgumentInfo info)
		{
			if(info.Used && !string.IsNullOrEmpty(info.ToolTip))
			{
				tooltip.SetToolTip(label, info.ToolTip);
				label.Font = new Font(label.Font, FontStyle.Underline);
				label.ForeColor = SystemColors.HotTrack;
			}
			else
			{
				tooltip.SetToolTip(label, null);
				label.Font = new Font(label.Font, FontStyle.Regular);
				label.ForeColor = SystemColors.WindowText;
			}
		}

		private void UpdateScriptArguments(ScriptItem item)
		{
			Label[] labels = { arg0label, arg1label, arg2label, arg3label, arg4label };
			ArgumentBox[] args = { arg0, arg1, arg2, arg3, arg4 };
			if(item != null)
			{
				string[] argnames = item.GetArgumentsDescriptions(action);
				for(int i = 0; i < labels.Length; i++)
				{
					if(!string.IsNullOrEmpty(argnames[i]))
					{
						labels[i].Text = argnames[i] + ":";
						labels[i].Enabled = true;
						labels[i].Font = new Font(labels[i].Font, FontStyle.Regular);
						labels[i].ForeColor = SystemColors.WindowText;
					}
					else
					{
						labels[i].Text = arginfo[i].Title + ":";
						labels[i].Enabled = arginfo[i].Used;
						UpdateToolTip(labels[i], arginfo[i]);
					}

					args[i].ForeColor = (labels[i].Enabled ? SystemColors.WindowText : SystemColors.GrayText);
				}
			}
			else
			{
				for(int i = 0; i < labels.Length; i++)
				{
					labels[i].Text = arginfo[i].Title + ":";
					labels[i].Enabled = arginfo[i].Used;
					UpdateToolTip(labels[i], arginfo[i]);
					args[i].ForeColor = (labels[i].Enabled ? SystemColors.WindowText : SystemColors.GrayText);
				}
			}
		}

		//mxd
		private static bool ArgumentInfosMatch(ArgumentInfo[] info1, ArgumentInfo[] info2)
		{
			if(info1.Length != info2.Length) return false;
			bool haveusedargs = false; // Arguments should still be reset if all arguments are unused
			
			for(int i = 0; i < info1.Length; i++)
			{
				if(info1[i].Used != info1[2].Used || info1[i].Type != info1[2].Type 
					|| info1[i].Title.ToUpperInvariant() != info2[i].Title.ToUpperInvariant())
					return false;

				haveusedargs |= (info1[i].Used || info1[2].Used);
			}

			return haveusedargs;
		}

		#endregion

		#region ================== Redraw control

		private void BeginUpdate()
		{
			SendMessage(this.Parent.Handle, WM_SETREDRAW, false, 0);
		}

		private void EndUpdate()
		{
			SendMessage(this.Parent.Handle, WM_SETREDRAW, true, 0);
			this.Parent.Refresh();
		}

		#endregion

		#region ================== Events

		private void cbuseargstr_CheckedChanged(object sender, EventArgs e)
		{
			if(!cbuseargstr.Visible) return;
			scriptnames.Visible = cbuseargstr.Checked;
			scriptnumbers.Visible = !cbuseargstr.Checked;
			arg0label.Text = (cbuseargstr.Checked ? "Script Name:" : "Script Number:");
			argzeromode = (cbuseargstr.Checked ? ArgZeroMode.SCRIPT_NAME : ArgZeroMode.SCRIPT_NUMBER);
		}

		private void scriptnumbers_TextChanged(object sender, EventArgs e)
		{
			if(string.IsNullOrEmpty(scriptnumbers.Text)) return;
			ScriptItem item = null;
			if(scriptnumbers.SelectedIndex != -1)
			{
				item = ((ScriptItem)((ColoredComboBoxItem)scriptnumbers.SelectedItem).Value);
			}
			else
			{
				int scriptindex;
				if(int.TryParse(scriptnumbers.Text, out scriptindex) && General.Map.NumberedScripts.ContainsKey(scriptindex))
					item = General.Map.NumberedScripts[scriptindex];
			}

			UpdateScriptArguments(item);
		}

		private void scriptnames_TextChanged(object sender, EventArgs e)
		{
			if(string.IsNullOrEmpty(scriptnames.Text)) return;
			ScriptItem item = null;
			if(scriptnames.SelectedIndex != -1)
			{
				item = ((ScriptItem)((ColoredComboBoxItem)scriptnames.SelectedItem).Value);
			}
			else
			{
				string scriptname = scriptnames.Text.Trim().ToLowerInvariant();
				if(General.Map.NamedScripts.ContainsKey(scriptname))
					item = General.Map.NamedScripts[scriptname];
			}

			UpdateScriptArguments(item);
		}

		#endregion
	}
}
