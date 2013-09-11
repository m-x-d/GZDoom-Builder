using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Forms;
using CodeImp.DoomBuilder.Editing;
using CodeImp.DoomBuilder.Geometry;
using CodeImp.DoomBuilder.Map;
using CodeImp.DoomBuilder.Windows;

namespace CodeImp.DoomBuilder.BuilderModes.ClassicModes
{
	[EditMode(DisplayName = "Snap Map Elements to Grid",
			  SwitchAction = "snapvertstogrid",
			  AllowCopyPaste = false,
			  Optional = false,
			  Volatile = true)]
	public class SnapVerticesMode : BaseClassicMode
	{
		public SnapVerticesMode() {
			// We have no destructor
			GC.SuppressFinalize(this);
		}
		
		// Mode engages
		public override void OnEngage() {
			base.OnEngage();

			//get selection
			General.Map.Map.ClearAllMarks(false);
			General.Map.Map.MarkAllSelectedGeometry(true, false, true, false, false);
			List<Vertex> verts = General.Map.Map.GetMarkedVertices(true);

			//nothing selected?
			if (verts.Count == 0) {
				//check things
				List<Thing> things = General.Map.Map.GetMarkedThings(true);
				if (things.Count == 0) {
					General.Interface.DisplayStatus(StatusType.Warning, "Select any map element first!");
					base.OnCancel();
					General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
				} else {
					snapThings(things);
				}
			} else {
				snapVertices(verts);
			}
		}

		private void snapVertices(List<Vertex> verts) {
			// Make undo for the snapping
			General.Map.UndoRedo.CreateUndo("Snap vertices");

			int snappedCount = 0;
			List<Vertex> movedVerts = new List<Vertex>();
			List<Linedef> movedLines = new List<Linedef>();

			//snap them all!
			foreach(Vertex v in verts) {
				Vector2D pos = v.Position;
				v.SnapToGrid();

				if(v.Position.x != pos.x || v.Position.y != pos.y) {
					snappedCount++;
					movedVerts.Add(v);
					foreach(Linedef l in v.Linedefs){
						if(!movedLines.Contains(l)) movedLines.Add(l);
					}
				}
			}

			//Create blockmap
			RectangleF area = MapSet.CreateArea(General.Map.Map.Vertices);
			BlockMap<BlockEntry> blockmap = new BlockMap<BlockEntry>(area);
			blockmap.AddVerticesSet(General.Map.Map.Vertices);

			//merge overlapping vertices using teh power of BLOCKMAP!!!11
			BlockEntry block;
			foreach (Vertex v in movedVerts) {
				block = blockmap.GetBlockAt(v.Position);
				if (block == null) continue;

				foreach (Vertex blockVert in block.Vertices) {
					if(!blockVert.IsDisposed && blockVert.Index != v.Index && blockVert.Position == v.Position) {
						foreach(Linedef l in blockVert.Linedefs) 
							if(!movedLines.Contains(l)) movedLines.Add(l);
						v.Join(blockVert);
						break;
					}
				}
			}

			// Update cached values of lines because we may need their length/angle
			General.Map.Map.Update(true, false);

			General.Map.Map.BeginAddRemove();
			MapSet.RemoveLoopedLinedefs(movedLines);
			MapSet.JoinOverlappingLines(movedLines);
			General.Map.Map.EndAddRemove();

			//get changed sectors
			List<Sector> changedSectors = new List<Sector>();
			foreach(Linedef l in movedLines) {
				if(l == null || l.IsDisposed) continue;
				if(l.Front != null && l.Front.Sector != null && !changedSectors.Contains(l.Front.Sector))
					changedSectors.Add(l.Front.Sector);
				if(l.Back != null && l.Back.Sector != null && !changedSectors.Contains(l.Back.Sector))
					changedSectors.Add(l.Back.Sector);
			}

			// Now update area of sectors
			General.Map.Map.Update(false, true);

			//fix invalid sectors
			foreach (Sector s in changedSectors) {
				if(s.BBox.IsEmpty) {
					s.Dispose();
				}else if (s.Sidedefs.Count < 3) {
					bool merged = false;
					foreach(Sidedef side in s.Sidedefs) {
						if(side.Other != null && side.Other.Sector != null) {
							s.Join(side.Other.Sector);
							merged = true;
							break;
						}
					}

					//oh well, I don't know what else I can do here...
					if(!merged) s.Dispose();
				}
			}

			//done
			General.Interface.DisplayStatus(StatusType.Info, "Snapped " + snappedCount + " vertices.");
			MessageBox.Show("Snapped " + snappedCount + " vertices." + Environment.NewLine + "It's a good idea to run Map Analysis Mode now.");
			base.OnAccept();
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
		}

		private void snapThings(List<Thing> things) {
			// Make undo for the snapping
			General.Map.UndoRedo.CreateUndo("Snap things");

			int snappedCount = 0;

			//snap them all!
			foreach(Thing t in things) {
				Vector2D pos = t.Position;
				t.SnapToGrid();

				if(t.Position.x != pos.x || t.Position.y != pos.y)
					snappedCount++;
			}

			//done
			General.Interface.DisplayStatus(StatusType.Info, "Snapped " + snappedCount + " things.");
			base.OnAccept();
			General.Editing.ChangeMode(General.Editing.PreviousStableMode.Name);
		}

		// Disenagaging
		public override void OnDisengage() {
			base.OnDisengage();
			Cursor.Current = Cursors.AppStarting;

			if (!cancelled) {
				// Update cached values
				General.Map.Map.Update();
				// Map is changed
				General.Map.IsChanged = true;
			}

			// Done
			Cursor.Current = Cursors.Default;
		}
	}
}
