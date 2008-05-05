
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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using CodeImp.DoomBuilder.IO;
using System.Windows.Forms;
using SlimDX.Direct3D9;
using CodeImp.DoomBuilder.Config;
using System.Threading;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	public sealed class DataManager
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		// Data containers
		private List<DataReader> containers;

		// Palette
		private Playpal palette;
		
		// Textures
		private Dictionary<long, ImageData> textures;
		private List<string> texturenames;
		
		// Flats
		private Dictionary<long, ImageData> flats;
		private List<string> flatnames;

		// Sprites
		private Dictionary<long, ImageData> sprites;

		// Background loading
		private LinkedList<ImageData> loadlist;
		private Thread backgroundloader;
		
		// Special images
		private ImageData missingtexture3d;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public Playpal Palette { get { return palette; } }
		public ICollection<ImageData> Textures { get { return textures.Values; } }
		public ICollection<ImageData> Flats { get { return flats.Values; } }
		public List<string> TextureNames { get { return texturenames; } }
		public List<string> FlatNames { get { return flatnames; } }
		public bool IsDisposed { get { return isdisposed; } }
		public ImageData MissingTexture3D { get { return missingtexture3d; } }
		
		public bool IsLoading
		{
			get
			{
				if(loadlist != null)
				{
					return (backgroundloader != null) && backgroundloader.IsAlive && (loadlist.Count > 0);
				}
				else
				{
					return false;
				}
			}
		}
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal DataManager()
		{
			// We have no destructor
			GC.SuppressFinalize(this);

			// Load special images
			missingtexture3d = new ResourceImage("MissingTexture3D.png");
			missingtexture3d.LoadImage();
		}

		// Disposer
		internal void Dispose()
		{
			// Not already disposed?
			if(!isdisposed)
			{
				// Clean up
				Unload();
				missingtexture3d.Dispose();
				missingtexture3d = null;
				
				// Done
				isdisposed = true;
			}
		}

		#endregion

		#region ================== Loading / Unloading

		// This loads all data resources
		internal void Load(DataLocationList configlist, DataLocationList maplist, DataLocation maplocation)
		{
			DataLocationList all = DataLocationList.Combined(configlist, maplist);
			all.Add(maplocation);
			Load(all);
		}

		// This loads all data resources
		internal void Load(DataLocationList configlist, DataLocationList maplist)
		{
			DataLocationList all = DataLocationList.Combined(configlist, maplist);
			Load(all);
		}

		// This loads all data resources
		internal void Load(DataLocationList locations)
		{
			int texcount, flatcount, spritecount;
			DataReader c;
			
			// Create collections
			containers = new List<DataReader>();
			textures = new Dictionary<long, ImageData>();
			flats = new Dictionary<long, ImageData>();
			sprites = new Dictionary<long, ImageData>();
			texturenames = new List<string>();
			flatnames = new List<string>();
			loadlist = new LinkedList<ImageData>();
			
			// Go for all locations
			foreach(DataLocation dl in locations)
			{
				// Nothing chosen yet
				c = null;

				// TODO: Make this work more elegant using reflection.
				// Make DataLocation.type of type Type and assign the
				// types of the desired reader classes.

				try
				{
					// Choose container type
					switch(dl.type)
					{
						// WAD file container
						case DataLocation.RESOURCE_WAD:
							c = new WADReader(dl);
							break;

						// Directory container
						case DataLocation.RESOURCE_DIRECTORY:
							c = new DirectoryReader(dl);
							break;

						// PK3 file container
						case DataLocation.RESOURCE_PK3:
							c = new PK3Reader(dl);
							break;
					}
				}
				catch(Exception)
				{
					// Unable to load resource
					General.ShowErrorMessage("Unable to load resources from location \"" + dl.location + "\". Please make sure the location is accessible and not in use by another program.", MessageBoxButtons.OK);
					continue;
				}	
				
				// Add container
				if(c != null) containers.Add(c);
			}

			// Load stuff
			LoadPalette();
			texcount = LoadTextures();
			flatcount = LoadFlats();
			spritecount = LoadSprites();

			// Sort names
			texturenames.Sort();
			flatnames.Sort();
			
			// Start background loading
			StartBackgroundLoader();

			// Output info
			General.WriteLogLine("Loaded " + texcount + " textures, " + flatcount + " flats, " + spritecount + " sprites");
		}

		// This unloads all data
		internal void Unload()
		{
			// Stop background loader
			StopBackgroundLoader();
			
			// Dispose resources
			foreach(KeyValuePair<long, ImageData> i in textures) i.Value.Dispose();
			foreach(KeyValuePair<long, ImageData> i in flats) i.Value.Dispose();
			foreach(KeyValuePair<long, ImageData> i in sprites) i.Value.Dispose();
			palette = null;

			// Dispose containers
			foreach(DataReader c in containers) c.Dispose();
			containers.Clear();

			// Trash collections
			containers = null;
			textures = null;
			flats = null;
			sprites = null;
			texturenames = null;
			flatnames = null;
			loadlist = null;
		}

		#endregion
		
		#region ================== Suspend / Resume

		// This suspends data resources
		internal void Suspend()
		{
			// Stop background loader
			StopBackgroundLoader();
			
			// Go for all containers
			foreach(DataReader d in containers)
			{
				// Suspend
				General.WriteLogLine("Suspended data resource '" + d.Location.location + "'");
				d.Suspend();
			}
		}

		// This resumes data resources
		internal void Resume()
		{
			// Go for all containers
			foreach(DataReader d in containers)
			{
				try
				{
					// Resume
					General.WriteLogLine("Resumed data resource '" + d.Location.location + "'");
					d.Resume();
				}
				catch(Exception)
				{
					// Unable to load resource
					General.ShowErrorMessage("Unable to load resources from location \"" + d.Location.location + "\". Please make sure the location is accessible and not in use by another program.", MessageBoxButtons.OK);
				}
			}
			
			// Start background loading
			StartBackgroundLoader();
		}
		
		#endregion

		#region ================== Background Loading

		// This starts background loading
		private void StartBackgroundLoader()
		{
			// If a loader is already running, stop it first
			if(backgroundloader != null) StopBackgroundLoader();

			// Start a low priority thread to load images in background
			General.WriteLogLine("Starting background resource loading...");
			backgroundloader = new Thread(new ThreadStart(BackgroundLoad));
			backgroundloader.Name = "BackgroundLoader";
			backgroundloader.Priority = ThreadPriority.Lowest;
			backgroundloader.Start();
		}

		// This stops background loading
		private void StopBackgroundLoader()
		{
			LinkedListNode<ImageData> n;
			
			General.WriteLogLine("Stopping background resource loading...");
			if(backgroundloader != null)
			{
				// Stop the thread and wait for it to end
				backgroundloader.Interrupt();
				backgroundloader.Join();

				// Reset load states on all images in the list
				n = loadlist.First;
				while(n != null)
				{
					n.Value.LoadState = ImageData.LOADSTATE_NONE;
					n.Value.LoadingTicket = null;
					n = n.Next;
				}
				loadlist.Clear();
				
				// Done
				backgroundloader = null;
				General.MainWindow.UpdateStatusIcon();
			}
		}

		// The background loader
		private void BackgroundLoad()
		{
			try
			{
				do
				{
					// Get next item
					ImageData image = null;
					lock(loadlist)
					{
						// Anything to do?
						if(loadlist.Count > 0)
						{
							// Fetch image
							image = loadlist.First.Value;
							image.LoadingTicket = null;
							loadlist.RemoveFirst();
							
							// Load or unload this image?
							switch(image.LoadState)
							{
								// Load image
								case ImageData.LOADSTATE_LOAD:
									image.LoadImage();
									//image.CreateTexture();	// Impossible from different thread
									break;

								// Unload image
								case ImageData.LOADSTATE_TRASH:
									image.UnloadImage();
									break;
							}
						}
					}

					// Did we do something?
					if(image != null)
					{
						// Wait a bit and update icon
						General.MainWindow.UpdateStatusIcon();
						Thread.Sleep(1);
					}
					else
					{
						// Wait longer to release CPU resources
						Thread.Sleep(50);
					}
				}
				while(true);
			}
			catch(ThreadInterruptedException)
			{
				return;
			}
		}

		// This adds an image for background loading or unloading
		public void BackgroundLoadImage(ImageData img, bool load)
		{
			int loadstate = load ? ImageData.LOADSTATE_LOAD : ImageData.LOADSTATE_TRASH;
			
			lock(loadlist)
			{
				// Already in the list?
				if(img.LoadingTicket != null)
				{
					// Just change the state
					img.LoadState = loadstate;
				}
				else
				{
					// Set load state and add to list
					img.LoadState = loadstate;
					img.LoadingTicket = loadlist.AddLast(img);
				}
			}
			
			// Update icon
			General.MainWindow.UpdateStatusIcon();
		}

		// This removes an image from background loading
		// This does not work for images that are being unloaded!
		public void BackgroundCancelImage(ImageData img)
		{
			// Queued?
			if(img.LoadingTicket != null)
			{
				// Not being trashed?
				if(img.LoadState != ImageData.LOADSTATE_TRASH)
				{
					lock(loadlist)
					{
						// Remove it from queue
						LinkedListNode<ImageData> ticket = img.LoadingTicket;
						img.LoadingTicket = null;
						loadlist.Remove(ticket);
					}
					
					// Update icon
					General.MainWindow.UpdateStatusIcon();
				}
			}
		}
		
		#endregion
		
		#region ================== Palette

		// This loads the PLAYPAL palette
		private void LoadPalette()
		{
			// Go for all opened containers
			for(int i = containers.Count - 1; i >= 0; i--)
			{
				// Load palette
				palette = containers[i].LoadPalette();
				if(palette != null) break;
			}

			// Make empty palette when still no palette found
			if(palette == null)
			{
				General.WriteLogLine("WARNING: None of the loaded resources define a color palette!");
				palette = new Playpal();
			}
		}

		#endregion

		#region ================== Textures
		
		// This loads the textures
		private int LoadTextures()
		{
			ICollection<ImageData> images;
			PatchNames pnames = new PatchNames();
			PatchNames newpnames;
			int counter = 0;
			
			// Go for all opened containers
			foreach(DataReader dr in containers)
			{
				// Load PNAMES info
				// Note that pnames is NOT set to null in the loop
				// because if a container has no pnames, the pnames
				// of the previous (higher) container should be used.
				newpnames = dr.LoadPatchNames();
				if(newpnames != null) pnames = newpnames;

				// Load textures
				images = dr.LoadTextures(pnames);
				if(images != null)
				{
					// Go for all textures
					foreach(ImageData img in images)
					{
						// Add or replace in textures list
						if(!textures.ContainsKey(img.LongName)) texturenames.Add(img.Name);
						textures.Remove(img.LongName);
						textures.Add(img.LongName, img);
						counter++;
						
						// Also add as flat when using mixed resources
						if(General.Map.Config.MixTexturesFlats)
						{
							if(!flats.ContainsKey(img.LongName)) flatnames.Add(img.Name);
							flats.Remove(img.LongName);
							flats.Add(img.LongName, img);
						}
					}
				}
			}
			
			// Output info
			return counter;
		}
		
		// This returns a specific patch stream
		internal Stream GetPatchData(string pname)
		{
			Stream patch;

			// Go for all opened containers
			for(int i = containers.Count - 1; i >= 0; i--)
			{
				// This contain provides this patch?
				patch = containers[i].GetPatchData(pname);
				if(patch != null) return patch;
			}

			// No such patch found
			return null;
		}
		
		// This returns an image by string
		public ImageData GetTextureImage(string name)
		{
			// Get the long name
			long longname = Lump.MakeLongName(name);
			return GetTextureImage(longname);
		}

		// This returns an image by long
		public ImageData GetTextureImage(long longname)
		{
			// Does this texture exist?
			if(textures.ContainsKey(longname))
			{
				// Return texture
				return textures[longname];
			}
			else
			{
				// Return null image
				return new NullImage();
			}
		}
		
		// This returns a bitmap by string
		public Bitmap GetTextureBitmap(string name)
		{
			ImageData img = GetTextureImage(name);
			img.LoadImage();
			return img.Bitmap;
		}

		// This returns a bitmap by string
		public Bitmap GetTextureBitmap(long longname)
		{
			ImageData img = GetTextureImage(longname);
			img.LoadImage();
			return img.Bitmap;
		}

		// This returns a texture by string
		public Texture GetTextureTexture(string name)
		{
			ImageData img = GetTextureImage(name);
			img.LoadImage();
			img.CreateTexture();
			return img.Texture;
		}

		// This returns a texture by string
		public Texture GetTextureTexture(long longname)
		{
			ImageData img = GetTextureImage(longname);
			img.LoadImage();
			img.CreateTexture();
			return img.Texture;
		}
		
		#endregion

		#region ================== Flats

		// This loads the flats
		private int LoadFlats()
		{
			ICollection<ImageData> images;
			int counter = 0;
			
			// Go for all opened containers
			foreach(DataReader dr in containers)
			{
				// Load flats
				images = dr.LoadFlats();
				if(images != null)
				{
					// Go for all flats
					foreach(ImageData img in images)
					{
						// Add or replace in flats list
						if(!flats.ContainsKey(img.LongName)) flatnames.Add(img.Name);
						flats.Remove(img.LongName);
						flats.Add(img.LongName, img);
						counter++;

						// Also add as texture when using mixed resources
						if(General.Map.Config.MixTexturesFlats)
						{
							if(!textures.ContainsKey(img.LongName)) texturenames.Add(img.Name);
							textures.Remove(img.LongName);
							textures.Add(img.LongName, img);
						}
					}
				}
			}

			// Output info
			return counter;
		}

		// This returns a specific flat stream
		internal Stream GetFlatData(string pname)
		{
			Stream flat;

			// Go for all opened containers
			for(int i = containers.Count - 1; i >= 0; i--)
			{
				// This contain provides this flat?
				flat = containers[i].GetFlatData(pname);
				if(flat != null) return flat;
			}

			// No such patch found
			return null;
		}
		
		// This returns an image by string
		public ImageData GetFlatImage(string name)
		{
			// Get the long name
			long longname = Lump.MakeLongName(name);
			return GetFlatImage(longname);
		}

		// This returns an image by long
		public ImageData GetFlatImage(long longname)
		{
			// Does this flat exist?
			if(flats.ContainsKey(longname))
			{
				// Return flat
				return flats[longname];
			}
			else
			{
				// Return null image
				return new NullImage();
			}
		}

		// This returns a bitmap by string
		public Bitmap GetFlatBitmap(string name)
		{
			ImageData img = GetFlatImage(name);
			img.LoadImage();
			return img.Bitmap;
		}

		// This returns a bitmap by string
		public Bitmap GetFlatBitmap(long longname)
		{
			ImageData img = GetFlatImage(longname);
			img.LoadImage();
			return img.Bitmap;
		}

		// This returns a texture by string
		public Texture GetFlatTexture(string name)
		{
			ImageData img = GetFlatImage(name);
			img.LoadImage();
			img.CreateTexture();
			return img.Texture;
		}

		// This returns a texture by string
		public Texture GetFlatTexture(long longname)
		{
			ImageData img = GetFlatImage(longname);
			img.LoadImage();
			img.CreateTexture();
			return img.Texture;
		}
		
		#endregion

		#region ================== Sprites

		// This loads the sprites
		private int LoadSprites()
		{
			Stream spritedata = null;
			SpriteImage image;
			
			// Go for all things
			foreach(ThingTypeInfo ti in General.Map.Config.Things)
			{
				// Sprite not added to collection yet?
				if(!sprites.ContainsKey(ti.SpriteLongName))
				{
					// Go for all opened containers
					for(int i = containers.Count - 1; i >= 0; i--)
					{
						// This contain provides this sprite?
						spritedata = containers[i].GetSpriteData(ti.Sprite);
						if(spritedata != null) break;
					}

					// Found anything?
					if(spritedata != null)
					{
						// Make new sprite image
						image = new SpriteImage(ti.Sprite);

						// Add to collection
						sprites.Add(ti.SpriteLongName, image);
					}
				}
			}

			// Output info
			return sprites.Count;
		}

		// This returns an image by long
		public ImageData GetSpriteImage(string name)
		{
			Stream spritedata = null;
			long longname = Lump.MakeLongName(name);
			SpriteImage image;

			// Sprite already loaded?
			if(sprites.ContainsKey(longname))
			{
				// Return exiting sprite
				return sprites[longname];
			}
			else
			{
				// Go for all opened containers
				for(int i = containers.Count - 1; i >= 0; i--)
				{
					// This contain provides this sprite?
					spritedata = containers[i].GetSpriteData(name);
					if(spritedata != null) break;
				}

				// Found anything?
				if(spritedata != null)
				{
					// Make new sprite image
					image = new SpriteImage(name);

					// Add to collection
					sprites.Add(longname, image);

					// Return result
					return image;
				}
				else
				{
					// Return null image
					return new NullImage();
				}
			}
		}

		// This returns a bitmap by string
		public Bitmap GetSpriteBitmap(string name)
		{
			ImageData img = GetSpriteImage(name);
			img.LoadImage();
			return img.Bitmap;
		}

		// This returns a texture by string
		public Texture GetSpriteTexture(string name)
		{
			ImageData img = GetSpriteImage(name);
			img.LoadImage();
			img.CreateTexture();
			return img.Texture;
		}
		
		#endregion
	}
}
