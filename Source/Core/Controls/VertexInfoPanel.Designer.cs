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
			this.labelCeilingOffset = new System.Windows.Forms.Label();
			this.labelFloorOffset = new System.Windows.Forms.Label();
			this.zceiling = new System.Windows.Forms.Label();
			this.zfloor = new System.Windows.Forms.Label();
			this.panelOffsets = new System.Windows.Forms.Panel();
			label1 = new System.Windows.Forms.Label();
			this.vertexinfo.SuspendLayout();
			this.panelOffsets.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(13, 28);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(47, 14);
			label1.TabIndex = 2;
			label1.Text = "Position:";
			// 
			// vertexinfo
			// 
			this.vertexinfo.Controls.Add(this.panelOffsets);
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
			this.position.Location = new System.Drawing.Point(66, 28);
			this.position.Name = "position";
			this.position.Size = new System.Drawing.Size(25, 14);
			this.position.TabIndex = 3;
			this.position.Text = "0, 0";
			// 
			// labelCeilingOffset
			// 
			this.labelCeilingOffset.AutoSize = true;
			this.labelCeilingOffset.Location = new System.Drawing.Point(7, 3);
			this.labelCeilingOffset.Name = "labelCeilingOffset";
			this.labelCeilingOffset.Size = new System.Drawing.Size(73, 14);
			this.labelCeilingOffset.TabIndex = 4;
			this.labelCeilingOffset.Text = "Ceiling offset:";
			// 
			// labelFloorOffset
			// 
			this.labelFloorOffset.AutoSize = true;
			this.labelFloorOffset.Location = new System.Drawing.Point(14, 26);
			this.labelFloorOffset.Name = "labelFloorOffset";
			this.labelFloorOffset.Size = new System.Drawing.Size(66, 14);
			this.labelFloorOffset.TabIndex = 5;
			this.labelFloorOffset.Text = "Floor offset:";
			// 
			// zceiling
			// 
			this.zceiling.AutoSize = true;
			this.zceiling.Location = new System.Drawing.Point(86, 3);
			this.zceiling.Name = "zceiling";
			this.zceiling.Size = new System.Drawing.Size(29, 14);
			this.zceiling.TabIndex = 6;
			this.zceiling.Text = "-512";
			// 
			// zfloor
			// 
			this.zfloor.AutoSize = true;
			this.zfloor.Location = new System.Drawing.Point(86, 26);
			this.zfloor.Name = "zfloor";
			this.zfloor.Size = new System.Drawing.Size(29, 14);
			this.zfloor.TabIndex = 7;
			this.zfloor.Text = "-512";
			// 
			// panelOffsets
			// 
			this.panelOffsets.Controls.Add(this.zfloor);
			this.panelOffsets.Controls.Add(this.labelCeilingOffset);
			this.panelOffsets.Controls.Add(this.zceiling);
			this.panelOffsets.Controls.Add(this.labelFloorOffset);
			this.panelOffsets.Location = new System.Drawing.Point(6, 49);
			this.panelOffsets.Name = "panelOffsets";
			this.panelOffsets.Size = new System.Drawing.Size(151, 46);
			this.panelOffsets.TabIndex = 1;
			// 
			// VertexInfoPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.Controls.Add(this.vertexinfo);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.MaximumSize = new System.Drawing.Size(10000, 100);
			this.MinimumSize = new System.Drawing.Size(100, 100);
			this.Name = "VertexInfoPanel";
			this.Size = new System.Drawing.Size(393, 100);
			this.vertexinfo.ResumeLayout(false);
			this.vertexinfo.PerformLayout();
			this.panelOffsets.ResumeLayout(false);
			this.panelOffsets.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label position;
		private System.Windows.Forms.GroupBox vertexinfo;
		private System.Windows.Forms.Label zfloor;
		private System.Windows.Forms.Label zceiling;
		private System.Windows.Forms.Panel panelOffsets;
		private System.Windows.Forms.Label labelCeilingOffset;
		private System.Windows.Forms.Label labelFloorOffset;

	}
}
