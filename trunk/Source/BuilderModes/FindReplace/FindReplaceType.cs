
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
using System.Drawing;
using CodeImp.DoomBuilder.Editing;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal class FindReplaceType
	{
		#region ================== Constants

		#endregion

		#region ================== Variables

		protected FindReplaceAttribute attribs;
		
		#endregion

		#region ================== Properties

		public FindReplaceAttribute Attributes { get { return attribs; } }

		#endregion

		#region ================== Constructor / Destructor

		// Constructor
		public FindReplaceType()
		{
			// Initialize
			object[] attrs = this.GetType().GetCustomAttributes(typeof(FindReplaceAttribute), true);
			attribs = (FindReplaceAttribute)attrs[0];
		}

		// Destructor
		~FindReplaceType()
		{
		}

		#endregion

		#region ================== Methods

		// This is called when the browse button is pressed
		public virtual string Browse(string initialvalue)
		{
			return "";
		}
		
		
		// This is called to perform a search (and replace)
		// Must return a list of items to show in the results list
		// replacewith is null when not replacing
		public virtual FindReplaceObject[] Find(string value, bool withinselection, string replacewith, bool keepselection)
		{
			return new FindReplaceObject[0];
		}

		// String representation
		public override string ToString()
		{
			return attribs.DisplayName;
		}

		// This is called when a specific object is selected from the list
		public virtual void ObjectSelected(FindReplaceObject obj)
		{
		}
		
		// This is called for rendering
		public virtual void PlotSelection(IRenderer2D renderer, FindReplaceObject[] selection)
		{
		}

		// This is called for rendering
		public virtual void RenderThingsSelection(IRenderer2D renderer, FindReplaceObject[] selection)
		{
		}

		// This is called for rendering
		public virtual void RenderOverlaySelection(IRenderer2D renderer, FindReplaceObject[] selection)
		{
		}
		
		#endregion
	}
}
