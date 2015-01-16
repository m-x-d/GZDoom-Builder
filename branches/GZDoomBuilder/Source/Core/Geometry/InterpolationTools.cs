using System;

namespace CodeImp.DoomBuilder.Geometry
{
	
	
	public static class InterpolationTools
	{
		public enum Mode
		{
			Linear,
			EaseInOutSine,
			EaseInSine,
			EaseOutSine,
		}
		
		public static int Interpolate(int val1, int val2, float delta, Mode mode)
		{
			switch (mode)
			{
				case Mode.Linear: return Linear(val1, val2, delta);
				case Mode.EaseInSine: return EaseInSine(val1, val2, delta);
				case Mode.EaseOutSine: return EaseOutSine(val1, val2, delta);
				case Mode.EaseInOutSine: return EaseInOutSine(val1, val2, delta);
				default: throw new NotImplementedException("InterpolationTools.Interpolate: '" + mode + "' mode is not supported!");
			}
		}
		
		//Based on Robert Penner's original easing equations (http://www.robertpenner.com/easing/)
		public static int Linear(int val1, int val2, float delta)
		{
			return (int)(delta * val2 + (1.0f - delta) * val1);
		}
		
		/**
		 * Easing equation function for a sinusoidal (sin(t)) easing in: accelerating from zero velocity.
		 */
		public static int EaseInSine(int val1, int val2, float delta) 
		{
			float f_val1 = val1;
			float f_val2 = val2 - f_val1;
			return (int)(-f_val2 * Math.Cos(delta * Angle2D.PIHALF) + f_val2 + f_val1);
		}

		/**
		 * Easing equation function for a sinusoidal (sin(t)) easing out: decelerating from zero velocity.
		 */
		public static int EaseOutSine(int val1, int val2, float delta) 
		{
			float f_val1 = val1;
			float f_val2 = val2;
			return (int)((f_val2 - f_val1) * Math.Sin(delta * Angle2D.PIHALF) + f_val1);
		}

		/**
		 * Easing equation function for a sinusoidal (sin(t)) easing in/out: acceleration until halfway, then deceleration.
		 */
		public static int EaseInOutSine(int val1, int val2, float delta) 
		{
			float f_val1, f_val2;
			if(val1 > val2)
			{
				f_val1 = val2;
				f_val2 = val1;
				delta = 1.0f - delta;
			} 
			else 
			{
				f_val1 = val1;
				f_val2 = val2;
			}
			return (int)(-f_val2 / 2.0f * (Math.Cos(Angle2D.PI * delta) - 1.0f) + f_val1);
		}
	}
}
