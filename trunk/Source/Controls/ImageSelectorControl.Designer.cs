namespace CodeImp.DoomBuilder.Controls
{
	partial class ImageSelectorControl
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
			this.preview = new System.Windows.Forms.Panel();
			this.name = new CodeImp.DoomBuilder.Controls.AutoSelectTextbox();
			this.SuspendLayout();
			// 
			// preview
			// 
			this.preview.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.preview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
			this.preview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.preview.Location = new System.Drawing.Point(0, 0);
			this.preview.Name = "preview";
			this.preview.Size = new System.Drawing.Size(68, 60);
			this.preview.TabIndex = 1;
			this.preview.Click += new System.EventHandler(this.preview_Click);
			// 
			// name
			// 
			this.name.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.name.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
			this.name.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.name.Location = new System.Drawing.Point(0, 64);
			this.name.Name = "name";
			this.name.Size = new System.Drawing.Size(68, 20);
			this.name.TabIndex = 2;
			this.name.TextChanged += new System.EventHandler(this.name_TextChanged);
			// 
			// ImageSelectorControl
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.name);
			this.Controls.Add(this.preview);
			this.Name = "ImageSelectorControl";
			this.Size = new System.Drawing.Size(115, 136);
			this.Layout += new System.Windows.Forms.LayoutEventHandler(this.ImageSelectorControl_Layout);
			this.Resize += new System.EventHandler(this.ImageSelectorControl_Resize);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		protected System.Windows.Forms.Panel preview;
		protected CodeImp.DoomBuilder.Controls.AutoSelectTextbox name;

	}
}
