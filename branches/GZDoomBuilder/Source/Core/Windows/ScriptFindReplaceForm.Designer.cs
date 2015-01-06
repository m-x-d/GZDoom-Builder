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
			this.replacetext = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.findnextbutton = new System.Windows.Forms.Button();
			this.replaceallbutton = new System.Windows.Forms.Button();
			this.closebutton = new System.Windows.Forms.Button();
			this.replacebutton = new System.Windows.Forms.Button();
			this.findpreviousbutton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(29, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(56, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Find what:";
			// 
			// findtext
			// 
			this.findtext.Location = new System.Drawing.Point(93, 20);
			this.findtext.Name = "findtext";
			this.findtext.Size = new System.Drawing.Size(152, 20);
			this.findtext.TabIndex = 0;
			// 
			// casesensitive
			// 
			this.casesensitive.AutoSize = true;
			this.casesensitive.Location = new System.Drawing.Point(93, 81);
			this.casesensitive.Name = "casesensitive";
			this.casesensitive.Size = new System.Drawing.Size(94, 17);
			this.casesensitive.TabIndex = 2;
			this.casesensitive.Text = "Case sensitive";
			this.casesensitive.UseVisualStyleBackColor = true;
			// 
			// wordonly
			// 
			this.wordonly.AutoSize = true;
			this.wordonly.Location = new System.Drawing.Point(93, 105);
			this.wordonly.Name = "wordonly";
			this.wordonly.Size = new System.Drawing.Size(105, 17);
			this.wordonly.TabIndex = 3;
			this.wordonly.Text = "Whole word only";
			this.wordonly.UseVisualStyleBackColor = true;
			// 
			// replacetext
			// 
			this.replacetext.Location = new System.Drawing.Point(93, 51);
			this.replacetext.Name = "replacetext";
			this.replacetext.Size = new System.Drawing.Size(152, 20);
			this.replacetext.TabIndex = 1;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(14, 54);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(72, 13);
			this.label2.TabIndex = 6;
			this.label2.Text = "Replace with:";
			// 
			// findnextbutton
			// 
			this.findnextbutton.Location = new System.Drawing.Point(251, 17);
			this.findnextbutton.Name = "findnextbutton";
			this.findnextbutton.Size = new System.Drawing.Size(80, 25);
			this.findnextbutton.TabIndex = 4;
			this.findnextbutton.Text = "Find Next";
			this.findnextbutton.UseVisualStyleBackColor = true;
			this.findnextbutton.Click += new System.EventHandler(this.findnextbutton_Click);
			// 
			// replaceallbutton
			// 
			this.replaceallbutton.Location = new System.Drawing.Point(337, 48);
			this.replaceallbutton.Name = "replaceallbutton";
			this.replaceallbutton.Size = new System.Drawing.Size(86, 25);
			this.replaceallbutton.TabIndex = 7;
			this.replaceallbutton.Text = "Replace All";
			this.replaceallbutton.UseVisualStyleBackColor = true;
			this.replaceallbutton.Click += new System.EventHandler(this.replaceallbutton_Click);
			// 
			// closebutton
			// 
			this.closebutton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.closebutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.closebutton.Location = new System.Drawing.Point(325, 96);
			this.closebutton.Name = "closebutton";
			this.closebutton.Size = new System.Drawing.Size(98, 25);
			this.closebutton.TabIndex = 8;
			this.closebutton.Text = "Close";
			this.closebutton.UseVisualStyleBackColor = true;
			this.closebutton.Click += new System.EventHandler(this.closebutton_Click);
			// 
			// replacebutton
			// 
			this.replacebutton.Location = new System.Drawing.Point(251, 48);
			this.replacebutton.Name = "replacebutton";
			this.replacebutton.Size = new System.Drawing.Size(80, 25);
			this.replacebutton.TabIndex = 6;
			this.replacebutton.Text = "Replace";
			this.replacebutton.UseVisualStyleBackColor = true;
			this.replacebutton.Click += new System.EventHandler(this.replacebutton_Click);
			// 
			// findpreviousbutton
			// 
			this.findpreviousbutton.Location = new System.Drawing.Point(337, 17);
			this.findpreviousbutton.Name = "findpreviousbutton";
			this.findpreviousbutton.Size = new System.Drawing.Size(86, 25);
			this.findpreviousbutton.TabIndex = 5;
			this.findpreviousbutton.Text = "Find Previous";
			this.findpreviousbutton.UseVisualStyleBackColor = true;
			this.findpreviousbutton.Click += new System.EventHandler(this.findpreviousbutton_Click);
			// 
			// ScriptFindReplaceForm
			// 
			this.AcceptButton = this.findnextbutton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.closebutton;
			this.ClientSize = new System.Drawing.Size(436, 127);
			this.Controls.Add(this.findpreviousbutton);
			this.Controls.Add(this.replacebutton);
			this.Controls.Add(this.closebutton);
			this.Controls.Add(this.replaceallbutton);
			this.Controls.Add(this.findnextbutton);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.replacetext);
			this.Controls.Add(this.wordonly);
			this.Controls.Add(this.casesensitive);
			this.Controls.Add(this.findtext);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ScriptFindReplaceForm";
			this.Opacity = 1;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "Find and Replace";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ScriptFindReplaceForm_FormClosing);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox findtext;
		private System.Windows.Forms.CheckBox casesensitive;
		private System.Windows.Forms.CheckBox wordonly;
		private System.Windows.Forms.TextBox replacetext;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button findnextbutton;
		private System.Windows.Forms.Button replaceallbutton;
		private System.Windows.Forms.Button closebutton;
		private System.Windows.Forms.Button replacebutton;
		private System.Windows.Forms.Button findpreviousbutton;
	}
}