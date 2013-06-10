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
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label6;
			this.groupposition = new System.Windows.Forms.GroupBox();
			this.zfloor = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.zceiling = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.positiony = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.positionx = new CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox();
			this.tabs = new System.Windows.Forms.TabControl();
			this.tabcustom = new System.Windows.Forms.TabPage();
			this.fieldslist = new CodeImp.DoomBuilder.Controls.FieldsEditorControl();
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			this.panelHeightControls = new System.Windows.Forms.Panel();
			tabproperties = new System.Windows.Forms.TabPage();
			label3 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			tabproperties.SuspendLayout();
			this.groupposition.SuspendLayout();
			this.tabs.SuspendLayout();
			this.tabcustom.SuspendLayout();
			this.panelHeightControls.SuspendLayout();
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
			this.groupposition.Controls.Add(this.panelHeightControls);
			this.groupposition.Controls.Add(this.positiony);
			this.groupposition.Controls.Add(this.positionx);
			this.groupposition.Controls.Add(label1);
			this.groupposition.Controls.Add(label6);
			this.groupposition.Location = new System.Drawing.Point(7, 6);
			this.groupposition.Name = "groupposition";
			this.groupposition.Size = new System.Drawing.Size(415, 194);
			this.groupposition.TabIndex = 0;
			this.groupposition.TabStop = false;
			this.groupposition.Text = " Position ";
			// 
			// zfloor
			// 
			this.zfloor.AllowDecimal = false;
			this.zfloor.AllowNegative = true;
			this.zfloor.AllowRelative = true;
			this.zfloor.ButtonStep = 1;
			this.zfloor.Location = new System.Drawing.Point(188, 32);
			this.zfloor.Name = "zfloor";
			this.zfloor.Size = new System.Drawing.Size(120, 24);
			this.zfloor.StepValues = null;
			this.zfloor.TabIndex = 29;
			// 
			// zceiling
			// 
			this.zceiling.AllowDecimal = false;
			this.zceiling.AllowNegative = true;
			this.zceiling.AllowRelative = true;
			this.zceiling.ButtonStep = 1;
			this.zceiling.Location = new System.Drawing.Point(188, 0);
			this.zceiling.Name = "zceiling";
			this.zceiling.Size = new System.Drawing.Size(120, 24);
			this.zceiling.StepValues = null;
			this.zceiling.TabIndex = 28;
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(63, 5);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(118, 14);
			label3.TabIndex = 27;
			label3.Text = "Absolute ceiling height:";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(71, 37);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(110, 14);
			label2.TabIndex = 26;
			label2.Text = "Absolute floor height:";
			// 
			// positiony
			// 
			this.positiony.AllowDecimal = false;
			this.positiony.AllowNegative = true;
			this.positiony.AllowRelative = true;
			this.positiony.ButtonStep = 1;
			this.positiony.Location = new System.Drawing.Point(236, 34);
			this.positiony.Name = "positiony";
			this.positiony.Size = new System.Drawing.Size(120, 24);
			this.positiony.StepValues = null;
			this.positiony.TabIndex = 25;
			// 
			// positionx
			// 
			this.positionx.AllowDecimal = false;
			this.positionx.AllowNegative = true;
			this.positionx.AllowRelative = true;
			this.positionx.ButtonStep = 1;
			this.positionx.Location = new System.Drawing.Point(68, 34);
			this.positionx.Name = "positionx";
			this.positionx.Size = new System.Drawing.Size(120, 24);
			this.positionx.StepValues = null;
			this.positionx.TabIndex = 24;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(212, 39);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(17, 14);
			label1.TabIndex = 23;
			label1.Text = "Y:";
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
			this.fieldslist.AllowInsert = true;
			this.fieldslist.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.fieldslist.AutoInsertUserPrefix = true;
			this.fieldslist.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.fieldslist.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.fieldslist.Location = new System.Drawing.Point(11, 11);
			this.fieldslist.Margin = new System.Windows.Forms.Padding(8);
			this.fieldslist.Name = "fieldslist";
			this.fieldslist.PropertyColumnVisible = true;
			this.fieldslist.PropertyColumnWidth = 150;
			this.fieldslist.Size = new System.Drawing.Size(406, 187);
			this.fieldslist.TabIndex = 2;
			this.fieldslist.TypeColumnVisible = true;
			this.fieldslist.TypeColumnWidth = 100;
			this.fieldslist.ValueColumnVisible = true;
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(334, 259);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(112, 25);
			this.cancel.TabIndex = 2;
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
			this.apply.TabIndex = 1;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// panelHeightControls
			// 
			this.panelHeightControls.Controls.Add(this.zceiling);
			this.panelHeightControls.Controls.Add(this.zfloor);
			this.panelHeightControls.Controls.Add(label2);
			this.panelHeightControls.Controls.Add(label3);
			this.panelHeightControls.Location = new System.Drawing.Point(48, 73);
			this.panelHeightControls.Name = "panelHeightControls";
			this.panelHeightControls.Size = new System.Drawing.Size(308, 100);
			this.panelHeightControls.TabIndex = 30;
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
			this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.VertexEditForm_HelpRequested);
			tabproperties.ResumeLayout(false);
			this.groupposition.ResumeLayout(false);
			this.groupposition.PerformLayout();
			this.tabs.ResumeLayout(false);
			this.tabcustom.ResumeLayout(false);
			this.panelHeightControls.ResumeLayout(false);
			this.panelHeightControls.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage tabcustom;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private CodeImp.DoomBuilder.Controls.FieldsEditorControl fieldslist;
		private System.Windows.Forms.GroupBox groupposition;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox positiony;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox positionx;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox zfloor;
		private CodeImp.DoomBuilder.Controls.ButtonsNumericTextbox zceiling;
		private System.Windows.Forms.Panel panelHeightControls;
	}
}