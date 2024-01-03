using System.Collections;
using System.Collections.Generic;
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
        private ComplexRailBuilder railBuilder;
        public Color Color { get; set; } = Color.red;


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


            BuildTwoStationsAndRoadBetween();
        }

        private void BuildTwoStationsAndRoadBetween()
        {
            sb.Configure(this);
            Station from = BuildStationAt(new Vector3(-50, 0, -50), 30, "Station from");
            Station to = BuildStationAt(new Vector3(30, 0, 30), -30, "Station to");
            BuildRoadBetweenStations(from, to);

            //run a train
            StartCoroutine(WaitThanSendTrain());
            IEnumerator WaitThanSendTrain()
            {
                //TODO: need a way to know when all roads are built and there will be no "cant find path" error and train is ready to go
                yield return new WaitForSeconds(3);
                Debug.Log("waited for 3 secs");

                List<Vector3> path = RouteManager.Instance.CreateRoute(new List<int> { from.GetInstanceID(), to.GetInstanceID() });
                Global.Instance.TrainContainer.SendTrain(path);
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

            //NODES ARE NOT REGISTERED IN ROUTE MANAGER AND GRAPH

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
