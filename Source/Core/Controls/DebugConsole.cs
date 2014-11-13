#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder
{
	#region ================== Enums

	[Flags]
	public enum DebugMessageType
	{
		Log = 1,
		Info = 2,
		Warning = 4,
		Error = 8,
		Special = 16,
	}

	#endregion

	public partial class DebugConsole : UserControl
	{
		#region ================== Variables

		private static readonly List<KeyValuePair<DebugMessageType, string>> messages = new List<KeyValuePair<DebugMessageType, string>>(1000);
		private DebugMessageType filters;

		//Colors
		private readonly Dictionary<DebugMessageType, Color> textcolors;
		private readonly Dictionary<DebugMessageType, string> textheaders;

		private static int charcount;
		private const int MAX_CHARS = short.MaxValue;
		private static long starttime = -1;
		private static DebugConsole me;

		#endregion

		#region ================== Properties

		public bool AlwaysOnTop { get { return alwaysontop.Checked; } }

		#endregion

		#region ================== Constructor

		public DebugConsole() 
		{
			InitializeComponent();
			me = this;

			// Setup filters
			foreach (ToolStripMenuItem item in filterselector.DropDownItems)
			{
				UpdateFilters(item);
			}

			// Setup colors
			textcolors = new Dictionary<DebugMessageType, Color> {
				             { DebugMessageType.Log, SystemColors.WindowText }, 
							 { DebugMessageType.Info, Color.DarkGreen }, 
							 { DebugMessageType.Warning, Color.DarkOrange }, 
							 { DebugMessageType.Error, Color.DarkRed }, 
							 { DebugMessageType.Special, Color.DarkMagenta }
			             };

			// Setup headers
			textheaders = new Dictionary<DebugMessageType, string> {
				              { DebugMessageType.Log, string.Empty}, 
							  { DebugMessageType.Info, string.Empty}, 
							  { DebugMessageType.Warning, "Warning: "}, 
							  { DebugMessageType.Error, "ERROR: "}, 
							  { DebugMessageType.Special, string.Empty}
			              };

			// Word wrap?
			wordwrap.Checked = console.WordWrap;

			// Pending messages?
			if (messages.Count > 0) UpdateMessages();
		}

		#endregion

		#region ================== Methods

		public static void Write(string text)
		{
			Write(DebugMessageType.Info, text);
		}

		public static void WriteLine(string text) 
		{
			Write(DebugMessageType.Info, text + Environment.NewLine);
		}

		public static void Write(DebugMessageType type, string text)
		{
			if(me != null && me.InvokeRequired) 
			{
				me.Invoke(new Action<DebugMessageType, string>(Write), new object[] { type, text });
			} 
			else 
			{
				messages.Add(new KeyValuePair<DebugMessageType, string>(type, text));
				if(me != null && (me.filters & type) == type) 
				{
					me.AddMessage(type, text, true);
				}
			}
		}

		public static void WriteLine(DebugMessageType type, string text)
		{
			Write(type, text + Environment.NewLine);
		}

		public static void Clear()
		{
			if (me != null && me.InvokeRequired)
			{
				me.Invoke(new Action(Clear));
			}
			else
			{
				if (me != null) me.console.Clear();
				messages.Clear();
				charcount = 0;
			}
		}

		public static void StartTimer() 
		{
			starttime = SlimDX.Configuration.Timer.ElapsedMilliseconds;
		}

		public static void StopTimer(string message) 
		{
			if (starttime == -1) 
			{
				Write(DebugMessageType.Warning, "Call General.Console.StartTimer before General.Console.StopTimer!");
			}
			else 
			{
				long endtime = SlimDX.Configuration.Timer.ElapsedMilliseconds;
				
				if (message.Contains("%"))
					message = message.Replace("&", (endtime - starttime) + " ms.");
				else 
					message = message.TrimEnd() + " " + (endtime - starttime) + " ms.";

				Write(DebugMessageType.Special, message);
			}

			starttime = -1;
		}

		private void AddMessage(DebugMessageType type, string text, bool scroll)
		{
			text = textheaders[type] + text;
			bool updatemessages = false;

			while (charcount + text.Length > MAX_CHARS) 
			{
				charcount -= messages[0].Value.Length;
				messages.RemoveAt(0);
				updatemessages = true;
			}

			if(updatemessages) UpdateMessages();
			charcount += text.Length; 

			console.SelectionStart = console.TextLength;
			console.SelectionColor = textcolors[type];
			console.AppendText(text);
			if(scroll && autoscroll.Checked) console.ScrollToCaret();
		}

		private void UpdateFilters(ToolStripMenuItem item)
		{
			DebugMessageType flag = (DebugMessageType)(int)item.Tag;
			if(item.Checked) 
			{
				filters |= flag;
			} 
			else 
			{
				filters &= ~flag;
			}
		}

		private void UpdateMessages()
		{
			console.Clear();
			charcount = 0;

			console.SuspendLayout();
			foreach (KeyValuePair<DebugMessageType, string> pair in messages)
			{
				if((filters & pair.Key) == pair.Key && CheckTextFilter(pair.Value, searchbox.Text))
				{
					AddMessage(pair.Key, pair.Value, false);
				}
			}

			console.ResumeLayout();
			console.ScrollToCaret();
		}

		// Should we display this message?
		private bool CheckTextFilter(string text, string filter) 
		{
			if (string.IsNullOrEmpty(filter) || filter.Length < 3) return true;
			return text.ToUpperInvariant().Contains(filter.ToUpperInvariant());
		}

		#endregion

		#region ================== Events

		private void clearall_Click(object sender, EventArgs e) 
		{
			Clear();
		}

		private void filters_Click(object sender, EventArgs e)
		{
			UpdateFilters(sender as ToolStripMenuItem);
			UpdateMessages();
		}

		private void wordwrap_Click(object sender, EventArgs e) 
		{
			console.WordWrap = wordwrap.Checked;
		}

		#endregion

		#region ================== Search events

		private void searchclear_Click(object sender, EventArgs e) 
		{
			searchbox.Clear();
		}

		private void searchbox_TextChanged(object sender, EventArgs e) 
		{
			if(string.IsNullOrEmpty(searchbox.Text) || searchbox.Text.Length > 2) UpdateMessages();
		}

		#endregion

	}
}
