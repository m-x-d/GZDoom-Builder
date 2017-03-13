﻿namespace CodeImp.DoomBuilder.Controls
{
	partial class ArgumentsControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.splitcontainer = new System.Windows.Forms.SplitContainer();
            this.arg0str = new System.Windows.Forms.TextBox();
            this.cbuseargstr = new System.Windows.Forms.CheckBox();
            this.arg0named = new CodeImp.DoomBuilder.Controls.ColoredComboBox();
            this.arg0int = new CodeImp.DoomBuilder.Controls.ColoredComboBox();
            this.arg0label = new System.Windows.Forms.Label();
            this.arg1label = new System.Windows.Forms.Label();
            this.arg2label = new System.Windows.Forms.Label();
            this.arg0 = new CodeImp.DoomBuilder.Controls.ArgumentBox();
            this.arg1 = new CodeImp.DoomBuilder.Controls.ArgumentBox();
            this.arg2 = new CodeImp.DoomBuilder.Controls.ArgumentBox();
            this.arg3label = new System.Windows.Forms.Label();
            this.arg4label = new System.Windows.Forms.Label();
            this.arg3 = new CodeImp.DoomBuilder.Controls.ArgumentBox();
            this.arg4 = new CodeImp.DoomBuilder.Controls.ArgumentBox();
            this.tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.splitcontainer.Panel1.SuspendLayout();
            this.splitcontainer.Panel2.SuspendLayout();
            this.splitcontainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitcontainer
            // 
            this.splitcontainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitcontainer.IsSplitterFixed = true;
            this.splitcontainer.Location = new System.Drawing.Point(0, 0);
            this.splitcontainer.Name = "splitcontainer";
            // 
            // splitcontainer.Panel1
            // 
            this.splitcontainer.Panel1.Controls.Add(this.arg0str);
            this.splitcontainer.Panel1.Controls.Add(this.cbuseargstr);
            this.splitcontainer.Panel1.Controls.Add(this.arg0named);
            this.splitcontainer.Panel1.Controls.Add(this.arg0int);
            this.splitcontainer.Panel1.Controls.Add(this.arg0label);
            this.splitcontainer.Panel1.Controls.Add(this.arg1label);
            this.splitcontainer.Panel1.Controls.Add(this.arg2label);
            this.splitcontainer.Panel1.Controls.Add(this.arg0);
            this.splitcontainer.Panel1.Controls.Add(this.arg1);
            this.splitcontainer.Panel1.Controls.Add(this.arg2);
            // 
            // splitcontainer.Panel2
            // 
            this.splitcontainer.Panel2.Controls.Add(this.arg3label);
            this.splitcontainer.Panel2.Controls.Add(this.arg4label);
            this.splitcontainer.Panel2.Controls.Add(this.arg3);
            this.splitcontainer.Panel2.Controls.Add(this.arg4);
            this.splitcontainer.Size = new System.Drawing.Size(700, 140);
            this.splitcontainer.SplitterDistance = 350;
            this.splitcontainer.SplitterWidth = 1;
            this.splitcontainer.TabIndex = 0;
            this.splitcontainer.TabStop = false;
            // 
            // arg0str
            // 
            this.arg0str.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.arg0str.Location = new System.Drawing.Point(237, 131);
            this.arg0str.Name = "arg0str";
            this.arg0str.Size = new System.Drawing.Size(110, 20);
            this.arg0str.TabIndex = 45;
            this.arg0str.Visible = false;
            // 
            // cbuseargstr
            // 
            this.cbuseargstr.Location = new System.Drawing.Point(3, -5);
            this.cbuseargstr.Name = "cbuseargstr";
            this.cbuseargstr.Size = new System.Drawing.Size(63, 40);
            this.cbuseargstr.TabIndex = 44;
            this.cbuseargstr.Text = "String";
            this.cbuseargstr.UseVisualStyleBackColor = true;
            this.cbuseargstr.CheckedChanged += new System.EventHandler(this.cbuseargstr_CheckedChanged);
            // 
            // arg0named
            // 
            this.arg0named.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.arg0named.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.arg0named.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.arg0named.BackColor = System.Drawing.Color.Honeydew;
            this.arg0named.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.arg0named.FormattingEnabled = true;
            this.arg0named.Location = new System.Drawing.Point(237, 104);
            this.arg0named.Name = "arg0named";
            this.arg0named.Size = new System.Drawing.Size(110, 21);
            this.arg0named.TabIndex = 41;
            this.arg0named.TextChanged += new System.EventHandler(this.arg0str_TextChanged);
            // 
            // arg0int
            // 
            this.arg0int.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.arg0int.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.arg0int.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.arg0int.BackColor = System.Drawing.Color.LemonChiffon;
            this.arg0int.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.arg0int.FormattingEnabled = true;
            this.arg0int.Location = new System.Drawing.Point(237, 80);
            this.arg0int.Name = "arg0int";
            this.arg0int.Size = new System.Drawing.Size(110, 21);
            this.arg0int.TabIndex = 40;
            this.arg0int.TextChanged += new System.EventHandler(this.arg0int_TextChanged);
            // 
            // arg0label
            // 
            this.arg0label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.arg0label.Location = new System.Drawing.Point(55, 8);
            this.arg0label.Name = "arg0label";
            this.arg0label.Size = new System.Drawing.Size(179, 14);
            this.arg0label.TabIndex = 33;
            this.arg0label.Text = "Argument 1:";
            this.arg0label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.arg0label.UseMnemonic = false;
            // 
            // arg1label
            // 
            this.arg1label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.arg1label.Location = new System.Drawing.Point(55, 33);
            this.arg1label.Name = "arg1label";
            this.arg1label.Size = new System.Drawing.Size(179, 14);
            this.arg1label.TabIndex = 42;
            this.arg1label.Text = "Argument 2:";
            this.arg1label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.arg1label.UseMnemonic = false;
            // 
            // arg2label
            // 
            this.arg2label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.arg2label.Location = new System.Drawing.Point(55, 58);
            this.arg2label.Name = "arg2label";
            this.arg2label.Size = new System.Drawing.Size(179, 14);
            this.arg2label.TabIndex = 43;
            this.arg2label.Text = "Argument 3:";
            this.arg2label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.arg2label.UseMnemonic = false;
            // 
            // arg0
            // 
            this.arg0.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.arg0.Location = new System.Drawing.Point(237, 3);
            this.arg0.Name = "arg0";
            this.arg0.Size = new System.Drawing.Size(110, 24);
            this.arg0.TabIndex = 2;
            // 
            // arg1
            // 
            this.arg1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.arg1.Location = new System.Drawing.Point(237, 28);
            this.arg1.Name = "arg1";
            this.arg1.Size = new System.Drawing.Size(110, 24);
            this.arg1.TabIndex = 34;
            // 
            // arg2
            // 
            this.arg2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.arg2.Location = new System.Drawing.Point(237, 53);
            this.arg2.Name = "arg2";
            this.arg2.Size = new System.Drawing.Size(110, 24);
            this.arg2.TabIndex = 35;
            // 
            // arg3label
            // 
            this.arg3label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.arg3label.Location = new System.Drawing.Point(55, 8);
            this.arg3label.Name = "arg3label";
            this.arg3label.Size = new System.Drawing.Size(179, 14);
            this.arg3label.TabIndex = 44;
            this.arg3label.Text = "Argument 4:";
            this.arg3label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.arg3label.UseMnemonic = false;
            // 
            // arg4label
            // 
            this.arg4label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.arg4label.Location = new System.Drawing.Point(55, 33);
            this.arg4label.Name = "arg4label";
            this.arg4label.Size = new System.Drawing.Size(179, 14);
            this.arg4label.TabIndex = 46;
            this.arg4label.Text = "Argument 5:";
            this.arg4label.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.arg4label.UseMnemonic = false;
            // 
            // arg3
            // 
            this.arg3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.arg3.Location = new System.Drawing.Point(236, 3);
            this.arg3.Name = "arg3";
            this.arg3.Size = new System.Drawing.Size(110, 24);
            this.arg3.TabIndex = 43;
            // 
            // arg4
            // 
            this.arg4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.arg4.Location = new System.Drawing.Point(236, 28);
            this.arg4.Name = "arg4";
            this.arg4.Size = new System.Drawing.Size(110, 24);
            this.arg4.TabIndex = 45;
            // 
            // ArgumentsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.splitcontainer);
            this.Name = "ArgumentsControl";
            this.Size = new System.Drawing.Size(700, 140);
            this.splitcontainer.Panel1.ResumeLayout(false);
            this.splitcontainer.Panel1.PerformLayout();
            this.splitcontainer.Panel2.ResumeLayout(false);
            this.splitcontainer.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitcontainer;
		private ArgumentBox arg0;
		private ArgumentBox arg1;
		private System.Windows.Forms.Label arg0label;
		private ColoredComboBox arg0int;
		private System.Windows.Forms.Label arg2label;
		private System.Windows.Forms.Label arg1label;
		private ColoredComboBox arg0named;
		private System.Windows.Forms.Label arg3label;
		private System.Windows.Forms.Label arg4label;
		private ArgumentBox arg3;
		private ArgumentBox arg4;
		private System.Windows.Forms.CheckBox cbuseargstr;
		private System.Windows.Forms.ToolTip tooltip;
		private ArgumentBox arg2;
        private System.Windows.Forms.TextBox arg0str;
    }
}
