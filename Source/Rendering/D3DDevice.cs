
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
using SlimDX.Direct3D9;
using SlimDX.Direct3D;
using System.ComponentModel;
using CodeImp.DoomBuilder.Geometry;
using SlimDX;
using CodeImp.DoomBuilder.Interface;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	public class D3DDevice : IDisposable
	{
		#region ================== Constants

		// NVPerfHUD device name
		public const string NVPERFHUD_ADAPTER = "NVPerfHUD";

		#endregion

		#region ================== Variables

		// Settings
		private int adapter;
		
		// Main objects
		private RenderTargetControl rendertarget;
		private Capabilities devicecaps;
		private Device device;
		private Viewport viewport;
		private List<ID3DResource> resources;
		private ShaderManager shaders;
		private Surface backbuffer;
		private Surface depthbuffer;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public Device Device { get { return device; } }
		public bool IsDisposed { get { return isdisposed; } }
		public RenderTargetControl RenderTarget { get { return rendertarget; } }
		public Viewport Viewport { get { return viewport; } }
		public ShaderManager Shaders { get { return shaders; } }
		public Surface BackBuffer { get { return backbuffer; } }
		public Surface DepthBuffer { get { return depthbuffer; } }
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public D3DDevice(RenderTargetControl rendertarget)
		{
			// Set render target
			this.rendertarget = rendertarget;

			// Create resources list
			resources = new List<ID3DResource>();
			
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
				foreach(ID3DResource res in resources) res.UnloadResource();
				if(shaders != null) shaders.Dispose();
				rendertarget = null;
				if(backbuffer != null) backbuffer.Dispose();
				if(depthbuffer != null) depthbuffer.Dispose();
				device.Dispose();
				
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
			device.SetRenderState(RenderState.AntialiasedLineEnable, false);
			device.SetRenderState(RenderState.Ambient, Color.White.ToArgb());
			device.SetRenderState(RenderState.AmbientMaterialSource, ColorSource.Material);
			device.SetRenderState(RenderState.ColorVertex, false);
			device.SetRenderState(RenderState.DiffuseMaterialSource, ColorSource.Color1);
			device.SetRenderState(RenderState.FillMode, FillMode.Solid);
			device.SetRenderState(RenderState.FogEnable, false);
			device.SetRenderState(RenderState.Lighting, false);
			device.SetRenderState(RenderState.LocalViewer, false);
			device.SetRenderState(RenderState.NormalizeNormals, false);
			device.SetRenderState(RenderState.SpecularEnable, false);
			device.SetRenderState(RenderState.StencilEnable, false);
			device.SetRenderState(RenderState.PointSpriteEnable, false);
			device.SetRenderState(RenderState.DitherEnable, true);
			device.SetRenderState(RenderState.AlphaBlendEnable, false);
			device.SetRenderState(RenderState.ZEnable, false);
			device.SetRenderState(RenderState.ZWriteEnable, false);
			device.SetRenderState(RenderState.Clipping, true);
			device.SetRenderState(RenderState.CullMode, Cull.None);

			// Sampler settings
			device.SetSamplerState(0, SamplerState.MagFilter, TextureFilter.Linear);
			device.SetSamplerState(0, SamplerState.MinFilter, TextureFilter.Linear);
			device.SetSamplerState(0, SamplerState.MipFilter, TextureFilter.Linear);
			device.SetSamplerState(0, SamplerState.MipMapLodBias, 0f);
			
			// Texture addressing
			device.SetSamplerState(0, SamplerState.AddressU, TextureAddress.Wrap);
			device.SetSamplerState(0, SamplerState.AddressV, TextureAddress.Wrap);
			device.SetSamplerState(0, SamplerState.AddressW, TextureAddress.Wrap);

			// First texture stage
			device.SetTextureStageState(0, TextureStage.ColorOperation, TextureOperation.Modulate);
			device.SetTextureStageState(0, TextureStage.ColorArg1, TextureArgument.Texture);
			device.SetTextureStageState(0, TextureStage.ColorArg2, TextureArgument.Diffuse);
			device.SetTextureStageState(0, TextureStage.ResultArg, TextureArgument.Current);
			device.SetTextureStageState(0, TextureStage.TexCoordIndex, 0);

			// No more further stages
			device.SetTextureStageState(1, TextureStage.ColorOperation, TextureOperation.Disable);
			
			// First alpha stage
			device.SetTextureStageState(0, TextureStage.AlphaOperation, TextureOperation.Modulate);
			device.SetTextureStageState(0, TextureStage.AlphaArg1, TextureArgument.Texture);
			device.SetTextureStageState(0, TextureStage.AlphaArg2, TextureArgument.Diffuse);
			
			// No more further stages
			device.SetTextureStageState(1, TextureStage.AlphaOperation, TextureOperation.Disable);
			
			// Setup material
			Material material = new Material();
			material.Ambient = ColorValue.FromColor(Color.White);
			material.Diffuse = ColorValue.FromColor(Color.White);
			material.Specular = ColorValue.FromColor(Color.White);
			device.Material = material;

			// Keep a reference to the original buffers
			backbuffer = device.GetBackBuffer(0, 0);
			depthbuffer = device.GetDepthStencilSurface();
			
			// Get the viewport
			viewport = device.Viewport;
		}

		#endregion

		#region ================== Initialization
		
		// This initializes the graphics
		public bool Initialize()
		{
			PresentParameters displaypp;
			DeviceType devtype;

			// Use default adapter
			this.adapter = 0; // Manager.Adapters.Default.Adapter;

			// Make present parameters
			displaypp = CreatePresentParameters(adapter);

			// Determine device type for compatability with NVPerfHUD
			if(Direct3D.Adapters[adapter].Details.Description.EndsWith(NVPERFHUD_ADAPTER))
				devtype = DeviceType.Reference;
			else
				devtype = DeviceType.Hardware;

			// Get the device capabilities
			devicecaps = Direct3D.GetDeviceCaps(adapter, devtype);

			try
			{
				// Check if this adapter supports TnL
				if((devicecaps.DeviceCaps & DeviceCaps.HWTransformAndLight) != 0)
				{
					// Initialize with hardware TnL
					device = new Device(adapter, devtype, rendertarget.Handle,
								CreateFlags.HardwareVertexProcessing, displaypp);
				}
				else
				{
					// Initialize with software TnL
					device = new Device(adapter, devtype, rendertarget.Handle,
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
			//device.DeviceResizing += new CancelEventHandler(CancelResize);

			// Create shader manager
			shaders = new ShaderManager(this);
			
			// Initialize settings
			SetupSettings();
			
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
			currentmode = Direct3D.Adapters[adapter].CurrentDisplayMode;

			// Make present parameters
			displaypp.Windowed = true;
			displaypp.SwapEffect = SwapEffect.Discard;
			displaypp.BackBufferCount = 1;
			displaypp.BackBufferFormat = currentmode.Format;
			displaypp.BackBufferWidth = rendertarget.ClientSize.Width;
			displaypp.BackBufferHeight = rendertarget.ClientSize.Height;
			displaypp.EnableAutoDepthStencil = true;
			displaypp.AutoDepthStencilFormat = Format.D16;
			displaypp.Multisample = MultisampleType.None;
			displaypp.PresentationInterval = PresentInterval.Immediate;

			// Return result
			return displaypp;
		}
		
		#endregion

		#region ================== Resetting

		// This registers a resource
		public void RegisterResource(ID3DResource res)
		{
			// Add resource
			resources.Add(res);
		}

		// This unregisters a resource
		public void UnregisterResource(ID3DResource res)
		{
			// Remove resource
			resources.Remove(res);
		}
		
		// This resets the device and returns true on success
		public bool Reset()
		{
			PresentParameters displaypp;

			// Unload all Direct3D resources
			foreach(ID3DResource res in resources) res.UnloadResource();
			
			// Lose backbuffers
			backbuffer.Dispose();
			depthbuffer.Dispose();
			backbuffer = null;
			depthbuffer = null;

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
			
			// Reload all Direct3D resources
			foreach(ID3DResource res in resources) res.ReloadResource();

			// Success
			return true;
		}

		#endregion
		
		#region ================== Rendering

		// This begins a drawing session
		public bool StartRendering(bool clear, int backcolor, Surface target, Surface depthbuffer)
		{
			CooperativeLevel coopresult;

			// When minimized, do not render anything
			if(General.MainWindow.WindowState != FormWindowState.Minimized)
			{
				// Test the cooperative level
				coopresult = device.CheckCooperativeLevel();
				
				// Check if device must be reset
				if(coopresult == CooperativeLevel.DeviceNotReset)
				{
					// Device is lost and must be reset now
					return Reset();
				}
				// Check if device is lost
				else if(coopresult == CooperativeLevel.DeviceLost)
				{
					// Device is lost and cannot be reset now
					return false;
				}

				// Set rendertarget
				device.SetDepthStencilSurface(depthbuffer);
				device.SetRenderTarget(0, target);
				
				// Clear the screen
				if(clear)
				{
					if(depthbuffer != null)
						device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, backcolor, 1f, 0);
					else
						device.Clear(ClearFlags.Target, backcolor, 1f, 0);
				}

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
			}
			// Errors are not a problem here
			catch(Exception) { }
		}

		// This presents what has been drawn
		public void Present()
		{
			try
			{
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
