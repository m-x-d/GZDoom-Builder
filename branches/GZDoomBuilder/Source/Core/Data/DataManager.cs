
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
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Compilers;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.GZBuilder.GZDoom;
using CodeImp.DoomBuilder.GZBuilder.MD3;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.ZDoom;

#endregion

namespace CodeImp.DoomBuilder.Data
{
	public sealed class DataManager
	{
		#region ================== Constants
		
		public const string INTERNAL_PREFIX = "internal:";
		public const int CLASIC_IMAGE_NAME_LENGTH = 8; //mxd
		
		#endregion

		#region ================== Variables
		
		// Data containers
		private List<DataReader> containers;
		private DataReader currentreader;
		
		// Palette
		private Playpal palette;
		
		// Textures, Flats and Sprites
		private Dictionary<long, ImageData> textures;
		private Dictionary<long, long> texturenamesshorttofull; //mxd
		private List<string> texturenames;
		private Dictionary<long, ImageData> flats;
		private Dictionary<long, long> flatnamesshorttofull; //mxd
		private List<string> flatnames;
		private Dictionary<long, ImageData> sprites;
		private List<MatchingTextureSet> texturesets;
		private List<ResourceTextureSet> resourcetextures;
		private AllTextureSet alltextures;

		//mxd 
		private Dictionary<int, ModelData> modeldefentries; //Thing.Type, Model entry
		private readonly Dictionary<int, DynamicLightData> gldefsentries; //Thing.Type, Light entry
		private MapInfo mapinfo;
		private Dictionary<string, KeyValuePair<int, int>> reverbs; //<name, <arg1, arg2> 
		private Dictionary<long, GlowingFlatData> glowingflats; // Texture name hash, Glowing Flat Data
		private List<string> soundsequences;
		
		// Background loading
		private Queue<ImageData> imageque;
		private Thread backgroundloader;
		private volatile bool updatedusedtextures;
		private bool notifiedbusy;
		
		// Image previews
		private PreviewManager previews;
		
		// Special images
		private ImageData missingtexture3d;
		private ImageData unknowntexture3d;
		private UnknownImage unknownimage; //mxd
		private ImageData hourglass3d;
		private ImageData crosshair;
		private ImageData crosshairbusy;
		private Dictionary<string, ImageData> internalsprites;
		private ImageData whitetexture;

		//mxd. Comment icons
		private ImageData[] commenttextures;
		
		// Used images
		private Dictionary<long, long> usedtextures; //mxd
		private Dictionary<long, long> usedflats; //mxd. Used only when MixTextursFlats is disabled
		
		// Things combined with things created from Decorate
		private DecorateParser decorate;
		private List<ThingCategory> thingcategories;
		private Dictionary<int, ThingTypeInfo> thingtypes;
		
		// Timing
		private float loadstarttime;
		private float loadfinishtime;
		
		// Disposing
		private bool isdisposed;

		#endregion

		#region ================== Properties

		//mxd
		internal Dictionary<int, ModelData> ModeldefEntries { get { return modeldefentries; } }
		internal Dictionary<int, DynamicLightData> GldefsEntries { get { return gldefsentries; } }
		public MapInfo MapInfo { get { return mapinfo; } }
		public Dictionary<string, KeyValuePair<int, int>> Reverbs { get { return reverbs; } }
		public Dictionary<long, GlowingFlatData> GlowingFlats { get { return glowingflats; } }
		public List<string> SoundSequences { get { return soundsequences; } }
		internal List<DataReader> Containers { get { return containers; } } //mxd

		public Playpal Palette { get { return palette; } }
		public PreviewManager Previews { get { return previews; } }
		public ICollection<ImageData> Textures { get { return textures.Values; } }
		public ICollection<ImageData> Flats { get { return flats.Values; } }
		public List<string> TextureNames { get { return texturenames; } }
		public List<string> FlatNames { get { return flatnames; } }
		public bool IsDisposed { get { return isdisposed; } }
		public ImageData MissingTexture3D { get { return missingtexture3d; } }
		public ImageData UnknownTexture3D { get { return unknowntexture3d; } }
		public ImageData Hourglass3D { get { return hourglass3d; } }
		public ImageData Crosshair3D { get { return crosshair; } }
		public ImageData CrosshairBusy3D { get { return crosshairbusy; } }
		public ImageData WhiteTexture { get { return whitetexture; } }
		public ImageData[] CommentTextures { get { return commenttextures; } } //mxd
		public List<ThingCategory> ThingCategories { get { return thingcategories; } }
		public ICollection<ThingTypeInfo> ThingTypes { get { return thingtypes.Values; } }
		public DecorateParser Decorate { get { return decorate; } }
		internal ICollection<MatchingTextureSet> TextureSets { get { return texturesets; } }
		internal ICollection<ResourceTextureSet> ResourceTextureSets { get { return resourcetextures; } }
		internal AllTextureSet AllTextureSet { get { return alltextures; } }
		
		public bool IsLoading
		{
			get
			{
				if(imageque != null)
					return (backgroundloader != null) && backgroundloader.IsAlive && ((imageque.Count > 0) || previews.IsLoading);
				return false;
			}
		}
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		internal DataManager()
		{
			// We have no destructor
			GC.SuppressFinalize(this);

			//mxd.
			modeldefentries = new Dictionary<int, ModelData>();
			gldefsentries = new Dictionary<int, DynamicLightData>();
			reverbs = new Dictionary<string, KeyValuePair<int, int>>();
			glowingflats = new Dictionary<long, GlowingFlatData>();
			soundsequences = new List<string>();

			// Load special images
			missingtexture3d = new ResourceImage("CodeImp.DoomBuilder.Resources.MissingTexture3D.png");
			missingtexture3d.LoadImage();
			unknowntexture3d = new ResourceImage("CodeImp.DoomBuilder.Resources.UnknownTexture3D.png");
			unknowntexture3d.LoadImage();
			hourglass3d = new ResourceImage("CodeImp.DoomBuilder.Resources.Hourglass3D.png");
			hourglass3d.LoadImage();
			crosshair = new ResourceImage("CodeImp.DoomBuilder.Resources.Crosshair.png");
			crosshair.LoadImage();
			crosshairbusy = new ResourceImage("CodeImp.DoomBuilder.Resources.CrosshairBusy.png");
			crosshairbusy.LoadImage();
			whitetexture = new ResourceImage("CodeImp.DoomBuilder.Resources.White.png");
			whitetexture.UseColorCorrection = false;
			whitetexture.LoadImage();
			whitetexture.CreateTexture();
			unknownimage = new UnknownImage(Properties.Resources.UnknownImage); //mxd. There should be only one!

			//mxd. Create comment icons
			commenttextures = new ImageData[]
			                  {
				                  new ResourceImage("CodeImp.DoomBuilder.Resources.CommentRegular.png"),
								  new ResourceImage("CodeImp.DoomBuilder.Resources.CommentInfo.png"),
								  new ResourceImage("CodeImp.DoomBuilder.Resources.CommentQuestion.png"),
								  new ResourceImage("CodeImp.DoomBuilder.Resources.CommentProblem.png"),
								  new ResourceImage("CodeImp.DoomBuilder.Resources.CommentSmile.png"),
			                  };

			//mxd. Load comment icons
			foreach (ImageData data in commenttextures)
			{
				data.LoadImage();
				data.CreateTexture();
			}
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
				unknowntexture3d.Dispose();
				unknowntexture3d = null;
				hourglass3d.Dispose();
				hourglass3d = null;
				crosshair.Dispose();
				crosshair = null;
				crosshairbusy.Dispose();
				crosshairbusy = null;
				whitetexture.Dispose();
				whitetexture = null;
				unknownimage.Dispose(); //mxd
				unknownimage = null; //mxd
				foreach(ImageData i in commenttextures) i.Dispose(); //mxd
				commenttextures = null;
				
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
			int texcount, flatcount, spritecount, thingcount, colormapcount;
			Dictionary<long, ImageData> texturesonly = new Dictionary<long, ImageData>();
			Dictionary<long, ImageData> colormapsonly = new Dictionary<long, ImageData>();
			Dictionary<long, ImageData> flatsonly = new Dictionary<long, ImageData>();
			DataReader c;
			
			// Create collections
			containers = new List<DataReader>();
			textures = new Dictionary<long, ImageData>();
			flats = new Dictionary<long, ImageData>();
			sprites = new Dictionary<long, ImageData>();
			texturenames = new List<string>();
			flatnames = new List<string>();
			texturenamesshorttofull = new Dictionary<long, long>(); //mxd
			flatnamesshorttofull = new Dictionary<long, long>(); //mxd
			imageque = new Queue<ImageData>();
			previews = new PreviewManager();
			texturesets = new List<MatchingTextureSet>();
			usedtextures = new Dictionary<long, long>(); //mxd
			usedflats = new Dictionary<long, long>(); //mxd
			internalsprites = new Dictionary<string, ImageData>(StringComparer.Ordinal);
			thingcategories = General.Map.Config.GetThingCategories();
			thingtypes = General.Map.Config.GetThingTypes();
			
			// Load texture sets
			foreach(DefinedTextureSet ts in General.Map.ConfigSettings.TextureSets)
				texturesets.Add(new MatchingTextureSet(ts));
			
			// Sort the texture sets
			texturesets.Sort();
			
			// Special textures sets
			alltextures = new AllTextureSet();
			resourcetextures = new List<ResourceTextureSet>();
			
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
				catch(Exception e)
				{
					// Unable to load resource
					General.ErrorLogger.Add(ErrorType.Error, "Unable to load resources from location \"" + dl.location + "\". Please make sure the location is accessible and not in use by another program. The resources will now be loaded with this location excluded. You may reload the resources to try again.\n" + e.GetType().Name + " when creating data reader: " + e.Message);
					General.WriteLogLine(e.StackTrace);
					continue;
				}	

				// Add container
				if(c != null)
				{
					containers.Add(c);
					resourcetextures.Add(c.TextureSet);
				}
			}
			
			// Load stuff
			LoadPalette();
			texcount = LoadTextures(texturesonly, texturenamesshorttofull);
			flatcount = LoadFlats(flatsonly, flatnamesshorttofull);
			colormapcount = LoadColormaps(colormapsonly);
			LoadSprites();

			//mxd. Load MAPINFO. Should happen before parisng DECORATE
			Dictionary<int, string> spawnnums;
			Dictionary<int, string> doomednums;
			LoadMapInfo(out spawnnums, out doomednums);

			thingcount = LoadDecorateThings(spawnnums, doomednums);
			spritecount = LoadThingSprites();
			LoadInternalSprites();

			//mxd. Load more stuff
			LoadReverbs();
			LoadSndSeq();
			LoadVoxels();
			Dictionary<string, List<int>> actorsbyclass = CreateActorsByClassList();
			LoadModeldefs(actorsbyclass);
			foreach (Thing t in General.Map.Map.Things) t.UpdateCache();
			General.MainWindow.DisplayReady();
			
			// Process colormaps (we just put them in as textures)
			foreach(KeyValuePair<long, ImageData> t in colormapsonly)
			{
				textures.Add(t.Key, t.Value);
				texturenames.Add(t.Value.Name);
			}
			
			// Process textures
			foreach(KeyValuePair<long, ImageData> t in texturesonly)
			{
				if(!textures.ContainsKey(t.Key))
				{
					textures.Add(t.Key, t.Value);

					//mxd. Add both short and long names?
					if(t.Value.HasLongName) texturenames.Add(t.Value.ShortName);
					texturenames.Add(t.Value.Name);
				}
			}

			// Process flats
			foreach(KeyValuePair<long, ImageData> f in flatsonly) 
			{
				flats.Add(f.Key, f.Value);

				//mxd. Add both short and long names?
				if(f.Value.HasLongName) flatnames.Add(f.Value.ShortName);
				flatnames.Add(f.Value.Name);
			}

			// Mixed textures and flats?
			if(General.Map.Config.MixTexturesFlats) 
			{
				// Add textures to flats
				foreach(KeyValuePair<long, ImageData> t in texturesonly) 
				{
					if(!flats.ContainsKey(t.Key)) 
					{
						flats.Add(t.Key, t.Value);

						//mxd. Add both short and long names?
						if(t.Value.HasLongName) flatnames.Add(t.Value.ShortName);
						flatnames.Add(t.Value.Name);
					}
					else if(t.Value is HighResImage || t.Value is SimpleTextureImage) //mxd. Textures defined in TEXTURES or placed between TX_START and TX_END markers override "regular" flats in ZDoom
					{
						//TODO: check this!
						flats[t.Key] = t.Value;
					}
				}

				//mxd
				foreach(KeyValuePair<long, long> t in texturenamesshorttofull)
					if(!flatnamesshorttofull.ContainsKey(t.Key)) flatnamesshorttofull.Add(t.Key, t.Value);

				// Add flats to textures
				foreach(KeyValuePair<long, ImageData> f in flatsonly) 
				{
					if(!textures.ContainsKey(f.Key)) 
					{
						textures.Add(f.Key, f.Value);

						//mxd. Add both short and long names?
						if(f.Value.HasLongName) texturenames.Add(f.Value.ShortName);
						texturenames.Add(f.Value.Name);
					}
				}

				//mxd
				foreach(KeyValuePair<long, long> t in flatnamesshorttofull)
					if(!texturenamesshorttofull.ContainsKey(t.Key)) texturenamesshorttofull.Add(t.Key, t.Value);

				// Do the same on the data readers
				foreach(DataReader dr in containers)
					dr.TextureSet.MixTexturesAndFlats();
			}

			//mxd. Should be done after loading textures...
			LoadGldefs(actorsbyclass);
			
			// Sort names
			texturenames.Sort();
			flatnames.Sort();

			// Sort things
			foreach(ThingCategory tc in thingcategories) tc.SortIfNeeded();

			// Update the used textures
			General.Map.Data.UpdateUsedTextures();
			
			// Add texture names to texture sets
			foreach(KeyValuePair<long, ImageData> img in textures)
			{
				// Add to all sets
				foreach(MatchingTextureSet ms in texturesets)
					ms.AddTexture(img.Value);

				// Add to all
				alltextures.AddTexture(img.Value);
			}
			
			// Add flat names to texture sets
			foreach(KeyValuePair<long, ImageData> img in flats)
			{
				// Add to all sets
				foreach(MatchingTextureSet ms in texturesets)
					ms.AddFlat(img.Value);
				
				// Add to all
				alltextures.AddFlat(img.Value);
			}
			
			// Start background loading
			StartBackgroundLoader();
			
			// Output info
			General.WriteLogLine("Loaded " + texcount + " textures, " + flatcount + " flats, " + 
				colormapcount + " colormaps, " + spritecount + " sprites, " + 
				thingcount + " decorate things, " + modeldefentries.Count + " model/voxel deinitions, " + 
				gldefsentries.Count + " dynamic light definitions, " + 
				glowingflats.Count + " glowing flat definitions, " + reverbs.Count + " sound environment definitions");
		}
		
		// This unloads all data
		private void Unload()
		{
			// Stop background loader
			StopBackgroundLoader();
			
			// Dispose preview manager
			previews.Dispose();
			previews = null;
			
			// Dispose decorate
			decorate.Dispose();
			
			// Dispose resources
			foreach(KeyValuePair<long, ImageData> i in textures) i.Value.Dispose();
			foreach(KeyValuePair<long, ImageData> i in flats) i.Value.Dispose();
			foreach(KeyValuePair<long, ImageData> i in sprites) i.Value.Dispose();
			palette = null;

			//mxd. Dispose models
			foreach(KeyValuePair<int, ModelData> i in modeldefentries) i.Value.Dispose();
		
			// Dispose containers
			foreach(DataReader c in containers) c.Dispose();
			containers.Clear();
			
			// Trash collections
			decorate = null;
			containers = null;
			textures = null;
			flats = null;
			sprites = null;
			modeldefentries = null;//mxd
			texturenames = null;
			flatnames = null;
			imageque = null;
			internalsprites = null;
			mapinfo = null; //mxd
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
				catch(Exception e)
				{
					// Unable to load resource
					General.ErrorLogger.Add(ErrorType.Error, "Unable to load resources from location \"" + d.Location.location + "\". Please make sure the location is accessible and not in use by another program. The resources will now be loaded with this location excluded. You may reload the resources to try again.\n" + e.GetType().Name + " when resuming data reader: " + e.Message + ")");
					General.WriteLogLine(e.StackTrace);
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
			// Timing
			loadstarttime = Clock.CurrentTime;
			loadfinishtime = 0;
			
			// If a loader is already running, stop it first
			if(backgroundloader != null) StopBackgroundLoader();

			// Start a low priority thread to load images in background
			General.WriteLogLine("Starting background resource loading...");
			backgroundloader = new Thread(BackgroundLoad);
			backgroundloader.Name = "Background Loader";
			backgroundloader.Priority = ThreadPriority.Lowest;
			backgroundloader.IsBackground = true;
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
				notifiedbusy = false;
				backgroundloader = null;
				General.SendMessage(General.MainWindow.Handle, (int)MainForm.ThreadMessages.UpdateStatus, 0, 0);
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
					if(updatedusedtextures) BackgroundUpdateUsedTextures();
					
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
						else if(!image.IsReferenced && image.AllowUnload && (image.ImageState != ImageLoadState.None))
						{
							// Still unreferenced?
							image.UnloadImage();
						}
					}
					
					// Doing something?
					if(image != null)
					{
						// Wait a bit and update icon
						if(!notifiedbusy)
						{
							notifiedbusy = true;
							General.SendMessage(General.MainWindow.Handle, (int)MainForm.ThreadMessages.UpdateStatus, 0, 0);
						}
						Thread.Sleep(0);
					}
					else
					{
						// Process previews only when we don't have images to process
						// because these are lower priority than the actual images
						if(previews.BackgroundLoad())
						{
							// Wait a bit and update icon
							if(!notifiedbusy)
							{
								notifiedbusy = true;
								General.SendMessage(General.MainWindow.Handle, (int)MainForm.ThreadMessages.UpdateStatus, 0, 0);
							}
							Thread.Sleep(0);
						}
						else
						{
							if(notifiedbusy)
							{
								notifiedbusy = false;
								General.SendMessage(General.MainWindow.Handle, (int)MainForm.ThreadMessages.UpdateStatus, 0, 0);
							}
							
							// Timing
							if(loadfinishtime == 0)
							{
								//mxd. Release PK3 files
								foreach (DataReader reader in containers)
								{
									if (reader is PK3Reader) (reader as PK3Reader).BathMode = false;
								}
								
								loadfinishtime = Clock.CurrentTime;
								float deltatimesec = (loadfinishtime - loadstarttime) / 1000.0f;
								General.WriteLogLine("Resources loading took " + deltatimesec.ToString("########0.00") + " seconds");
							}
							
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
			if((img.ImageState == ImageLoadState.Ready) && !img.IsReferenced && img.AllowUnload)
			{
				// Add for unloading
				img.ImageState = ImageLoadState.Unloading;
				lock(imageque) { imageque.Enqueue(img); }
			}
			
			// Update icon
			General.SendMessage(General.MainWindow.Handle, (int)MainForm.ThreadMessages.UpdateStatus, 0, 0);
		}

		//mxd. This loads a model
		internal bool ProcessModel(int type) 
		{
			if(modeldefentries[type].LoadState != ModelLoadState.None) return true;

			//create models
			ModelReader.Load(modeldefentries[type], containers, General.Map.Graphics.Device);

			if(modeldefentries[type].Model != null) 
			{
				modeldefentries[type].LoadState = ModelLoadState.Ready;
				return true;
			}

			modeldefentries.Remove(type);
			return false;
		}

		// This updates the used-in-map status on all textures and flats
		private void BackgroundUpdateUsedTextures()
		{
			if(General.Map.Config.MixTexturesFlats)
			{
				lock(usedtextures)
				{
					// Set used on all textures
					foreach(KeyValuePair<long, ImageData> i in textures)
					{
						i.Value.SetUsedInMap(usedtextures.ContainsKey(i.Key));
						if(i.Value.IsImageLoaded != i.Value.IsReferenced) ProcessImage(i.Value);
					}

					// Set used on all flats
					foreach(KeyValuePair<long, ImageData> i in flats)
					{
						i.Value.SetUsedInMap(usedtextures.ContainsKey(i.Key));
						if(i.Value.IsImageLoaded != i.Value.IsReferenced) ProcessImage(i.Value);
					}

					// Done
					updatedusedtextures = false;
				}
			}
			//mxd. Use separate collections
			else
			{
				lock(usedtextures)
				{
					// Set used on all textures
					foreach(KeyValuePair<long, ImageData> i in textures)
					{
						i.Value.SetUsedInMap(usedtextures.ContainsKey(i.Key));
						if(i.Value.IsImageLoaded != i.Value.IsReferenced) ProcessImage(i.Value);
					}
				}

				lock(usedflats)
				{
					// Set used on all flats
					foreach(KeyValuePair<long, ImageData> i in flats)
					{
						i.Value.SetUsedInMap(usedflats.ContainsKey(i.Key));
						if(i.Value.IsImageLoaded != i.Value.IsReferenced) ProcessImage(i.Value);
					}
				}
				
				// Done
				updatedusedtextures = false;
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
				General.ErrorLogger.Add(ErrorType.Warning, "None of the loaded resources define a color palette. Did you forget to configure an IWAD for this game configuration?");
				palette = new Playpal();
			}
		}

		#endregion

		#region ================== Colormaps

		// This loads the colormaps
		private int LoadColormaps(Dictionary<long, ImageData> list)
		{
			ICollection<ImageData> images;
			int counter = 0;

			// Go for all opened containers
			foreach(DataReader dr in containers)
			{
				// Load colormaps
				images = dr.LoadColormaps();
				if(images != null)
				{
					// Go for all colormaps
					foreach(ImageData img in images)
					{
						// Add or replace in flats list
						list.Remove(img.LongName);
						list.Add(img.LongName, img);
						counter++;

						// Add to preview manager
						previews.AddImage(img);
					}
				}
			}

			// Output info
			return counter;
		}

		// This returns a specific colormap stream
		internal Stream GetColormapData(string pname)
		{
			Stream colormap;

			// Go for all opened containers
			for(int i = containers.Count - 1; i >= 0; i--)
			{
				// This contain provides this flat?
				colormap = containers[i].GetColormapData(pname);
				if(colormap != null) return colormap;
			}

			// No such patch found
			return null;
		}

		#endregion

		#region ================== Textures

		// This loads the textures
		private int LoadTextures(Dictionary<long, ImageData> list, Dictionary<long, long> nametranslation)
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
						list.Remove(img.LongName);
						list.Add(img.LongName, img);
						counter++;

						//mxd. Also add as short name when texture name is longer than 8 chars
						// Or remove when a wad image with short name overrides previously added 
						// resource image with long name
						if(img.HasLongName) 
						{
							long longshortname = Lump.MakeLongName(Path.GetFileNameWithoutExtension(img.Name), false);
							nametranslation.Remove(longshortname);
							nametranslation.Add(longshortname, img.LongName);
						} 
						else if (img is TextureImage && nametranslation.ContainsKey(img.LongName))
						{
							nametranslation.Remove(img.LongName);
						}
						
						// Add to preview manager
						previews.AddImage(img);
					}
				}
			}
			
			// Output info
			return counter;
		}
		
		// This returns a specific patch stream
		internal Stream GetPatchData(string pname, bool longname)
		{
			Stream patch;

			// Go for all opened containers
			for(int i = containers.Count - 1; i > -1; i--)
			{
				// This contain provides this patch?
				patch = containers[i].GetPatchData(pname, longname);
				if(patch != null) return patch;
			}

			// No such patch found
			return null;
		}

		// This returns a specific texture stream
		internal Stream GetTextureData(string pname, bool longname)
		{
			Stream patch;

			// Go for all opened containers
			for(int i = containers.Count - 1; i >= 0; i--)
			{
				// This contain provides this patch?
				patch = containers[i].GetTextureData(pname, longname);
				if(patch != null) return patch;
			}

			// No such patch found
			return null;
		}
		
		// This checks if a given texture is known
		public bool GetTextureExists(string name)
		{
			return GetTextureExists(Lump.MakeLongName(name)); //mxd
		}
		
		// This checks if a given texture is known
		public bool GetTextureExists(long longname)
		{
			return textures.ContainsKey(longname) || texturenamesshorttofull.ContainsKey(longname);
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
			if(textures.ContainsKey(longname) && textures[longname] is HighResImage) return textures[longname]; //TEXTURES textures should still override regular ones...
			if(texturenamesshorttofull.ContainsKey(longname)) return textures[texturenamesshorttofull[longname]]; //mxd
			if(textures.ContainsKey(longname)) return textures[longname];

			// Return null image
			return unknownimage; //mxd
		}

		//mxd
		public string GetFullTextureName(string name)
		{
			if (Path.GetFileNameWithoutExtension(name) == name && name.Length > CLASIC_IMAGE_NAME_LENGTH) 
				name = name.Substring(0, CLASIC_IMAGE_NAME_LENGTH);
			long hash = MurmurHash2.Hash(name.Trim().ToUpperInvariant());

			if(textures.ContainsKey(hash) && textures[hash] is HighResImage) return textures[hash].Name; //TEXTURES textures should still override regular ones...
			if(texturenamesshorttofull.ContainsKey(hash)) return textures[texturenamesshorttofull[hash]].Name;
			if(textures.ContainsKey(hash)) return textures[hash].Name;
			return name;
		}

		//mxd
		internal long GetFullLongTextureName(long hash)
		{
			if(textures.ContainsKey(hash) && textures[hash] is HighResImage) return hash; //TEXTURES textures should still override regular ones...
			return (General.Map.Config.UseLongTextureNames && texturenamesshorttofull.ContainsKey(hash) ? texturenamesshorttofull[hash] : hash);
		}
		
		#endregion

		#region ================== Flats

		// This loads the flats
		private int LoadFlats(Dictionary<long, ImageData> list, Dictionary<long, long> nametranslation)
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
						list.Remove(img.LongName);
						list.Add(img.LongName, img);
						counter++;

						//mxd. Also add as short name when texture name is longer than 8 chars
						// Or remove when a wad image with short name overrides previously added 
						// resource image with long name
						if (img.HasLongName)
						{
							long longshortname = Lump.MakeLongName(Path.GetFileNameWithoutExtension(img.Name), false);
							nametranslation.Remove(longshortname);
							nametranslation.Add(longshortname, img.LongName);
						} 
						else if(img is FlatImage && nametranslation.ContainsKey(img.LongName)) 
						{
							nametranslation.Remove(img.LongName);
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
		internal Stream GetFlatData(string pname, bool longname)
		{
			Stream flat;

			// Go for all opened containers
			for(int i = containers.Count - 1; i >= 0; i--)
			{
				// This contain provides this flat?
				flat = containers[i].GetFlatData(pname, longname);
				if(flat != null) return flat;
			}

			// No such patch found
			return null;
		}

		// This checks if a flat is known
		public bool GetFlatExists(string name)
		{
			return GetFlatExists(Lump.MakeLongName(name)); //mxd
		}

		// This checks if a flat is known
		public bool GetFlatExists(long longname)
		{
			return flats.ContainsKey(longname) || flatnamesshorttofull.ContainsKey(longname);
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
			if(flats.ContainsKey(longname) && flats[longname] is HighResImage) return flats[longname]; //TEXTURES flats should still override regular ones...
			if(flatnamesshorttofull.ContainsKey(longname)) return flats[flatnamesshorttofull[longname]]; //mxd
			if(flats.ContainsKey(longname)) return flats[longname];
			
			// Return null image
			return unknownimage; //mxd
		}

		// This returns an image by long and doesn't check if it exists
		/*public ImageData GetFlatImageKnown(long longname)
		{
			// Return flat
			return flatnamesshorttofull.ContainsKey(longname) ? flats[flatnamesshorttofull[longname]] : flats[longname]; //mxd
		}*/

		//mxd. Gets full flat name by short flat name
		public string GetFullFlatName(string name)
		{
			if(Path.GetFileNameWithoutExtension(name) == name && name.Length > CLASIC_IMAGE_NAME_LENGTH)
				name = name.Substring(0, CLASIC_IMAGE_NAME_LENGTH);
			long hash = MurmurHash2.Hash(name.ToUpperInvariant());

			if(flats.ContainsKey(hash) && flats[hash] is HighResImage) return flats[hash].Name; //TEXTURES flats should still override regular ones...
			if(flatnamesshorttofull.ContainsKey(hash)) return flats[flatnamesshorttofull[hash]].Name;
			if(flats.ContainsKey(hash)) return flats[hash].Name;
			return name;
		}

		//mxd
		internal long GetFullLongFlatName(long hash)
		{
			if(flats.ContainsKey(hash) && flats[hash] is HighResImage) return hash; //TEXTURES flats should still override regular ones...
			return (General.Map.Config.UseLongTextureNames && flatnamesshorttofull.ContainsKey(hash) ? flatnamesshorttofull[hash] : hash);
		}
		
		#endregion

		#region ================== Sprites

		// This loads the hard defined sprites (not all the lumps, we do that on a need-to-know basis, see LoadThingSprites)
		private int LoadSprites()
		{
			ICollection<ImageData> images;
			int counter = 0;
			
			// Load all defined sprites. Note that we do not use all sprites,
			// so we don't add them for previews just yet.
			foreach(DataReader dr in containers)
			{
				// Load sprites
				images = dr.LoadSprites();
				if(images != null)
				{
					// Add or replace in sprites list
					foreach(ImageData img in images)
					{
						sprites[img.LongName] = img;
						counter++;
					}
				}
			}
			
			// Output info
			return counter;
		}
		
		// This loads the sprites that we really need for things
		private int LoadThingSprites()
		{
			// Go for all things
			foreach(ThingTypeInfo ti in General.Map.Data.ThingTypes)
			{
				// Valid sprite name?
				if(ti.Sprite.Length == 0 || ti.Sprite.Length > CLASIC_IMAGE_NAME_LENGTH) continue; //mxd
					
				ImageData image = null;

				// Sprite not in our collection yet?
				if(!sprites.ContainsKey(ti.SpriteLongName)) 
				{
					// Find sprite data
					Stream spritedata = GetSpriteData(ti.Sprite);
					if(spritedata != null) 
					{
						// Make new sprite image
						image = new SpriteImage(ti.Sprite);

						// Add to collection
						sprites.Add(ti.SpriteLongName, image);
					} 
					else //mxd
					{
						General.ErrorLogger.Add(ErrorType.Error, "Missing sprite lump '" + ti.Sprite + "'. Forgot to include required resources?");
					}
				} 
				else 
				{
					image = sprites[ti.SpriteLongName];
				}

				// Add to preview manager
				if(image != null) previews.AddImage(image);
			}
			
			// Output info
			return sprites.Count;
		}
		
		// This returns a specific patch stream
		internal Stream GetSpriteData(string pname)
		{
			if(!string.IsNullOrEmpty(pname))
			{
				// Go for all opened containers
				for(int i = containers.Count - 1; i >= 0; i--)
				{
					// This contain provides this patch?
					Stream spritedata = containers[i].GetSpriteData(pname);
					if(spritedata != null) return spritedata;
				}
			}
			
			// No such patch found
			return null;
		}

		// This tests if a given sprite can be found
		internal bool GetSpriteExists(string pname)
		{
			if(!string.IsNullOrEmpty(pname))
			{
				long longname = Lump.MakeLongName(pname);
				if(sprites.ContainsKey(longname)) return true;
				
				// Go for all opened containers
				for(int i = containers.Count - 1; i >= 0; i--)
				{
					// This contain provides this patch?
					if(containers[i].GetSpriteExists(pname)) return true;
				}
			}
			
			// No such patch found
			return false;
		}
		
		// This loads the internal sprites
		private void LoadInternalSprites()
		{
			// Add sprite icon files from directory
			string[] files = Directory.GetFiles(General.SpritesPath, "*.png", SearchOption.TopDirectoryOnly);
			foreach(string spritefile in files)
			{
				ImageData img = new FileImage(Path.GetFileNameWithoutExtension(spritefile).ToLowerInvariant(), spritefile);
				img.LoadImage();
				img.AllowUnload = false;
				internalsprites.Add(img.Name, img);
			}
			
			// Add some internal resources
			if(!internalsprites.ContainsKey("nothing"))
			{
				ImageData img = new ResourceImage("CodeImp.DoomBuilder.Resources.Nothing.png");
				img.LoadImage();
				img.AllowUnload = false;
				internalsprites.Add("nothing", img);
			}
			
			if(!internalsprites.ContainsKey("unknownthing"))
			{
				ImageData img = new ResourceImage("CodeImp.DoomBuilder.Resources.UnknownThing.png");
				img.LoadImage();
				img.AllowUnload = false;
				internalsprites.Add("unknownthing", img);
			}

			//mxd
			if(!internalsprites.ContainsKey("missingthing")) 
			{
				ImageData img = new ResourceImage("CodeImp.DoomBuilder.Resources.MissingThing.png");
				img.LoadImage();
				img.AllowUnload = false;
				internalsprites.Add("missingthing", img);
			}
		}
		
		// This returns an image by name
		public ImageData GetSpriteImage(string name)
		{
			// Is this referring to an internal sprite image?
			if((name.Length > INTERNAL_PREFIX.Length) && name.ToLowerInvariant().StartsWith(INTERNAL_PREFIX))
			{
				// Get the internal sprite
				string internalname = name.Substring(INTERNAL_PREFIX.Length).ToLowerInvariant();
				if(internalsprites.ContainsKey(internalname))
					return internalsprites[internalname];

				return internalsprites["unknownthing"]; //mxd
			}
			else
			{
				// Get the long name
				long longname = Lump.MakeLongName(name);

				// Sprite already loaded?
				if(sprites.ContainsKey(longname))
				{
					// Return exiting sprite
					return sprites[longname];
				}
				else
				{
					Stream spritedata = null;
					
					// Go for all opened containers
					for(int i = containers.Count - 1; i >= 0; i--)
					{
						// This container provides this sprite?
						spritedata = containers[i].GetSpriteData(name);
						if(spritedata != null) break;
					}
					
					// Found anything?
					if(spritedata != null)
					{
						// Make new sprite image
						SpriteImage image = new SpriteImage(name);

						// Add to collection
						sprites.Add(longname, image);

						// Return result
						return image;
					}
					else //mxd
					{
						ImageData img = string.IsNullOrEmpty(name) ? internalsprites["unknownthing"] : internalsprites["missingthing"];
						
						// Add to collection
						sprites.Add(longname, img);
					
						// Return image
						return img; 
					}
				}
			}
		}
		
		#endregion

		#region ================== Things
		
		// This loads the things from Decorate
		private int LoadDecorateThings(Dictionary<int, string> spawnnumsoverride, Dictionary<int, string> doomednumsoverride)
		{
			int counter = 0;
			char[] catsplitter = new[] {Path.AltDirectorySeparatorChar}; //mxd
			
			// Create new parser
			decorate = new DecorateParser();
			decorate.OnInclude = LoadDecorateFromLocation;
			
			// Only load these when the game configuration supports the use of decorate
			if(!string.IsNullOrEmpty(General.Map.Config.DecorateGames))
			{
				// Go for all opened containers
				foreach(DataReader dr in containers)
				{
					// Load Decorate info cumulatively (the last Decorate is added to the previous)
					// I'm not sure if this is the right thing to do though.
					currentreader = dr;
					Dictionary<string, Stream> decostreams = dr.GetDecorateData("DECORATE");
					foreach(KeyValuePair<string, Stream> group in decostreams)
					{
						// Parse the data
						group.Value.Seek(0, SeekOrigin.Begin);
						decorate.Parse(group.Value, group.Key, true);
						
						// Check for errors
						if(decorate.HasError)
						{
							General.ErrorLogger.Add(ErrorType.Error, "DECORATE error in '" + decorate.ErrorSource
								+ (decorate.ErrorLine != CompilerError.NO_LINE_NUMBER ? "', line " + decorate.ErrorLine : "'") + ". " + decorate.ErrorDescription + ".");
							break;
						}
					}
				}
				
				currentreader = null;
				
				if(!decorate.HasError)
				{
					// Step 1. Go for all actors in the decorate to make things or update things
					foreach(ActorStructure actor in decorate.Actors)
					{
						// Check if we want to add this actor
						if(actor.DoomEdNum > 0)
						{
							string catname = ZDTextParser.StripQuotes(actor.GetPropertyAllValues("$category"));
							string[] catnames; //mxd
							if(string.IsNullOrEmpty(catname.Trim()))
								catnames = new[] { "decorate" };
							else
								catnames = catname.Split(catsplitter, StringSplitOptions.RemoveEmptyEntries); //mxd
							
							// Check if we can find this thing in our existing collection
							if(thingtypes.ContainsKey(actor.DoomEdNum))
							{
								// Update the thing
								thingtypes[actor.DoomEdNum].ModifyByDecorateActor(actor);
							}
							else
							{
								// Find the category to put the actor in
								ThingCategory cat = GetThingCategory(null, thingcategories, catnames); //mxd
								
								// Add new thing
								ThingTypeInfo t = new ThingTypeInfo(cat, actor);
								cat.AddThing(t);
								thingtypes.Add(t.Index, t);
							}
							
							// Count
							counter++;
						}
					}

					//mxd. Step 2. Apply DoomEdNum MAPINFO overrides, remove actors disabled in MAPINFO
					if(doomednumsoverride.Count > 0) 
					{
						List<int> toremove = new List<int>();
						Dictionary<string, ThingTypeInfo> thingtypesbyclass = new Dictionary<string, ThingTypeInfo>();
						foreach (KeyValuePair<int, ThingTypeInfo> group in thingtypes)
						{
							if(string.IsNullOrEmpty(group.Value.ClassName)) continue;
							thingtypesbyclass[group.Value.ClassName.ToLowerInvariant()] = group.Value;
						}

						foreach(KeyValuePair<int, string> group in doomednumsoverride) 
						{
							// Remove thing from the list?
							if(group.Value == "none") 
							{
								toremove.Add(group.Key);
								continue;
							}

							// Skip if already added.
							if(thingtypes.ContainsKey(group.Key) && thingtypes[group.Key].ClassName.ToLowerInvariant() == group.Value) 
							{
								continue;
							}

							// Try to find the actor...
							ActorStructure actor = null;

							//... in ActorsByClass
							if(decorate.ActorsByClass.ContainsKey(group.Value))
							{
								actor = decorate.ActorsByClass[group.Value];
							}
							// Try finding in ArchivedActors
							else if(decorate.AllActorsByClass.ContainsKey(group.Value))
							{
								actor = decorate.AllActorsByClass[group.Value];
							}

							if(actor != null)
							{
								// Find the category to put the actor in
								string catname = ZDTextParser.StripQuotes(actor.GetPropertyAllValues("$category"));
								string[] catnames; //mxd
								if(string.IsNullOrEmpty(catname.Trim()))
									catnames = new[] { "decorate" };
								else
									catnames = catname.Split(catsplitter, StringSplitOptions.RemoveEmptyEntries); //mxd

								ThingCategory cat = GetThingCategory(null, thingcategories, catnames); //mxd

								// Add a new ThingTypeInfo, replacing already existing one if necessary
								ThingTypeInfo info = new ThingTypeInfo(cat, actor, group.Key);
								thingtypes[group.Key] = info;
								cat.AddThing(info);
							}
							// Check thingtypes...
							else if(thingtypesbyclass.ContainsKey(group.Value))
							{
								ThingTypeInfo t = new ThingTypeInfo(group.Key, thingtypesbyclass[group.Value]);

								// Add new thing, replacing already existing one if necessary
								t.Category.AddThing(t);
								thingtypes[group.Key] = t;
							}
							// Loudly give up...
							else
							{
								General.ErrorLogger.Add(ErrorType.Warning, "Failed to apply MAPINFO DoomEdNum override '" + group.Key + " = " + group.Value + ": failed to find corresponding actor class...");
							}
						}

						// Remove items
						foreach(int id in toremove) 
						{
							if(thingtypes.ContainsKey(id))
							{
								thingtypes[id].Category.RemoveThing(thingtypes[id]);
								thingtypes.Remove(id);
							}
						}
					}

					//mxd. Step 3. Gather DECORATE SpawnIDs
					Dictionary<int, EnumItem> configspawnnums = new Dictionary<int, EnumItem>();

					// Update or create the main enums list
					if(General.Map.Config.Enums.ContainsKey("spawnthing")) 
					{
						foreach(EnumItem item in General.Map.Config.Enums["spawnthing"])
							configspawnnums.Add(item.GetIntValue(), item);
					}

					bool spawnidschanged = false;
					if(!decorate.HasError)
					{
						foreach(ActorStructure actor in decorate.Actors)
						{
							int spawnid = actor.GetPropertyValueInt("spawnid", 0);
							if(spawnid != 0)
							{
								configspawnnums[spawnid] = new EnumItem(spawnid.ToString(), (actor.HasPropertyWithValue("$title") ? actor.GetPropertyAllValues("$title") : actor.ClassName));
								spawnidschanged = true;
							}
						}
					}

					//mxd. Step 4. Update SpawnNums using MAPINFO overrides
					if(spawnnumsoverride.Count > 0)
					{
						// Modify by MAPINFO data
						foreach(KeyValuePair<int, string> group in spawnnumsoverride) 
						{
							configspawnnums[group.Key] = new EnumItem(group.Key.ToString(), (thingtypes.ContainsKey(group.Key) ? thingtypes[group.Key].Title : group.Value));
						}

						spawnidschanged = true;
					}

					if(spawnidschanged)
					{
						// Update the main collection
						EnumList newenums = new EnumList();
						newenums.AddRange(configspawnnums.Values);
						newenums.Sort();
						General.Map.Config.Enums["spawnthing"] = newenums;

						// Update all ArgumentInfos...
						foreach (ThingTypeInfo info in thingtypes.Values)
						{
							foreach(ArgumentInfo ai in info.Args) 
								if(ai.Enum.Name == "spawnthing") ai.Enum = newenums;
						}

						foreach (LinedefActionInfo info in General.Map.Config.LinedefActions.Values)
						{
							foreach(ArgumentInfo ai in info.Args)
								if(ai.Enum.Name == "spawnthing") ai.Enum = newenums;
						}
					}
				}
			}
			
			// Output info
			return counter;
		}

		//mxd
		private static ThingCategory GetThingCategory(ThingCategory parent, List<ThingCategory> categories, string[] catnames) 
		{
			// Find the category to put the actor in
			ThingCategory cat = null;
			string catname = catnames[0].ToLowerInvariant().Trim();
			if(string.IsNullOrEmpty(catname)) catname = "decorate";

			// First search by Title...
			foreach(ThingCategory c in categories) 
			{
				if(c.Title.ToLowerInvariant() == catname) cat = c;
			}

			// Make full name
			if(parent != null) catname = parent.Name.ToLowerInvariant() + "." + catname;

			//...then - by Name
			if(cat == null) 
			{
				foreach(ThingCategory c in categories) 
				{
					if(c.Name.ToLowerInvariant() == catname) cat = c;
				}
			}

			// Make the category if needed
			if(cat == null)
			{
				string cattitle = catnames[0].Trim();
				if(string.IsNullOrEmpty(cattitle)) cattitle = "Decorate";
				cat = new ThingCategory(parent, catname, cattitle);
				categories.Add(cat); // ^.^
			}

			// Still have subcategories?
			if(catnames.Length > 1)
			{
				string[] remainingnames = new string[catnames.Length - 1];
				Array.Copy(catnames, 1, remainingnames, 0, remainingnames.Length);
				return GetThingCategory(cat, cat.Children, remainingnames);
			}

			return cat;
		}
		
		// This loads Decorate data from a specific file or lump name
		private void LoadDecorateFromLocation(DecorateParser parser, string location)
		{
			//General.WriteLogLine("Including DECORATE resource '" + location + "'...");
			Dictionary<string, Stream> decostreams = currentreader.GetDecorateData(location);
			foreach(KeyValuePair<string, Stream> group in decostreams)
			{
				// Parse this data
				parser.Parse(group.Value, Path.Combine(currentreader.Location.location, group.Key));
			}
		}
		
		// This gets thing information by index
		public ThingTypeInfo GetThingInfo(int thingtype)
		{
			// Index in config?
			if(thingtypes.ContainsKey(thingtype))
			{
				// Return from config
				return thingtypes[thingtype];
			}

			// Create unknown thing info
			return new ThingTypeInfo(thingtype);
		}

		// This gets thing information by index
		// Returns null when thing type info could not be found
		public ThingTypeInfo GetThingInfoEx(int thingtype)
		{
			// Index in config?
			if(thingtypes.ContainsKey(thingtype))
			{
				// Return from config
				return thingtypes[thingtype];
			}

			// No such thing type known
			return null;
		}
		
		#endregion

		#region ================== mxd. Modeldef, Voxeldef, Gldefs, Mapinfo

		//mxd. This creates <Actor Class, Thing.Type> dictionary. Should be called after all DECORATE actors are parsed
		private Dictionary<string, List<int>> CreateActorsByClassList() 
		{
			Dictionary<string, List<int>> actors = new Dictionary<string, List<int>>(StringComparer.Ordinal);
			if(string.IsNullOrEmpty(General.Map.Config.DecorateGames)) return actors;

			//read our new shiny ClassNames for default game things
			foreach(KeyValuePair<int, ThingTypeInfo> ti in thingtypes) 
			{
				if(!string.IsNullOrEmpty(ti.Value.ClassName))
				{
					string classname = ti.Value.ClassName.ToLowerInvariant();
					if(!actors.ContainsKey(classname)) actors.Add(classname, new List<int>());
					actors[classname].Add(ti.Key);
				}
			}

			if(actors.Count == 0) 
				General.ErrorLogger.Add(ErrorType.Warning, "Warning: unable to find any DECORATE actor definitions!");

			return actors;
		}

		//mxd
		public void ReloadModeldef() 
		{
			if(modeldefentries != null) 
			{
				foreach(KeyValuePair<int, ModelData> group in modeldefentries)
					group.Value.Dispose();
			}

			// Bail out when not supported by currect game configuration
			if(string.IsNullOrEmpty(General.Map.Config.DecorateGames)) return;

			General.MainWindow.DisplayStatus(StatusType.Busy, "Reloading model definitions...");
			LoadModeldefs(CreateActorsByClassList());

			General.MainWindow.DisplayStatus(StatusType.Busy, "Reloading voxel definitions...");
			LoadVoxels();

			foreach(Thing t in General.Map.Map.Things) t.UpdateCache();

			//rebuild geometry if in Visual mode
			if(General.Editing.Mode != null && General.Editing.Mode.GetType().Name == "BaseVisualMode") 
			{
				General.Editing.Mode.OnReloadResources();
			}

			General.MainWindow.DisplayReady();
		}

		//mxd
		public void ReloadGldefs() 
		{
			// Bail out when not supported by currect game configuration
			if(string.IsNullOrEmpty(General.Map.Config.DecorateGames)) return;
			
			General.MainWindow.DisplayStatus(StatusType.Busy, "Reloading GLDEFS...");

			try 
			{
				LoadGldefs(CreateActorsByClassList());
			} 
			catch(ArgumentNullException) 
			{
				MessageBox.Show("GLDEFS reload failed. Try using 'Reload Resources' instead.\nCheck 'Errors and Warnings' window for more details.");
				General.MainWindow.DisplayReady();
				return;
			}

			//rebuild geometry if in Visual mode
			if(General.Editing.Mode != null && General.Editing.Mode.GetType().Name == "BaseVisualMode") 
			{
				General.Editing.Mode.OnReloadResources();
			}

			General.MainWindow.DisplayReady();
		}

		//mxd. This parses modeldefs. Should be called after all DECORATE actors are parsed and actorsByClass dictionary created
		private void LoadModeldefs(Dictionary<string, List<int>> actorsByClass) 
		{
			//if no actors defined in DECORATE or game config...
			if(actorsByClass.Count == 0) return;

			Dictionary<string, ModelData> modelDefEntriesByName = new Dictionary<string, ModelData>(StringComparer.Ordinal);
			ModeldefParser parser = new ModeldefParser();

			foreach(DataReader dr in containers) 
			{
				currentreader = dr;

				Dictionary<string, Stream> streams = dr.GetModeldefData();
				foreach(KeyValuePair<string, Stream> group in streams) 
				{
					// Parse the data
					if(parser.Parse(group.Value, currentreader.Location.location + "\\" + group.Key)) 
					{
						foreach(KeyValuePair<string, ModelData> g in parser.Entries) 
						{
							if(modelDefEntriesByName.ContainsKey(g.Key)) 
							{
								General.ErrorLogger.Add(ErrorType.Warning, "Model definition for actor '" + g.Key + "' is double-defined in '" + group.Key + "'");
								modelDefEntriesByName[g.Key] = g.Value;
							} 
							else 
							{
								modelDefEntriesByName.Add(g.Key, g.Value);
							}
						}
					}
				}
			}

			currentreader = null;

			foreach(KeyValuePair<string, ModelData> e in modelDefEntriesByName) 
			{
				if(actorsByClass.ContainsKey(e.Key))
				{
					foreach(int i in actorsByClass[e.Key]) modeldefentries[i] = modelDefEntriesByName[e.Key];
				}
				else if(!decorate.ActorsByClass.ContainsKey(e.Key))
				{
					General.ErrorLogger.Add(ErrorType.Warning, "Got MODELDEF override for class '" + e.Key + "', but haven't found such class in Decorate");
				}
			}
		}

		//mxd
		private void LoadVoxels() 
		{
			// Bail out when not supported by currect game configuration
			if(string.IsNullOrEmpty(General.Map.Config.DecorateGames)) return;
			
			//Get names of all voxel models, which can be used "as is"
			Dictionary<string, bool> voxelNames = new Dictionary<string, bool>(StringComparer.Ordinal);
			
			foreach(DataReader dr in containers) 
			{
				currentreader = dr;

				string[] result = dr.GetVoxelNames();
				if(result == null) continue;

				foreach(string s in result) 
				{
					if(!voxelNames.ContainsKey(s)) voxelNames.Add(s, false);
				}
			}

			Dictionary<string, List<int>> sprites = new Dictionary<string, List<int>>(StringComparer.Ordinal);

			// Go for all things
			foreach(ThingTypeInfo ti in thingtypes.Values) 
			{
				// Valid sprite name?
				string sprite;

				if(ti.Sprite.Length == 0 || ti.Sprite.Length > CLASIC_IMAGE_NAME_LENGTH) 
				{
					if(ti.Actor == null) continue;
					sprite = ti.Actor.FindSuitableVoxel(voxelNames);
				} 
				else 
				{
					sprite = ti.Sprite;
				}

				if(string.IsNullOrEmpty(sprite)) continue;
				if(!sprites.ContainsKey(sprite)) sprites.Add(sprite, new List<int>());
				sprites[sprite].Add(ti.Index);
			}

			VoxeldefParser parser = new VoxeldefParser();
			Dictionary<string, bool> processed = new Dictionary<string, bool>(StringComparer.Ordinal);

			//parse VOXLEDEF
			foreach(DataReader dr in containers) 
			{
				currentreader = dr;

				KeyValuePair<string, Stream> group = dr.GetVoxeldefData();
				if(group.Value != null && parser.Parse(group.Value, group.Key)) 
				{
					foreach(KeyValuePair<string, ModelData> entry in parser.Entries)
					{
						foreach(KeyValuePair<string, List<int>> sc in sprites) 
						{
							if(sc.Key.Contains(entry.Key)) 
							{
								foreach(int id in sc.Value) modeldefentries[id] = entry.Value;
								processed.Add(entry.Key, false);
							}
						}
					}
				}
			}

			currentreader = null;

			//get voxel models
			foreach(KeyValuePair<string, bool> group in voxelNames) 
			{
				if(processed.ContainsKey(group.Key)) continue;
				foreach(KeyValuePair<string, List<int>> sc in sprites) 
				{
					if(sc.Key.Contains(group.Key)) 
					{
						//it's a model without a definition, and it corresponds to a sprite we can display, so let's add it
						ModelData data = new ModelData { IsVoxel = true };
						data.ModelNames.Add(group.Key);

						foreach(int id in sprites[sc.Key]) modeldefentries[id] = data;
					}
				}
			}
		}

		//mxd. This parses gldefs. Should be called after all DECORATE actors are parsed and actorsByClass dictionary created
		private void LoadGldefs(Dictionary<string, List<int>> actorsByClass) 
		{
			//if no actors defined in DECORATE or game config...
			if(actorsByClass.Count == 0) return;

			GldefsParser parser = new GldefsParser { OnInclude = ParseFromLocation };

			//load gldefs from resources
			foreach(DataReader dr in containers) 
			{
				currentreader = dr;
				parser.ClearIncludesList();
				Dictionary<string, Stream> streams = dr.GetGldefsData(General.Map.Config.GameType);

				foreach(KeyValuePair<string, Stream> group in streams)
					parser.Parse(group.Value, group.Key);
			}

			//create gldefsEntries dictionary
			foreach(KeyValuePair<string, string> e in parser.Objects) //ClassName, Light name
			{ 
				//if we have decorate actor and light definition for given ClassName...
				if(actorsByClass.ContainsKey(e.Key) && parser.LightsByName.ContainsKey(e.Value)) 
				{
					foreach(int i in actorsByClass[e.Key])
					{
						if(gldefsentries.ContainsKey(i))
							gldefsentries[i] = parser.LightsByName[e.Value];
						else
							gldefsentries.Add(i, parser.LightsByName[e.Value]);
					}
				} 
				else if(!decorate.AllActorsByClass.ContainsKey(e.Key))
				{
					General.ErrorLogger.Add(ErrorType.Warning, "Got GLDEFS light for class '" + e.Key + "', but haven't found such class in DECORATE");
				}
			}

			// Grab them glowy flats!
			glowingflats = parser.GlowingFlats;
		}

		//mxd. This loads (Z)MAPINFO
		private void LoadMapInfo(out Dictionary<int, string> spawnnums, out Dictionary<int, string> doomednums)
		{
			MapinfoParser parser = new MapinfoParser { OnInclude = ParseFromLocation };

			foreach(DataReader dr in containers)
			{
				currentreader = dr;

				Dictionary<string, Stream> streams = dr.GetMapinfoData();
				foreach(KeyValuePair<string, Stream> group in streams) 
				{
					// Parse the data
					parser.Parse(group.Value, Path.Combine(currentreader.Location.location, group.Key), General.Map.Options.LevelName); 
				}
			}

			// Set the output values
			spawnnums = parser.SpawnNums;
			doomednums = parser.DoomEdNums;

			// Store to our MapInfo property
			currentreader = null;
			mapinfo = parser.MapInfo ?? new MapInfo();
		}

		private void ParseFromLocation(ZDTextParser parser, string location)
		{
			if(currentreader.IsSuspended) throw new Exception("Data reader is suspended");
			Stream s = currentreader.LoadFile(location);
			if(s != null) parser.Parse(s, location);
		}

		//mxd. This loads REVERBS
		private void LoadReverbs() 
		{
			reverbs.Clear();

			// Bail out when not supported by currect game configuration
			if(string.IsNullOrEmpty(General.Map.Config.DecorateGames)) return;

			ReverbsParser parser = new ReverbsParser();
			foreach(DataReader dr in containers) 
			{
				currentreader = dr;
				Dictionary<string, Stream> streams = dr.GetReverbsData();
				foreach(KeyValuePair<string, Stream> group in streams) 
				{
					// Parse the data
					parser.Parse(group.Value, group.Key);
				}
			}

			currentreader = null;
			reverbs = parser.GetReverbs();
		}

		//mxd. This loads SNDSEQ
		private void LoadSndSeq()
		{
			soundsequences.Clear();

			// Bail out when not supported by currect game configuration
			if(string.IsNullOrEmpty(General.Map.Config.DecorateGames)) return;

			SndSeqParser parser = new SndSeqParser();
			foreach(DataReader dr in containers) 
			{
				currentreader = dr;
				List<Stream> streams = dr.GetSndSeqData();

				// Parse the data
				foreach(Stream s in streams)
				{
					if(s != null) parser.Parse(s, "SNDSEQ");
				}
			}

			currentreader = null;
			soundsequences = parser.GetSoundSequences();
		}

		//mxd
		internal MemoryStream LoadFile(string name) 
		{
			// Relative path?
			if(name.StartsWith("..\\")) name = name.Replace("..\\", "");
			
			foreach(DataReader dr in containers)
				if(dr.FileExists(name)) return dr.LoadFile(name);
			return null;
		}

		#endregion

		#region ================== Tools

		// This finds the first IWAD resource
		// Returns false when not found
		public bool FindFirstIWAD(out DataLocation result)
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
			if(General.Map.Config.MixTexturesFlats)
			{
				lock(usedtextures)
				{
					usedtextures.Clear();

					// Go through the map to find the used textures
					foreach(Sidedef sd in General.Map.Map.Sidedefs)
					{
						// Add used textures to dictionary
						if(sd.LongHighTexture != MapSet.EmptyLongName) usedtextures[sd.LongHighTexture] = 0;
						if(sd.LongMiddleTexture != MapSet.EmptyLongName) usedtextures[sd.LongMiddleTexture] = 0;
						if(sd.LongLowTexture != MapSet.EmptyLongName) usedtextures[sd.LongLowTexture] = 0;
					}

					// Go through the map to find the used flats
					foreach(Sector s in General.Map.Map.Sectors)
					{
						// Add used flats to dictionary
						usedtextures[s.LongFloorTexture] = 0;
						usedtextures[s.LongCeilTexture] = 0;
					}

					// Notify the background thread that it needs to update the images
					updatedusedtextures = true;
				}
			}
			//mxd. Use separate collections
			else
			{
				lock(usedtextures)
				{
					usedtextures.Clear();

					// Go through the map to find the used textures
					foreach(Sidedef sd in General.Map.Map.Sidedefs)
					{
						// Add used textures to dictionary
						if(sd.LongHighTexture != MapSet.EmptyLongName) usedtextures[sd.LongHighTexture] = 0;
						if(sd.LongMiddleTexture != MapSet.EmptyLongName) usedtextures[sd.LongMiddleTexture] = 0;
						if(sd.LongLowTexture != MapSet.EmptyLongName) usedtextures[sd.LongLowTexture] = 0;
					}
				}

				lock(usedflats)
				{
					usedflats.Clear();

					// Go through the map to find the used flats
					foreach(Sector s in General.Map.Map.Sectors)
					{
						// Add used flats to dictionary
						usedflats[s.LongFloorTexture] = 0;
						usedflats[s.LongCeilTexture] = 0;
					}
				}
				
				// Notify the background thread that it needs to update the images
				updatedusedtextures = true;
			}
		}
		
		#endregion
	}
}
