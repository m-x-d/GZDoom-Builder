using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace CodeImp.DoomBuilder.BuilderModes.Interface
{
	public partial class WavefrontSettingsForm : Form
	{
		public string FilePath { get { return filePath; } }
		private string filePath;
		
		public WavefrontSettingsForm(int sectorsCount) {
			InitializeComponent();

			saveFileDialog.InitialDirectory = General.Map.FilePathName;
			saveFileDialog.FileName = Path.GetDirectoryName(General.Map.FilePathName) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(General.Map.FileTitle) + "_" + General.Map.Options.LevelName + ".obj";
			tbExportPath.Text = saveFileDialog.FileName;
			cbExportTextures.Checked = BuilderPlug.Me.ObjExportTextures;
			cbFixScale.Checked = BuilderPlug.Me.ObjGZDoomScale;
			nudScale.Value = (decimal)BuilderPlug.Me.ObjScale;

			this.Text = "Export " + (sectorsCount == -1 ? "whole map" : sectorsCount + (sectorsCount > 1 ? "sectors" : "sector")) + " to Wavefront .obj";
		}

		private void browse_Click(object sender, EventArgs e) {
			if(saveFileDialog.ShowDialog() == DialogResult.OK) {
				tbExportPath.Text = saveFileDialog.FileName;
			}
		}

		private void export_Click(object sender, EventArgs e) {
			filePath = tbExportPath.Text.Trim();

			BuilderPlug.Me.ObjExportTextures = cbExportTextures.Checked;
			BuilderPlug.Me.ObjGZDoomScale = cbFixScale.Checked;
			BuilderPlug.Me.ObjScale = (float)nudScale.Value;

			//save settings
			General.Settings.WritePluginSetting("objexporttextures", cbExportTextures.Checked);//mxd
			General.Settings.WritePluginSetting("objgzdoomscale", cbFixScale.Checked);//mxd
			General.Settings.WritePluginSetting("objscale", (float)nudScale.Value);//mxd

			//verify path
			if(Directory.Exists(Path.GetDirectoryName(filePath))) {
				filePath = tbExportPath.Text;
				this.DialogResult = DialogResult.OK;
				this.Close();
			} else {
				MessageBox.Show("Selected path does not exist!");
			}
		}

		private void cancel_Click(object sender, EventArgs e) {
			this.Close();
		}
	}
}
