using System;
using CodeImp.DoomBuilder.Map;
using SlimDX;
using CodeImp.DoomBuilder.Geometry;

namespace CodeImp.DoomBuilder.VisualModes
{
	public class VisualVertexPair
	{
		private VisualVertex v1;
		private VisualVertex v2;

		public VisualVertex[] Vertices { get { return new VisualVertex[] { v1, v2 }; } }
		public VisualVertex Vertex1 { get { return v1; } }
		public VisualVertex Vertex2 { get { return v2; } }
		public bool Changed { set { v1.Changed = true; v2.Changed = true; } }

		public VisualVertexPair(VisualVertex v1, VisualVertex v2) {
			if(v1.CeilingVertex == v2.CeilingVertex)
				throw new Exception("VisualVertexPair: both verts have the same alignment! We cannot tolerate this!");

			this.v1 = v1;
			this.v2 = v2;
		}

		public void Update() {
			if(v1.Changed) v1.Update();
			if(v2.Changed) v2.Update();
		}

		public void Deselect() {
			v1.Selected = false;
			v2.Selected = false;
		}
	}

	public abstract class VisualVertex : IVisualPickable, IComparable<VisualVertex>
	{
		//Variables
		protected Vertex vertex;
		private Matrix position;
		private float cameradistance;
		protected bool selected;
		protected bool changed;
		protected bool ceilingVertex;
		protected bool haveOffset;

		//Properties
		internal Matrix Position { get { return position; } }
		public Vertex Vertex { get { return vertex; } }
		public bool Selected { get { return selected; } set { selected = value; } }
		public bool Changed { get { return changed; } set { changed |= value; } }
		public bool CeilingVertex { get { return ceilingVertex; } }
		public bool HaveHeightOffset { get { return haveOffset; } }

		public VisualVertex(Vertex v, bool ceilingVertex) {
			vertex = v;
			position = Matrix.Identity;
			this.ceilingVertex = ceilingVertex;
		}

		// This sets the distance from the camera
		internal void CalculateCameraDistance(Vector2D campos) {
			cameradistance = Vector2D.DistanceSq(vertex.Position, campos);
		}

		public void SetPosition(Vector3D pos) {
			position = Matrix.Translation(pos.x, pos.y, pos.z);
		}

		public virtual void Update() { }

		/// <summary>
		/// This is called when the thing must be tested for line intersection. This should reject
		/// as fast as possible to rule out all geometry that certainly does not touch the line.
		/// </summary>
		public virtual bool PickFastReject(Vector3D from, Vector3D to, Vector3D dir) {
			return false;
		}

		/// <summary>
		/// This is called when the thing must be tested for line intersection. This should perform
		/// accurate hit detection and set u_ray to the position on the ray where this hits the geometry.
		/// </summary>
		public virtual bool PickAccurate(Vector3D from, Vector3D to, Vector3D dir, ref float u_ray) {
			return false;
		}

		/// <summary>
		/// This sorts things by distance from the camera. Farthest first.
		/// </summary>
		public int CompareTo(VisualVertex other) {
			return Math.Sign(other.cameradistance - this.cameradistance);
		}
	}
}
