﻿namespace CodeImp.DoomBuilder.BuilderModes
{
	partial class SelectSimilarElementOptionsPanel
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
			this.enableall = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			this.cancel = new System.Windows.Forms.Button();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.sectors = new System.Windows.Forms.TabPage();
			this.sectorflags = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
			this.linedefs = new System.Windows.Forms.TabPage();
			this.lineflags = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
			this.sidedefs = new System.Windows.Forms.TabPage();
			this.sideflags = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
			this.things = new System.Windows.Forms.TabPage();
			this.thingflags = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
			this.vertices = new System.Windows.Forms.TabPage();
			this.vertexflags = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
			this.tabControl.SuspendLayout();
			this.sectors.SuspendLayout();
			this.linedefs.SuspendLayout();
			this.sidedefs.SuspendLayout();
			this.things.SuspendLayout();
			this.vertices.SuspendLayout();
			this.SuspendLayout();
			// 
			// enableall
			// 
			this.enableall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.enableall.Location = new System.Drawing.Point(11, 144);
			this.enableall.Name = "enableall";
			this.enableall.Size = new System.Drawing.Size(70, 23);
			this.enableall.TabIndex = 7;
			this.enableall.Text = "Toggle All";
			this.enableall.UseVisualStyleBackColor = true;
			this.enableall.Click += new System.EventHandler(this.enableall_Click);
			// 
			// apply
			// 
			this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.apply.Location = new System.Drawing.Point(122, 144);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(75, 23);
			this.apply.TabIndex = 6;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(203, 144);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(75, 23);
			this.cancel.TabIndex = 5;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// tabControl
			// 
			this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl.Controls.Add(this.sectors);
			this.tabControl.Controls.Add(this.linedefs);
			this.tabControl.Controls.Add(this.sidedefs);
			this.tabControl.Controls.Add(this.things);
			this.tabControl.Controls.Add(this.vertices);
			this.tabControl.Location = new System.Drawing.Point(12, 12);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(266, 127);
			this.tabControl.TabIndex = 8;
			// 
			// sectors
			// 
			this.sectors.Controls.Add(this.sectorflags);
			this.sectors.Location = new System.Drawing.Point(4, 22);
			this.sectors.Name = "sectors";
			this.sectors.Padding = new System.Windows.Forms.Padding(3);
			this.sectors.Size = new System.Drawing.Size(258, 101);
			this.sectors.TabIndex = 0;
			this.sectors.Text = "Sectors";
			this.sectors.UseVisualStyleBackColor = true;
			// 
			// sectorflags
			// 
			this.sectorflags.AutoScroll = true;
			this.sectorflags.Columns = 2;
			this.sectorflags.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sectorflags.Location = new System.Drawing.Point(3, 3);
			this.sectorflags.Name = "sectorflags";
			this.sectorflags.Size = new System.Drawing.Size(252, 95);
			this.sectorflags.TabIndex = 3;
			this.sectorflags.VerticalSpacing = 1;
			// 
			// linedefs
			// 
			this.linedefs.Controls.Add(this.lineflags);
			this.linedefs.Location = new System.Drawing.Point(4, 23);
			this.linedefs.Name = "linedefs";
			this.linedefs.Padding = new System.Windows.Forms.Padding(3);
			this.linedefs.Size = new System.Drawing.Size(258, 100);
			this.linedefs.TabIndex = 1;
			this.linedefs.Text = "Linedefs";
			this.linedefs.UseVisualStyleBackColor = true;
			// 
			// lineflags
			// 
			this.lineflags.AutoScroll = true;
			this.lineflags.Columns = 2;
			this.lineflags.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lineflags.Location = new System.Drawing.Point(3, 3);
			this.lineflags.Name = "lineflags";
			this.lineflags.Size = new System.Drawing.Size(252, 94);
			this.lineflags.TabIndex = 3;
			this.lineflags.VerticalSpacing = 1;
			// 
			// sidedefs
			// 
			this.sidedefs.Controls.Add(this.sideflags);
			this.sidedefs.Location = new System.Drawing.Point(4, 23);
			this.sidedefs.Name = "sidedefs";
			this.sidedefs.Padding = new System.Windows.Forms.Padding(3);
			this.sidedefs.Size = new System.Drawing.Size(258, 100);
			this.sidedefs.TabIndex = 2;
			this.sidedefs.Text = "Sidedefs";
			this.sidedefs.UseVisualStyleBackColor = true;
			// 
			// sideflags
			// 
			this.sideflags.AutoScroll = true;
			this.sideflags.Columns = 2;
			this.sideflags.Dock = System.Windows.Forms.DockStyle.Fill;
			this.sideflags.Location = new System.Drawing.Point(3, 3);
			this.sideflags.Name = "sideflags";
			this.sideflags.Size = new System.Drawing.Size(252, 94);
			this.sideflags.TabIndex = 2;
			this.sideflags.VerticalSpacing = 1;
			// 
			// things
			// 
			this.things.Controls.Add(this.thingflags);
			this.things.Location = new System.Drawing.Point(4, 23);
			this.things.Name = "things";
			this.things.Padding = new System.Windows.Forms.Padding(3);
			this.things.Size = new System.Drawing.Size(258, 100);
			this.things.TabIndex = 3;
			this.things.Text = "Things";
			this.things.UseVisualStyleBackColor = true;
			// 
			// thingflags
			// 
			this.thingflags.AutoScroll = true;
			this.thingflags.Columns = 2;
			this.thingflags.Dock = System.Windows.Forms.DockStyle.Fill;
			this.thingflags.Location = new System.Drawing.Point(3, 3);
			this.thingflags.Name = "thingflags";
			this.thingflags.Size = new System.Drawing.Size(252, 94);
			this.thingflags.TabIndex = 2;
			this.thingflags.VerticalSpacing = 1;
			// 
			// vertices
			// 
			this.vertices.Controls.Add(this.vertexflags);
			this.vertices.Location = new System.Drawing.Point(4, 23);
			this.vertices.Name = "vertices";
			this.vertices.Padding = new System.Windows.Forms.Padding(3);
			this.vertices.Size = new System.Drawing.Size(258, 100);
			this.vertices.TabIndex = 4;
			this.vertices.Text = "Vertices";
			this.vertices.UseVisualStyleBackColor = true;
			// 
			// vertexflags
			// 
			this.vertexflags.AutoScroll = true;
			this.vertexflags.Columns = 2;
			this.vertexflags.Dock = System.Windows.Forms.DockStyle.Fill;
			this.vertexflags.Location = new System.Drawing.Point(3, 3);
			this.vertexflags.Name = "vertexflags";
			this.vertexflags.Size = new System.Drawing.Size(252, 94);
			this.vertexflags.TabIndex = 1;
			this.vertexflags.VerticalSpacing = 1;
			// 
			// SelectSimilarElementOptionsPanel
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(290, 171);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.enableall);
			this.Controls.Add(this.apply);
			this.Controls.Add(this.cancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SelectSimilarElementOptionsPanel";
			this.Opacity = 1;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Selection Options";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SelectSimilarElementOptionsPanel_FormClosing);
			this.tabControl.ResumeLayout(false);
			this.sectors.ResumeLayout(false);
			this.linedefs.ResumeLayout(false);
			this.sidedefs.ResumeLayout(false);
			this.things.ResumeLayout(false);
			this.vertices.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button enableall;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.TabPage sectors;
		private CodeImp.DoomBuilder.Controls.CheckboxArrayControl sectorflags;
		private System.Windows.Forms.TabPage linedefs;
		private CodeImp.DoomBuilder.Controls.CheckboxArrayControl lineflags;
		private System.Windows.Forms.TabPage sidedefs;
		private CodeImp.DoomBuilder.Controls.CheckboxArrayControl sideflags;
		private System.Windows.Forms.TabPage things;
		private CodeImp.DoomBuilder.Controls.CheckboxArrayControl thingflags;
		private System.Windows.Forms.TabPage vertices;
		private CodeImp.DoomBuilder.Controls.CheckboxArrayControl vertexflags;
	}
}