
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
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Reflection;

#endregion

namespace CodeImp.DoomBuilder.Controls
{
	[AttributeUsage(AttributeTargets.Method, Inherited=true, AllowMultiple=true)]
	internal class ActionAttribute : Attribute
	{
		#region ================== Variables

		// The action to bind to
		private string action;
		
		#endregion

		#region ================== Constructor / Disposer

		// Constructor
		public ActionAttribute(string action)
		{
			// Initialize
			this.action = action;
		}

		#endregion

		#region ================== Static Methods

		// This binds all methods marked with this attribute
		public static void BindMethods(Type type)
		{
			// Bind static methods
			BindMethods(null, type);
		}

		// This binds all methods marked with this attribute
		public static void BindMethods(object obj)
		{
			// Bind instance methods
			BindMethods(obj, obj.GetType());
		}

		// This binds all methods marked with this attribute
		private static void BindMethods(object obj, Type type)
		{
			MethodInfo[] methods;
			ActionAttribute[] attrs;
			ActionDelegate del;

			if(obj == null)
				General.WriteLogLine("Binding static action methods for class " + type.Name + "...");
			else
				General.WriteLogLine("Binding action methods for " + type.Name + " object...");
			
			// Go for all methods on obj
			methods = type.GetMethods();
			foreach(MethodInfo m in methods)
			{
				// Check if the method has this attribute
				attrs = (ActionAttribute[])m.GetCustomAttributes(typeof(ActionAttribute), true);
				
				// Go for all attributes
				foreach(ActionAttribute a in attrs)
				{
					// Create a delegate for this method
					del = (ActionDelegate)Delegate.CreateDelegate(typeof(ActionDelegate), obj, m);
					
					// Bind method to action
					if(General.Actions.Exists(a.action))
						General.Actions[a.action].Bind(del);
					else
						throw new ArgumentException("Could not bind " + m.ReflectedType.Name + "." + m.Name + " to action \"" + a.action + "\", that action does not exist! Refer to, or edit Actions.cfg for all available application actions.");
				}
			}
		}

		// This unbinds all methods marked with this attribute
		public static void UnbindMethods(Type type)
		{
			// Unbind static methods
			UnbindMethods(null, type);
		}

		// This unbinds all methods marked with this attribute
		public static void UnbindMethods(object obj)
		{
			// Unbind instance methods
			UnbindMethods(obj, obj.GetType());
		}
		
		// This unbinds all methods marked with this attribute
		private static void UnbindMethods(object obj, Type type)
		{
			MethodInfo[] methods;
			ActionAttribute[] attrs;
			ActionDelegate del;

			if(obj == null)
				General.WriteLogLine("Unbinding static action methods for class " + type.Name + "...");
			else
				General.WriteLogLine("Unbinding action methods for " + type.Name + " object...");

			// Go for all methods on obj
			methods = type.GetMethods();
			foreach(MethodInfo m in methods)
			{
				// Check if the method has this attribute
				attrs = (ActionAttribute[])m.GetCustomAttributes(typeof(ActionAttribute), true);

				// Go for all attributes
				foreach(ActionAttribute a in attrs)
				{
					// Create a delegate for this method
					del = (ActionDelegate)Delegate.CreateDelegate(typeof(ActionDelegate), obj, m);

					// Unbind method from action
					General.Actions[a.action].Unbind(del);
				}
			}
		}
		
		#endregion
	}
}
