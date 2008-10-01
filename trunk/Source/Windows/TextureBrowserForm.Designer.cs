namespace CodeImp.DoomBuilder.Windows
{
	partial class TextureBrowserForm
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
			this.browser = new CodeImp.DoomBuilder.Controls.ImageBrowserControl();
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			this.texturesets = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.SuspendLayout();
			// 
			// browser
			// 
			this.browser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.browser.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.browser.HideInputBox = false;
			this.browser.LabelText = "Select or enter a texture name:";
			this.browser.Location = new System.Drawing.Point(187, 9);
			this.browser.Name = "browser";
			this.browser.PreventSelection = false;
			this.browser.Size = new System.Drawing.Size(589, 510);
			this.browser.TabIndex = 0;
			this.browser.SelectedItemChanged += new CodeImp.DoomBuilder.Controls.ImageBrowserControl.SelectedItemChangedDelegate(this.browser_SelectedItemChanged);
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(676, 496);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(100, 25);
			this.cancel.TabIndex = 22;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// apply
			// 
			this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.apply.Location = new System.Drawing.Point(570, 496);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(100, 25);
			this.apply.TabIndex = 21;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// texturesets
			// 
			this.texturesets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.texturesets.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
			this.texturesets.FullRowSelect = true;
			this.texturesets.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
			this.texturesets.HideSelection = false;
			this.texturesets.Location = new System.Drawing.Point(12, 9);
			this.texturesets.MultiSelect = false;
			this.texturesets.Name = "texturesets";
			this.texturesets.Size = new System.Drawing.Size(166, 476);
			this.texturesets.TabIndex = 23;
			this.texturesets.UseCompatibleStateImageBehavior = false;
			this.texturesets.View = System.Windows.Forms.View.Details;
			this.texturesets.SelectedIndexChanged += new System.EventHandler(this.texturesets_SelectedIndexChanged);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Name";
			this.columnHeader1.Width = 141;
			// 
			// TextureBrowserForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(788, 531);
			this.Controls.Add(this.texturesets);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Controls.Add(this.browser);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TextureBrowserForm";
			this.Opacity = 0;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Browse Textures";
			this.Activated += new System.EventHandler(this.TextureBrowserForm_Activated);
			this.Move += new System.EventHandler(this.TextureBrowserForm_Move);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TextureBrowserForm_FormClosing);
			this.ResizeEnd += new System.EventHandler(this.TextureBrowserForm_ResizeEnd);
			this.Load += new System.EventHandler(this.TextureBrowserForm_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private CodeImp.DoomBuilder.Controls.ImageBrowserControl browser;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.ListView texturesets;
		private System.Windows.Forms.ColumnHeader columnHeader1;
	}
}