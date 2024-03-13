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
        [field: SerializeField] public Cargo Supply { get; set; } = Cargo.AllZero;
        [field: SerializeField] public Cargo Demand { get; set; } = Cargo.AllZero;
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

        private void Tick(object sender, EventArgs e)
        {

        }

        public void LoadCargoTo(CarCargo carCargo)
        {
            //Assert.IsTrue(carCargo != null, $"car cargo should not be null");

            CargoType ct = carCargo.CargoType;
            int subtrackted = Supply.SubtractFullCarAmnt(ct);
            carCargo.Amnt += subtrackted;
        }

        public void UnloadCargoFrom(CarCargo car)
        {
            Supply.Add(car);
            car.Erase();
        }

        public void OnFootCargoCame(FootCargo footCargo)
        {
            Supply.Amnts[footCargo.CargoType] += footCargo.Amnt;
        }
    }
}
