using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class StationCargoHandler : MonoBehaviour
    {
        [field: SerializeField] public Cargo Supply { get; set; } = new();
        [field: SerializeField] public Cargo Demand { get; set; } = new();

        private void Awake()
        {
            Global.OnTick_3 += Tick;
        }

        public void LoadCargoTo(Carriage car)
        {
            Dictionary<CargoType, int> maxAmnts = CarCargo.MaxAmnts;

            //is station to needs that cargo?
            //load not more than max amnt

            //    Cargo.Add(train.Data.Cargo);
            //    train.Data.Cargo.Erase();
        }

        public void UnloadCargoFrom(Carriage car)
        {
            Supply.Add(car.Cargo);
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

        private void Tick(object sender, EventArgs e)
        {
            CalcDemand();
        }

        private void CalcDemand()
        {
            
        }
    }
}
