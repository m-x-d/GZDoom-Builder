namespace CodeImp.DoomBuilder.Interface
{
	partial class ThingInfoPanel
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
			System.Windows.Forms.GroupBox groupBox1;
			System.Windows.Forms.Label label5;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label1;
			this.ceilingpanel = new System.Windows.Forms.GroupBox();
			this.spritename = new System.Windows.Forms.Label();
			this.spritetex = new System.Windows.Forms.Panel();
			this.angle = new System.Windows.Forms.Label();
			this.tag = new System.Windows.Forms.Label();
			this.position = new System.Windows.Forms.Label();
			this.action = new System.Windows.Forms.Label();
			this.type = new System.Windows.Forms.Label();
			groupBox1 = new System.Windows.Forms.GroupBox();
			label5 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			this.ceilingpanel.SuspendLayout();
			groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// ceilingpanel
			// 
			this.ceilingpanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.ceilingpanel.Controls.Add(this.spritename);
			this.ceilingpanel.Controls.Add(this.spritetex);
			this.ceilingpanel.Location = new System.Drawing.Point(273, 0);
			this.ceilingpanel.Name = "ceilingpanel";
			this.ceilingpanel.Size = new System.Drawing.Size(93, 100);
			this.ceilingpanel.TabIndex = 5;
			this.ceilingpanel.TabStop = false;
			this.ceilingpanel.Text = " Sprite ";
			// 
			// spritename
			// 
			this.spritename.Location = new System.Drawing.Point(11, 80);
			this.spritename.Name = "spritename";
			this.spritename.Size = new System.Drawing.Size(72, 13);
			this.spritename.TabIndex = 1;
			this.spritename.Text = "BIGDOOR6";
			this.spritename.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// spritetex
			// 
			this.spritetex.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.spritetex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.spritetex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.spritetex.Location = new System.Drawing.Point(13, 19);
			this.spritetex.Name = "spritetex";
			this.spritetex.Size = new System.Drawing.Size(68, 60);
			this.spritetex.TabIndex = 0;
			// 
			// groupBox1
			// 
			groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			groupBox1.Controls.Add(this.angle);
			groupBox1.Controls.Add(label5);
			groupBox1.Controls.Add(this.tag);
			groupBox1.Controls.Add(this.position);
			groupBox1.Controls.Add(this.action);
			groupBox1.Controls.Add(label4);
			groupBox1.Controls.Add(label3);
			groupBox1.Controls.Add(label2);
			groupBox1.Controls.Add(this.type);
			groupBox1.Controls.Add(label1);
			groupBox1.Location = new System.Drawing.Point(0, 0);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(267, 100);
			groupBox1.TabIndex = 4;
			groupBox1.TabStop = false;
			groupBox1.Text = " Thing ";
			// 
			// angle
			// 
			this.angle.AutoSize = true;
			this.angle.Location = new System.Drawing.Point(206, 77);
			this.angle.Name = "angle";
			this.angle.Size = new System.Drawing.Size(13, 14);
			this.angle.TabIndex = 11;
			this.angle.Text = "0";
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(165, 77);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(38, 14);
			label5.TabIndex = 8;
			label5.Text = "Angle:";
			// 
			// tag
			// 
			this.tag.AutoSize = true;
			this.tag.Location = new System.Drawing.Point(61, 77);
			this.tag.Name = "tag";
			this.tag.Size = new System.Drawing.Size(13, 14);
			this.tag.TabIndex = 7;
			this.tag.Text = "0";
			// 
			// position
			// 
			this.position.AutoSize = true;
			this.position.Location = new System.Drawing.Point(61, 58);
			this.position.Name = "position";
			this.position.Size = new System.Drawing.Size(91, 14);
			this.position.TabIndex = 6;
			this.position.Text = "1024, 1024, 1024";
			// 
			// action
			// 
			this.action.AutoSize = true;
			this.action.Location = new System.Drawing.Point(61, 39);
			this.action.Name = "action";
			this.action.Size = new System.Drawing.Size(180, 14);
			this.action.TabIndex = 5;
			this.action.Text = "0 - Spawn a Blue Poopie and Ammo";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(30, 77);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(28, 14);
			label4.TabIndex = 4;
			label4.Text = "Tag:";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(11, 58);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(47, 14);
			label3.TabIndex = 3;
			label3.Text = "Position:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(17, 39);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(41, 14);
			label2.TabIndex = 2;
			label2.Text = "Action:";
			// 
			// type
			// 
			this.type.AutoSize = true;
			this.type.Location = new System.Drawing.Point(61, 19);
			this.type.Name = "type";
			this.type.Size = new System.Drawing.Size(99, 14);
			this.type.TabIndex = 1;
			this.type.Text = "0 - Big Brown Pimp";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(24, 19);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(34, 14);
			label1.TabIndex = 0;
			label1.Text = "Type:";
			// 
			// ThingInfoPanel
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.ceilingpanel);
			this.Controls.Add(groupBox1);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximumSize = new System.Drawing.Size(10000, 100);
			this.MinimumSize = new System.Drawing.Size(100, 100);
			this.Name = "ThingInfoPanel";
			this.Size = new System.Drawing.Size(388, 100);
			this.ceilingpanel.ResumeLayout(false);
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox ceilingpanel;
		private System.Windows.Forms.Label spritename;
		private System.Windows.Forms.Panel spritetex;
		private System.Windows.Forms.Label angle;
		private System.Windows.Forms.Label tag;
		private System.Windows.Forms.Label position;
		private System.Windows.Forms.Label action;
		private System.Windows.Forms.Label type;

	}
}
