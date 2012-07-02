namespace CodeImp.DoomBuilder.TagExplorer
{
    partial class TagExplorer
    {
        /// <summary> 
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Обязательный метод для поддержки конструктора - не изменяйте 
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TagExplorer));
            this.treeView = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.cbDisplayMode = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbSortMode = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbCenterOnSelected = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbCommentsOnly = new System.Windows.Forms.CheckBox();
            this.cbSelectOnClick = new System.Windows.Forms.CheckBox();
            this.labelSearch = new System.Windows.Forms.Label();
            this.btnClearSearch = new System.Windows.Forms.Button();
            this.tbSearch = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeView.HideSelection = false;
            this.treeView.ImageIndex = 0;
            this.treeView.ImageList = this.imageList1;
            this.treeView.Location = new System.Drawing.Point(3, 172);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = 0;
            this.treeView.Size = new System.Drawing.Size(266, 172);
            this.treeView.TabIndex = 0;
            this.treeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseDoubleClick);
            this.treeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView_AfterLabelEdit);
            this.treeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseClick);
            this.treeView.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView_BeforeLabelEdit);
            this.treeView.MouseLeave += new System.EventHandler(this.treeView_MouseLeave);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "ThingsGroup.png");
            this.imageList1.Images.SetKeyName(1, "Things.png");
            this.imageList1.Images.SetKeyName(2, "SectorsGroup.png");
            this.imageList1.Images.SetKeyName(3, "Sectors.png");
            this.imageList1.Images.SetKeyName(4, "LinesGroup.png");
            this.imageList1.Images.SetKeyName(5, "Lines.png");
            // 
            // cbDisplayMode
            // 
            this.cbDisplayMode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDisplayMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDisplayMode.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cbDisplayMode.Location = new System.Drawing.Point(57, 13);
            this.cbDisplayMode.Name = "cbDisplayMode";
            this.cbDisplayMode.Size = new System.Drawing.Size(203, 22);
            this.cbDisplayMode.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 14);
            this.label1.TabIndex = 3;
            this.label1.Text = "Show:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbSortMode
            // 
            this.cbSortMode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSortMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSortMode.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cbSortMode.Location = new System.Drawing.Point(57, 40);
            this.cbSortMode.Name = "cbSortMode";
            this.cbSortMode.Size = new System.Drawing.Size(203, 22);
            this.cbSortMode.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(21, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 14);
            this.label2.TabIndex = 5;
            this.label2.Text = "Sort:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbCenterOnSelected
            // 
            this.cbCenterOnSelected.AutoSize = true;
            this.cbCenterOnSelected.Location = new System.Drawing.Point(12, 94);
            this.cbCenterOnSelected.Name = "cbCenterOnSelected";
            this.cbCenterOnSelected.Size = new System.Drawing.Size(207, 18);
            this.cbCenterOnSelected.TabIndex = 6;
            this.cbCenterOnSelected.Text = "Center view on selected map element";
            this.cbCenterOnSelected.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.cbCommentsOnly);
            this.groupBox1.Controls.Add(this.cbSelectOnClick);
            this.groupBox1.Controls.Add(this.labelSearch);
            this.groupBox1.Controls.Add(this.btnClearSearch);
            this.groupBox1.Controls.Add(this.tbSearch);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cbCenterOnSelected);
            this.groupBox1.Controls.Add(this.cbDisplayMode);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cbSortMode);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(266, 163);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            // 
            // cbCommentsOnly
            // 
            this.cbCommentsOnly.AutoSize = true;
            this.cbCommentsOnly.Location = new System.Drawing.Point(12, 140);
            this.cbCommentsOnly.Name = "cbCommentsOnly";
            this.cbCommentsOnly.Size = new System.Drawing.Size(184, 18);
            this.cbCommentsOnly.TabIndex = 11;
            this.cbCommentsOnly.Text = "Hide elements without comments";
            this.cbCommentsOnly.UseVisualStyleBackColor = true;
            this.cbCommentsOnly.CheckedChanged += new System.EventHandler(this.cbCommentsOnly_CheckedChanged);
            // 
            // cbSelectOnClick
            // 
            this.cbSelectOnClick.AutoSize = true;
            this.cbSelectOnClick.Location = new System.Drawing.Point(12, 117);
            this.cbSelectOnClick.Name = "cbSelectOnClick";
            this.cbSelectOnClick.Size = new System.Drawing.Size(95, 18);
            this.cbSelectOnClick.TabIndex = 10;
            this.cbSelectOnClick.Text = "Select on click";
            this.cbSelectOnClick.UseVisualStyleBackColor = true;
            // 
            // labelSearch
            // 
            this.labelSearch.AutoSize = true;
            this.labelSearch.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelSearch.Location = new System.Drawing.Point(18, 71);
            this.labelSearch.Name = "labelSearch";
            this.labelSearch.Size = new System.Drawing.Size(33, 14);
            this.labelSearch.TabIndex = 9;
            this.labelSearch.Text = "Filter:";
            this.labelSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnClearSearch
            // 
            this.btnClearSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearSearch.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnClearSearch.Image = global::CodeImp.DoomBuilder.TagExplorer.Properties.Resources.SearchClear;
            this.btnClearSearch.Location = new System.Drawing.Point(236, 65);
            this.btnClearSearch.Name = "btnClearSearch";
            this.btnClearSearch.Size = new System.Drawing.Size(24, 24);
            this.btnClearSearch.TabIndex = 8;
            this.toolTip1.SetToolTip(this.btnClearSearch, "Clear Search");
            this.btnClearSearch.UseVisualStyleBackColor = true;
            this.btnClearSearch.Click += new System.EventHandler(this.btnClearSearch_Click);
            // 
            // tbSearch
            // 
            this.tbSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSearch.Location = new System.Drawing.Point(57, 68);
            this.tbSearch.Name = "tbSearch";
            this.tbSearch.Size = new System.Drawing.Size(175, 20);
            this.tbSearch.TabIndex = 7;
            this.tbSearch.TextChanged += new System.EventHandler(this.tbSearch_TextChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Location = new System.Drawing.Point(3, 344);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(266, 12);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            // 
            // TagExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.treeView);
            this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Name = "TagExplorer";
            this.Size = new System.Drawing.Size(272, 359);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.ComboBox cbDisplayMode;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbSortMode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.CheckBox cbCenterOnSelected;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label labelSearch;
        private System.Windows.Forms.Button btnClearSearch;
        private System.Windows.Forms.TextBox tbSearch;
        private System.Windows.Forms.CheckBox cbSelectOnClick;
        private System.Windows.Forms.CheckBox cbCommentsOnly;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}
