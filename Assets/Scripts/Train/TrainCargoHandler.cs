using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class TrainCargoHandler : MonoBehaviour
    {
        [SerializeField] bool debug = true;
        public List<string> CarsCargoDisplay;

        Train train;
        float unloadTime = 3, loadTime = 3;

        public TrainCargoHandler Confgure(Train train)
        {
            this.train = train;
            return this;
        }

        public IEnumerator Unload_Rtn(List<Carriage> cars)
        {
            yield return CarsPlayDelayedAnims_Crtn(0.4f);
            IEnumerator CarsPlayDelayedAnims_Crtn(float delay)
            {
                foreach (var car in cars)
                {
                    decimal worth  = car.Cargo.GetWorthValue();
                    train.Owner.AddProfitForDeliveredCargo(worth);
                    train.Route.StationTo.CargoHandler.UnloadCargoFrom(car.Cargo);
                    car.PlayProfitAnim($"+{(int)worth}$");
                    yield return new WaitForSeconds(delay);
                }
            }

            CarsCargoDisplay = cars.Select(c => c.Cargo).Select(c => $"{c.CargoType}, {c.Amnt}").ToList();

            yield return new WaitForSeconds(unloadTime);
        }

        public IEnumerator Load_Rtn(List<Carriage> cars)
        {
            List<CarCargo> cargoTypesOnly = train.Route.GetCargoTypesToLoad(cars.Count);

            yield return CarsPlayDelayedAnims_Crtn(0.4f);
            IEnumerator CarsPlayDelayedAnims_Crtn(float delay)
            {
                for (int i = 0; i < cargoTypesOnly.Count; i++)
                {
                    CarCargo c = cargoTypesOnly[i];
                    train.Route.StationFrom.CargoHandler.LoadCargoTo(c);
                    cars[i].Cargo = c;
                    if (debug)
                        cars[i].PlayProfitAnim($"Loaded {cars[i].Cargo.CargoType}, amnt {cars[i].Cargo.Amnt}$");

                    yield return new WaitForSeconds(delay);
                }
            }

            CarsCargoDisplay = cars.Select(c => c.Cargo).Select(c => $"{c.CargoType}, {c.Amnt}").ToList();

            yield return new WaitForSeconds(loadTime);
        }
    }
}
