
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
using CodeImp.DoomBuilder.Windows;
using CodeImp.DoomBuilder.IO;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Rendering;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Types;
using CodeImp.DoomBuilder.Config;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	public class ErrorChecker
	{
		#region ================== Variables
		
		private int lastprogress;
		private int totalprogress = -1;
		
		#endregion
		
		#region ================== Properties
		
		public int TotalProgress { get { return totalprogress; } }
		
		#endregion
		
		#region ================== Constructor / Destructor
		
		// Constructor
		// Override this to determine and set the total progress
		public ErrorChecker()
		{
		}
		
		#endregion
		
		#region ================== Methods
		
		// Override this to run your check
		// Use a Sleep and Try/Catch to handle thread interruption
		public virtual void Run()
		{
			if(totalprogress < 0) throw new InvalidOperationException("You must set the total progress through the SetTotalProgress method before this check can be performed!");
			lastprogress = 0;
		}
		
		// This submits a result to show in the results list
		protected void SubmitResult(ErrorResult result)
		{
			
		}
		
		// This reports a change in progress
		protected void ShowProgress(int percent)
		{
			// Any change?
			if(percent > lastprogress)
			{
				if(percent < 0) percent = 0;
				if(percent > totalprogress) percent = totalprogress;
				lastprogress = percent;
			}
		}
		
		// This sets the total progress
		protected void SetTotalProgress(int totalprogress)
		{
			if(totalprogress < 0) throw new ArgumentOutOfRangeException("Total progress cannot be less than zero!");
			this.totalprogress = totalprogress;
		}
		
		#endregion
	}
}
