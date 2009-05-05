namespace CodeImp.DoomBuilder.BuilderModes
{
	partial class PreferencesForm
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
			this.tabs = new System.Windows.Forms.TabControl();
			this.taboptions = new System.Windows.Forms.TabPage();
			this.additiveselect = new System.Windows.Forms.CheckBox();
			this.editnewsector = new System.Windows.Forms.CheckBox();
			this.editnewthing = new System.Windows.Forms.CheckBox();
			this.heightbysidedef = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tabs.SuspendLayout();
			this.taboptions.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabs
			// 
			this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabs.Controls.Add(this.taboptions);
			this.tabs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabs.Location = new System.Drawing.Point(12, 12);
			this.tabs.Name = "tabs";
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(654, 424);
			this.tabs.TabIndex = 0;
			// 
			// taboptions
			// 
			this.taboptions.Controls.Add(this.additiveselect);
			this.taboptions.Controls.Add(this.editnewsector);
			this.taboptions.Controls.Add(this.editnewthing);
			this.taboptions.Controls.Add(this.heightbysidedef);
			this.taboptions.Controls.Add(this.label1);
			this.taboptions.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.taboptions.Location = new System.Drawing.Point(4, 23);
			this.taboptions.Name = "taboptions";
			this.taboptions.Padding = new System.Windows.Forms.Padding(3);
			this.taboptions.Size = new System.Drawing.Size(646, 397);
			this.taboptions.TabIndex = 0;
			this.taboptions.Text = "Editing";
			this.taboptions.UseVisualStyleBackColor = true;
			// 
			// additiveselect
			// 
			this.additiveselect.AutoSize = true;
			this.additiveselect.Location = new System.Drawing.Point(21, 107);
			this.additiveselect.Name = "additiveselect";
			this.additiveselect.Size = new System.Drawing.Size(211, 18);
			this.additiveselect.TabIndex = 3;
			this.additiveselect.Text = "Additive selecting without holding shift";
			this.additiveselect.UseVisualStyleBackColor = true;
			// 
			// editnewsector
			// 
			this.editnewsector.AutoSize = true;
			this.editnewsector.Location = new System.Drawing.Point(21, 81);
			this.editnewsector.Name = "editnewsector";
			this.editnewsector.Size = new System.Drawing.Size(271, 18);
			this.editnewsector.TabIndex = 2;
			this.editnewsector.Text = "Edit sector properties when drawing a new sector";
			this.editnewsector.UseVisualStyleBackColor = true;
			// 
			// editnewthing
			// 
			this.editnewthing.AutoSize = true;
			this.editnewthing.Location = new System.Drawing.Point(21, 55);
			this.editnewthing.Name = "editnewthing";
			this.editnewthing.Size = new System.Drawing.Size(256, 18);
			this.editnewthing.TabIndex = 1;
			this.editnewthing.Text = "Edit thing properties when inserting a new thing";
			this.editnewthing.UseVisualStyleBackColor = true;
			// 
			// heightbysidedef
			// 
			this.heightbysidedef.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.heightbysidedef.FormattingEnabled = true;
			this.heightbysidedef.Items.AddRange(new object[] {
            "Do nothing",
            "Change the ceiling height",
            "Change the floor height",
            "Change both floor and ceiling height"});
			this.heightbysidedef.Location = new System.Drawing.Point(353, 16);
			this.heightbysidedef.Name = "heightbysidedef";
			this.heightbysidedef.Size = new System.Drawing.Size(199, 22);
			this.heightbysidedef.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(18, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(315, 14);
			this.label1.TabIndex = 0;
			this.label1.Text = "When sector height changes are used on a wall in Visual Mode:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// PreferencesForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(678, 448);
			this.Controls.Add(this.tabs);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PreferencesForm";
			this.ShowIcon = false;
			this.Text = "PreferencesForm";
			this.tabs.ResumeLayout(false);
			this.taboptions.ResumeLayout(false);
			this.taboptions.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage taboptions;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox heightbysidedef;
		private System.Windows.Forms.CheckBox editnewsector;
		private System.Windows.Forms.CheckBox editnewthing;
		private System.Windows.Forms.CheckBox additiveselect;
	}
}