namespace CodeImp.DoomBuilder.Interface
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
			this.tabs = new System.Windows.Forms.TabControl();
			this.wadfiletab = new System.Windows.Forms.TabPage();
			this.browsewad = new System.Windows.Forms.Button();
			this.wadlocation = new System.Windows.Forms.TextBox();
			this.directorytab = new System.Windows.Forms.TabPage();
			this.dir_flats = new System.Windows.Forms.CheckBox();
			this.dir_textures = new System.Windows.Forms.CheckBox();
			this.browsedir = new System.Windows.Forms.Button();
			this.dirlocation = new System.Windows.Forms.TextBox();
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			this.wadfiledialog = new System.Windows.Forms.OpenFileDialog();
			this.dirdialog = new System.Windows.Forms.FolderBrowserDialog();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			this.tabs.SuspendLayout();
			this.wadfiletab.SuspendLayout();
			this.directorytab.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(15, 20);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(104, 14);
			label1.TabIndex = 0;
			label1.Text = "WAD File Resource:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(15, 75);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(104, 14);
			label2.TabIndex = 3;
			label2.Text = "Directory Resource:";
			// 
			// tabs
			// 
			this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabs.Controls.Add(this.wadfiletab);
			this.tabs.Controls.Add(this.directorytab);
			this.tabs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabs.ItemSize = new System.Drawing.Size(110, 19);
			this.tabs.Location = new System.Drawing.Point(12, 12);
			this.tabs.Name = "tabs";
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(353, 161);
			this.tabs.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.tabs.TabIndex = 0;
			// 
			// wadfiletab
			// 
			this.wadfiletab.Controls.Add(this.browsewad);
			this.wadfiletab.Controls.Add(this.wadlocation);
			this.wadfiletab.Controls.Add(label1);
			this.wadfiletab.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.wadfiletab.Location = new System.Drawing.Point(4, 23);
			this.wadfiletab.Name = "wadfiletab";
			this.wadfiletab.Padding = new System.Windows.Forms.Padding(3);
			this.wadfiletab.Size = new System.Drawing.Size(345, 134);
			this.wadfiletab.TabIndex = 0;
			this.wadfiletab.Text = "From WAD File";
			this.wadfiletab.UseVisualStyleBackColor = true;
			// 
			// browsewad
			// 
			this.browsewad.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.browsewad.Location = new System.Drawing.Point(296, 36);
			this.browsewad.Name = "browsewad";
			this.browsewad.Size = new System.Drawing.Size(30, 23);
			this.browsewad.TabIndex = 2;
			this.browsewad.Text = "...";
			this.browsewad.UseVisualStyleBackColor = true;
			this.browsewad.Click += new System.EventHandler(this.browsewad_Click);
			// 
			// wadlocation
			// 
			this.wadlocation.Location = new System.Drawing.Point(17, 37);
			this.wadlocation.Name = "wadlocation";
			this.wadlocation.ReadOnly = true;
			this.wadlocation.Size = new System.Drawing.Size(273, 20);
			this.wadlocation.TabIndex = 1;
			// 
			// directorytab
			// 
			this.directorytab.Controls.Add(this.dir_flats);
			this.directorytab.Controls.Add(this.dir_textures);
			this.directorytab.Controls.Add(this.browsedir);
			this.directorytab.Controls.Add(this.dirlocation);
			this.directorytab.Controls.Add(label2);
			this.directorytab.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.directorytab.Location = new System.Drawing.Point(4, 23);
			this.directorytab.Name = "directorytab";
			this.directorytab.Padding = new System.Windows.Forms.Padding(3);
			this.directorytab.Size = new System.Drawing.Size(345, 134);
			this.directorytab.TabIndex = 1;
			this.directorytab.Text = "From Directory";
			this.directorytab.UseVisualStyleBackColor = true;
			// 
			// dir_flats
			// 
			this.dir_flats.AutoSize = true;
			this.dir_flats.Location = new System.Drawing.Point(17, 45);
			this.dir_flats.Name = "dir_flats";
			this.dir_flats.Size = new System.Drawing.Size(126, 18);
			this.dir_flats.TabIndex = 7;
			this.dir_flats.Text = "Load images as flats";
			this.dir_flats.UseVisualStyleBackColor = true;
			// 
			// dir_textures
			// 
			this.dir_textures.AutoSize = true;
			this.dir_textures.Location = new System.Drawing.Point(17, 21);
			this.dir_textures.Name = "dir_textures";
			this.dir_textures.Size = new System.Drawing.Size(145, 18);
			this.dir_textures.TabIndex = 6;
			this.dir_textures.Text = "Load images as textures";
			this.dir_textures.UseVisualStyleBackColor = true;
			// 
			// browsedir
			// 
			this.browsedir.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.browsedir.Location = new System.Drawing.Point(296, 91);
			this.browsedir.Name = "browsedir";
			this.browsedir.Size = new System.Drawing.Size(30, 23);
			this.browsedir.TabIndex = 5;
			this.browsedir.Text = "...";
			this.browsedir.UseVisualStyleBackColor = true;
			this.browsedir.Click += new System.EventHandler(this.browsedir_Click);
			// 
			// dirlocation
			// 
			this.dirlocation.BackColor = System.Drawing.SystemColors.Control;
			this.dirlocation.Location = new System.Drawing.Point(17, 92);
			this.dirlocation.Name = "dirlocation";
			this.dirlocation.ReadOnly = true;
			this.dirlocation.Size = new System.Drawing.Size(273, 20);
			this.dirlocation.TabIndex = 4;
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(253, 189);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(112, 25);
			this.cancel.TabIndex = 15;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// apply
			// 
			this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.apply.Location = new System.Drawing.Point(135, 189);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 14;
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
			// ResourceOptionsForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(377, 226);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Controls.Add(this.tabs);
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ResourceOptionsForm";
			this.Opacity = 0;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Resource Options";
			this.tabs.ResumeLayout(false);
			this.wadfiletab.ResumeLayout(false);
			this.wadfiletab.PerformLayout();
			this.directorytab.ResumeLayout(false);
			this.directorytab.PerformLayout();
			this.ResumeLayout(false);

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
	}
}