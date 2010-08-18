namespace CodeImp.DoomBuilder.Windows
{
	partial class SectorEditForm
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
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label taglabel;
            System.Windows.Forms.Label label8;
            System.Windows.Forms.GroupBox groupfloorceiling;
            System.Windows.Forms.Label label5;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label6;
            System.Windows.Forms.GroupBox groupBox1;
            System.Windows.Forms.GroupBox groupBox2;
            System.Windows.Forms.GroupBox groupBox3;
            this.floorheight = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
            this.ceilingheight = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
            this.sectorheight = new System.Windows.Forms.Label();
            this.sectorheightlabel = new System.Windows.Forms.Label();
            this.floortex = new CodeImp.DoomBuilder.Controls.FlatSelectorControl();
            this.ceilingtex = new CodeImp.DoomBuilder.Controls.FlatSelectorControl();
            this.floorcolor = new CodeImp.DoomBuilder.Controls.ColorControl();
            this.lowercolor = new CodeImp.DoomBuilder.Controls.ColorControl();
            this.thingcolor = new CodeImp.DoomBuilder.Controls.ColorControl();
            this.topcolor = new CodeImp.DoomBuilder.Controls.ColorControl();
            this.ceilingcolor = new CodeImp.DoomBuilder.Controls.ColorControl();
            this.button14 = new System.Windows.Forms.Button();
            this.button15 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button19 = new System.Windows.Forms.Button();
            this.button20 = new System.Windows.Forms.Button();
            this.button17 = new System.Windows.Forms.Button();
            this.button18 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button16 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.groupaction = new System.Windows.Forms.GroupBox();
            this.tag = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
            this.newtag = new System.Windows.Forms.Button();
            this.groupeffect = new System.Windows.Forms.GroupBox();
            this.brightness = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
            this.browseeffect = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.effect = new CodeImp.DoomBuilder.Controls.ActionSelectorControl();
            this.cancel = new System.Windows.Forms.Button();
            this.apply = new System.Windows.Forms.Button();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabproperties = new System.Windows.Forms.TabPage();
            this.settingsgroup = new System.Windows.Forms.GroupBox();
            this.flags = new CodeImp.DoomBuilder.Controls.CheckboxArrayControl();
            this.tabcustom = new System.Windows.Forms.TabPage();
            this.fieldslist = new CodeImp.DoomBuilder.Controls.FieldsEditorControl();
            this.tabLights = new System.Windows.Forms.TabPage();
            this.flatSelectorControl2 = new CodeImp.DoomBuilder.Controls.FlatSelectorControl();
            this.flatSelectorControl1 = new CodeImp.DoomBuilder.Controls.FlatSelectorControl();
            label1 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            taglabel = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            groupfloorceiling = new System.Windows.Forms.GroupBox();
            label5 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            groupBox1 = new System.Windows.Forms.GroupBox();
            groupBox2 = new System.Windows.Forms.GroupBox();
            groupBox3 = new System.Windows.Forms.GroupBox();
            groupfloorceiling.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            this.groupaction.SuspendLayout();
            this.groupeffect.SuspendLayout();
            this.tabs.SuspendLayout();
            this.tabproperties.SuspendLayout();
            this.settingsgroup.SuspendLayout();
            this.tabcustom.SuspendLayout();
            this.tabLights.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.Location = new System.Drawing.Point(271, 18);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(83, 16);
            label1.TabIndex = 15;
            label1.Text = "Floor";
            label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label3
            // 
            label3.Location = new System.Drawing.Point(363, 18);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(83, 16);
            label3.TabIndex = 14;
            label3.Text = "Ceiling";
            label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // taglabel
            // 
            taglabel.AutoSize = true;
            taglabel.Location = new System.Drawing.Point(55, 31);
            taglabel.Name = "taglabel";
            taglabel.Size = new System.Drawing.Size(28, 14);
            taglabel.TabIndex = 9;
            taglabel.Text = "Tag:";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(38, 31);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(45, 14);
            label8.TabIndex = 0;
            label8.Text = "Special:";
            // 
            // groupfloorceiling
            // 
            groupfloorceiling.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            groupfloorceiling.Controls.Add(this.floorheight);
            groupfloorceiling.Controls.Add(this.ceilingheight);
            groupfloorceiling.Controls.Add(this.sectorheight);
            groupfloorceiling.Controls.Add(this.sectorheightlabel);
            groupfloorceiling.Controls.Add(label5);
            groupfloorceiling.Controls.Add(label2);
            groupfloorceiling.Controls.Add(label4);
            groupfloorceiling.Controls.Add(this.floortex);
            groupfloorceiling.Controls.Add(this.ceilingtex);
            groupfloorceiling.Controls.Add(label6);
            groupfloorceiling.Location = new System.Drawing.Point(7, 6);
            groupfloorceiling.Name = "groupfloorceiling";
            groupfloorceiling.Size = new System.Drawing.Size(436, 161);
            groupfloorceiling.TabIndex = 0;
            groupfloorceiling.TabStop = false;
            groupfloorceiling.Text = "Floor and Ceiling ";
            // 
            // floorheight
            // 
            this.floorheight.AllowDecimal = false;
            this.floorheight.AllowNegative = true;
            this.floorheight.AllowRelative = true;
            this.floorheight.ButtonStep = 8;
            this.floorheight.Location = new System.Drawing.Point(112, 69);
            this.floorheight.Name = "floorheight";
            this.floorheight.Size = new System.Drawing.Size(88, 24);
            this.floorheight.StepValues = null;
            this.floorheight.TabIndex = 23;
            this.floorheight.WhenTextChanged += new System.EventHandler(this.floorheight_TextChanged);
            // 
            // ceilingheight
            // 
            this.ceilingheight.AllowDecimal = false;
            this.ceilingheight.AllowNegative = true;
            this.ceilingheight.AllowRelative = true;
            this.ceilingheight.ButtonStep = 8;
            this.ceilingheight.Location = new System.Drawing.Point(112, 35);
            this.ceilingheight.Name = "ceilingheight";
            this.ceilingheight.Size = new System.Drawing.Size(88, 24);
            this.ceilingheight.StepValues = null;
            this.ceilingheight.TabIndex = 22;
            this.ceilingheight.WhenTextChanged += new System.EventHandler(this.ceilingheight_TextChanged);
            // 
            // sectorheight
            // 
            this.sectorheight.AutoSize = true;
            this.sectorheight.Location = new System.Drawing.Point(113, 109);
            this.sectorheight.Name = "sectorheight";
            this.sectorheight.Size = new System.Drawing.Size(13, 14);
            this.sectorheight.TabIndex = 21;
            this.sectorheight.Text = "0";
            // 
            // sectorheightlabel
            // 
            this.sectorheightlabel.AutoSize = true;
            this.sectorheightlabel.Location = new System.Drawing.Point(32, 109);
            this.sectorheightlabel.Name = "sectorheightlabel";
            this.sectorheightlabel.Size = new System.Drawing.Size(74, 14);
            this.sectorheightlabel.TabIndex = 20;
            this.sectorheightlabel.Text = "Sector height:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new System.Drawing.Point(40, 74);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(66, 14);
            label5.TabIndex = 17;
            label5.Text = "Floor height:";
            // 
            // label2
            // 
            label2.Location = new System.Drawing.Point(237, 18);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(83, 16);
            label2.TabIndex = 15;
            label2.Text = "Floor";
            label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label4
            // 
            label4.Location = new System.Drawing.Point(332, 18);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(83, 16);
            label4.TabIndex = 14;
            label4.Text = "Ceiling";
            label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // floortex
            // 
            this.floortex.Location = new System.Drawing.Point(237, 37);
            this.floortex.Name = "floortex";
            this.floortex.Size = new System.Drawing.Size(83, 105);
            this.floortex.TabIndex = 2;
            this.floortex.TextureName = "";
            // 
            // ceilingtex
            // 
            this.ceilingtex.Location = new System.Drawing.Point(332, 37);
            this.ceilingtex.Name = "ceilingtex";
            this.ceilingtex.Size = new System.Drawing.Size(83, 105);
            this.ceilingtex.TabIndex = 3;
            this.ceilingtex.TextureName = "";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(33, 40);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(73, 14);
            label6.TabIndex = 19;
            label6.Text = "Ceiling height:";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(this.floorcolor);
            groupBox1.Controls.Add(this.lowercolor);
            groupBox1.Controls.Add(this.thingcolor);
            groupBox1.Controls.Add(this.topcolor);
            groupBox1.Controls.Add(this.ceilingcolor);
            groupBox1.Location = new System.Drawing.Point(17, 20);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(190, 198);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Colored Lighting Info";
            // 
            // floorcolor
            // 
            this.floorcolor.BackColor = System.Drawing.Color.Transparent;
            this.floorcolor.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.floorcolor.Label = "Floor:";
            this.floorcolor.Location = new System.Drawing.Point(16, 151);
            this.floorcolor.MaximumSize = new System.Drawing.Size(10000, 23);
            this.floorcolor.MinimumSize = new System.Drawing.Size(100, 23);
            this.floorcolor.Name = "floorcolor";
            this.floorcolor.Size = new System.Drawing.Size(150, 23);
            this.floorcolor.TabIndex = 6;
            // 
            // lowercolor
            // 
            this.lowercolor.BackColor = System.Drawing.Color.Transparent;
            this.lowercolor.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lowercolor.Label = "Bottom Half Wall:";
            this.lowercolor.Location = new System.Drawing.Point(16, 122);
            this.lowercolor.MaximumSize = new System.Drawing.Size(10000, 23);
            this.lowercolor.MinimumSize = new System.Drawing.Size(100, 23);
            this.lowercolor.Name = "lowercolor";
            this.lowercolor.Size = new System.Drawing.Size(150, 23);
            this.lowercolor.TabIndex = 5;
            // 
            // thingcolor
            // 
            this.thingcolor.BackColor = System.Drawing.Color.Transparent;
            this.thingcolor.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.thingcolor.Label = "Thing:";
            this.thingcolor.Location = new System.Drawing.Point(16, 93);
            this.thingcolor.MaximumSize = new System.Drawing.Size(10000, 23);
            this.thingcolor.MinimumSize = new System.Drawing.Size(100, 23);
            this.thingcolor.Name = "thingcolor";
            this.thingcolor.Size = new System.Drawing.Size(150, 23);
            this.thingcolor.TabIndex = 4;
            // 
            // topcolor
            // 
            this.topcolor.BackColor = System.Drawing.Color.Transparent;
            this.topcolor.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.topcolor.Label = "Top Half Wall:";
            this.topcolor.Location = new System.Drawing.Point(16, 64);
            this.topcolor.MaximumSize = new System.Drawing.Size(10000, 23);
            this.topcolor.MinimumSize = new System.Drawing.Size(100, 23);
            this.topcolor.Name = "topcolor";
            this.topcolor.Size = new System.Drawing.Size(150, 23);
            this.topcolor.TabIndex = 3;
            // 
            // ceilingcolor
            // 
            this.ceilingcolor.BackColor = System.Drawing.Color.Transparent;
            this.ceilingcolor.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ceilingcolor.Label = "Ceiling:";
            this.ceilingcolor.Location = new System.Drawing.Point(16, 35);
            this.ceilingcolor.MaximumSize = new System.Drawing.Size(10000, 23);
            this.ceilingcolor.MinimumSize = new System.Drawing.Size(100, 23);
            this.ceilingcolor.Name = "ceilingcolor";
            this.ceilingcolor.Size = new System.Drawing.Size(150, 23);
            this.ceilingcolor.TabIndex = 2;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(this.button14);
            groupBox2.Controls.Add(this.button15);
            groupBox2.Controls.Add(this.button12);
            groupBox2.Controls.Add(this.button13);
            groupBox2.Controls.Add(this.button10);
            groupBox2.Controls.Add(this.button11);
            groupBox2.Controls.Add(this.button8);
            groupBox2.Controls.Add(this.button9);
            groupBox2.Controls.Add(this.button7);
            groupBox2.Controls.Add(this.button6);
            groupBox2.Location = new System.Drawing.Point(213, 20);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(98, 198);
            groupBox2.TabIndex = 3;
            groupBox2.TabStop = false;
            groupBox2.Text = "Copy/Paste";
            // 
            // button14
            // 
            this.button14.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button14.Image = global::CodeImp.DoomBuilder.Properties.Resources.Paste;
            this.button14.Location = new System.Drawing.Point(54, 151);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(20, 20);
            this.button14.TabIndex = 60;
            this.button14.UseVisualStyleBackColor = true;
            this.button14.Click += new System.EventHandler(this.button14_Click_1);
            // 
            // button15
            // 
            this.button15.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button15.Image = global::CodeImp.DoomBuilder.Properties.Resources.Copy;
            this.button15.Location = new System.Drawing.Point(23, 151);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(20, 20);
            this.button15.TabIndex = 59;
            this.button15.UseVisualStyleBackColor = true;
            this.button15.Click += new System.EventHandler(this.button15_Click_1);
            // 
            // button12
            // 
            this.button12.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button12.Image = global::CodeImp.DoomBuilder.Properties.Resources.Paste;
            this.button12.Location = new System.Drawing.Point(54, 122);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(20, 20);
            this.button12.TabIndex = 58;
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.button12_Click_1);
            // 
            // button13
            // 
            this.button13.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button13.Image = global::CodeImp.DoomBuilder.Properties.Resources.Copy;
            this.button13.Location = new System.Drawing.Point(23, 122);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(20, 20);
            this.button13.TabIndex = 57;
            this.button13.UseVisualStyleBackColor = true;
            this.button13.Click += new System.EventHandler(this.button13_Click_1);
            // 
            // button10
            // 
            this.button10.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button10.Image = global::CodeImp.DoomBuilder.Properties.Resources.Paste;
            this.button10.Location = new System.Drawing.Point(54, 93);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(20, 20);
            this.button10.TabIndex = 56;
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click_1);
            // 
            // button11
            // 
            this.button11.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button11.Image = global::CodeImp.DoomBuilder.Properties.Resources.Copy;
            this.button11.Location = new System.Drawing.Point(23, 93);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(20, 20);
            this.button11.TabIndex = 55;
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.button11_Click_1);
            // 
            // button8
            // 
            this.button8.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button8.Image = global::CodeImp.DoomBuilder.Properties.Resources.Paste;
            this.button8.Location = new System.Drawing.Point(54, 64);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(20, 20);
            this.button8.TabIndex = 54;
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click_1);
            // 
            // button9
            // 
            this.button9.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button9.Image = global::CodeImp.DoomBuilder.Properties.Resources.Copy;
            this.button9.Location = new System.Drawing.Point(23, 64);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(20, 20);
            this.button9.TabIndex = 53;
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click_1);
            // 
            // button7
            // 
            this.button7.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button7.Image = global::CodeImp.DoomBuilder.Properties.Resources.Paste;
            this.button7.Location = new System.Drawing.Point(54, 35);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(20, 20);
            this.button7.TabIndex = 52;
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click_1);
            // 
            // button6
            // 
            this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button6.Image = global::CodeImp.DoomBuilder.Properties.Resources.Copy;
            this.button6.Location = new System.Drawing.Point(23, 35);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(20, 20);
            this.button6.TabIndex = 51;
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click_1);
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(this.button19);
            groupBox3.Controls.Add(this.button20);
            groupBox3.Controls.Add(this.button17);
            groupBox3.Controls.Add(this.button18);
            groupBox3.Controls.Add(this.button5);
            groupBox3.Controls.Add(this.button16);
            groupBox3.Controls.Add(this.button3);
            groupBox3.Controls.Add(this.button4);
            groupBox3.Controls.Add(this.button2);
            groupBox3.Controls.Add(this.button1);
            groupBox3.Location = new System.Drawing.Point(317, 20);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new System.Drawing.Size(112, 198);
            groupBox3.TabIndex = 4;
            groupBox3.TabStop = false;
            groupBox3.Text = "Inc/Dec Intensity";
            // 
            // button19
            // 
            this.button19.Location = new System.Drawing.Point(59, 151);
            this.button19.Name = "button19";
            this.button19.Size = new System.Drawing.Size(42, 20);
            this.button19.TabIndex = 61;
            this.button19.Text = "-";
            this.button19.UseVisualStyleBackColor = true;
            this.button19.Click += new System.EventHandler(this.button19_Click);
            // 
            // button20
            // 
            this.button20.Location = new System.Drawing.Point(11, 151);
            this.button20.Name = "button20";
            this.button20.Size = new System.Drawing.Size(42, 20);
            this.button20.TabIndex = 60;
            this.button20.Text = "+";
            this.button20.UseVisualStyleBackColor = true;
            this.button20.Click += new System.EventHandler(this.button20_Click);
            // 
            // button17
            // 
            this.button17.Location = new System.Drawing.Point(59, 122);
            this.button17.Name = "button17";
            this.button17.Size = new System.Drawing.Size(42, 20);
            this.button17.TabIndex = 59;
            this.button17.Text = "-";
            this.button17.UseVisualStyleBackColor = true;
            this.button17.Click += new System.EventHandler(this.button17_Click);
            // 
            // button18
            // 
            this.button18.Location = new System.Drawing.Point(11, 122);
            this.button18.Name = "button18";
            this.button18.Size = new System.Drawing.Size(42, 20);
            this.button18.TabIndex = 58;
            this.button18.Text = "+";
            this.button18.UseVisualStyleBackColor = true;
            this.button18.Click += new System.EventHandler(this.button18_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(59, 93);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(42, 20);
            this.button5.TabIndex = 57;
            this.button5.Text = "-";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button16
            // 
            this.button16.Location = new System.Drawing.Point(11, 93);
            this.button16.Name = "button16";
            this.button16.Size = new System.Drawing.Size(42, 20);
            this.button16.TabIndex = 56;
            this.button16.Text = "+";
            this.button16.UseVisualStyleBackColor = true;
            this.button16.Click += new System.EventHandler(this.button16_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(59, 64);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(42, 20);
            this.button3.TabIndex = 55;
            this.button3.Text = "-";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(11, 64);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(42, 20);
            this.button4.TabIndex = 54;
            this.button4.Text = "+";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(59, 35);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(42, 20);
            this.button2.TabIndex = 53;
            this.button2.Text = "-";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(11, 35);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(42, 20);
            this.button1.TabIndex = 52;
            this.button1.Text = "+";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupaction
            // 
            this.groupaction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupaction.Controls.Add(this.tag);
            this.groupaction.Controls.Add(taglabel);
            this.groupaction.Controls.Add(this.newtag);
            this.groupaction.Location = new System.Drawing.Point(7, 290);
            this.groupaction.Name = "groupaction";
            this.groupaction.Size = new System.Drawing.Size(436, 71);
            this.groupaction.TabIndex = 2;
            this.groupaction.TabStop = false;
            this.groupaction.Text = " Identification ";
            // 
            // tag
            // 
            this.tag.AllowDecimal = false;
            this.tag.AllowNegative = false;
            this.tag.AllowRelative = true;
            this.tag.ButtonStep = 1;
            this.tag.Location = new System.Drawing.Point(89, 26);
            this.tag.Name = "tag";
            this.tag.Size = new System.Drawing.Size(73, 24);
            this.tag.StepValues = null;
            this.tag.TabIndex = 25;
            // 
            // newtag
            // 
            this.newtag.Location = new System.Drawing.Point(174, 27);
            this.newtag.Name = "newtag";
            this.newtag.Size = new System.Drawing.Size(76, 23);
            this.newtag.TabIndex = 1;
            this.newtag.Text = "New Tag";
            this.newtag.UseVisualStyleBackColor = true;
            this.newtag.Click += new System.EventHandler(this.newtag_Click);
            // 
            // groupeffect
            // 
            this.groupeffect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupeffect.Controls.Add(this.brightness);
            this.groupeffect.Controls.Add(this.browseeffect);
            this.groupeffect.Controls.Add(this.label9);
            this.groupeffect.Controls.Add(this.effect);
            this.groupeffect.Controls.Add(label8);
            this.groupeffect.Location = new System.Drawing.Point(7, 176);
            this.groupeffect.Name = "groupeffect";
            this.groupeffect.Size = new System.Drawing.Size(436, 105);
            this.groupeffect.TabIndex = 1;
            this.groupeffect.TabStop = false;
            this.groupeffect.Text = " Effects ";
            // 
            // brightness
            // 
            this.brightness.AllowDecimal = false;
            this.brightness.AllowNegative = false;
            this.brightness.AllowRelative = true;
            this.brightness.ButtonStep = 8;
            this.brightness.Location = new System.Drawing.Point(89, 61);
            this.brightness.Name = "brightness";
            this.brightness.Size = new System.Drawing.Size(73, 24);
            this.brightness.StepValues = null;
            this.brightness.TabIndex = 24;
            // 
            // browseeffect
            // 
            this.browseeffect.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.browseeffect.Image = global::CodeImp.DoomBuilder.Properties.Resources.treeview;
            this.browseeffect.Location = new System.Drawing.Point(385, 27);
            this.browseeffect.Name = "browseeffect";
            this.browseeffect.Padding = new System.Windows.Forms.Padding(0, 0, 1, 3);
            this.browseeffect.Size = new System.Drawing.Size(30, 23);
            this.browseeffect.TabIndex = 1;
            this.browseeffect.Text = " ";
            this.browseeffect.UseVisualStyleBackColor = true;
            this.browseeffect.Click += new System.EventHandler(this.browseeffect_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(21, 66);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(62, 14);
            this.label9.TabIndex = 2;
            this.label9.Text = "Brightness:";
            // 
            // effect
            // 
            this.effect.BackColor = System.Drawing.Color.Transparent;
            this.effect.Cursor = System.Windows.Forms.Cursors.Default;
            this.effect.Empty = false;
            this.effect.GeneralizedCategories = null;
            this.effect.Location = new System.Drawing.Point(89, 28);
            this.effect.Name = "effect";
            this.effect.Size = new System.Drawing.Size(290, 21);
            this.effect.TabIndex = 0;
            this.effect.Value = 402;
            // 
            // cancel
            // 
            this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(355, 571);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(112, 25);
            this.cancel.TabIndex = 2;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // apply
            // 
            this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.apply.Location = new System.Drawing.Point(236, 571);
            this.apply.Name = "apply";
            this.apply.Size = new System.Drawing.Size(112, 25);
            this.apply.TabIndex = 1;
            this.apply.Text = "OK";
            this.apply.UseVisualStyleBackColor = true;
            this.apply.Click += new System.EventHandler(this.apply_Click);
            // 
            // tabs
            // 
            this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabs.Controls.Add(this.tabproperties);
            this.tabs.Controls.Add(this.tabcustom);
            this.tabs.Controls.Add(this.tabLights);
            this.tabs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabs.Location = new System.Drawing.Point(10, 10);
            this.tabs.Margin = new System.Windows.Forms.Padding(1);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(457, 544);
            this.tabs.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabs.TabIndex = 0;
            // 
            // tabproperties
            // 
            this.tabproperties.Controls.Add(this.settingsgroup);
            this.tabproperties.Controls.Add(this.groupaction);
            this.tabproperties.Controls.Add(this.groupeffect);
            this.tabproperties.Controls.Add(groupfloorceiling);
            this.tabproperties.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabproperties.Location = new System.Drawing.Point(4, 23);
            this.tabproperties.Name = "tabproperties";
            this.tabproperties.Padding = new System.Windows.Forms.Padding(3);
            this.tabproperties.Size = new System.Drawing.Size(449, 517);
            this.tabproperties.TabIndex = 0;
            this.tabproperties.Text = "Properties";
            this.tabproperties.UseVisualStyleBackColor = true;
            // 
            // settingsgroup
            // 
            this.settingsgroup.Controls.Add(this.flags);
            this.settingsgroup.Location = new System.Drawing.Point(7, 367);
            this.settingsgroup.Name = "settingsgroup";
            this.settingsgroup.Size = new System.Drawing.Size(436, 142);
            this.settingsgroup.TabIndex = 3;
            this.settingsgroup.TabStop = false;
            this.settingsgroup.Text = "Settings";
            // 
            // flags
            // 
            this.flags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.flags.AutoScroll = true;
            this.flags.Columns = 3;
            this.flags.Location = new System.Drawing.Point(6, 14);
            this.flags.Name = "flags";
            this.flags.Size = new System.Drawing.Size(424, 122);
            this.flags.TabIndex = 4;
            // 
            // tabcustom
            // 
            this.tabcustom.Controls.Add(this.fieldslist);
            this.tabcustom.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabcustom.Location = new System.Drawing.Point(4, 23);
            this.tabcustom.Name = "tabcustom";
            this.tabcustom.Padding = new System.Windows.Forms.Padding(3);
            this.tabcustom.Size = new System.Drawing.Size(449, 517);
            this.tabcustom.TabIndex = 1;
            this.tabcustom.Text = "Custom";
            this.tabcustom.UseVisualStyleBackColor = true;
            // 
            // fieldslist
            // 
            this.fieldslist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fieldslist.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.fieldslist.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fieldslist.Location = new System.Drawing.Point(11, 11);
            this.fieldslist.Margin = new System.Windows.Forms.Padding(8);
            this.fieldslist.Name = "fieldslist";
            this.fieldslist.Size = new System.Drawing.Size(427, 347);
            this.fieldslist.TabIndex = 1;
            // 
            // tabLights
            // 
            this.tabLights.Controls.Add(groupBox3);
            this.tabLights.Controls.Add(groupBox2);
            this.tabLights.Controls.Add(groupBox1);
            this.tabLights.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabLights.Location = new System.Drawing.Point(4, 23);
            this.tabLights.Name = "tabLights";
            this.tabLights.Size = new System.Drawing.Size(449, 517);
            this.tabLights.TabIndex = 2;
            this.tabLights.Text = "Lights";
            this.tabLights.UseVisualStyleBackColor = true;
            // 
            // flatSelectorControl2
            // 
            this.flatSelectorControl2.Location = new System.Drawing.Point(271, 37);
            this.flatSelectorControl2.Name = "flatSelectorControl2";
            this.flatSelectorControl2.Size = new System.Drawing.Size(83, 105);
            this.flatSelectorControl2.TabIndex = 13;
            this.flatSelectorControl2.TextureName = "";
            // 
            // flatSelectorControl1
            // 
            this.flatSelectorControl1.Location = new System.Drawing.Point(363, 37);
            this.flatSelectorControl1.Name = "flatSelectorControl1";
            this.flatSelectorControl1.Size = new System.Drawing.Size(83, 105);
            this.flatSelectorControl1.TabIndex = 12;
            this.flatSelectorControl1.TextureName = "";
            // 
            // SectorEditForm
            // 
            this.AcceptButton = this.apply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(477, 606);
            this.Controls.Add(this.tabs);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.apply);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SectorEditForm";
            this.Opacity = 0;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Sector";
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.SectorEditForm_HelpRequested);
            groupfloorceiling.ResumeLayout(false);
            groupfloorceiling.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            this.groupaction.ResumeLayout(false);
            this.groupaction.PerformLayout();
            this.groupeffect.ResumeLayout(false);
            this.groupeffect.PerformLayout();
            this.tabs.ResumeLayout(false);
            this.tabproperties.ResumeLayout(false);
            this.settingsgroup.ResumeLayout(false);
            this.tabcustom.ResumeLayout(false);
            this.tabLights.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage tabproperties;
		private System.Windows.Forms.TabPage tabcustom;
		private CodeImp.DoomBuilder.Controls.FlatSelectorControl floortex;
		private CodeImp.DoomBuilder.Controls.FlatSelectorControl ceilingtex;
		private CodeImp.DoomBuilder.Controls.FlatSelectorControl flatSelectorControl2;
		private CodeImp.DoomBuilder.Controls.FlatSelectorControl flatSelectorControl1;
		private CodeImp.DoomBuilder.Controls.FieldsEditorControl fieldslist;
		private System.Windows.Forms.Label sectorheight;
		private CodeImp.DoomBuilder.Controls.ActionSelectorControl effect;
		private System.Windows.Forms.Button newtag;
		private System.Windows.Forms.Button browseeffect;
		private System.Windows.Forms.Label sectorheightlabel;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox ceilingheight;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox floorheight;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox brightness;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox tag;
        private System.Windows.Forms.GroupBox settingsgroup;
        private CodeImp.DoomBuilder.Controls.CheckboxArrayControl flags;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.GroupBox groupeffect;
        private System.Windows.Forms.GroupBox groupaction;
        private System.Windows.Forms.TabPage tabLights;
        private CodeImp.DoomBuilder.Controls.ColorControl floorcolor;
        private CodeImp.DoomBuilder.Controls.ColorControl lowercolor;
        private CodeImp.DoomBuilder.Controls.ColorControl thingcolor;
        private CodeImp.DoomBuilder.Controls.ColorControl topcolor;
        private CodeImp.DoomBuilder.Controls.ColorControl ceilingcolor;
        private System.Windows.Forms.Button button14;
        private System.Windows.Forms.Button button15;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Button button13;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button19;
        private System.Windows.Forms.Button button20;
        private System.Windows.Forms.Button button17;
        private System.Windows.Forms.Button button18;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button16;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
	}
}
