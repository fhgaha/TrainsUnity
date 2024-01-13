using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public interface IPlayer
    {
        public Color Color { get; set; }
    }

    public class AiPlayer : MonoBehaviour, IPlayer
    {
        [SerializeField] private RailBuilder rb;
        [SerializeField] private StationBuilder sb;
        [SerializeField] private Camera cam;
        public Color Color { get; set; } = Color.red;
        private ComplexRailBuilder railBuilder;

        private void Awake()
        {
            railBuilder = new ComplexRailBuilder(rb);
        }

        private void Start()
        {
            rb.Parent = this;
            rb.gameObject.SetActive(true);

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


            BuildTwoStationsRoadSendTrain();
        }

        private void BuildTwoStationsRoadSendTrain()
        {
            StartCoroutine(Routine());
            IEnumerator Routine()
            {
                sb.Configure(this);
                yield return new WaitUntil(() => sb.AssertConfigured());

                Station from = BuildStationAt(new Vector3(-50, 0, -50), 30, "Station from");
                yield return new WaitUntil(() => from != null);
                 
                Station to = BuildStationAt(new Vector3(30, 0, 30), -30, "Station to");
                yield return new WaitUntil(() => to != null);

                yield return railBuilder.Build_Routine(
                    from.Entry1,
                    new Vector3(-10, 0, -50),
                    new Vector3(10, 0, 0),
                    new Vector3(-10, 0, 50),
                    to.Entry1
                );

                List<Vector3> pathTo = RouteManager.Instance.CreateRoutePoints(new List<int> { from.GetInstanceID(), to.GetInstanceID() });
                List<Vector3> pathBack = pathTo.AsEnumerable().Reverse().ToList();
                Global.Instance.TrainContainer.SendTrain(new List<Station> { from, to }, pathTo, pathBack);
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

            railBuilder.Build(
                from.Entry1,
                new Vector3(-10, 0, -50),
                new Vector3(10, 0, 0),
                new Vector3(-10, 0, 50),
                to.Entry1
                );

            //TODO: build road between the shortest ends
        }
    }
}
