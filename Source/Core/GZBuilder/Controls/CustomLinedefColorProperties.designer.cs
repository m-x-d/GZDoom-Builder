namespace CodeImp.DoomBuilder.GZBuilder.Controls
{
	partial class CustomLinedefColorProperties
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
			this.gbLineColor = new System.Windows.Forms.GroupBox();
			this.labelErrors = new System.Windows.Forms.Label();
			this.tcLineSettings = new System.Windows.Forms.TabControl();
			this.tabFlags = new System.Windows.Forms.TabPage();
			this.flags = new System.Windows.Forms.CheckedListBox();
			this.cbUseFlags = new System.Windows.Forms.CheckBox();
			this.tabAction = new System.Windows.Forms.TabPage();
			this.cbUseAction = new System.Windows.Forms.CheckBox();
			this.tabActivation = new System.Windows.Forms.TabPage();
			this.activation = new System.Windows.Forms.ComboBox();
			this.cbUseActivation = new System.Windows.Forms.CheckBox();
			this.udmfactivates = new System.Windows.Forms.CheckedListBox();
			this.lineColor = new CodeImp.DoomBuilder.Controls.ColorControl();
			this.action = new CodeImp.DoomBuilder.Controls.ActionSelectorControl();
			this.gbLineColor.SuspendLayout();
			this.tcLineSettings.SuspendLayout();
			this.tabFlags.SuspendLayout();
			this.tabAction.SuspendLayout();
			this.tabActivation.SuspendLayout();
			this.SuspendLayout();
			// 
			// gbLineColor
			// 
			this.gbLineColor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.gbLineColor.Controls.Add(this.labelErrors);
			this.gbLineColor.Controls.Add(this.lineColor);
			this.gbLineColor.Location = new System.Drawing.Point(3, 3);
			this.gbLineColor.Name = "gbLineColor";
			this.gbLineColor.Size = new System.Drawing.Size(329, 42);
			this.gbLineColor.TabIndex = 13;
			this.gbLineColor.TabStop = false;
			// 
			// labelErrors
			// 
			this.labelErrors.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.labelErrors.ForeColor = System.Drawing.Color.Maroon;
			this.labelErrors.Location = new System.Drawing.Point(145, 10);
			this.labelErrors.Name = "labelErrors";
			this.labelErrors.Size = new System.Drawing.Size(178, 30);
			this.labelErrors.TabIndex = 1;
			this.labelErrors.Text = "Teh Error occured!";
			this.labelErrors.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tcLineSettings
			// 
			this.tcLineSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tcLineSettings.Controls.Add(this.tabFlags);
			this.tcLineSettings.Controls.Add(this.tabAction);
			this.tcLineSettings.Controls.Add(this.tabActivation);
			this.tcLineSettings.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.tcLineSettings.Location = new System.Drawing.Point(3, 50);
			this.tcLineSettings.Name = "tcLineSettings";
			this.tcLineSettings.SelectedIndex = 0;
			this.tcLineSettings.Size = new System.Drawing.Size(329, 264);
			this.tcLineSettings.TabIndex = 12;
			// 
			// tabFlags
			// 
			this.tabFlags.Controls.Add(this.flags);
			this.tabFlags.Controls.Add(this.cbUseFlags);
			this.tabFlags.Location = new System.Drawing.Point(4, 23);
			this.tabFlags.Name = "tabFlags";
			this.tabFlags.Padding = new System.Windows.Forms.Padding(3);
			this.tabFlags.Size = new System.Drawing.Size(321, 237);
			this.tabFlags.TabIndex = 0;
			this.tabFlags.Text = "Flags";
			this.tabFlags.UseVisualStyleBackColor = true;
			// 
			// flags
			// 
			this.flags.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.flags.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.flags.CheckOnClick = true;
			this.flags.FormattingEnabled = true;
			this.flags.Location = new System.Drawing.Point(6, 30);
			this.flags.Name = "flags";
			this.flags.Size = new System.Drawing.Size(309, 197);
			this.flags.TabIndex = 5;
			this.flags.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.flags_ItemCheck);
			this.flags.SelectedValueChanged += new System.EventHandler(this.flags_SelectedValueChanged);
			// 
			// cbUseFlags
			// 
			this.cbUseFlags.AutoSize = true;
			this.cbUseFlags.Location = new System.Drawing.Point(6, 6);
			this.cbUseFlags.Name = "cbUseFlags";
			this.cbUseFlags.Size = new System.Drawing.Size(72, 18);
			this.cbUseFlags.TabIndex = 4;
			this.cbUseFlags.Text = "Use flags";
			this.cbUseFlags.UseVisualStyleBackColor = true;
			this.cbUseFlags.CheckedChanged += new System.EventHandler(this.cbUseFlags_CheckedChanged);
			// 
			// tabAction
			// 
			this.tabAction.Controls.Add(this.action);
			this.tabAction.Controls.Add(this.cbUseAction);
			this.tabAction.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.tabAction.Location = new System.Drawing.Point(4, 23);
			this.tabAction.Name = "tabAction";
			this.tabAction.Padding = new System.Windows.Forms.Padding(3);
			this.tabAction.Size = new System.Drawing.Size(321, 237);
			this.tabAction.TabIndex = 1;
			this.tabAction.Text = "Action";
			this.tabAction.UseVisualStyleBackColor = true;
			// 
			// cbUseAction
			// 
			this.cbUseAction.AutoSize = true;
			this.cbUseAction.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.cbUseAction.Location = new System.Drawing.Point(6, 6);
			this.cbUseAction.Name = "cbUseAction";
			this.cbUseAction.Size = new System.Drawing.Size(78, 18);
			this.cbUseAction.TabIndex = 4;
			this.cbUseAction.Text = "Use Action";
			this.cbUseAction.UseVisualStyleBackColor = true;
			this.cbUseAction.CheckedChanged += new System.EventHandler(this.cbUseAction_CheckedChanged);
			// 
			// tabActivation
			// 
			this.tabActivation.Controls.Add(this.activation);
			this.tabActivation.Controls.Add(this.cbUseActivation);
			this.tabActivation.Controls.Add(this.udmfactivates);
			this.tabActivation.Location = new System.Drawing.Point(4, 23);
			this.tabActivation.Name = "tabActivation";
			this.tabActivation.Size = new System.Drawing.Size(321, 237);
			this.tabActivation.TabIndex = 2;
			this.tabActivation.Text = "Activation type";
			this.tabActivation.UseVisualStyleBackColor = true;
			// 
			// activation
			// 
			this.activation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.activation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.activation.FormattingEnabled = true;
			this.activation.Location = new System.Drawing.Point(3, 30);
			this.activation.Name = "activation";
			this.activation.Size = new System.Drawing.Size(315, 22);
			this.activation.TabIndex = 0;
			this.activation.SelectedIndexChanged += new System.EventHandler(this.activation_SelectedIndexChanged);
			// 
			// cbUseActivation
			// 
			this.cbUseActivation.AutoSize = true;
			this.cbUseActivation.Location = new System.Drawing.Point(6, 6);
			this.cbUseActivation.Name = "cbUseActivation";
			this.cbUseActivation.Size = new System.Drawing.Size(94, 18);
			this.cbUseActivation.TabIndex = 4;
			this.cbUseActivation.Text = "Use activation";
			this.cbUseActivation.UseVisualStyleBackColor = true;
			this.cbUseActivation.CheckedChanged += new System.EventHandler(this.cbUseActivation_CheckedChanged);
			// 
			// udmfactivates
			// 
			this.udmfactivates.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.udmfactivates.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.udmfactivates.CheckOnClick = true;
			this.udmfactivates.FormattingEnabled = true;
			this.udmfactivates.Location = new System.Drawing.Point(3, 30);
			this.udmfactivates.Name = "udmfactivates";
			this.udmfactivates.Size = new System.Drawing.Size(315, 197);
			this.udmfactivates.TabIndex = 6;
			this.udmfactivates.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.flags_ItemCheck);
			this.udmfactivates.SelectedValueChanged += new System.EventHandler(this.udmfactivates_SelectedValueChanged);
			// 
			// lineColor
			// 
			this.lineColor.BackColor = System.Drawing.Color.Transparent;
			this.lineColor.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lineColor.Label = "Linedef color:";
			this.lineColor.Location = new System.Drawing.Point(6, 13);
			this.lineColor.MaximumSize = new System.Drawing.Size(10000, 23);
			this.lineColor.MinimumSize = new System.Drawing.Size(100, 23);
			this.lineColor.Name = "lineColor";
			this.lineColor.Size = new System.Drawing.Size(132, 23);
			this.lineColor.TabIndex = 0;
			this.lineColor.ColorChanged += new System.EventHandler(this.lineColor_ColorChanged);
			// 
			// action
			// 
			this.action.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.action.BackColor = System.Drawing.Color.Transparent;
			this.action.Cursor = System.Windows.Forms.Cursors.Default;
			this.action.Empty = false;
			this.action.GeneralizedCategories = null;
			this.action.Location = new System.Drawing.Point(6, 30);
			this.action.Name = "action";
			this.action.Size = new System.Drawing.Size(309, 21);
			this.action.TabIndex = 6;
			this.action.Value = 0;
			this.action.ValueChanges += new System.EventHandler(this.action_ValueChanges);
			// 
			// CustomLinedefColorProperties
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.gbLineColor);
			this.Controls.Add(this.tcLineSettings);
			this.Name = "CustomLinedefColorProperties";
			this.Size = new System.Drawing.Size(337, 319);
			this.gbLineColor.ResumeLayout(false);
			this.tcLineSettings.ResumeLayout(false);
			this.tabFlags.ResumeLayout(false);
			this.tabFlags.PerformLayout();
			this.tabAction.ResumeLayout(false);
			this.tabAction.PerformLayout();
			this.tabActivation.ResumeLayout(false);
			this.tabActivation.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox gbLineColor;
		private System.Windows.Forms.Label labelErrors;
		private CodeImp.DoomBuilder.Controls.ColorControl lineColor;
		private System.Windows.Forms.TabControl tcLineSettings;
		private System.Windows.Forms.TabPage tabFlags;
		private System.Windows.Forms.CheckedListBox flags;
		private System.Windows.Forms.CheckBox cbUseFlags;
		private System.Windows.Forms.TabPage tabAction;
		private System.Windows.Forms.CheckBox cbUseAction;
		private System.Windows.Forms.TabPage tabActivation;
		private System.Windows.Forms.ComboBox activation;
		private System.Windows.Forms.CheckBox cbUseActivation;
		private System.Windows.Forms.CheckedListBox udmfactivates;
		private CodeImp.DoomBuilder.Controls.ActionSelectorControl action;
	}
}
