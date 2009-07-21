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
			// 
			// tabs
			// 
			this.tabs.Alignment = System.Windows.Forms.TabAlignment.Left;
			this.tabs.Controls.Add(this.tabPage1);
			this.tabs.Controls.Add(this.tabPage2);
			this.tabs.Controls.Add(this.tabPage3);
			this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabs.ItemSize = new System.Drawing.Size(96, 28);
			this.tabs.Location = new System.Drawing.Point(0, 0);
			this.tabs.Multiline = true;
			this.tabs.Name = "tabs";
			this.tabs.Padding = new System.Drawing.Point(12, 5);
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(308, 541);
			this.tabs.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.tabs.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Location = new System.Drawing.Point(32, 4);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(272, 533);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Undo / Redo";
			// 
			// tabPage2
			// 
			this.tabPage2.Location = new System.Drawing.Point(31, 4);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(273, 533);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Clipboard";
			// 
			// tabPage3
			// 
			this.tabPage3.Location = new System.Drawing.Point(31, 4);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(273, 533);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Prefabs";
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
