using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Trains
{
    public class StationCargoHandler : MonoBehaviour, IFootCargoDestination
    {
        [field: SerializeField] public Cargo Supply { get; set; } = new();
        [field: SerializeField] public Cargo Demand { get; set; } = new();
        private static int average = 50;
        private static int avgStep = 1;

        public Station Station => station;
        private Station station;

        public StationCargoHandler Configure(Station station)
        {
            this.station = station;
            return this;
        }

        private void OnEnable()
        {
            Global.OnTick_3 += Tick;
        }

        private void OnDisable()
        {
            Global.OnTick_3 -= Tick;
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

        private void Tick(object sender, EventArgs e)
        {
            if (station.IsBlueprint) return;

            CalcDemand();
        }

        private void CalcDemand()
        {
            //demand
            //for (int i = 0; i < Demand.Amnts.Count; i++)
            //{
            //    (CargoType ct, int amnt) = Demand.Amnts.ElementAt(i);
            //    if (amnt < average)
            //        Demand.Amnts[ct] += avgStep;
            //}

            //supply
            for (int i = 0; i < Supply.Amnts.Count; i++)
            {
                (CargoType ct, int amnt) = Supply.Amnts.ElementAt(i);
                //switch (ct)
                //{
                //    case CargoType.Passengers:
                //        if (amnt < average)
                //            Supply.Amnts[ct] += avgStep;
                //        break;
                //    case CargoType.Mail:
                //        if (amnt < average)
                //            Supply.Amnts[ct] += avgStep;
                //        break;
                //    case CargoType.Logs:
                //        if (amnt > 0)
                //            Supply.Amnts[ct] -= avgStep;
                //        break;
                //}
            }

        }
    }
}
