namespace CodeImp.DoomBuilder.Controls
{
	partial class StatisticsControl
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
			this.thingscount = new System.Windows.Forms.Label();
			this.sectorscount = new System.Windows.Forms.Label();
			this.sidedefscount = new System.Windows.Forms.Label();
			this.linedefscount = new System.Windows.Forms.Label();
			this.verticescount = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// thingscount
			// 
			this.thingscount.Location = new System.Drawing.Point(8, 81);
			this.thingscount.Name = "thingscount";
			this.thingscount.Size = new System.Drawing.Size(43, 14);
			this.thingscount.TabIndex = 19;
			this.thingscount.Text = "000000";
			this.thingscount.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// sectorscount
			// 
			this.sectorscount.Location = new System.Drawing.Point(8, 62);
			this.sectorscount.Name = "sectorscount";
			this.sectorscount.Size = new System.Drawing.Size(43, 14);
			this.sectorscount.TabIndex = 18;
			this.sectorscount.Text = "000000";
			this.sectorscount.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// sidedefscount
			// 
			this.sidedefscount.Location = new System.Drawing.Point(8, 44);
			this.sidedefscount.Name = "sidedefscount";
			this.sidedefscount.Size = new System.Drawing.Size(43, 14);
			this.sidedefscount.TabIndex = 17;
			this.sidedefscount.Text = "000000";
			this.sidedefscount.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// linedefscount
			// 
			this.linedefscount.Location = new System.Drawing.Point(8, 26);
			this.linedefscount.Name = "linedefscount";
			this.linedefscount.Size = new System.Drawing.Size(43, 14);
			this.linedefscount.TabIndex = 16;
			this.linedefscount.Text = "000000";
			this.linedefscount.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// verticescount
			// 
			this.verticescount.Location = new System.Drawing.Point(8, 8);
			this.verticescount.Name = "verticescount";
			this.verticescount.Size = new System.Drawing.Size(43, 14);
			this.verticescount.TabIndex = 15;
			this.verticescount.Text = "000000";
			this.verticescount.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(55, 81);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(54, 15);
			this.label5.TabIndex = 14;
			this.label5.Text = "Things";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(55, 62);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(54, 15);
			this.label4.TabIndex = 13;
			this.label4.Text = "Sectors";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(55, 44);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(54, 15);
			this.label3.TabIndex = 12;
			this.label3.Text = "Sidedefs";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(55, 26);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(54, 15);
			this.label2.TabIndex = 11;
			this.label2.Text = "Linedefs";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(55, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(54, 15);
			this.label1.TabIndex = 10;
			this.label1.Text = "Vertices";
			// 
			// StatisticsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.thingscount);
			this.Controls.Add(this.sectorscount);
			this.Controls.Add(this.sidedefscount);
			this.Controls.Add(this.linedefscount);
			this.Controls.Add(this.verticescount);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.ForeColor = System.Drawing.SystemColors.GrayText;
			this.Name = "StatisticsControl";
			this.Size = new System.Drawing.Size(118, 104);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label thingscount;
		private System.Windows.Forms.Label sectorscount;
		private System.Windows.Forms.Label sidedefscount;
		private System.Windows.Forms.Label linedefscount;
		private System.Windows.Forms.Label verticescount;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
	}
}
