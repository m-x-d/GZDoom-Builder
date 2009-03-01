namespace CodeImp.DoomBuilder.Windows
{
	partial class ScriptFindReplaceForm
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
			this.label1 = new System.Windows.Forms.Label();
			this.findtext = new System.Windows.Forms.TextBox();
			this.casesensitive = new System.Windows.Forms.CheckBox();
			this.wordonly = new System.Windows.Forms.CheckBox();
			this.replace = new System.Windows.Forms.CheckBox();
			this.replacetext = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.findnextbutton = new System.Windows.Forms.Button();
			this.replaceallbutton = new System.Windows.Forms.Button();
			this.closebutton = new System.Windows.Forms.Button();
			this.replacebutton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(29, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(30, 14);
			this.label1.TabIndex = 0;
			this.label1.Text = "Find:";
			// 
			// findtext
			// 
			this.findtext.Location = new System.Drawing.Point(65, 20);
			this.findtext.Name = "findtext";
			this.findtext.Size = new System.Drawing.Size(180, 20);
			this.findtext.TabIndex = 1;
			// 
			// casesensitive
			// 
			this.casesensitive.AutoSize = true;
			this.casesensitive.Location = new System.Drawing.Point(65, 52);
			this.casesensitive.Name = "casesensitive";
			this.casesensitive.Size = new System.Drawing.Size(97, 18);
			this.casesensitive.TabIndex = 2;
			this.casesensitive.Text = "Case sensitive";
			this.casesensitive.UseVisualStyleBackColor = true;
			// 
			// wordonly
			// 
			this.wordonly.AutoSize = true;
			this.wordonly.Location = new System.Drawing.Point(65, 76);
			this.wordonly.Name = "wordonly";
			this.wordonly.Size = new System.Drawing.Size(108, 18);
			this.wordonly.TabIndex = 3;
			this.wordonly.Text = "Whole word only";
			this.wordonly.UseVisualStyleBackColor = true;
			// 
			// replace
			// 
			this.replace.AutoSize = true;
			this.replace.Location = new System.Drawing.Point(21, 105);
			this.replace.Name = "replace";
			this.replace.Size = new System.Drawing.Size(65, 18);
			this.replace.TabIndex = 4;
			this.replace.Text = "Replace";
			this.replace.UseVisualStyleBackColor = true;
			// 
			// replacetext
			// 
			this.replacetext.Location = new System.Drawing.Point(65, 129);
			this.replacetext.Name = "replacetext";
			this.replacetext.Size = new System.Drawing.Size(180, 20);
			this.replacetext.TabIndex = 5;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(28, 132);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(31, 14);
			this.label2.TabIndex = 6;
			this.label2.Text = "With:";
			// 
			// findnextbutton
			// 
			this.findnextbutton.Location = new System.Drawing.Point(277, 18);
			this.findnextbutton.Name = "findnextbutton";
			this.findnextbutton.Size = new System.Drawing.Size(98, 25);
			this.findnextbutton.TabIndex = 7;
			this.findnextbutton.Text = "Find Next";
			this.findnextbutton.UseVisualStyleBackColor = true;
			// 
			// replaceallbutton
			// 
			this.replaceallbutton.Location = new System.Drawing.Point(277, 80);
			this.replaceallbutton.Name = "replaceallbutton";
			this.replaceallbutton.Size = new System.Drawing.Size(98, 25);
			this.replaceallbutton.TabIndex = 8;
			this.replaceallbutton.Text = "Replace All";
			this.replaceallbutton.UseVisualStyleBackColor = true;
			// 
			// closebutton
			// 
			this.closebutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.closebutton.Location = new System.Drawing.Point(277, 127);
			this.closebutton.Name = "closebutton";
			this.closebutton.Size = new System.Drawing.Size(98, 25);
			this.closebutton.TabIndex = 9;
			this.closebutton.Text = "Close";
			this.closebutton.UseVisualStyleBackColor = true;
			// 
			// replacebutton
			// 
			this.replacebutton.Location = new System.Drawing.Point(277, 49);
			this.replacebutton.Name = "replacebutton";
			this.replacebutton.Size = new System.Drawing.Size(98, 25);
			this.replacebutton.TabIndex = 10;
			this.replacebutton.Text = "Replace";
			this.replacebutton.UseVisualStyleBackColor = true;
			// 
			// ScriptFindReplaceForm
			// 
			this.AcceptButton = this.findnextbutton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.closebutton;
			this.ClientSize = new System.Drawing.Size(387, 171);
			this.Controls.Add(this.replacebutton);
			this.Controls.Add(this.closebutton);
			this.Controls.Add(this.replaceallbutton);
			this.Controls.Add(this.findnextbutton);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.replacetext);
			this.Controls.Add(this.replace);
			this.Controls.Add(this.wordonly);
			this.Controls.Add(this.casesensitive);
			this.Controls.Add(this.findtext);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "ScriptFindReplaceForm";
			this.Text = "Find and Replace";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox findtext;
		private System.Windows.Forms.CheckBox casesensitive;
		private System.Windows.Forms.CheckBox wordonly;
		private System.Windows.Forms.CheckBox replace;
		private System.Windows.Forms.TextBox replacetext;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button findnextbutton;
		private System.Windows.Forms.Button replaceallbutton;
		private System.Windows.Forms.Button closebutton;
		private System.Windows.Forms.Button replacebutton;
	}
}