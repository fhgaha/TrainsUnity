using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class StationRotator : MonoBehaviour
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
                station.UpdatePos();
            }

            if (Input.GetKey(KeyCode.E))
            {
                transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
                station.UpdatePos();
            }
        }
    }
}
