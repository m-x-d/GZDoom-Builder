namespace CodeImp.DoomBuilder.Controls
{
	partial class ThingBrowserControl
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
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label1;
			System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Monsters");
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ThingBrowserControl));
			this.sizelabel = new System.Windows.Forms.Label();
			this.blockinglabel = new System.Windows.Forms.Label();
			this.positionlabel = new System.Windows.Forms.Label();
			this.typeid = new CodeImp.DoomBuilder.Controls.NumericTextbox();
			this.typelist = new System.Windows.Forms.TreeView();
			this.thingimages = new System.Windows.Forms.ImageList(this.components);
			label4 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label4
			// 
			label4.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(169, 275);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(30, 13);
			label4.TabIndex = 16;
			label4.Text = "Size:";
			// 
			// label3
			// 
			label3.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(148, 304);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(51, 13);
			label3.TabIndex = 14;
			label3.Text = "Blocking:";
			// 
			// label2
			// 
			label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(3, 304);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(47, 13);
			label2.TabIndex = 12;
			label2.Text = "Position:";
			// 
			// label1
			// 
			label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(3, 275);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(34, 13);
			label1.TabIndex = 10;
			label1.Text = "Type:";
			// 
			// sizelabel
			// 
			this.sizelabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.sizelabel.AutoSize = true;
			this.sizelabel.Location = new System.Drawing.Point(203, 275);
			this.sizelabel.Name = "sizelabel";
			this.sizelabel.Size = new System.Drawing.Size(42, 13);
			this.sizelabel.TabIndex = 17;
			this.sizelabel.Text = "16 x 96";
			// 
			// blockinglabel
			// 
			this.blockinglabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.blockinglabel.AutoSize = true;
			this.blockinglabel.Location = new System.Drawing.Point(201, 304);
			this.blockinglabel.Name = "blockinglabel";
			this.blockinglabel.Size = new System.Drawing.Size(63, 13);
			this.blockinglabel.TabIndex = 15;
			this.blockinglabel.Text = "True-Height";
			// 
			// positionlabel
			// 
			this.positionlabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.positionlabel.AutoSize = true;
			this.positionlabel.Location = new System.Drawing.Point(53, 304);
			this.positionlabel.Name = "positionlabel";
			this.positionlabel.Size = new System.Drawing.Size(38, 13);
			this.positionlabel.TabIndex = 13;
			this.positionlabel.Text = "Ceiling";
			// 
			// typeid
			// 
			this.typeid.AllowDecimal = false;
			this.typeid.AllowNegative = false;
			this.typeid.AllowRelative = false;
			this.typeid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.typeid.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.typeid.Location = new System.Drawing.Point(46, 272);
			this.typeid.Name = "typeid";
			this.typeid.Size = new System.Drawing.Size(68, 20);
			this.typeid.TabIndex = 1;
			this.typeid.TextChanged += new System.EventHandler(this.typeid_TextChanged);
			// 
			// typelist
			// 
			this.typelist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.typelist.HideSelection = false;
			this.typelist.ImageIndex = 0;
			this.typelist.ImageList = this.thingimages;
			this.typelist.Location = new System.Drawing.Point(0, 0);
			this.typelist.Margin = new System.Windows.Forms.Padding(8, 8, 9, 8);
			this.typelist.Name = "typelist";
			treeNode1.Name = "Node0";
			treeNode1.Text = "Monsters";
			this.typelist.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
			this.typelist.SelectedImageIndex = 0;
			this.typelist.Size = new System.Drawing.Size(304, 261);
			this.typelist.TabIndex = 0;
			this.typelist.DoubleClick += new System.EventHandler(this.typelist_DoubleClick);
			this.typelist.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.typelist_AfterSelect);
			// 
			// thingimages
			// 
			this.thingimages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("thingimages.ImageStream")));
			this.thingimages.TransparentColor = System.Drawing.SystemColors.Window;
			this.thingimages.Images.SetKeyName(0, "thing00.png");
			this.thingimages.Images.SetKeyName(1, "thing01.png");
			this.thingimages.Images.SetKeyName(2, "thing02.png");
			this.thingimages.Images.SetKeyName(3, "thing03.png");
			this.thingimages.Images.SetKeyName(4, "thing04.png");
			this.thingimages.Images.SetKeyName(5, "thing05.png");
			this.thingimages.Images.SetKeyName(6, "thing06.png");
			this.thingimages.Images.SetKeyName(7, "thing07.png");
			this.thingimages.Images.SetKeyName(8, "thing08.png");
			this.thingimages.Images.SetKeyName(9, "thing09.png");
			this.thingimages.Images.SetKeyName(10, "thing10.png");
			this.thingimages.Images.SetKeyName(11, "thing11.png");
			this.thingimages.Images.SetKeyName(12, "thing12.png");
			this.thingimages.Images.SetKeyName(13, "thing13.png");
			this.thingimages.Images.SetKeyName(14, "thing14.png");
			this.thingimages.Images.SetKeyName(15, "thing15.png");
			this.thingimages.Images.SetKeyName(16, "thing16.png");
			this.thingimages.Images.SetKeyName(17, "thing17.png");
			this.thingimages.Images.SetKeyName(18, "thing18.png");
			this.thingimages.Images.SetKeyName(19, "thing19.png");
			// 
			// ThingBrowserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.sizelabel);
			this.Controls.Add(label4);
			this.Controls.Add(this.blockinglabel);
			this.Controls.Add(label3);
			this.Controls.Add(this.positionlabel);
			this.Controls.Add(label2);
			this.Controls.Add(this.typeid);
			this.Controls.Add(label1);
			this.Controls.Add(this.typelist);
			this.Name = "ThingBrowserControl";
			this.Size = new System.Drawing.Size(304, 320);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label sizelabel;
		private System.Windows.Forms.Label blockinglabel;
		private System.Windows.Forms.Label positionlabel;
		private NumericTextbox typeid;
		private System.Windows.Forms.TreeView typelist;
		private System.Windows.Forms.ImageList thingimages;
	}
}
