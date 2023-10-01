using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class TrainContainer : MonoBehaviour
    {
        [SerializeField] private GameObject locoPrefab;
        [SerializeField] private StationContainer sc;
        [SerializeField] private RailContainer rc;

        private void Start()
        {
            
        }

        public void SendTrain(List<Vector3> path)
        {
            if (path.Count == 0)
            {
                Debug.LogError("cant find path", this);
                return;
            }

            GameObject train = Instantiate(locoPrefab, transform);

            //this is temporary for test purposes
            train.transform.position = path[0];
            LocomotiveMove locoMover = train.GetComponent<LocomotiveMove>();
            locoMover.Points = path;
        }

        //public void SendTrain(params Node[] stations)
        //{
        //    GameObject train = Instantiate(locoPrefab, transform);

        //    //this is temporary for test purposes
        //    Station fromStation = sc.Stations.Values.First(s => s.Entry1 == stations[0].Pos || s.Entry2 == stations[0].Pos);
        //    Station toStation = sc.Stations.Values.First(s => s.Entry1 == stations[^1].Pos || s.Entry2 == stations[^1].Pos);
        //    train.transform.position = fromStation.transform.position;
        //    LocomotiveMove locoMover = train.GetComponent<LocomotiveMove>();
        //    var points = stations.Select(s => s.Pos).ToList();
        //    locoMover.Points = points;
        //}
    }
}
