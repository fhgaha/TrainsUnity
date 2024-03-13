using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Trains
{
    [Serializable]
    public class Building : MonoBehaviour, IProfitBuilding, IFootCargoDestination
    {
        //set these in inspector for each prefab
        [field: SerializeField] public Cargo Supply { get; set; }
        [field: SerializeField] public Cargo Demand { get; set; }
        
        [SerializeField] FootCargo cargoMovingUnitPrefab;
        public List<StationCargoHandler> StationsInReach = new();
        public MeshRenderer Visual { get; private set; }
        float radius;

        private void Awake()
        {
            Visual = GetComponentInChildren<MeshRenderer>();
            radius = transform.Find("DetectStations/Collider").GetComponent<CapsuleCollider>().radius;
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

                IFootCargoDestination trg = FindTarget(ct);
                if (trg != null)
                    SendCargoByFootTo(ct, amnt, trg);
            }

        }

        private IFootCargoDestination FindTarget(CargoType ct)
        {
            List<Building> demandBuildings = BuildingContainer.Instance.Buildings.Where(b => b.Demand.Amnts.Keys.Contains(ct)).ToList();

            var stationsInRadius = Global.Instance.StationContainer.Stations
                .Where(p => Vector3.SqrMagnitude(p.Value.transform.position - transform.position) < radius * radius)
                .Select(p => (station: p.Value, dist: Vector3.SqrMagnitude(p.Value.transform.position - transform.position)))
                .OrderBy(t => t.dist).ToList();

            List<(Vector3 start, float dist_start_station1,
                Station station1, float dist_station1_station2,
                Station station2, float dist_station2_target,
                Building target, float dist_total)> railPaths = new();

            foreach ((Station stationFrom, float start_StationFrom_Dist) in stationsInRadius)
            {
                List<Station> connected = RouteManager.Instance.FindConnectedStations(stationFrom);
                foreach (Station stationTo in connected)
                {
                    float from_To_Dist = Vector3.SqrMagnitude(stationFrom.transform.position - stationTo.transform.position);
                    foreach (Building d in demandBuildings)
                    {
                        float to_Target_Dist = Vector3.SqrMagnitude(stationTo.transform.position - d.transform.position);
                        var path = (
                            start: transform.position, dist_start_station1: start_StationFrom_Dist,
                            station1: stationFrom, dist_station1_station2: from_To_Dist,
                            station2: stationTo, dist_station2_target: to_Target_Dist,
                            target: d, dist_total: start_StationFrom_Dist + from_To_Dist + to_Target_Dist);
                        railPaths.Add(path);
                    }
                }
            }

            if (railPaths.Count == 0 && demandBuildings.Count == 0)
            {
                //stockpile
                return null;
            }
            else if (railPaths.Count != 0 && demandBuildings.Count == 0)
            {
                //calc for railPaths
                //var shortestRailPath = railPaths.OrderBy(p => p.dist_total).First();
                //return shortestRailPath.station1.CargoHandler;

                //dont go cause no demand building anywhere
                return null;
            }
            else if (railPaths.Count == 0 && demandBuildings.Count != 0)
            {
                //calc for demandBuildings
                var closestBuilding = demandBuildings.OrderBy(d => Vector3.SqrMagnitude(transform.position - d.transform.position)).First();
                return closestBuilding;
            }
            else
            {
                var shortestRailPath = railPaths.OrderBy(p => p.dist_total).First();
                shortestRailPath.dist_total *= 0.1f;
                var closestBuilding = demandBuildings.OrderBy(d => Vector3.SqrMagnitude(transform.position - d.transform.position)).First();

                float railPathCost = shortestRailPath.dist_total / shortestRailPath.target.Demand.Amnts[ct] * (float)Prices.AsDict[ct];
                float footPathCost =
                    Vector3.SqrMagnitude(transform.position - closestBuilding.transform.position) / closestBuilding.Demand.Amnts[ct] * (float)Prices.AsDict[ct];

                if (railPathCost < footPathCost)
                    return shortestRailPath.station1.CargoHandler;
                else
                    return closestBuilding;
            }

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

        //public void SendGoodsToStation(StationCargoHandler sch)
        //{
        //    int toSend = UnityEngine.Random.Range(1, 4);
        //    foreach (CargoType ct in Supply.Amnts.Keys.ToList())
        //    {
        //        if (Supply.Amnts[ct] >= toSend)
        //        {
        //            Supply.Amnts[ct] -= toSend;
        //            //sch.Supply.Amnts[ct] += toSend;

        //            if (cargoMovingUnitPrefab != null)
        //                SendCargoByFootTo(sch);

        //        }
        //    }
        //}

        public void SendCargoByFootTo(CargoType cargoType, int amnt, IFootCargoDestination destiation)
        {
            FootCargo inst = Instantiate(cargoMovingUnitPrefab);
            inst.Configure(cargoType, amnt, transform.position, destiation);
        }
    }

    public interface IProfitBuilding
    {
        public Cargo Supply { get; set; }
        public Cargo Demand { get; set; }
        public MeshRenderer Visual { get; }
        public GameObject gameObject { get; }

        //public void SendGoodsToStation(StationCargoHandler sch);
    }


}
