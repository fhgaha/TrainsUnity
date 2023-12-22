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
        private ComplexRailBuilder buildHelper;

        private void Awake()
        {
            buildHelper = new ComplexRailBuilder(rb);
        }

        private void Start()
        {
            rb.Parent = this;
            rb.gameObject.SetActive(true);

            //Turn off this game object if tests are not required

            //buildHelper.Build_I();
            //buildHelper.Build_I_II();
            //buildHelper.Build_I_fromLeftTo_I_fromRight_sameZ();
            //buildHelper.Build_T_fromLooseEndToConnection();
            //buildHelper.Build_T_fromConnectionToLooseEnd();
            //buildHelper.Build_H();
            //buildHelper.Build_C();
            //buildHelper.Build_IT();

            //buildHelper.BuildAndDestroyAllOnce();
            //buildHelper.BuildAndDestroyAllSeveralTimes();


            sb.Configure(this);
            Station from = BuildStationAt(new Vector3(-50, 0, -50), 30, "Station from");
            Station to = BuildStationAt(new Vector3(30, 0, 30), -30,    "Station to");
            BuildRoadBetweenStations(from, to);
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
            buildHelper.Build(from.Entry1, new Vector3(0, 0, 1), to.Entry1);

            //TODO: build road between the shortest ends
        }
    }
}
