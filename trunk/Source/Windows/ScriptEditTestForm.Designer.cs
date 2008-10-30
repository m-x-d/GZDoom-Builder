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
			this.script = new CodeImp.DoomBuilder.Controls.BuilderScriptControl();
			this.SuspendLayout();
			// 
			// script
			// 
			this.script.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.script.Location = new System.Drawing.Point(12, 12);
			this.script.Name = "script";
			this.script.Size = new System.Drawing.Size(643, 487);
			this.script.TabIndex = 0;
			// 
			// ScriptEditTestForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(667, 511);
			this.Controls.Add(this.script);
			this.Name = "ScriptEditTestForm";
			this.Text = "ScriptEditTestForm";
			this.ResumeLayout(false);

		}

		#endregion

		private CodeImp.DoomBuilder.Controls.BuilderScriptControl script;


	}
}