namespace CodeImp.DoomBuilder.Interface
{
	partial class MapOptionsForm
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
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label1;
			System.Windows.Forms.GroupBox panelsettings;
			System.Windows.Forms.Label label4;
			this.levelname = new System.Windows.Forms.TextBox();
			this.config = new System.Windows.Forms.ComboBox();
			this.apply = new System.Windows.Forms.Button();
			this.cancel = new System.Windows.Forms.Button();
			this.panelres = new System.Windows.Forms.GroupBox();
			this.datalocations = new CodeImp.DoomBuilder.Interface.ResourceListEditor();
			label3 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			panelsettings = new System.Windows.Forms.GroupBox();
			label4 = new System.Windows.Forms.Label();
			panelsettings.SuspendLayout();
			this.panelres.SuspendLayout();
			this.SuspendLayout();
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new System.Drawing.Point(239, 76);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(90, 14);
			label3.TabIndex = 9;
			label3.Text = "example:  MAP01";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(58, 76);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(65, 14);
			label2.TabIndex = 7;
			label2.Text = "Level name:";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(18, 34);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(105, 14);
			label1.TabIndex = 5;
			label1.Text = "Game Configuration:";
			// 
			// panelsettings
			// 
			panelsettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			panelsettings.Controls.Add(label3);
			panelsettings.Controls.Add(this.levelname);
			panelsettings.Controls.Add(label2);
			panelsettings.Controls.Add(this.config);
			panelsettings.Controls.Add(label1);
			panelsettings.Location = new System.Drawing.Point(12, 12);
			panelsettings.Name = "panelsettings";
			panelsettings.Size = new System.Drawing.Size(367, 118);
			panelsettings.TabIndex = 10;
			panelsettings.TabStop = false;
			panelsettings.Text = " Settings ";
			// 
			// levelname
			// 
			this.levelname.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.levelname.Location = new System.Drawing.Point(129, 73);
			this.levelname.Name = "levelname";
			this.levelname.Size = new System.Drawing.Size(94, 20);
			this.levelname.TabIndex = 8;
			// 
			// config
			// 
			this.config.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.config.FormattingEnabled = true;
			this.config.Location = new System.Drawing.Point(129, 31);
			this.config.Name = "config";
			this.config.Size = new System.Drawing.Size(213, 22);
			this.config.TabIndex = 6;
			this.config.SelectedIndexChanged += new System.EventHandler(this.config_SelectedIndexChanged);
			// 
			// label4
			// 
			label4.Location = new System.Drawing.Point(15, 161);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(336, 34);
			label4.TabIndex = 17;
			label4.Text = "Drag items to change order (lower items override higher items). Grayed items are " +
				"loaded according to the game configuration.";
			// 
			// apply
			// 
			this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.apply.Location = new System.Drawing.Point(149, 355);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 12;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			this.apply.Click += new System.EventHandler(this.apply_Click);
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(267, 355);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(112, 25);
			this.cancel.TabIndex = 13;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			this.cancel.Click += new System.EventHandler(this.cancel_Click);
			// 
			// panelres
			// 
			this.panelres.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.panelres.Controls.Add(this.datalocations);
			this.panelres.Controls.Add(label4);
			this.panelres.Location = new System.Drawing.Point(12, 141);
			this.panelres.Name = "panelres";
			this.panelres.Size = new System.Drawing.Size(367, 198);
			this.panelres.TabIndex = 14;
			this.panelres.TabStop = false;
			this.panelres.Text = " Custom Resources ";
			// 
			// datalocations
			// 
			this.datalocations.DialogOffset = new System.Drawing.Point(40, 20);
			this.datalocations.Location = new System.Drawing.Point(15, 28);
			this.datalocations.Name = "datalocations";
			this.datalocations.Size = new System.Drawing.Size(336, 130);
			this.datalocations.TabIndex = 18;
			// 
			// MapOptionsForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(391, 392);
			this.Controls.Add(this.panelres);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Controls.Add(panelsettings);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MapOptionsForm";
			this.Opacity = 0;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Map Options";
			panelsettings.ResumeLayout(false);
			panelsettings.PerformLayout();
			this.panelres.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox levelname;
		private System.Windows.Forms.ComboBox config;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.GroupBox panelres;
		private ResourceListEditor datalocations;


	}
}