using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Trains
{
    [Serializable]
    public class Building : MonoBehaviour, IProfitBuilding, ICargoUnitDestination
    {
        //set these in inspector for each prefab
        [field: SerializeField] public Cargo Supply { get; set; }
        [field: SerializeField] public Cargo Demand { get; set; }
        [SerializeField] CargoSelfMovingUnit cargoMovingUnitPrefab;
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
            foreach ((CargoType ct, int amnt) in Supply.Amnts.ToList())
            {
                Supply.Amnts[ct] += 1;

                if (TryFindStationDemanding(ct, out StationCargoHandler s))
                {
                    SendCargoUnitTo(s);
                }
                else if (TryFindBuildingDemanding(ct, out Building b))
                {
                    SendCargoUnitTo(b);
                }
                else
                {
                    //pile up produced goods
                }
            }

        }

        private bool TryFindStationDemanding(CargoType ct, out StationCargoHandler s)
        {
            
            //RouteManager.Instance.FindShortestPath()



            s = null;
            return false;
        }

        private bool TryFindBuildingDemanding(CargoType ct, out Building b)
        {
            b = null;
            return false;
        }

        public void SendGoodsToStation(StationCargoHandler sch)
        {
            int toSend = UnityEngine.Random.Range(1, 4);
            foreach (CargoType ct in Supply.Amnts.Keys.ToList())
            {
                if (Supply.Amnts[ct] >= toSend)
                {
                    Supply.Amnts[ct] -= toSend;
                    //sch.Supply.Amnts[ct] += toSend;

                    if (cargoMovingUnitPrefab != null)
                        SendCargoUnitTo(sch);

                }
            }
        }

        public void SendCargoUnitTo(ICargoUnitDestination destiation)
        {
            CargoSelfMovingUnit inst = Instantiate(cargoMovingUnitPrefab);
            inst.Configure(transform.position, destiation);
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
