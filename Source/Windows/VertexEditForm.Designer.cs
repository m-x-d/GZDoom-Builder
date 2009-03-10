namespace CodeImp.DoomBuilder.Windows
{
	partial class VertexEditForm
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
			System.Windows.Forms.TabPage tabproperties;
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label6;
			this.groupposition = new System.Windows.Forms.GroupBox();
			this.positiony = new CodeImp.DoomBuilder.Controls.NumericTextbox();
			this.positionx = new CodeImp.DoomBuilder.Controls.NumericTextbox();
			this.tabs = new System.Windows.Forms.TabControl();
			this.tabcustom = new System.Windows.Forms.TabPage();
			this.fieldslist = new CodeImp.DoomBuilder.Controls.FieldsEditorControl();
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			tabproperties = new System.Windows.Forms.TabPage();
			label1 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			tabproperties.SuspendLayout();
			this.groupposition.SuspendLayout();
			this.tabs.SuspendLayout();
			this.tabcustom.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabproperties
			// 
			tabproperties.Controls.Add(this.groupposition);
			tabproperties.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			tabproperties.Location = new System.Drawing.Point(4, 23);
			tabproperties.Name = "tabproperties";
			tabproperties.Padding = new System.Windows.Forms.Padding(3);
			tabproperties.Size = new System.Drawing.Size(428, 206);
			tabproperties.TabIndex = 0;
			tabproperties.Text = "Properties";
			tabproperties.UseVisualStyleBackColor = true;
			// 
			// groupposition
			// 
			this.groupposition.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupposition.Controls.Add(label1);
			this.groupposition.Controls.Add(this.positiony);
			this.groupposition.Controls.Add(label6);
			this.groupposition.Controls.Add(this.positionx);
			this.groupposition.Location = new System.Drawing.Point(7, 6);
			this.groupposition.Name = "groupposition";
			this.groupposition.Size = new System.Drawing.Size(415, 194);
			this.groupposition.TabIndex = 24;
			this.groupposition.TabStop = false;
			this.groupposition.Text = " Position ";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(188, 39);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(18, 14);
			label1.TabIndex = 23;
			label1.Text = "Y:";
			// 
			// positiony
			// 
			this.positiony.AllowDecimal = false;
			this.positiony.AllowNegative = true;
			this.positiony.AllowRelative = true;
			this.positiony.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.positiony.Location = new System.Drawing.Point(211, 36);
			this.positiony.Name = "positiony";
			this.positiony.Size = new System.Drawing.Size(95, 20);
			this.positiony.TabIndex = 22;
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(45, 39);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(17, 14);
			label6.TabIndex = 21;
			label6.Text = "X:";
			// 
			// positionx
			// 
			this.positionx.AllowDecimal = false;
			this.positionx.AllowNegative = true;
			this.positionx.AllowRelative = true;
			this.positionx.ImeMode = System.Windows.Forms.ImeMode.Off;
			this.positionx.Location = new System.Drawing.Point(68, 36);
			this.positionx.Name = "positionx";
			this.positionx.Size = new System.Drawing.Size(95, 20);
			this.positionx.TabIndex = 20;
			// 
			// tabs
			// 
			this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabs.Controls.Add(tabproperties);
			this.tabs.Controls.Add(this.tabcustom);
			this.tabs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabs.Location = new System.Drawing.Point(10, 10);
			this.tabs.Margin = new System.Windows.Forms.Padding(1);
			this.tabs.Name = "tabs";
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(436, 233);
			this.tabs.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.tabs.TabIndex = 0;
			// 
			// tabcustom
			// 
			this.tabcustom.Controls.Add(this.fieldslist);
			this.tabcustom.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabcustom.Location = new System.Drawing.Point(4, 23);
			this.tabcustom.Name = "tabcustom";
			this.tabcustom.Padding = new System.Windows.Forms.Padding(3);
			this.tabcustom.Size = new System.Drawing.Size(428, 206);
			this.tabcustom.TabIndex = 1;
			this.tabcustom.Text = "Custom";
			this.tabcustom.UseVisualStyleBackColor = true;
			// 
			// fieldslist
			// 
			this.fieldslist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.fieldslist.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.fieldslist.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.fieldslist.Location = new System.Drawing.Point(11, 11);
			this.fieldslist.Margin = new System.Windows.Forms.Padding(8);
			this.fieldslist.Name = "fieldslist";
			this.fieldslist.Size = new System.Drawing.Size(406, 187);
			this.fieldslist.TabIndex = 2;
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(334, 259);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(112, 25);
			this.cancel.TabIndex = 21;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// apply
			// 
			this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.apply.Location = new System.Drawing.Point(215, 259);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 20;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// VertexEditForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(456, 294);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Controls.Add(this.tabs);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "VertexEditForm";
			this.Opacity = 0;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit Vertex";
			tabproperties.ResumeLayout(false);
			this.groupposition.ResumeLayout(false);
			this.groupposition.PerformLayout();
			this.tabs.ResumeLayout(false);
			this.tabcustom.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage tabcustom;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private CodeImp.DoomBuilder.Controls.FieldsEditorControl fieldslist;
		private CodeImp.DoomBuilder.Controls.NumericTextbox positiony;
		private System.Windows.Forms.GroupBox groupposition;
		private CodeImp.DoomBuilder.Controls.NumericTextbox positionx;
	}
}