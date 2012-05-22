namespace CodeImp.DoomBuilder.Plugins.ChocoRenderLimits
{
	partial class TestSetupForm
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
			this.arealabel = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.granularity = new System.Windows.Forms.TrackBar();
			this.granularitylabel = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.processeslabel = new System.Windows.Forms.Label();
			this.processes = new System.Windows.Forms.TrackBar();
			this.cancelbutton = new System.Windows.Forms.Button();
			this.startbutton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.granularity)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.processes)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(59, 31);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(34, 14);
			this.label1.TabIndex = 0;
			this.label1.Text = "Area:";
			// 
			// arealabel
			// 
			this.arealabel.AutoSize = true;
			this.arealabel.Location = new System.Drawing.Point(99, 31);
			this.arealabel.Name = "arealabel";
			this.arealabel.Size = new System.Drawing.Size(139, 14);
			this.arealabel.TabIndex = 1;
			this.arealabel.Text = "(233, 2030)   ̶   (-35, -2412)";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(30, 75);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(63, 14);
			this.label2.TabIndex = 2;
			this.label2.Text = "Granularity:";
			// 
			// granularity
			// 
			this.granularity.Location = new System.Drawing.Point(99, 70);
			this.granularity.Maximum = 6;
			this.granularity.Name = "granularity";
			this.granularity.Size = new System.Drawing.Size(152, 45);
			this.granularity.TabIndex = 3;
			this.granularity.Value = 3;
			this.granularity.ValueChanged += new System.EventHandler(this.granularity_ValueChanged);
			// 
			// granularitylabel
			// 
			this.granularitylabel.AutoSize = true;
			this.granularitylabel.Location = new System.Drawing.Point(257, 75);
			this.granularitylabel.Name = "granularitylabel";
			this.granularitylabel.Size = new System.Drawing.Size(36, 14);
			this.granularitylabel.TabIndex = 5;
			this.granularitylabel.Text = "32 mp";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(30, 122);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(62, 14);
			this.label3.TabIndex = 6;
			this.label3.Text = "Processes:";
			// 
			// processeslabel
			// 
			this.processeslabel.AutoSize = true;
			this.processeslabel.Location = new System.Drawing.Point(257, 122);
			this.processeslabel.Name = "processeslabel";
			this.processeslabel.Size = new System.Drawing.Size(13, 14);
			this.processeslabel.TabIndex = 8;
			this.processeslabel.Text = "1";
			// 
			// processes
			// 
			this.processes.Location = new System.Drawing.Point(99, 121);
			this.processes.Maximum = 16;
			this.processes.Minimum = 1;
			this.processes.Name = "processes";
			this.processes.Size = new System.Drawing.Size(152, 45);
			this.processes.TabIndex = 7;
			this.processes.Value = 1;
			this.processes.ValueChanged += new System.EventHandler(this.processes_ValueChanged);
			// 
			// cancelbutton
			// 
			this.cancelbutton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.cancelbutton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelbutton.Location = new System.Drawing.Point(175, 201);
			this.cancelbutton.Name = "cancelbutton";
			this.cancelbutton.Size = new System.Drawing.Size(112, 25);
			this.cancelbutton.TabIndex = 10;
			this.cancelbutton.Text = "Cancel";
			this.cancelbutton.UseVisualStyleBackColor = true;
			this.cancelbutton.Click += new System.EventHandler(this.cancelbutton_Click);
			// 
			// startbutton
			// 
			this.startbutton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.startbutton.Location = new System.Drawing.Point(57, 201);
			this.startbutton.Name = "startbutton";
			this.startbutton.Size = new System.Drawing.Size(112, 25);
			this.startbutton.TabIndex = 9;
			this.startbutton.Text = "Start";
			this.startbutton.UseVisualStyleBackColor = true;
			this.startbutton.Click += new System.EventHandler(this.startbutton_Click);
			// 
			// TestSetupForm
			// 
			this.AcceptButton = this.startbutton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancelbutton;
			this.ClientSize = new System.Drawing.Size(344, 238);
			this.Controls.Add(this.cancelbutton);
			this.Controls.Add(this.startbutton);
			this.Controls.Add(this.processeslabel);
			this.Controls.Add(this.processes);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.granularitylabel);
			this.Controls.Add(this.granularity);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.arealabel);
			this.Controls.Add(this.label1);
			this.DoubleBuffered = true;
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TestSetupForm";
			this.ShowIcon = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "ChocoRenderLimits Test Setup";
			((System.ComponentModel.ISupportInitialize)(this.granularity)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.processes)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label arealabel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TrackBar granularity;
		private System.Windows.Forms.Label granularitylabel;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label processeslabel;
		private System.Windows.Forms.TrackBar processes;
		private System.Windows.Forms.Button cancelbutton;
		private System.Windows.Forms.Button startbutton;
	}
}