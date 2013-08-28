namespace CodeImp.DoomBuilder.GZBuilder.Controls
{
	partial class TagSelector
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.label1 = new System.Windows.Forms.Label();
			this.cbTagPicker = new System.Windows.Forms.ComboBox();
			this.newTag = new System.Windows.Forms.Button();
			this.unusedTag = new System.Windows.Forms.Button();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(7, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(29, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Tag:";
			// 
			// cbTagPicker
			// 
			this.cbTagPicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.cbTagPicker.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.cbTagPicker.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this.cbTagPicker.FormattingEnabled = true;
			this.cbTagPicker.Location = new System.Drawing.Point(42, 7);
			this.cbTagPicker.Name = "cbTagPicker";
			this.cbTagPicker.Size = new System.Drawing.Size(190, 21);
			this.cbTagPicker.TabIndex = 1;
			// 
			// newTag
			// 
			this.newTag.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.newTag.Location = new System.Drawing.Point(238, 6);
			this.newTag.Name = "newTag";
			this.newTag.Size = new System.Drawing.Size(54, 23);
			this.newTag.TabIndex = 2;
			this.newTag.Text = "New";
			this.toolTip1.SetToolTip(this.newTag, "Finds a tag, which is not used as a tag or tag action argument \r\nby any map eleme" +
					"nt");
			this.newTag.UseVisualStyleBackColor = true;
			this.newTag.Click += new System.EventHandler(this.newTag_Click);
			// 
			// unusedTag
			// 
			this.unusedTag.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.unusedTag.Location = new System.Drawing.Point(298, 6);
			this.unusedTag.Name = "unusedTag";
			this.unusedTag.Size = new System.Drawing.Size(54, 23);
			this.unusedTag.TabIndex = 3;
			this.unusedTag.Text = "Unused";
			this.toolTip1.SetToolTip(this.unusedTag, "Finds a tag, which is not used as a tag \r\nby any map element of this type");
			this.unusedTag.UseVisualStyleBackColor = true;
			this.unusedTag.Click += new System.EventHandler(this.unusedTag_Click);
			// 
			// TagSelector
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.unusedTag);
			this.Controls.Add(this.newTag);
			this.Controls.Add(this.cbTagPicker);
			this.Controls.Add(this.label1);
			this.Name = "TagSelector";
			this.Size = new System.Drawing.Size(356, 35);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox cbTagPicker;
		private System.Windows.Forms.Button newTag;
		private System.Windows.Forms.Button unusedTag;
		private System.Windows.Forms.ToolTip toolTip1;
	}
}
