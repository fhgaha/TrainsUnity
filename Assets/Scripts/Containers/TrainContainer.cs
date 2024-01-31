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

        public void SendTrain(Route route, IPlayer owner)
        {
            if (route.PathForward.Count == 0 || route.PathBack.Count == 0) return;

            GameObject trainObj = new("Train", typeof(Train));
            trainObj.transform.parent = transform;

            Train trainComp = trainObj.GetComponent<Train>();

            List<CarCargo> cargoes = new()
            {
                new CarCargo { CargoType = CargoType.Passengers, Amnt = 5 },
                new CarCargo { CargoType = CargoType.Mail, Amnt = 8 },
                new CarCargo { CargoType = CargoType.Mail, Amnt = 8 },
            };

            trainComp.Configure(route, locoPrefab, carriagePrefab, cargoes, owner);
        }

    }
}
