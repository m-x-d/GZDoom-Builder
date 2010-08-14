
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
using System.ComponentModel;
using CodeImp.DoomBuilder.Map;
using SlimDX.Direct3D9;
using SlimDX;
using CodeImp.DoomBuilder.Geometry;
using System.Drawing.Imaging;
using CodeImp.DoomBuilder.Data;
using CodeImp.DoomBuilder.Editing;

using Configuration = CodeImp.DoomBuilder.IO.Configuration;

#endregion

namespace CodeImp.DoomBuilder.Rendering
{
	internal class SurfaceManager : ID3DResource
	{
		#region ================== Constants
		
		// The true maximum lies at 65535 if I remember correctly, but that
		// is a scary big number for a vertexbuffer.
		private const int MAX_VERTICES_PER_BUFFER = 30000;
		
		#endregion
		
		#region ================== Variables
		
		// Set of buffers for a specific number of vertices per sector
		private Dictionary<int, SurfaceBufferSet> sets;
		
		// List of buffers that are locked
		// This is null when not in the process of updating
		private List<VertexBuffer> lockedbuffers;
		
		// Surface to be rendered.
		// Each BinaryHeap in the Dictionary contains all geometry that needs
		// to be rendered with the associated ImageData.
		// The BinaryHeap sorts the geometry by sector to minimize stream switchs.
		// This is null when not in the process of rendering
		private Dictionary<ImageData, List<SurfaceEntry>> surfaces;
		
		// This is 1 to add the number of vertices to the offset
		// (effectively rendering the ceiling vertices instead of floor vertices)
		private int surfacevertexoffsetmul;
		
		// This is set to true when the resources have been unloaded
		private bool resourcesunloaded;

		#endregion

		#region ================== Properties

		#endregion

		#region ================== Constructor / Disposer
		
		// Constructor
		public SurfaceManager()
		{
			sets = new Dictionary<int, SurfaceBufferSet>();
			lockedbuffers = new List<VertexBuffer>();

			General.Map.Graphics.RegisterResource(this);
		}
		
		// Disposer
		public void Dispose()
		{
			if(sets != null)
			{
				General.Map.Graphics.UnregisterResource(this);
				
				// Dispose all sets
				foreach(KeyValuePair<int, SurfaceBufferSet> set in sets)
				{
					// Dispose vertex buffers
					for(int i = 0; i < set.Value.buffers.Count; i++)
					{
						if(set.Value.buffers[i] != null)
						{
							set.Value.buffers[i].Dispose();
							set.Value.buffers[i] = null;
						}
					}
				}
				
				sets = null;
			}
		}
		
		#endregion

		#region ================== Management

		// Called when all resource must be unloaded
		public void UnloadResource()
		{
			resourcesunloaded = true;
			foreach(KeyValuePair<int, SurfaceBufferSet> set in sets)
			{
				// Dispose vertex buffers
				for(int i = 0; i < set.Value.buffers.Count; i++)
				{
					if(set.Value.buffers[i] != null)
					{
						set.Value.buffers[i].Dispose();
						set.Value.buffers[i] = null;
					}
				}
			}
			
			lockedbuffers.Clear();
		}

		// Called when all resource must be reloaded
		public void ReloadResource()
		{
			foreach(KeyValuePair<int, SurfaceBufferSet> set in sets)
			{
				// Rebuild vertex buffers
				for(int i = 0; i < set.Value.buffersizes.Count; i++)
				{
					// Make the new buffer!
					VertexBuffer b = new VertexBuffer(General.Map.Graphics.Device, FlatVertex.Stride * set.Value.buffersizes[i],
													Usage.WriteOnly | Usage.Dynamic, VertexFormat.None, Pool.Default);

					// Start refilling the buffer with sector geometry
					int vertexoffset = 0;
					DataStream bstream = b.Lock(0, FlatVertex.Stride * set.Value.buffersizes[i], LockFlags.Discard);
					foreach(SurfaceEntry e in set.Value.entries)
					{
						if(e.bufferindex == i)
						{
							// Fill buffer
							bstream.Seek(e.vertexoffset * FlatVertex.Stride, SeekOrigin.Begin);
							bstream.WriteRange(e.floorvertices);
							bstream.WriteRange(e.ceilvertices);
						}
					}

					// Unlock buffer
					b.Unlock();
					bstream.Dispose();
					
					// Add to list
					set.Value.buffers[i] = b;
				}
			}
			
			resourcesunloaded = false;
		}
		
		// This resets all buffers and requires all sectors to get new entries
		public void Reset()
		{
			// Clear all items
			foreach(KeyValuePair<int, SurfaceBufferSet> set in sets)
			{
				foreach(SurfaceEntry entry in set.Value.entries)
				{
					entry.numvertices = -1;
					entry.bufferindex = -1;
				}
				
				foreach(SurfaceEntry entry in set.Value.holes)
				{
					entry.numvertices = -1;
					entry.bufferindex = -1;
				}

				foreach(VertexBuffer vb in set.Value.buffers)
					vb.Dispose();
			}

			// New dictionary
			sets = new Dictionary<int, SurfaceBufferSet>();
		}

		// Updating sector surface geometry should go in this order;
		// - Triangulate sectors
		// - Call FreeSurfaces to remove entries that have changed number of vertices
		// - Call AllocateBuffers
		// - Call UpdateSurfaces to add/update entries
		// - Call UnlockBuffers
		
		// This (re)allocates the buffers based on an analysis of the map
		// The map must be updated (triangulated) before calling this
		public void AllocateBuffers()
		{
			// Make analysis of sector geometry
			Dictionary<int, int> sectorverts = new Dictionary<int, int>();
			foreach(Sector s in General.Map.Map.Sectors)
			{
				if(s.Triangles != null)
				{
					// We count the number of sectors that have specific number of vertices
					if(!sectorverts.ContainsKey(s.Triangles.Vertices.Count))
						sectorverts.Add(s.Triangles.Vertices.Count, 0);
					sectorverts[s.Triangles.Vertices.Count]++;
				}
			}
			
			// Now (re)allocate the needed buffers
			foreach(KeyValuePair<int, int> sv in sectorverts)
			{
				// Zero vertices can't be drawn
				if(sv.Key > 0)
				{
					SurfaceBufferSet set = GetSet(sv.Key);
					
					// Calculte how many free entries we need
					int neededentries = sv.Value;
					int freeentriesneeded = neededentries - set.entries.Count;

					// Allocate the space needed
					EnsureFreeBufferSpace(set, freeentriesneeded);
				}
			}
		}

		// This ensures there is enough space for a given number of free entries (also adds new bufers if needed)
		private void EnsureFreeBufferSpace(SurfaceBufferSet set, int freeentries)
		{
			DataStream bstream = null;
			VertexBuffer vb = null;
			
			// Check if we have to add entries
			int addentries = freeentries - set.holes.Count;

			// Begin resizing buffers starting with the last in this set
			int bufferindex = set.buffers.Count - 1;

			// Calculate the maximum number of entries we can put in a new buffer
			// Note that verticesperentry is the number of vertices multiplied by 2, because
			// we have to store both the floor and ceiling
			int verticesperentry = set.numvertices * 2;
			int maxentriesperbuffer = MAX_VERTICES_PER_BUFFER / verticesperentry;

			// Make a new bufer when the last one is full
			if((bufferindex > -1) && (set.buffersizes[bufferindex] >= (maxentriesperbuffer * verticesperentry)))
				bufferindex = -1;
			
			while(addentries > 0)
			{
				// Create a new buffer?
				if((bufferindex == -1) || (bufferindex > (set.buffers.Count - 1)))
				{
					// Determine the number of entries we will be making this buffer for
					int bufferentries = (addentries > maxentriesperbuffer) ? maxentriesperbuffer : addentries;

					// Calculate the number of vertices that will be
					int buffernumvertices = bufferentries * verticesperentry;

					if(!resourcesunloaded)
					{
						// Make the new buffer!
						vb = new VertexBuffer(General.Map.Graphics.Device, FlatVertex.Stride * buffernumvertices,
												Usage.WriteOnly | Usage.Dynamic, VertexFormat.None, Pool.Default);

						// Add it.
						set.buffers.Add(vb);
					}
					else
					{
						// We can't make a vertexbuffer right now
						set.buffers.Add(null);
					}
					
					// Also add available entries as holes, because they are not used yet.
					set.buffersizes.Add(buffernumvertices);
					for(int i = 0; i < bufferentries; i++)
						set.holes.Add(new SurfaceEntry(set.numvertices, set.buffers.Count - 1, i * verticesperentry));

					// Done
					addentries -= bufferentries;
				}
				// Reallocate a buffer
				else
				{
					// Trash the old buffer
					if(set.buffers[bufferindex].Tag != null)
					{
						bstream = (DataStream)set.buffers[bufferindex].Tag;
						set.buffers[bufferindex].Unlock();
						bstream.Dispose();
						set.buffers[bufferindex].Tag = null;
					}

					if((set.buffers[bufferindex] != null) && !resourcesunloaded)
						set.buffers[bufferindex].Dispose();

					// Get the entries that are in this buffer only
					List<SurfaceEntry> theseentries = new List<SurfaceEntry>();
					foreach(SurfaceEntry e in set.entries)
					{
						if(e.bufferindex == bufferindex)
							theseentries.Add(e);
					}

					// Determine the number of entries we will be making this buffer for
					int bufferentries = ((theseentries.Count + addentries) > maxentriesperbuffer) ? maxentriesperbuffer : (theseentries.Count + addentries);

					// Calculate the number of vertices that will be
					int buffernumvertices = bufferentries * verticesperentry;

					if(!resourcesunloaded)
					{
						// Make the new buffer and lock it
						vb = new VertexBuffer(General.Map.Graphics.Device, FlatVertex.Stride * buffernumvertices,
												Usage.WriteOnly | Usage.Dynamic, VertexFormat.None, Pool.Default);
						bstream = vb.Lock(0, FlatVertex.Stride * theseentries.Count * verticesperentry, LockFlags.Discard);
					}
					
					// Start refilling the buffer with sector geometry
					int vertexoffset = 0;
					foreach(SurfaceEntry e in theseentries)
					{
						if(!resourcesunloaded)
						{
							// Fill buffer
							bstream.WriteRange(e.floorvertices);
							bstream.WriteRange(e.ceilvertices);
						}

						// Set the new location in the buffer
						e.vertexoffset = vertexoffset;

						// Move on
						vertexoffset += verticesperentry;
					}

					if(!resourcesunloaded)
					{
						// Unlock buffer
						vb.Unlock();
						bstream.Dispose();
						set.buffers[bufferindex] = vb;
					}
					else
					{
						// No vertex buffer at this time, sorry
						set.buffers[bufferindex] = null;
					}

					// Set the new buffer and add available entries as holes, because they are not used yet.
					set.buffersizes[bufferindex] = buffernumvertices;
					set.holes.Clear();
					for(int i = 0; i < bufferentries - theseentries.Count; i++)
						set.holes.Add(new SurfaceEntry(set.numvertices, bufferindex, i * verticesperentry + vertexoffset));

					// Done
					addentries -= bufferentries;
				}

				// Always continue in next (new) buffer
				bufferindex = set.buffers.Count;
			}
		}
		
		// This adds or updates sector geometry into a buffer.
		// Always specify the entry when a previous entry was already given for that sector!
		// Sector must set the floorvertices and ceilvertices members on the entry.
		// Returns the new surface entry for the stored geometry, floorvertices and ceilvertices will be preserved.
		public SurfaceEntry UpdateSurfaces(SurfaceEntry entry)
		{
			if(entry.floorvertices.Length != entry.ceilvertices.Length)
				General.Fail("Floor vertices has different length from ceiling vertices!");

			int numvertices = entry.floorvertices.Length;
			
			// Free entry when number of vertices have changed
			if((entry.numvertices != numvertices) && (entry.numvertices != -1))
				FreeSurfaces(entry);
			
			// Check if we can render this at all
			if(numvertices > 0)
			{
				SurfaceBufferSet set = GetSet(numvertices);

				// Update bounding box
				entry.UpdateBBox();
				
				// Check if we need a new entry
				if(entry.numvertices == -1)
				{
					EnsureFreeBufferSpace(set, 1);
					SurfaceEntry nentry = set.holes[set.holes.Count - 1];
					set.holes.RemoveAt(set.holes.Count - 1);
					nentry.ceilvertices = entry.ceilvertices;
					nentry.floorvertices = entry.floorvertices;
					nentry.floortexture = entry.floortexture;
					nentry.ceiltexture = entry.ceiltexture;
					nentry.bbox = entry.bbox;
					set.entries.Add(nentry);
					entry = nentry;
				}

				if(!resourcesunloaded)
				{
					// Lock the buffer
					DataStream bstream;
					VertexBuffer vb = set.buffers[entry.bufferindex];
					if(vb.Tag == null)
					{
						// Note: DirectX warns me that I am not using LockFlags.Discard or LockFlags.NoOverwrite here,
						// but we don't care (we don't have much of a choice since we want to update our data)
						bstream = vb.Lock(0, set.buffersizes[entry.bufferindex] * FlatVertex.Stride, LockFlags.None);
						vb.Tag = bstream;
						lockedbuffers.Add(vb);
					}
					else
					{
						bstream = (DataStream)vb.Tag;
					}

					// Write the vertices to buffer
					bstream.Seek(entry.vertexoffset * FlatVertex.Stride, SeekOrigin.Begin);
					bstream.WriteRange(entry.floorvertices);
					bstream.WriteRange(entry.ceilvertices);
				}
			}
			
			return entry;
		}

		// This frees the given surface entry
		public void FreeSurfaces(SurfaceEntry entry)
		{
			if((entry.numvertices > 0) && (entry.bufferindex > -1))
			{
				SurfaceBufferSet set = sets[entry.numvertices];
				set.entries.Remove(entry);
				SurfaceEntry newentry = new SurfaceEntry(entry);
				set.holes.Add(newentry);
			}
			entry.numvertices = -1;
			entry.bufferindex = -1;
		}
		
		// This unlocks the locked buffers
		public void UnlockBuffers()
		{
			if(!resourcesunloaded)
			{
				foreach(VertexBuffer vb in lockedbuffers)
				{
					if(vb.Tag != null)
					{
						DataStream bstream = (DataStream)vb.Tag;
						vb.Unlock();
						bstream.Dispose();
						vb.Tag = null;
					}
				}

				// Clear list
				lockedbuffers = new List<VertexBuffer>();
			}
		}
		
		// This gets or creates a set for a specific number of vertices
		private SurfaceBufferSet GetSet(int numvertices)
		{
			SurfaceBufferSet set;
			
			// Get or create the set
			if(!sets.ContainsKey(numvertices))
			{
				set = new SurfaceBufferSet();
				set.numvertices = numvertices;
				set.buffers = new List<VertexBuffer>();
				set.buffersizes = new List<int>();
				set.entries = new List<SurfaceEntry>();
				set.holes = new List<SurfaceEntry>();
				sets.Add(numvertices, set);
			}
			else
			{
				set = sets[numvertices];
			}

			return set;
		}
		
		#endregion
		
		#region ================== Rendering
		
		// This renders all sector floors
		internal void RenderSectorFloors(RectangleF viewport)
		{
			surfaces = new Dictionary<ImageData, List<SurfaceEntry>>();
			surfacevertexoffsetmul = 0;
			
			// Go for all surfaces as they are sorted in the buffers, so that
			// they are automatically already sorted by vertexbuffer
			foreach(KeyValuePair<int, SurfaceBufferSet> set in sets)
			{
				foreach(SurfaceEntry entry in set.Value.entries)
				{
					if(entry.bbox.IntersectsWith(viewport))
						AddSurfaceEntryForRendering(entry, entry.floortexture);
				}
			}
		}
		
		// This renders all sector ceilings
		internal void RenderSectorCeilings(RectangleF viewport)
		{
			surfaces = new Dictionary<ImageData, List<SurfaceEntry>>();
			surfacevertexoffsetmul = 1;
			
			// Go for all surfaces as they are sorted in the buffers, so that
			// they are automatically already sorted by vertexbuffer
			foreach(KeyValuePair<int, SurfaceBufferSet> set in sets)
			{
				foreach(SurfaceEntry entry in set.Value.entries)
				{
					if(entry.bbox.IntersectsWith(viewport))
						AddSurfaceEntryForRendering(entry, entry.ceiltexture);
				}
			}
		}

		// This renders all sector brightness levels
		internal void RenderSectorBrightness(RectangleF viewport)
		{
			surfaces = new Dictionary<ImageData, List<SurfaceEntry>>();
			surfacevertexoffsetmul = 0;
			
			// Go for all surfaces as they are sorted in the buffers, so that
			// they are automatically already sorted by vertexbuffer
			foreach(KeyValuePair<int, SurfaceBufferSet> set in sets)
			{
				foreach(SurfaceEntry entry in set.Value.entries)
				{
					if(entry.bbox.IntersectsWith(viewport))
						AddSurfaceEntryForRendering(entry, 0);
				}
			}
		}

		// This adds a surface entry to the list of surfaces
		private void AddSurfaceEntryForRendering(SurfaceEntry entry, long longimagename)
		{
			// Determine texture to use
			ImageData img;
			if(longimagename == 0)
			{
				img = General.Map.Data.WhiteTexture;
			}
			else
			{
				if(General.Map.Data.GetFlatExists(longimagename))
				{
					img = General.Map.Data.GetFlatImageKnown(longimagename);
					
					// Is the texture loaded?
					if(img.IsImageLoaded && !img.LoadFailed)
					{
						if(img.Texture == null) img.CreateTexture();
					}
					else
					{
						img = General.Map.Data.WhiteTexture;
					}
				}
				else
				{
					img = General.Map.Data.UnknownTexture3D;
				}
			}
			
			// Store by texture
			if(!surfaces.ContainsKey(img))
				surfaces.Add(img, new List<SurfaceEntry>());
			surfaces[img].Add(entry);
		}
		
		// This renders the sorted sector surfaces
		internal void RenderSectorSurfaces(D3DDevice graphics)
		{
			int counter = 0;
			if(!resourcesunloaded)
			{
				graphics.Shaders.Display2D.Begin();
				foreach(KeyValuePair<ImageData, List<SurfaceEntry>> imgsurfaces in surfaces)
				{
					// Set texture
					graphics.Shaders.Display2D.Texture1 = imgsurfaces.Key.Texture;
					if(!graphics.Shaders.Enabled) graphics.Device.SetTexture(0, imgsurfaces.Key.Texture);

					graphics.Shaders.Display2D.BeginPass(1);
					
					// Go for all surfaces
					VertexBuffer lastbuffer = null;
					foreach(SurfaceEntry entry in imgsurfaces.Value)
					{
						// Set the vertex buffer
						SurfaceBufferSet set = sets[entry.numvertices];
						if(set.buffers[entry.bufferindex] != lastbuffer)
						{
							lastbuffer = set.buffers[entry.bufferindex];
							graphics.Device.SetStreamSource(0, lastbuffer, 0, FlatVertex.Stride);
						}

						// Draw
						counter++;
						graphics.Device.DrawPrimitives(PrimitiveType.TriangleList, entry.vertexoffset + (entry.numvertices * surfacevertexoffsetmul), entry.numvertices / 3);
					}
					
					graphics.Shaders.Display2D.EndPass();
				}
				graphics.Shaders.Display2D.End();
			}
			Console.WriteLine("Calls: " + counter);
		}
		
		#endregion
	}
}
