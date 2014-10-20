#region ================== Namespaces

using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Config;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.GZBuilder.Data;

#endregion

namespace CodeImp.DoomBuilder.GZBuilder
{
	//mxd. should get rid of this class one day...
	public static class GZGeneral
	{
		#region ================== Properties

		//gzdoom light types
		private static readonly int[] gzLights = { /* normal lights */ 9800, 9801, 9802, 9803, 9804, /* additive lights */ 9810, 9811, 9812, 9813, 9814, /* negative lights */ 9820, 9821, 9822, 9823, 9824, /* vavoom lights */ 1502, 1503};
		public static int[] GZ_LIGHTS { get { return gzLights; } }
		private static readonly int[] gzLightTypes = { 5, 10, 15 }; //these are actually offsets in gz_lights
		public static int[] GZ_LIGHT_TYPES { get { return gzLightTypes; } }
		private static readonly DynamicLightType[] gzAnimatedLightTypes = { DynamicLightType.FLICKER, DynamicLightType.RANDOM, DynamicLightType.PULSE };
		public static DynamicLightType[] GZ_ANIMATED_LIGHT_TYPES { get { return gzAnimatedLightTypes; } }

		//asc script action specials
		private static readonly int[] acsSpecials = { 80, 81, 82, 83, 84, 85, 226 };
		public static int[] ACS_SPECIALS { get { return acsSpecials; } }

		#endregion

		#region ================== Methods

		public static void Init() 
		{
			//bind actions
			General.Actions.BindMethods(typeof(GZGeneral));
			General.MainWindow.UpdateGZDoomPanel();
		}

		#endregion

		#region ================== Actions

		[BeginAction("gztogglemodels")]
		private static void ToggleModelsRenderingMode() 
		{
			switch(General.Settings.GZDrawModelsMode)
			{
				case ModelRenderMode.NONE:
					General.Settings.GZDrawModelsMode = ModelRenderMode.SELECTION;
					General.MainWindow.DisplayStatus(StatusType.Action, "Models rendering mode: SELECTION ONLY");
					break;

				case ModelRenderMode.SELECTION:
					General.Settings.GZDrawModelsMode = ModelRenderMode.ALL;
					General.MainWindow.DisplayStatus(StatusType.Action, "Models rendering mode: ALL");
					break;

				case ModelRenderMode.ALL:
					General.Settings.GZDrawModelsMode = ModelRenderMode.NONE;
					General.MainWindow.DisplayStatus(StatusType.Action, "Models rendering mode: NONE");
					break;
			}
			
			General.MainWindow.RedrawDisplay();
			General.MainWindow.UpdateGZDoomPanel();
		}

		[BeginAction("gztogglelights")]
		private static void ToggleLightsRenderingMode() 
		{
			switch(General.Settings.GZDrawLightsMode) 
			{
				case LightRenderMode.NONE:
					General.Settings.GZDrawLightsMode = LightRenderMode.ALL;
					General.MainWindow.DisplayStatus(StatusType.Action, "Dynamic lights rendering mode: ALL");
					break;

				case LightRenderMode.ALL:
					General.Settings.GZDrawLightsMode = LightRenderMode.ALL_ANIMATED;
					General.MainWindow.DisplayStatus(StatusType.Action, "Models rendering mode: ANIMATED");
					break;

				case LightRenderMode.ALL_ANIMATED:
					General.Settings.GZDrawLightsMode = LightRenderMode.NONE;
					General.MainWindow.DisplayStatus(StatusType.Action, "Models rendering mode: NONE");
					break;
			}
			
			General.MainWindow.RedrawDisplay();
			General.MainWindow.UpdateGZDoomPanel();
		}

		[BeginAction("gztogglefog")]
		private static void ToggleFog() 
		{
			General.Settings.GZDrawFog = !General.Settings.GZDrawFog;
			General.MainWindow.DisplayStatus(StatusType.Action, "Fog rendering is " + (General.Settings.GZDrawFog ? "ENABLED" : "DISABLED"));
			General.MainWindow.RedrawDisplay();
			General.MainWindow.UpdateGZDoomPanel();
		}

		[BeginAction("gztogglefx")]
		private static void ToggleFx() 
		{
			int on = 0;
			on += General.Settings.GZDrawFog ? 1 : -1;
			on += General.Settings.GZDrawLightsMode != LightRenderMode.NONE ? 1 : -1;
			on += General.Settings.GZDrawModelsMode != ModelRenderMode.NONE ? 1 : -1;

			bool enable = (on < 0);

			General.Settings.GZDrawFog = enable;
			General.Settings.GZDrawLightsMode = (enable ? LightRenderMode.ALL : LightRenderMode.NONE);
			General.Settings.GZDrawModelsMode = (enable ? ModelRenderMode.ALL : ModelRenderMode.NONE);
			General.MainWindow.DisplayStatus(StatusType.Action, "Advanced effects are " + (enable ? "ENABLED" : "DISABLED") );

			General.MainWindow.RedrawDisplay();
			General.MainWindow.UpdateGZDoomPanel();
		}

		[BeginAction("gztoggleeventlines")]
		private static void ToggleEventLines() 
		{
			General.Settings.GZShowEventLines = !General.Settings.GZShowEventLines;
			General.MainWindow.DisplayStatus(StatusType.Action, "Event lines are " + (General.Settings.GZShowEventLines ? "ENABLED" : "DISABLED"));
			General.MainWindow.RedrawDisplay();
			General.MainWindow.UpdateGZDoomPanel();
		}

		[BeginAction("gztogglevisualvertices")]
		private static void ToggleVisualVertices() 
		{
			General.Settings.GZShowVisualVertices = !General.Settings.GZShowVisualVertices;
			General.MainWindow.DisplayStatus(StatusType.Action, "Visual vertices are " + (General.Settings.GZShowVisualVertices ? "ENABLED" : "DISABLED"));
			General.MainWindow.RedrawDisplay();
			General.MainWindow.UpdateGZDoomPanel();
		}

		//main menu actions
		[BeginAction("gzreloadmodeldef")]
		private static void ReloadModeldef() 
		{
			if(General.Map != null) General.Map.Data.ReloadModeldef();
		}

		[BeginAction("gzreloadgldefs")]
		private static void ReloadGldefs() 
		{
			if (General.Map != null) General.Map.Data.ReloadGldefs();
		}

		[BeginAction("gzreloadmapinfo")]
		private static void ReloadMapInfo() 
		{
			if (General.Map != null) General.Map.Data.ReloadMapInfo();
		}

		#endregion
	}
}