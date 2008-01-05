namespace CodeImp.DoomBuilder.Interface
{
	partial class SectorEditForm
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
			System.Windows.Forms.GroupBox groupfloorceiling;
			System.Windows.Forms.GroupBox groupeffect;
			System.Windows.Forms.GroupBox groupaction;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label1;
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			this.flatSelectorControl1 = new CodeImp.DoomBuilder.Interface.FlatSelectorControl();
			this.flatSelectorControl2 = new CodeImp.DoomBuilder.Interface.FlatSelectorControl();
			groupfloorceiling = new System.Windows.Forms.GroupBox();
			groupeffect = new System.Windows.Forms.GroupBox();
			groupaction = new System.Windows.Forms.GroupBox();
			label3 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			groupfloorceiling.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupfloorceiling
			// 
			groupfloorceiling.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			groupfloorceiling.Controls.Add(label1);
			groupfloorceiling.Controls.Add(label3);
			groupfloorceiling.Controls.Add(this.flatSelectorControl2);
			groupfloorceiling.Controls.Add(this.flatSelectorControl1);
			groupfloorceiling.Location = new System.Drawing.Point(12, 12);
			groupfloorceiling.Name = "groupfloorceiling";
			groupfloorceiling.Size = new System.Drawing.Size(465, 161);
			groupfloorceiling.TabIndex = 0;
			groupfloorceiling.TabStop = false;
			groupfloorceiling.Text = "Floor and Ceiling ";
			// 
			// groupeffect
			// 
			groupeffect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			groupeffect.Location = new System.Drawing.Point(12, 182);
			groupeffect.Name = "groupeffect";
			groupeffect.Size = new System.Drawing.Size(465, 83);
			groupeffect.TabIndex = 1;
			groupeffect.TabStop = false;
			groupeffect.Text = " Effect ";
			// 
			// groupaction
			// 
			groupaction.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			groupaction.Location = new System.Drawing.Point(12, 274);
			groupaction.Name = "groupaction";
			groupaction.Size = new System.Drawing.Size(465, 83);
			groupaction.TabIndex = 2;
			groupaction.TabStop = false;
			groupaction.Text = " Action ";
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(365, 371);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(112, 25);
			this.cancel.TabIndex = 19;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			// 
			// apply
			// 
			this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.apply.Location = new System.Drawing.Point(246, 371);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 18;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			// 
			// flatSelectorControl1
			// 
			this.flatSelectorControl1.Location = new System.Drawing.Point(363, 37);
			this.flatSelectorControl1.Name = "flatSelectorControl1";
			this.flatSelectorControl1.Size = new System.Drawing.Size(83, 105);
			this.flatSelectorControl1.TabIndex = 12;
			this.flatSelectorControl1.TextureName = "";
			// 
			// flatSelectorControl2
			// 
			this.flatSelectorControl2.Location = new System.Drawing.Point(271, 37);
			this.flatSelectorControl2.Name = "flatSelectorControl2";
			this.flatSelectorControl2.Size = new System.Drawing.Size(83, 105);
			this.flatSelectorControl2.TabIndex = 13;
			this.flatSelectorControl2.TextureName = "";
			// 
			// label3
			// 
			label3.Location = new System.Drawing.Point(363, 18);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(83, 16);
			label3.TabIndex = 14;
			label3.Text = "Ceiling";
			label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label1
			// 
			label1.Location = new System.Drawing.Point(271, 18);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(83, 16);
			label1.TabIndex = 15;
			label1.Text = "Floor";
			label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// SectorEditForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(489, 407);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Controls.Add(groupaction);
			this.Controls.Add(groupeffect);
			this.Controls.Add(groupfloorceiling);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SectorEditForm";
			this.Opacity = 0;
			this.ShowInTaskbar = false;
			this.Text = "Edit Sector";
			groupfloorceiling.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private FlatSelectorControl flatSelectorControl2;
		private FlatSelectorControl flatSelectorControl1;
	}
}