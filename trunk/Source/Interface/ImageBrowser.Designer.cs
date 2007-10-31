namespace CodeImp.DoomBuilder.Interface
{
	partial class ImageBrowser
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
			if(graphics != null) graphics.Dispose();
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.splitter = new System.Windows.Forms.SplitContainer();
			this.rendertarget = new CodeImp.DoomBuilder.Interface.RenderTargetControl();
			this.splitter.Panel1.SuspendLayout();
			this.splitter.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitter
			// 
			this.splitter.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitter.IsSplitterFixed = true;
			this.splitter.Location = new System.Drawing.Point(0, 0);
			this.splitter.Name = "splitter";
			this.splitter.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitter.Panel1
			// 
			this.splitter.Panel1.Controls.Add(this.rendertarget);
			this.splitter.Size = new System.Drawing.Size(518, 346);
			this.splitter.SplitterDistance = 310;
			this.splitter.TabIndex = 0;
			this.splitter.TabStop = false;
			// 
			// rendertarget
			// 
			this.rendertarget.BackColor = System.Drawing.SystemColors.Window;
			this.rendertarget.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.rendertarget.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rendertarget.Location = new System.Drawing.Point(0, 0);
			this.rendertarget.Name = "rendertarget";
			this.rendertarget.Size = new System.Drawing.Size(518, 310);
			this.rendertarget.TabIndex = 3;
			// 
			// ImageBrowser
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitter);
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.Name = "ImageBrowser";
			this.Size = new System.Drawing.Size(518, 346);
			this.splitter.Panel1.ResumeLayout(false);
			this.splitter.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitter;
		private RenderTargetControl rendertarget;

	}
}
