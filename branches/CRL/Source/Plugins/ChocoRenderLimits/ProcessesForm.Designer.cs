namespace CodeImp.DoomBuilder.Plugins.ChocoRenderLimits
{
	partial class ProcessesForm
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProcessesForm));
			this.list = new System.Windows.Forms.ListView();
			this.colarea = new System.Windows.Forms.ColumnHeader();
			this.colgranularity = new System.Windows.Forms.ColumnHeader();
			this.colthreads = new System.Windows.Forms.ColumnHeader();
			this.colstatus = new System.Windows.Forms.ColumnHeader();
			this.newbutton = new System.Windows.Forms.Button();
			this.importbutton = new System.Windows.Forms.Button();
			this.removebutton = new System.Windows.Forms.Button();
			this.closebutton = new System.Windows.Forms.Button();
			this.updatetimer = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// list
			// 
			this.list.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.list.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colarea,
            this.colgranularity,
            this.colthreads,
            this.colstatus});
			this.list.FullRowSelect = true;
			this.list.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.list.Location = new System.Drawing.Point(12, 12);
			this.list.Name = "list";
			this.list.Size = new System.Drawing.Size(513, 177);
			this.list.TabIndex = 0;
			this.list.UseCompatibleStateImageBehavior = false;
			this.list.View = System.Windows.Forms.View.Details;
			// 
			// colarea
			// 
			this.colarea.Text = "Area";
			this.colarea.Width = 200;
			// 
			// colgranularity
			// 
			this.colgranularity.Text = "Granularity";
			this.colgranularity.Width = 80;
			// 
			// colthreads
			// 
			this.colthreads.Text = "Threads";
			this.colthreads.Width = 80;
			// 
			// colstatus
			// 
			this.colstatus.Text = "Status";
			this.colstatus.Width = 107;
			// 
			// newbutton
			// 
			this.newbutton.Location = new System.Drawing.Point(12, 205);
			this.newbutton.Name = "newbutton";
			this.newbutton.Size = new System.Drawing.Size(106, 26);
			this.newbutton.TabIndex = 1;
			this.newbutton.Text = "New Test...";
			this.newbutton.UseVisualStyleBackColor = true;
			this.newbutton.Click += new System.EventHandler(this.newbutton_Click);
			// 
			// importbutton
			// 
			this.importbutton.Location = new System.Drawing.Point(124, 205);
			this.importbutton.Name = "importbutton";
			this.importbutton.Size = new System.Drawing.Size(106, 26);
			this.importbutton.TabIndex = 2;
			this.importbutton.Text = "Import Results";
			this.importbutton.UseVisualStyleBackColor = true;
			// 
			// removebutton
			// 
			this.removebutton.Location = new System.Drawing.Point(236, 205);
			this.removebutton.Name = "removebutton";
			this.removebutton.Size = new System.Drawing.Size(106, 26);
			this.removebutton.TabIndex = 3;
			this.removebutton.Text = "Abort Test";
			this.removebutton.UseVisualStyleBackColor = true;
			// 
			// closebutton
			// 
			this.closebutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.closebutton.Location = new System.Drawing.Point(419, 205);
			this.closebutton.Name = "closebutton";
			this.closebutton.Size = new System.Drawing.Size(106, 26);
			this.closebutton.TabIndex = 4;
			this.closebutton.Text = "Close";
			this.closebutton.UseVisualStyleBackColor = true;
			this.closebutton.Click += new System.EventHandler(this.closebutton_Click);
			// 
			// updatetimer
			// 
			this.updatetimer.Enabled = true;
			this.updatetimer.Interval = 319;
			this.updatetimer.Tick += new System.EventHandler(this.updatetimer_Tick);
			// 
			// ProcessesForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.closebutton;
			this.ClientSize = new System.Drawing.Size(537, 243);
			this.Controls.Add(this.closebutton);
			this.Controls.Add(this.removebutton);
			this.Controls.Add(this.importbutton);
			this.Controls.Add(this.newbutton);
			this.Controls.Add(this.list);
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ProcessesForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "ChocoRenderLimits Processes";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView list;
		private System.Windows.Forms.ColumnHeader colarea;
		private System.Windows.Forms.ColumnHeader colgranularity;
		private System.Windows.Forms.ColumnHeader colthreads;
		private System.Windows.Forms.ColumnHeader colstatus;
		private System.Windows.Forms.Button newbutton;
		private System.Windows.Forms.Button importbutton;
		private System.Windows.Forms.Button removebutton;
		private System.Windows.Forms.Button closebutton;
		private System.Windows.Forms.Timer updatetimer;
	}
}