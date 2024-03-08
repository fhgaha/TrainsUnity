using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

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

        public void LoadCargoTo(CarCargo carCargo)
        {
            //Assert.IsTrue(carCargo != null, $"car cargo should not be null");

            CargoType ct = carCargo.CargoType;
            int subtrackted = Supply.SubtractFullCarAmnt(ct);
            carCargo.Amnt += subtrackted;
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
