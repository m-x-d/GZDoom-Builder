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
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label2;
			this.labelTag = new System.Windows.Forms.Label();
			this.labelEffect = new System.Windows.Forms.Label();
			this.ceilingLightLabel = new System.Windows.Forms.Label();
			this.ceilingScaleLabel = new System.Windows.Forms.Label();
			this.ceilingAngleLabel = new System.Windows.Forms.Label();
			this.ceilingOffsetLabel = new System.Windows.Forms.Label();
			this.floorOffsetLabel = new System.Windows.Forms.Label();
			this.floorLightLabel = new System.Windows.Forms.Label();
			this.floorAngleLabel = new System.Windows.Forms.Label();
			this.floorScaleLabel = new System.Windows.Forms.Label();
			this.sectorinfo = new System.Windows.Forms.GroupBox();
			this.panelFadeColor = new System.Windows.Forms.Panel();
			this.panelLightColor = new System.Windows.Forms.Panel();
			this.labelFade = new System.Windows.Forms.Label();
			this.labelLight = new System.Windows.Forms.Label();
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
			this.ceilingname = new System.Windows.Forms.Label();
			this.ceilingtex = new System.Windows.Forms.Panel();
			this.labelCeilTextureSize = new System.Windows.Forms.Label();
			this.floorpanel = new System.Windows.Forms.GroupBox();
			this.floorLight = new System.Windows.Forms.Label();
			this.floorScale = new System.Windows.Forms.Label();
			this.floorOffset = new System.Windows.Forms.Label();
			this.floorAngle = new System.Windows.Forms.Label();
			this.floorname = new System.Windows.Forms.Label();
			this.floortex = new System.Windows.Forms.Panel();
			this.labelFloorTextureSize = new System.Windows.Forms.Label();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.flagsPanel = new System.Windows.Forms.GroupBox();
			this.flags = new System.Windows.Forms.ListView();
			label13 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			this.sectorinfo.SuspendLayout();
			this.ceilingpanel.SuspendLayout();
			this.ceilingtex.SuspendLayout();
			this.floorpanel.SuspendLayout();
			this.floortex.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.flagsPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// label13
			// 
			label13.Location = new System.Drawing.Point(183, 64);
			label13.Name = "label13";
			label13.Size = new System.Drawing.Size(70, 14);
			label13.TabIndex = 14;
			label13.Text = "Brightness:";
			label13.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label5
			// 
			label5.Location = new System.Drawing.Point(8, 64);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(44, 14);
			label5.TabIndex = 8;
			label5.Text = "Height:";
			label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label3
			// 
			label3.Location = new System.Drawing.Point(8, 49);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(44, 14);
			label3.TabIndex = 3;
			label3.Text = "Floor:";
			label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label2
			// 
			label2.Location = new System.Drawing.Point(8, 34);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(44, 14);
			label2.TabIndex = 2;
			label2.Text = "Ceiling:";
			label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelTag
			// 
			this.labelTag.Location = new System.Drawing.Point(8, 79);
			this.labelTag.Name = "labelTag";
			this.labelTag.Size = new System.Drawing.Size(44, 14);
			this.labelTag.TabIndex = 4;
			this.labelTag.Text = "Tag:";
			this.labelTag.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelEffect
			// 
			this.labelEffect.Location = new System.Drawing.Point(8, 19);
			this.labelEffect.Name = "labelEffect";
			this.labelEffect.Size = new System.Drawing.Size(44, 14);
			this.labelEffect.TabIndex = 0;
			this.labelEffect.Text = "Effect:";
			this.labelEffect.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ceilingLightLabel
			// 
			this.ceilingLightLabel.Location = new System.Drawing.Point(77, 63);
			this.ceilingLightLabel.Name = "ceilingLightLabel";
			this.ceilingLightLabel.Size = new System.Drawing.Size(45, 14);
			this.ceilingLightLabel.TabIndex = 27;
			this.ceilingLightLabel.Text = "Light:";
			this.ceilingLightLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ceilingScaleLabel
			// 
			this.ceilingScaleLabel.Location = new System.Drawing.Point(77, 47);
			this.ceilingScaleLabel.Name = "ceilingScaleLabel";
			this.ceilingScaleLabel.Size = new System.Drawing.Size(45, 14);
			this.ceilingScaleLabel.TabIndex = 26;
			this.ceilingScaleLabel.Text = "Scale:";
			this.ceilingScaleLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ceilingAngleLabel
			// 
			this.ceilingAngleLabel.Location = new System.Drawing.Point(77, 31);
			this.ceilingAngleLabel.Name = "ceilingAngleLabel";
			this.ceilingAngleLabel.Size = new System.Drawing.Size(45, 14);
			this.ceilingAngleLabel.TabIndex = 24;
			this.ceilingAngleLabel.Text = "Angle:";
			this.ceilingAngleLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// ceilingOffsetLabel
			// 
			this.ceilingOffsetLabel.Location = new System.Drawing.Point(77, 15);
			this.ceilingOffsetLabel.Name = "ceilingOffsetLabel";
			this.ceilingOffsetLabel.Size = new System.Drawing.Size(45, 14);
			this.ceilingOffsetLabel.TabIndex = 22;
			this.ceilingOffsetLabel.Text = "Offset:";
			this.ceilingOffsetLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// floorOffsetLabel
			// 
			this.floorOffsetLabel.Location = new System.Drawing.Point(77, 15);
			this.floorOffsetLabel.Name = "floorOffsetLabel";
			this.floorOffsetLabel.Size = new System.Drawing.Size(45, 14);
			this.floorOffsetLabel.TabIndex = 22;
			this.floorOffsetLabel.Text = "Offset:";
			this.floorOffsetLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// floorLightLabel
			// 
			this.floorLightLabel.Location = new System.Drawing.Point(77, 63);
			this.floorLightLabel.Name = "floorLightLabel";
			this.floorLightLabel.Size = new System.Drawing.Size(45, 14);
			this.floorLightLabel.TabIndex = 27;
			this.floorLightLabel.Text = "Light:";
			this.floorLightLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// floorAngleLabel
			// 
			this.floorAngleLabel.Location = new System.Drawing.Point(77, 31);
			this.floorAngleLabel.Name = "floorAngleLabel";
			this.floorAngleLabel.Size = new System.Drawing.Size(45, 14);
			this.floorAngleLabel.TabIndex = 24;
			this.floorAngleLabel.Text = "Angle:";
			this.floorAngleLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// floorScaleLabel
			// 
			this.floorScaleLabel.Location = new System.Drawing.Point(77, 47);
			this.floorScaleLabel.Name = "floorScaleLabel";
			this.floorScaleLabel.Size = new System.Drawing.Size(45, 14);
			this.floorScaleLabel.TabIndex = 26;
			this.floorScaleLabel.Text = "Scale:";
			this.floorScaleLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// sectorinfo
			// 
			this.sectorinfo.Controls.Add(this.panelFadeColor);
			this.sectorinfo.Controls.Add(this.panelLightColor);
			this.sectorinfo.Controls.Add(this.labelFade);
			this.sectorinfo.Controls.Add(this.labelLight);
			this.sectorinfo.Controls.Add(this.brightness);
			this.sectorinfo.Controls.Add(label13);
			this.sectorinfo.Controls.Add(this.height);
			this.sectorinfo.Controls.Add(label5);
			this.sectorinfo.Controls.Add(this.tag);
			this.sectorinfo.Controls.Add(this.floor);
			this.sectorinfo.Controls.Add(this.ceiling);
			this.sectorinfo.Controls.Add(this.labelTag);
			this.sectorinfo.Controls.Add(label3);
			this.sectorinfo.Controls.Add(label2);
			this.sectorinfo.Controls.Add(this.effect);
			this.sectorinfo.Controls.Add(this.labelEffect);
			this.sectorinfo.Location = new System.Drawing.Point(0, 0);
			this.sectorinfo.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.sectorinfo.Name = "sectorinfo";
			this.sectorinfo.Size = new System.Drawing.Size(300, 100);
			this.sectorinfo.TabIndex = 2;
			this.sectorinfo.TabStop = false;
			this.sectorinfo.Text = " Sector ";
			// 
			// panelFadeColor
			// 
			this.panelFadeColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelFadeColor.Location = new System.Drawing.Point(260, 50);
			this.panelFadeColor.Name = "panelFadeColor";
			this.panelFadeColor.Size = new System.Drawing.Size(20, 12);
			this.panelFadeColor.TabIndex = 21;
			// 
			// panelLightColor
			// 
			this.panelLightColor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelLightColor.Location = new System.Drawing.Point(260, 35);
			this.panelLightColor.Name = "panelLightColor";
			this.panelLightColor.Size = new System.Drawing.Size(20, 12);
			this.panelLightColor.TabIndex = 20;
			// 
			// labelFade
			// 
			this.labelFade.Location = new System.Drawing.Point(183, 49);
			this.labelFade.Name = "labelFade";
			this.labelFade.Size = new System.Drawing.Size(70, 14);
			this.labelFade.TabIndex = 19;
			this.labelFade.Text = "Fade:";
			this.labelFade.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// labelLight
			// 
			this.labelLight.Location = new System.Drawing.Point(183, 34);
			this.labelLight.Name = "labelLight";
			this.labelLight.Size = new System.Drawing.Size(70, 14);
			this.labelLight.TabIndex = 18;
			this.labelLight.Text = "Light:";
			this.labelLight.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// brightness
			// 
			this.brightness.Location = new System.Drawing.Point(257, 64);
			this.brightness.Name = "brightness";
			this.brightness.Size = new System.Drawing.Size(38, 14);
			this.brightness.TabIndex = 17;
			this.brightness.Text = "0";
			// 
			// height
			// 
			this.height.Location = new System.Drawing.Point(55, 64);
			this.height.Name = "height";
			this.height.Size = new System.Drawing.Size(38, 14);
			this.height.TabIndex = 11;
			this.height.Text = "0";
			// 
			// tag
			// 
			this.tag.Location = new System.Drawing.Point(55, 79);
			this.tag.Name = "tag";
			this.tag.Size = new System.Drawing.Size(239, 14);
			this.tag.TabIndex = 7;
			this.tag.Text = "0";
			// 
			// floor
			// 
			this.floor.Location = new System.Drawing.Point(55, 49);
			this.floor.Name = "floor";
			this.floor.Size = new System.Drawing.Size(42, 14);
			this.floor.TabIndex = 6;
			this.floor.Text = "360";
			// 
			// ceiling
			// 
			this.ceiling.Location = new System.Drawing.Point(55, 34);
			this.ceiling.Name = "ceiling";
			this.ceiling.Size = new System.Drawing.Size(42, 14);
			this.ceiling.TabIndex = 5;
			this.ceiling.Text = "1024";
			// 
			// effect
			// 
			this.effect.AutoSize = true;
			this.effect.Location = new System.Drawing.Point(55, 19);
			this.effect.Name = "effect";
			this.effect.Size = new System.Drawing.Size(123, 13);
			this.effect.TabIndex = 1;
			this.effect.Text = "0 - Whacky Pool of Fluid";
			// 
			// ceilingLight
			// 
			this.ceilingLight.AutoSize = true;
			this.ceilingLight.Location = new System.Drawing.Point(124, 63);
			this.ceilingLight.Name = "ceilingLight";
			this.ceilingLight.Size = new System.Drawing.Size(13, 13);
			this.ceilingLight.TabIndex = 29;
			this.ceilingLight.Text = "--";
			// 
			// ceilingScale
			// 
			this.ceilingScale.AutoSize = true;
			this.ceilingScale.Location = new System.Drawing.Point(124, 47);
			this.ceilingScale.Name = "ceilingScale";
			this.ceilingScale.Size = new System.Drawing.Size(25, 13);
			this.ceilingScale.TabIndex = 28;
			this.ceilingScale.Text = "--, --";
			// 
			// ceilingAngle
			// 
			this.ceilingAngle.AutoSize = true;
			this.ceilingAngle.Location = new System.Drawing.Point(124, 31);
			this.ceilingAngle.Name = "ceilingAngle";
			this.ceilingAngle.Size = new System.Drawing.Size(13, 13);
			this.ceilingAngle.TabIndex = 25;
			this.ceilingAngle.Text = "--";
			// 
			// ceilingOffset
			// 
			this.ceilingOffset.AutoSize = true;
			this.ceilingOffset.Location = new System.Drawing.Point(124, 15);
			this.ceilingOffset.Name = "ceilingOffset";
			this.ceilingOffset.Size = new System.Drawing.Size(25, 13);
			this.ceilingOffset.TabIndex = 23;
			this.ceilingOffset.Text = "--, --";
			// 
			// ceilingpanel
			// 
			this.ceilingpanel.Controls.Add(this.ceilingOffsetLabel);
			this.ceilingpanel.Controls.Add(this.ceilingOffset);
			this.ceilingpanel.Controls.Add(this.ceilingLight);
			this.ceilingpanel.Controls.Add(this.ceilingAngleLabel);
			this.ceilingpanel.Controls.Add(this.ceilingAngle);
			this.ceilingpanel.Controls.Add(this.ceilingScaleLabel);
			this.ceilingpanel.Controls.Add(this.ceilingScale);
			this.ceilingpanel.Controls.Add(this.ceilingLightLabel);
			this.ceilingpanel.Controls.Add(this.ceilingname);
			this.ceilingpanel.Controls.Add(this.ceilingtex);
			this.ceilingpanel.Location = new System.Drawing.Point(506, 0);
			this.ceilingpanel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.ceilingpanel.Name = "ceilingpanel";
			this.ceilingpanel.Size = new System.Drawing.Size(200, 100);
			this.ceilingpanel.TabIndex = 3;
			this.ceilingpanel.TabStop = false;
			this.ceilingpanel.Text = " Ceiling ";
			// 
			// ceilingname
			// 
			this.ceilingname.AutoSize = true;
			this.ceilingname.Location = new System.Drawing.Point(6, 81);
			this.ceilingname.Name = "ceilingname";
			this.ceilingname.Size = new System.Drawing.Size(73, 13);
			this.ceilingname.TabIndex = 1;
			this.ceilingname.Text = "BROWNHUG";
			// 
			// ceilingtex
			// 
			this.ceilingtex.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.ceilingtex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.ceilingtex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.ceilingtex.Controls.Add(this.labelCeilTextureSize);
			this.ceilingtex.Location = new System.Drawing.Point(7, 14);
			this.ceilingtex.Name = "ceilingtex";
			this.ceilingtex.Size = new System.Drawing.Size(64, 64);
			this.ceilingtex.TabIndex = 0;
			// 
			// labelCeilTextureSize
			// 
			this.labelCeilTextureSize.AutoSize = true;
			this.labelCeilTextureSize.BackColor = System.Drawing.Color.Black;
			this.labelCeilTextureSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelCeilTextureSize.ForeColor = System.Drawing.Color.White;
			this.labelCeilTextureSize.Location = new System.Drawing.Point(1, 1);
			this.labelCeilTextureSize.MaximumSize = new System.Drawing.Size(0, 13);
			this.labelCeilTextureSize.Name = "labelCeilTextureSize";
			this.labelCeilTextureSize.Size = new System.Drawing.Size(48, 13);
			this.labelCeilTextureSize.TabIndex = 1;
			this.labelCeilTextureSize.Text = "128x128";
			// 
			// floorpanel
			// 
			this.floorpanel.Controls.Add(this.floorLight);
			this.floorpanel.Controls.Add(this.floorOffsetLabel);
			this.floorpanel.Controls.Add(this.floorLightLabel);
			this.floorpanel.Controls.Add(this.floorScale);
			this.floorpanel.Controls.Add(this.floorOffset);
			this.floorpanel.Controls.Add(this.floorAngleLabel);
			this.floorpanel.Controls.Add(this.floorScaleLabel);
			this.floorpanel.Controls.Add(this.floorAngle);
			this.floorpanel.Controls.Add(this.floorname);
			this.floorpanel.Controls.Add(this.floortex);
			this.floorpanel.Location = new System.Drawing.Point(303, 0);
			this.floorpanel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.floorpanel.Name = "floorpanel";
			this.floorpanel.Size = new System.Drawing.Size(200, 100);
			this.floorpanel.TabIndex = 4;
			this.floorpanel.TabStop = false;
			this.floorpanel.Text = " Floor ";
			// 
			// floorLight
			// 
			this.floorLight.AutoSize = true;
			this.floorLight.Location = new System.Drawing.Point(124, 63);
			this.floorLight.Name = "floorLight";
			this.floorLight.Size = new System.Drawing.Size(13, 13);
			this.floorLight.TabIndex = 29;
			this.floorLight.Text = "--";
			// 
			// floorScale
			// 
			this.floorScale.AutoSize = true;
			this.floorScale.Location = new System.Drawing.Point(124, 47);
			this.floorScale.Name = "floorScale";
			this.floorScale.Size = new System.Drawing.Size(25, 13);
			this.floorScale.TabIndex = 28;
			this.floorScale.Text = "--, --";
			// 
			// floorOffset
			// 
			this.floorOffset.AutoSize = true;
			this.floorOffset.Location = new System.Drawing.Point(124, 15);
			this.floorOffset.Name = "floorOffset";
			this.floorOffset.Size = new System.Drawing.Size(25, 13);
			this.floorOffset.TabIndex = 23;
			this.floorOffset.Text = "--, --";
			// 
			// floorAngle
			// 
			this.floorAngle.AutoSize = true;
			this.floorAngle.Location = new System.Drawing.Point(124, 31);
			this.floorAngle.Name = "floorAngle";
			this.floorAngle.Size = new System.Drawing.Size(13, 13);
			this.floorAngle.TabIndex = 25;
			this.floorAngle.Text = "--";
			// 
			// floorname
			// 
			this.floorname.AutoSize = true;
			this.floorname.Location = new System.Drawing.Point(6, 81);
			this.floorname.Name = "floorname";
			this.floorname.Size = new System.Drawing.Size(73, 13);
			this.floorname.TabIndex = 1;
			this.floorname.Text = "BROWNHUG";
			// 
			// floortex
			// 
			this.floortex.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.floortex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.floortex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.floortex.Controls.Add(this.labelFloorTextureSize);
			this.floortex.Location = new System.Drawing.Point(7, 14);
			this.floortex.Name = "floortex";
			this.floortex.Size = new System.Drawing.Size(64, 64);
			this.floortex.TabIndex = 0;
			// 
			// labelFloorTextureSize
			// 
			this.labelFloorTextureSize.AutoSize = true;
			this.labelFloorTextureSize.BackColor = System.Drawing.Color.Black;
			this.labelFloorTextureSize.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelFloorTextureSize.ForeColor = System.Drawing.Color.White;
			this.labelFloorTextureSize.Location = new System.Drawing.Point(1, 1);
			this.labelFloorTextureSize.MaximumSize = new System.Drawing.Size(0, 13);
			this.labelFloorTextureSize.Name = "labelFloorTextureSize";
			this.labelFloorTextureSize.Size = new System.Drawing.Size(48, 13);
			this.labelFloorTextureSize.TabIndex = 0;
			this.labelFloorTextureSize.Text = "128x128";
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Controls.Add(this.sectorinfo);
			this.flowLayoutPanel1.Controls.Add(this.floorpanel);
			this.flowLayoutPanel1.Controls.Add(this.ceilingpanel);
			this.flowLayoutPanel1.Controls.Add(this.flagsPanel);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(1400, 100);
			this.flowLayoutPanel1.TabIndex = 5;
			this.flowLayoutPanel1.WrapContents = false;
			// 
			// flagsPanel
			// 
			this.flagsPanel.Controls.Add(this.flags);
			this.flagsPanel.Location = new System.Drawing.Point(709, 0);
			this.flagsPanel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.flagsPanel.Name = "flagsPanel";
			this.flagsPanel.Size = new System.Drawing.Size(455, 100);
			this.flagsPanel.TabIndex = 7;
			this.flagsPanel.TabStop = false;
			this.flagsPanel.Text = " Flags";
			// 
			// flags
			// 
			this.flags.BackColor = System.Drawing.SystemColors.Control;
			this.flags.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.flags.CheckBoxes = true;
			this.flags.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.flags.Location = new System.Drawing.Point(6, 18);
			this.flags.Name = "flags";
			this.flags.Scrollable = false;
			this.flags.ShowGroups = false;
			this.flags.Size = new System.Drawing.Size(443, 73);
			this.flags.TabIndex = 0;
			this.flags.UseCompatibleStateImageBehavior = false;
			this.flags.View = System.Windows.Forms.View.List;
			// 
			// SectorInfoPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.flowLayoutPanel1);
			this.MaximumSize = new System.Drawing.Size(10000, 100);
			this.MinimumSize = new System.Drawing.Size(100, 100);
			this.Name = "SectorInfoPanel";
			this.Size = new System.Drawing.Size(1400, 100);
			this.sectorinfo.ResumeLayout(false);
			this.sectorinfo.PerformLayout();
			this.ceilingpanel.ResumeLayout(false);
			this.ceilingpanel.PerformLayout();
			this.ceilingtex.ResumeLayout(false);
			this.ceilingtex.PerformLayout();
			this.floorpanel.ResumeLayout(false);
			this.floorpanel.PerformLayout();
			this.floortex.ResumeLayout(false);
			this.floortex.PerformLayout();
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flagsPanel.ResumeLayout(false);
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
		private System.Windows.Forms.Label labelCeilTextureSize;
		private System.Windows.Forms.Label labelFloorTextureSize;
		private System.Windows.Forms.Panel panelFadeColor;
		private System.Windows.Forms.Panel panelLightColor;
		private System.Windows.Forms.Label labelFade;
		private System.Windows.Forms.Label labelLight;
		private System.Windows.Forms.GroupBox flagsPanel;
		private System.Windows.Forms.ListView flags;
		private System.Windows.Forms.Label labelTag;
		private System.Windows.Forms.Label labelEffect;
	}
}
