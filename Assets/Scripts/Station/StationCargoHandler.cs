using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class StationCargoHandler : MonoBehaviour
    {
        [field: SerializeField] public Cargo Cargo { get; set; } = new();
        [field: SerializeField] public Cargo Demand { get; set; } = new();

        public void LoadCargoTo(Carriage car)
        {
            Dictionary<CargoType, int> maxAmnts = CarCargo.MaxAmnts;

            //is station to needs that cargo?
            //load not more than max amnt
        }

        public void UnloadCargoFrom(Carriage car)
        {
            Cargo.Add(car.Cargo);
            car.Cargo.Erase();
        }

        //this is triggered on train creation on scene start
        private void OnTriggerEnter(Collider other)
        {
            HandleTrainEnter(other);
        }

        private void HandleTrainEnter(Collider collider)
        {
            if (collider.TryGetComponent(out LocomotiveMove locMove))
            {
                //unload and load train


            }
        }

        //public void UnloadCargo(Train train)
        //{
        //    Cargo.Add(train.Data.Cargo);
        //    train.Data.Cargo.Erase();
        //}
    }
}
