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
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections.ObjectModel;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Rendering;
using SlimDX.Direct3D9;
using SlimDX;
#endregion

namespace CodeImp.DoomBuilder.Map
{
    public struct Lights
    {
        #region ================== Constants
        #endregion

        #region ================== Variables

        // Properties
        public PixelColor color;
        public UInt16 tag;

        #endregion

        #region ================== Constructor / Disposer

        public Lights(byte r, byte g, byte b, UInt16 tag)
        {
            this.color.r = r;
            this.color.g = g;
            this.color.b = b;
            this.color.a = 255;
            this.tag = tag;
        }

        #endregion

        #region ================== Methods

        public int GetColor()
        {
            return GetFactor(this).ToInt();
        }

        public int GetColor(Lights light)
        {
            return GetFactor(light).ToInt();
        }

        public int GetTopColor(Sidedef s)
        {
            if (s.Line.IsFlagSet("67108864"))
                return GetColor(s.Sector.LowerColor);

            return GetColor(s.Sector.TopColor);
        }

        public int GetLowerColor(Sidedef s)
        {
            //if (s.Line.IsFlagSet("67108864"))
                //return s.Sector.TopColor.color.ToInt();

            return GetColor(s.Sector.LowerColor);
        }

        public int UnpegUpperLight(Sidedef s)
        {
            int height;
            Lights c = new Lights();
            int sh1;
            int sh2;
            Sector front;
            Sector back;
            float r1, g1, b1;
            float r2, g2, b2;
            Lights ltop, lbottom;

            if (s.Line.IsFlagSet("67108864"))
            {
                ltop = s.Sector.LowerColor;
                lbottom = s.Sector.TopColor;
            }
            else
            {
                ltop = s.Sector.TopColor;
                lbottom = s.Sector.LowerColor;
            }

            ltop.color = GetFactor(ltop);
            lbottom.color = GetFactor(lbottom);

            if (s.Line.IsFlagSet("2097152"))
                return lbottom.color.ToInt();

            if (s.IsFront)
            {
                front = s.Line.Front.Sector;
                back = s.Line.Back.Sector;
            }
            else
            {
                front = s.Line.Back.Sector;
                back = s.Line.Front.Sector;
            }

            height = front.CeilHeight - front.FloorHeight;

            if (height == 0)
                return lbottom.color.ToInt();

            sh1 = back.CeilHeight - front.FloorHeight;
            sh2 = front.CeilHeight - back.CeilHeight;

            r1 = ltop.color.r;
            g1 = ltop.color.g;
            b1 = ltop.color.b;

            r2 = lbottom.color.r;
            g2 = lbottom.color.g;
            b2 = lbottom.color.b;

            r1 = ((r1 / height) * sh1);
            g1 = ((g1 / height) * sh1);
            b1 = ((b1 / height) * sh1);

            r2 = ((r2 / height) * sh2);
            g2 = ((g2 / height) * sh2);
            b2 = ((b2 / height) * sh2);

            c.color.r = (byte)Math.Min((int)(r1 + r2), (int)255);
            c.color.g = (byte)Math.Min((int)(g1 + g2), (int)255);
            c.color.b = (byte)Math.Min((int)(b1 + b2), (int)255);
            c.color.a = 255;

            return c.color.ToInt();
        }

        public int UnpegLowerLight(Sidedef s)
        {
            int height;
            Lights c = new Lights();
            int sh1;
            int sh2;
            Sector front;
            Sector back;
            float r1, g1, b1;
            float r2, g2, b2;
            Lights ltop, lbottom;

            //if (s.Line.IsFlagSet("67108864"))
            //{
            //    ltop = s.Sector.LowerColor;
            //    lbottom = s.Sector.TopColor;
            //}
            //else
            //{
                ltop = s.Sector.TopColor;
                lbottom = s.Sector.LowerColor;
            //}

            ltop.color = GetFactor(ltop);
            lbottom.color = GetFactor(lbottom);

            if (s.Line.IsFlagSet("4194304"))
                return ltop.color.ToInt();

            if (s.IsFront)
            {
                front = s.Line.Front.Sector;
                back = s.Line.Back.Sector;
            }
            else
            {
                front = s.Line.Back.Sector;
                back = s.Line.Front.Sector;
            }

            height = front.CeilHeight - front.FloorHeight;

            if (height == 0)
                return ltop.color.ToInt();

            sh1 = back.FloorHeight - front.FloorHeight;
            sh2 = front.CeilHeight - back.FloorHeight;

            r1 = ltop.color.r;
            g1 = ltop.color.g;
            b1 = ltop.color.b;

            r2 = lbottom.color.r;
            g2 = lbottom.color.g;
            b2 = lbottom.color.b;

            r1 = ((r1 / height) * sh1);
            g1 = ((g1 / height) * sh1);
            b1 = ((b1 / height) * sh1);

            r2 = ((r2 / height) * sh2);
            g2 = ((g2 / height) * sh2);
            b2 = ((b2 / height) * sh2);

            c.color.r = (byte)Math.Min((int)(r1 + r2), (int)255);
            c.color.g = (byte)Math.Min((int)(g1 + g2), (int)255);
            c.color.b = (byte)Math.Min((int)(b1 + b2), (int)255);
            c.color.a = 255;

            return c.color.ToInt();
        }

        private static bool PreciseCmp(float f1, float f2)
        {
            float precision = 0.00001f;
            if (((f1 - precision) < f2) &&
                ((f1 + precision) > f2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static int[] GetHSV(PixelColor color)
        {
            int[] hsv = new int[3];
            byte r = color.r;
            byte g = color.g;
            byte b = color.b;
            int min = r;
            int max = r;
            float delta = 0.0f;
            float j = 0.0f;
            float x = 0.0f;
            float xr = 0.0f;
            float xg = 0.0f;
            float xb = 0.0f;
            float sum = 0.0f;

            if (g < min) min = g;
            if (b < min) min = b;

            if (g > max) max = g;
            if (b > max) max = b;

            delta = ((float)max / 255.0f);

            if (PreciseCmp(delta, 0.0f))
                delta = 0;
            else
            {
                j = ((delta - ((float)min / 255.0f)) / delta);
            }

            if (!PreciseCmp(j, 0.0f))
            {
                xr = ((float)r / 255.0f);

                if (!PreciseCmp(xr, delta))
                {
                    xg = ((float)g / 255.0f);

                    if (!PreciseCmp(xg, delta))
                    {
                        xb = ((float)b / 255.0f);

                        if (PreciseCmp(xb, delta))
                        {
                            sum = ((((delta - xg) / (delta - (min / 255.0f))) + 4.0f) -
                                ((delta - xr) / (delta - (min / 255.0f))));
                        }
                    }
                    else
                    {
                        sum = ((((delta - xr) / (delta - (min / 255.0f))) + 2.0f) -
                            ((delta - (b / 255.0f)) / (delta - (min / 255.0f))));
                    }
                }
                else
                {
                    sum = (((delta - (b / 255.0f))) / (delta - (min / 255.0f))) -
                        ((delta - (g / 255.0f)) / (delta - (min / 255.0f)));
                }

                x = (sum * 60.0f);

                if (x < 0)
                    x += 360.0f;
            }
            else
                j = 0.0f;

            hsv[0] = (int)((x / 360.0f) * 255.0f);
            hsv[1] = (int)(j * 255.0f);
            hsv[2] = (int)(delta * 255.0f);

            return hsv;
        }

        private static PixelColor GetRGB(int[] hsv)
        {
            float x = 0.0f;
            float j = 0.0f;
            float i = 0.0f;
            int table = 0;
            float xr = 0.0f;
            float xg = 0.0f;
            float xb = 0.0f;
            int h = hsv[0];
            int s = hsv[1];
            int v = hsv[2];
            PixelColor color = new PixelColor();

            j = (h / 255.0f) * 360.0f;

            if (360.0f <= j)
                j -= 360.0f;

            x = (s / 255.0f);
            i = (v / 255.0f);

            if (!PreciseCmp(x, 0.0f))
            {
                table = (int)(j / 60.0f);
                if (table < 6)
                {
                    float t = (j / 60.0f);
                    switch (table)
                    {
                        case 0:
                            xr = i;
                            xg = ((1.0f - ((1.0f - (t - (float)table)) * x)) * i);
                            xb = ((1.0f - x) * i);
                            break;
                        case 1:
                            xr = ((1.0f - (x * (t - (float)table))) * i);
                            xg = i;
                            xb = ((1.0f - x) * i);
                            break;
                        case 2:
                            xr = ((1.0f - x) * i);
                            xg = i;
                            xb = ((1.0f - ((1.0f - (t - (float)table)) * x)) * i);
                            break;
                        case 3:
                            xr = ((1.0f - x) * i);
                            xg = ((1.0f - (x * (t - (float)table))) * i);
                            xb = i;
                            break;
                        case 4:
                            xr = ((1.0f - ((1.0f - (t - (float)table)) * x)) * i);
                            xg = ((1.0f - x) * i);
                            xb = i;
                            break;
                        case 5:
                            xr = i;
                            xg = ((1.0f - x) * i);
                            xb = ((1.0f - (x * (t - (float)table))) * i);
                            break;
                    }
                }
            }
            else
                xr = xg = xb = i;

            color.r = (byte)(xr * 255.0f);
            color.g = (byte)(xg * 255.0f);
            color.b = (byte)(xb * 255.0f);
            color.a = 255;

            return color;
        }

        private static PixelColor GetFactor(Lights light)
        {
            float factor = 1.0f + (General.Settings.LightIntensity / 10.0f);
            int[] hsv = GetHSV(light.color);
            hsv[2] = Math.Min((int)((float)hsv[2] * factor), 255);
            return GetRGB(hsv);
        }

        public void SetIntensity(float value)
        {
            float factor = 1.0f + value;
            int[] hsv = GetHSV(this.color);
            hsv[2] = Math.Min((int)((float)hsv[2] * factor), 255);
            this.color = GetRGB(hsv);
        }

        #endregion

    }
}
