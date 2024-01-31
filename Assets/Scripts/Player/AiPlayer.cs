using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public interface IPlayer
    {
        public int Id { get; set; }
        public Color Color { get; set; }
        public decimal AddProfitForDeliveredCargo(Cargo cargo);
        public decimal AddProfitForDeliveredCargo(CarriageCargo cargo);
    }

    public class AiPlayer : MonoBehaviour, IPlayer
    {
        public int Id { get; set; }
        public Color Color { get; set; } = Color.red;
        public decimal MoneyBalance { get; set; } = 1000;

        [SerializeField] private RailBuilder rb;
        [SerializeField] private StationBuilder sb;
        [SerializeField] private Camera cam;

        private RailGenerator railGen;
        
        private void Awake()
        {
            Id = GetInstanceID();
            railGen = new RailGenerator(rb);
        }

        private void Start()
        {
            rb.Configure(this);
            rb.gameObject.SetActive(true);
            sb.Configure(this);

            //Turn off this game object if tests are not required

            //railBuilder.Build_I();
            //railBuilder.Build_I_II();
            //railBuilder.Build_I_fromLeftTo_I_fromRight_sameZ();
            //railBuilder.Build_T_fromLooseEndToConnection();
            //railBuilder.Build_T_fromConnectionToLooseEnd();
            //railBuilder.Build_H();
            //railBuilder.Build_C();
            //railBuilder.Build_IT();

            //railBuilder.BuildAndDestroyAllOnce();
            //railBuilder.BuildAndDestroyAllSeveralTimes();


            //BuildTwoStationsCurvyRoadSendTrain();

            BuildTwoStationsStraightRoadSendTrain();
        }

        private void BuildTwoStationsStraightRoadSendTrain()
        {
            StartCoroutine(Routine());
            IEnumerator Routine()
            {
                Station from = BuildStationAt(new Vector3(-50, 0, -50), 30, "Station from");
                from.Cargo = Cargo.Empty;
                from.Cargo.PassengersAmnt = 10;
                Station to = BuildStationAt(new Vector3(30, 0, 30), -30, "Station to");
                to.Cargo = Cargo.Empty;
                to.Cargo.MailAmnt = 15;

                yield return railGen.Build_Routine(
                    from.Entry1,
                    to.Entry1
                );

                Route r = RouteManager.Instance.CreateRoute(new List<int> { from.GetInstanceID(), to.GetInstanceID() });
                Global.Instance.TrainContainer.SendTrain(r, this);
            }
        }

        private void BuildTwoStationsCurvyRoadSendTrain()
        {
            StartCoroutine(Routine());
            IEnumerator Routine()
            {
                Station from = BuildStationAt(new Vector3(-50, 0, -50), 30, "Station from");
                Station to = BuildStationAt(new Vector3(30, 0, 30), -30, "Station to");

                yield return railGen.Build_Routine(
                    from.Entry1,
                    new Vector3(-10, 0, -50),
                    new Vector3(10, 0, 0),
                    new Vector3(-10, 0, 50),
                    to.Entry1
                );

                Route r = RouteManager.Instance.CreateRoute(new List<int> { from.GetInstanceID(), to.GetInstanceID() });
                Global.Instance.TrainContainer.SendTrain(r, this);
            }
        }

        private Station BuildStationAt(Vector3 vector3, float angle, string name)
        {
            Station instance = sb.PlaceStation(vector3, angle);
            instance.name = name;
            return instance;
        }

        private void BuildRoadBetweenStations(Station from, Station to)
        {
            //Doesn't work with Vector3.zero but works any other

            railGen.Build(
                from.Entry1,
                new Vector3(-10, 0, -50),
                new Vector3(10, 0, 0),
                new Vector3(-10, 0, 50),
                to.Entry1
                );

            //TODO: build road between the shortest ends
        }

        public decimal AddProfitForDeliveredCargo(Cargo cargo)
        {
            var worth = cargo.GetWorthValue();
            MoneyBalance += worth;
            return worth;
        }

        public decimal AddProfitForDeliveredCargo(CarriageCargo cargo)
        {
            var worth = cargo.GetWorthValue();
            MoneyBalance += worth;
            return worth;
        }
    }
}
