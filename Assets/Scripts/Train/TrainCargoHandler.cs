using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class TrainCargoHandler : MonoBehaviour
    {
        Train train;
        float unloadTime = 3, loadTime = 3;

        public TrainCargoHandler Confgure(Train train)
        {
            this.train = train;
            return this;
        }

        public IEnumerator Unload_Rtn(List<Carriage> cars)
        {
            //decimal worth = Owner.AddProfitForDeliveredCargo(Data.Cargo);
            //Data.Route.StationTo.UnloadCargo(this);
            //string carText = $"+{(int)worth / 2}$";

            yield return CarsPlayDelayedAnims_Crtn(0.4f);
            IEnumerator CarsPlayDelayedAnims_Crtn(float delay)
            {
                foreach (var car in cars)
                {
                    decimal worth = train.Owner.AddProfitForDeliveredCargo(car.Cargo);
                    train.Route.StationTo.UnloadCargoFrom(car);
                    car.PlayProfitAnim($"+{(int)worth}$");
                    yield return new WaitForSeconds(delay);
                }
            }

            yield return new WaitForSeconds(unloadTime);
        }

        public IEnumerator Load_Rtn(List<Carriage> cars)
        {
            List<CarCargo> onlyCargoTypesSet = train.Route.GetCargoToLoad_NoSubtraction(cars.Count);
            yield return Load_Rtn(cars, onlyCargoTypesSet);
        }

        public IEnumerator Load_Rtn(List<Carriage> cars, List<CarCargo> cargos)
        {
            //load each car
            for (int i = 0; i < cargos.Count; i++)
            {
                CarCargo c = cargos[i];
                train.Route.StationFrom.CargoHandler.LoadCargoTo(c);
                cars[i].Cargo = c;
                //yield return new WaitForSeconds(0.4f);
            }

            yield return new WaitForSeconds(loadTime);
        }
    }
}
