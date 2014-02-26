namespace CodeImp.DoomBuilder.BuilderModes
{
	partial class MenusForm
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
			this.menustrip = new System.Windows.Forms.MenuStrip();
			this.linedefsmenu = new System.Windows.Forms.ToolStripMenuItem();
			this.placethingsl = new System.Windows.Forms.ToolStripMenuItem();
			this.selectInSectorsItem3 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.selectsinglesideditem = new System.Windows.Forms.ToolStripMenuItem();
			this.selectdoublesideditem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
			this.fliplinedefsitem = new System.Windows.Forms.ToolStripMenuItem();
			this.flipsidedefsitem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.curvelinedefsitem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
			this.splitlinedefsitem = new System.Windows.Forms.ToolStripMenuItem();
			this.alignLinedefsItem = new System.Windows.Forms.ToolStripMenuItem();
			this.alignFloorToFrontItem = new System.Windows.Forms.ToolStripMenuItem();
			this.alignFloorToBackItem = new System.Windows.Forms.ToolStripMenuItem();
			this.alignCeilingToFrontItem = new System.Windows.Forms.ToolStripMenuItem();
			this.alignCeilingToBackItem = new System.Windows.Forms.ToolStripMenuItem();
			this.sectorsmenu = new System.Windows.Forms.ToolStripMenuItem();
			this.placethingss = new System.Windows.Forms.ToolStripMenuItem();
			this.selectInSectorsItem2 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.joinsectorsitem = new System.Windows.Forms.ToolStripMenuItem();
			this.mergesectorsitem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			this.thingsmenu = new System.Windows.Forms.ToolStripMenuItem();
			this.selectInSectorsItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.alignToWallItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pointAtCursorItem = new System.Windows.Forms.ToolStripMenuItem();
			this.vertsmenu = new System.Windows.Forms.ToolStripMenuItem();
			this.placethingsv = new System.Windows.Forms.ToolStripMenuItem();
			this.globalstrip = new System.Windows.Forms.ToolStrip();
			this.manualstrip = new System.Windows.Forms.ToolStrip();
			this.buttoncopyproperties = new System.Windows.Forms.ToolStripButton();
			this.buttonpasteproperties = new System.Windows.Forms.ToolStripButton();
			this.seperatorcopypaste = new System.Windows.Forms.ToolStripSeparator();
			this.buttonselectionnumbers = new System.Windows.Forms.ToolStripButton();
			this.buttonselectioneffects = new System.Windows.Forms.ToolStripButton();
			this.separatorsectors1 = new System.Windows.Forms.ToolStripSeparator();
			this.buttonbrightnessgradient = new System.Windows.Forms.ToolStripButton();
			this.buttonfloorgradient = new System.Windows.Forms.ToolStripButton();
			this.buttonceilinggradient = new System.Windows.Forms.ToolStripButton();
			this.buttonflipselectionh = new System.Windows.Forms.ToolStripButton();
			this.buttonflipselectionv = new System.Windows.Forms.ToolStripButton();
			this.buttoncurvelinedefs = new System.Windows.Forms.ToolStripButton();
			this.brightnessGradientMode = new System.Windows.Forms.ToolStripComboBox();
			this.buttonMarqueSelectTouching = new System.Windows.Forms.ToolStripButton();
			this.buttonAlignThingsToWall = new System.Windows.Forms.ToolStripButton();
			this.buttonTextureOffsetLock = new System.Windows.Forms.ToolStripButton();
			this.buttonMakeDoor = new System.Windows.Forms.ToolStripButton();
			this.menustrip.SuspendLayout();
			this.manualstrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// menustrip
			// 
			this.menustrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.linedefsmenu,
            this.sectorsmenu,
            this.thingsmenu,
            this.vertsmenu});
			this.menustrip.Location = new System.Drawing.Point(0, 0);
			this.menustrip.Name = "menustrip";
			this.menustrip.Size = new System.Drawing.Size(588, 24);
			this.menustrip.TabIndex = 0;
			this.menustrip.Text = "menustrip";
			// 
			// linedefsmenu
			// 
			this.linedefsmenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.placethingsl,
            this.selectInSectorsItem3,
            this.toolStripSeparator2,
            this.selectsinglesideditem,
            this.selectdoublesideditem,
            this.toolStripMenuItem4,
            this.fliplinedefsitem,
            this.flipsidedefsitem,
            this.toolStripMenuItem1,
            this.curvelinedefsitem,
            this.toolStripMenuItem3,
            this.splitlinedefsitem,
            this.alignLinedefsItem});
			this.linedefsmenu.Name = "linedefsmenu";
			this.linedefsmenu.Size = new System.Drawing.Size(63, 20);
			this.linedefsmenu.Text = "&Linedefs";
			this.linedefsmenu.Visible = false;
			this.linedefsmenu.DropDownOpening += new System.EventHandler(this.linedefsmenu_DropDownOpening);
			// 
			// placethingsl
			// 
			this.placethingsl.Name = "placethingsl";
			this.placethingsl.Size = new System.Drawing.Size(245, 22);
			this.placethingsl.Tag = "placethings";
			this.placethingsl.Text = "&Place Things...";
			this.placethingsl.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// selectInSectorsItem3
			// 
			this.selectInSectorsItem3.Name = "selectInSectorsItem3";
			this.selectInSectorsItem3.Size = new System.Drawing.Size(245, 22);
			this.selectInSectorsItem3.Tag = "thingsselectinsectors";
			this.selectInSectorsItem3.Text = "&Select Things in Selected Sectors";
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(242, 6);
			// 
			// selectsinglesideditem
			// 
			this.selectsinglesideditem.Name = "selectsinglesideditem";
			this.selectsinglesideditem.Size = new System.Drawing.Size(245, 22);
			this.selectsinglesideditem.Tag = "selectsinglesided";
			this.selectsinglesideditem.Text = "Select &Single-sided only";
			this.selectsinglesideditem.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// selectdoublesideditem
			// 
			this.selectdoublesideditem.Name = "selectdoublesideditem";
			this.selectdoublesideditem.Size = new System.Drawing.Size(245, 22);
			this.selectdoublesideditem.Tag = "selectdoublesided";
			this.selectdoublesideditem.Text = "Select &Double-sided only";
			this.selectdoublesideditem.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// toolStripMenuItem4
			// 
			this.toolStripMenuItem4.Name = "toolStripMenuItem4";
			this.toolStripMenuItem4.Size = new System.Drawing.Size(242, 6);
			// 
			// fliplinedefsitem
			// 
			this.fliplinedefsitem.Name = "fliplinedefsitem";
			this.fliplinedefsitem.Size = new System.Drawing.Size(245, 22);
			this.fliplinedefsitem.Tag = "fliplinedefs";
			this.fliplinedefsitem.Text = "&Flip Linedefs";
			this.fliplinedefsitem.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// flipsidedefsitem
			// 
			this.flipsidedefsitem.Name = "flipsidedefsitem";
			this.flipsidedefsitem.Size = new System.Drawing.Size(245, 22);
			this.flipsidedefsitem.Tag = "flipsidedefs";
			this.flipsidedefsitem.Text = "F&lip Sidedefs";
			this.flipsidedefsitem.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(242, 6);
			// 
			// curvelinedefsitem
			// 
			this.curvelinedefsitem.Name = "curvelinedefsitem";
			this.curvelinedefsitem.Size = new System.Drawing.Size(245, 22);
			this.curvelinedefsitem.Tag = "curvelinesmode";
			this.curvelinedefsitem.Text = "&Curve Linedefs...";
			this.curvelinedefsitem.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// toolStripMenuItem3
			// 
			this.toolStripMenuItem3.Name = "toolStripMenuItem3";
			this.toolStripMenuItem3.Size = new System.Drawing.Size(242, 6);
			// 
			// splitlinedefsitem
			// 
			this.splitlinedefsitem.Name = "splitlinedefsitem";
			this.splitlinedefsitem.Size = new System.Drawing.Size(245, 22);
			this.splitlinedefsitem.Tag = "splitlinedefs";
			this.splitlinedefsitem.Text = "S&plit Linedefs";
			this.splitlinedefsitem.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// alignLinedefsItem
			// 
			this.alignLinedefsItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.alignFloorToFrontItem,
            this.alignFloorToBackItem,
            this.alignCeilingToFrontItem,
            this.alignCeilingToBackItem});
			this.alignLinedefsItem.Name = "alignLinedefsItem";
			this.alignLinedefsItem.Size = new System.Drawing.Size(245, 22);
			this.alignLinedefsItem.Text = "&Align Textures";
			// 
			// alignFloorToFrontItem
			// 
			this.alignFloorToFrontItem.Name = "alignFloorToFrontItem";
			this.alignFloorToFrontItem.Size = new System.Drawing.Size(181, 22);
			this.alignFloorToFrontItem.Tag = "alignfloortofront";
			this.alignFloorToFrontItem.Text = "Floor to Front Side";
			this.alignFloorToFrontItem.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// alignFloorToBackItem
			// 
			this.alignFloorToBackItem.Name = "alignFloorToBackItem";
			this.alignFloorToBackItem.Size = new System.Drawing.Size(181, 22);
			this.alignFloorToBackItem.Tag = "alignfloortoback";
			this.alignFloorToBackItem.Text = "Floor to Back Side";
			this.alignFloorToBackItem.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// alignCeilingToFrontItem
			// 
			this.alignCeilingToFrontItem.Name = "alignCeilingToFrontItem";
			this.alignCeilingToFrontItem.Size = new System.Drawing.Size(181, 22);
			this.alignCeilingToFrontItem.Tag = "alignceilingtofront";
			this.alignCeilingToFrontItem.Text = "Ceiling to Front Side";
			this.alignCeilingToFrontItem.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// alignCeilingToBackItem
			// 
			this.alignCeilingToBackItem.Name = "alignCeilingToBackItem";
			this.alignCeilingToBackItem.Size = new System.Drawing.Size(181, 22);
			this.alignCeilingToBackItem.Tag = "alignceilingtoback";
			this.alignCeilingToBackItem.Text = "Ceiling to Back Side";
			this.alignCeilingToBackItem.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// sectorsmenu
			// 
			this.sectorsmenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.placethingss,
            this.selectInSectorsItem2,
            this.toolStripSeparator1,
            this.joinsectorsitem,
            this.mergesectorsitem,
            this.toolStripMenuItem2});
			this.sectorsmenu.Name = "sectorsmenu";
			this.sectorsmenu.Size = new System.Drawing.Size(57, 20);
			this.sectorsmenu.Text = "&Sectors";
			this.sectorsmenu.Visible = false;
			// 
			// placethingss
			// 
			this.placethingss.Name = "placethingss";
			this.placethingss.Size = new System.Drawing.Size(245, 22);
			this.placethingss.Tag = "placethings";
			this.placethingss.Text = "&Place Things...";
			this.placethingss.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// selectInSectorsItem2
			// 
			this.selectInSectorsItem2.Name = "selectInSectorsItem2";
			this.selectInSectorsItem2.Size = new System.Drawing.Size(245, 22);
			this.selectInSectorsItem2.Tag = "thingsselectinsectors";
			this.selectInSectorsItem2.Text = "&Select Things in Selected Sectors";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(242, 6);
			// 
			// joinsectorsitem
			// 
			this.joinsectorsitem.Name = "joinsectorsitem";
			this.joinsectorsitem.Size = new System.Drawing.Size(245, 22);
			this.joinsectorsitem.Tag = "joinsectors";
			this.joinsectorsitem.Text = "&Join Sectors";
			this.joinsectorsitem.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// mergesectorsitem
			// 
			this.mergesectorsitem.Name = "mergesectorsitem";
			this.mergesectorsitem.Size = new System.Drawing.Size(245, 22);
			this.mergesectorsitem.Tag = "mergesectors";
			this.mergesectorsitem.Text = "&Merge Sectors";
			this.mergesectorsitem.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// toolStripMenuItem2
			// 
			this.toolStripMenuItem2.Name = "toolStripMenuItem2";
			this.toolStripMenuItem2.Size = new System.Drawing.Size(242, 6);
			this.toolStripMenuItem2.Visible = false;
			// 
			// thingsmenu
			// 
			this.thingsmenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectInSectorsItem,
            this.toolStripSeparator3,
            this.alignToWallItem,
            this.pointAtCursorItem});
			this.thingsmenu.Name = "thingsmenu";
			this.thingsmenu.Size = new System.Drawing.Size(55, 20);
			this.thingsmenu.Text = "Things";
			this.thingsmenu.Visible = false;
			// 
			// selectInSectorsItem
			// 
			this.selectInSectorsItem.Name = "selectInSectorsItem";
			this.selectInSectorsItem.Size = new System.Drawing.Size(245, 22);
			this.selectInSectorsItem.Tag = "thingsselectinsectors";
			this.selectInSectorsItem.Text = "&Select Things in Selected Sectors";
			this.selectInSectorsItem.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(242, 6);
			// 
			// alignToWallItem
			// 
			this.alignToWallItem.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.AlignThings;
			this.alignToWallItem.Name = "alignToWallItem";
			this.alignToWallItem.Size = new System.Drawing.Size(245, 22);
			this.alignToWallItem.Tag = "thingaligntowall";
			this.alignToWallItem.Text = "&Align To Closest Linedef";
			this.alignToWallItem.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// pointAtCursorItem
			// 
			this.pointAtCursorItem.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.ThingPointAtCursor;
			this.pointAtCursorItem.Name = "pointAtCursorItem";
			this.pointAtCursorItem.Size = new System.Drawing.Size(245, 22);
			this.pointAtCursorItem.Tag = "thinglookatcursor";
			this.pointAtCursorItem.Text = "&Point at Cursor";
			this.pointAtCursorItem.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// vertsmenu
			// 
			this.vertsmenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.placethingsv});
			this.vertsmenu.Name = "vertsmenu";
			this.vertsmenu.Size = new System.Drawing.Size(60, 20);
			this.vertsmenu.Text = "Vertices";
			this.vertsmenu.Visible = false;
			// 
			// placethingsv
			// 
			this.placethingsv.Name = "placethingsv";
			this.placethingsv.Size = new System.Drawing.Size(150, 22);
			this.placethingsv.Tag = "placethings";
			this.placethingsv.Text = "&Place Things...";
			this.placethingsv.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// globalstrip
			// 
			this.globalstrip.Location = new System.Drawing.Point(0, 24);
			this.globalstrip.Name = "globalstrip";
			this.globalstrip.Size = new System.Drawing.Size(588, 25);
			this.globalstrip.TabIndex = 1;
			this.globalstrip.Text = "toolstrip";
			// 
			// manualstrip
			// 
			this.manualstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttoncopyproperties,
            this.buttonpasteproperties,
            this.seperatorcopypaste,
            this.buttonselectionnumbers,
            this.buttonselectioneffects,
            this.separatorsectors1,
            this.buttonbrightnessgradient,
            this.buttonfloorgradient,
            this.buttonceilinggradient,
            this.buttonflipselectionh,
            this.buttonflipselectionv,
            this.buttoncurvelinedefs,
            this.brightnessGradientMode,
            this.buttonMarqueSelectTouching,
            this.buttonAlignThingsToWall,
            this.buttonTextureOffsetLock,
            this.buttonMakeDoor});
			this.manualstrip.Location = new System.Drawing.Point(0, 49);
			this.manualstrip.Name = "manualstrip";
			this.manualstrip.Size = new System.Drawing.Size(588, 25);
			this.manualstrip.TabIndex = 2;
			this.manualstrip.Text = "toolStrip1";
			// 
			// buttoncopyproperties
			// 
			this.buttoncopyproperties.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttoncopyproperties.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.CopyProperties;
			this.buttoncopyproperties.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttoncopyproperties.Name = "buttoncopyproperties";
			this.buttoncopyproperties.Size = new System.Drawing.Size(23, 22);
			this.buttoncopyproperties.Tag = "classiccopyproperties";
			this.buttoncopyproperties.Text = "Copy Properties";
			this.buttoncopyproperties.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttonpasteproperties
			// 
			this.buttonpasteproperties.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonpasteproperties.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.PasteProperties;
			this.buttonpasteproperties.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonpasteproperties.Name = "buttonpasteproperties";
			this.buttonpasteproperties.Size = new System.Drawing.Size(23, 22);
			this.buttonpasteproperties.Tag = "classicpasteproperties";
			this.buttonpasteproperties.Text = "Paste Properties";
			this.buttonpasteproperties.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// seperatorcopypaste
			// 
			this.seperatorcopypaste.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.seperatorcopypaste.Name = "seperatorcopypaste";
			this.seperatorcopypaste.Size = new System.Drawing.Size(6, 25);
			// 
			// buttonselectionnumbers
			// 
			this.buttonselectionnumbers.CheckOnClick = true;
			this.buttonselectionnumbers.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonselectionnumbers.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.ViewSelectionIndex;
			this.buttonselectionnumbers.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonselectionnumbers.Name = "buttonselectionnumbers";
			this.buttonselectionnumbers.Size = new System.Drawing.Size(23, 22);
			this.buttonselectionnumbers.Text = "View Selection Numbering";
			this.buttonselectionnumbers.Click += new System.EventHandler(this.buttonselectionnumbers_Click);
			// 
			// buttonselectioneffects
			// 
			this.buttonselectioneffects.CheckOnClick = true;
			this.buttonselectioneffects.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonselectioneffects.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.ViewSelectionEffects;
			this.buttonselectioneffects.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonselectioneffects.Name = "buttonselectioneffects";
			this.buttonselectioneffects.Size = new System.Drawing.Size(23, 22);
			this.buttonselectioneffects.Text = "View Tags and Effects";
			this.buttonselectioneffects.Click += new System.EventHandler(this.buttonselectioneffects_Click);
			// 
			// separatorsectors1
			// 
			this.separatorsectors1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.separatorsectors1.Name = "separatorsectors1";
			this.separatorsectors1.Size = new System.Drawing.Size(6, 25);
			// 
			// buttonbrightnessgradient
			// 
			this.buttonbrightnessgradient.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonbrightnessgradient.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.BrightnessGradient;
			this.buttonbrightnessgradient.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
			this.buttonbrightnessgradient.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonbrightnessgradient.Name = "buttonbrightnessgradient";
			this.buttonbrightnessgradient.Size = new System.Drawing.Size(23, 22);
			this.buttonbrightnessgradient.Tag = "gradientbrightness";
			this.buttonbrightnessgradient.Text = "Make Brightness Gradient";
			this.buttonbrightnessgradient.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttonfloorgradient
			// 
			this.buttonfloorgradient.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonfloorgradient.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.FloorsGradient;
			this.buttonfloorgradient.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonfloorgradient.Name = "buttonfloorgradient";
			this.buttonfloorgradient.Size = new System.Drawing.Size(23, 22);
			this.buttonfloorgradient.Tag = "gradientfloors";
			this.buttonfloorgradient.Text = "Make Floor Heights Gradient";
			this.buttonfloorgradient.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttonceilinggradient
			// 
			this.buttonceilinggradient.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonceilinggradient.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.CeilsGradient;
			this.buttonceilinggradient.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonceilinggradient.Name = "buttonceilinggradient";
			this.buttonceilinggradient.Size = new System.Drawing.Size(23, 22);
			this.buttonceilinggradient.Tag = "gradientceilings";
			this.buttonceilinggradient.Text = "Make Ceiling Heights Gradient";
			this.buttonceilinggradient.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttonflipselectionh
			// 
			this.buttonflipselectionh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonflipselectionh.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.FlipSelectionH;
			this.buttonflipselectionh.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonflipselectionh.Name = "buttonflipselectionh";
			this.buttonflipselectionh.Size = new System.Drawing.Size(23, 22);
			this.buttonflipselectionh.Tag = "flipselectionh";
			this.buttonflipselectionh.Text = "Flip Selection Horizontally";
			this.buttonflipselectionh.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttonflipselectionv
			// 
			this.buttonflipselectionv.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonflipselectionv.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.FlipSelectionV;
			this.buttonflipselectionv.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonflipselectionv.Name = "buttonflipselectionv";
			this.buttonflipselectionv.Size = new System.Drawing.Size(23, 22);
			this.buttonflipselectionv.Tag = "flipselectionv";
			this.buttonflipselectionv.Text = "Flip Selection Vertically";
			this.buttonflipselectionv.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttoncurvelinedefs
			// 
			this.buttoncurvelinedefs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttoncurvelinedefs.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.CurveLines;
			this.buttoncurvelinedefs.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttoncurvelinedefs.Name = "buttoncurvelinedefs";
			this.buttoncurvelinedefs.Size = new System.Drawing.Size(23, 22);
			this.buttoncurvelinedefs.Tag = "curvelinesmode";
			this.buttoncurvelinedefs.Text = "Curve Linedefs";
			this.buttoncurvelinedefs.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// brightnessGradientMode
			// 
			this.brightnessGradientMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.brightnessGradientMode.Name = "brightnessGradientMode";
			this.brightnessGradientMode.Size = new System.Drawing.Size(75, 25);
			this.brightnessGradientMode.ToolTipText = "Brightness Gradient affects:";
			this.brightnessGradientMode.DropDownClosed += new System.EventHandler(this.brightnessGradientMode_DropDownClosed);
			// 
			// buttonMarqueSelectTouching
			// 
			this.buttonMarqueSelectTouching.CheckOnClick = true;
			this.buttonMarqueSelectTouching.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonMarqueSelectTouching.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.SelectTouching;
			this.buttonMarqueSelectTouching.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonMarqueSelectTouching.Name = "buttonMarqueSelectTouching";
			this.buttonMarqueSelectTouching.Size = new System.Drawing.Size(23, 22);
			this.buttonMarqueSelectTouching.Text = "Select Touching";
			this.buttonMarqueSelectTouching.ToolTipText = "Toggle between \"select touching\" and \"select inside\"\r\nrectangular selection modes" +
				"";
			this.buttonMarqueSelectTouching.Click += new System.EventHandler(this.buttonMarqueSelectTouching_Click);
			// 
			// buttonAlignThingsToWall
			// 
			this.buttonAlignThingsToWall.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonAlignThingsToWall.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.AlignThings;
			this.buttonAlignThingsToWall.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonAlignThingsToWall.Name = "buttonAlignThingsToWall";
			this.buttonAlignThingsToWall.Size = new System.Drawing.Size(23, 22);
			this.buttonAlignThingsToWall.Tag = "thingaligntowall";
			this.buttonAlignThingsToWall.ToolTipText = "Align selected things to closest linedef";
			this.buttonAlignThingsToWall.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// buttonTextureOffsetLock
			// 
			this.buttonTextureOffsetLock.CheckOnClick = true;
			this.buttonTextureOffsetLock.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonTextureOffsetLock.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.TextureLock;
			this.buttonTextureOffsetLock.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonTextureOffsetLock.Name = "buttonTextureOffsetLock";
			this.buttonTextureOffsetLock.Size = new System.Drawing.Size(23, 22);
			this.buttonTextureOffsetLock.ToolTipText = "Pin Texture Offsets.\r\nWhen enabled, keeps floor and ceiling texture offsets\r\ncons" +
				"tant while sector is dragged";
			this.buttonTextureOffsetLock.Click += new System.EventHandler(this.buttonTextureOffsetLock_Click);
			// 
			// buttonMakeDoor
			// 
			this.buttonMakeDoor.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonMakeDoor.Image = global::CodeImp.DoomBuilder.BuilderModes.Properties.Resources.Door;
			this.buttonMakeDoor.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonMakeDoor.Name = "buttonMakeDoor";
			this.buttonMakeDoor.Size = new System.Drawing.Size(23, 22);
			this.buttonMakeDoor.Tag = "makedoor";
			this.buttonMakeDoor.Text = "Make Door From Selection";
			this.buttonMakeDoor.Click += new System.EventHandler(this.InvokeTaggedAction);
			// 
			// MenusForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(588, 100);
			this.Controls.Add(this.manualstrip);
			this.Controls.Add(this.globalstrip);
			this.Controls.Add(this.menustrip);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MainMenuStrip = this.menustrip;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MenusForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "MenusForm";
			this.menustrip.ResumeLayout(false);
			this.menustrip.PerformLayout();
			this.manualstrip.ResumeLayout(false);
			this.manualstrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menustrip;
		private System.Windows.Forms.ToolStripMenuItem linedefsmenu;
		private System.Windows.Forms.ToolStripMenuItem sectorsmenu;
		private System.Windows.Forms.ToolStripMenuItem fliplinedefsitem;
		private System.Windows.Forms.ToolStripMenuItem flipsidedefsitem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem curvelinedefsitem;
		private System.Windows.Forms.ToolStripMenuItem joinsectorsitem;
		private System.Windows.Forms.ToolStripMenuItem mergesectorsitem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem splitlinedefsitem;
		private System.Windows.Forms.ToolStrip globalstrip;
		private System.Windows.Forms.ToolStrip manualstrip;
		private System.Windows.Forms.ToolStripButton buttonbrightnessgradient;
		private System.Windows.Forms.ToolStripMenuItem selectsinglesideditem;
		private System.Windows.Forms.ToolStripMenuItem selectdoublesideditem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
		private System.Windows.Forms.ToolStripButton buttonflipselectionh;
		private System.Windows.Forms.ToolStripButton buttonflipselectionv;
		private System.Windows.Forms.ToolStripButton buttonselectionnumbers;
		private System.Windows.Forms.ToolStripSeparator separatorsectors1;
		private System.Windows.Forms.ToolStripButton buttonfloorgradient;
		private System.Windows.Forms.ToolStripButton buttonceilinggradient;
		private System.Windows.Forms.ToolStripButton buttoncurvelinedefs;
		private System.Windows.Forms.ToolStripButton buttoncopyproperties;
		private System.Windows.Forms.ToolStripButton buttonpasteproperties;
		private System.Windows.Forms.ToolStripSeparator seperatorcopypaste;
		private System.Windows.Forms.ToolStripComboBox brightnessGradientMode;
		private System.Windows.Forms.ToolStripButton buttonMarqueSelectTouching;
		private System.Windows.Forms.ToolStripMenuItem thingsmenu;
		private System.Windows.Forms.ToolStripMenuItem alignToWallItem;
		private System.Windows.Forms.ToolStripButton buttonAlignThingsToWall;
		private System.Windows.Forms.ToolStripMenuItem pointAtCursorItem;
		private System.Windows.Forms.ToolStripButton buttonTextureOffsetLock;
		private System.Windows.Forms.ToolStripMenuItem selectInSectorsItem;
		private System.Windows.Forms.ToolStripMenuItem alignLinedefsItem;
		private System.Windows.Forms.ToolStripMenuItem alignFloorToFrontItem;
		private System.Windows.Forms.ToolStripMenuItem alignFloorToBackItem;
		private System.Windows.Forms.ToolStripMenuItem alignCeilingToFrontItem;
		private System.Windows.Forms.ToolStripMenuItem alignCeilingToBackItem;
		private System.Windows.Forms.ToolStripMenuItem vertsmenu;
		private System.Windows.Forms.ToolStripMenuItem placethingsv;
		private System.Windows.Forms.ToolStripMenuItem placethingsl;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripMenuItem placethingss;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripButton buttonselectioneffects;
		private System.Windows.Forms.ToolStripMenuItem selectInSectorsItem3;
		private System.Windows.Forms.ToolStripMenuItem selectInSectorsItem2;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripButton buttonMakeDoor;
	}
}