namespace CodeImp.DoomBuilder.BuilderModes.Interface
{
	partial class MakeDoorForm
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
			this.doortexture = new CodeImp.DoomBuilder.Controls.TextureSelectorControl();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.ceilingtexture = new CodeImp.DoomBuilder.Controls.FlatSelectorControl();
			this.floortexture = new CodeImp.DoomBuilder.Controls.FlatSelectorControl();
			this.label3 = new System.Windows.Forms.Label();
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// doortexture
			// 
			this.doortexture.Location = new System.Drawing.Point(22, 34);
			this.doortexture.Name = "doortexture";
			this.doortexture.Required = false;
			this.doortexture.Size = new System.Drawing.Size(96, 115);
			this.doortexture.TabIndex = 0;
			this.doortexture.TextureName = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(22, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(96, 21);
			this.label1.TabIndex = 1;
			this.label1.Text = "Door";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(132, 15);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(99, 21);
			this.label2.TabIndex = 2;
			this.label2.Text = "Ceiling";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// ceilingtexture
			// 
			this.ceilingtexture.Location = new System.Drawing.Point(135, 34);
			this.ceilingtexture.Name = "ceilingtexture";
			this.ceilingtexture.Size = new System.Drawing.Size(96, 115);
			this.ceilingtexture.TabIndex = 3;
			this.ceilingtexture.TextureName = "";
			// 
			// floortexture
			// 
			this.floortexture.Location = new System.Drawing.Point(248, 34);
			this.floortexture.Name = "floortexture";
			this.floortexture.Size = new System.Drawing.Size(96, 115);
			this.floortexture.TabIndex = 5;
			this.floortexture.TextureName = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(245, 15);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(99, 21);
			this.label3.TabIndex = 4;
			this.label3.Text = "Floor";
			this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(187, 180);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(112, 25);
			this.cancel.TabIndex = 15;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// apply
			// 
			this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.apply.Location = new System.Drawing.Point(69, 180);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 14;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// MakeDoorForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(368, 219);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Controls.Add(this.floortexture);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.ceilingtexture);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.doortexture);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MakeDoorForm";
			this.Opacity = 0;
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Make Door";
			this.ResumeLayout(false);

		}

		#endregion

		private CodeImp.DoomBuilder.Controls.TextureSelectorControl doortexture;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private CodeImp.DoomBuilder.Controls.FlatSelectorControl ceilingtexture;
		private CodeImp.DoomBuilder.Controls.FlatSelectorControl floortexture;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
	}
}