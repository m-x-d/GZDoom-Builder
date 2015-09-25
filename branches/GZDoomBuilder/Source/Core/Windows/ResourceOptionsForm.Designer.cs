namespace CodeImp.DoomBuilder.Windows
{
	partial class ResourceOptionsForm
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
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label3;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResourceOptionsForm));
			this.tabs = new System.Windows.Forms.TabControl();
			this.wadfiletab = new System.Windows.Forms.TabPage();
			this.label6 = new System.Windows.Forms.Label();
			this.strictpatches = new System.Windows.Forms.CheckBox();
			this.browsewad = new System.Windows.Forms.Button();
			this.wadlocation = new System.Windows.Forms.TextBox();
			this.directorytab = new System.Windows.Forms.TabPage();
			this.directorylink = new System.Windows.Forms.LinkLabel();
			this.dir_flats = new System.Windows.Forms.CheckBox();
			this.dir_textures = new System.Windows.Forms.CheckBox();
			this.browsedir = new System.Windows.Forms.Button();
			this.dirlocation = new System.Windows.Forms.TextBox();
			this.pk3filetab = new System.Windows.Forms.TabPage();
			this.pk3link = new System.Windows.Forms.LinkLabel();
			this.browsepk3 = new System.Windows.Forms.Button();
			this.pk3location = new System.Windows.Forms.TextBox();
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			this.wadfiledialog = new System.Windows.Forms.OpenFileDialog();
			this.dirdialog = new System.Windows.Forms.FolderBrowserDialog();
			this.pk3filedialog = new System.Windows.Forms.OpenFileDialog();
			this.notfortesting = new System.Windows.Forms.CheckBox();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			this.tabs.SuspendLayout();
			this.wadfiletab.SuspendLayout();
			this.directorytab.SuspendLayout();
			this.pk3filetab.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(15, 20);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(104, 13);
			label1.TabIndex = 0;
			label1.Text = "WAD File Resource:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(15, 20);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(101, 13);
			label2.TabIndex = 3;
			label2.Text = "Directory Resource:";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(15, 20);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(133, 13);
			label3.TabIndex = 3;
			label3.Text = "PK3 or PK7 File Resource:";
			// 
			// tabs
			// 
			this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabs.Controls.Add(this.wadfiletab);
			this.tabs.Controls.Add(this.directorytab);
			this.tabs.Controls.Add(this.pk3filetab);
			this.tabs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.tabs.Location = new System.Drawing.Point(9, 11);
			this.tabs.Name = "tabs";
			this.tabs.Padding = new System.Drawing.Point(16, 3);
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(369, 211);
			this.tabs.TabIndex = 0;
			// 
			// wadfiletab
			// 
			this.wadfiletab.Controls.Add(this.label6);
			this.wadfiletab.Controls.Add(this.strictpatches);
			this.wadfiletab.Controls.Add(this.browsewad);
			this.wadfiletab.Controls.Add(this.wadlocation);
			this.wadfiletab.Controls.Add(label1);
			this.wadfiletab.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.wadfiletab.Location = new System.Drawing.Point(4, 23);
			this.wadfiletab.Name = "wadfiletab";
			this.wadfiletab.Padding = new System.Windows.Forms.Padding(3);
			this.wadfiletab.Size = new System.Drawing.Size(361, 184);
			this.wadfiletab.TabIndex = 0;
			this.wadfiletab.Text = "From WAD File";
			this.wadfiletab.UseVisualStyleBackColor = true;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(14, 102);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(329, 58);
			this.label6.TabIndex = 8;
			this.label6.Text = resources.GetString("label6.Text");
			// 
			// strictpatches
			// 
			this.strictpatches.AutoSize = true;
			this.strictpatches.Location = new System.Drawing.Point(17, 72);
			this.strictpatches.Name = "strictpatches";
			this.strictpatches.Size = new System.Drawing.Size(299, 17);
			this.strictpatches.TabIndex = 2;
			this.strictpatches.Text = "Strictly load patches between P_START and P_END only";
			this.strictpatches.UseVisualStyleBackColor = true;
			// 
			// browsewad
			// 
			this.browsewad.Image = global::CodeImp.DoomBuilder.Properties.Resources.Folder;
			this.browsewad.Location = new System.Drawing.Point(315, 35);
			this.browsewad.Name = "browsewad";
			this.browsewad.Padding = new System.Windows.Forms.Padding(0, 0, 1, 3);
			this.browsewad.Size = new System.Drawing.Size(28, 24);
			this.browsewad.TabIndex = 1;
			this.browsewad.Text = " ";
			this.browsewad.UseVisualStyleBackColor = true;
			this.browsewad.Click += new System.EventHandler(this.browsewad_Click);
			// 
			// wadlocation
			// 
			this.wadlocation.Location = new System.Drawing.Point(17, 37);
			this.wadlocation.Name = "wadlocation";
			this.wadlocation.ReadOnly = true;
			this.wadlocation.Size = new System.Drawing.Size(292, 20);
			this.wadlocation.TabIndex = 0;
			// 
			// directorytab
			// 
			this.directorytab.Controls.Add(this.directorylink);
			this.directorytab.Controls.Add(this.dir_flats);
			this.directorytab.Controls.Add(this.dir_textures);
			this.directorytab.Controls.Add(this.browsedir);
			this.directorytab.Controls.Add(this.dirlocation);
			this.directorytab.Controls.Add(label2);
			this.directorytab.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.directorytab.Location = new System.Drawing.Point(4, 23);
			this.directorytab.Name = "directorytab";
			this.directorytab.Padding = new System.Windows.Forms.Padding(3);
			this.directorytab.Size = new System.Drawing.Size(361, 184);
			this.directorytab.TabIndex = 1;
			this.directorytab.Text = "From Directory";
			this.directorytab.UseVisualStyleBackColor = true;
			// 
			// directorylink
			// 
			this.directorylink.LinkArea = new System.Windows.Forms.LinkArea(26, 29);
			this.directorylink.LinkColor = System.Drawing.SystemColors.HotTrack;
			this.directorylink.Location = new System.Drawing.Point(14, 127);
			this.directorylink.Name = "directorylink";
			this.directorylink.Size = new System.Drawing.Size(329, 54);
			this.directorylink.TabIndex = 9;
			this.directorylink.TabStop = true;
			this.directorylink.Text = "The directory may use the ZDoom PK3 directory structure, or you can choose to use" +
				" the options above to load texture or flat images from the directory root.";
			this.directorylink.UseCompatibleTextRendering = true;
			this.directorylink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.link_Click);
			// 
			// dir_flats
			// 
			this.dir_flats.AutoSize = true;
			this.dir_flats.Location = new System.Drawing.Point(17, 98);
			this.dir_flats.Name = "dir_flats";
			this.dir_flats.Size = new System.Drawing.Size(197, 17);
			this.dir_flats.TabIndex = 3;
			this.dir_flats.Text = "Load images in directory root as flats";
			this.dir_flats.UseVisualStyleBackColor = true;
			// 
			// dir_textures
			// 
			this.dir_textures.AutoSize = true;
			this.dir_textures.Location = new System.Drawing.Point(17, 72);
			this.dir_textures.Name = "dir_textures";
			this.dir_textures.Size = new System.Drawing.Size(215, 17);
			this.dir_textures.TabIndex = 2;
			this.dir_textures.Text = "Load images in directory root as textures";
			this.dir_textures.UseVisualStyleBackColor = true;
			// 
			// browsedir
			// 
			this.browsedir.Image = global::CodeImp.DoomBuilder.Properties.Resources.Folder;
			this.browsedir.Location = new System.Drawing.Point(315, 35);
			this.browsedir.Name = "browsedir";
			this.browsedir.Padding = new System.Windows.Forms.Padding(0, 0, 1, 3);
			this.browsedir.Size = new System.Drawing.Size(28, 24);
			this.browsedir.TabIndex = 1;
			this.browsedir.UseVisualStyleBackColor = true;
			this.browsedir.Click += new System.EventHandler(this.browsedir_Click);
			// 
			// dirlocation
			// 
			this.dirlocation.BackColor = System.Drawing.SystemColors.Control;
			this.dirlocation.Location = new System.Drawing.Point(17, 37);
			this.dirlocation.Name = "dirlocation";
			this.dirlocation.ReadOnly = true;
			this.dirlocation.Size = new System.Drawing.Size(292, 20);
			this.dirlocation.TabIndex = 0;
			// 
			// pk3filetab
			// 
			this.pk3filetab.Controls.Add(this.pk3link);
			this.pk3filetab.Controls.Add(this.browsepk3);
			this.pk3filetab.Controls.Add(this.pk3location);
			this.pk3filetab.Controls.Add(label3);
			this.pk3filetab.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.pk3filetab.Location = new System.Drawing.Point(4, 23);
			this.pk3filetab.Name = "pk3filetab";
			this.pk3filetab.Size = new System.Drawing.Size(361, 184);
			this.pk3filetab.TabIndex = 2;
			this.pk3filetab.Text = "From PK3/PK7";
			this.pk3filetab.UseVisualStyleBackColor = true;
			// 
			// pk3link
			// 
			this.pk3link.LinkArea = new System.Windows.Forms.LinkArea(40, 33);
			this.pk3link.LinkColor = System.Drawing.SystemColors.HotTrack;
			this.pk3link.Location = new System.Drawing.Point(15, 72);
			this.pk3link.Name = "pk3link";
			this.pk3link.Size = new System.Drawing.Size(328, 47);
			this.pk3link.TabIndex = 7;
			this.pk3link.TabStop = true;
			this.pk3link.Text = "The archive file is expected to use the ZDoom archive directory structure.";
			this.pk3link.UseCompatibleTextRendering = true;
			this.pk3link.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.link_Click);
			// 
			// browsepk3
			// 
			this.browsepk3.Image = global::CodeImp.DoomBuilder.Properties.Resources.Folder;
			this.browsepk3.Location = new System.Drawing.Point(315, 35);
			this.browsepk3.Name = "browsepk3";
			this.browsepk3.Padding = new System.Windows.Forms.Padding(0, 0, 1, 3);
			this.browsepk3.Size = new System.Drawing.Size(28, 24);
			this.browsepk3.TabIndex = 1;
			this.browsepk3.UseVisualStyleBackColor = true;
			this.browsepk3.Click += new System.EventHandler(this.browsepk3_Click);
			// 
			// pk3location
			// 
			this.pk3location.Location = new System.Drawing.Point(17, 37);
			this.pk3location.Name = "pk3location";
			this.pk3location.ReadOnly = true;
			this.pk3location.Size = new System.Drawing.Size(292, 20);
			this.pk3location.TabIndex = 0;
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(262, 260);
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
			this.apply.Location = new System.Drawing.Point(144, 260);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 1;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// wadfiledialog
			// 
			this.wadfiledialog.Filter = "Doom WAD Files (*.wad)|*.wad";
			this.wadfiledialog.Title = "Browse WAD File";
			// 
			// dirdialog
			// 
			this.dirdialog.Description = "Please select a directory from which to load images when editing your map...";
			// 
			// pk3filedialog
			// 
			this.pk3filedialog.Filter = "Doom PK3/PK7 Files (*.pk3;*.pk7)|*.pk3;*.pk7";
			this.pk3filedialog.Title = "Browse PK3 or PK7 File";
			// 
			// notfortesting
			// 
			this.notfortesting.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.notfortesting.AutoSize = true;
			this.notfortesting.Location = new System.Drawing.Point(12, 232);
			this.notfortesting.Name = "notfortesting";
			this.notfortesting.Size = new System.Drawing.Size(239, 17);
			this.notfortesting.TabIndex = 3;
			this.notfortesting.Text = "Exclude this resource from testing parameters";
			this.notfortesting.UseVisualStyleBackColor = true;
			// 
			// ResourceOptionsForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(386, 293);
			this.Controls.Add(this.notfortesting);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Controls.Add(this.tabs);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ResourceOptionsForm";
			this.Opacity = 1;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Resource Options";
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ResourceOptionsForm_HelpRequested);
			this.tabs.ResumeLayout(false);
			this.wadfiletab.ResumeLayout(false);
			this.wadfiletab.PerformLayout();
			this.directorytab.ResumeLayout(false);
			this.directorytab.PerformLayout();
			this.pk3filetab.ResumeLayout(false);
			this.pk3filetab.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage wadfiletab;
		private System.Windows.Forms.TabPage directorytab;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.TextBox wadlocation;
		private System.Windows.Forms.Button browsewad;
		private System.Windows.Forms.Button browsedir;
		private System.Windows.Forms.TextBox dirlocation;
		private System.Windows.Forms.CheckBox dir_flats;
		private System.Windows.Forms.CheckBox dir_textures;
		private System.Windows.Forms.OpenFileDialog wadfiledialog;
		private System.Windows.Forms.FolderBrowserDialog dirdialog;
		private System.Windows.Forms.TabPage pk3filetab;
		private System.Windows.Forms.Button browsepk3;
		private System.Windows.Forms.TextBox pk3location;
		private System.Windows.Forms.OpenFileDialog pk3filedialog;
		private System.Windows.Forms.LinkLabel pk3link;
		private System.Windows.Forms.LinkLabel directorylink;
		private System.Windows.Forms.CheckBox strictpatches;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.CheckBox notfortesting;
	}
}