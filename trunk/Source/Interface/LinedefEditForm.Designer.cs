namespace CodeImp.DoomBuilder.Interface
{
	partial class LinedefEditForm
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
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label5;
			System.Windows.Forms.Label label6;
			System.Windows.Forms.Label label7;
			System.Windows.Forms.Label label8;
			System.Windows.Forms.Label label9;
			System.Windows.Forms.Label label10;
			this.cancel = new System.Windows.Forms.Button();
			this.apply = new System.Windows.Forms.Button();
			this.actiongroup = new System.Windows.Forms.GroupBox();
			this.newtag = new System.Windows.Forms.Button();
			this.tag = new CodeImp.DoomBuilder.Interface.NumericTextbox();
			this.action = new CodeImp.DoomBuilder.Interface.ActionSelectorControl();
			this.browseaction = new System.Windows.Forms.Button();
			this.settingsgroup = new System.Windows.Forms.GroupBox();
			this.flags = new CodeImp.DoomBuilder.Interface.CheckboxArrayControl();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.tabs = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.backside = new System.Windows.Forms.CheckBox();
			this.backgroup = new System.Windows.Forms.GroupBox();
			this.backlow = new CodeImp.DoomBuilder.Interface.TextureSelectorControl();
			this.backmid = new CodeImp.DoomBuilder.Interface.TextureSelectorControl();
			this.backhigh = new CodeImp.DoomBuilder.Interface.TextureSelectorControl();
			this.backoffsety = new CodeImp.DoomBuilder.Interface.NumericTextbox();
			this.backoffsetx = new CodeImp.DoomBuilder.Interface.NumericTextbox();
			this.backsector = new System.Windows.Forms.Button();
			this.frontside = new System.Windows.Forms.CheckBox();
			this.frontgroup = new System.Windows.Forms.GroupBox();
			this.frontlow = new CodeImp.DoomBuilder.Interface.TextureSelectorControl();
			this.frontmid = new CodeImp.DoomBuilder.Interface.TextureSelectorControl();
			this.fronthigh = new CodeImp.DoomBuilder.Interface.TextureSelectorControl();
			this.frontoffsety = new CodeImp.DoomBuilder.Interface.NumericTextbox();
			this.frontoffsetx = new CodeImp.DoomBuilder.Interface.NumericTextbox();
			this.frontsector = new System.Windows.Forms.Button();
			label2 = new System.Windows.Forms.Label();
			label1 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label5 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			label7 = new System.Windows.Forms.Label();
			label8 = new System.Windows.Forms.Label();
			label9 = new System.Windows.Forms.Label();
			label10 = new System.Windows.Forms.Label();
			this.actiongroup.SuspendLayout();
			this.settingsgroup.SuspendLayout();
			this.tabs.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.backgroup.SuspendLayout();
			this.frontgroup.SuspendLayout();
			this.SuspendLayout();
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new System.Drawing.Point(15, 30);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(41, 14);
			label2.TabIndex = 9;
			label2.Text = "Action:";
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(28, 78);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(28, 14);
			label1.TabIndex = 6;
			label1.Text = "Tag:";
			// 
			// label3
			// 
			label3.Location = new System.Drawing.Point(252, 18);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(68, 16);
			label3.TabIndex = 3;
			label3.Text = "Upper";
			label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label4
			// 
			label4.Location = new System.Drawing.Point(334, 18);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(68, 16);
			label4.TabIndex = 4;
			label4.Text = "Middle";
			label4.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label5
			// 
			label5.Location = new System.Drawing.Point(416, 18);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(68, 16);
			label5.TabIndex = 5;
			label5.Text = "Lower";
			label5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Location = new System.Drawing.Point(16, 104);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(81, 14);
			label6.TabIndex = 7;
			label6.Text = "Texture Offset:";
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(16, 104);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(81, 14);
			label7.TabIndex = 7;
			label7.Text = "Texture Offset:";
			// 
			// label8
			// 
			label8.Location = new System.Drawing.Point(416, 18);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(68, 16);
			label8.TabIndex = 5;
			label8.Text = "Lower";
			label8.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label9
			// 
			label9.Location = new System.Drawing.Point(334, 18);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(68, 16);
			label9.TabIndex = 4;
			label9.Text = "Middle";
			label9.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// label10
			// 
			label10.Location = new System.Drawing.Point(252, 18);
			label10.Name = "label10";
			label10.Size = new System.Drawing.Size(68, 16);
			label10.TabIndex = 3;
			label10.Text = "Upper";
			label10.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// cancel
			// 
			this.cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancel.Location = new System.Drawing.Point(421, 359);
			this.cancel.Name = "cancel";
			this.cancel.Size = new System.Drawing.Size(112, 25);
			this.cancel.TabIndex = 17;
			this.cancel.Text = "Cancel";
			this.cancel.UseVisualStyleBackColor = true;
			// 
			// apply
			// 
			this.apply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.apply.Location = new System.Drawing.Point(302, 359);
			this.apply.Name = "apply";
			this.apply.Size = new System.Drawing.Size(112, 25);
			this.apply.TabIndex = 16;
			this.apply.Text = "OK";
			this.apply.UseVisualStyleBackColor = true;
			// 
			// actiongroup
			// 
			this.actiongroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.actiongroup.Controls.Add(label2);
			this.actiongroup.Controls.Add(this.newtag);
			this.actiongroup.Controls.Add(this.tag);
			this.actiongroup.Controls.Add(label1);
			this.actiongroup.Controls.Add(this.action);
			this.actiongroup.Controls.Add(this.browseaction);
			this.actiongroup.Location = new System.Drawing.Point(8, 169);
			this.actiongroup.Name = "actiongroup";
			this.actiongroup.Size = new System.Drawing.Size(499, 128);
			this.actiongroup.TabIndex = 18;
			this.actiongroup.TabStop = false;
			this.actiongroup.Text = " Action ";
			// 
			// newtag
			// 
			this.newtag.Location = new System.Drawing.Point(136, 74);
			this.newtag.Name = "newtag";
			this.newtag.Size = new System.Drawing.Size(76, 23);
			this.newtag.TabIndex = 8;
			this.newtag.Text = "New Tag";
			this.newtag.UseVisualStyleBackColor = true;
			// 
			// tag
			// 
			this.tag.Location = new System.Drawing.Point(62, 75);
			this.tag.Name = "tag";
			this.tag.Size = new System.Drawing.Size(68, 20);
			this.tag.TabIndex = 7;
			// 
			// action
			// 
			this.action.BackColor = System.Drawing.SystemColors.Control;
			this.action.Cursor = System.Windows.Forms.Cursors.Default;
			this.action.Empty = false;
			this.action.Location = new System.Drawing.Point(62, 27);
			this.action.Name = "action";
			this.action.Size = new System.Drawing.Size(386, 21);
			this.action.TabIndex = 5;
			this.action.Value = 402;
			// 
			// browseaction
			// 
			this.browseaction.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.browseaction.Image = global::CodeImp.DoomBuilder.Properties.Resources.treeview;
			this.browseaction.Location = new System.Drawing.Point(454, 26);
			this.browseaction.Name = "browseaction";
			this.browseaction.Padding = new System.Windows.Forms.Padding(0, 0, 1, 3);
			this.browseaction.Size = new System.Drawing.Size(30, 23);
			this.browseaction.TabIndex = 3;
			this.browseaction.Text = " ";
			this.browseaction.UseVisualStyleBackColor = true;
			// 
			// settingsgroup
			// 
			this.settingsgroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.settingsgroup.Controls.Add(this.flags);
			this.settingsgroup.Location = new System.Drawing.Point(8, 8);
			this.settingsgroup.Name = "settingsgroup";
			this.settingsgroup.Size = new System.Drawing.Size(499, 152);
			this.settingsgroup.TabIndex = 19;
			this.settingsgroup.TabStop = false;
			this.settingsgroup.Text = " Settings ";
			// 
			// flags
			// 
			this.flags.AutoScroll = true;
			this.flags.Columns = 3;
			this.flags.Location = new System.Drawing.Point(18, 26);
			this.flags.Name = "flags";
			this.flags.Size = new System.Drawing.Size(475, 119);
			this.flags.TabIndex = 0;
			// 
			// checkBox1
			// 
			this.checkBox1.Location = new System.Drawing.Point(0, 0);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(104, 24);
			this.checkBox1.TabIndex = 0;
			this.checkBox1.Text = "checkBox1";
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// tabs
			// 
			this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tabs.Controls.Add(this.tabPage1);
			this.tabs.Controls.Add(this.tabPage2);
			this.tabs.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabs.Location = new System.Drawing.Point(10, 10);
			this.tabs.Margin = new System.Windows.Forms.Padding(1);
			this.tabs.Name = "tabs";
			this.tabs.SelectedIndex = 0;
			this.tabs.Size = new System.Drawing.Size(523, 332);
			this.tabs.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
			this.tabs.TabIndex = 20;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.settingsgroup);
			this.tabPage1.Controls.Add(this.actiongroup);
			this.tabPage1.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabPage1.Location = new System.Drawing.Point(4, 23);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(5);
			this.tabPage1.Size = new System.Drawing.Size(515, 305);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Properties";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.backside);
			this.tabPage2.Controls.Add(this.backgroup);
			this.tabPage2.Controls.Add(this.frontside);
			this.tabPage2.Controls.Add(this.frontgroup);
			this.tabPage2.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabPage2.Location = new System.Drawing.Point(4, 23);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(5);
			this.tabPage2.Size = new System.Drawing.Size(515, 305);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Sidedefs";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// backside
			// 
			this.backside.AutoSize = true;
			this.backside.Location = new System.Drawing.Point(20, 155);
			this.backside.Name = "backside";
			this.backside.Size = new System.Drawing.Size(74, 18);
			this.backside.TabIndex = 2;
			this.backside.Text = "Back Side";
			this.backside.UseVisualStyleBackColor = true;
			// 
			// backgroup
			// 
			this.backgroup.Controls.Add(this.backlow);
			this.backgroup.Controls.Add(this.backmid);
			this.backgroup.Controls.Add(this.backhigh);
			this.backgroup.Controls.Add(this.backoffsety);
			this.backgroup.Controls.Add(this.backoffsetx);
			this.backgroup.Controls.Add(label7);
			this.backgroup.Controls.Add(this.backsector);
			this.backgroup.Controls.Add(label8);
			this.backgroup.Controls.Add(label9);
			this.backgroup.Controls.Add(label10);
			this.backgroup.Location = new System.Drawing.Point(8, 157);
			this.backgroup.Name = "backgroup";
			this.backgroup.Size = new System.Drawing.Size(499, 140);
			this.backgroup.TabIndex = 1;
			this.backgroup.TabStop = false;
			this.backgroup.Text = "     ";
			// 
			// backlow
			// 
			this.backlow.Location = new System.Drawing.Point(416, 37);
			this.backlow.Name = "backlow";
			this.backlow.Required = false;
			this.backlow.Size = new System.Drawing.Size(68, 84);
			this.backlow.TabIndex = 15;
			this.backlow.TextureName = "";
			// 
			// backmid
			// 
			this.backmid.Location = new System.Drawing.Point(334, 37);
			this.backmid.Name = "backmid";
			this.backmid.Required = false;
			this.backmid.Size = new System.Drawing.Size(68, 84);
			this.backmid.TabIndex = 14;
			this.backmid.TextureName = "";
			// 
			// backhigh
			// 
			this.backhigh.Location = new System.Drawing.Point(252, 37);
			this.backhigh.Name = "backhigh";
			this.backhigh.Required = false;
			this.backhigh.Size = new System.Drawing.Size(68, 84);
			this.backhigh.TabIndex = 13;
			this.backhigh.TextureName = "";
			// 
			// backoffsety
			// 
			this.backoffsety.Location = new System.Drawing.Point(154, 101);
			this.backoffsety.Name = "backoffsety";
			this.backoffsety.Size = new System.Drawing.Size(45, 20);
			this.backoffsety.TabIndex = 9;
			// 
			// backoffsetx
			// 
			this.backoffsetx.Location = new System.Drawing.Point(103, 101);
			this.backoffsetx.Name = "backoffsetx";
			this.backoffsetx.Size = new System.Drawing.Size(45, 20);
			this.backoffsetx.TabIndex = 8;
			// 
			// backsector
			// 
			this.backsector.Location = new System.Drawing.Point(103, 37);
			this.backsector.Name = "backsector";
			this.backsector.Size = new System.Drawing.Size(96, 24);
			this.backsector.TabIndex = 6;
			this.backsector.Text = "Select Sector";
			this.backsector.UseVisualStyleBackColor = true;
			// 
			// frontside
			// 
			this.frontside.AutoSize = true;
			this.frontside.Location = new System.Drawing.Point(20, 6);
			this.frontside.Name = "frontside";
			this.frontside.Size = new System.Drawing.Size(75, 18);
			this.frontside.TabIndex = 0;
			this.frontside.Text = "Front Side";
			this.frontside.UseVisualStyleBackColor = true;
			// 
			// frontgroup
			// 
			this.frontgroup.Controls.Add(this.frontlow);
			this.frontgroup.Controls.Add(this.frontmid);
			this.frontgroup.Controls.Add(this.fronthigh);
			this.frontgroup.Controls.Add(this.frontoffsety);
			this.frontgroup.Controls.Add(this.frontoffsetx);
			this.frontgroup.Controls.Add(label6);
			this.frontgroup.Controls.Add(this.frontsector);
			this.frontgroup.Controls.Add(label5);
			this.frontgroup.Controls.Add(label4);
			this.frontgroup.Controls.Add(label3);
			this.frontgroup.Location = new System.Drawing.Point(8, 8);
			this.frontgroup.Name = "frontgroup";
			this.frontgroup.Size = new System.Drawing.Size(499, 140);
			this.frontgroup.TabIndex = 0;
			this.frontgroup.TabStop = false;
			this.frontgroup.Text = "     ";
			// 
			// frontlow
			// 
			this.frontlow.Location = new System.Drawing.Point(416, 37);
			this.frontlow.Name = "frontlow";
			this.frontlow.Required = false;
			this.frontlow.Size = new System.Drawing.Size(68, 84);
			this.frontlow.TabIndex = 12;
			this.frontlow.TextureName = "";
			// 
			// frontmid
			// 
			this.frontmid.Location = new System.Drawing.Point(334, 37);
			this.frontmid.Name = "frontmid";
			this.frontmid.Required = false;
			this.frontmid.Size = new System.Drawing.Size(68, 84);
			this.frontmid.TabIndex = 11;
			this.frontmid.TextureName = "";
			// 
			// fronthigh
			// 
			this.fronthigh.Location = new System.Drawing.Point(252, 37);
			this.fronthigh.Name = "fronthigh";
			this.fronthigh.Required = false;
			this.fronthigh.Size = new System.Drawing.Size(68, 84);
			this.fronthigh.TabIndex = 10;
			this.fronthigh.TextureName = "";
			// 
			// frontoffsety
			// 
			this.frontoffsety.Location = new System.Drawing.Point(154, 101);
			this.frontoffsety.Name = "frontoffsety";
			this.frontoffsety.Size = new System.Drawing.Size(45, 20);
			this.frontoffsety.TabIndex = 9;
			// 
			// frontoffsetx
			// 
			this.frontoffsetx.Location = new System.Drawing.Point(103, 101);
			this.frontoffsetx.Name = "frontoffsetx";
			this.frontoffsetx.Size = new System.Drawing.Size(45, 20);
			this.frontoffsetx.TabIndex = 8;
			// 
			// frontsector
			// 
			this.frontsector.Location = new System.Drawing.Point(103, 37);
			this.frontsector.Name = "frontsector";
			this.frontsector.Size = new System.Drawing.Size(96, 24);
			this.frontsector.TabIndex = 6;
			this.frontsector.Text = "Select Sector";
			this.frontsector.UseVisualStyleBackColor = true;
			// 
			// LinedefEditForm
			// 
			this.AcceptButton = this.apply;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.cancel;
			this.ClientSize = new System.Drawing.Size(543, 394);
			this.Controls.Add(this.tabs);
			this.Controls.Add(this.cancel);
			this.Controls.Add(this.apply);
			this.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LinedefEditForm";
			this.Opacity = 0;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Edit Linedefs";
			this.actiongroup.ResumeLayout(false);
			this.actiongroup.PerformLayout();
			this.settingsgroup.ResumeLayout(false);
			this.tabs.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.backgroup.ResumeLayout(false);
			this.backgroup.PerformLayout();
			this.frontgroup.ResumeLayout(false);
			this.frontgroup.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button cancel;
		private System.Windows.Forms.Button apply;
		private System.Windows.Forms.GroupBox actiongroup;
		private System.Windows.Forms.GroupBox settingsgroup;
		private CheckboxArrayControl flags;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.Button browseaction;
		private ActionSelectorControl action;
		private NumericTextbox tag;
		private System.Windows.Forms.Button newtag;
		private System.Windows.Forms.TabControl tabs;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.GroupBox frontgroup;
		private System.Windows.Forms.CheckBox frontside;
		private System.Windows.Forms.Button frontsector;
		private NumericTextbox frontoffsety;
		private NumericTextbox frontoffsetx;
		private System.Windows.Forms.CheckBox backside;
		private System.Windows.Forms.GroupBox backgroup;
		private NumericTextbox backoffsety;
		private NumericTextbox backoffsetx;
		private System.Windows.Forms.Button backsector;
		private TextureSelectorControl frontlow;
		private TextureSelectorControl frontmid;
		private TextureSelectorControl fronthigh;
		private TextureSelectorControl backlow;
		private TextureSelectorControl backmid;
		private TextureSelectorControl backhigh;
	}
}