namespace CodeImp.DoomBuilder.Windows
{
	partial class ScriptEditTestForm
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
			this.scriptedit = new CodeImp.DoomBuilder.Controls.ScriptEditControl();
			this.SuspendLayout();
			// 
			// scriptedit
			// 
			this.scriptedit.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.scriptedit.Location = new System.Drawing.Point(12, 12);
			this.scriptedit.Name = "scriptedit";
			this.scriptedit.Size = new System.Drawing.Size(643, 487);
			this.scriptedit.TabIndex = 0;
			this.scriptedit.Text = "";
			// 
			// ScriptEditTestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(667, 511);
			this.Controls.Add(this.scriptedit);
			this.Name = "ScriptEditTestForm";
			this.Text = "ScriptEditTestForm";
			this.ResumeLayout(false);

		}

		#endregion

		private CodeImp.DoomBuilder.Controls.ScriptEditControl scriptedit;
	}
}