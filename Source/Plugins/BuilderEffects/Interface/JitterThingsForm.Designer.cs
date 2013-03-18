namespace CodeImp.DoomBuilder.BuilderEffects
{
	partial class JitterThingsForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.bApply = new System.Windows.Forms.Button();
			this.bCancel = new System.Windows.Forms.Button();
			this.bUpdateTranslation = new System.Windows.Forms.Button();
			this.bUpdateAngle = new System.Windows.Forms.Button();
			this.bUpdateHeight = new System.Windows.Forms.Button();
			this.cbRelativePos = new System.Windows.Forms.CheckBox();
			this.cbRelativeHeight = new System.Windows.Forms.CheckBox();
			this.positionJitterAmmount = new IntControl();
			this.rotationJitterAmmount = new IntControl();
			this.heightJitterAmmount = new IntControl();
			this.SuspendLayout();
			// 
			// bApply
			// 
			this.bApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.bApply.Location = new System.Drawing.Point(282, 96);
			this.bApply.Name = "bApply";
			this.bApply.Size = new System.Drawing.Size(75, 23);
			this.bApply.TabIndex = 0;
			this.bApply.Text = "Apply";
			this.bApply.UseVisualStyleBackColor = true;
			this.bApply.Click += new System.EventHandler(this.bApply_Click);
			// 
			// bCancel
			// 
			this.bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.bCancel.Location = new System.Drawing.Point(201, 96);
			this.bCancel.Name = "bCancel";
			this.bCancel.Size = new System.Drawing.Size(75, 23);
			this.bCancel.TabIndex = 1;
			this.bCancel.Text = "Cancel";
			this.bCancel.UseVisualStyleBackColor = true;
			this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
			// 
			// bUpdateTranslation
			// 
			this.bUpdateTranslation.Image = global::CodeImp.DoomBuilder.BuilderEffects.Properties.Resources.Update;
			this.bUpdateTranslation.Location = new System.Drawing.Point(337, 11);
			this.bUpdateTranslation.Name = "bUpdateTranslation";
			this.bUpdateTranslation.Size = new System.Drawing.Size(23, 23);
			this.bUpdateTranslation.TabIndex = 5;
			this.bUpdateTranslation.UseVisualStyleBackColor = true;
			this.bUpdateTranslation.Click += new System.EventHandler(this.bUpdateTranslation_Click);
			// 
			// bUpdateAngle
			// 
			this.bUpdateAngle.Image = global::CodeImp.DoomBuilder.BuilderEffects.Properties.Resources.Update;
			this.bUpdateAngle.Location = new System.Drawing.Point(337, 67);
			this.bUpdateAngle.Name = "bUpdateAngle";
			this.bUpdateAngle.Size = new System.Drawing.Size(23, 23);
			this.bUpdateAngle.TabIndex = 5;
			this.bUpdateAngle.UseVisualStyleBackColor = true;
			this.bUpdateAngle.Click += new System.EventHandler(this.bUpdateAngle_Click);
			// 
			// bUpdateHeight
			// 
			this.bUpdateHeight.Image = global::CodeImp.DoomBuilder.BuilderEffects.Properties.Resources.Update;
			this.bUpdateHeight.Location = new System.Drawing.Point(337, 39);
			this.bUpdateHeight.Name = "bUpdateHeight";
			this.bUpdateHeight.Size = new System.Drawing.Size(23, 23);
			this.bUpdateHeight.TabIndex = 5;
			this.bUpdateHeight.UseVisualStyleBackColor = true;
			this.bUpdateHeight.Click += new System.EventHandler(this.bUpdateHeight_Click);
			// 
			// cbRelativePos
			// 
			this.cbRelativePos.AutoSize = true;
			this.cbRelativePos.Location = new System.Drawing.Point(12, 16);
			this.cbRelativePos.Name = "cbRelativePos";
			this.cbRelativePos.Size = new System.Drawing.Size(64, 18);
			this.cbRelativePos.TabIndex = 11;
			this.cbRelativePos.Text = "Relative";
			this.cbRelativePos.UseVisualStyleBackColor = true;
			this.cbRelativePos.CheckedChanged += new System.EventHandler(this.cbRelativePos_CheckedChanged);
			// 
			// cbRelativeHeight
			// 
			this.cbRelativeHeight.AutoSize = true;
			this.cbRelativeHeight.Location = new System.Drawing.Point(12, 44);
			this.cbRelativeHeight.Name = "cbRelativeHeight";
			this.cbRelativeHeight.Size = new System.Drawing.Size(64, 18);
			this.cbRelativeHeight.TabIndex = 12;
			this.cbRelativeHeight.Text = "Relative";
			this.cbRelativeHeight.UseVisualStyleBackColor = true;
			this.cbRelativeHeight.CheckedChanged += new System.EventHandler(this.cbRelativeHeight_CheckedChanged);
			// 
			// positionJitterAmmount
			// 
			this.positionJitterAmmount.AllowNegative = false;
			this.positionJitterAmmount.ExtendedLimits = true;
			this.positionJitterAmmount.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.positionJitterAmmount.Label = "Position (%):";
			this.positionJitterAmmount.Location = new System.Drawing.Point(65, 12);
			this.positionJitterAmmount.Maximum = 100;
			this.positionJitterAmmount.Minimum = 0;
			this.positionJitterAmmount.Name = "positionJitterAmmount";
			this.positionJitterAmmount.Size = new System.Drawing.Size(266, 22);
			this.positionJitterAmmount.TabIndex = 6;
			this.positionJitterAmmount.Value = 0;
			this.positionJitterAmmount.OnValueChanging += new System.EventHandler(this.positionJitterAmmount_OnValueChanged);
			// 
			// rotationJitterAmmount
			// 
			this.rotationJitterAmmount.AllowNegative = false;
			this.rotationJitterAmmount.ExtendedLimits = false;
			this.rotationJitterAmmount.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.rotationJitterAmmount.Label = "Angle:";
			this.rotationJitterAmmount.Location = new System.Drawing.Point(65, 68);
			this.rotationJitterAmmount.Maximum = 359;
			this.rotationJitterAmmount.Minimum = 0;
			this.rotationJitterAmmount.Name = "rotationJitterAmmount";
			this.rotationJitterAmmount.Size = new System.Drawing.Size(266, 22);
			this.rotationJitterAmmount.TabIndex = 8;
			this.rotationJitterAmmount.Value = 0;
			this.rotationJitterAmmount.OnValueChanging += new System.EventHandler(this.rotationJitterAmmount_OnValueChanged);
			// 
			// heightJitterAmmount
			// 
			this.heightJitterAmmount.AllowNegative = false;
			this.heightJitterAmmount.ExtendedLimits = false;
			this.heightJitterAmmount.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.heightJitterAmmount.Label = "Height (%):";
			this.heightJitterAmmount.Location = new System.Drawing.Point(65, 40);
			this.heightJitterAmmount.Maximum = 100;
			this.heightJitterAmmount.Minimum = 0;
			this.heightJitterAmmount.Name = "heightJitterAmmount";
			this.heightJitterAmmount.Size = new System.Drawing.Size(266, 22);
			this.heightJitterAmmount.TabIndex = 6;
			this.heightJitterAmmount.Value = 0;
			this.heightJitterAmmount.OnValueChanging += new System.EventHandler(this.heightJitterAmmount_OnValueChanging);
			// 
			// JitterThingsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(366, 122);
			this.Controls.Add(this.cbRelativeHeight);
			this.Controls.Add(this.cbRelativePos);
			this.Controls.Add(this.rotationJitterAmmount);
			this.Controls.Add(this.bUpdateAngle);
			this.Controls.Add(this.heightJitterAmmount);
			this.Controls.Add(this.bUpdateHeight);
			this.Controls.Add(this.bUpdateTranslation);
			this.Controls.Add(this.positionJitterAmmount);
			this.Controls.Add(this.bCancel);
			this.Controls.Add(this.bApply);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "JitterThingsForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
			this.Text = "Jitter Settings";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.JitterThingsForm_FormClosing);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button bApply;
		private System.Windows.Forms.Button bCancel;
		private System.Windows.Forms.Button bUpdateTranslation;
		private IntControl positionJitterAmmount;
		private System.Windows.Forms.Button bUpdateAngle;
		private IntControl rotationJitterAmmount;
		private IntControl heightJitterAmmount;
		private System.Windows.Forms.Button bUpdateHeight;
		private System.Windows.Forms.CheckBox cbRelativePos;
		private System.Windows.Forms.CheckBox cbRelativeHeight;
	}
}