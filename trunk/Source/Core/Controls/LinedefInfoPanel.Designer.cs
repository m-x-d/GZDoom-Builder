namespace CodeImp.DoomBuilder.Controls
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
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label2;
			this.infopanel = new System.Windows.Forms.GroupBox();
			this.unpegged = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.arg5 = new System.Windows.Forms.Label();
			this.arglbl5 = new System.Windows.Forms.Label();
			this.arglbl4 = new System.Windows.Forms.Label();
			this.arg4 = new System.Windows.Forms.Label();
			this.arglbl3 = new System.Windows.Forms.Label();
			this.arglbl2 = new System.Windows.Forms.Label();
			this.arg3 = new System.Windows.Forms.Label();
			this.arglbl1 = new System.Windows.Forms.Label();
			this.arg2 = new System.Windows.Forms.Label();
			this.backoffset = new System.Windows.Forms.Label();
			this.arg1 = new System.Windows.Forms.Label();
			this.backoffsetlabel = new System.Windows.Forms.Label();
			this.frontoffset = new System.Windows.Forms.Label();
			this.frontoffsetlabel = new System.Windows.Forms.Label();
			this.tag = new System.Windows.Forms.Label();
			this.angle = new System.Windows.Forms.Label();
			this.length = new System.Windows.Forms.Label();
			this.action = new System.Windows.Forms.Label();
			this.frontpanel = new System.Windows.Forms.GroupBox();
			this.frontsector = new System.Windows.Forms.Label();
			this.frontlowname = new System.Windows.Forms.Label();
			this.frontlowtex = new System.Windows.Forms.Panel();
			this.frontmidname = new System.Windows.Forms.Label();
			this.frontmidtex = new System.Windows.Forms.Panel();
			this.fronthighname = new System.Windows.Forms.Label();
			this.fronthightex = new System.Windows.Forms.Panel();
			this.backpanel = new System.Windows.Forms.GroupBox();
			this.backsector = new System.Windows.Forms.Label();
			this.backlowname = new System.Windows.Forms.Label();
			this.backlowtex = new System.Windows.Forms.Panel();
			this.backmidname = new System.Windows.Forms.Label();
			this.backmidtex = new System.Windows.Forms.Panel();
			this.backhighname = new System.Windows.Forms.Label();
			this.backhightex = new System.Windows.Forms.Panel();
			label1 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			this.infopanel.SuspendLayout();
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
			// infopanel
			// 
			this.infopanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.infopanel.Controls.Add(this.unpegged);
			this.infopanel.Controls.Add(this.label6);
			this.infopanel.Controls.Add(this.arg5);
			this.infopanel.Controls.Add(this.arglbl5);
			this.infopanel.Controls.Add(this.arglbl4);
			this.infopanel.Controls.Add(this.arg4);
			this.infopanel.Controls.Add(this.arglbl3);
			this.infopanel.Controls.Add(this.arglbl2);
			this.infopanel.Controls.Add(this.arg3);
			this.infopanel.Controls.Add(this.arglbl1);
			this.infopanel.Controls.Add(this.arg2);
			this.infopanel.Controls.Add(this.backoffset);
			this.infopanel.Controls.Add(this.arg1);
			this.infopanel.Controls.Add(this.backoffsetlabel);
			this.infopanel.Controls.Add(this.frontoffset);
			this.infopanel.Controls.Add(this.frontoffsetlabel);
			this.infopanel.Controls.Add(this.tag);
			this.infopanel.Controls.Add(this.angle);
			this.infopanel.Controls.Add(this.length);
			this.infopanel.Controls.Add(label4);
			this.infopanel.Controls.Add(label3);
			this.infopanel.Controls.Add(label2);
			this.infopanel.Controls.Add(this.action);
			this.infopanel.Controls.Add(label1);
			this.infopanel.Location = new System.Drawing.Point(0, 0);
			this.infopanel.Name = "infopanel";
			this.infopanel.Size = new System.Drawing.Size(461, 100);
			this.infopanel.TabIndex = 1;
			this.infopanel.TabStop = false;
			this.infopanel.Text = " Linedef ";
			// 
			// unpegged
			// 
			this.unpegged.AutoSize = true;
			this.unpegged.Location = new System.Drawing.Point(180, 39);
			this.unpegged.Name = "unpegged";
			this.unpegged.Size = new System.Drawing.Size(32, 14);
			this.unpegged.TabIndex = 29;
			this.unpegged.Text = "None";
			this.unpegged.UseMnemonic = false;
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(118, 39);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(59, 14);
			this.label6.TabIndex = 28;
			this.label6.Text = "Unpegged:";
			// 
			// arg5
			// 
			this.arg5.AutoEllipsis = true;
			this.arg5.Location = new System.Drawing.Point(373, 79);
			this.arg5.Name = "arg5";
			this.arg5.Size = new System.Drawing.Size(83, 14);
			this.arg5.TabIndex = 27;
			this.arg5.Text = "Arg 1:";
			this.arg5.UseMnemonic = false;
			// 
			// arglbl5
			// 
			this.arglbl5.AutoEllipsis = true;
			this.arglbl5.BackColor = System.Drawing.Color.Transparent;
			this.arglbl5.Location = new System.Drawing.Point(246, 79);
			this.arglbl5.Name = "arglbl5";
			this.arglbl5.Size = new System.Drawing.Size(121, 14);
			this.arglbl5.TabIndex = 22;
			this.arglbl5.Text = "Arg 1:";
			this.arglbl5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arglbl5.UseMnemonic = false;
			// 
			// arglbl4
			// 
			this.arglbl4.AutoEllipsis = true;
			this.arglbl4.BackColor = System.Drawing.Color.Transparent;
			this.arglbl4.Location = new System.Drawing.Point(246, 64);
			this.arglbl4.Name = "arglbl4";
			this.arglbl4.Size = new System.Drawing.Size(121, 14);
			this.arglbl4.TabIndex = 21;
			this.arglbl4.Text = "Arg 1:";
			this.arglbl4.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arglbl4.UseMnemonic = false;
			// 
			// arg4
			// 
			this.arg4.AutoEllipsis = true;
			this.arg4.Location = new System.Drawing.Point(373, 64);
			this.arg4.Name = "arg4";
			this.arg4.Size = new System.Drawing.Size(83, 14);
			this.arg4.TabIndex = 26;
			this.arg4.Text = "Arg 1:";
			this.arg4.UseMnemonic = false;
			// 
			// arglbl3
			// 
			this.arglbl3.AutoEllipsis = true;
			this.arglbl3.BackColor = System.Drawing.Color.Transparent;
			this.arglbl3.Location = new System.Drawing.Point(246, 49);
			this.arglbl3.Name = "arglbl3";
			this.arglbl3.Size = new System.Drawing.Size(121, 14);
			this.arglbl3.TabIndex = 20;
			this.arglbl3.Text = "Arg 1:";
			this.arglbl3.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arglbl3.UseMnemonic = false;
			// 
			// arglbl2
			// 
			this.arglbl2.AutoEllipsis = true;
			this.arglbl2.BackColor = System.Drawing.Color.Transparent;
			this.arglbl2.Location = new System.Drawing.Point(246, 34);
			this.arglbl2.Name = "arglbl2";
			this.arglbl2.Size = new System.Drawing.Size(121, 14);
			this.arglbl2.TabIndex = 19;
			this.arglbl2.Text = "Arg 1:";
			this.arglbl2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arglbl2.UseMnemonic = false;
			// 
			// arg3
			// 
			this.arg3.AutoEllipsis = true;
			this.arg3.Location = new System.Drawing.Point(373, 49);
			this.arg3.Name = "arg3";
			this.arg3.Size = new System.Drawing.Size(83, 14);
			this.arg3.TabIndex = 25;
			this.arg3.Text = "Arg 1:";
			this.arg3.UseMnemonic = false;
			// 
			// arglbl1
			// 
			this.arglbl1.AutoEllipsis = true;
			this.arglbl1.BackColor = System.Drawing.Color.Transparent;
			this.arglbl1.Location = new System.Drawing.Point(246, 19);
			this.arglbl1.Name = "arglbl1";
			this.arglbl1.Size = new System.Drawing.Size(121, 14);
			this.arglbl1.TabIndex = 18;
			this.arglbl1.Text = "Arg 1:";
			this.arglbl1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arglbl1.UseMnemonic = false;
			// 
			// arg2
			// 
			this.arg2.AutoEllipsis = true;
			this.arg2.Location = new System.Drawing.Point(373, 34);
			this.arg2.Name = "arg2";
			this.arg2.Size = new System.Drawing.Size(83, 14);
			this.arg2.TabIndex = 24;
			this.arg2.Text = "Arg 1:";
			this.arg2.UseMnemonic = false;
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
			// arg1
			// 
			this.arg1.AutoEllipsis = true;
			this.arg1.Location = new System.Drawing.Point(373, 19);
			this.arg1.Name = "arg1";
			this.arg1.Size = new System.Drawing.Size(83, 14);
			this.arg1.TabIndex = 23;
			this.arg1.Text = "Arg 1:";
			this.arg1.UseMnemonic = false;
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
			// action
			// 
			this.action.AutoEllipsis = true;
			this.action.BackColor = System.Drawing.Color.Transparent;
			this.action.Location = new System.Drawing.Point(55, 19);
			this.action.Name = "action";
			this.action.Size = new System.Drawing.Size(205, 14);
			this.action.TabIndex = 1;
			this.action.Text = "0 - Big Door that goes Wobbly Wobbly";
			this.action.UseMnemonic = false;
			// 
			// frontpanel
			// 
			this.frontpanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.frontpanel.Controls.Add(this.frontsector);
			this.frontpanel.Controls.Add(this.frontlowname);
			this.frontpanel.Controls.Add(this.frontlowtex);
			this.frontpanel.Controls.Add(this.frontmidtex);
			this.frontpanel.Controls.Add(this.fronthighname);
			this.frontpanel.Controls.Add(this.fronthightex);
			this.frontpanel.Controls.Add(this.frontmidname);
			this.frontpanel.Location = new System.Drawing.Point(467, 0);
			this.frontpanel.Name = "frontpanel";
			this.frontpanel.Size = new System.Drawing.Size(257, 100);
			this.frontpanel.TabIndex = 2;
			this.frontpanel.TabStop = false;
			this.frontpanel.Text = " Front ";
			// 
			// frontsector
			// 
			this.frontsector.AutoSize = true;
			this.frontsector.BackColor = System.Drawing.SystemColors.Control;
			this.frontsector.Location = new System.Drawing.Point(186, 0);
			this.frontsector.Name = "frontsector";
			this.frontsector.Size = new System.Drawing.Size(60, 14);
			this.frontsector.TabIndex = 6;
			this.frontsector.Text = "Sector 666";
			// 
			// frontlowname
			// 
			this.frontlowname.BackColor = System.Drawing.SystemColors.Control;
			this.frontlowname.Location = new System.Drawing.Point(170, 80);
			this.frontlowname.Name = "frontlowname";
			this.frontlowname.Size = new System.Drawing.Size(82, 13);
			this.frontlowname.TabIndex = 5;
			this.frontlowname.Text = "BROWNHUG";
			this.frontlowname.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.frontlowname.UseMnemonic = false;
			// 
			// frontlowtex
			// 
			this.frontlowtex.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.frontlowtex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.frontlowtex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.frontlowtex.Location = new System.Drawing.Point(177, 19);
			this.frontlowtex.Name = "frontlowtex";
			this.frontlowtex.Size = new System.Drawing.Size(68, 60);
			this.frontlowtex.TabIndex = 4;
			// 
			// frontmidname
			// 
			this.frontmidname.BackColor = System.Drawing.SystemColors.Control;
			this.frontmidname.Location = new System.Drawing.Point(88, 80);
			this.frontmidname.Name = "frontmidname";
			this.frontmidname.Size = new System.Drawing.Size(82, 13);
			this.frontmidname.TabIndex = 3;
			this.frontmidname.Text = "BROWNHUG";
			this.frontmidname.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.frontmidname.UseMnemonic = false;
			// 
			// frontmidtex
			// 
			this.frontmidtex.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.frontmidtex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.frontmidtex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.frontmidtex.Location = new System.Drawing.Point(95, 19);
			this.frontmidtex.Name = "frontmidtex";
			this.frontmidtex.Size = new System.Drawing.Size(68, 60);
			this.frontmidtex.TabIndex = 2;
			// 
			// fronthighname
			// 
			this.fronthighname.BackColor = System.Drawing.SystemColors.Control;
			this.fronthighname.Location = new System.Drawing.Point(6, 80);
			this.fronthighname.Name = "fronthighname";
			this.fronthighname.Size = new System.Drawing.Size(82, 13);
			this.fronthighname.TabIndex = 1;
			this.fronthighname.Text = "BROWNHUG";
			this.fronthighname.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.fronthighname.UseMnemonic = false;
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
			this.backpanel.Controls.Add(this.backsector);
			this.backpanel.Controls.Add(this.backlowname);
			this.backpanel.Controls.Add(this.backlowtex);
			this.backpanel.Controls.Add(this.backmidname);
			this.backpanel.Controls.Add(this.backmidtex);
			this.backpanel.Controls.Add(this.backhighname);
			this.backpanel.Controls.Add(this.backhightex);
			this.backpanel.Location = new System.Drawing.Point(730, 0);
			this.backpanel.Name = "backpanel";
			this.backpanel.Size = new System.Drawing.Size(257, 100);
			this.backpanel.TabIndex = 3;
			this.backpanel.TabStop = false;
			this.backpanel.Text = " Back ";
			// 
			// backsector
			// 
			this.backsector.AutoSize = true;
			this.backsector.BackColor = System.Drawing.SystemColors.Control;
			this.backsector.Location = new System.Drawing.Point(186, 0);
			this.backsector.Name = "backsector";
			this.backsector.Size = new System.Drawing.Size(60, 14);
			this.backsector.TabIndex = 7;
			this.backsector.Text = "Sector 666";
			// 
			// backlowname
			// 
			this.backlowname.BackColor = System.Drawing.SystemColors.Control;
			this.backlowname.Location = new System.Drawing.Point(170, 80);
			this.backlowname.Name = "backlowname";
			this.backlowname.Size = new System.Drawing.Size(82, 13);
			this.backlowname.TabIndex = 5;
			this.backlowname.Text = "BROWNHUG";
			this.backlowname.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.backlowname.UseMnemonic = false;
			// 
			// backlowtex
			// 
			this.backlowtex.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.backlowtex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.backlowtex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.backlowtex.Location = new System.Drawing.Point(177, 19);
			this.backlowtex.Name = "backlowtex";
			this.backlowtex.Size = new System.Drawing.Size(68, 60);
			this.backlowtex.TabIndex = 4;
			// 
			// backmidname
			// 
			this.backmidname.BackColor = System.Drawing.SystemColors.Control;
			this.backmidname.Location = new System.Drawing.Point(88, 80);
			this.backmidname.Name = "backmidname";
			this.backmidname.Size = new System.Drawing.Size(82, 13);
			this.backmidname.TabIndex = 3;
			this.backmidname.Text = "BROWNHUG";
			this.backmidname.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.backmidname.UseMnemonic = false;
			// 
			// backmidtex
			// 
			this.backmidtex.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.backmidtex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.backmidtex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.backmidtex.Location = new System.Drawing.Point(95, 19);
			this.backmidtex.Name = "backmidtex";
			this.backmidtex.Size = new System.Drawing.Size(68, 60);
			this.backmidtex.TabIndex = 2;
			// 
			// backhighname
			// 
			this.backhighname.BackColor = System.Drawing.SystemColors.Control;
			this.backhighname.Location = new System.Drawing.Point(6, 80);
			this.backhighname.Name = "backhighname";
			this.backhighname.Size = new System.Drawing.Size(82, 13);
			this.backhighname.TabIndex = 1;
			this.backhighname.Text = "BROWNHUG";
			this.backhighname.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			this.backhighname.UseMnemonic = false;
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
			this.Controls.Add(this.infopanel);
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximumSize = new System.Drawing.Size(10000, 100);
			this.MinimumSize = new System.Drawing.Size(100, 100);
			this.Name = "LinedefInfoPanel";
			this.Size = new System.Drawing.Size(1047, 100);
			this.infopanel.ResumeLayout(false);
			this.infopanel.PerformLayout();
			this.frontpanel.ResumeLayout(false);
			this.frontpanel.PerformLayout();
			this.backpanel.ResumeLayout(false);
			this.backpanel.PerformLayout();
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
		private System.Windows.Forms.Label arglbl5;
		private System.Windows.Forms.Label arglbl4;
		private System.Windows.Forms.Label arglbl3;
		private System.Windows.Forms.Label arglbl2;
		private System.Windows.Forms.Label arglbl1;
		private System.Windows.Forms.Label arg5;
		private System.Windows.Forms.Label arg4;
		private System.Windows.Forms.Label arg3;
		private System.Windows.Forms.Label arg2;
		private System.Windows.Forms.Label arg1;
		private System.Windows.Forms.GroupBox infopanel;
		private System.Windows.Forms.Label unpegged;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label frontsector;
		private System.Windows.Forms.Label backsector;

	}
}
