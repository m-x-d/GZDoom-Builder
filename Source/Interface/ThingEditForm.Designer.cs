namespace CodeImp.DoomBuilder.Interface
{
	partial class ThingEditForm
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
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.GroupBox groupBox1;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label1;
			System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Monsters");
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ThingEditForm));
			System.Windows.Forms.GroupBox groupBox2;
			System.Windows.Forms.Label label6;
			System.Windows.Forms.Label label5;
			System.Windows.Forms.Label taglabel;
			System.Windows.Forms.Label label7;
			this.sizelabel = new System.Windows.Forms.Label();
			this.blockinglabel = new System.Windows.Forms.Label();
			this.positionlabel = new System.Windows.Forms.Label();
			this.typeid = new CodeImp.DoomBuilder.Interface.NumericTextbox();
			this.typelist = new System.Windows.Forms.TreeView();
			this.thingimages = new System.Windows.Forms.ImageList(this.components);
			this.height = new CodeImp.DoomBuilder.Interface.NumericTextbox();
			this.angle = new CodeImp.DoomBuilder.Interface.NumericTextbox();
			this.anglecontrol = new CodeImp.DoomBuilder.Interface.AngleControl();
			this.tabs = new System.Windows.Forms.TabControl();
			this.tabproperties = new System.Windows.Forms.TabPage();
			this.spritetex = new System.Windows.Forms.Panel();
			this.settingsgroup = new System.Windows.Forms.GroupBox();
			this.flags = new CodeImp.DoomBuilder.Interface.CheckboxArrayControl();
			this.tabeffects = new System.Windows.Forms.TabPage();
			this.actiongroup = new System.Windows.Forms.GroupBox();
			this.hexenpanel = new System.Windows.Forms.Panel();
			this.arg3 = new CodeImp.DoomBuilder.Interface.NumericTextbox();
			this.arg2 = new CodeImp.DoomBuilder.Interface.NumericTextbox();
			this.arg4 = new CodeImp.DoomBuilder.Interface.NumericTextbox();
			this.arg1 = new CodeImp.DoomBuilder.Interface.NumericTextbox();
			this.arg0 = new CodeImp.DoomBuilder.Interface.NumericTextbox();
			this.arg1label = new System.Windows.Forms.Label();
			this.arg0label = new System.Windows.Forms.Label();
			this.arg3label = new System.Windows.Forms.Label();
			this.arg2label = new System.Windows.Forms.Label();
			this.arg4label = new System.Windows.Forms.Label();
			this.action = new CodeImp.DoomBuilder.Interface.ActionSelectorControl();
			this.browseaction = new System.Windows.Forms.Button();
			this.doompanel = new System.Windows.Forms.Panel();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.tag = new CodeImp.DoomBuilder.Interface.NumericTextbox();
			this.newtag = new System.Windows.Forms.Button();
			this.tabcustom = new System.Windows.Forms.TabPage();
			this.fieldslist = new CodeImp.DoomBuilder.Interface.FieldsEditorControl();
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			groupBox1 = new System.Windows.Forms.GroupBox();
			label4 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			groupBox2 = new System.Windows.Forms.GroupBox();
			label6 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			taglabel = new System.Windows.Forms.Label();
			label7 = new System.Windows.Forms.Label();
			groupBox1.SuspendLayout();
			groupBox2.SuspendLayout();
			this.tabs.SuspendLayout();
			this.tabproperties.SuspendLayout();
			this.settingsgroup.SuspendLayout();
			this.tabeffects.SuspendLayout();
			this.actiongroup.SuspendLayout();
			this.hexenpanel.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.tabcustom.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			groupBox1.Controls.Add(this.sizelabel);
			groupBox1.Controls.Add(label4);
			groupBox1.Controls.Add(this.blockinglabel);
			groupBox1.Controls.Add(label3);
			groupBox1.Controls.Add(this.positionlabel);
			groupBox1.Controls.Add(label2);
			groupBox1.Controls.Add(this.typeid);
			groupBox1.Controls.Add(label1);
			groupBox1.Controls.Add(this.typelist);
			groupBox1.Location = new System.Drawing.Point(6, 6);
			groupBox1.Name = "groupBox1";
			groupBox1.Size = new System.Drawing.Size(269, 340);
			groupBox1.TabIndex = 1;
			groupBox1.TabStop = false;
			groupBox1.Text = " Thing ";
			// 
			// sizelabel
			// 
			this.sizelabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.sizelabel.AutoSize = true;
			this.sizelabel.Location = new System.Drawing.Point(191, 285);
			this.sizelabel.Name = "sizelabel";
			this.sizelabel.Size = new System.Drawing.Size(43, 14);
			this.sizelabel.TabIndex = 8;
			this.sizelabel.Text = "16 x 96";
			// 
			// label4
			// 
			label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			label4.AutoSize = true;
			label4.Location = new System.Drawing.Point(157, 285);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(31, 14);
			label4.TabIndex = 7;
			label4.Text = "Size:";
			// 
			// blockinglabel
			// 
			this.blockinglabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.blockinglabel.AutoSize = true;
			this.blockinglabel.Location = new System.Drawing.Point(191, 314);
			this.blockinglabel.Name = "blockinglabel";
			this.blockinglabel.Size = new System.Drawing.Size(63, 14);
			this.blockinglabel.TabIndex = 6;
			this.blockinglabel.Text = "True-Height";
			// 
			// label3
			// 
			label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(138, 314);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(50, 14);
			label3.TabIndex = 5;
			label3.Text = "Blocking:";
			// 
			// positionlabel
			// 
			this.positionlabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.positionlabel.AutoSize = true;
			this.positionlabel.Location = new System.Drawing.Point(58, 314);
			this.positionlabel.Name = "positionlabel";
			this.positionlabel.Size = new System.Drawing.Size(38, 14);
			this.positionlabel.TabIndex = 4;
			this.positionlabel.Text = "Ceiling";
			// 
			// label2
			// 
			label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(8, 314);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(47, 14);
			label2.TabIndex = 3;
			label2.Text = "Position:";
			// 
			// typeid
			// 
			this.typeid.AllowNegative = false;
			this.typeid.AllowRelative = false;
			this.typeid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.typeid.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.typeid.Location = new System.Drawing.Point(51, 282);
			this.typeid.Name = "typeid";
			this.typeid.Size = new System.Drawing.Size(68, 20);
			this.typeid.TabIndex = 2;
			this.typeid.TextChanged += new System.EventHandler(this.typeid_TextChanged);
			// 
			// label1
			// 
			label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(8, 285);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(34, 14);
			label1.TabIndex = 1;
			label1.Text = "Type:";
			// 
			// typelist
			// 
			this.typelist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.typelist.HideSelection = false;
			this.typelist.ImageIndex = 0;
			this.typelist.ImageList = this.thingimages;
			this.typelist.Location = new System.Drawing.Point(11, 24);
			this.typelist.Margin = new System.Windows.Forms.Padding(8, 8, 9, 8);
			this.typelist.Name = "typelist";
			treeNode2.Name = "Node0";
			treeNode2.Text = "Monsters";
			this.typelist.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode2});
			this.typelist.SelectedImageIndex = 0;
			this.typelist.Size = new System.Drawing.Size(246, 248);
			this.typelist.TabIndex = 0;
			this.typelist.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.typelist_AfterSelect);
			// 
			// thingimages
			// 
			this.thingimages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("thingimages.ImageStream")));
			this.thingimages.TransparentColor = System.Drawing.SystemColors.Window;
			this.thingimages.Images.SetKeyName(0, "ThingsListIcon.png");
			// 
			// groupBox2
			// 
			groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			groupBox2.Controls.Add(this.height);
			groupBox2.Controls.Add(label6);
			groupBox2.Controls.Add(label5);
			groupBox2.Controls.Add(this.angle);
			groupBox2.Controls.Add(this.anglecontrol);
			groupBox2.Location = new System.Drawing.Point(397, 241);
			groupBox2.Name = "groupBox2";
			groupBox2.Size = new System.Drawing.Size(230, 105);
			groupBox2.TabIndex = 21;
			groupBox2.TabStop = false;
			groupBox2.Text = " Coordination ";
			// 
			// height
			// 
			this.height.AllowNegative = true;
			this.height.AllowRelative = true;
			this.height.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.height.Location = new System.Drawing.Point(68, 63);
			this.height.Name = "height";
			this.height.Size = new System.Drawing.Size(50, 20);
			this.height.TabIndex = 10;
			// 
			// label6
			// 
			label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(12, 66);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(50, 14);
			label6.TabIndex = 9;
			label6.Text = "Z Height:";
			// 
			// label5
			// 
			label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			label5.AutoSize = true;
			label5.Location = new System.Drawing.Point(24, 31);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(38, 14);
			label5.TabIndex = 8;
			label5.Text = "Angle:";
			// 
			// angle
			// 
			this.angle.AllowNegative = true;
			this.angle.AllowRelative = true;
			this.angle.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.angle.Location = new System.Drawing.Point(68, 28);
			this.angle.Name = "angle";
			this.angle.Size = new System.Drawing.Size(50, 20);
			this.angle.TabIndex = 1;
			this.angle.TextChanged += new System.EventHandler(this.angle_TextChanged);
			// 
			// anglecontrol
			// 
			this.anglecontrol.BackColor = System.Drawing.SystemColors.Control;
			this.anglecontrol.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.anglecontrol.Location = new System.Drawing.Point(141, 15);
			this.anglecontrol.Name = "anglecontrol";
			this.anglecontrol.Size = new System.Drawing.Size(80, 80);
			this.anglecontrol.TabIndex = 0;
			this.anglecontrol.Value = 0;
			this.anglecontrol.ButtonClicked += new System.EventHandler(this.anglecontrol_ButtonClicked);
			// 
			// taglabel
			// 
			taglabel.AutoSize = true;
			taglabel.Location = new System.Drawing.Point(28, 31);
			taglabel.Name = "taglabel";
			taglabel.Size = new System.Drawing.Size(28, 14);
			taglabel.TabIndex = 6;
			taglabel.Text = "Tag:";
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(15, 30);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(41, 14);
			label7.TabIndex = 9;
			label7.Text = "Action:";
			// 
			// tabs
			// 
			this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabs.Controls.Add(this.tabproperties);
			this.tabs.Controls.Add(this.tabeffects);
			this.tabs.Controls.Add(this.tabcustom);
			this.tabs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabs.Location = new System.Drawing.Point(10, 10);
			this.tabs.Margin = new System.Windows.Forms.Padding(1);
			this.tabs.Name = "tabs";
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(641, 379);
			this.tabs.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.tabs.TabIndex = 2;
			// 
			// tabproperties
			// 
			this.tabproperties.Controls.Add(this.spritetex);
			this.tabproperties.Controls.Add(groupBox2);
			this.tabproperties.Controls.Add(this.settingsgroup);
			this.tabproperties.Controls.Add(groupBox1);
			this.tabproperties.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabproperties.Location = new System.Drawing.Point(4, 23);
			this.tabproperties.Name = "tabproperties";
			this.tabproperties.Padding = new System.Windows.Forms.Padding(3);
			this.tabproperties.Size = new System.Drawing.Size(633, 352);
			this.tabproperties.TabIndex = 0;
			this.tabproperties.Text = "Properties";
			this.tabproperties.UseVisualStyleBackColor = true;
			// 
			// spritetex
			// 
			this.spritetex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.spritetex.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.spritetex.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.spritetex.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.spritetex.Location = new System.Drawing.Point(284, 246);
			this.spritetex.Name = "spritetex";
			this.spritetex.Size = new System.Drawing.Size(104, 100);
			this.spritetex.TabIndex = 22;
			// 
			// settingsgroup
			// 
			this.settingsgroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.settingsgroup.Controls.Add(this.flags);
			this.settingsgroup.Location = new System.Drawing.Point(284, 6);
			this.settingsgroup.Name = "settingsgroup";
			this.settingsgroup.Size = new System.Drawing.Size(343, 229);
			this.settingsgroup.TabIndex = 0;
			this.settingsgroup.TabStop = false;
			this.settingsgroup.Text = " Settings ";
			// 
			// flags
			// 
			this.flags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.flags.AutoScroll = true;
			this.flags.Columns = 2;
			this.flags.Location = new System.Drawing.Point(18, 26);
			this.flags.Name = "flags";
			this.flags.Size = new System.Drawing.Size(319, 196);
			this.flags.TabIndex = 0;
			// 
			// tabeffects
			// 
			this.tabeffects.Controls.Add(this.actiongroup);
			this.tabeffects.Controls.Add(this.groupBox3);
			this.tabeffects.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabeffects.Location = new System.Drawing.Point(4, 23);
			this.tabeffects.Name = "tabeffects";
			this.tabeffects.Padding = new System.Windows.Forms.Padding(3);
			this.tabeffects.Size = new System.Drawing.Size(633, 352);
			this.tabeffects.TabIndex = 1;
			this.tabeffects.Text = "Effects";
			this.tabeffects.UseVisualStyleBackColor = true;
			// 
			// actiongroup
			// 
			this.actiongroup.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.actiongroup.Controls.Add(this.hexenpanel);
			this.actiongroup.Controls.Add(label7);
			this.actiongroup.Controls.Add(this.action);
			this.actiongroup.Controls.Add(this.browseaction);
			this.actiongroup.Controls.Add(this.doompanel);
			this.actiongroup.Location = new System.Drawing.Point(6, 78);
			this.actiongroup.Name = "actiongroup";
			this.actiongroup.Size = new System.Drawing.Size(621, 268);
			this.actiongroup.TabIndex = 22;
			this.actiongroup.TabStop = false;
			this.actiongroup.Text = " Action ";
			// 
			// hexenpanel
			// 
			this.hexenpanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.hexenpanel.Controls.Add(this.arg3);
			this.hexenpanel.Controls.Add(this.arg2);
			this.hexenpanel.Controls.Add(this.arg4);
			this.hexenpanel.Controls.Add(this.arg1);
			this.hexenpanel.Controls.Add(this.arg0);
			this.hexenpanel.Controls.Add(this.arg1label);
			this.hexenpanel.Controls.Add(this.arg0label);
			this.hexenpanel.Controls.Add(this.arg3label);
			this.hexenpanel.Controls.Add(this.arg2label);
			this.hexenpanel.Controls.Add(this.arg4label);
			this.hexenpanel.Location = new System.Drawing.Point(6, 53);
			this.hexenpanel.Name = "hexenpanel";
			this.hexenpanel.Size = new System.Drawing.Size(609, 208);
			this.hexenpanel.TabIndex = 13;
			// 
			// arg3
			// 
			this.arg3.AllowNegative = false;
			this.arg3.AllowRelative = true;
			this.arg3.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.arg3.Location = new System.Drawing.Point(388, 13);
			this.arg3.Name = "arg3";
			this.arg3.Size = new System.Drawing.Size(50, 20);
			this.arg3.TabIndex = 21;
			// 
			// arg2
			// 
			this.arg2.AllowNegative = false;
			this.arg2.AllowRelative = true;
			this.arg2.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.arg2.Location = new System.Drawing.Point(178, 65);
			this.arg2.Name = "arg2";
			this.arg2.Size = new System.Drawing.Size(50, 20);
			this.arg2.TabIndex = 19;
			// 
			// arg4
			// 
			this.arg4.AllowNegative = false;
			this.arg4.AllowRelative = true;
			this.arg4.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.arg4.Location = new System.Drawing.Point(388, 39);
			this.arg4.Name = "arg4";
			this.arg4.Size = new System.Drawing.Size(50, 20);
			this.arg4.TabIndex = 17;
			// 
			// arg1
			// 
			this.arg1.AllowNegative = false;
			this.arg1.AllowRelative = true;
			this.arg1.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.arg1.Location = new System.Drawing.Point(178, 39);
			this.arg1.Name = "arg1";
			this.arg1.Size = new System.Drawing.Size(50, 20);
			this.arg1.TabIndex = 15;
			// 
			// arg0
			// 
			this.arg0.AllowNegative = false;
			this.arg0.AllowRelative = true;
			this.arg0.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.arg0.Location = new System.Drawing.Point(178, 13);
			this.arg0.Name = "arg0";
			this.arg0.Size = new System.Drawing.Size(50, 20);
			this.arg0.TabIndex = 13;
			// 
			// arg1label
			// 
			this.arg1label.Location = new System.Drawing.Point(-7, 42);
			this.arg1label.Name = "arg1label";
			this.arg1label.Size = new System.Drawing.Size(179, 14);
			this.arg1label.TabIndex = 14;
			this.arg1label.Text = "Argument 2:";
			this.arg1label.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arg1label.UseMnemonic = false;
			// 
			// arg0label
			// 
			this.arg0label.Location = new System.Drawing.Point(-7, 16);
			this.arg0label.Name = "arg0label";
			this.arg0label.Size = new System.Drawing.Size(179, 14);
			this.arg0label.TabIndex = 12;
			this.arg0label.Text = "Argument 1:";
			this.arg0label.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arg0label.UseMnemonic = false;
			// 
			// arg3label
			// 
			this.arg3label.Location = new System.Drawing.Point(203, 16);
			this.arg3label.Name = "arg3label";
			this.arg3label.Size = new System.Drawing.Size(179, 14);
			this.arg3label.TabIndex = 20;
			this.arg3label.Text = "Argument 4:";
			this.arg3label.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arg3label.UseMnemonic = false;
			// 
			// arg2label
			// 
			this.arg2label.Location = new System.Drawing.Point(-7, 68);
			this.arg2label.Name = "arg2label";
			this.arg2label.Size = new System.Drawing.Size(179, 14);
			this.arg2label.TabIndex = 18;
			this.arg2label.Text = "Argument 3:";
			this.arg2label.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arg2label.UseMnemonic = false;
			// 
			// arg4label
			// 
			this.arg4label.Location = new System.Drawing.Point(203, 42);
			this.arg4label.Name = "arg4label";
			this.arg4label.Size = new System.Drawing.Size(179, 14);
			this.arg4label.TabIndex = 16;
			this.arg4label.Text = "Argument 5:";
			this.arg4label.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.arg4label.UseMnemonic = false;
			// 
			// action
			// 
			this.action.BackColor = System.Drawing.SystemColors.Control;
			this.action.Cursor = System.Windows.Forms.Cursors.Default;
			this.action.Empty = false;
			this.action.GeneralizedCategories = null;
			this.action.Location = new System.Drawing.Point(62, 27);
			this.action.Name = "action";
			this.action.Size = new System.Drawing.Size(401, 21);
			this.action.TabIndex = 5;
			this.action.Value = 402;
			this.action.ValueChanges += new System.EventHandler(this.action_ValueChanges);
			// 
			// browseaction
			// 
			this.browseaction.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.browseaction.Image = global::CodeImp.DoomBuilder.Properties.Resources.treeview;
			this.browseaction.Location = new System.Drawing.Point(469, 26);
			this.browseaction.Name = "browseaction";
			this.browseaction.Padding = new System.Windows.Forms.Padding(0, 0, 1, 3);
			this.browseaction.Size = new System.Drawing.Size(30, 23);
			this.browseaction.TabIndex = 3;
			this.browseaction.Text = " ";
			this.browseaction.UseVisualStyleBackColor = true;
			this.browseaction.Click += new System.EventHandler(this.browseaction_Click);
			// 
			// doompanel
			// 
			this.doompanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.doompanel.Location = new System.Drawing.Point(6, 54);
			this.doompanel.Name = "doompanel";
			this.doompanel.Size = new System.Drawing.Size(609, 208);
			this.doompanel.TabIndex = 12;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.tag);
			this.groupBox3.Controls.Add(taglabel);
			this.groupBox3.Controls.Add(this.newtag);
			this.groupBox3.Location = new System.Drawing.Point(6, 6);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(621, 66);
			this.groupBox3.TabIndex = 21;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = " Identification ";
			// 
			// tag
			// 
			this.tag.AllowNegative = false;
			this.tag.AllowRelative = true;
			this.tag.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.tag.Location = new System.Drawing.Point(62, 28);
			this.tag.Name = "tag";
			this.tag.Size = new System.Drawing.Size(68, 20);
			this.tag.TabIndex = 7;
			// 
			// newtag
			// 
			this.newtag.Location = new System.Drawing.Point(136, 27);
			this.newtag.Name = "newtag";
			this.newtag.Size = new System.Drawing.Size(76, 23);
			this.newtag.TabIndex = 8;
			this.newtag.Text = "New Tag";
			this.newtag.UseVisualStyleBackColor = true;
			this.newtag.Click += new System.EventHandler(this.newtag_Click);
			// 
			// tabcustom
			// 
			this.tabcustom.Controls.Add(this.fieldslist);
			this.tabcustom.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabcustom.Location = new System.Drawing.Point(4, 23);
			this.tabcustom.Name = "tabcustom";
			this.tabcustom.Size = new System.Drawing.Size(633, 352);
			this.tabcustom.TabIndex = 2;
			this.tabcustom.Text = "Custom";
			this.tabcustom.UseVisualStyleBackColor = true;
			// 
			// fieldslist
			// 
			this.fieldslist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.fieldslist.Location = new System.Drawing.Point(8, 9);
			this.fieldslist.Margin = new System.Windows.Forms.Padding(8, 9, 8, 9);
			this.fieldslist.Name = "fieldslist";
			this.fieldslist.Size = new System.Drawing.Size(617, 334);
			this.fieldslist.TabIndex = 1;
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(539, 406);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(112, 25);
			this.cancel.TabIndex = 19;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// apply
			// 
			this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.apply.Location = new System.Drawing.Point(420, 406);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 18;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// ThingEditForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(661, 441);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Controls.Add(this.tabs);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ThingEditForm";
			this.Opacity = 0;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit Thing";
			groupBox1.ResumeLayout(false);
			groupBox1.PerformLayout();
			groupBox2.ResumeLayout(false);
			groupBox2.PerformLayout();
			this.tabs.ResumeLayout(false);
			this.tabproperties.ResumeLayout(false);
			this.settingsgroup.ResumeLayout(false);
			this.tabeffects.ResumeLayout(false);
			this.actiongroup.ResumeLayout(false);
			this.actiongroup.PerformLayout();
			this.hexenpanel.ResumeLayout(false);
			this.hexenpanel.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.tabcustom.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage tabproperties;
		private System.Windows.Forms.TabPage tabeffects;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.TreeView typelist;
		private System.Windows.Forms.ImageList thingimages;
		private System.Windows.Forms.TabPage tabcustom;
		private System.Windows.Forms.Label positionlabel;
		private NumericTextbox typeid;
		private System.Windows.Forms.Label blockinglabel;
		private System.Windows.Forms.Label sizelabel;
		private System.Windows.Forms.GroupBox settingsgroup;
		private CheckboxArrayControl flags;
		private System.Windows.Forms.Panel spritetex;
		private AngleControl anglecontrol;
		private NumericTextbox height;
		private NumericTextbox angle;
		private System.Windows.Forms.GroupBox groupBox3;
		private NumericTextbox tag;
		private System.Windows.Forms.Button newtag;
		private System.Windows.Forms.GroupBox actiongroup;
		private System.Windows.Forms.Panel hexenpanel;
		private NumericTextbox arg3;
		private NumericTextbox arg2;
		private NumericTextbox arg4;
		private NumericTextbox arg1;
		private NumericTextbox arg0;
		private System.Windows.Forms.Label arg1label;
		private System.Windows.Forms.Label arg0label;
		private System.Windows.Forms.Label arg3label;
		private System.Windows.Forms.Label arg2label;
		private System.Windows.Forms.Label arg4label;
		private ActionSelectorControl action;
		private System.Windows.Forms.Button browseaction;
		private System.Windows.Forms.Panel doompanel;
		private FieldsEditorControl fieldslist;
	}
}