namespace CodeImp.DoomBuilder.Controls
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
			this.ceilingLightLabel = new System.Windows.Forms.Label();
			this.ceilingScaleLabel = new System.Windows.Forms.Label();
			this.ceilingAngleLabel = new System.Windows.Forms.Label();
			this.ceilingOffsetLabel = new System.Windows.Forms.Label();
			this.floorOffsetLabel = new System.Windows.Forms.Label();
			this.floorLightLabel = new System.Windows.Forms.Label();
			this.floorAngleLabel = new System.Windows.Forms.Label();
			this.floorScaleLabel = new System.Windows.Forms.Label();
			this.sectorinfo = new System.Windows.Forms.GroupBox();
			this.brightness = new System.Windows.Forms.Label();
			this.height = new System.Windows.Forms.Label();
			this.tag = new System.Windows.Forms.Label();
			this.floor = new System.Windows.Forms.Label();
			this.ceiling = new System.Windows.Forms.Label();
			this.effect = new System.Windows.Forms.Label();
			this.ceilingLight = new System.Windows.Forms.Label();
			this.ceilingScale = new System.Windows.Forms.Label();
			this.ceilingAngle = new System.Windows.Forms.Label();
			this.ceilingOffset = new System.Windows.Forms.Label();
			this.ceilingpanel = new System.Windows.Forms.GroupBox();
			this.ceilingInfo = new System.Windows.Forms.Panel();
			this.ceilingname = new System.Windows.Forms.Label();
			this.ceilingtex = new System.Windows.Forms.Panel();
			this.floorpanel = new System.Windows.Forms.GroupBox();
			this.floorInfo = new System.Windows.Forms.Panel();
			this.floorLight = new System.Windows.Forms.Label();
			this.floorScale = new System.Windows.Forms.Label();
			this.floorOffset = new System.Windows.Forms.Label();
			this.floorAngle = new System.Windows.Forms.Label();
			this.floorname = new System.Windows.Forms.Label();
			this.floortex = new System.Windows.Forms.Panel();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			label13 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			this.sectorinfo.SuspendLayout();
			this.ceilingpanel.SuspendLayout();
			this.ceilingInfo.SuspendLayout();
			this.floorpanel.SuspendLayout();
			this.floorInfo.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label13
			// 
			label13.Location = new System.Drawing.Point(103, 75);
			label13.Name = "label13";
			label13.Size = new System.Drawing.Size(70, 14);
			label13.TabIndex = 14;
			label13.Text = "Brightness:";
			label13.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label5
			// 
			label5.Location = new System.Drawing.Point(103, 55);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(70, 14);
			label5.TabIndex = 8;
			label5.Text = "Height:";
			label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label4
			// 
			label4.Location = new System.Drawing.Point(8, 75);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(44, 14);
			label4.TabIndex = 4;
			label4.Text = "Tag:";
			label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label3
			// 
			label3.Location = new System.Drawing.Point(8, 55);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(44, 14);
			label3.TabIndex = 3;
			label3.Text = "Floor:";
			label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label2
			// 
			label2.Location = new System.Drawing.Point(8, 35);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(44, 14);
			label2.TabIndex = 2;
			label2.Text = "Ceiling:";
			label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(13, 15);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(39, 14);
			label1.TabIndex = 0;
			label1.Text = "Effect:";
			// 
			// ceilingLightLabel
			// 
			this.ceilingLightLabel.Location = new System.Drawing.Point(3, 61);
			this.ceilingLightLabel.Name = "ceilingLightLabel";
			this.ceilingLightLabel.Size = new System.Drawing.Size(45, 14);
			this.ceilingLightLabel.TabIndex = 27;
			this.ceilingLightLabel.Text = "Light:";
			this.ceilingLightLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ceilingScaleLabel
			// 
			this.ceilingScaleLabel.Location = new System.Drawing.Point(3, 43);
			this.ceilingScaleLabel.Name = "ceilingScaleLabel";
			this.ceilingScaleLabel.Size = new System.Drawing.Size(45, 14);
			this.ceilingScaleLabel.TabIndex = 26;
			this.ceilingScaleLabel.Text = "Scale:";
			this.ceilingScaleLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ceilingAngleLabel
			// 
			this.ceilingAngleLabel.Location = new System.Drawing.Point(3, 25);
			this.ceilingAngleLabel.Name = "ceilingAngleLabel";
			this.ceilingAngleLabel.Size = new System.Drawing.Size(45, 14);
			this.ceilingAngleLabel.TabIndex = 24;
			this.ceilingAngleLabel.Text = "Angle:";
			this.ceilingAngleLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ceilingOffsetLabel
			// 
			this.ceilingOffsetLabel.Location = new System.Drawing.Point(3, 7);
			this.ceilingOffsetLabel.Name = "ceilingOffsetLabel";
			this.ceilingOffsetLabel.Size = new System.Drawing.Size(45, 14);
			this.ceilingOffsetLabel.TabIndex = 22;
			this.ceilingOffsetLabel.Text = "Offset:";
			this.ceilingOffsetLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// floorOffsetLabel
			// 
			this.floorOffsetLabel.Location = new System.Drawing.Point(3, 7);
			this.floorOffsetLabel.Name = "floorOffsetLabel";
			this.floorOffsetLabel.Size = new System.Drawing.Size(45, 14);
			this.floorOffsetLabel.TabIndex = 22;
			this.floorOffsetLabel.Text = "Offset:";
			this.floorOffsetLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// floorLightLabel
			// 
			this.floorLightLabel.Location = new System.Drawing.Point(3, 61);
			this.floorLightLabel.Name = "floorLightLabel";
			this.floorLightLabel.Size = new System.Drawing.Size(45, 14);
			this.floorLightLabel.TabIndex = 27;
			this.floorLightLabel.Text = "Light:";
			this.floorLightLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// floorAngleLabel
			// 
			this.floorAngleLabel.Location = new System.Drawing.Point(3, 25);
			this.floorAngleLabel.Name = "floorAngleLabel";
			this.floorAngleLabel.Size = new System.Drawing.Size(45, 14);
			this.floorAngleLabel.TabIndex = 24;
			this.floorAngleLabel.Text = "Angle:";
			this.floorAngleLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// floorScaleLabel
			// 
			this.floorScaleLabel.Location = new System.Drawing.Point(3, 43);
			this.floorScaleLabel.Name = "floorScaleLabel";
			this.floorScaleLabel.Size = new System.Drawing.Size(45, 14);
			this.floorScaleLabel.TabIndex = 26;
			this.floorScaleLabel.Text = "Scale:";
			this.floorScaleLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// sectorinfo
			// 
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
			this.sectorinfo.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.sectorinfo.Name = "sectorinfo";
			this.sectorinfo.Size = new System.Drawing.Size(220, 100);
			this.sectorinfo.TabIndex = 2;
			this.sectorinfo.TabStop = false;
			this.sectorinfo.Text = " Sector ";
			// 
			// brightness
			// 
			this.brightness.Location = new System.Drawing.Point(177, 75);
			this.brightness.Name = "brightness";
			this.brightness.Size = new System.Drawing.Size(56, 14);
			this.brightness.TabIndex = 17;
			this.brightness.Text = "0";
			// 
			// height
			// 
			this.height.Location = new System.Drawing.Point(177, 55);
			this.height.Name = "height";
			this.height.Size = new System.Drawing.Size(56, 14);
			this.height.TabIndex = 11;
			this.height.Text = "0";
			// 
			// tag
			// 
			this.tag.Location = new System.Drawing.Point(55, 75);
			this.tag.Name = "tag";
			this.tag.Size = new System.Drawing.Size(42, 14);
			this.tag.TabIndex = 7;
			this.tag.Text = "0";
			// 
			// floor
			// 
			this.floor.Location = new System.Drawing.Point(55, 55);
			this.floor.Name = "floor";
			this.floor.Size = new System.Drawing.Size(42, 14);
			this.floor.TabIndex = 6;
			this.floor.Text = "360";
			// 
			// ceiling
			// 
			this.ceiling.Location = new System.Drawing.Point(55, 35);
			this.ceiling.Name = "ceiling";
			this.ceiling.Size = new System.Drawing.Size(42, 14);
			this.ceiling.TabIndex = 5;
			this.ceiling.Text = "1024";
			// 
			// effect
			// 
			this.effect.AutoSize = true;
			this.effect.Location = new System.Drawing.Point(55, 15);
			this.effect.Name = "effect";
			this.effect.Size = new System.Drawing.Size(123, 14);
			this.effect.TabIndex = 1;
			this.effect.Text = "0 - Whacky Pool of Fluid";
			// 
			// ceilingLight
			// 
			this.ceilingLight.Location = new System.Drawing.Point(54, 61);
			this.ceilingLight.Name = "ceilingLight";
			this.ceilingLight.Size = new System.Drawing.Size(60, 14);
			this.ceilingLight.TabIndex = 29;
			this.ceilingLight.Text = "255 (abs.)";
			// 
			// ceilingScale
			// 
			this.ceilingScale.Location = new System.Drawing.Point(54, 43);
			this.ceilingScale.Name = "ceilingScale";
			this.ceilingScale.Size = new System.Drawing.Size(60, 14);
			this.ceilingScale.TabIndex = 28;
			this.ceilingScale.Text = "-1.0, -1.0";
			// 
			// ceilingAngle
			// 
			this.ceilingAngle.Location = new System.Drawing.Point(54, 25);
			this.ceilingAngle.Name = "ceilingAngle";
			this.ceilingAngle.Size = new System.Drawing.Size(60, 14);
			this.ceilingAngle.TabIndex = 25;
			this.ceilingAngle.Text = "45";
			// 
			// ceilingOffset
			// 
			this.ceilingOffset.Location = new System.Drawing.Point(54, 7);
			this.ceilingOffset.Name = "ceilingOffset";
			this.ceilingOffset.Size = new System.Drawing.Size(60, 14);
			this.ceilingOffset.TabIndex = 23;
			this.ceilingOffset.Text = "-100, -100";
			// 
			// ceilingpanel
			// 
			this.ceilingpanel.Controls.Add(this.ceilingInfo);
			this.ceilingpanel.Controls.Add(this.ceilingname);
			this.ceilingpanel.Controls.Add(this.ceilingtex);
			this.ceilingpanel.Location = new System.Drawing.Point(426, 0);
			this.ceilingpanel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.ceilingpanel.Name = "ceilingpanel";
			this.ceilingpanel.Size = new System.Drawing.Size(200, 100);
			this.ceilingpanel.TabIndex = 3;
			this.ceilingpanel.TabStop = false;
			this.ceilingpanel.Text = " Ceiling ";
			// 
			// ceilingInfo
			// 
			this.ceilingInfo.Controls.Add(this.ceilingLight);
			this.ceilingInfo.Controls.Add(this.ceilingOffsetLabel);
			this.ceilingInfo.Controls.Add(this.ceilingScale);
			this.ceilingInfo.Controls.Add(this.ceilingOffset);
			this.ceilingInfo.Controls.Add(this.ceilingLightLabel);
			this.ceilingInfo.Controls.Add(this.ceilingAngleLabel);
			this.ceilingInfo.Controls.Add(this.ceilingScaleLabel);
			this.ceilingInfo.Controls.Add(this.ceilingAngle);
			this.ceilingInfo.Location = new System.Drawing.Point(82, 15);
			this.ceilingInfo.Name = "ceilingInfo";
			this.ceilingInfo.Size = new System.Drawing.Size(118, 80);
			this.ceilingInfo.TabIndex = 2;
			// 
			// ceilingname
			// 
			this.ceilingname.Location = new System.Drawing.Point(8, 80);
			this.ceilingname.Name = "ceilingname";
			this.ceilingname.Size = new System.Drawing.Size(68, 13);
			this.ceilingname.TabIndex = 1;
			this.ceilingname.Text = "BROWNHUG";
			this.ceilingname.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// ceilingtex
			// 
			this.ceilingtex.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.ceilingtex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.ceilingtex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.ceilingtex.Location = new System.Drawing.Point(8, 19);
			this.ceilingtex.Name = "ceilingtex";
			this.ceilingtex.Size = new System.Drawing.Size(68, 60);
			this.ceilingtex.TabIndex = 0;
			// 
			// floorpanel
			// 
			this.floorpanel.Controls.Add(this.floorInfo);
			this.floorpanel.Controls.Add(this.floorname);
			this.floorpanel.Controls.Add(this.floortex);
			this.floorpanel.Location = new System.Drawing.Point(223, 0);
			this.floorpanel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.floorpanel.Name = "floorpanel";
			this.floorpanel.Size = new System.Drawing.Size(200, 100);
			this.floorpanel.TabIndex = 4;
			this.floorpanel.TabStop = false;
			this.floorpanel.Text = " Floor ";
			// 
			// floorInfo
			// 
			this.floorInfo.Controls.Add(this.floorLight);
			this.floorInfo.Controls.Add(this.floorOffsetLabel);
			this.floorInfo.Controls.Add(this.floorScale);
			this.floorInfo.Controls.Add(this.floorOffset);
			this.floorInfo.Controls.Add(this.floorLightLabel);
			this.floorInfo.Controls.Add(this.floorAngleLabel);
			this.floorInfo.Controls.Add(this.floorScaleLabel);
			this.floorInfo.Controls.Add(this.floorAngle);
			this.floorInfo.Location = new System.Drawing.Point(82, 15);
			this.floorInfo.Name = "floorInfo";
			this.floorInfo.Size = new System.Drawing.Size(118, 80);
			this.floorInfo.TabIndex = 30;
			// 
			// floorLight
			// 
			this.floorLight.Location = new System.Drawing.Point(54, 61);
			this.floorLight.Name = "floorLight";
			this.floorLight.Size = new System.Drawing.Size(60, 14);
			this.floorLight.TabIndex = 29;
			this.floorLight.Text = "255 (abs.)";
			// 
			// floorScale
			// 
			this.floorScale.Location = new System.Drawing.Point(54, 43);
			this.floorScale.Name = "floorScale";
			this.floorScale.Size = new System.Drawing.Size(60, 14);
			this.floorScale.TabIndex = 28;
			this.floorScale.Text = "-1.0, -1.0";
			// 
			// floorOffset
			// 
			this.floorOffset.Location = new System.Drawing.Point(54, 7);
			this.floorOffset.Name = "floorOffset";
			this.floorOffset.Size = new System.Drawing.Size(60, 14);
			this.floorOffset.TabIndex = 23;
			this.floorOffset.Text = "-100, -100";
			// 
			// floorAngle
			// 
			this.floorAngle.Location = new System.Drawing.Point(54, 25);
			this.floorAngle.Name = "floorAngle";
			this.floorAngle.Size = new System.Drawing.Size(60, 14);
			this.floorAngle.TabIndex = 25;
			this.floorAngle.Text = "45";
			// 
			// floorname
			// 
			this.floorname.Location = new System.Drawing.Point(8, 80);
			this.floorname.Name = "floorname";
			this.floorname.Size = new System.Drawing.Size(68, 13);
			this.floorname.TabIndex = 1;
			this.floorname.Text = "BROWNHUG";
			this.floorname.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// floortex
			// 
			this.floortex.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.floortex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.floortex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.floortex.Location = new System.Drawing.Point(8, 19);
			this.floortex.Name = "floortex";
			this.floortex.Size = new System.Drawing.Size(68, 60);
			this.floortex.TabIndex = 0;
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Controls.Add(this.sectorinfo);
			this.flowLayoutPanel1.Controls.Add(this.floorpanel);
			this.flowLayoutPanel1.Controls.Add(this.ceilingpanel);
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(642, 100);
			this.flowLayoutPanel1.TabIndex = 5;
			this.flowLayoutPanel1.WrapContents = false;
			// 
			// SectorInfoPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.flowLayoutPanel1);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximumSize = new System.Drawing.Size(10000, 100);
			this.MinimumSize = new System.Drawing.Size(100, 100);
			this.Name = "SectorInfoPanel";
			this.Size = new System.Drawing.Size(642, 100);
			this.sectorinfo.ResumeLayout(false);
			this.sectorinfo.PerformLayout();
			this.ceilingpanel.ResumeLayout(false);
			this.ceilingInfo.ResumeLayout(false);
			this.floorpanel.ResumeLayout(false);
			this.floorInfo.ResumeLayout(false);
			this.flowLayoutPanel1.ResumeLayout(false);
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
		private System.Windows.Forms.Label ceilingLight;
		private System.Windows.Forms.Label ceilingScale;
		private System.Windows.Forms.Label ceilingAngle;
		private System.Windows.Forms.Label ceilingOffset;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Panel ceilingInfo;
		private System.Windows.Forms.Panel floorInfo;
		private System.Windows.Forms.Label floorLight;
		private System.Windows.Forms.Label floorScale;
		private System.Windows.Forms.Label floorOffset;
		private System.Windows.Forms.Label floorAngle;
		private System.Windows.Forms.Label ceilingLightLabel;
		private System.Windows.Forms.Label ceilingScaleLabel;
		private System.Windows.Forms.Label ceilingAngleLabel;
		private System.Windows.Forms.Label ceilingOffsetLabel;
		private System.Windows.Forms.Label floorOffsetLabel;
		private System.Windows.Forms.Label floorLightLabel;
		private System.Windows.Forms.Label floorAngleLabel;
		private System.Windows.Forms.Label floorScaleLabel;
	}
}