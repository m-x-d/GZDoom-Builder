namespace CodeImp.DoomBuilder.Controls
{
	partial class DockersControl
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
			this.splitter = new CodeImp.DoomBuilder.Controls.TransparentPanel();
			this.tabs = new CodeImp.DoomBuilder.Controls.DockersTabsControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.tabs.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitter
			// 
			this.splitter.Cursor = System.Windows.Forms.Cursors.SizeWE;
			this.splitter.Dock = System.Windows.Forms.DockStyle.Right;
			this.splitter.Location = new System.Drawing.Point(304, 0);
			this.splitter.Name = "splitter";
			this.splitter.Size = new System.Drawing.Size(4, 541);
			this.splitter.TabIndex = 1;
			this.splitter.MouseLeave += new System.EventHandler(this.RaiseMouseContainerLeave);
			this.splitter.MouseMove += new System.Windows.Forms.MouseEventHandler(this.splitter_MouseMove);
			this.splitter.MouseDown += new System.Windows.Forms.MouseEventHandler(this.splitter_MouseDown);
			this.splitter.MouseUp += new System.Windows.Forms.MouseEventHandler(this.splitter_MouseUp);
			this.splitter.MouseEnter += new System.EventHandler(this.RaiseMouseContainerEnter);
			// 
			// tabs
			// 
			this.tabs.Alignment = System.Windows.Forms.TabAlignment.Right;
			this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabs.Controls.Add(this.tabPage1);
			this.tabs.Controls.Add(this.tabPage2);
			this.tabs.Controls.Add(this.tabPage3);
			this.tabs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabs.ItemSize = new System.Drawing.Size(100, 26);
			this.tabs.Location = new System.Drawing.Point(0, 0);
			this.tabs.Margin = new System.Windows.Forms.Padding(0);
			this.tabs.Multiline = true;
			this.tabs.Name = "tabs";
			this.tabs.Padding = new System.Drawing.Point(10, 5);
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(308, 541);
			this.tabs.TabIndex = 0;
			this.tabs.TabStop = false;
			this.tabs.MouseLeave += new System.EventHandler(this.RaiseMouseContainerLeave);
			this.tabs.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabs_Selected);
			this.tabs.Enter += new System.EventHandler(this.tabs_Enter);
			this.tabs.MouseUp += new System.Windows.Forms.MouseEventHandler(this.tabs_MouseUp);
			this.tabs.SelectedIndexChanged += new System.EventHandler(this.tabs_SelectedIndexChanged);
			this.tabs.MouseEnter += new System.EventHandler(this.RaiseMouseContainerEnter);
			// 
			// tabPage1
			// 
			this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
			this.tabPage1.Location = new System.Drawing.Point(4, 4);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(274, 533);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Undo / Redo";
			this.tabPage1.MouseLeave += new System.EventHandler(this.RaiseMouseContainerLeave);
			this.tabPage1.MouseEnter += new System.EventHandler(this.RaiseMouseContainerEnter);
			// 
			// tabPage2
			// 
			this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
			this.tabPage2.Location = new System.Drawing.Point(4, 4);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(274, 533);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Clipboard";
			this.tabPage2.MouseLeave += new System.EventHandler(this.RaiseMouseContainerLeave);
			this.tabPage2.MouseEnter += new System.EventHandler(this.RaiseMouseContainerEnter);
			// 
			// tabPage3
			// 
			this.tabPage3.BackColor = System.Drawing.SystemColors.Control;
			this.tabPage3.Location = new System.Drawing.Point(4, 4);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(274, 533);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Prefabs";
			this.tabPage3.MouseLeave += new System.EventHandler(this.RaiseMouseContainerLeave);
			this.tabPage3.MouseEnter += new System.EventHandler(this.RaiseMouseContainerEnter);
			// 
			// DockersControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.splitter);
			this.Controls.Add(this.tabs);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "DockersControl";
			this.Size = new System.Drawing.Size(308, 541);
			this.MouseLeave += new System.EventHandler(this.RaiseMouseContainerLeave);
			this.MouseEnter += new System.EventHandler(this.RaiseMouseContainerEnter);
			this.tabs.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private DockersTabsControl tabs;
		private TransparentPanel splitter;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TabPage tabPage3;
	}
}
