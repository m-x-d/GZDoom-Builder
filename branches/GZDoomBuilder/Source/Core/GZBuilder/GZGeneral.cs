using System;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.GZBuilder.Data;

namespace CodeImp.DoomBuilder.GZBuilder
{
	//mxd. should get rid of this class one day...
	public static class GZGeneral
	{
		//gzdoom light types
		private static int[] gzLights = { /* normal lights */ 9800, 9801, 9802, 9803, 9804, /* additive lights */ 9810, 9811, 9812, 9813, 9814, /* negative lights */ 9820, 9821, 9822, 9823, 9824, /* vavoom lights */ 1502, 1503};
		public static int[] GZ_LIGHTS { get { return gzLights; } }
		private static int[] gzLightTypes = { 5, 10, 15 }; //these are actually offsets in gz_lights
		public static int[] GZ_LIGHT_TYPES { get { return gzLightTypes; } }
		private static DynamicLightType[] gzAnimatedLightTypes = { DynamicLightType.FLICKER, DynamicLightType.RANDOM, DynamicLightType.PULSE };
		public static DynamicLightType[] GZ_ANIMATED_LIGHT_TYPES { get { return gzAnimatedLightTypes; } }

		//asc script action specials
		private static int[] acsSpecials = { 80, 81, 82, 83, 84, 85, 226 };
		public static int[] ACS_SPECIALS { get { return acsSpecials; } }

		//version
		public const float Version = 1.14f;
		public const char Revision = ' ';

		public static void Init() {
			//bind actions
			General.Actions.BindMethods(typeof(GZGeneral));
			General.MainWindow.UpdateGZDoomPanel();
		}

//actions
		[BeginAction("gztogglemodels")]
		private static void toggleModels() {
			General.Settings.GZDrawModels = !General.Settings.GZDrawModels;
			General.MainWindow.DisplayStatus(StatusType.Action, "Models rendering is " + (General.Settings.GZDrawModels ? "ENABLED" : "DISABLED"));
			General.MainWindow.RedrawDisplay();
			General.MainWindow.UpdateGZDoomPanel();
		}

		[BeginAction("gztogglelights")]
		private static void toggleLights() {
			General.Settings.GZDrawLights = !General.Settings.GZDrawLights;
			General.MainWindow.DisplayStatus(StatusType.Action, "Dynamic lights rendering is " + (General.Settings.GZDrawLights ? "ENABLED" : "DISABLED"));
			General.MainWindow.RedrawDisplay();
			General.MainWindow.UpdateGZDoomPanel();
		}

		[BeginAction("gztogglelightsanimation")]
		private static void toggleLightsAnimation() {
			General.Settings.GZAnimateLights = !General.Settings.GZAnimateLights;
			General.MainWindow.DisplayStatus(StatusType.Action, "Dynamic lights animation is " + (General.Settings.GZAnimateLights ? "ENABLED" : "DISABLED"));
			General.MainWindow.RedrawDisplay();
			General.MainWindow.UpdateGZDoomPanel();
		}

		[BeginAction("gztogglefog")]
		private static void toggleFog() {
			General.Settings.GZDrawFog = !General.Settings.GZDrawFog;
			General.MainWindow.DisplayStatus(StatusType.Action, "Fog rendering is " + (General.Settings.GZDrawFog ? "ENABLED" : "DISABLED"));
			General.MainWindow.RedrawDisplay();
			General.MainWindow.UpdateGZDoomPanel();
		}

		[BeginAction("gzdrawselectedmodelsonly")]
		private static void toggleDrawSelectedModelsOnly() {
			General.Settings.GZDrawSelectedModelsOnly = !General.Settings.GZDrawSelectedModelsOnly;
			General.MainWindow.DisplayStatus(StatusType.Action, "Rendering " + (General.Settings.GZDrawSelectedModelsOnly ? "only selected" : "all") + " models.");
			General.MainWindow.RedrawDisplay();
			General.MainWindow.UpdateGZDoomPanel();
		}

		[BeginAction("gztogglefx")]
		private static void toggleFx() {
			int on = 0;
			on += General.Settings.GZDrawFog ? 1 : -1;
			on += General.Settings.GZDrawLights ? 1 : -1;
			on += General.Settings.GZDrawModels ? 1 : -1;

			bool enable = (on < 0);

			General.Settings.GZDrawFog = enable;
			General.Settings.GZDrawLights = enable;
			General.Settings.GZDrawModels = enable;
			General.MainWindow.DisplayStatus(StatusType.Action, "Advanced effects are " + (enable ? "ENABLED" : "DISABLED") );

			General.MainWindow.RedrawDisplay();
			General.MainWindow.UpdateGZDoomPanel();
		}

		[BeginAction("gztoggleeventlines")]
		private static void toggleEventLines() {
			General.Settings.GZShowEventLines = !General.Settings.GZShowEventLines;
			General.MainWindow.DisplayStatus(StatusType.Action, "Event lines are " + (General.Settings.GZShowEventLines ? "ENABLED" : "DISABLED"));
			General.MainWindow.RedrawDisplay();
			General.MainWindow.UpdateGZDoomPanel();
		}

		[BeginAction("gztogglevisualvertices")]
		private static void toggleVisualVertices() {
			General.Settings.GZShowVisualVertices = !General.Settings.GZShowVisualVertices;
			General.MainWindow.DisplayStatus(StatusType.Action, "Visual vertices are " + (General.Settings.GZShowVisualVertices ? "ENABLED" : "DISABLED"));
			General.MainWindow.RedrawDisplay();
			General.MainWindow.UpdateGZDoomPanel();
		}

		//main menu actions
		[BeginAction("gzreloadmodeldef")]
		private static void reloadModeldef() {
			if(General.Map != null) {
				General.Map.Data.ReloadModeldef();
				General.MainWindow.RedrawDisplay(); //dbg?
			}
		}

		[BeginAction("gzreloadgldefs")]
		private static void reloadGldefs() {
			if (General.Map != null)
				General.Map.Data.ReloadGldefs();
		}

		[BeginAction("gzreloadmapinfo")]
		private static void reloadMapInfo() {
			if (General.Map != null)
				General.Map.Data.ReloadMapInfo();
		}
	}
}