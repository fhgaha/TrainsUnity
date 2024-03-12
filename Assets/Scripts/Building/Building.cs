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
        [SerializeField] FootCargo cargoMovingUnitPrefab;
        public List<StationCargoHandler> StationsInReach = new();
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

        private bool TryFindStationDemanding(CargoType ct, out StationCargoHandler station)
        {
            //var striaght 
            //var dist_this_StationFrom 
            //var dist_StationTo_building =  Distance(stationTo, building)
            //var rrLength = ...
            //var total_dist = dist_this_StationFrom  + rrLength + dist_StationTo_building 
            //which one is better (faster? cheaper to travel? more profitable?)
            //var straightCost = straight / Demand
            //var rrCost = 0.1 * total_dist / Demand 

            //make navmesh points less weight?

            //make link?

            //

            List<Building> demands = BuildingContainer.Instance.Buildings.Where(b => b.Demand.Amnts.Keys.Contains(ct)).ToList();
            if (demands.Count == 0)
            {
                //stockpile
                station = null;
                return false;
            }

            float radius = 50;
            var stationsInRadius = Global.Instance.StationContainer.Stations
                .Where(p => Vector3.SqrMagnitude(p.Value.transform.position - transform.position) < radius * radius)
                .Select(p => (station: p.Value, dist: Vector3.SqrMagnitude(p.Value.transform.position - transform.position)))
                .OrderBy(t => t.dist).ToList();

            List<(Vector3 start, float dist_start_station1,
                Station station1, float dist_station1_station2,
                Station station2, float dist_station2_target,
                Building target, float dist_total)> paths = new();

            foreach ((Station stationFrom, float start_StationFrom_Dist) in stationsInRadius)
            {
                List<Station> connected = RouteManager.Instance.FindConnectedStations(stationFrom);
                foreach (Station stationTo in connected)
                {
                    float from_To_Dist = Vector3.SqrMagnitude(stationFrom.transform.position - stationTo.transform.position);
                    foreach (Building d in demands)
                    {
                        float to_Target_Dist = Vector3.SqrMagnitude(stationTo.transform.position - d.transform.position);
                        var path = (
                            start: transform.position, dist_start_station1: start_StationFrom_Dist,
                            station1: stationFrom, dist_station1_station2: from_To_Dist,
                            station2: stationTo, dist_station2_target: to_Target_Dist,
                            target: d, dist_total: start_StationFrom_Dist + from_To_Dist + to_Target_Dist);
                        paths.Add(path);
                    }
                }
            }

            if (paths.Count != 0)
            {
                //follow the path
                var shortest = paths.OrderBy(p => p.dist_total).First();
                station = shortest.station1.CargoHandler;
                return true;
            }

            //go by foot
            station = null;
            return false;
        }

        private bool TryFindBuildingDemanding(CargoType ct, out Building building)
        {
            List<Building> demands = BuildingContainer.Instance.Buildings.Where(b => b.Demand.Amnts.Keys.Contains(ct)).ToList();
            if (demands.Count == 0)
            {
                building = null;
                return false;
            }
            
            List<Building> ordered = demands.OrderBy(d => Vector3.SqrMagnitude(transform.position - d.transform.position)).ToList();
            building = ordered[0];
            return true;
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
            FootCargo inst = Instantiate(cargoMovingUnitPrefab);
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
