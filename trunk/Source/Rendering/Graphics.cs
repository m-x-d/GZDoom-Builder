
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
using Microsoft.DirectX;
using CodeImp.DoomBuilder.Geometry;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	internal class Graphics : IDisposable
	{
		#region ================== Constants

		// NVPerfHUD device name
		public const string NVPERFHUD_ADAPTER = "NVPerfHUD";

		#endregion

		#region ================== Variables

		// Settings
		private int adapter;
		
		// Main objects
		private Control rendertarget;
		private Caps devicecaps;
		private Device device;
		private Renderer2D renderer2d;
		private Renderer3D renderer3d;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public Device Device { get { return device; } }
		public bool IsDisposed { get { return isdisposed; } }
		public Renderer2D Renderer2D { get { return renderer2d; } }
		public Renderer3D Renderer3D { get { return renderer3d; } }
		public Control RenderTarget { get { return rendertarget; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public Graphics(Control rendertarget)
		{
			// Set render target
			this.rendertarget = rendertarget;
			
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
				renderer2d.Dispose();
				renderer3d.Dispose();
				device.Dispose();
				rendertarget = null;

				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Renderstates

		// This completes initialization after the device has started or has been reset
		private void SetupSettings()
		{
			// Setup renderstates
			device.SetRenderState(RenderStates.AntialiasedLineEnable, false);
			device.SetRenderState(RenderStates.Ambient, Color.White.ToArgb());
			device.SetRenderState(RenderStates.AmbientMaterialSource, (int)ColorSource.Material);
			device.SetRenderState(RenderStates.ColorVertex, false);
			device.SetRenderState(RenderStates.DiffuseMaterialSource, (int)ColorSource.Color1);
			device.SetRenderState(RenderStates.FillMode, (int)FillMode.Solid);
			device.SetRenderState(RenderStates.FogEnable, false);
			device.SetRenderState(RenderStates.Lighting, false);
			device.SetRenderState(RenderStates.LocalViewer, false);
			device.SetRenderState(RenderStates.NormalizeNormals, false);
			device.SetRenderState(RenderStates.SpecularEnable, false);
			device.SetRenderState(RenderStates.StencilEnable, false);
			device.SetRenderState(RenderStates.PointSpriteEnable, false);
			device.SetRenderState(RenderStates.DitherEnable, true);
			device.SetRenderState(RenderStates.AlphaBlendEnable, false);
			device.SetRenderState(RenderStates.ZEnable, false);
			device.SetRenderState(RenderStates.ZBufferWriteEnable, false);
			device.SetRenderState(RenderStates.Clipping, true);
			device.SetRenderState(RenderStates.CullMode, (int)Cull.None);
			device.VertexFormat = PTVertex.Format;

			// Sampler settings
			device.SamplerState[0].MagFilter = TextureFilter.Linear;
			device.SamplerState[0].MinFilter = TextureFilter.Linear;
			device.SamplerState[0].MipFilter = TextureFilter.Linear;

			// Texture addressing
			device.SamplerState[0].AddressU = TextureAddress.Wrap;
			device.SamplerState[0].AddressV = TextureAddress.Wrap;
			device.SamplerState[0].AddressW = TextureAddress.Wrap;

			// First texture stage
			device.TextureState[0].ColorOperation = TextureOperation.Modulate;
			device.TextureState[0].ColorArgument1 = TextureArgument.Current;
			device.TextureState[0].ColorArgument2 = TextureArgument.TFactor;
			device.TextureState[0].ResultArgument = TextureArgument.Current;
			device.TextureState[0].TextureCoordinateIndex = 0;

			// No more further stages
			device.TextureState[1].ColorOperation = TextureOperation.Disable;
			
			// First alpha stage
			device.TextureState[0].AlphaOperation = TextureOperation.Modulate;
			device.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;
			device.TextureState[0].AlphaArgument2 = TextureArgument.TFactor;

			// No more further stages
			device.TextureState[1].AlphaOperation = TextureOperation.Disable;
			
			// Setup material
			Material material = new Material();
			material.Ambient = Color.White;
			material.Diffuse = Color.White;
			material.Specular = Color.White;
			device.Material = material;
		}

		#endregion

		#region ================== Initialization
		
		// This initializes the graphics
		public bool Initialize()
		{
			PresentParameters displaypp;
			DeviceType devtype;

			// Use default adapter
			this.adapter = Manager.Adapters.Default.Adapter;

			// Make present parameters
			displaypp = CreatePresentParameters(adapter);

			// Determine device type for compatability with NVPerfHUD
			if(Manager.Adapters[adapter].Information.Description.EndsWith(NVPERFHUD_ADAPTER))
				devtype = DeviceType.Reference;
			else
				devtype = DeviceType.Hardware;

			// Get the device capabilities
			devicecaps = Manager.GetDeviceCaps(adapter, devtype);

			try
			{
				// Check if this adapter supports TnL
				if(devicecaps.DeviceCaps.SupportsHardwareTransformAndLight)
				{
					// Initialize with hardware TnL
					device = new Device(adapter, devtype, rendertarget,
								CreateFlags.HardwareVertexProcessing, displaypp);
				}
				else
				{
					// Initialize with software TnL
					device = new Device(adapter, devtype, rendertarget,
								CreateFlags.SoftwareVertexProcessing, displaypp);
				}
			}
			catch(Exception)
			{
				// Failed
				MessageBox.Show(General.MainWindow, "Unable to initialize the Direct3D video device. Another application may have taken exclusive mode on this video device.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}

			// Add event to cancel resize event
			device.DeviceResizing += new CancelEventHandler(CancelResize);

			// Initialize settings
			SetupSettings();
			
			// Create renderers
			renderer2d = new Renderer2D(this);
			renderer3d = new Renderer3D(this);
			
			// Done
			return true;
		}

		// This is to disable the automatic resize reset
		private static void CancelResize(object sender, CancelEventArgs e)
		{
			// Cancel resize event
			e.Cancel = true;
		}
		
		// This creates present parameters
		private PresentParameters CreatePresentParameters(int adapter)
		{
			PresentParameters displaypp = new PresentParameters();
			DisplayMode currentmode;
			
			// Get current display mode
			currentmode = Manager.Adapters[adapter].CurrentDisplayMode;

			// Make present parameters
			displaypp.Windowed = true;
			displaypp.SwapEffect = SwapEffect.Discard;
			displaypp.BackBufferCount = 1;
			displaypp.BackBufferFormat = currentmode.Format;
			displaypp.BackBufferWidth = rendertarget.ClientSize.Width;
			displaypp.BackBufferHeight = rendertarget.ClientSize.Height;
			displaypp.EnableAutoDepthStencil = true;
			displaypp.AutoDepthStencilFormat = DepthFormat.D16;
			displaypp.MultiSample = MultiSampleType.None;
			displaypp.PresentationInterval = PresentInterval.Immediate;

			// Return result
			return displaypp;
		}
		
		#endregion

		#region ================== Resetting

		// This resets the device and returns true on success
		public bool Reset()
		{
			PresentParameters displaypp;

			// TODO: Unload all Direct3D resources

			// Make present parameters
			displaypp = CreatePresentParameters(adapter);
			
			try
			{
				// Reset the device
				device.Reset(displaypp);
			}
			catch(Exception)
			{
				// Failed to re-initialize
				return false;
			}

			// Initialize settings
			SetupSettings();

			// TODO: Reload all Direct3D resources

			// Success
			return true;
		}

		#endregion
		
		#region ================== Rendering

		// This begins a drawing session
		public bool StartRendering()
		{
			int coopresult;

			// When minimized, do not render anything
			if(General.MainWindow.WindowState != FormWindowState.Minimized)
			{
				// Test the cooperative level
				device.CheckCooperativeLevel(out coopresult);

				// Check if device must be reset
				if(coopresult == (int)ResultCode.DeviceNotReset)
				{
					// Device is lost and must be reset now
					return Reset();
				}
				// Check if device is lost
				else if(coopresult == (int)ResultCode.DeviceLost)
				{
					// Device is lost and cannot be reset now
					return false;
				}

				// Clear the screen
				device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, 0, 1f, 0);

				// Ready to render
				device.BeginScene();
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
				device.EndScene();

				// Display the scene
				device.Present();
			}
			// Errors are not a problem here
			catch(Exception) { }
		}

		#endregion

		#region ================== Tools

		// Make a color from ARGB
		public static int ARGB(float a, float r, float g, float b)
		{
			return Color.FromArgb((int)(a * 255f), (int)(r * 255f), (int)(g * 255f), (int)(b * 255f)).ToArgb();
		}

		// Make a color from RGB
		public static int RGB(int r, int g, int b)
		{
			return Color.FromArgb(255, r, g, b).ToArgb();
		}

		// This makes a Vector3 from Vector3D
		public static Vector3 V3(Vector3D v3d)
		{
			return new Vector3(v3d.x, v3d.y, v3d.z);
		}

		// This makes a Vector3D from Vector3
		public static Vector3D V3D(Vector3 v3)
		{
			return new Vector3D(v3.X, v3.Y, v3.Z);
		}

		#endregion
	}
}
