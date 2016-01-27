namespace CodeImp.DoomBuilder.Controls
{
	partial class ScriptEditorControl
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.scriptedit = new ScintillaNET.Scintilla();
			this.scriptpanel = new System.Windows.Forms.Panel();
			this.functionbar = new System.Windows.Forms.ComboBox();
			this.scriptpanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// scriptedit
			// 
			this.scriptedit.AutoCIgnoreCase = true;
			this.scriptedit.AutoCMaxHeight = 12;
			this.scriptedit.AutoCOrder = ScintillaNET.Order.Custom;
			this.scriptedit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.scriptedit.CaretWidth = 2;
			this.scriptedit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.scriptedit.ExtraAscent = 1;
			this.scriptedit.ExtraDescent = 1;
			this.scriptedit.FontQuality = ScintillaNET.FontQuality.LcdOptimized;
			this.scriptedit.Location = new System.Drawing.Point(0, 0);
			this.scriptedit.Name = "scriptedit";
			this.scriptedit.ScrollWidth = 200;
			this.scriptedit.Size = new System.Drawing.Size(474, 381);
			this.scriptedit.TabIndex = 0;
			this.scriptedit.TabStop = false;
			this.scriptedit.UseTabs = true;
			this.scriptedit.WhitespaceSize = 2;
			this.scriptedit.TextChanged += new System.EventHandler(this.scriptedit_TextChanged);
			this.scriptedit.CharAdded += new System.EventHandler<ScintillaNET.CharAddedEventArgs>(this.scriptedit_CharAdded);
			this.scriptedit.AutoCCompleted += new System.EventHandler<ScintillaNET.AutoCSelectionEventArgs>(this.scriptedit_AutoCCompleted);
			this.scriptedit.InsertCheck += new System.EventHandler<ScintillaNET.InsertCheckEventArgs>(this.scriptedit_InsertCheck);
			this.scriptedit.KeyUp += new System.Windows.Forms.KeyEventHandler(this.scriptedit_KeyUp);
			this.scriptedit.UpdateUI += new System.EventHandler<ScintillaNET.UpdateUIEventArgs>(this.scriptedit_UpdateUI);
			this.scriptedit.KeyDown += new System.Windows.Forms.KeyEventHandler(this.scriptedit_KeyDown);
			// 
			// scriptpanel
			// 
			this.scriptpanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.scriptpanel.Controls.Add(this.scriptedit);
			this.scriptpanel.Location = new System.Drawing.Point(0, 27);
			this.scriptpanel.Name = "scriptpanel";
			this.scriptpanel.Size = new System.Drawing.Size(474, 381);
			this.scriptpanel.TabIndex = 2;
			// 
			// functionbar
			// 
			this.functionbar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.functionbar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.functionbar.FormattingEnabled = true;
			this.functionbar.Location = new System.Drawing.Point(0, 0);
			this.functionbar.Name = "functionbar";
			this.functionbar.Size = new System.Drawing.Size(474, 21);
			this.functionbar.TabIndex = 2;
			this.functionbar.TabStop = false;
			// 
			// ScriptEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.scriptpanel);
			this.Controls.Add(this.functionbar);
			this.Name = "ScriptEditorControl";
			this.Size = new System.Drawing.Size(474, 408);
			this.scriptpanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private ScintillaNET.Scintilla scriptedit;
		private System.Windows.Forms.Panel scriptpanel;
		private System.Windows.Forms.ComboBox functionbar;
	}
}
