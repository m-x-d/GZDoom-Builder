namespace CodeImp.DoomBuilder.Interface
{
	partial class LinedefInfoPanel
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
			System.Windows.Forms.Label label1;
			System.Windows.Forms.GroupBox groupBox1;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label2;
			this.backoffset = new System.Windows.Forms.Label();
			this.backoffsetlabel = new System.Windows.Forms.Label();
			this.frontoffset = new System.Windows.Forms.Label();
			this.frontoffsetlabel = new System.Windows.Forms.Label();
			this.tag = new System.Windows.Forms.Label();
			this.angle = new System.Windows.Forms.Label();
			this.length = new System.Windows.Forms.Label();
			this.action = new System.Windows.Forms.Label();
			this.frontpanel = new System.Windows.Forms.GroupBox();
			this.frontlowname = new System.Windows.Forms.Label();
			this.frontlowtex = new System.Windows.Forms.Panel();
			this.frontmidname = new System.Windows.Forms.Label();
			this.frontmidtex = new System.Windows.Forms.Panel();
			this.fronthighname = new System.Windows.Forms.Label();
			this.fronthightex = new System.Windows.Forms.Panel();
			this.backpanel = new System.Windows.Forms.GroupBox();
			this.backlowname = new System.Windows.Forms.Label();
			this.backlowtex = new System.Windows.Forms.Panel();
			this.backmidname = new System.Windows.Forms.Label();
			this.backmidtex = new System.Windows.Forms.Panel();
			this.backhighname = new System.Windows.Forms.Label();
			this.backhightex = new System.Windows.Forms.Panel();
			label1 = new System.Windows.Forms.Label();
			groupBox1 = new System.Windows.Forms.GroupBox();
			label4 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			groupBox1.SuspendLayout();
			this.frontpanel.SuspendLayout();
			this.backpanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(11, 19);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(41, 14);
			label1.TabIndex = 0;
			label1.Text = "Action:";
			// 
			// groupBox1
			// 
			groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			groupBox1.Controls.Add(this.backoffset);
			groupBox1.Controls.Add(this.backoffsetlabel);
			groupBox1.Controls.Add(this.frontoffset);
			groupBox1.Controls.Add(this.frontoffsetlabel);
			groupBox1.Controls.Add(this.tag);
			groupBox1.Controls.Add(this.angle);
			groupBox1.Controls.Add(this.length);
			groupBox1.Controls.Add(label4);
			groupBox1.Controls.Add(label3);
			groupBox1.Controls.Add(label2);
			groupBox1.Controls.Add(this.action);
			groupBox1.Controls.Add(label1);
			groupBox1.Location = new System.Drawing.Point(0, 0);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(258, 100);
			groupBox1.TabIndex = 1;
			groupBox1.TabStop = false;
			groupBox1.Text = " Linedef ";
			// 
			// backoffset
			// 
			this.backoffset.AutoSize = true;
			this.backoffset.Location = new System.Drawing.Point(180, 77);
			this.backoffset.Name = "backoffset";
			this.backoffset.Size = new System.Drawing.Size(49, 14);
			this.backoffset.TabIndex = 17;
			this.backoffset.Text = "100, 100";
			// 
			// backoffsetlabel
			// 
			this.backoffsetlabel.AutoSize = true;
			this.backoffsetlabel.Location = new System.Drawing.Point(111, 77);
			this.backoffsetlabel.Name = "backoffsetlabel";
			this.backoffsetlabel.Size = new System.Drawing.Size(66, 14);
			this.backoffsetlabel.TabIndex = 14;
			this.backoffsetlabel.Text = "Back offset:";
			// 
			// frontoffset
			// 
			this.frontoffset.AutoSize = true;
			this.frontoffset.Location = new System.Drawing.Point(180, 58);
			this.frontoffset.Name = "frontoffset";
			this.frontoffset.Size = new System.Drawing.Size(49, 14);
			this.frontoffset.TabIndex = 11;
			this.frontoffset.Text = "100, 100";
			// 
			// frontoffsetlabel
			// 
			this.frontoffsetlabel.AutoSize = true;
			this.frontoffsetlabel.Location = new System.Drawing.Point(110, 58);
			this.frontoffsetlabel.Name = "frontoffsetlabel";
			this.frontoffsetlabel.Size = new System.Drawing.Size(67, 14);
			this.frontoffsetlabel.TabIndex = 8;
			this.frontoffsetlabel.Text = "Front offset:";
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
			// angle
			// 
			this.angle.AutoSize = true;
			this.angle.Location = new System.Drawing.Point(55, 58);
			this.angle.Name = "angle";
			this.angle.Size = new System.Drawing.Size(25, 14);
			this.angle.TabIndex = 6;
			this.angle.Text = "360";
			// 
			// length
			// 
			this.length.AutoSize = true;
			this.length.Location = new System.Drawing.Point(55, 39);
			this.length.Name = "length";
			this.length.Size = new System.Drawing.Size(31, 14);
			this.length.TabIndex = 5;
			this.length.Text = "1024";
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
			label3.Location = new System.Drawing.Point(14, 58);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(38, 14);
			label3.TabIndex = 3;
			label3.Text = "Angle:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(9, 39);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(43, 14);
			label2.TabIndex = 2;
			label2.Text = "Length:";
			// 
			// action
			// 
			this.action.AutoSize = true;
			this.action.Location = new System.Drawing.Point(55, 19);
			this.action.Name = "action";
			this.action.Size = new System.Drawing.Size(190, 14);
			this.action.TabIndex = 1;
			this.action.Text = "0 - Big Door that goes Wobbly Wobbly";
			// 
			// frontpanel
			// 
			this.frontpanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.frontpanel.Controls.Add(this.frontlowname);
			this.frontpanel.Controls.Add(this.frontlowtex);
			this.frontpanel.Controls.Add(this.frontmidname);
			this.frontpanel.Controls.Add(this.frontmidtex);
			this.frontpanel.Controls.Add(this.fronthighname);
			this.frontpanel.Controls.Add(this.fronthightex);
			this.frontpanel.Location = new System.Drawing.Point(264, 0);
			this.frontpanel.Name = "frontpanel";
			this.frontpanel.Size = new System.Drawing.Size(241, 100);
			this.frontpanel.TabIndex = 2;
			this.frontpanel.TabStop = false;
			this.frontpanel.Text = " Front ";
			// 
			// frontlowname
			// 
			this.frontlowname.Location = new System.Drawing.Point(159, 80);
			this.frontlowname.Name = "frontlowname";
			this.frontlowname.Size = new System.Drawing.Size(72, 13);
			this.frontlowname.TabIndex = 5;
			this.frontlowname.Text = "BIGDOOR6";
			this.frontlowname.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// frontlowtex
			// 
			this.frontlowtex.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.frontlowtex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.frontlowtex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.frontlowtex.Location = new System.Drawing.Point(161, 19);
			this.frontlowtex.Name = "frontlowtex";
			this.frontlowtex.Size = new System.Drawing.Size(68, 60);
			this.frontlowtex.TabIndex = 4;
			// 
			// frontmidname
			// 
			this.frontmidname.Location = new System.Drawing.Point(85, 80);
			this.frontmidname.Name = "frontmidname";
			this.frontmidname.Size = new System.Drawing.Size(72, 13);
			this.frontmidname.TabIndex = 3;
			this.frontmidname.Text = "BIGDOOR6";
			this.frontmidname.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// frontmidtex
			// 
			this.frontmidtex.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.frontmidtex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.frontmidtex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.frontmidtex.Location = new System.Drawing.Point(87, 19);
			this.frontmidtex.Name = "frontmidtex";
			this.frontmidtex.Size = new System.Drawing.Size(68, 60);
			this.frontmidtex.TabIndex = 2;
			// 
			// fronthighname
			// 
			this.fronthighname.Location = new System.Drawing.Point(11, 80);
			this.fronthighname.Name = "fronthighname";
			this.fronthighname.Size = new System.Drawing.Size(72, 13);
			this.fronthighname.TabIndex = 1;
			this.fronthighname.Text = "BIGDOOR6";
			this.fronthighname.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// fronthightex
			// 
			this.fronthightex.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.fronthightex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.fronthightex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.fronthightex.Location = new System.Drawing.Point(13, 19);
			this.fronthightex.Name = "fronthightex";
			this.fronthightex.Size = new System.Drawing.Size(68, 60);
			this.fronthightex.TabIndex = 0;
			// 
			// backpanel
			// 
			this.backpanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.backpanel.Controls.Add(this.backlowname);
			this.backpanel.Controls.Add(this.backlowtex);
			this.backpanel.Controls.Add(this.backmidname);
			this.backpanel.Controls.Add(this.backmidtex);
			this.backpanel.Controls.Add(this.backhighname);
			this.backpanel.Controls.Add(this.backhightex);
			this.backpanel.Location = new System.Drawing.Point(511, 0);
			this.backpanel.Name = "backpanel";
			this.backpanel.Size = new System.Drawing.Size(241, 100);
			this.backpanel.TabIndex = 3;
			this.backpanel.TabStop = false;
			this.backpanel.Text = " Back ";
			// 
			// backlowname
			// 
			this.backlowname.Location = new System.Drawing.Point(159, 80);
			this.backlowname.Name = "backlowname";
			this.backlowname.Size = new System.Drawing.Size(72, 13);
			this.backlowname.TabIndex = 5;
			this.backlowname.Text = "BIGDOOR6";
			this.backlowname.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// backlowtex
			// 
			this.backlowtex.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.backlowtex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.backlowtex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.backlowtex.Location = new System.Drawing.Point(161, 19);
			this.backlowtex.Name = "backlowtex";
			this.backlowtex.Size = new System.Drawing.Size(68, 60);
			this.backlowtex.TabIndex = 4;
			// 
			// backmidname
			// 
			this.backmidname.Location = new System.Drawing.Point(85, 80);
			this.backmidname.Name = "backmidname";
			this.backmidname.Size = new System.Drawing.Size(72, 13);
			this.backmidname.TabIndex = 3;
			this.backmidname.Text = "BIGDOOR6";
			this.backmidname.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// backmidtex
			// 
			this.backmidtex.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.backmidtex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.backmidtex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.backmidtex.Location = new System.Drawing.Point(87, 19);
			this.backmidtex.Name = "backmidtex";
			this.backmidtex.Size = new System.Drawing.Size(68, 60);
			this.backmidtex.TabIndex = 2;
			// 
			// backhighname
			// 
			this.backhighname.Location = new System.Drawing.Point(11, 80);
			this.backhighname.Name = "backhighname";
			this.backhighname.Size = new System.Drawing.Size(72, 13);
			this.backhighname.TabIndex = 1;
			this.backhighname.Text = "BIGDOOR6";
			this.backhighname.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// backhightex
			// 
			this.backhightex.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.backhightex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.backhightex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.backhightex.Location = new System.Drawing.Point(13, 19);
			this.backhightex.Name = "backhightex";
			this.backhightex.Size = new System.Drawing.Size(68, 60);
			this.backhightex.TabIndex = 0;
			// 
			// LinedefInfoPanel
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.backpanel);
			this.Controls.Add(this.frontpanel);
			this.Controls.Add(groupBox1);
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximumSize = new System.Drawing.Size(10000, 100);
			this.MinimumSize = new System.Drawing.Size(100, 100);
			this.Name = "LinedefInfoPanel";
			this.Size = new System.Drawing.Size(770, 100);
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			this.frontpanel.ResumeLayout(false);
			this.backpanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label action;
		private System.Windows.Forms.Label tag;
		private System.Windows.Forms.Label angle;
		private System.Windows.Forms.Label length;
		private System.Windows.Forms.Label frontoffset;
		private System.Windows.Forms.Label backoffset;
		private System.Windows.Forms.Panel fronthightex;
		private System.Windows.Forms.Label frontlowname;
		private System.Windows.Forms.Panel frontlowtex;
		private System.Windows.Forms.Label frontmidname;
		private System.Windows.Forms.Panel frontmidtex;
		private System.Windows.Forms.Label fronthighname;
		private System.Windows.Forms.Label backlowname;
		private System.Windows.Forms.Panel backlowtex;
		private System.Windows.Forms.Label backmidname;
		private System.Windows.Forms.Panel backmidtex;
		private System.Windows.Forms.Label backhighname;
		private System.Windows.Forms.Panel backhightex;
		private System.Windows.Forms.GroupBox frontpanel;
		private System.Windows.Forms.GroupBox backpanel;
		private System.Windows.Forms.Label backoffsetlabel;
		private System.Windows.Forms.Label frontoffsetlabel;

	}
}
