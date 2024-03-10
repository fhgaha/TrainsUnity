using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class StationCollider : MonoBehaviour
    {
        public Collider Collider { get; private set; }
        Station parent;

        private void Awake()
        {
            Collider = GetComponent<Collider>();
        }

        public StationCollider Configure(Station parent)
        {
            this.parent = parent;
            return this;
        }

        private void OnTriggerEnter(Collider other)
        {
            parent.OnColliderTriggerEnter(other);
            //visual.HandleStatoinEnter(other);
            //visual.HandleRoadEnter(other);
            ////visual.HandleBuildingEnter(other);
        }

        private void OnTriggerExit(Collider other)
        {
            parent.OnColliderTriggerExit(other);
            //visual.HandleStationExit(other);
            //visual.HandleRoadExit(other);
            ////visual.HandleBuildingExit(other);
        }
    }
}
