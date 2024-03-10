using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Trains
{
    public class ProfitBuildingDetector : MonoBehaviour
    {
        public List<IProfitBuilding> Detected => detected;
        List<IProfitBuilding> detected = new();
        Station station;

        public ProfitBuildingDetector Configure(Station station)
        {
            this.station = station;
            return this;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IProfitBuilding building)) return;

            //Debug.Log($"{this}: {building.gameObject.name}");
            Assert.IsTrue(!detected.Contains(building));
            building.OwnedByStation = station;
            detected.Add(building);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent(out IProfitBuilding building)) return;

            building.OwnedByStation = null;
            detected.Remove(building);
        }
    }
}
