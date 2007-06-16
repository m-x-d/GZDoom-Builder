
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
		private Device d3dd;
		private Renderer2D renderer2d;
		private Renderer3D renderer3d;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public Device Device { get { return d3dd; } }
		public bool IsDisposed { get { return isdisposed; } }
		public Renderer2D Renderer2D { get { return renderer2d; } }
		public Renderer3D Renderer3D { get { return renderer3d; } }
		
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
				d3dd.Dispose();
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
			d3dd.SetRenderState(RenderStates.AntialiasedLineEnable, true);
			d3dd.SetRenderState(RenderStates.Ambient, Color.White.ToArgb());
			d3dd.SetRenderState(RenderStates.AmbientMaterialSource, (int)ColorSource.Material);
			d3dd.SetRenderState(RenderStates.ColorVertex, false);
			d3dd.SetRenderState(RenderStates.DiffuseMaterialSource, (int)ColorSource.Color1);
			d3dd.SetRenderState(RenderStates.FillMode, (int)FillMode.Solid);
			d3dd.SetRenderState(RenderStates.FogEnable, false);
			d3dd.SetRenderState(RenderStates.Lighting, false);
			d3dd.SetRenderState(RenderStates.LocalViewer, false);
			d3dd.SetRenderState(RenderStates.NormalizeNormals, false);
			d3dd.SetRenderState(RenderStates.SpecularEnable, false);
			d3dd.SetRenderState(RenderStates.StencilEnable, false);
			d3dd.SetRenderState(RenderStates.PointSpriteEnable, false);
			d3dd.SetRenderState(RenderStates.DitherEnable, true);
			d3dd.SetRenderState(RenderStates.AlphaBlendEnable, false);
			d3dd.SetRenderState(RenderStates.ZEnable, false);
			d3dd.SetRenderState(RenderStates.ZBufferWriteEnable, false);
			d3dd.SetRenderState(RenderStates.Clipping, true);
			d3dd.SetRenderState(RenderStates.CullMode, (int)Cull.CounterClockwise);
			d3dd.VertexFormat = PTVertex.Format;

			// Sampler settings
			d3dd.SamplerState[0].MagFilter = TextureFilter.Linear;
			d3dd.SamplerState[0].MinFilter = TextureFilter.Linear;
			d3dd.SamplerState[0].MipFilter = TextureFilter.Linear;

			// Texture addressing
			d3dd.SamplerState[0].AddressU = TextureAddress.Wrap;
			d3dd.SamplerState[0].AddressV = TextureAddress.Wrap;
			d3dd.SamplerState[0].AddressW = TextureAddress.Wrap;

			// First texture stage
			d3dd.TextureState[0].ColorOperation = TextureOperation.Modulate;
			d3dd.TextureState[0].ColorArgument1 = TextureArgument.TextureColor;
			d3dd.TextureState[0].ColorArgument2 = TextureArgument.TFactor;
			d3dd.TextureState[0].ResultArgument = TextureArgument.Current;
			d3dd.TextureState[0].TextureCoordinateIndex = 0;

			// No more further stages
			d3dd.TextureState[1].ColorOperation = TextureOperation.Disable;
			
			// First alpha stage
			d3dd.TextureState[0].AlphaOperation = TextureOperation.Modulate;
			d3dd.TextureState[0].AlphaArgument1 = TextureArgument.TextureColor;
			d3dd.TextureState[0].AlphaArgument2 = TextureArgument.TFactor;

			// No more further stages
			d3dd.TextureState[1].AlphaOperation = TextureOperation.Disable;
			
			// Setup material
			Material material = new Material();
			material.Ambient = Color.White;
			material.Diffuse = Color.White;
			material.Specular = Color.White;
			d3dd.Material = material;
		}

		#endregion

		#region ================== Initialization
		
		// This initializes the graphics
		public bool Initialize()
		{
			AdapterInformation adapterinfo;
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
					d3dd = new Device(adapter, devtype, rendertarget,
								CreateFlags.HardwareVertexProcessing, displaypp);
				}
				else
				{
					// Initialize with software TnL
					d3dd = new Device(adapter, devtype, rendertarget,
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
			d3dd.DeviceResizing += new CancelEventHandler(CancelResize);

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
				d3dd.Reset(displaypp);
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
	}
}
