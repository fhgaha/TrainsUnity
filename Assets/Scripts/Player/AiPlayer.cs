using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public interface IPlayer { }

    public class AiPlayer : MonoBehaviour, IPlayer
    {
        [SerializeField] private RailBuilder rb;
        [SerializeField] private StationBuilder sb;
        [SerializeField] private Camera cam;
        private ComplexRailBuilder crb;

        private void Awake()
        {
            crb = new ComplexRailBuilder(rb);
        }

        private void Start()
        {
            rb.Parent = this;
            rb.gameObject.SetActive(true);

            //Turn off this game object if tests are not required

            //crb.Build_I();
            //crb.Build_I_II();
            //crb.Build_I_fromLeftTo_I_fromRight_sameZ();
            //crb.Build_T_fromLooseEndToConnection();
            //crb.Build_T_fromConnectionToLooseEnd();
            //crb.Build_H();
            //crb.Build_C();
            //crb.Build_IT();

            //crb.BuildAndDestroyAllOnce();
            //crb.BuildAndDestroyAllSeveralTimes();


            sb.Configure(this);
            Station from = BuildStationAt(new Vector3(-50, 0, -50), 30);
            Station to = BuildStationAt(new Vector3(30, 0, 30), -30);
            //BuildRoadBetweenStations(from, to);
        }

        private Station BuildStationAt(Vector3 vector3, float angle) => sb.PlaceStation(vector3, angle);

        private void BuildRoadBetweenStations(Station from, Station to)
        {
            var dist1 = (from.Entry1 - to.Entry1).magnitude;
            var dist2 = (from.Entry1 - to.Entry2).magnitude;
            var dist3 = (from.Entry2 - to.Entry1).magnitude;
            var dist4 = (from.Entry2 - to.Entry2).magnitude;

            var shortest = Mathf.Min(dist1, dist2, dist3, dist4);

            //build road between the shortest ends
        }
    }
}
