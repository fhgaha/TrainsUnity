using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Trains
{
    public class ProfitBuildingDetector : MonoBehaviour
    {
        public List<IProfitBuilding> Detected => detected;
        [SerializeField] List<string> detectedDisplay = new();
        [SerializeField] List<IProfitBuilding> detected = new();
        Station parent;

        public ProfitBuildingDetector Configure(Station station)
        {
            parent = station;
            return this;
        }

        private void OnTriggerEnter(Collider other)
        {
            DetectProfitBuilding(other);
        }

        private void OnTriggerExit(Collider other)
        {
            UndetectProfitBuilding(other);
        }

        private void OnEnable()
        {
            Global.OnTick_3 += Tick;
        }

        private void OnDisable()
        {
            Global.OnTick_3 -= Tick;

            detected.Clear();
            detectedDisplay.Clear();
        }

        private void DetectProfitBuilding(Collider other)
        {
            if (!other.TryGetComponent(out IProfitBuilding building)) return;
            Assert.IsTrue(!detected.Contains(building));

            //this link shows how to paint objects in area using URP
            //https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@12.0/manual/renderer-feature-decal.html
            building.Visual.material.color = Color.yellow;
            detected.Add(building);
            detectedDisplay = detected.Select(p => p.ToString()).ToList();
            
            if (!parent.IsBlueprint)
            {
                if (building is Building b)
                    b.StationsInReach.Add(parent.CargoHandler);
            }
        }

        private void UndetectProfitBuilding(Collider other)
        {
            if (!other.TryGetComponent(out IProfitBuilding building)) return;

            //building.OwnedByStation = null;
            building.Visual.material.color = Color.blue;
            detected.Remove(building);
            detectedDisplay = detected.Select(p => p.ToString()).ToList();

            if (!parent.IsBlueprint)
            {
                if (building is Building b)
                    b.StationsInReach.Remove(parent.CargoHandler);
            }
        }

        private void Tick(object sender, EventArgs e)
        {
            foreach (IProfitBuilding build in detected)
            {
                build.SendGoodsToStation(parent.CargoHandler);
            }
        }

    }
}
