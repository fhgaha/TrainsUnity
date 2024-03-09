using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{

    public class BuildingDetector : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"{transform.parent.gameObject.name}: OnTriggerEnter");

            //TODO move away from BuildingDetector_Sender (there is one in Station prefab)
        }
    }

}
