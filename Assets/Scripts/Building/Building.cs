using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class Building : MonoBehaviour, IProfitBuilding
    {
        //set these in inspector for each prefab
        [field: SerializeField] public Cargo Supply { get; set; }
        [field: SerializeField] public Cargo Demand { get; set; }
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
            foreach (CargoType ct in Supply.Amnts.Keys.ToList())
            {
                Supply.Amnts[ct] += 1;
            }

        }

        public void SendGoodsToStation(StationCargoHandler sch)
        {
            int toSend = UnityEngine.Random.Range(1, 4);
            foreach (CargoType ct in Supply.Amnts.Keys.ToList())
            {
                if (Supply.Amnts[ct] >= toSend)
                {
                    Supply.Amnts[ct] -= toSend;
                    sch.Supply.Amnts[ct] += toSend;
                }
            }
        } 

    }

    public interface IProfitBuilding
    {
        public Cargo Supply { get; set; }
        public Cargo Demand { get; set; }
        public MeshRenderer Visual { get; }
        public GameObject gameObject { get; }

        public void SendGoodsToStation(StationCargoHandler sch);
    }


}
