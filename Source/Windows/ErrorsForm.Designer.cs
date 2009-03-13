namespace CodeImp.DoomBuilder.Windows
{
	partial class ErrorsForm
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorsForm));
			this.list = new System.Windows.Forms.ListView();
			this.colmessage = new System.Windows.Forms.ColumnHeader();
			this.images = new System.Windows.Forms.ImageList(this.components);
			this.copyselected = new System.Windows.Forms.Button();
			this.clearlist = new System.Windows.Forms.Button();
			this.close = new System.Windows.Forms.Button();
			this.checkerrors = new System.Windows.Forms.Timer(this.components);
			this.checkshow = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// list
			// 
			this.list.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colmessage});
			this.list.FullRowSelect = true;
			this.list.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.list.HideSelection = false;
			this.list.Location = new System.Drawing.Point(12, 12);
			this.list.Name = "list";
			this.list.Size = new System.Drawing.Size(732, 395);
			this.list.SmallImageList = this.images;
			this.list.TabIndex = 0;
			this.list.UseCompatibleStateImageBehavior = false;
			this.list.View = System.Windows.Forms.View.Details;
			// 
			// colmessage
			// 
			this.colmessage.Text = "";
			this.colmessage.Width = 702;
			// 
			// images
			// 
			this.images.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("images.ImageStream")));
			this.images.TransparentColor = System.Drawing.Color.Transparent;
			this.images.Images.SetKeyName(0, "ErrorLarge.png");
			this.images.Images.SetKeyName(1, "WarningLarge.png");
			// 
			// copyselected
			// 
			this.copyselected.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.copyselected.Location = new System.Drawing.Point(12, 416);
			this.copyselected.Name = "copyselected";
			this.copyselected.Size = new System.Drawing.Size(122, 25);
			this.copyselected.TabIndex = 1;
			this.copyselected.Text = "Copy Selection";
			this.copyselected.UseVisualStyleBackColor = true;
			this.copyselected.Click += new System.EventHandler(this.copyselected_Click);
			// 
			// clearlist
			// 
			this.clearlist.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.clearlist.Location = new System.Drawing.Point(150, 416);
			this.clearlist.Name = "clearlist";
			this.clearlist.Size = new System.Drawing.Size(122, 25);
			this.clearlist.TabIndex = 2;
			this.clearlist.Text = "Clear";
			this.clearlist.UseVisualStyleBackColor = true;
			this.clearlist.Click += new System.EventHandler(this.clearlist_Click);
			// 
			// close
			// 
			this.close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.close.Location = new System.Drawing.Point(622, 416);
			this.close.Name = "close";
			this.close.Size = new System.Drawing.Size(122, 25);
			this.close.TabIndex = 3;
			this.close.Text = "Close";
			this.close.UseVisualStyleBackColor = true;
			this.close.Click += new System.EventHandler(this.close_Click);
			// 
			// checkerrors
			// 
			this.checkerrors.Interval = 1000;
			this.checkerrors.Tick += new System.EventHandler(this.checkerrors_Tick);
			// 
			// checkshow
			// 
			this.checkshow.AutoSize = true;
			this.checkshow.Location = new System.Drawing.Point(301, 420);
			this.checkshow.Name = "checkshow";
			this.checkshow.Size = new System.Drawing.Size(213, 18);
			this.checkshow.TabIndex = 4;
			this.checkshow.Text = "Show this window when errors occur";
			this.checkshow.UseVisualStyleBackColor = true;
			// 
			// ErrorsForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.close;
			this.ClientSize = new System.Drawing.Size(756, 453);
			this.Controls.Add(this.checkshow);
			this.Controls.Add(this.close);
			this.Controls.Add(this.clearlist);
			this.Controls.Add(this.copyselected);
			this.Controls.Add(this.list);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ErrorsForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Errors and Warnings";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ErrorsForm_FormClosing);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ListView list;
		private System.Windows.Forms.Button copyselected;
		private System.Windows.Forms.Button clearlist;
		private System.Windows.Forms.Button close;
		private System.Windows.Forms.ColumnHeader colmessage;
		private System.Windows.Forms.ImageList images;
		private System.Windows.Forms.Timer checkerrors;
		private System.Windows.Forms.CheckBox checkshow;
	}
}