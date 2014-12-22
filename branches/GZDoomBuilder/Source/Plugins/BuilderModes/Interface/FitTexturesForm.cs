#region ================== Namespaces

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal struct FitTextureOptions
	{
		public int HorizontalRepeat;
		public int VerticalRepeat;
		public bool FitWidth;
		public bool FitHeight;
		public bool FitAcrossSurfaces;
		public Rectangle GlobalBounds;
		public Rectangle Bounds;

		//Initial texture coordinats
		public float InitialOffsetX;
		public float InitialOffsetY;
		public float InitialScaleX;
		public float InitialScaleY;
	}
	
	internal partial class FitTexturesForm : DelayedForm
	{
		#region ================== Event handlers

		#endregion

		#region ================== Variables

		private static Point location = Point.Empty;
		private bool blockupdate;

		// Settings
		private static int horizontalrepeat = 1;
		private static int verticalrepeat = 1;
		private static bool fitacrosssurfaces = true;
		private static bool fitwidth = true;
		private static bool fitheight = true;

		//Surface stuff
		private List<SortedVisualSide> strips;
		private Rectangle bounds;

		#endregion

		#region ================== Constructor

		public FitTexturesForm() 
		{
			InitializeComponent();

			if (!location.IsEmpty)
			{
				this.StartPosition = FormStartPosition.Manual;
				this.Location = location;
			}
		}

		#endregion

		#region ================== Methods

		public void Setup(IEnumerable<BaseVisualGeometrySidedef> sides)
		{
			// Get shapes
			strips = BuilderModesTools.SortVisualSides(sides);

			// No dice...
			if(strips.Count == 0)
			{
				General.Interface.DisplayStatus(StatusType.Warning, "Failed to setup sidedef chains...");
				this.DialogResult = DialogResult.Cancel;
				this.Close();
				return;
			}

			// Create bounds
			int minx = int.MaxValue;
			int maxx = int.MinValue;
			int miny = int.MaxValue;
			int maxy = int.MinValue;

			foreach(SortedVisualSide side in strips) 
			{
				if(side.Bounds.X < minx) minx = side.Bounds.X;
				if(side.Bounds.X + side.Bounds.Width > maxx) maxx = side.Bounds.X + side.Bounds.Width;
				if(side.Bounds.Y < miny) miny = side.Bounds.Y;
				if(side.Bounds.Y + side.Bounds.Height > maxy) maxy = side.Bounds.Y + side.Bounds.Height;
			}

			bounds = new Rectangle(minx, miny, maxx-minx, maxy-miny);

			// Normalize Y-offset
			foreach (SortedVisualSide side in strips) side.Bounds.Y -= bounds.Y;
			bounds.Y = 0;

#if DEBUG
			//debug
			DrawDebugUV();
#endif

			// Restore settings
			blockupdate = true;

			horizrepeat.Value = horizontalrepeat;
			vertrepeat.Value = verticalrepeat;
			cbfitconnected.Checked = fitacrosssurfaces;
			cbfitwidth.Checked = fitwidth;
			cbfitheight.Checked = fitheight;
			UpdateRepeatGroup();

			blockupdate = false;

			//trigger update
			UpdateChanges();
		}

		private void UpdateChanges()
		{
			// Apply changes
			FitTextureOptions options = new FitTextureOptions
			                            {
				                            GlobalBounds = bounds, 
											FitAcrossSurfaces = cbfitconnected.Checked,
											FitWidth = cbfitwidth.Checked,
											FitHeight = cbfitheight.Checked,
											HorizontalRepeat = (int)horizrepeat.Value,
											VerticalRepeat = (int)vertrepeat.Value
			                            };

			foreach(SortedVisualSide side in strips) side.OnTextureFit(options);
		}

		private void UpdateRepeatGroup()
		{
			// Disable whole group?
			repeatgroup.Enabled = cbfitwidth.Checked || cbfitheight.Checked;
			if(!repeatgroup.Enabled) return;

			// Update control status
			labelhorizrepeat.Enabled = cbfitwidth.Checked;
			horizrepeat.Enabled = cbfitwidth.Checked;
			resethoriz.Enabled = cbfitwidth.Checked;

			labelvertrepeat.Enabled = cbfitheight.Checked;
			vertrepeat.Enabled = cbfitheight.Checked;
			resetvert.Enabled = cbfitheight.Checked;
		}

		#endregion

		#region ================== Events

		private void FitTexturesForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			location = this.Location;

			// Store settings
			if (this.DialogResult == DialogResult.OK)
			{
				horizontalrepeat = (int)horizrepeat.Value;
				verticalrepeat = (int)vertrepeat.Value;
				fitacrosssurfaces = cbfitwidth.Checked;
				fitwidth = cbfitwidth.Checked;
				fitheight = cbfitheight.Checked;
			}
		}

		private void resethoriz_Click(object sender, EventArgs e)
		{
			horizrepeat.Value = 1;
		}

		private void resetvert_Click(object sender, EventArgs e)
		{
			vertrepeat.Value = 1;
		}

		private void repeat_ValueChanged(object sender, EventArgs e) 
		{
			if(blockupdate) return;
			UpdateChanges();
		}

		private void accept_Click(object sender, EventArgs e) 
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void cancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

		private void cbfitwidth_CheckedChanged(object sender, EventArgs e) 
		{
			if(blockupdate) return;
			UpdateRepeatGroup();
			UpdateChanges();
		}

		private void cbfitheight_CheckedChanged(object sender, EventArgs e) 
		{
			if(blockupdate) return;
			UpdateRepeatGroup();
			UpdateChanges();
		}

		#endregion

		#region ================== Debug

#if DEBUG
		private void DrawDebugUV()
		{
			const int margin = 20;
			
			//find bounds
			int minx = int.MaxValue;
			int maxx = int.MinValue;
			int miny = int.MaxValue;
			int maxy = int.MinValue;

			foreach(SortedVisualSide side in strips) 
			{
				if(side.Bounds.X < minx) minx = side.Bounds.X;
				if(side.Bounds.X + side.Bounds.Width > maxx) maxx = side.Bounds.X + side.Bounds.Width;
				if(side.Bounds.Y < miny) miny = side.Bounds.Y;
				if(side.Bounds.Y + side.Bounds.Height > maxy) maxy = side.Bounds.Y + side.Bounds.Height;
			}

			Bitmap bmp = new Bitmap(maxx - minx + margin * 2, maxy - miny + margin * 2);

			using(Graphics g = Graphics.FromImage(bmp))
			{
				int i = 0;
				
				foreach(SortedVisualSide side in strips)
				{
					Color c = General.Colors.BrightColors[General.Random(0, General.Colors.BrightColors.Length - 1)].ToColor();
					Pen p = new Pen(c);
					Brush b = new SolidBrush(c);
					
					int x = side.Bounds.X - minx + margin;
					int y = side.Bounds.Y - miny + margin;
					
					g.DrawRectangle(p, x, y, side.Bounds.Width, side.Bounds.Height);
					g.DrawString(i++ + ": line " + side.Side.Sidedef.Line.Index + "; x:" + side.Bounds.X + " y:" + side.Bounds.Y, this.Font, b, x + 2, y + 2);
				}
			}

			bmp.Save("testuv.png", ImageFormat.Png);

			General.Interface.DisplayStatus(StatusType.Info, "Saved test image!");
		}
#endif

		#endregion
	}
}
