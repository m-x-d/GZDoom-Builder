#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

#endregion

namespace CodeImp.DoomBuilder
{
	#region ================== Enums

	[Flags]
	public enum DebugMessageType
	{
		LOG = 1,
		INFO = 2,
		WARNING = 4,
		ERROR = 8,
		SPECIAL = 16,
	}

	#endregion

	public partial class DebugConsole : UserControl
	{
		#region ================== Variables

		private const int MAX_MESSAGES = 1024;
		private static readonly List<KeyValuePair<DebugMessageType, string>> messages = new List<KeyValuePair<DebugMessageType, string>>(MAX_MESSAGES);

		//Colors
		private readonly Dictionary<DebugMessageType, Color> textcolors;
		private readonly Dictionary<DebugMessageType, string> textheaders;

		private DebugMessageType filters;
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
				             { DebugMessageType.LOG, SystemColors.WindowText }, 
							 { DebugMessageType.INFO, Color.DarkGreen }, 
							 { DebugMessageType.WARNING, Color.DarkOrange }, 
							 { DebugMessageType.ERROR, Color.DarkRed }, 
							 { DebugMessageType.SPECIAL, Color.DarkMagenta }
			             };

			// Setup headers
			textheaders = new Dictionary<DebugMessageType, string> {
				              { DebugMessageType.LOG, string.Empty}, 
							  { DebugMessageType.INFO, string.Empty}, 
							  { DebugMessageType.WARNING, "Warning: "}, 
							  { DebugMessageType.ERROR, "ERROR: "}, 
							  { DebugMessageType.SPECIAL, string.Empty}
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
			Write(DebugMessageType.INFO, text);
		}

		public static void WriteLine(string text) 
		{
			Write(DebugMessageType.INFO, text + Environment.NewLine);
		}

		public static void Write(DebugMessageType type, string text)
		{
			if(me != null && me.InvokeRequired) 
			{
				me.Invoke(new Action<DebugMessageType, string>(Write), new object[] { type, text });
			} 
			else 
			{
				if (messages.Count + 1 > MAX_MESSAGES) lock (messages) { messages.RemoveAt(0); }
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
			}
		}

		public static void StartTimer() 
		{
			starttime = SlimDX.Configuration.Timer.ElapsedMilliseconds;
		}

		public static void StopTimer(string message) 
		{
			if(starttime == -1) throw new InvalidOperationException("DebugConsole.StartTimer() must be called before DebugConsole.StopTimer()!");

			long duration = SlimDX.Configuration.Timer.ElapsedMilliseconds - starttime;
			
			if (message.Contains("%"))
				message = message.Replace("%", duration.ToString(CultureInfo.InvariantCulture));
			else
				message = message.TrimEnd() + " " + duration + " ms.";

			WriteLine(DebugMessageType.SPECIAL, message);

			starttime = -1;
		}

		private void AddMessage(DebugMessageType type, string text, bool scroll)
		{
			text = textheaders[type] + text;
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
		private static bool CheckTextFilter(string text, string filter) 
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
