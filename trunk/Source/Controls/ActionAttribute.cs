
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
	/// <summary>
	/// This binds a method to an action.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, Inherited=true, AllowMultiple=true)]
	public class ActionAttribute : Attribute
	{
		#region ================== Variables

		// The action to bind to
		private string action;
		private bool baseaction;
		
		#endregion

		#region ================== Properties

		/// <summary>
		/// Set to true to indicate this is a core Doom Builder action when used within a plugin.
		/// </summary>
		public bool BaseAction { get { return baseaction; } set { baseaction = value; } }

		#endregion

		#region ================== Constructor / Disposer

		/// <summary>
		/// This binds a method to an action.
		/// </summary>
		/// <param name="action">The action name as defined in Actions.cfg resource.</param>
		public ActionAttribute(string action)
		{
			// Initialize
			this.action = action;
			this.baseaction = false;
		}

		#endregion

		#region ================== Methods

		// This makes the proper name
		public string GetFullActionName(Assembly asm)
		{
			string asmname;
			
			if(baseaction)
				asmname = General.ThisAssembly.GetName().Name.ToLowerInvariant();
			else
				asmname = asm.GetName().Name.ToLowerInvariant();
			
			return asmname + "_" + action;
		}

		#endregion
		
		#region ================== Static Methods

		// This makes the proper name
		public string GetFullActionName(Assembly asm, bool baseaction, string actionname)
		{
			string asmname;

			if(baseaction)
				asmname = General.ThisAssembly.GetName().Name.ToLowerInvariant();
			else
				asmname = asm.GetName().Name.ToLowerInvariant();

			return asmname + "_" + actionname;
		}

		// This binds all methods marked with this attribute
		internal static void BindMethods(Type type)
		{
			// Bind static methods
			BindMethods(null, type);
		}

		// This binds all methods marked with this attribute
		internal static void BindMethods(object obj)
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
			string actionname;
			
			if(obj == null)
				General.WriteLogLine("Binding static action methods for class " + type.Name + "...");
			else
				General.WriteLogLine("Binding action methods for " + type.Name + " object...");
			
			// Go for all methods on obj
			methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
			foreach(MethodInfo m in methods)
			{
				// Check if the method has this attribute
				attrs = (ActionAttribute[])m.GetCustomAttributes(typeof(ActionAttribute), true);
				
				// Go for all attributes
				foreach(ActionAttribute a in attrs)
				{
					// Create a delegate for this method
					del = (ActionDelegate)Delegate.CreateDelegate(typeof(ActionDelegate), obj, m);
					
					// Make proper name
					actionname = a.GetFullActionName(type.Assembly);
					
					// Bind method to action
					if(General.Actions.Exists(actionname))
						General.Actions[actionname].Bind(del);
					else
						throw new ArgumentException("Could not bind " + m.ReflectedType.Name + "." + m.Name + " to action \"" + actionname + "\", that action does not exist! Refer to, or edit Actions.cfg for all available application actions.");
				}
			}
		}
		
		// This binds a delegate manually
		internal static void BindDelegate(Assembly asm, ActionDelegate d, ActionAttribute a)
		{
			string actionname;
			
			// Make proper name
			actionname = a.GetFullActionName(asm);
			
			// Bind delegate to action
			if(General.Actions.Exists(actionname))
				General.Actions[actionname].Bind(d);
			else
				General.WriteLogLine("WARNING: Could not bind delegate for " + d.Method.Name + " to action \"" + a.action + "\" (" + actionname + "), that action does not exist! Refer to, or edit Actions.cfg for all available application actions.");
		}
		
		// This unbinds all methods marked with this attribute
		internal static void UnbindMethods(Type type)
		{
			// Unbind static methods
			UnbindMethods(null, type);
		}

		// This unbinds all methods marked with this attribute
		internal static void UnbindMethods(object obj)
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
			string actionname;

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

					// Make proper name
					actionname = a.GetFullActionName(type.Assembly);

					// Unbind method from action
					General.Actions[actionname].Unbind(del);
				}
			}
		}

		// This unbinds a delegate manually
		internal static void UnbindDelegate(Assembly asm, ActionDelegate d, ActionAttribute a)
		{
			string actionname;

			// Make proper name
			actionname = a.GetFullActionName(asm);

			// Unbind delegate to action
			General.Actions[actionname].Unbind(d);
		}
		
		#endregion
	}
}
