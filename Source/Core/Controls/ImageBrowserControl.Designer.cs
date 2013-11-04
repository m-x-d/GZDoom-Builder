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
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.cbMixMode = new System.Windows.Forms.ComboBox();
			this.texturesize = new System.Windows.Forms.Label();
			this.texturesizelabel = new System.Windows.Forms.Label();
			this.objectname = new System.Windows.Forms.TextBox();
			this.refreshtimer = new System.Windows.Forms.Timer(this.components);
			this.texturesizetimer = new System.Windows.Forms.Timer(this.components);
			this.list = new CodeImp.DoomBuilder.Controls.OptimizedListView();
			this.filterHeight = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.filterWidth = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
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
			this.labelMixMode.Size = new System.Drawing.Size(39, 14);
			this.labelMixMode.TabIndex = 3;
			this.labelMixMode.Text = "Show:";
			// 
			// label
			// 
			this.label.AutoSize = true;
			this.label.Location = new System.Drawing.Point(127, 9);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(33, 14);
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
			this.splitter.Panel2.Controls.Add(this.label2);
			this.splitter.Panel2.Controls.Add(this.filterHeight);
			this.splitter.Panel2.Controls.Add(this.label1);
			this.splitter.Panel2.Controls.Add(this.filterWidth);
			this.splitter.Panel2.Controls.Add(this.cbMixMode);
			this.splitter.Panel2.Controls.Add(this.labelMixMode);
			this.splitter.Panel2.Controls.Add(this.texturesize);
			this.splitter.Panel2.Controls.Add(this.texturesizelabel);
			this.splitter.Panel2.Controls.Add(this.objectname);
			this.splitter.Panel2.Controls.Add(this.label);
			this.splitter.Size = new System.Drawing.Size(639, 346);
			this.splitter.SplitterDistance = 312;
			this.splitter.TabIndex = 0;
			this.splitter.TabStop = false;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(336, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(40, 14);
			this.label2.TabIndex = 8;
			this.label2.Text = "Height:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(237, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(37, 14);
			this.label1.TabIndex = 6;
			this.label1.Text = "Width:";
			// 
			// cbMixMode
			// 
			this.cbMixMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbMixMode.FormattingEnabled = true;
			this.cbMixMode.Items.AddRange(new object[] {
            "All",
            "Textures",
            "Flats"});
			this.cbMixMode.Location = new System.Drawing.Point(48, 5);
			this.cbMixMode.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
			this.cbMixMode.Name = "cbMixMode";
			this.cbMixMode.Size = new System.Drawing.Size(70, 22);
			this.cbMixMode.TabIndex = 0;
			this.cbMixMode.TabStop = false;
			this.cbMixMode.SelectedIndexChanged += new System.EventHandler(this.cbMixMode_SelectedIndexChanged);
			// 
			// texturesize
			// 
			this.texturesize.Location = new System.Drawing.Point(479, 9);
			this.texturesize.Name = "texturesize";
			this.texturesize.Size = new System.Drawing.Size(100, 14);
			this.texturesize.TabIndex = 2;
			this.texturesize.Text = "1024 x 1024";
			this.texturesize.Visible = false;
			// 
			// texturesizelabel
			// 
			this.texturesizelabel.AutoSize = true;
			this.texturesizelabel.Location = new System.Drawing.Point(442, 9);
			this.texturesizelabel.Name = "texturesizelabel";
			this.texturesizelabel.Size = new System.Drawing.Size(31, 14);
			this.texturesizelabel.TabIndex = 1;
			this.texturesizelabel.Text = "Size:";
			this.texturesizelabel.Visible = false;
			// 
			// objectname
			// 
			this.objectname.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.objectname.Location = new System.Drawing.Point(163, 6);
			this.objectname.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
			this.objectname.Name = "objectname";
			this.objectname.Size = new System.Drawing.Size(69, 20);
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
			// texturesizetimer
			// 
			this.texturesizetimer.Interval = 3;
			this.texturesizetimer.Tick += new System.EventHandler(this.texturesizetimer_Tick);
			// 
			// list
			// 
			this.list.Dock = System.Windows.Forms.DockStyle.Fill;
			this.list.HideSelection = false;
			this.list.Location = new System.Drawing.Point(0, 0);
			this.list.MultiSelect = false;
			this.list.Name = "list";
			this.list.OwnerDraw = true;
			this.list.Size = new System.Drawing.Size(639, 312);
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
			// filterHeight
			// 
			this.filterHeight.AllowDecimal = false;
			this.filterHeight.AllowNegative = false;
			this.filterHeight.AllowRelative = false;
			this.filterHeight.ButtonStep = 1;
			this.filterHeight.ButtonStepFloat = 1F;
			this.filterHeight.Location = new System.Drawing.Point(380, 4);
			this.filterHeight.Name = "filterHeight";
			this.filterHeight.Size = new System.Drawing.Size(54, 24);
			this.filterHeight.StepValues = null;
			this.filterHeight.TabIndex = 0;
			this.filterHeight.WhenTextChanged += new System.EventHandler(this.filterSize_WhenTextChanged);
			// 
			// filterWidth
			// 
			this.filterWidth.AllowDecimal = false;
			this.filterWidth.AllowNegative = false;
			this.filterWidth.AllowRelative = false;
			this.filterWidth.ButtonStep = 1;
			this.filterWidth.ButtonStepFloat = 1F;
			this.filterWidth.Location = new System.Drawing.Point(278, 4);
			this.filterWidth.Name = "filterWidth";
			this.filterWidth.Size = new System.Drawing.Size(54, 24);
			this.filterWidth.StepValues = null;
			this.filterWidth.TabIndex = 0;
			this.filterWidth.WhenTextChanged += new System.EventHandler(this.filterSize_WhenTextChanged);
			// 
			// ImageBrowserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.splitter);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "ImageBrowserControl";
			this.Size = new System.Drawing.Size(639, 346);
			this.Resize += new System.EventHandler(this.ImageBrowserControl_Resize);
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
		private System.Windows.Forms.Label texturesize;
		private System.Windows.Forms.Label texturesizelabel;
		private System.Windows.Forms.Timer texturesizetimer;
		private System.Windows.Forms.ComboBox cbMixMode;
		private System.Windows.Forms.Label label;
		private System.Windows.Forms.Label labelMixMode;
		private ButtonsNumericTextbox filterWidth;
		private System.Windows.Forms.Label label2;
		private ButtonsNumericTextbox filterHeight;
		private System.Windows.Forms.Label label1;

	}
}
