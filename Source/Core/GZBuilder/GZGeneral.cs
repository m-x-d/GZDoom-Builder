using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using CodeImp.DoomBuilder.Controls;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.ZDoom;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.Config;

using CodeImp.DoomBuilder.GZBuilder.Data;
using CodeImp.DoomBuilder.GZBuilder.Controls;

namespace CodeImp.DoomBuilder.GZBuilder
{
    //mxd. should get rid of this class one day...
    public class GZGeneral
    {
        //gzdoom light types
        private static int[] gzLights = { /* normal lights */ 9800, 9801, 9802, 9803, 9804, /* additive lights */ 9810, 9811, 9812, 9813, 9814, /* negative lights */ 9820, 9821, 9822, 9823, 9824, /* vavoom lights */ 1502, 1503};
        public static int[] GZ_LIGHTS { get { return gzLights; } }
        private static int[] gzLightTypes = { 5, 10, 15 }; //this is actually offsets in gz_lights
        public static int[] GZ_LIGHT_TYPES { get { return gzLightTypes; } }
        private static int[] gzAnimatedLightTypes = { (int)GZDoomLightType.FLICKER, (int)GZDoomLightType.RANDOM, (int)GZDoomLightType.PULSE };
        public static int[] GZ_ANIMATED_LIGHT_TYPES {  get { return gzAnimatedLightTypes; } }

        public static bool UDMF;

        //public static float[] FogTable; // light to fog conversion table for black fog

        //version
        public const float Version = 1.06f;

        //debug console
#if DEBUG
        private static Docker console;
#endif


        public static void Init() {
            //bind actions
            General.Actions.BindMethods(typeof(GZGeneral));
            General.MainWindow.UpdateGZDoomPannel();

            //create fog table
            /*FogTable = new float[256];
            byte gl_distfog = 255;

            for (int i = 0; i < 256; i++) {
                if (i < 164) {
                    FogTable[i] = (gl_distfog >> 1) + (gl_distfog) * (164 - i) / 164;
                } else if (i < 230) {
                    FogTable[i] = (gl_distfog >> 1) - (gl_distfog >> 1) * (i - 164) / (230 - 164);
                } else FogTable[i] = 0;

                //if (i < 128) {
                    //distfogtable[1][i] = 6.f + (gl_distfog >> 1) + (gl_distfog) * (128 - i) / 48;
                //} else if (i < 216) {
                    //distfogtable[1][i] = (216.f - i) / ((216.f - 128.f)) * gl_distfog / 10;
                //} else distfogtable[1][i] = 0;
            }*/

            //float[] ft = FogTable;

            //create console
#if DEBUG
            ConsoleDocker cd = new ConsoleDocker();
            console = new Docker("consoledockerpannel", "Console", cd);
            ((MainForm)General.Interface).addDocker(console);
            ((MainForm)General.Interface).selectDocker(console);
#endif
        }

        public static void OnMapOpenEnd() {
            UDMF = (General.Map.Config.FormatInterface == "UniversalMapSetIO");
            General.MainWindow.UpdateGZDoomPannel();
        }

        public static void OnReloadResources() {
#if DEBUG
            ((ConsoleDocker)console.Control).Clear();
#endif
        }

//debug
        public static void LogAndTraceWarning(string message) {
            General.ErrorLogger.Add(ErrorType.Warning, message);
            General.WriteLogLine(message);
#if DEBUG
            Trace(message);
#endif
        }

        public static void Trace(string message) {
#if DEBUG
            ((ConsoleDocker)console.Control).Trace(message);
#endif
        }
        public static void Trace(string message, bool addLineBreak) {
#if DEBUG
            ((ConsoleDocker)console.Control).Trace(message, addLineBreak);
#endif
        }
        public static void ClearTrace() {
#if DEBUG
            ((ConsoleDocker)console.Control).Clear();
#endif
        }

        public static void TraceInHeader(string message) {
#if DEBUG
            ((ConsoleDocker)console.Control).TraceInHeader(message);
#endif
        }

//actions
        [BeginAction("gztogglemodels")]
        private static void toggleModels() {
            General.Settings.GZDrawModels = !General.Settings.GZDrawModels;
            General.MainWindow.DisplayStatus(StatusType.Action, "MD3 models rendering is " + (General.Settings.GZDrawModels ? "ENABLED" : "DISABLED"));
            General.MainWindow.RedrawDisplay();
            General.MainWindow.UpdateGZDoomPannel();
        }

        [BeginAction("gztogglelights")]
        private static void toggleLights() {
            General.Settings.GZDrawLights = !General.Settings.GZDrawLights;
            General.MainWindow.DisplayStatus(StatusType.Action, "Dynamic lights rendering is " + (General.Settings.GZDrawLights ? "ENABLED" : "DISABLED"));
            General.MainWindow.RedrawDisplay();
            General.MainWindow.UpdateGZDoomPannel();
        }

        [BeginAction("gztogglelightsanimation")]
        private static void toggleLightsAnimation() {
            General.Settings.GZAnimateLights = !General.Settings.GZAnimateLights;
            General.MainWindow.DisplayStatus(StatusType.Action, "Dynamic lights animation is " + (General.Settings.GZAnimateLights ? "ENABLED" : "DISABLED"));
            General.MainWindow.RedrawDisplay();
            General.MainWindow.UpdateGZDoomPannel();
        }

        [BeginAction("gztogglefog")]
        private static void toggleFog() {
            General.Settings.GZDrawFog = !General.Settings.GZDrawFog;
            General.MainWindow.DisplayStatus(StatusType.Action, "Fog rendering is " + (General.Settings.GZDrawFog ? "ENABLED" : "DISABLED"));
            General.MainWindow.RedrawDisplay();
            General.MainWindow.UpdateGZDoomPannel();
        }

        [BeginAction("gzdrawselectedmodelsonly")]
        private static void toggleDrawSelectedModelsOnly() {
            General.Settings.GZDrawSelectedModelsOnly = !General.Settings.GZDrawSelectedModelsOnly;
            General.MainWindow.DisplayStatus(StatusType.Action, "Rendering " + (General.Settings.GZDrawSelectedModelsOnly ? "only selected" : "all") + " models.");
            General.MainWindow.RedrawDisplay();
            General.MainWindow.UpdateGZDoomPannel();
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
            General.MainWindow.UpdateGZDoomPannel();
        }
    }
}
