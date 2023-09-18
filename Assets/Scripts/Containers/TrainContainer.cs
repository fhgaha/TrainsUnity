using System;
using System.Collections;
using System.Collections.Generic;
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
        
        public void SendTrain(int from, int to)
        {
            //GameObject train = Instantiate(locoPrefab, transform);

            //this is temporary for test purposes
            //Station startStation = sc.Get(0);
            //train.transform.position = startStation.transform.position;
            //LocomotiveMove locoMover = train.GetComponent<LocomotiveMove>();
            //locoMover.Points = rc.Get(0).Points;
        }
    }
}
