#region ================== Namespaces

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Actions;
using CodeImp.DoomBuilder.Plugins;
using CodeImp.DoomBuilder.VisualModes;
using CodeImp.DoomBuilder.Windows;

#endregion

namespace CodeImp.DoomBuilder.BuilderEffects
{
	public class BuilderPlug : Plug
	{
		// Static instance
		private static BuilderPlug me;

		// Main objects
		private MenusForm menusForm;
		private Form form;

		private Point formLocation; //used to keep form's location constant

		public override string Name { get { return "Builder Effects"; } }
		public static BuilderPlug Me { get { return me; } }

		// When plugin is initialized
		public override void OnInitialize() 
		{
			// Setup
			base.OnInitialize();
			me = this;

			// Load menus form
			menusForm = new MenusForm();

			General.Actions.BindMethods(this);
		}

		// Disposer
		public override void Dispose() 
		{
			// Not already disposed?
			if(!IsDisposed) 
			{
				menusForm.Unregister();
				menusForm.Dispose();
				menusForm = null;

				// Done
				me = null;
				base.Dispose();
			}
		}

		public override void OnMapNewEnd() 
		{
			base.OnMapNewEnd();
			menusForm.Register();
		}

		public override void OnMapOpenEnd() 
		{
			base.OnMapOpenEnd();
			menusForm.Register();
		}

		public override void OnMapCloseEnd() 
		{
			base.OnMapCloseEnd();
			menusForm.Unregister();
		}

		public override void OnReloadResources() 
		{
			base.OnReloadResources();
			menusForm.Register();
		}

		//actions
		[BeginAction("applyjitter")]
		private void ApplyJitterTransform() 
		{
			if(General.Editing.Mode == null) return;
			string currentModeName = General.Editing.Mode.GetType().Name;

			//display one of colorPickers or tell the user why we can't do that
			if(currentModeName == "ThingsMode") 
			{
				if(General.Map.Map.SelectedThingsCount == 0) 
				{
					General.Interface.DisplayStatus(StatusType.Warning, "Select some things first!");
					return;
				}
				form = new JitterThingsForm(currentModeName);
			} 
			else if(currentModeName == "SectorsMode") 
			{
				if(General.Map.Map.SelectedSectorsCount == 0) 
				{
					General.Interface.DisplayStatus(StatusType.Warning, "Select some sectors first!");
					return;
				}
				form = new JitterSectorsForm(currentModeName);
			} 
			else if(currentModeName == "LinedefsMode")
			{
				if(General.Map.Map.SelectedLinedefsCount == 0) 
				{
					General.Interface.DisplayStatus(StatusType.Warning, "Select some linedefs first!");
					return;
				}
				form = new JitterVerticesForm(currentModeName);
			} 
			else if(currentModeName == "VerticesMode")
			{
				if(General.Map.Map.SelectedVerticessCount == 0) 
				{
					General.Interface.DisplayStatus(StatusType.Warning, "Select some vertices first!");
					return;
				}
				form = new JitterVerticesForm(currentModeName);
			} 
			else if(currentModeName == "BaseVisualMode") 
			{
				//no visual things selected in visual mode?
				if(((VisualMode)General.Editing.Mode).GetSelectedVisualThings(true).Count == 0) 
				{
					//check selected geometry
					List<VisualGeometry> list = ((VisualMode)General.Editing.Mode).GetSelectedSurfaces();
					if(list.Count > 0) 
					{
						foreach(VisualGeometry vg in list) 
						{
							if(vg.GeometryType == VisualGeometryType.CEILING 
								|| vg.GeometryType == VisualGeometryType.FLOOR) 
							{
								form = new JitterSectorsForm(currentModeName);
								break;
							}
						}

						if(form == null) form = new JitterVerticesForm(currentModeName);
					} 
					else 
					{
						General.Interface.DisplayStatus(StatusType.Warning, "Select some things, sectors or surfaces first!");
						return;
					}
				} 
				else 
				{
					form = new JitterThingsForm(currentModeName);
				}
			} 
			else //wrong mode
			{ 
				General.Interface.DisplayStatus(StatusType.Warning, "Switch to Sectors, Things, Vertices, Linedefs or Visual mode first!");
				return;
			}

			//position and show form
			if(formLocation.X == 0 && formLocation.Y == 0)
			{
				Size displaySize = General.Interface.Display.Size;
				Point displayLocation = General.Interface.Display.LocationAbs;
				formLocation = new Point(displayLocation.X + displaySize.Width - form.Width - 16, displayLocation.Y + 16);
			}

			form.Location = formLocation;
			form.FormClosed += form_FormClosed;
			form.ShowDialog(Form.ActiveForm);
		}

//events
		private void form_FormClosed(object sender, FormClosedEventArgs e) 
		{
			formLocation = form.Location;
			form.Dispose();
			form = null;
		}
	}
}
