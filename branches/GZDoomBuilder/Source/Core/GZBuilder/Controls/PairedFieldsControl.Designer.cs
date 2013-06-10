﻿namespace CodeImp.DoomBuilder.GZBuilder.Controls
{
	partial class PairedFieldsControl
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
			this.label = new System.Windows.Forms.Label();
			this.bReset = new System.Windows.Forms.Button();
			this.value1 = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.value2 = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.SuspendLayout();
			// 
			// label
			// 
			this.label.Location = new System.Drawing.Point(11, 6);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(76, 14);
			this.label.TabIndex = 36;
			this.label.Text = "Upper Offset:";
			this.label.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// bReset
			// 
			this.bReset.Image = global::CodeImp.DoomBuilder.Properties.Resources.Reset;
			this.bReset.Location = new System.Drawing.Point(222, 1);
			this.bReset.Name = "bReset";
			this.bReset.Size = new System.Drawing.Size(23, 23);
			this.bReset.TabIndex = 39;
			this.bReset.UseVisualStyleBackColor = true;
			this.bReset.Click += new System.EventHandler(this.bReset_Click);
			// 
			// value1
			// 
			this.value1.AllowDecimal = false;
			this.value1.AllowNegative = true;
			this.value1.AllowRelative = true;
			this.value1.ButtonStep = 1;
			this.value1.Location = new System.Drawing.Point(89, 1);
			this.value1.Name = "value1";
			this.value1.Size = new System.Drawing.Size(62, 24);
			this.value1.StepValues = null;
			this.value1.TabIndex = 37;
			this.value1.Tag = "offsetx_top";
			this.value1.WhenTextChanged += new System.EventHandler(this.value1_WhenTextChanged);
			// 
			// value2
			// 
			this.value2.AllowDecimal = false;
			this.value2.AllowNegative = true;
			this.value2.AllowRelative = true;
			this.value2.ButtonStep = 1;
			this.value2.Location = new System.Drawing.Point(157, 1);
			this.value2.Name = "value2";
			this.value2.Size = new System.Drawing.Size(62, 24);
			this.value2.StepValues = null;
			this.value2.TabIndex = 38;
			this.value2.Tag = "offsety_top";
			this.value2.WhenTextChanged += new System.EventHandler(this.value1_WhenTextChanged);
			// 
			// PairedFieldsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.bReset);
			this.Controls.Add(this.label);
			this.Controls.Add(this.value1);
			this.Controls.Add(this.value2);
			this.Name = "PairedFieldsControl";
			this.Size = new System.Drawing.Size(249, 26);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button bReset;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox value1;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox value2;
		private System.Windows.Forms.Label label;
	}
}
