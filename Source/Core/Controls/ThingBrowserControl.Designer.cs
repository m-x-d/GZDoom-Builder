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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ThingBrowserControl));
			this.sizecaption = new System.Windows.Forms.Label();
			this.blockingcaption = new System.Windows.Forms.Label();
			this.positioncaption = new System.Windows.Forms.Label();
			this.typecaption = new System.Windows.Forms.Label();
			this.sizelabel = new System.Windows.Forms.Label();
			this.blockinglabel = new System.Windows.Forms.Label();
			this.positionlabel = new System.Windows.Forms.Label();
			this.thingimages = new System.Windows.Forms.ImageList(this.components);
			this.infopanel = new System.Windows.Forms.Panel();
			this.typeid = new CodeImp.DoomBuilder.Controls.NumericTextbox();
			this.label1 = new System.Windows.Forms.Label();
			this.tbFilter = new System.Windows.Forms.TextBox();
			this.bClear = new System.Windows.Forms.Button();
			this.typelist = new CodeImp.DoomBuilder.GZBuilder.Controls.MultiSelectTreeview();
			this.infopanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// sizecaption
			// 
			this.sizecaption.AutoSize = true;
			this.sizecaption.Location = new System.Drawing.Point(166, 8);
			this.sizecaption.Name = "sizecaption";
			this.sizecaption.Size = new System.Drawing.Size(30, 13);
			this.sizecaption.TabIndex = 16;
			this.sizecaption.Text = "Size:";
			// 
			// blockingcaption
			// 
			this.blockingcaption.AutoSize = true;
			this.blockingcaption.Location = new System.Drawing.Point(145, 30);
			this.blockingcaption.Name = "blockingcaption";
			this.blockingcaption.Size = new System.Drawing.Size(51, 13);
			this.blockingcaption.TabIndex = 14;
			this.blockingcaption.Text = "Blocking:";
			// 
			// positioncaption
			// 
			this.positioncaption.AutoSize = true;
			this.positioncaption.Location = new System.Drawing.Point(-2, 30);
			this.positioncaption.Name = "positioncaption";
			this.positioncaption.Size = new System.Drawing.Size(47, 13);
			this.positioncaption.TabIndex = 12;
			this.positioncaption.Text = "Position:";
			// 
			// typecaption
			// 
			this.typecaption.AutoSize = true;
			this.typecaption.Location = new System.Drawing.Point(-2, 8);
			this.typecaption.Name = "typecaption";
			this.typecaption.Size = new System.Drawing.Size(34, 13);
			this.typecaption.TabIndex = 10;
			this.typecaption.Text = "Type:";
			// 
			// sizelabel
			// 
			this.sizelabel.AutoSize = true;
			this.sizelabel.Location = new System.Drawing.Point(200, 8);
			this.sizelabel.Name = "sizelabel";
			this.sizelabel.Size = new System.Drawing.Size(42, 13);
			this.sizelabel.TabIndex = 17;
			this.sizelabel.Text = "16 x 96";
			// 
			// blockinglabel
			// 
			this.blockinglabel.AutoSize = true;
			this.blockinglabel.Location = new System.Drawing.Point(198, 30);
			this.blockinglabel.Name = "blockinglabel";
			this.blockinglabel.Size = new System.Drawing.Size(63, 13);
			this.blockinglabel.TabIndex = 15;
			this.blockinglabel.Text = "True-Height";
			// 
			// positionlabel
			// 
			this.positionlabel.AutoSize = true;
			this.positionlabel.Location = new System.Drawing.Point(48, 30);
			this.positionlabel.Name = "positionlabel";
			this.positionlabel.Size = new System.Drawing.Size(38, 13);
			this.positionlabel.TabIndex = 13;
			this.positionlabel.Text = "Ceiling";
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
			// infopanel
			// 
			this.infopanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.infopanel.Controls.Add(this.sizelabel);
			this.infopanel.Controls.Add(this.typecaption);
			this.infopanel.Controls.Add(this.sizecaption);
			this.infopanel.Controls.Add(this.typeid);
			this.infopanel.Controls.Add(this.blockinglabel);
			this.infopanel.Controls.Add(this.positioncaption);
			this.infopanel.Controls.Add(this.blockingcaption);
			this.infopanel.Controls.Add(this.positionlabel);
			this.infopanel.Location = new System.Drawing.Point(0, 272);
			this.infopanel.Name = "infopanel";
			this.infopanel.Size = new System.Drawing.Size(304, 48);
			this.infopanel.TabIndex = 18;
			// 
			// typeid
			// 
			this.typeid.AllowDecimal = false;
			this.typeid.AllowNegative = false;
			this.typeid.AllowRelative = false;
			this.typeid.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.typeid.Location = new System.Drawing.Point(41, 5);
			this.typeid.Name = "typeid";
			this.typeid.Size = new System.Drawing.Size(68, 20);
			this.typeid.TabIndex = 1;
			this.typeid.TextChanged += new System.EventHandler(this.typeid_TextChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(32, 13);
			this.label1.TabIndex = 19;
			this.label1.Text = "Filter:";
			// 
			// tbFilter
			// 
			this.tbFilter.Location = new System.Drawing.Point(42, 3);
			this.tbFilter.Name = "tbFilter";
			this.tbFilter.Size = new System.Drawing.Size(155, 20);
			this.tbFilter.TabIndex = 20;
			this.tbFilter.TextChanged += new System.EventHandler(this.tbFilter_TextChanged);
			// 
			// bClear
			// 
			this.bClear.Image = global::CodeImp.DoomBuilder.Properties.Resources.SearchClear;
			this.bClear.Location = new System.Drawing.Point(201, 1);
			this.bClear.Name = "bClear";
			this.bClear.Size = new System.Drawing.Size(24, 23);
			this.bClear.TabIndex = 21;
			this.bClear.UseVisualStyleBackColor = true;
			this.bClear.Click += new System.EventHandler(this.bClear_Click);
			// 
			// typelist
			// 
			this.typelist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.typelist.HideSelection = false;
			this.typelist.ImageIndex = 0;
			this.typelist.ImageList = this.thingimages;
			this.typelist.Location = new System.Drawing.Point(0, 28);
			this.typelist.Margin = new System.Windows.Forms.Padding(8, 8, 9, 8);
			this.typelist.Name = "typelist";
			this.typelist.SelectedImageIndex = 0;
			this.typelist.SelectedNodes = ((System.Collections.Generic.List<System.Windows.Forms.TreeNode>)(resources.GetObject("typelist.SelectedNodes")));
			this.typelist.Size = new System.Drawing.Size(304, 244);
			this.typelist.TabIndex = 22;
			this.typelist.UseMultiSelection = false;
			this.typelist.DoubleClick += new System.EventHandler(this.typelist_DoubleClick);
			this.typelist.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.typelist_AfterSelect);
			this.typelist.MouseEnter += new System.EventHandler(this.typelist_MouseEnter);
			// 
			// ThingBrowserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.typelist);
			this.Controls.Add(this.bClear);
			this.Controls.Add(this.tbFilter);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.infopanel);
			this.Name = "ThingBrowserControl";
			this.Size = new System.Drawing.Size(304, 320);
			this.Layout += new System.Windows.Forms.LayoutEventHandler(this.ThingBrowserControl_Layout);
			this.Resize += new System.EventHandler(this.ThingBrowserControl_Resize);
			this.SizeChanged += new System.EventHandler(this.ThingBrowserControl_SizeChanged);
			this.infopanel.ResumeLayout(false);
			this.infopanel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label sizelabel;
		private System.Windows.Forms.Label blockinglabel;
		private System.Windows.Forms.Label positionlabel;
		private NumericTextbox typeid;
		private System.Windows.Forms.ImageList thingimages;
		private System.Windows.Forms.Panel infopanel;
		private System.Windows.Forms.Label sizecaption;
		private System.Windows.Forms.Label blockingcaption;
		private System.Windows.Forms.Label positioncaption;
		private System.Windows.Forms.Label typecaption;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbFilter;
		private System.Windows.Forms.Button bClear;
		private CodeImp.DoomBuilder.GZBuilder.Controls.MultiSelectTreeview typelist;
	}
}
