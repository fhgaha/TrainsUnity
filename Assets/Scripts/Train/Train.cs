using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class Train : MonoBehaviour
    {
        [field: SerializeField] public TrainData Data { get; private set; }
        private LocomotiveMove loco;
        private List<CarriageMove> wagons = new();

        public void Configure(TrainData data, GameObject locoPrefab, GameObject carriagePrefab)
        {
            Data = data;

            loco = Instantiate(locoPrefab, transform).GetComponent<LocomotiveMove>().Configure(data.Route.PathForward, data.Route.PathBack, 20);
            CarriageMove wagon1 = Instantiate(carriagePrefab, transform).GetComponent<CarriageMove>().Configure(data.Route.PathForward, loco.Joint);
            wagons.Add(wagon1);

            StartCoroutine(loco.Move_Routine(3, 3, 21));
            //StartCoroutine(wagon1.Move_Routine(3, 3, 11));
        }

        private void Update()
        {
            foreach (var w in wagons)
            {

            }
        }


    }
}
