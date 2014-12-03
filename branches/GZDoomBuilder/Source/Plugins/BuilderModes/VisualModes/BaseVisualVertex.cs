#region ================== Namespaces

using System.Collections.Generic;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.VisualModes;

#endregion

namespace CodeImp.DoomBuilder.BuilderModes
{
	internal class BaseVisualVertex : VisualVertex, IVisualEventReceiver
	{
		#region ================== Variables

		protected BaseVisualMode mode;
		private float cageradius2;
		private Vector3D boxp1;
		private Vector3D boxp2;

		// Undo/redo
		private int undoticket;

		//updating
		private static Dictionary<BaseVisualSector, bool> updateList;

		#endregion

		// Constructor
		public BaseVisualVertex(BaseVisualMode mode, Vertex v, bool ceilingVertex)
			: base(v, ceilingVertex) 
		{

			this.mode = mode;
			cageradius2 = DEFAULT_SIZE * General.Settings.GZVertexScale3D * Angle2D.SQRT2;
			cageradius2 = cageradius2 * cageradius2;

			changed = true;
			Update();
		}

		//this updates the handle itself
		public override void Update() 
		{
			if(!changed) return;
			float z = ceilingVertex ? vertex.ZCeiling : vertex.ZFloor;

			if(!float.IsNaN(z)) 
			{
				haveOffset = true;
			} 
			else 
			{
				z = GetSectorHeight();
				haveOffset = false;
			}

			Vector3D pos = new Vector3D(vertex.Position.x, vertex.Position.y, z);
			SetPosition(pos);

			float radius = DEFAULT_SIZE * General.Settings.GZVertexScale3D;
			boxp1 = new Vector3D(pos.x - radius, pos.y - radius, pos.z - radius);
			boxp2 = new Vector3D(pos.x + radius, pos.y + radius, pos.z + radius);

			changed = false;
		}

		private void UpdateGeometry(Vertex v) 
		{
			VertexData vd = mode.GetVertexData(v);
			foreach(KeyValuePair<Sector, bool> s in vd.UpdateAlso) 
			{
				if(mode.VisualSectorExists(s.Key)) 
				{
					BaseVisualSector vs = (BaseVisualSector)mode.GetVisualSector(s.Key);
					vs.UpdateSectorGeometry(s.Value);
				}
			}
		}

		//get the most appropriate height from sectors
		private int GetSectorHeight() 
		{
			int height;

			VertexData vd = mode.GetVertexData(vertex);
			Sector[] sectors = new Sector[vd.UpdateAlso.Keys.Count];
			vd.UpdateAlso.Keys.CopyTo(sectors, 0);

			if(ceilingVertex) 
			{
				height = sectors[0].CeilHeight;
				for(int i = 1; i < sectors.Length; i++) 
				{
					if(sectors[i].CeilHeight < height)
						height = sectors[i].CeilHeight;
				}
			} 
			else 
			{
				height = sectors[0].FloorHeight;
				for(int i = 1; i < sectors.Length; i++) 
				{
					if(sectors[i].FloorHeight > height)
						height = sectors[i].FloorHeight;
				}
			}

			return height;
		}

		//mxd
		public virtual bool IsSelected() 
		{
			return selected;
		}

		#region ================== Object picking

		// This performs a fast test in object picking
		public override bool PickFastReject(Vector3D from, Vector3D to, Vector3D dir) 
		{
			float distance2 = Line2D.GetDistanceToLineSq(from, to, vertex.Position, false);
			return (distance2 <= cageradius2);
		}

		// This performs an accurate test for object picking
		public override bool PickAccurate(Vector3D from, Vector3D to, Vector3D dir, ref float u_ray) 
		{
			Vector3D delta = to - from;
			float tfar = float.MaxValue;
			float tnear = float.MinValue;

			// Ray-Box intersection code
			// See http://www.masm32.com/board/index.php?topic=9941.0

			// Check X slab
			if(delta.x == 0.0f) 
			{
				if(from.x > boxp2.x || from.x < boxp1.x) 
				{
					// Ray is parallel to the planes & outside slab
					return false;
				}
			}
			else 
			{
				float tmp = 1.0f / delta.x;
				float t1 = (boxp1.x - from.x) * tmp;
				float t2 = (boxp2.x - from.x) * tmp;
				if(t1 > t2)
					General.Swap(ref t1, ref t2);
				if(t1 > tnear)
					tnear = t1;
				if(t2 < tfar)
					tfar = t2;
				if(tnear > tfar || tfar < 0.0f) 
				{
					// Ray missed box or box is behind ray
					return false;
				}
			}

			// Check Y slab
			if(delta.y == 0.0f) 
			{
				if(from.y > boxp2.y || from.y < boxp1.y) 
				{
					// Ray is parallel to the planes & outside slab
					return false;
				}
			} 
			else 
			{
				float tmp = 1.0f / delta.y;
				float t1 = (boxp1.y - from.y) * tmp;
				float t2 = (boxp2.y - from.y) * tmp;
				if(t1 > t2)
					General.Swap(ref t1, ref t2);
				if(t1 > tnear)
					tnear = t1;
				if(t2 < tfar)
					tfar = t2;
				if(tnear > tfar || tfar < 0.0f) 
				{
					// Ray missed box or box is behind ray
					return false;
				}
			}

			// Check Z slab
			if(delta.z == 0.0f) 
			{
				if(from.z > boxp2.z || from.z < boxp1.z) 
				{
					// Ray is parallel to the planes & outside slab
					return false;
				}
			} 
			else 
			{
				float tmp = 1.0f / delta.z;
				float t1 = (boxp1.z - from.z) * tmp;
				float t2 = (boxp2.z - from.z) * tmp;
				if(t1 > t2)
					General.Swap(ref t1, ref t2);
				if(t1 > tnear)
					tnear = t1;
				if(t2 < tfar)
					tfar = t2;
				if(tnear > tfar || tfar < 0.0f) 
				{
					// Ray missed box or box is behind ray
					return false;
				}
			}

			// Set interpolation point
			u_ray = (tnear > 0.0f) ? tnear : tfar;
			return true;
		}

		#endregion

		#region ================== Unused events

		// Unused
		public virtual void OnSelectBegin() { }
		public virtual void OnEditBegin() { }
		public virtual void OnMouseMove(MouseEventArgs e) { }
		public virtual void OnChangeTargetBrightness(bool up) { }
		public virtual void OnChangeTextureOffset(int horizontal, int vertical, bool doSurfaceAngleCorrection) { }
		public virtual void OnChangeTextureScale(float incrementX, float incrementY) { }
		public virtual void OnSelectTexture() { }
		public virtual void OnCopyTexture() { }
		public virtual void OnPasteTexture() { }
		public virtual void OnCopyTextureOffsets() { }
		public virtual void OnPasteTextureOffsets() { }
		public virtual void OnTextureAlign(bool alignx, bool aligny) { }
		public virtual void OnTextureFit(bool fitWidth, bool fitHeight) { } //mxd
		public virtual void OnToggleUpperUnpegged() { }
		public virtual void OnToggleLowerUnpegged() { }
		public virtual void OnResetTextureOffset() { }
		public virtual void OnResetLocalTextureOffset() { } //mxd
		public virtual void OnProcess(float deltatime) { }
		public virtual void OnTextureFloodfill() { }
		public virtual void OnInsert() { }
		public virtual void ApplyTexture(string texture) { }
		public virtual void ApplyUpperUnpegged(bool set) { }
		public virtual void ApplyLowerUnpegged(bool set) { }
		public virtual string GetTextureName() { return ""; }
		public virtual void SelectNeighbours(bool select, bool withSameTexture, bool withSameHeight) { } //mxd

		#endregion

		#region ================== Events

		// Select or deselect
		public virtual void OnSelectEnd() 
		{
			if(this.selected) 
			{
				this.selected = false;
				mode.RemoveSelectedObject(this);
			} 
			else 
			{
				this.selected = true;
				mode.AddSelectedObject(this);
			}
		}

		// Copy properties
		public virtual void OnCopyProperties() 
		{
			BuilderPlug.Me.CopiedVertexProps = new VertexProperties(vertex);
			mode.SetActionResult("Copied vertex properties.");
		}

		// Paste properties
		public virtual void OnPasteProperties() 
		{
			if(BuilderPlug.Me.CopiedVertexProps != null) 
			{
				mode.CreateUndo("Paste vertex properties");
				mode.SetActionResult("Pasted vertex properties.");
				BuilderPlug.Me.CopiedVertexProps.Apply(vertex);
				
				//update affected sectors
				UpdateGeometry(vertex);
				changed = true;
				mode.ShowTargetInfo();
			}
		}

		//Delete key pressed - remove zoffset field
		public virtual void OnDelete() 
		{
			mode.CreateUndo("Clear vertex height offset");
			mode.SetActionResult("Cleared vertex height offset.");
			
			if(ceilingVertex) 
			{
				if(float.IsNaN(vertex.ZCeiling)) return;
				vertex.ZCeiling = float.NaN;

				//update affected sectors
				UpdateGeometry(vertex);
				changed = true;
				mode.ShowTargetInfo();
			} 
			else 
			{
				if(float.IsNaN(vertex.ZFloor)) return;
				vertex.ZFloor = float.NaN;

				//update affected sectors
				UpdateGeometry(vertex);
				changed = true;
				mode.ShowTargetInfo();
			}
		}

		// Edit button released
		public virtual void OnEditEnd() 
		{
			if(General.Interface.IsActiveWindow) 
			{
				List<Vertex> verts = mode.GetSelectedVertices();
				updateList = new Dictionary<BaseVisualSector, bool>();

				foreach(Vertex v in verts)
				{
					VertexData vd = mode.GetVertexData(v);
					foreach(KeyValuePair<Sector, bool> s in vd.UpdateAlso) 
					{
						if(mode.VisualSectorExists(s.Key)) 
						{
							BaseVisualSector vs = (BaseVisualSector)mode.GetVisualSector(s.Key);
							updateList.Add(vs, s.Value);
						}
					}
				}

				General.Interface.OnEditFormValuesChanged += Interface_OnEditFormValuesChanged;
				mode.StartRealtimeInterfaceUpdate(SelectionType.Vertices);
				General.Interface.ShowEditVertices(verts, false);
				mode.StopRealtimeInterfaceUpdate(SelectionType.Vertices);
				General.Interface.OnEditFormValuesChanged -= Interface_OnEditFormValuesChanged;

				updateList.Clear();
				updateList = null;
			}
		}

		//mxd
		private void Interface_OnEditFormValuesChanged(object sender, System.EventArgs e) 
		{
			foreach(KeyValuePair<BaseVisualSector, bool> group in updateList) 
				group.Key.UpdateSectorGeometry(group.Value);

			changed = true;
			Update();
		}

		// Raise/lower thing
		public virtual void OnChangeTargetHeight(int amount) 
		{
			if((General.Map.UndoRedo.NextUndo == null) || (General.Map.UndoRedo.NextUndo.TicketID != undoticket))
				undoticket = mode.CreateUndo("Change vertex height");

			if(ceilingVertex) 
			{
				vertex.ZCeiling = (float.IsNaN(vertex.ZCeiling) ? GetSectorHeight() + amount : vertex.ZCeiling + amount);
				mode.SetActionResult("Changed vertex height to " + vertex.ZCeiling + ".");
			} 
			else 
			{
				vertex.ZFloor = (float.IsNaN(vertex.ZFloor) ? GetSectorHeight() + amount : vertex.ZFloor + amount);
				mode.SetActionResult("Changed vertex height to " + vertex.ZFloor + ".");
			}

			// Update what must be updated
			UpdateGeometry(vertex);
			changed = true;
		}

		#endregion
	}
}
