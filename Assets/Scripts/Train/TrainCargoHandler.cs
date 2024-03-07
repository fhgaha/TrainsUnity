using System.Collections;
using System.Collections.Generic;
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
            //get most profitable goods



            //load each car
            foreach (var car in cars)
            {
                train.Route.StationTo.LoadCargoTo(car);
                yield return new WaitForSeconds(0.4f);
            }

            //yield return new WaitForSeconds(loadTime);
        }

    }
}
