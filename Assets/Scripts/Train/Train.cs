using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class Train : MonoBehaviour
    {
        [field: SerializeField] public TrainData Data { get; private set; }

        public void Configure(TrainData data, GameObject locoPrefab)
        {
            Data = data;

            LocomotiveMove loco = Instantiate(locoPrefab, transform).GetComponent<LocomotiveMove>().Configure(data.Route.PathForward, data.Route.PathBack, 20);
            LocomotiveMove wagon1 = Instantiate(locoPrefab, transform).GetComponent<LocomotiveMove>().Configure(data.Route.PathForward, data.Route.PathBack, 10);

            StartCoroutine(loco.Move_Routine(3, 3, 21));
            StartCoroutine(wagon1.Move_Routine(3, 3, 11));
        }


    }
}
