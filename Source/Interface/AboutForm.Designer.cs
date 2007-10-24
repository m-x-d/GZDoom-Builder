namespace CodeImp.DoomBuilder.Interface
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
			this.label1 = new System.Windows.Forms.Label();
			this.close = new System.Windows.Forms.Button();
			this.builderlink = new System.Windows.Forms.LinkLabel();
			pictureBox1 = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(pictureBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			pictureBox1.Image = global::CodeImp.DoomBuilder.Properties.Resources.Splash2small;
			pictureBox1.Location = new System.Drawing.Point(12, 12);
			pictureBox1.Name = "pictureBox1";
			pictureBox1.Size = new System.Drawing.Size(137, 82);
			pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			pictureBox1.TabIndex = 0;
			pictureBox1.TabStop = false;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(166, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(342, 50);
			this.label1.TabIndex = 2;
			this.label1.Text = "Doom Builder is designed and programmed by Pascal vd Heiden.\r\nSeveral game config" +
				"urations were written by various members of the Doom community.";
			// 
			// close
			// 
			this.close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.close.Location = new System.Drawing.Point(392, 69);
			this.close.Name = "close";
			this.close.Size = new System.Drawing.Size(116, 25);
			this.close.TabIndex = 3;
			this.close.Text = "Close";
			this.close.UseVisualStyleBackColor = true;
			// 
			// builderlink
			// 
			this.builderlink.AutoSize = true;
			this.builderlink.Location = new System.Drawing.Point(166, 62);
			this.builderlink.Name = "builderlink";
			this.builderlink.Size = new System.Drawing.Size(121, 14);
			this.builderlink.TabIndex = 5;
			this.builderlink.TabStop = true;
			this.builderlink.Text = "www.doombuilder.com";
			this.builderlink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.builderlink_LinkClicked);
			// 
			// AboutForm
			// 
			this.AcceptButton = this.close;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.close;
			this.ClientSize = new System.Drawing.Size(518, 104);
			this.Controls.Add(this.builderlink);
			this.Controls.Add(this.close);
			this.Controls.Add(this.label1);
			this.Controls.Add(pictureBox1);
			this.DoubleBuffered = true;
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
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button close;
		private System.Windows.Forms.LinkLabel builderlink;
	}
}