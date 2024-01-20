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

        public void SendTrain(List<Station> stations, List<Vector3> pathForward, List<Vector3> pathBack)
        {
            if (pathForward.Count == 0 || pathBack.Count == 0) return;

            GameObject trainObj = new GameObject("Train", typeof(Train));
            trainObj.transform.parent = transform;

            TrainData data = ScriptableObject.CreateInstance<TrainData>();
            data.Configure(
                new Route(stations, pathForward, pathBack),
                new Cargo { Freight = new Freight(10), Mail = 5, Passengers = 15 }
            );
            Train trainComp = trainObj.GetComponent<Train>();
            trainComp.Configure(data, locoPrefab);
        }

    }
}
