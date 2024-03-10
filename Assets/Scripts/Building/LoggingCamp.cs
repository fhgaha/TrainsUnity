using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class LoggingCamp : MonoBehaviour, IProfitBuilding
    {
        //this is set to blueprint
        //[field: SerializeField] public Station OwnedByStation { get; set; }
        public CargoType CargoType { get; private set; } = CargoType.Logs;
        public int Amnt { get; set; } = 0;
        public MeshRenderer Visual { get; private set; }

        private void Awake()
        {
            Visual = GetComponentInChildren<MeshRenderer>();
        }

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

        public void SendGoodsToStation(StationCargoHandler sch)
        {
            int toSend = UnityEngine.Random.Range(1, 3);
            if (Amnt >= toSend)
            {
                Amnt -= toSend;
                Debug.Log($"old supplyValue: {sch.Supply.Amnts[CargoType] }");
                sch.Supply.Amnts[CargoType] += toSend;
                Debug.Log($"toSend: {toSend}, new supplyValue: {sch.Supply.Amnts[CargoType] }");
            }
        }

    }

    public interface IProfitBuilding
    {
        //public Station OwnedByStation { get; set; }
        public CargoType CargoType { get; }
        public int Amnt { get; set; }
        public MeshRenderer Visual { get; }
        public GameObject gameObject { get; }

        public void SendGoodsToStation(StationCargoHandler sch);
    }


}
