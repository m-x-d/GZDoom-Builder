namespace CodeImp.DoomBuilder.Interface
{
	partial class SectorInfoPanel
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
			System.Windows.Forms.Label label13;
			System.Windows.Forms.Label label5;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label1;
			this.sectorinfo = new System.Windows.Forms.GroupBox();
			this.brightness = new System.Windows.Forms.Label();
			this.height = new System.Windows.Forms.Label();
			this.tag = new System.Windows.Forms.Label();
			this.floor = new System.Windows.Forms.Label();
			this.ceiling = new System.Windows.Forms.Label();
			this.effect = new System.Windows.Forms.Label();
			this.ceilingpanel = new System.Windows.Forms.GroupBox();
			this.ceilingname = new System.Windows.Forms.Label();
			this.ceilingtex = new System.Windows.Forms.Panel();
			this.floorpanel = new System.Windows.Forms.GroupBox();
			this.floorname = new System.Windows.Forms.Label();
			this.floortex = new System.Windows.Forms.Panel();
			label13 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			this.sectorinfo.SuspendLayout();
			this.ceilingpanel.SuspendLayout();
			this.floorpanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// sectorinfo
			// 
			this.sectorinfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.sectorinfo.Controls.Add(this.brightness);
			this.sectorinfo.Controls.Add(label13);
			this.sectorinfo.Controls.Add(this.height);
			this.sectorinfo.Controls.Add(label5);
			this.sectorinfo.Controls.Add(this.tag);
			this.sectorinfo.Controls.Add(this.floor);
			this.sectorinfo.Controls.Add(this.ceiling);
			this.sectorinfo.Controls.Add(label4);
			this.sectorinfo.Controls.Add(label3);
			this.sectorinfo.Controls.Add(label2);
			this.sectorinfo.Controls.Add(this.effect);
			this.sectorinfo.Controls.Add(label1);
			this.sectorinfo.Location = new System.Drawing.Point(0, 0);
			this.sectorinfo.Name = "sectorinfo";
			this.sectorinfo.Size = new System.Drawing.Size(230, 100);
			this.sectorinfo.TabIndex = 2;
			this.sectorinfo.TabStop = false;
			this.sectorinfo.Text = " Sector ";
			// 
			// brightness
			// 
			this.brightness.AutoSize = true;
			this.brightness.Location = new System.Drawing.Point(177, 77);
			this.brightness.Name = "brightness";
			this.brightness.Size = new System.Drawing.Size(13, 14);
			this.brightness.TabIndex = 17;
			this.brightness.Text = "0";
			// 
			// label13
			// 
			label13.AutoSize = true;
			label13.Location = new System.Drawing.Point(111, 77);
			label13.Name = "label13";
			label13.Size = new System.Drawing.Size(62, 14);
			label13.TabIndex = 14;
			label13.Text = "Brightness:";
			// 
			// height
			// 
			this.height.AutoSize = true;
			this.height.Location = new System.Drawing.Point(177, 58);
			this.height.Name = "height";
			this.height.Size = new System.Drawing.Size(13, 14);
			this.height.TabIndex = 11;
			this.height.Text = "0";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(133, 58);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(40, 14);
			label5.TabIndex = 8;
			label5.Text = "Height:";
			// 
			// tag
			// 
			this.tag.AutoSize = true;
			this.tag.Location = new System.Drawing.Point(55, 77);
			this.tag.Name = "tag";
			this.tag.Size = new System.Drawing.Size(13, 14);
			this.tag.TabIndex = 7;
			this.tag.Text = "0";
			// 
			// floor
			// 
			this.floor.AutoSize = true;
			this.floor.Location = new System.Drawing.Point(55, 58);
			this.floor.Name = "floor";
			this.floor.Size = new System.Drawing.Size(25, 14);
			this.floor.TabIndex = 6;
			this.floor.Text = "360";
			// 
			// ceiling
			// 
			this.ceiling.AutoSize = true;
			this.ceiling.Location = new System.Drawing.Point(55, 39);
			this.ceiling.Name = "ceiling";
			this.ceiling.Size = new System.Drawing.Size(31, 14);
			this.ceiling.TabIndex = 5;
			this.ceiling.Text = "1024";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(24, 77);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(28, 14);
			label4.TabIndex = 4;
			label4.Text = "Tag:";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(18, 58);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(34, 14);
			label3.TabIndex = 3;
			label3.Text = "Floor:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(11, 39);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(41, 14);
			label2.TabIndex = 2;
			label2.Text = "Ceiling:";
			// 
			// effect
			// 
			this.effect.AutoSize = true;
			this.effect.Location = new System.Drawing.Point(55, 19);
			this.effect.Name = "effect";
			this.effect.Size = new System.Drawing.Size(123, 14);
			this.effect.TabIndex = 1;
			this.effect.Text = "0 - Whacky Pool of Fluid";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(13, 19);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(39, 14);
			label1.TabIndex = 0;
			label1.Text = "Effect:";
			// 
			// ceilingpanel
			// 
			this.ceilingpanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.ceilingpanel.Controls.Add(this.ceilingname);
			this.ceilingpanel.Controls.Add(this.ceilingtex);
			this.ceilingpanel.Location = new System.Drawing.Point(236, 0);
			this.ceilingpanel.Name = "ceilingpanel";
			this.ceilingpanel.Size = new System.Drawing.Size(93, 100);
			this.ceilingpanel.TabIndex = 3;
			this.ceilingpanel.TabStop = false;
			this.ceilingpanel.Text = " Ceiling ";
			// 
			// ceilingname
			// 
			this.ceilingname.Location = new System.Drawing.Point(11, 80);
			this.ceilingname.Name = "ceilingname";
			this.ceilingname.Size = new System.Drawing.Size(72, 13);
			this.ceilingname.TabIndex = 1;
			this.ceilingname.Text = "BIGDOOR6";
			this.ceilingname.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// ceilingtex
			// 
			this.ceilingtex.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.ceilingtex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.ceilingtex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.ceilingtex.Location = new System.Drawing.Point(13, 19);
			this.ceilingtex.Name = "ceilingtex";
			this.ceilingtex.Size = new System.Drawing.Size(68, 60);
			this.ceilingtex.TabIndex = 0;
			// 
			// floorpanel
			// 
			this.floorpanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.floorpanel.Controls.Add(this.floorname);
			this.floorpanel.Controls.Add(this.floortex);
			this.floorpanel.Location = new System.Drawing.Point(335, 0);
			this.floorpanel.Name = "floorpanel";
			this.floorpanel.Size = new System.Drawing.Size(93, 100);
			this.floorpanel.TabIndex = 4;
			this.floorpanel.TabStop = false;
			this.floorpanel.Text = " Floor ";
			// 
			// floorname
			// 
			this.floorname.Location = new System.Drawing.Point(11, 80);
			this.floorname.Name = "floorname";
			this.floorname.Size = new System.Drawing.Size(72, 13);
			this.floorname.TabIndex = 1;
			this.floorname.Text = "BIGDOOR6";
			this.floorname.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// floortex
			// 
			this.floortex.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.floortex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.floortex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.floortex.Location = new System.Drawing.Point(13, 19);
			this.floortex.Name = "floortex";
			this.floortex.Size = new System.Drawing.Size(68, 60);
			this.floortex.TabIndex = 0;
			// 
			// SectorInfoPanel
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.floorpanel);
			this.Controls.Add(this.ceilingpanel);
			this.Controls.Add(this.sectorinfo);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximumSize = new System.Drawing.Size(10000, 100);
			this.MinimumSize = new System.Drawing.Size(100, 100);
			this.Name = "SectorInfoPanel";
			this.Size = new System.Drawing.Size(450, 100);
			this.sectorinfo.ResumeLayout(false);
			this.sectorinfo.PerformLayout();
			this.ceilingpanel.ResumeLayout(false);
			this.floorpanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label brightness;
		private System.Windows.Forms.Label height;
		private System.Windows.Forms.Label tag;
		private System.Windows.Forms.Label floor;
		private System.Windows.Forms.Label ceiling;
		private System.Windows.Forms.Label effect;
		private System.Windows.Forms.GroupBox ceilingpanel;
		private System.Windows.Forms.Label ceilingname;
		private System.Windows.Forms.Panel ceilingtex;
		private System.Windows.Forms.GroupBox floorpanel;
		private System.Windows.Forms.Label floorname;
		private System.Windows.Forms.Panel floortex;
		private System.Windows.Forms.GroupBox sectorinfo;
	}
}
