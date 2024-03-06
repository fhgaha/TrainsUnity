using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class StationMovement : MonoBehaviour
    {
        private Station station;
        [SerializeField] private float rotationSpeed = 80;

        public void Configure(Station station)
        {
            this.station = station;
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.Q))
            {
                transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
                station.UpdateSegmPoints();
            }

            if (Input.GetKey(KeyCode.E))
            {
                transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
                station.UpdateSegmPoints();
            }
        }

        public void UpdateRotation(float yAngle) => transform.rotation = Quaternion.Euler(0, yAngle, 0);

        public void UpdatePos(Vector3 newPos, RoadSegment segm, List<Vector3> originalPoints)
        {
            transform.position = newPos;
            segm.UpdatePoints(transform.rotation, originalPoints);
        }

    }
}
