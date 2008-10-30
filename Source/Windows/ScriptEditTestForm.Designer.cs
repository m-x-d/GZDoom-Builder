namespace CodeImp.DoomBuilder.Windows
{
	partial class ScriptEditTestForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.scriptedit = new CodeImp.DoomBuilder.Controls.ScriptEditControl();
			this.SuspendLayout();
			// 
			// scriptedit
			// 
			this.scriptedit.AnchorPosition = 0;
			this.scriptedit.AutoCSeparator = 0;
			this.scriptedit.AutoCTypeSeparator = 0;
			this.scriptedit.BackColor = System.Drawing.SystemColors.Window;
			this.scriptedit.CaretFore = 0;
			this.scriptedit.CaretLineBack = 0;
			this.scriptedit.CaretPeriod = 0;
			this.scriptedit.CaretWidth = 0;
			this.scriptedit.CodePage = 0;
			this.scriptedit.ControlCharSymbol = 0;
			this.scriptedit.CurrentPos = 0;
			this.scriptedit.CursorType = 0;
			this.scriptedit.DocPointer = 0;
			this.scriptedit.EdgeColour = 0;
			this.scriptedit.EdgeColumn = 0;
			this.scriptedit.EdgeMode = 0;
			this.scriptedit.EndAtLastLine = 0;
			this.scriptedit.EndOfLineMode = CodeImp.DoomBuilder.Controls.ScriptEndOfLine.CRLF;
			this.scriptedit.EOLMode = 0;
			this.scriptedit.HighlightGuide = 0;
			this.scriptedit.Indent = 0;
			this.scriptedit.IsAutoCGetAutoHide = false;
			this.scriptedit.IsAutoCGetCancelAtStart = false;
			this.scriptedit.IsAutoCGetChooseSingle = false;
			this.scriptedit.IsAutoCGetDropRestOfWord = false;
			this.scriptedit.IsAutoCGetIgnoreCase = false;
			this.scriptedit.IsBackSpaceUnIndents = false;
			this.scriptedit.IsBufferedDraw = false;
			this.scriptedit.IsCaretLineVisible = false;
			this.scriptedit.IsFocus = false;
			this.scriptedit.IsHScrollBar = false;
			this.scriptedit.IsIndentationGuides = false;
			this.scriptedit.IsMouseDownCaptures = false;
			this.scriptedit.IsOvertype = false;
			this.scriptedit.IsReadOnly = false;
			this.scriptedit.IsTabIndents = false;
			this.scriptedit.IsTwoPhaseDraw = false;
			this.scriptedit.IsUndoCollection = false;
			this.scriptedit.IsUsePalette = false;
			this.scriptedit.IsUseTabs = false;
			this.scriptedit.IsViewEOL = false;
			this.scriptedit.IsVScrollBar = false;
			this.scriptedit.LayoutCache = 0;
			this.scriptedit.Lexer = 0;
			this.scriptedit.Location = new System.Drawing.Point(12, 12);
			this.scriptedit.MarginLeft = 0;
			this.scriptedit.MarginRight = 0;
			this.scriptedit.ModEventMask = 0;
			this.scriptedit.MouseDwellTime = 0;
			this.scriptedit.Name = "scriptedit";
			this.scriptedit.PrintColourMode = 0;
			this.scriptedit.PrintMagnification = 0;
			this.scriptedit.PrintWrapMode = 0;
			this.scriptedit.ScrollWidth = 0;
			this.scriptedit.SearchFlags = 0;
			this.scriptedit.SelectionEnd = 0;
			this.scriptedit.SelectionMode = 0;
			this.scriptedit.SelectionStart = 0;
			this.scriptedit.Size = new System.Drawing.Size(643, 487);
			this.scriptedit.Status = 0;
			this.scriptedit.StyleBits = 0;
			this.scriptedit.TabIndex = 0;
			this.scriptedit.TabWidth = 0;
			this.scriptedit.TargetEnd = 0;
			this.scriptedit.TargetStart = 0;
			this.scriptedit.ViewWhitespace = CodeImp.DoomBuilder.Controls.ScriptWhiteSpace.Invisible;
			this.scriptedit.ViewWS = 0;
			this.scriptedit.WrapMode = 0;
			this.scriptedit.WrapStartIndent = 0;
			this.scriptedit.WrapVisualFlags = 0;
			this.scriptedit.WrapVisualFlagsLocation = 0;
			this.scriptedit.XOffset = 0;
			this.scriptedit.ZoomLevel = 0;
			// 
			// ScriptEditTestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(667, 511);
			this.Controls.Add(this.scriptedit);
			this.Name = "ScriptEditTestForm";
			this.Text = "ScriptEditTestForm";
			this.ResumeLayout(false);

		}

		#endregion

		private CodeImp.DoomBuilder.Controls.ScriptEditControl scriptedit;

	}
}