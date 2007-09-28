namespace CodeImp.DoomBuilder.Interface
{
	partial class ParametersForm
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ParametersForm));
			this.axCodeSense1 = new AxCodeSense.AxCodeSense();
			((System.ComponentModel.ISupportInitialize)(this.axCodeSense1)).BeginInit();
			this.SuspendLayout();
			// 
			// axCodeSense1
			// 
			this.axCodeSense1.Location = new System.Drawing.Point(12, 182);
			this.axCodeSense1.Name = "axCodeSense1";
			this.axCodeSense1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axCodeSense1.OcxState")));
			this.axCodeSense1.Size = new System.Drawing.Size(458, 21);
			this.axCodeSense1.TabIndex = 0;
			// 
			// ParametersForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(482, 273);
			this.Controls.Add(this.axCodeSense1);
			this.Name = "ParametersForm";
			this.Text = "ParametersForm";
			((System.ComponentModel.ISupportInitialize)(this.axCodeSense1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private AxCodeSense.AxCodeSense axCodeSense1;
	}
}