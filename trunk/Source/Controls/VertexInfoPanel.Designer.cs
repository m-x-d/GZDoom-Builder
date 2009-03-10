namespace CodeImp.DoomBuilder.Controls
{
	partial class VertexInfoPanel
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
			System.Windows.Forms.Label label1;
			this.vertexinfo = new System.Windows.Forms.GroupBox();
			this.position = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			this.vertexinfo.SuspendLayout();
			this.SuspendLayout();
			// 
			// vertexinfo
			// 
			this.vertexinfo.Controls.Add(this.position);
			this.vertexinfo.Controls.Add(label1);
			this.vertexinfo.Location = new System.Drawing.Point(0, 0);
			this.vertexinfo.Name = "vertexinfo";
			this.vertexinfo.Size = new System.Drawing.Size(163, 100);
			this.vertexinfo.TabIndex = 0;
			this.vertexinfo.TabStop = false;
			this.vertexinfo.Text = " Vertex ";
			// 
			// position
			// 
			this.position.AutoSize = true;
			this.position.Location = new System.Drawing.Point(66, 34);
			this.position.Name = "position";
			this.position.Size = new System.Drawing.Size(25, 14);
			this.position.TabIndex = 3;
			this.position.Text = "0, 0";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(13, 34);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(47, 14);
			label1.TabIndex = 2;
			label1.Text = "Position:";
			// 
			// VertexInfoPanel
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.vertexinfo);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximumSize = new System.Drawing.Size(10000, 100);
			this.MinimumSize = new System.Drawing.Size(100, 100);
			this.Name = "VertexInfoPanel";
			this.Size = new System.Drawing.Size(393, 100);
			this.vertexinfo.ResumeLayout(false);
			this.vertexinfo.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label position;
		private System.Windows.Forms.GroupBox vertexinfo;

	}
}
