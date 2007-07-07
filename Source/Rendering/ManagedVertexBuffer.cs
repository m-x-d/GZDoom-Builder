
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
using System.ComponentModel;
using CodeImp.DoomBuilder.Geometry;
using SlimDX;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	internal class ManagedVertexBuffer : IDisposable, IResource
	{
		#region ================== Constants

		// Minimum items
		private const int MIN_ITEMS = 500;

		#endregion

		#region ================== Events / Delegates

		public event ReloadResourceDelegate ReloadResources;

		#endregion

		#region ================== Variables

		// Buffer info
		private int bytesperitem;
		private int maxitems;
		private LinkedList<int> freeitems;

		// The vertexbuffer
		private VertexBuffer buffer;

		// The stream for updating
		private GraphicsStream stream;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public VertexBuffer VertexBuffer { get { return buffer; } }
		public int ItemCapacity { get { return maxitems; } }
		public int ItemCount { get { return maxitems - freeitems.Count; } }
		public bool IsDisposed { get { return isdisposed; } }

		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public ManagedVertexBuffer(int bytesperitem, int initialsize)
		{
			// Initialize
			this.bytesperitem = bytesperitem;
			this.maxitems = initialsize;
			if(this.maxitems < MIN_ITEMS) this.maxitems = MIN_ITEMS;
			this.freeitems = new LinkedList<int>();

			// Add free items to list
			for(int i = 0; i < maxitems; i++) freeitems.AddLast(i);

			// Create the buffer
			CreateBuffer();
			
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
				freeitems = null;
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Buffer

		// This unloads the vertex buffer
		public void UnloadResource()
		{
			// Clean up
			if(stream != null) EndUpdate();
			if(buffer != null) buffer.Dispose();

			// Done
			buffer = null;
		}

		// This reloads the vertex buffer
		public void ReloadResource()
		{
			// Create the buffer
			CreateBuffer();

			// Signal that the buffer needs to be rebuild
			if(ReloadResources != null) ReloadResources();
		}

		// This creates the buffer according to settings
		private void CreateBuffer()
		{
			// Create the buffer
			buffer = new VertexBuffer(General.Map.Graphics.Device, maxitems * bytesperitem,
									  Usage.WriteOnly, PTVertex.Format, Pool.Managed);
		}
		
		// This starts an update session
		public void BeginUpdate()
		{
			// Lock the buffer and get the stream
			stream = buffer.Lock(0, LockFlags.None);
		}
		
		// This stops an update session
		public void EndUpdate()
		{
			// Unlock the buffer
			buffer.Unlock();
			stream.Dispose();
			stream = null;
		}
		
		// This doubles the buffer size
		private void DoubleBuffer()
		{
			VertexBuffer newbuf;
			GraphicsStream newstream;
			byte[] copybuf;
			int newmaxitems;
			bool updating;
			
			// Not in an updating session yet?
			if(stream == null)
			{
				// Start updating
				BeginUpdate();
				
				// Remember to stop updating when done
				updating = false;
			}
			else
			{
				// Remember we want to keep the updating session open
				updating = true;
			}
			
			// Increase size
			newmaxitems = maxitems * 2;
			
			// Add free items to list
			for(int i = maxitems; i < newmaxitems; i++) freeitems.AddLast(i);

			// Create a new buffer
			newbuf = new VertexBuffer(General.Map.Graphics.Device, newmaxitems * bytesperitem,
									  Usage.WriteOnly, PTVertex.Format, Pool.Managed);

			// Copy old data to new buffer
			newstream = newbuf.Lock(0, LockFlags.None);
			stream.Seek(0, SeekOrigin.Begin);
			copybuf = new byte[maxitems * bytesperitem];
			stream.Read(copybuf, 0, copybuf.Length);
			newstream.Write(copybuf, 0, copybuf.Length);
			
			// Dispose old buffer
			buffer.Unlock();
			stream.Dispose();
			buffer.Dispose();

			// Switch to new buffer
			maxitems = newmaxitems;
			buffer = newbuf;
			stream = newstream;

			// Stop updating session?
			if(!updating)
			{
				// Stop updating now
				EndUpdate();
			}
		}

		#endregion

		#region ================== Items

		// This adds an item to the buffer and returns its unique index
		public int AddItem()
		{
			int itemindex;

			// Double the buffer for more items when none are available
			if(freeitems.Count == 0) DoubleBuffer();
			
			// Fetch the first free index from the list
			itemindex = freeitems.First.Value;
			freeitems.RemoveFirst();
			
			// Return result
			return itemindex;
		}

		// This frees an item
		public void FreeItem(int index)
		{
			// Add item back into the list
			freeitems.AddLast(index);
		}
		
		// This seeks the stream to the position for a specific item
		public void SeekToItem(int index)
		{
			// Seek to item start position
			stream.Seek(index * bytesperitem, SeekOrigin.Begin);
		}
		
		// This allows writing to the stream
		public void WriteItem<T>(T item) where T : struct
		{
			// Write the item
			stream.Write(item);
		}
		
		#endregion
	}
}
