namespace CodeImp.DoomBuilder.Interface
{
	partial class CheckboxArrayControl
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
			this.SuspendLayout();
			// 
			// CheckboxArrayControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.DoubleBuffered = true;
			this.Name = "CheckboxArrayControl";
			this.Size = new System.Drawing.Size(361, 163);
			this.Layout += new System.Windows.Forms.LayoutEventHandler(this.CheckboxArrayControl_Layout);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.CheckboxArrayControl_Paint);
			this.ResumeLayout(false);

		}

		#endregion


	}
}
