using System;
using UnityEngine;

namespace Trains
{
    [Serializable]
    public struct HeadedPoint
    {
        public static HeadedPoint Empty => new(Vector3.zero, 0);

        public Vector3 pos;
        public float heading;   //Degrees, zero heading is 12 o'clock

        public HeadedPoint(Vector3 pos, float heading)
        {
            this.pos = pos;
            this.heading = heading;
        }

        public Vector3 ToDir() => new Vector3 { x = Mathf.Sin(heading * Mathf.Deg2Rad), z = Mathf.Cos(heading * Mathf.Deg2Rad) }.normalized;

        public override string ToString() => $"HeadedPoint. Pos: {pos}, heading: {heading}";
    }
}
