namespace CodeImp.DoomBuilder.Windows
{
	partial class AboutForm
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
			System.Windows.Forms.PictureBox pictureBox1;
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label2;
			this.close = new System.Windows.Forms.Button();
			this.builderlink = new System.Windows.Forms.LinkLabel();
			this.version = new System.Windows.Forms.Label();
			this.slimdxlogo = new System.Windows.Forms.PictureBox();
			pictureBox1 = new System.Windows.Forms.PictureBox();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.slimdxlogo)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			pictureBox1.Image = global::CodeImp.DoomBuilder.Properties.Resources.Splash3_small;
			pictureBox1.Location = new System.Drawing.Point(10, 12);
			pictureBox1.Name = "pictureBox1";
			pictureBox1.Size = new System.Drawing.Size(226, 80);
			pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			pictureBox1.TabIndex = 0;
			pictureBox1.TabStop = false;
			// 
			// label1
			// 
			label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			label1.Location = new System.Drawing.Point(15, 119);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(335, 50);
			label1.TabIndex = 2;
			label1.Text = "Doom Builder is designed and programmed by Pascal vd Heiden.\r\nSeveral game config" +
				"urations were written by various members of the Doom community. See the website " +
				"for a complete list of credits.";
			// 
			// label2
			// 
			label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			label2.Location = new System.Drawing.Point(15, 233);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(213, 29);
			label2.TabIndex = 8;
			label2.Text = "Doom Builder is powered by SlimDX,\r\na DirectX API for Microsoft .NET 2.0";
			// 
			// close
			// 
			this.close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.close.Location = new System.Drawing.Point(234, 237);
			this.close.Name = "close";
			this.close.Size = new System.Drawing.Size(116, 25);
			this.close.TabIndex = 3;
			this.close.Text = "Close";
			this.close.UseVisualStyleBackColor = true;
			// 
			// builderlink
			// 
			this.builderlink.AutoSize = true;
			this.builderlink.Location = new System.Drawing.Point(12, 169);
			this.builderlink.Name = "builderlink";
			this.builderlink.Size = new System.Drawing.Size(121, 14);
			this.builderlink.TabIndex = 5;
			this.builderlink.TabStop = true;
			this.builderlink.Text = "www.doombuilder.com";
			this.builderlink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.builderlink_LinkClicked);
			// 
			// version
			// 
			this.version.AutoSize = true;
			this.version.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.version.Location = new System.Drawing.Point(15, 98);
			this.version.Name = "version";
			this.version.Size = new System.Drawing.Size(138, 14);
			this.version.TabIndex = 6;
			this.version.Text = "Doom Builder some version";
			// 
			// slimdxlogo
			// 
			this.slimdxlogo.Cursor = System.Windows.Forms.Cursors.Hand;
			this.slimdxlogo.Image = global::CodeImp.DoomBuilder.Properties.Resources.SlimDX_small;
			this.slimdxlogo.Location = new System.Drawing.Point(10, 210);
			this.slimdxlogo.Name = "slimdxlogo";
			this.slimdxlogo.Size = new System.Drawing.Size(80, 20);
			this.slimdxlogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.slimdxlogo.TabIndex = 7;
			this.slimdxlogo.TabStop = false;
			this.slimdxlogo.Click += new System.EventHandler(this.slimdxlogo_Click);
			// 
			// AboutForm
			// 
			this.AcceptButton = this.close;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.close;
			this.ClientSize = new System.Drawing.Size(358, 270);
			this.Controls.Add(label2);
			this.Controls.Add(this.slimdxlogo);
			this.Controls.Add(this.version);
			this.Controls.Add(this.builderlink);
			this.Controls.Add(this.close);
			this.Controls.Add(label1);
			this.Controls.Add(pictureBox1);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutForm";
			this.Opacity = 0;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "About Doom Builder";
			((System.ComponentModel.ISupportInitialize)(pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.slimdxlogo)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button close;
		private System.Windows.Forms.LinkLabel builderlink;
		private System.Windows.Forms.Label version;
		private System.Windows.Forms.PictureBox slimdxlogo;
	}
}