using System;
using UnityEngine;

namespace Trains
{
    [Serializable]
    public struct RoadSegmentData
    {
        public HeadedPoint start, end;
        public Vector3 tangent1, tangent2;

        public RoadSegmentData(HeadedPoint start, HeadedPoint end, Vector3 tangent1, Vector3 tangent2)
        {
            this.start = start;
            this.end = end;
            this.tangent1 = tangent1;
            this.tangent2 = tangent2;
        }
    }
}
