using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class LoggingCamp : MonoBehaviour, IProfitBuilding
    {
        public Station OwnedByStation { get; set; }
        public CargoType Type = CargoType.Logs;
        public int Amnt = 0;

        private void OnEnable()
        {
            Global.OnTick_3 += Tick;
        }

        private void OnDisable()
        {
            Global.OnTick_3 -= Tick;
        }

        private void Tick(object sender, EventArgs e)
        {
            Amnt += 1;
        }

    }

    public interface IProfitBuilding
    {
        public Station OwnedByStation { get; set; }
        public GameObject gameObject { get; }
    }


}
