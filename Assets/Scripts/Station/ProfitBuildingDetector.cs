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
        Station parent;

        public ProfitBuildingDetector Configure(Station station)
        {
            parent = station;
            return this;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IProfitBuilding building)) return;

            Debug.Log($"{this}: {building.gameObject.name}");

            Assert.IsTrue(!detected.Contains(building));
            building.OwnedByStation = parent;
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
