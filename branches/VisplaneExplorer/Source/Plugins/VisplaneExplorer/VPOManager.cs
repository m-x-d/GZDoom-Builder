#region === Copyright (c) 2010 Pascal van der Heiden ===

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

#endregion

namespace CodeImp.DoomBuilder.Plugins.VisplaneExplorer
{
	internal class VPOManager : IDisposable
	{
		#region ================== Constants

		public const int POINTS_PER_ITERATION = 100;
		private const int EXPECTED_RESULTS_BUFFER = 2000;

		private readonly int[] TEST_ANGLES = new int[] { 0, 90, 180, 270, 45, 135, 225, 315 /*, 22, 67, 112, 157, 202, 247, 292, 337 */ };
		private const int TEST_HEIGHT = 41 + 8;

		private const int RESULT_OK = 0;
		private const int RESULT_BAD_Z = -1;
		private const int RESULT_IN_VOID = -2;
		private const int RESULT_OVERFLOW = -3;
		
		#endregion

		#region ================== APIs

		[DllImport("kernel32.dll")]
		public static extern IntPtr LoadLibrary(string filename);

		[DllImport("kernel32.dll")]
		public static extern IntPtr GetProcAddress(IntPtr modulehandle, string procedurename);

		[DllImport("kernel32.dll")]
		public static extern bool FreeLibrary(IntPtr modulehandle);
		
		#endregion

		#region ================== Delegates

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate string VPO_GetError();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int VPO_LoadWAD(string filename);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int VPO_OpenMap(string mapname);

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void VPO_FreeWAD();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate void VPO_CloseMap();

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int VPO_TestSpot(int x, int y, int dz, int angle,
			ref int visplanes, ref int drawsegs, ref int openings, ref int solidsegs);

		#endregion

		#region ================== Variables

		// Main objects
		private string[] tempfiles;
		private IntPtr[] dlls;
		private Thread[] threads;

		// Map to load
		private string filename;
		private string mapname;

		// Input and output queue (both require a lock on 'points' !)
		private Queue<Point> points = new Queue<Point>(EXPECTED_RESULTS_BUFFER);
		private Queue<PointData> results = new Queue<PointData>(EXPECTED_RESULTS_BUFFER);
		
		#endregion

		#region ================== Properties

		public int NumThreads { get { return Environment.ProcessorCount; } }

		#endregion

		#region ================== Constructor / Destructor
		
		// Constructor
		public VPOManager()
		{
			// Load a DLL for each thread
			dlls = new IntPtr[NumThreads];
			tempfiles = new string[NumThreads];
			string originaldll = Path.Combine(General.PluginsPath, "vpo.dll");
			for(int i = 0; i < dlls.Length; i++)
			{
				// We must copy the DLL file with a unique name, because LoadLibrary will
				// share loaded libraries with the same names and LoadLibraryEx does not
				// have a flag to force loading it multiple times.
				tempfiles[i] = BuilderPlug.MakeTempFilename(".dll");
				File.Copy(originaldll, tempfiles[i]);
				dlls[i] = LoadLibrary(tempfiles[i]);
				if(dlls[i] == IntPtr.Zero) throw new Exception("Unable to load vpo.dll");
			}
		}

		// Disposer
		public void Dispose()
		{
			if(threads != null) Stop();
			
			if(dlls != null)
			{
				for(int i = 0; i < dlls.Length; i++)
				{
					FreeLibrary(dlls[i]);
				}
				dlls = null;
			}
		}
		
		#endregion

		#region ================== Processing

		// The thread!
		private void ProcessingThread(object index)
		{
			// Get function pointers
			VPO_GetError GetError = (VPO_GetError)Marshal.GetDelegateForFunctionPointer(GetProcAddress(dlls[(int)index], "VPO_GetError"), typeof(VPO_GetError));
			VPO_LoadWAD LoadWAD = (VPO_LoadWAD)Marshal.GetDelegateForFunctionPointer(GetProcAddress(dlls[(int)index], "VPO_LoadWAD"), typeof(VPO_LoadWAD));
			VPO_OpenMap OpenMap = (VPO_OpenMap)Marshal.GetDelegateForFunctionPointer(GetProcAddress(dlls[(int)index], "VPO_OpenMap"), typeof(VPO_OpenMap));
			VPO_FreeWAD FreeWAD = (VPO_FreeWAD)Marshal.GetDelegateForFunctionPointer(GetProcAddress(dlls[(int)index], "VPO_FreeWAD"), typeof(VPO_FreeWAD));
			VPO_CloseMap CloseMap = (VPO_CloseMap)Marshal.GetDelegateForFunctionPointer(GetProcAddress(dlls[(int)index], "VPO_CloseMap"), typeof(VPO_CloseMap));
			VPO_TestSpot TestSpot = (VPO_TestSpot)Marshal.GetDelegateForFunctionPointer(GetProcAddress(dlls[(int)index], "VPO_TestSpot"), typeof(VPO_TestSpot));

			try
			{
				// Load the map
				LoadWAD(filename);
				OpenMap(mapname);

				// Processing
				Queue<Point> todo = new Queue<Point>(POINTS_PER_ITERATION);
				Queue<PointData> done = new Queue<PointData>(POINTS_PER_ITERATION);
				while(true)
				{
					lock(points)
					{
						// Flush done points to the results
						int numdone = done.Count;
						for(int i = 0; i < numdone; i++)
							results.Enqueue(done.Dequeue());
						
						// Get points from the waiting queue into my todo queue for processing
						int numtodo = Math.Min(POINTS_PER_ITERATION, points.Count);
						for(int i = 0; i < numtodo; i++)
							todo.Enqueue(points.Dequeue());
					}

					// Don't keep locking!
					if(todo.Count == 0)
						Thread.Sleep(31);
					
					// Process the points
					while(todo.Count > 0)
					{
						Point p = todo.Dequeue();
						PointData pd = new PointData();
						pd.x = p.X;
						pd.y = p.Y;

						for(int i = 0; i < TEST_ANGLES.Length; i++)
						{
							int vp = 0;
							int ds = 0;
							int ss = 0;
							int op = 0;
							int result = TestSpot(p.X, p.Y, TEST_HEIGHT, TEST_ANGLES[i], ref vp, ref ds, ref op, ref ss);

							switch(result)
							{
								case RESULT_OK:
									pd.visplanes = Math.Max(pd.visplanes, vp);
									pd.drawsegs = Math.Max(pd.drawsegs, ds);
									pd.solidsegs = Math.Max(pd.solidsegs, ss);
									pd.openings = Math.Max(pd.openings, op);
									break;

								case RESULT_BAD_Z:
									// Ignore this position.
									break;

								case RESULT_IN_VOID:
									// No data.
									break;

								case RESULT_OVERFLOW:
									pd.visplanes = int.MaxValue;
									pd.drawsegs = int.MaxValue;
									pd.solidsegs = int.MaxValue;
									pd.openings = int.MaxValue;
									break;
							}
						}

						done.Enqueue(pd);
					}
				}
			}
			catch(ThreadInterruptedException)
			{
			}
			finally
			{
				CloseMap();
				FreeWAD();
			}
		}

		#endregion

		#region ================== Public Methods

		// This loads a map
		public void Start(string filename, string mapname)
		{
			this.filename = filename;
			this.mapname = mapname;
			results.Clear();
			
			// Start a thread on each core
			threads = new Thread[dlls.Length];
			for(int i = 0; i < threads.Length; i++)
			{
				threads[i] = new Thread(ProcessingThread);
				threads[i].Priority = ThreadPriority.BelowNormal;
				threads[i].Name = "Visplane Explorer " + i;
				threads[i].Start(i);
			}
		}

		// This frees the map
		public void Stop()
		{
			if(threads != null)
			{
				lock(points)
				{
					// Stop all threads
					for(int i = 0; i < threads.Length; i++)
					{
						threads[i].Interrupt();
						threads[i].Join();
					}
					threads = null;
					points.Clear();
				}
			}
		}

		// This clears the list of enqueued points
		public void ClearPoints()
		{
			lock(points)
			{
				points.Clear();
			}
		}

		// This gives points to process
		public void EnqueuePoints(IEnumerable<Point> newpoints)
		{
			lock(points)
			{
				foreach(Point p in newpoints)
					points.Enqueue(p);
			}
		}

		// This fetches results (in 'data') and returns the number of points
		// remaining to be processed. Note that the number of points returned
		// does not include the points currently being processed in a single
		// iteration for each thread.
		public int DequeueResults(List<PointData> data)
		{
			lock(points)
			{
				int numresults = results.Count;
				if(data.Capacity - data.Count < numresults)
					data.Capacity = data.Count + numresults;
				for(int i = 0; i < numresults; i++)
					data.Add(results.Dequeue());
				return points.Count;
			}
		}

		#endregion
	}
}
