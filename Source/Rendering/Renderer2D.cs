
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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Drawing;
using Microsoft.DirectX.Direct3D;
using System.ComponentModel;
using CodeImp.DoomBuilder.Map;
using Microsoft.DirectX;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	internal class Renderer2D : IDisposable
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Owner
		private Graphics graphics;
		
		// Main objects
		private Line line;

		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		// Disposing
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public Renderer2D(Graphics graphics)
		{
			// Initialize
			this.graphics = graphics;

			// Create line object
			line = new Line(graphics.Device);
			line.Width = 2f;
			line.Antialias = true;
			
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
				line.Dispose();
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Methods

		// This begins a drawing session
		public bool StartRendering()
		{
			int coopresult;

			// When minimized, do not render anything
			if(General.MainWindow.WindowState != FormWindowState.Minimized)
			{
				// Test the cooperative level
				graphics.Device.CheckCooperativeLevel(out coopresult);

				// Check if device must be reset
				if(coopresult == (int)ResultCode.DeviceNotReset)
				{
					// TODO: Device is lost and must be reset now
					//return Reset();
					return false;
				}
				// Check if device is lost
				else if(coopresult == (int)ResultCode.DeviceLost)
				{
					// Device is lost and cannot be reset now
					return false;
				}

				// Clear the screen
				graphics.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, 0, 1f, 0);
				
				// Ready to render
				graphics.Device.BeginScene();
				return true;
			}
			else
			{
				// Minimized, you cannot see anything
				return false;
			}
		}
		
		// This ends a drawing session
		public void FinishRendering()
		{
			try
			{
				// Done
				graphics.Device.EndScene();
				
				// Display the scene
				graphics.Device.Present();
			}
			// Errors are not a problem here
			catch(Exception) { }
		}
		
		// This renders a set of Linedefs
		public void RenderLinedefs(IEnumerable<Linedef> linedefs)
		{
			Vector2[] vertices = new Vector2[2];

			line.Begin();

			// Go for all linedefs
			foreach(Linedef l in linedefs)
			{
				// Make vertices
				vertices[0].X = l.Start.Position.x;
				vertices[0].Y = l.Start.Position.y;
				vertices[1].X = l.End.Position.x;
				vertices[1].Y = l.End.Position.y;

				// Draw line
				line.Draw(vertices, -1);
			}

			line.End();
		}

		#endregion
	}
}
