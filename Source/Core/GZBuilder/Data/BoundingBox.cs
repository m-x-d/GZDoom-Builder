using CodeImp.DoomBuilder.Rendering;
using SlimDX;

namespace CodeImp.DoomBuilder.GZBuilder.Data
{
	public struct BoundingBoxSizes
	{
		public short MinX;
		public short MaxX;
		public short MinY;
		public short MaxY;
		public short MinZ;
		public short MaxZ;

		//we need some reference here
		public BoundingBoxSizes(WorldVertex v) 
		{
			MinX = MaxX = (short)v.x;
			MinY = MaxY = (short)v.y;
			MinZ = MaxZ = (short)v.z;
		}
	}

	public static class BoundingBoxTools
	{
		//this creates array of vectors resembling bounding box
		public static Vector3[] CalculateBoundingBox(BoundingBoxSizes bbs) 
		{
			//center
			Vector3 v0 = new Vector3(bbs.MinX + (bbs.MaxX - bbs.MinX) / 2, bbs.MinY + (bbs.MaxY - bbs.MinY) / 2, bbs.MinZ + (bbs.MaxZ - bbs.MinZ) / 2);

			//corners
			Vector3 v1 = new Vector3(bbs.MinX, bbs.MinY, bbs.MinZ);
			Vector3 v2 = new Vector3(bbs.MaxX, bbs.MinY, bbs.MinZ);
			Vector3 v3 = new Vector3(bbs.MinX, bbs.MaxY, bbs.MinZ);
			Vector3 v4 = new Vector3(bbs.MaxX, bbs.MaxY, bbs.MinZ);
			Vector3 v5 = new Vector3(bbs.MinX, bbs.MinY, bbs.MaxZ);
			Vector3 v6 = new Vector3(bbs.MaxX, bbs.MinY, bbs.MaxZ);
			Vector3 v7 = new Vector3(bbs.MinX, bbs.MaxY, bbs.MaxZ);
			Vector3 v8 = new Vector3(bbs.MaxX, bbs.MaxY, bbs.MaxZ);

			return new[] { v0, v1, v2, v3, v4, v5, v6, v7, v8 };
		}

		public static Vector3[] CalculateBoundingPlane(BoundingBoxSizes bbs) 
		{
			//mxd. looks like I need only these 2 points, so...
			//center
			Vector3 v0 = new Vector3(bbs.MinX + (bbs.MaxX - bbs.MinX) / 2, bbs.MinY + (bbs.MaxY - bbs.MinY) / 2, bbs.MinZ + (bbs.MaxZ - bbs.MinZ) / 2);
			Vector3 v1 = new Vector3(bbs.MinX, bbs.MinY, bbs.MinZ);
			return new[] { v0, v1 };
		}

		public static void UpdateBoundingBoxSizes(ref BoundingBoxSizes bbs, WorldVertex v) 
		{
			if (v.x < bbs.MinX) bbs.MinX = (short)v.x;
			else if (v.x > bbs.MaxX) bbs.MaxX = (short)v.x;

			if (v.z < bbs.MinZ) bbs.MinZ = (short)v.z;
			else if (v.z > bbs.MaxZ) bbs.MaxZ = (short)v.z;

			if (v.y < bbs.MinY) bbs.MinY = (short)v.y;
			else if (v.y > bbs.MaxY) bbs.MaxY = (short)v.y;
		}
	}
}
