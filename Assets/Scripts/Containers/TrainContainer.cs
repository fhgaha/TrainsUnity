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
        [SerializeField] private GameObject carriagePrefab;
        [SerializeField] private StationContainer sc;
        [SerializeField] private RailContainer rc;

        private void Start()
        {

        }

        public void SendTrain(Route route)
        {
            if (route.PathForward.Count == 0 || route. PathBack.Count == 0) return;

            GameObject trainObj = new GameObject("Train", typeof(Train));
            trainObj.transform.parent = transform;

            TrainData data = ScriptableObject.CreateInstance<TrainData>();
            data.Configure(
                route,
                new Cargo { Freight = new Freight(10), Mail = 5, Passengers = 15 }
            );
            Train trainComp = trainObj.GetComponent<Train>();
            trainComp.Configure(data, locoPrefab, carriagePrefab);
        }

    }
}
