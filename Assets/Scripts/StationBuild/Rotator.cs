using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class Rotator : MonoBehaviour
    {
        private Station parent;
        private float rotationSpeed = 80;

        public void Configure(Station parent)
        {
            this.parent = parent;
        }

        void Update()
        {
            if (Input.GetKey(KeyCode.Q))
            {
                transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
                //vectorToRotate = Quaternion.Euler(0, -rotationSpeed * Time.deltaTime, 0) * vectorToRotate;

                //for (int i = 0; i < parent.segment.Points.Count; i++)
                //{
                //    parent.segment.Points[i] = Quaternion.Euler(0, -rotationSpeed * Time.deltaTime, 0) * parent.segment.Points[i];
                //}
            }

            if (Input.GetKey(KeyCode.E))
            {
                transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

                //for (int i = 0; i < parent.segment.Points.Count; i++)
                //{
                //    parent.segment.Points[i] = Quaternion.Euler(0, -rotationSpeed * Time.deltaTime, 0) * parent.segment.Points[i];
                //}
            }
        }
    }
}
