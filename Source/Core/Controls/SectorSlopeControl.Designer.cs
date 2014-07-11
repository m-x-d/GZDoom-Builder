namespace CodeImp.DoomBuilder.Controls
{
	partial class SectorSlopeControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.slopeangle = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.label23 = new System.Windows.Forms.Label();
			this.sloperotation = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.label24 = new System.Windows.Forms.Label();
			this.reset = new System.Windows.Forms.Button();
			this.slopeoffset = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.label18 = new System.Windows.Forms.Label();
			this.rotationcontrol = new CodeImp.DoomBuilder.GZBuilder.Controls.AngleControl();
			this.angletrackbar = new System.Windows.Forms.TrackBar();
			this.label1 = new System.Windows.Forms.Label();
			this.pivotmodeselector = new System.Windows.Forms.ComboBox();
			((System.ComponentModel.ISupportInitialize)(this.angletrackbar)).BeginInit();
			this.SuspendLayout();
			// 
			// slopeangle
			// 
			this.slopeangle.AllowDecimal = true;
			this.slopeangle.AllowNegative = true;
			this.slopeangle.AllowRelative = true;
			this.slopeangle.ButtonStep = 1;
			this.slopeangle.ButtonStepFloat = 1F;
			this.slopeangle.Location = new System.Drawing.Point(85, 78);
			this.slopeangle.Name = "slopeangle";
			this.slopeangle.Size = new System.Drawing.Size(82, 24);
			this.slopeangle.StepValues = null;
			this.slopeangle.TabIndex = 29;
			this.slopeangle.WhenTextChanged += new System.EventHandler(this.slopeangle_WhenTextChanged);
			// 
			// label23
			// 
			this.label23.Location = new System.Drawing.Point(3, 83);
			this.label23.Name = "label23";
			this.label23.Size = new System.Drawing.Size(76, 14);
			this.label23.TabIndex = 28;
			this.label23.Text = "Slope angle:";
			this.label23.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// sloperotation
			// 
			this.sloperotation.AllowDecimal = true;
			this.sloperotation.AllowNegative = true;
			this.sloperotation.AllowRelative = true;
			this.sloperotation.ButtonStep = 1;
			this.sloperotation.ButtonStepFloat = 1F;
			this.sloperotation.Location = new System.Drawing.Point(85, 48);
			this.sloperotation.Name = "sloperotation";
			this.sloperotation.Size = new System.Drawing.Size(82, 24);
			this.sloperotation.StepValues = null;
			this.sloperotation.TabIndex = 27;
			this.sloperotation.WhenTextChanged += new System.EventHandler(this.sloperotation_WhenTextChanged);
			// 
			// label24
			// 
			this.label24.Location = new System.Drawing.Point(3, 53);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(76, 14);
			this.label24.TabIndex = 26;
			this.label24.Text = "Rotation:";
			this.label24.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// reset
			// 
			this.reset.Location = new System.Drawing.Point(85, 138);
			this.reset.Name = "reset";
			this.reset.Size = new System.Drawing.Size(82, 23);
			this.reset.TabIndex = 25;
			this.reset.Text = "Reset";
			this.reset.UseVisualStyleBackColor = true;
			this.reset.Click += new System.EventHandler(this.reset_Click);
			// 
			// slopeoffset
			// 
			this.slopeoffset.AllowDecimal = true;
			this.slopeoffset.AllowNegative = true;
			this.slopeoffset.AllowRelative = true;
			this.slopeoffset.ButtonStep = 1;
			this.slopeoffset.ButtonStepFloat = 16F;
			this.slopeoffset.Location = new System.Drawing.Point(85, 108);
			this.slopeoffset.Name = "slopeoffset";
			this.slopeoffset.Size = new System.Drawing.Size(82, 24);
			this.slopeoffset.StepValues = null;
			this.slopeoffset.TabIndex = 24;
			this.slopeoffset.WhenTextChanged += new System.EventHandler(this.slopeoffset_WhenTextChanged);
			// 
			// label18
			// 
			this.label18.Location = new System.Drawing.Point(3, 113);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(76, 14);
			this.label18.TabIndex = 23;
			this.label18.Text = "Height offset:";
			this.label18.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// rotationcontrol
			// 
			this.rotationcontrol.Angle = 0;
			this.rotationcontrol.Location = new System.Drawing.Point(173, 36);
			this.rotationcontrol.Name = "rotationcontrol";
			this.rotationcontrol.Size = new System.Drawing.Size(44, 44);
			this.rotationcontrol.TabIndex = 56;
			this.rotationcontrol.AngleChanged += new CodeImp.DoomBuilder.GZBuilder.Controls.AngleControl.AngleChangedDelegate(this.rotationcontrol_AngleChanged);
			// 
			// angletrackbar
			// 
			this.angletrackbar.Location = new System.Drawing.Point(173, 82);
			this.angletrackbar.Maximum = 85;
			this.angletrackbar.Minimum = -85;
			this.angletrackbar.Name = "angletrackbar";
			this.angletrackbar.Size = new System.Drawing.Size(175, 45);
			this.angletrackbar.TabIndex = 57;
			this.angletrackbar.TickFrequency = 10;
			this.angletrackbar.ValueChanged += new System.EventHandler(this.angletrackbar_ValueChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(3, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(76, 14);
			this.label1.TabIndex = 58;
			this.label1.Text = "Pivot:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// pivotmodeselector
			// 
			this.pivotmodeselector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.pivotmodeselector.FormattingEnabled = true;
			this.pivotmodeselector.Items.AddRange(new object[] {
            "Origin",
            "Selection center",
            "Sector center"});
			this.pivotmodeselector.Location = new System.Drawing.Point(85, 9);
			this.pivotmodeselector.Name = "pivotmodeselector";
			this.pivotmodeselector.Size = new System.Drawing.Size(132, 21);
			this.pivotmodeselector.TabIndex = 59;
			this.pivotmodeselector.SelectedIndexChanged += new System.EventHandler(this.pivotmodeselector_SelectedIndexChanged);
			// 
			// SectorSlopeControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.pivotmodeselector);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.angletrackbar);
			this.Controls.Add(this.rotationcontrol);
			this.Controls.Add(this.slopeangle);
			this.Controls.Add(this.label23);
			this.Controls.Add(this.sloperotation);
			this.Controls.Add(this.label24);
			this.Controls.Add(this.reset);
			this.Controls.Add(this.slopeoffset);
			this.Controls.Add(this.label18);
			this.Name = "SectorSlopeControl";
			this.Size = new System.Drawing.Size(353, 169);
			((System.ComponentModel.ISupportInitialize)(this.angletrackbar)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ButtonsNumericTextbox slopeangle;
		private System.Windows.Forms.Label label23;
		private ButtonsNumericTextbox sloperotation;
		private System.Windows.Forms.Label label24;
		private System.Windows.Forms.Button reset;
		private ButtonsNumericTextbox slopeoffset;
		private System.Windows.Forms.Label label18;
		private CodeImp.DoomBuilder.GZBuilder.Controls.AngleControl rotationcontrol;
		private System.Windows.Forms.TrackBar angletrackbar;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox pivotmodeselector;
	}
}
