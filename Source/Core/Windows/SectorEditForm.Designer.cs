namespace CodeImp.DoomBuilder.Windows
{
	partial class SectorEditForm
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
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.GroupBox groupeffect;
			System.Windows.Forms.Label label9;
			System.Windows.Forms.Label label8;
			System.Windows.Forms.GroupBox groupfloorceiling;
			System.Windows.Forms.Label label6;
			System.Windows.Forms.Label label5;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label4;
			this.tagSelector = new CodeImp.DoomBuilder.GZBuilder.Controls.TagSelector();
			this.brightness = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.browseeffect = new System.Windows.Forms.Button();
			this.effect = new CodeImp.DoomBuilder.Controls.ActionSelectorControl();
			this.ceilingheight = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.sectorheightlabel = new System.Windows.Forms.Label();
			this.sectorheight = new System.Windows.Forms.Label();
			this.floortex = new CodeImp.DoomBuilder.Controls.FlatSelectorControl();
			this.floorheight = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.ceilingtex = new CodeImp.DoomBuilder.Controls.FlatSelectorControl();
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			this.flatSelectorControl2 = new CodeImp.DoomBuilder.Controls.FlatSelectorControl();
			this.flatSelectorControl1 = new CodeImp.DoomBuilder.Controls.FlatSelectorControl();
			this.panel1 = new System.Windows.Forms.Panel();
			label1 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			groupeffect = new System.Windows.Forms.GroupBox();
			label9 = new System.Windows.Forms.Label();
			label8 = new System.Windows.Forms.Label();
			groupfloorceiling = new System.Windows.Forms.GroupBox();
			label6 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			groupeffect.SuspendLayout();
			groupfloorceiling.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.Location = new System.Drawing.Point(271, 18);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(83, 16);
			label1.TabIndex = 15;
			label1.Text = "Floor";
			label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label3
			// 
			label3.Location = new System.Drawing.Point(363, 18);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(83, 16);
			label3.TabIndex = 14;
			label3.Text = "Ceiling";
			label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// tagSelector
			// 
			this.tagSelector.Location = new System.Drawing.Point(47, 52);
			this.tagSelector.Name = "tagSelector";
			this.tagSelector.Size = new System.Drawing.Size(384, 34);
			this.tagSelector.TabIndex = 0;
			// 
			// groupeffect
			// 
			groupeffect.Controls.Add(this.browseeffect);
			groupeffect.Controls.Add(this.effect);
			groupeffect.Controls.Add(label8);
			groupeffect.Controls.Add(this.tagSelector);
			groupeffect.Location = new System.Drawing.Point(3, 165);
			groupeffect.Name = "groupeffect";
			groupeffect.Size = new System.Drawing.Size(436, 92);
			groupeffect.TabIndex = 1;
			groupeffect.TabStop = false;
			groupeffect.Text = "Effect and Identification";
			// 
			// brightness
			// 
			this.brightness.AllowDecimal = false;
			this.brightness.AllowNegative = false;
			this.brightness.AllowRelative = true;
			this.brightness.ButtonStep = 8F;
			this.brightness.Location = new System.Drawing.Point(99, 124);
			this.brightness.Name = "brightness";
			this.brightness.Size = new System.Drawing.Size(73, 24);
			this.brightness.StepValues = null;
			this.brightness.TabIndex = 24;
			// 
			// browseeffect
			// 
			this.browseeffect.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.browseeffect.Image = global::CodeImp.DoomBuilder.Properties.Resources.List;
			this.browseeffect.Location = new System.Drawing.Point(402, 26);
			this.browseeffect.Name = "browseeffect";
			this.browseeffect.Padding = new System.Windows.Forms.Padding(0, 0, 1, 3);
			this.browseeffect.Size = new System.Drawing.Size(28, 25);
			this.browseeffect.TabIndex = 1;
			this.browseeffect.Text = " ";
			this.browseeffect.UseVisualStyleBackColor = true;
			this.browseeffect.Click += new System.EventHandler(this.browseeffect_Click);
			// 
			// label9
			// 
			label9.AutoSize = true;
			label9.Location = new System.Drawing.Point(31, 129);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(62, 14);
			label9.TabIndex = 2;
			label9.Text = "Brightness:";
			// 
			// effect
			// 
			this.effect.BackColor = System.Drawing.Color.Transparent;
			this.effect.Cursor = System.Windows.Forms.Cursors.Default;
			this.effect.Empty = false;
			this.effect.GeneralizedCategories = null;
			this.effect.Location = new System.Drawing.Point(89, 28);
			this.effect.Name = "effect";
			this.effect.Size = new System.Drawing.Size(307, 21);
			this.effect.TabIndex = 0;
			this.effect.Value = 402;
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.Location = new System.Drawing.Point(38, 31);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(45, 14);
			label8.TabIndex = 0;
			label8.Text = "Special:";
			// 
			// groupfloorceiling
			// 
			groupfloorceiling.Controls.Add(this.brightness);
			groupfloorceiling.Controls.Add(this.ceilingheight);
			groupfloorceiling.Controls.Add(label6);
			groupfloorceiling.Controls.Add(label9);
			groupfloorceiling.Controls.Add(label5);
			groupfloorceiling.Controls.Add(label2);
			groupfloorceiling.Controls.Add(this.sectorheightlabel);
			groupfloorceiling.Controls.Add(label4);
			groupfloorceiling.Controls.Add(this.sectorheight);
			groupfloorceiling.Controls.Add(this.floortex);
			groupfloorceiling.Controls.Add(this.floorheight);
			groupfloorceiling.Controls.Add(this.ceilingtex);
			groupfloorceiling.Location = new System.Drawing.Point(3, 3);
			groupfloorceiling.Name = "groupfloorceiling";
			groupfloorceiling.Size = new System.Drawing.Size(436, 156);
			groupfloorceiling.TabIndex = 0;
			groupfloorceiling.TabStop = false;
			groupfloorceiling.Text = "Floor and Ceiling ";
			// 
			// ceilingheight
			// 
			this.ceilingheight.AllowDecimal = false;
			this.ceilingheight.AllowNegative = true;
			this.ceilingheight.AllowRelative = true;
			this.ceilingheight.ButtonStep = 8F;
			this.ceilingheight.Location = new System.Drawing.Point(99, 35);
			this.ceilingheight.Name = "ceilingheight";
			this.ceilingheight.Size = new System.Drawing.Size(88, 24);
			this.ceilingheight.StepValues = null;
			this.ceilingheight.TabIndex = 22;
			this.ceilingheight.WhenTextChanged += new System.EventHandler(this.ceilingheight_TextChanged);
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(20, 40);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(73, 14);
			label6.TabIndex = 19;
			label6.Text = "Ceiling height:";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(27, 70);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(66, 14);
			label5.TabIndex = 17;
			label5.Text = "Floor height:";
			// 
			// label2
			// 
			label2.Location = new System.Drawing.Point(242, 16);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(83, 16);
			label2.TabIndex = 15;
			label2.Text = "Floor";
			label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// sectorheightlabel
			// 
			this.sectorheightlabel.AutoSize = true;
			this.sectorheightlabel.Location = new System.Drawing.Point(19, 100);
			this.sectorheightlabel.Name = "sectorheightlabel";
			this.sectorheightlabel.Size = new System.Drawing.Size(74, 14);
			this.sectorheightlabel.TabIndex = 20;
			this.sectorheightlabel.Text = "Sector height:";
			// 
			// label4
			// 
			label4.Location = new System.Drawing.Point(337, 16);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(83, 16);
			label4.TabIndex = 14;
			label4.Text = "Ceiling";
			label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// sectorheight
			// 
			this.sectorheight.AutoSize = true;
			this.sectorheight.Location = new System.Drawing.Point(100, 100);
			this.sectorheight.Name = "sectorheight";
			this.sectorheight.Size = new System.Drawing.Size(13, 14);
			this.sectorheight.TabIndex = 21;
			this.sectorheight.Text = "0";
			// 
			// floortex
			// 
			this.floortex.Location = new System.Drawing.Point(242, 35);
			this.floortex.Name = "floortex";
			this.floortex.Size = new System.Drawing.Size(83, 105);
			this.floortex.TabIndex = 2;
			this.floortex.TextureName = "";
			// 
			// floorheight
			// 
			this.floorheight.AllowDecimal = false;
			this.floorheight.AllowNegative = true;
			this.floorheight.AllowRelative = true;
			this.floorheight.ButtonStep = 8F;
			this.floorheight.Location = new System.Drawing.Point(99, 65);
			this.floorheight.Name = "floorheight";
			this.floorheight.Size = new System.Drawing.Size(88, 24);
			this.floorheight.StepValues = null;
			this.floorheight.TabIndex = 23;
			this.floorheight.WhenTextChanged += new System.EventHandler(this.floorheight_TextChanged);
			// 
			// ceilingtex
			// 
			this.ceilingtex.Location = new System.Drawing.Point(337, 35);
			this.ceilingtex.Name = "ceilingtex";
			this.ceilingtex.Size = new System.Drawing.Size(83, 105);
			this.ceilingtex.TabIndex = 3;
			this.ceilingtex.TextureName = "";
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(346, 278);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(112, 25);
			this.cancel.TabIndex = 2;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// apply
			// 
			this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.apply.Location = new System.Drawing.Point(227, 278);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 1;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// flatSelectorControl2
			// 
			this.flatSelectorControl2.Location = new System.Drawing.Point(271, 37);
			this.flatSelectorControl2.Name = "flatSelectorControl2";
			this.flatSelectorControl2.Size = new System.Drawing.Size(83, 105);
			this.flatSelectorControl2.TabIndex = 13;
			this.flatSelectorControl2.TextureName = "";
			// 
			// flatSelectorControl1
			// 
			this.flatSelectorControl1.Location = new System.Drawing.Point(363, 37);
			this.flatSelectorControl1.Name = "flatSelectorControl1";
			this.flatSelectorControl1.Size = new System.Drawing.Size(83, 105);
			this.flatSelectorControl1.TabIndex = 12;
			this.flatSelectorControl1.TextureName = "";
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.panel1.Controls.Add(groupfloorceiling);
			this.panel1.Controls.Add(groupeffect);
			this.panel1.Location = new System.Drawing.Point(12, 10);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(443, 260);
			this.panel1.TabIndex = 3;
			// 
			// SectorEditForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(468, 310);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SectorEditForm";
			this.Opacity = 0;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit Sector";
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.SectorEditForm_HelpRequested);
			groupeffect.ResumeLayout(false);
			groupeffect.PerformLayout();
			groupfloorceiling.ResumeLayout(false);
			groupfloorceiling.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private CodeImp.DoomBuilder.Controls.FlatSelectorControl floortex;
		private CodeImp.DoomBuilder.Controls.FlatSelectorControl ceilingtex;
		private CodeImp.DoomBuilder.Controls.FlatSelectorControl flatSelectorControl2;
		private CodeImp.DoomBuilder.Controls.FlatSelectorControl flatSelectorControl1;
		private System.Windows.Forms.Label sectorheight;
		private CodeImp.DoomBuilder.Controls.ActionSelectorControl effect;
		private System.Windows.Forms.Button browseeffect;
		private System.Windows.Forms.Label sectorheightlabel;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox ceilingheight;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox floorheight;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox brightness;
		private CodeImp.DoomBuilder.GZBuilder.Controls.TagSelector tagSelector;
		private System.Windows.Forms.Panel panel1;
	}
}