
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
using CodeImp.DoomBuilder.Map;

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
		
		// Textures, Flats and Sprites
		private Dictionary<long, ImageData> textures;
		private List<string> texturenames;
		private Dictionary<long, ImageData> flats;
		private List<string> flatnames;
		private Dictionary<long, ImageData> sprites;
		private List<MatchingTextureSet> texturesets;
		private OthersTextureSet othertextures;
		
		// Background loading
		private Queue<ImageData> imageque;
		private Thread backgroundloader;
		private volatile bool updatedusedtextures;
		
		// Image previews
		private PreviewManager previews;
		
		// Special images
		private ImageData missingtexture3d;
		private ImageData hourglass3d;
		
		// Disposing
		private bool isdisposed = false;

		#endregion

		#region ================== Properties

		public Playpal Palette { get { return palette; } }
		public PreviewManager Previews { get { return previews; } }
		public ICollection<ImageData> Textures { get { return textures.Values; } }
		public ICollection<ImageData> Flats { get { return flats.Values; } }
		public List<string> TextureNames { get { return texturenames; } }
		public List<string> FlatNames { get { return flatnames; } }
		public bool IsDisposed { get { return isdisposed; } }
		public ImageData MissingTexture3D { get { return missingtexture3d; } }
		public ImageData Hourglass3D { get { return hourglass3d; } }
		internal ICollection<MatchingTextureSet> TextureSets { get { return texturesets; } }
		internal OthersTextureSet OthersTextureSet { get { return othertextures; } }
		
		public bool IsLoading
		{
			get
			{
				if(imageque != null)
				{
					return (backgroundloader != null) && backgroundloader.IsAlive && ((imageque.Count > 0) || previews.IsLoading);
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
			hourglass3d = new ResourceImage("Hourglass3D.png");
			hourglass3d.LoadImage();
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
			imageque = new Queue<ImageData>();
			previews = new PreviewManager();
			texturesets = new List<MatchingTextureSet>();
			
			// Load texture sets
			foreach(DefinedTextureSet ts in General.Map.ConfigSettings.TextureSets)
				texturesets.Add(new MatchingTextureSet(ts));
			
			// Sort the texture sets
			texturesets.Sort();
			
			// Other textures set
			othertextures = new OthersTextureSet();
			
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

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();
			
			// Add texture names to texture sets
			foreach(KeyValuePair<long, ImageData> img in textures)
			{
				// Add to all sets where it matches
				bool matchfound = false;
				foreach(MatchingTextureSet ms in texturesets)
					matchfound |= ms.AddTexture(img.Value);
				
				// If not matched in any set, add it to the others
				if(!matchfound) othertextures.AddTexture(img.Value);
			}
			
			// Add flat names to texture sets
			foreach(KeyValuePair<long, ImageData> img in flats)
			{
				// Add to all sets where it matches
				bool matchfound = false;
				foreach(MatchingTextureSet ms in texturesets)
					matchfound |= ms.AddFlat(img.Value);
				
				// If not matched in any set, add it to the others
				if(!matchfound) othertextures.AddFlat(img.Value);
			}
			
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
			
			// Dispose preview manager
			previews.Dispose();
			previews = null;
			
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
			imageque = null;
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
			backgroundloader.Name = "Background Loader";
			backgroundloader.Priority = ThreadPriority.Lowest;
			backgroundloader.Start();
		}

		// This stops background loading
		private void StopBackgroundLoader()
		{
			ImageData img;
			
			General.WriteLogLine("Stopping background resource loading...");
			if(backgroundloader != null)
			{
				// Stop the thread and wait for it to end
				backgroundloader.Interrupt();
				backgroundloader.Join();

				// Reset load states on all images in the list
				while(imageque.Count > 0)
				{
					img = imageque.Dequeue();
					
					switch(img.ImageState)
					{
						case ImageLoadState.Loading:
							img.ImageState = ImageLoadState.None;
							break;

						case ImageLoadState.Unloading:
							img.ImageState = ImageLoadState.Ready;
							break;
					}

					switch(img.PreviewState)
					{
						case ImageLoadState.Loading:
							img.PreviewState = ImageLoadState.None;
							break;

						case ImageLoadState.Unloading:
							img.PreviewState = ImageLoadState.Ready;
							break;
					}
				}
				
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
					// Do we have to update the used-in-map status?
					if(updatedusedtextures)
					{
						BackgroundUpdateUsedTextures();
						updatedusedtextures = false;
					}
					
					// Get next item
					ImageData image = null;
					lock(imageque)
					{
						// Fetch next image to process
						if(imageque.Count > 0) image = imageque.Dequeue();
					}
					
					// Any image to process?
					if(image != null)
					{
						// Load this image?
						if(image.IsReferenced && (image.ImageState != ImageLoadState.Ready))
						{
							image.LoadImage();
						}
						
						// Unload this image?
						if(!image.IsReferenced && (image.ImageState != ImageLoadState.None))
						{
							// Still unreferenced?
							image.UnloadImage();
						}
					}
					
					// Doing something?
					if(image != null)
					{
						// Wait a bit and update icon
						General.MainWindow.UpdateStatusIcon();
						Thread.Sleep(0);
					}
					else
					{
						// Process previews only when we don't have images to process
						// because these are lower priority than the actual images
						if(previews.BackgroundLoad())
						{
							// Wait a bit and update icon
							General.MainWindow.UpdateStatusIcon();
							Thread.Sleep(0);
						}
						else
						{
							// Wait longer to release CPU resources
							Thread.Sleep(50);
						}
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
		internal void ProcessImage(ImageData img)
		{
			// Load this image?
			if((img.ImageState == ImageLoadState.None) && img.IsReferenced)
			{
				// Add for loading
				img.ImageState = ImageLoadState.Loading;
				lock(imageque) { imageque.Enqueue(img); }
			}
			
			// Unload this image?
			if((img.ImageState == ImageLoadState.Ready) && !img.IsReferenced)
			{
				// Add for unloading
				img.ImageState = ImageLoadState.Unloading;
				lock(imageque) { imageque.Enqueue(img); }
			}
			
			// Update icon
			General.MainWindow.UpdateStatusIcon();
		}

		// This updates the used-in-map status on all textures and flats
		private void BackgroundUpdateUsedTextures()
		{
			Dictionary<long, long> useditems = new Dictionary<long, long>();

			// Go through the map to find the used textures
			foreach(Sidedef sd in General.Map.Map.Sidedefs)
			{
				// Add used textures to dictionary
				if(sd.HighTexture.Length > 0) useditems[sd.LongHighTexture] = 0;
				if(sd.LowTexture.Length > 0) useditems[sd.LongMiddleTexture] = 0;
				if(sd.MiddleTexture.Length > 0) useditems[sd.LongLowTexture] = 0;
			}

			// Go through the map to find the used flats
			foreach(Sector s in General.Map.Map.Sectors)
			{
				// Add used flats to dictionary
				useditems[s.LongFloorTexture] = 0;
				useditems[s.LongCeilTexture] = 0;
			}

			// Set used on all textures
			foreach(KeyValuePair<long, ImageData> i in textures)
				i.Value.SetUsedInMap(useditems.ContainsKey(i.Key));

			// Flats are not already included with the textures?
			if(!General.Map.Config.MixTexturesFlats)
			{
				// Set used on all flats
				foreach(KeyValuePair<long, ImageData> i in flats)
					i.Value.SetUsedInMap(useditems.ContainsKey(i.Key));
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
			long firsttexture = 0;
			
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
						if(firsttexture == 0) firsttexture = img.LongName;
						counter++;
						
						// Also add as flat when using mixed resources
						if(General.Map.Config.MixTexturesFlats)
						{
							if(!flats.ContainsKey(img.LongName)) flatnames.Add(img.Name);
							flats.Remove(img.LongName);
							flats.Add(img.LongName, img);
						}
						
						// Add to preview manager
						previews.AddImage(img);
					}
				}
			}
			
			// The first texture cannot be used, because in the game engine it
			// has index 0 which means "no texture", so remove it from the list.
			textures.Remove(firsttexture);
			if(General.Map.Config.MixTexturesFlats) flats.Remove(firsttexture);
			
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
		

		// BAD! These block while loading the image. That is not
		// what our background loading system is for!
		/*
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
		*/
		
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

						// Add to preview manager
						previews.AddImage(img);
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

		// BAD! These block while loading the image. That is not
		// what our background loading system is for!
		/*
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
		*/
		
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
						
						// Add to preview manager
						previews.AddImage(image);
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

		// BAD! These block while loading the image. That is not
		// what our background loading system is for!
		/*
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
		*/
		
		#endregion
		
		#region ================== Tools

		// This finds the first IWAD resource
		// Returns false when not found
		internal bool FindFirstIWAD(out DataLocation result)
		{
			// Go for all data containers
			foreach(DataReader dr in containers)
			{
				// Container is a WAD file?
				if(dr is WADReader)
				{
					// Check if it is an IWAD
					WADReader wr = dr as WADReader;
					if(wr.IsIWAD)
					{
						// Return location!
						result = wr.Location;
						return true;
					}
				}
			}

			// No IWAD found
			result = new DataLocation();
			return false;
		}

		// This signals the background thread to update the
		// used-in-map status on all textures and flats
		public void UpdateUsedTextures()
		{
			updatedusedtextures = true;
		}
		
		#endregion
	}
}
