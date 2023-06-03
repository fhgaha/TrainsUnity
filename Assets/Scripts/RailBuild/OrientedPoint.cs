using UnityEngine;

namespace Trains
{
	public struct OrientedPoint
	{
		public Vector3 pos;
		public Quaternion rot;

		public OrientedPoint(Vector3 pos, Quaternion rot)
		{
			this.pos = pos;
			this.rot = rot;	
		}

		public OrientedPoint(Vector3 pos, Vector3 forward)
		{
			this.pos = pos;
			this.rot = Quaternion.LookRotation(forward);
		}

		//returns vector from global (0; 0) to localSpacePos
		public Vector3 LocalToWorldPos(Vector3 localSpacePos)
		{
			return pos + rot * localSpacePos;
		}

		//retruns vector from global (0; 0), pointing the same direction as in local coords
		public Vector3 LocalToWorldVect(Vector3 localSpacePos)
		{
			return rot * localSpacePos;
		}
	}
}