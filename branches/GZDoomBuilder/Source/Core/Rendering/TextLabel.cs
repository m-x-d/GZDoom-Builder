
#region ================== Copyright (c) 2007 Pascal vd Heiden

/*
 * Copyright (c) 2007 Pascal vd Heiden, www.codeimp.com
 * This program is released under GNU General Public License
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 */

#endregion

#region ================== Namespaces

using System;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Drawing;
using SlimDX.Direct3D9;
using SlimDX;
using CodeImp.DoomBuilder.Geometry;
using Font = System.Drawing.Font;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	public class TextLabel : IDisposable, ID3DResource
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// The text is stored as a polygon in a vertex buffer
		private VertexBuffer textbuffer;
		private Texture texture;
		private Font font; //mxd
		
		// Text settings
		private string text;
		private RectangleF rect;
		private RectangleF absview; //mxd
		private bool transformcoords;
		private PixelColor color;
		private PixelColor backcolor;
		private TextAlignmentX alignx;
		private TextAlignmentY aligny;
		private SizeF textsize;
		private bool drawbg; //mxd
		
		// This keeps track if changes were made
		private bool updateneeded;
		private bool textureupdateneeded; //mxd
		private float lasttranslatex = float.MinValue;
		private float lasttranslatey;
		private float lastscalex;
		private float lastscaley;
		
		// Disposing
		private bool isdisposed;

		#endregion

		#region ================== Properties

		// Properties
		public RectangleF Rectangle { get { return rect; } set { rect = value; updateneeded = true; } }
		public float Left { get { return rect.X; } set { rect.X = value; updateneeded = true; } }
		public float Top { get { return rect.Y; } set { rect.Y = value; updateneeded = true; } }
		public float Width { get { return rect.Width; } set { rect.Width = value; updateneeded = true; } }
		public float Height { get { return rect.Height; } set { rect.Height = value; updateneeded = true; } }
		public float Right { get { return rect.Right; } set { rect.Width = value - rect.X + 1f; updateneeded = true; } }
		public float Bottom { get { return rect.Bottom; } set { rect.Height = value - rect.Y + 1f; updateneeded = true; } }
		public string Text { get { return text; } set { if(text != value) { text = value; textureupdateneeded = true; } } }
		public Font Font { get { return font; } set { font = value; textureupdateneeded = true; } } //mxd
		public bool TransformCoords { get { return transformcoords; } set { transformcoords = value; updateneeded = true; } }
		public SizeF TextSize { get { return textsize; } }
		public TextAlignmentX AlignX { get { return alignx; } set { alignx = value; updateneeded = true; } }
		public TextAlignmentY AlignY { get { return aligny; } set { aligny = value; updateneeded = true; } }
		public PixelColor Color { get { return color; } set { if(!color.Equals(value)) { color = value; textureupdateneeded = true; } } }
		public PixelColor Backcolor { get { return backcolor; } set { if(!backcolor.Equals(value)) { backcolor = value; textureupdateneeded = true; } } }
		public bool DrawBackground { get { return drawbg; } set { if(drawbg != value) { drawbg = value; textureupdateneeded = true; } } } //mxd
		internal Texture Texture { get { return texture; } } //mxd
		internal VertexBuffer VertexBuffer { get { return textbuffer; } }
		internal bool SkipRendering; //mxd
		
		// Disposing
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public TextLabel()
		{
			// Initialize
			this.text = "";
			this.font = new Font(General.MainWindow.Font.FontFamily, (float)Math.Round(General.MainWindow.Font.SizeInPoints * 1.25f), FontStyle.Regular); //mxd
			this.rect = new RectangleF(0f, 0f, 1f, 1f);
			this.color = new PixelColor(255, 255, 255, 255);
			this.backcolor = new PixelColor(255, 0, 0, 0);
			this.alignx = TextAlignmentX.Center;
			this.aligny = TextAlignmentY.Top;
			this.textsize = new SizeF();
			this.updateneeded = true;
			this.textureupdateneeded = true; //mxd
			
			// Register as resource
			General.Map.Graphics.RegisterResource(this);
			
			// We have no destructor
			GC.SuppressFinalize(this);
		}

		// Diposer
		public void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				UnloadResource();
				
				// Unregister resource
				General.Map.Graphics.UnregisterResource(this);
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Methods
		
		// This updates the text if needed
		internal RectangleF Update(float translatex, float translatey, float scalex, float scaley)
		{
			// Check if transformation changed and needs to be updated
			if(transformcoords && (translatex != lasttranslatex || translatey != lasttranslatey ||
			   scalex != lastscalex || scaley != lastscaley))
			{
				lasttranslatex = translatex; //mxd
				lasttranslatey = translatey; //mxd
				lastscalex = scalex; //mxd
				lastscaley = scaley; //mxd
				updateneeded = true;
			}

			//mxd. Update texture if needed
			if(textureupdateneeded)
			{
				// Get rid of old texture
				if(texture != null)
				{
					texture.Dispose(); 
					texture = null;
				}

				// Create label image
				Bitmap img = CreateLabelImage(text, font, color, backcolor, drawbg);
				textsize = img.Size;

				// Create texture
				MemoryStream memstream = new MemoryStream((img.Size.Width * img.Size.Height * 4) + 4096);
				img.Save(memstream, ImageFormat.Bmp);
				memstream.Seek(0, SeekOrigin.Begin);

				texture = Texture.FromStream(General.Map.Graphics.Device, memstream, (int)memstream.Length,
						img.Size.Width, img.Size.Height, 1, Usage.None, Format.Unknown,
						Pool.Managed, General.Map.Graphics.PostFilter, General.Map.Graphics.MipGenerateFilter, 0);
			}
			
			// Update if needed
			if(updateneeded || textureupdateneeded)
			{
				// Only build when there are any vertices
				if(text.Length > 0)
				{
					// Transform?
					if(transformcoords)
					{
						// Calculate absolute coordinates
						Vector2D lt = new Vector2D(rect.Left, rect.Top);
						Vector2D rb = new Vector2D(rect.Right, rect.Bottom);
						lt = lt.GetTransformed(translatex, translatey, scalex, scaley);
						rb = rb.GetTransformed(translatex, translatey, scalex, scaley);
						absview = new RectangleF((float)Math.Round(lt.x), (float)Math.Round(lt.y), rb.x - lt.x, rb.y - lt.y);
					}
					else
					{
						// Fixed coordinates
						absview = rect;
					}

					// Align the text horizontally
					float beginx = 0;
					switch(alignx)
					{
						case TextAlignmentX.Left: beginx = absview.X; break;
						case TextAlignmentX.Center: beginx = absview.X + (absview.Width - textsize.Width) * 0.5f; break;
						case TextAlignmentX.Right: beginx = absview.X + absview.Width - textsize.Width; break;
					}

					// Align the text vertically
					float beginy = 0;
					switch(aligny)
					{
						case TextAlignmentY.Top: beginy = absview.Y; break;
						case TextAlignmentY.Middle: beginy = absview.Y + (absview.Height - textsize.Height) * 0.5f; break;
						case TextAlignmentY.Bottom: beginy = absview.Y + absview.Height - textsize.Height; break;
					}

					// Do we have to make a new buffer?
					if(textbuffer == null)
					{
						// Create the buffer
						textbuffer = new VertexBuffer(General.Map.Graphics.Device, 4 * FlatVertex.Stride,
													  Usage.Dynamic | Usage.WriteOnly, VertexFormat.None, Pool.Default);
					}

					//mxd. Lock the buffer
					using(DataStream stream = textbuffer.Lock(0, 4 * FlatVertex.Stride, LockFlags.Discard | LockFlags.NoSystemLock))
					{
						FlatQuad quad = new FlatQuad(PrimitiveType.TriangleStrip, beginx, beginy, beginx + textsize.Width, beginy + textsize.Height);
						stream.WriteRange(quad.Vertices);
					}

					// Done filling the vertex buffer
					textbuffer.Unlock();
				}
				else
				{
					// No faces in polygon
					if(textbuffer != null) textbuffer.Dispose(); //mxd
					textsize = new SizeF();
				}

				// Text updated
				updateneeded = false;
				textureupdateneeded = false; //mxd
			}

			return absview; //mxd
		}

		//mxd
		private static Bitmap CreateLabelImage(string text, Font font, PixelColor color, PixelColor backcolor, bool drawbg)
		{
			PointF textorigin = new PointF(4, 3);
			RectangleF textrect = new RectangleF(textorigin, General.Interface.MeasureString(text, font));
			textrect.Width = (float)Math.Round(textrect.Width);
			textrect.Height = (float)Math.Round(textrect.Height);
			RectangleF bgrect = new RectangleF(0, 0, textrect.Width + textorigin.X * 2, textrect.Height + textorigin.Y * 2);

			Bitmap result = new Bitmap((int)bgrect.Width, (int)bgrect.Height);
			using(Graphics g = Graphics.FromImage(result))
			{
				g.SmoothingMode = SmoothingMode.HighQuality;
				g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
				g.CompositingQuality = CompositingQuality.HighQuality;

				// Draw text
				using(StringFormat sf = new StringFormat())
				{
					sf.FormatFlags = StringFormatFlags.NoWrap;
					sf.Alignment = StringAlignment.Center;
					sf.LineAlignment = StringAlignment.Center;

					// Draw text with BG
					if(drawbg)
					{
						GraphicsPath p = new GraphicsPath();
						float radius = textorigin.X;
						const float outlinewidth = 1;

						RectangleF pathrect = bgrect;
						pathrect.Width -= 1;
						pathrect.Height -= 1;

						// Left line
						p.AddLine(pathrect.Left, pathrect.Bottom - radius + outlinewidth, pathrect.Left, pathrect.Top + radius);
						p.AddArc(pathrect.Left, pathrect.Top, radius, radius, 180, 90);

						// Top line
						p.AddLine(pathrect.Left + radius, pathrect.Top, pathrect.Right - radius, pathrect.Top);
						p.AddArc(pathrect.Right - radius, pathrect.Top, radius, radius, 270, 90);

						// Right line
						p.AddLine(pathrect.Right, pathrect.Top + radius, pathrect.Right, pathrect.Bottom - radius);
						p.AddArc(pathrect.Right - radius, pathrect.Bottom - radius, radius, radius, 0, 90);

						// Bottom line
						p.AddLine(pathrect.Left + radius, pathrect.Bottom, pathrect.Left + radius, pathrect.Bottom);
						p.AddArc(pathrect.Left, pathrect.Bottom - radius, radius, radius, 90, 90);

						// Fill'n'draw bg
						using(SolidBrush brush = new SolidBrush(color.ToColor()))
							g.FillPath(brush, p);

						using(Pen pen = new Pen(backcolor.ToColor(), outlinewidth))
							g.DrawPath(pen, p);

						// Draw text
						using(SolidBrush brush = new SolidBrush(backcolor.ToColor()))
							g.DrawString(text, font, brush, textrect, sf);
					}
					// Draw text with outline 
					else
					{
						RectangleF pathrect = textrect;
						pathrect.Inflate(1, 3);

						GraphicsPath p = new GraphicsPath();
						p.AddString(text, font.FontFamily, (int)font.Style, g.DpiY * font.Size / 72f, pathrect, sf);

						// Draw'n'fill text
						using(Pen pen = new Pen(backcolor.ToColor(), 3))
							g.DrawPath(pen, p);

						using(SolidBrush brush = new SolidBrush(color.ToColor()))
							g.FillPath(brush, p);
					}
				}
			}

			return result;
		}

		// This unloads the resources
		public void UnloadResource()
		{
			// Clean up
			if(textbuffer != null)
			{
				textbuffer.Dispose();
				textbuffer = null;
			}

			if(texture != null) //mxd
			{
				texture.Dispose(); 
				texture = null;
			}

			// Need to update before we can render
			updateneeded = true;
			textureupdateneeded = true; //mxd
		}

		// This (re)loads the resources
		public void ReloadResource() { }
		
		#endregion
	}
}
