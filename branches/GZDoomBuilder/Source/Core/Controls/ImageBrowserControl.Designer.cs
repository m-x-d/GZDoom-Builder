namespace CodeImp.DoomBuilder.Controls
{
	partial class ImageBrowserControl
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
				CleanUp();
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
			this.labelMixMode = new System.Windows.Forms.Label();
			this.label = new System.Windows.Forms.Label();
			this.splitter = new System.Windows.Forms.SplitContainer();
			this.list = new CodeImp.DoomBuilder.Controls.OptimizedListView();
			this.showtexturesize = new System.Windows.Forms.CheckBox();
			this.longtexturenames = new System.Windows.Forms.CheckBox();
			this.filterheightlabel = new System.Windows.Forms.Label();
			this.filterHeight = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.filterwidthlabel = new System.Windows.Forms.Label();
			this.filterWidth = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.cbMixMode = new System.Windows.Forms.ComboBox();
			this.objectname = new System.Windows.Forms.TextBox();
			this.refreshtimer = new System.Windows.Forms.Timer(this.components);
			this.splitter.Panel1.SuspendLayout();
			this.splitter.Panel2.SuspendLayout();
			this.splitter.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelMixMode
			// 
			this.labelMixMode.AutoSize = true;
			this.labelMixMode.Location = new System.Drawing.Point(3, 9);
			this.labelMixMode.Name = "labelMixMode";
			this.labelMixMode.Size = new System.Drawing.Size(37, 13);
			this.labelMixMode.TabIndex = 0;
			this.labelMixMode.Text = "Show:";
			// 
			// label
			// 
			this.label.AutoSize = true;
			this.label.Location = new System.Drawing.Point(131, 9);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(32, 13);
			this.label.TabIndex = 0;
			this.label.Text = "Filter:";
			// 
			// splitter
			// 
			this.splitter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitter.IsSplitterFixed = true;
			this.splitter.Location = new System.Drawing.Point(0, 0);
			this.splitter.Name = "splitter";
			this.splitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitter.Panel1
			// 
			this.splitter.Panel1.Controls.Add(this.list);
			// 
			// splitter.Panel2
			// 
			this.splitter.Panel2.Controls.Add(this.showtexturesize);
			this.splitter.Panel2.Controls.Add(this.longtexturenames);
			this.splitter.Panel2.Controls.Add(this.filterheightlabel);
			this.splitter.Panel2.Controls.Add(this.filterHeight);
			this.splitter.Panel2.Controls.Add(this.filterwidthlabel);
			this.splitter.Panel2.Controls.Add(this.filterWidth);
			this.splitter.Panel2.Controls.Add(this.cbMixMode);
			this.splitter.Panel2.Controls.Add(this.labelMixMode);
			this.splitter.Panel2.Controls.Add(this.objectname);
			this.splitter.Panel2.Controls.Add(this.label);
			this.splitter.Size = new System.Drawing.Size(840, 346);
			this.splitter.SplitterDistance = 312;
			this.splitter.TabIndex = 0;
			this.splitter.TabStop = false;
			// 
			// list
			// 
			this.list.Dock = System.Windows.Forms.DockStyle.Fill;
			this.list.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.list.HideSelection = false;
			this.list.Location = new System.Drawing.Point(0, 0);
			this.list.MultiSelect = false;
			this.list.Name = "list";
			this.list.OwnerDraw = true;
			this.list.ShowItemToolTips = true;
			this.list.Size = new System.Drawing.Size(840, 312);
			this.list.TabIndex = 1;
			this.list.TabStop = false;
			this.list.TileSize = new System.Drawing.Size(90, 90);
			this.list.UseCompatibleStateImageBehavior = false;
			this.list.View = System.Windows.Forms.View.Tile;
			this.list.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.list_DrawItem);
			this.list.DoubleClick += new System.EventHandler(this.list_DoubleClick);
			this.list.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.list_ItemSelectionChanged);
			this.list.KeyDown += new System.Windows.Forms.KeyEventHandler(this.list_KeyDown);
			// 
			// showtexturesize
			// 
			this.showtexturesize.AutoSize = true;
			this.showtexturesize.Location = new System.Drawing.Point(470, 9);
			this.showtexturesize.Name = "showtexturesize";
			this.showtexturesize.Size = new System.Drawing.Size(105, 17);
			this.showtexturesize.TabIndex = 0;
			this.showtexturesize.Text = "Show image size";
			this.showtexturesize.UseVisualStyleBackColor = true;
			this.showtexturesize.CheckedChanged += new System.EventHandler(this.showtexturesize_CheckedChanged);
			// 
			// longtexturenames
			// 
			this.longtexturenames.AutoSize = true;
			this.longtexturenames.Location = new System.Drawing.Point(585, 9);
			this.longtexturenames.Name = "longtexturenames";
			this.longtexturenames.Size = new System.Drawing.Size(119, 17);
			this.longtexturenames.TabIndex = 0;
			this.longtexturenames.Text = "Long texture names";
			this.longtexturenames.UseVisualStyleBackColor = true;
			this.longtexturenames.CheckedChanged += new System.EventHandler(this.longtexturenames_CheckedChanged);
			// 
			// filterheightlabel
			// 
			this.filterheightlabel.AutoSize = true;
			this.filterheightlabel.Location = new System.Drawing.Point(367, 9);
			this.filterheightlabel.Name = "filterheightlabel";
			this.filterheightlabel.Size = new System.Drawing.Size(41, 13);
			this.filterheightlabel.TabIndex = 0;
			this.filterheightlabel.Text = "Height:";
			// 
			// filterHeight
			// 
			this.filterHeight.AllowDecimal = false;
			this.filterHeight.AllowNegative = false;
			this.filterHeight.AllowRelative = false;
			this.filterHeight.ButtonStep = 1;
			this.filterHeight.ButtonStepFloat = 1F;
			this.filterHeight.ButtonStepsWrapAround = false;
			this.filterHeight.Location = new System.Drawing.Point(410, 4);
			this.filterHeight.Name = "filterHeight";
			this.filterHeight.Size = new System.Drawing.Size(54, 24);
			this.filterHeight.StepValues = null;
			this.filterHeight.TabIndex = 0;
			this.filterHeight.TabStop = false;
			this.filterHeight.WhenTextChanged += new System.EventHandler(this.filterSize_WhenTextChanged);
			// 
			// filterwidthlabel
			// 
			this.filterwidthlabel.AutoSize = true;
			this.filterwidthlabel.Location = new System.Drawing.Point(268, 9);
			this.filterwidthlabel.Name = "filterwidthlabel";
			this.filterwidthlabel.Size = new System.Drawing.Size(38, 13);
			this.filterwidthlabel.TabIndex = 0;
			this.filterwidthlabel.Text = "Width:";
			// 
			// filterWidth
			// 
			this.filterWidth.AllowDecimal = false;
			this.filterWidth.AllowNegative = false;
			this.filterWidth.AllowRelative = false;
			this.filterWidth.ButtonStep = 1;
			this.filterWidth.ButtonStepFloat = 1F;
			this.filterWidth.ButtonStepsWrapAround = false;
			this.filterWidth.Location = new System.Drawing.Point(308, 4);
			this.filterWidth.Name = "filterWidth";
			this.filterWidth.Size = new System.Drawing.Size(54, 24);
			this.filterWidth.StepValues = null;
			this.filterWidth.TabIndex = 0;
			this.filterWidth.TabStop = false;
			this.filterWidth.WhenTextChanged += new System.EventHandler(this.filterSize_WhenTextChanged);
			// 
			// cbMixMode
			// 
			this.cbMixMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbMixMode.FormattingEnabled = true;
			this.cbMixMode.Items.AddRange(new object[] {
            "All",
            "Textures",
            "Flats",
            "By sel. type"});
			this.cbMixMode.Location = new System.Drawing.Point(43, 5);
			this.cbMixMode.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
			this.cbMixMode.Name = "cbMixMode";
			this.cbMixMode.Size = new System.Drawing.Size(80, 21);
			this.cbMixMode.TabIndex = 0;
			this.cbMixMode.TabStop = false;
			this.cbMixMode.SelectedIndexChanged += new System.EventHandler(this.cbMixMode_SelectedIndexChanged);
			// 
			// objectname
			// 
			this.objectname.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.objectname.Location = new System.Drawing.Point(166, 6);
			this.objectname.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
			this.objectname.Name = "objectname";
			this.objectname.Size = new System.Drawing.Size(94, 20);
			this.objectname.TabIndex = 0;
			this.objectname.TabStop = false;
			this.objectname.TextChanged += new System.EventHandler(this.objectname_TextChanged);
			this.objectname.KeyDown += new System.Windows.Forms.KeyEventHandler(this.objectname_KeyDown);
			// 
			// refreshtimer
			// 
			this.refreshtimer.Interval = 500;
			this.refreshtimer.Tick += new System.EventHandler(this.refreshtimer_Tick);
			// 
			// ImageBrowserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.splitter);
			this.Name = "ImageBrowserControl";
			this.Size = new System.Drawing.Size(840, 346);
			this.splitter.Panel1.ResumeLayout(false);
			this.splitter.Panel2.ResumeLayout(false);
			this.splitter.Panel2.PerformLayout();
			this.splitter.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitter;
		private OptimizedListView list;
		private System.Windows.Forms.Timer refreshtimer;
		private System.Windows.Forms.TextBox objectname;
		private System.Windows.Forms.ComboBox cbMixMode;
		private System.Windows.Forms.Label label;
		private System.Windows.Forms.Label labelMixMode;
		private ButtonsNumericTextbox filterWidth;
		private System.Windows.Forms.Label filterheightlabel;
		private ButtonsNumericTextbox filterHeight;
		private System.Windows.Forms.Label filterwidthlabel;
		private System.Windows.Forms.CheckBox longtexturenames;
		private System.Windows.Forms.CheckBox showtexturesize;

	}
}
