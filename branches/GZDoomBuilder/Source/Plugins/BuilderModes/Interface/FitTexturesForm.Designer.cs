namespace CodeImp.DoomBuilder.BuilderModes
{
	partial class FitTexturesForm
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
			this.labelhorizrepeat = new System.Windows.Forms.Label();
			this.repeatgroup = new System.Windows.Forms.GroupBox();
			this.labelvertrepeat = new System.Windows.Forms.Label();
			this.horizrepeat = new System.Windows.Forms.NumericUpDown();
			this.vertrepeat = new System.Windows.Forms.NumericUpDown();
			this.resethoriz = new System.Windows.Forms.Button();
			this.resetvert = new System.Windows.Forms.Button();
			this.cbfitwidth = new System.Windows.Forms.CheckBox();
			this.accept = new System.Windows.Forms.Button();
			this.cancel = new System.Windows.Forms.Button();
			this.cbfitheight = new System.Windows.Forms.CheckBox();
			this.cbfitconnected = new System.Windows.Forms.CheckBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.repeatgroup.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horizrepeat)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.vertrepeat)).BeginInit();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelhorizrepeat
			// 
			this.labelhorizrepeat.AutoSize = true;
			this.labelhorizrepeat.Location = new System.Drawing.Point(23, 26);
			this.labelhorizrepeat.Name = "labelhorizrepeat";
			this.labelhorizrepeat.Size = new System.Drawing.Size(57, 13);
			this.labelhorizrepeat.TabIndex = 0;
			this.labelhorizrepeat.Text = "Horizontal:";
			// 
			// repeatgroup
			// 
			this.repeatgroup.Controls.Add(this.resetvert);
			this.repeatgroup.Controls.Add(this.resethoriz);
			this.repeatgroup.Controls.Add(this.vertrepeat);
			this.repeatgroup.Controls.Add(this.horizrepeat);
			this.repeatgroup.Controls.Add(this.labelvertrepeat);
			this.repeatgroup.Controls.Add(this.labelhorizrepeat);
			this.repeatgroup.Location = new System.Drawing.Point(12, 107);
			this.repeatgroup.Name = "repeatgroup";
			this.repeatgroup.Size = new System.Drawing.Size(183, 80);
			this.repeatgroup.TabIndex = 1;
			this.repeatgroup.TabStop = false;
			this.repeatgroup.Text = " Texture Repeating ";
			// 
			// labelvertrepeat
			// 
			this.labelvertrepeat.AutoSize = true;
			this.labelvertrepeat.Location = new System.Drawing.Point(33, 52);
			this.labelvertrepeat.Name = "labelvertrepeat";
			this.labelvertrepeat.Size = new System.Drawing.Size(45, 13);
			this.labelvertrepeat.TabIndex = 1;
			this.labelvertrepeat.Text = "Vertical:";
			// 
			// horizrepeat
			// 
			this.horizrepeat.Location = new System.Drawing.Point(84, 22);
			this.horizrepeat.Maximum = new decimal(new int[] {
            512,
            0,
            0,
            0});
			this.horizrepeat.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.horizrepeat.Name = "horizrepeat";
			this.horizrepeat.Size = new System.Drawing.Size(60, 20);
			this.horizrepeat.TabIndex = 2;
			this.horizrepeat.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.horizrepeat.ValueChanged += new System.EventHandler(this.repeat_ValueChanged);
			// 
			// vertrepeat
			// 
			this.vertrepeat.Location = new System.Drawing.Point(84, 48);
			this.vertrepeat.Maximum = new decimal(new int[] {
            512,
            0,
            0,
            0});
			this.vertrepeat.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.vertrepeat.Name = "vertrepeat";
			this.vertrepeat.Size = new System.Drawing.Size(60, 20);
			this.vertrepeat.TabIndex = 3;
			this.vertrepeat.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this.vertrepeat.ValueChanged += new System.EventHandler(this.repeat_ValueChanged);
			// 
			// resethoriz
			// 
			this.resethoriz.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Reset;
			this.resethoriz.Location = new System.Drawing.Point(150, 20);
			this.resethoriz.Name = "resethoriz";
			this.resethoriz.Size = new System.Drawing.Size(24, 24);
			this.resethoriz.TabIndex = 4;
			this.resethoriz.UseVisualStyleBackColor = true;
			this.resethoriz.Click += new System.EventHandler(this.resethoriz_Click);
			// 
			// resetvert
			// 
			this.resetvert.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Reset;
			this.resetvert.Location = new System.Drawing.Point(150, 46);
			this.resetvert.Name = "resetvert";
			this.resetvert.Size = new System.Drawing.Size(24, 24);
			this.resetvert.TabIndex = 5;
			this.resetvert.UseVisualStyleBackColor = true;
			this.resetvert.Click += new System.EventHandler(this.resetvert_Click);
			// 
			// cbfitwidth
			// 
			this.cbfitwidth.AutoSize = true;
			this.cbfitwidth.Location = new System.Drawing.Point(10, 19);
			this.cbfitwidth.Name = "cbfitwidth";
			this.cbfitwidth.Size = new System.Drawing.Size(65, 17);
			this.cbfitwidth.TabIndex = 2;
			this.cbfitwidth.Text = "Fit width";
			this.cbfitwidth.UseVisualStyleBackColor = true;
			this.cbfitwidth.CheckedChanged += new System.EventHandler(this.cbfitwidth_CheckedChanged);
			// 
			// accept
			// 
			this.accept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.accept.Location = new System.Drawing.Point(106, 194);
			this.accept.Name = "accept";
			this.accept.Size = new System.Drawing.Size(88, 24);
			this.accept.TabIndex = 6;
			this.accept.Text = "Apply";
			this.accept.UseVisualStyleBackColor = true;
			this.accept.Click += new System.EventHandler(this.accept_Click);
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.cancel.Location = new System.Drawing.Point(12, 194);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(88, 24);
			this.cancel.TabIndex = 7;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// cbfitheight
			// 
			this.cbfitheight.AutoSize = true;
			this.cbfitheight.Location = new System.Drawing.Point(10, 42);
			this.cbfitheight.Name = "cbfitheight";
			this.cbfitheight.Size = new System.Drawing.Size(69, 17);
			this.cbfitheight.TabIndex = 8;
			this.cbfitheight.Text = "Fit height";
			this.cbfitheight.UseVisualStyleBackColor = true;
			this.cbfitheight.CheckedChanged += new System.EventHandler(this.cbfitheight_CheckedChanged);
			// 
			// cbfitconnected
			// 
			this.cbfitconnected.AutoSize = true;
			this.cbfitconnected.Location = new System.Drawing.Point(10, 65);
			this.cbfitconnected.Name = "cbfitconnected";
			this.cbfitconnected.Size = new System.Drawing.Size(168, 17);
			this.cbfitconnected.TabIndex = 9;
			this.cbfitconnected.Text = "Fit across connected surfaces";
			this.cbfitconnected.UseVisualStyleBackColor = true;
			this.cbfitconnected.CheckedChanged += new System.EventHandler(this.repeat_ValueChanged);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.cbfitwidth);
			this.groupBox2.Controls.Add(this.cbfitconnected);
			this.groupBox2.Controls.Add(this.cbfitheight);
			this.groupBox2.Location = new System.Drawing.Point(12, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(183, 89);
			this.groupBox2.TabIndex = 10;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = " Options ";
			// 
			// FitTexturesForm
			// 
			this.AcceptButton = this.accept;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(206, 223);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.accept);
			this.Controls.Add(this.repeatgroup);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FitTexturesForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Fit Textures";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FitTexturesForm_FormClosing);
			this.repeatgroup.ResumeLayout(false);
			this.repeatgroup.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.horizrepeat)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.vertrepeat)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label labelhorizrepeat;
		private System.Windows.Forms.GroupBox repeatgroup;
		private System.Windows.Forms.Button resethoriz;
		private System.Windows.Forms.NumericUpDown vertrepeat;
		private System.Windows.Forms.NumericUpDown horizrepeat;
		private System.Windows.Forms.Label labelvertrepeat;
		private System.Windows.Forms.Button resetvert;
		private System.Windows.Forms.CheckBox cbfitwidth;
		private System.Windows.Forms.Button accept;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.CheckBox cbfitheight;
		private System.Windows.Forms.CheckBox cbfitconnected;
		private System.Windows.Forms.GroupBox groupBox2;
	}
}