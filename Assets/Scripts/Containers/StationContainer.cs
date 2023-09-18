using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class StationContainer : MonoBehaviour
    {
        [SerializeField] private Dictionary<int, Station> stations = new();
        [SerializeField] private RailContainer railContainer;

        public void Add(Station station)
        {
            Station copy = Instantiate(station, transform);
            Destroy(copy.GetComponent<Rotator>());
            copy.name = $"Station {copy.GetInstanceID()}";
            copy.SetUpRoadSegment();
            copy.segment.Points = station.segment.Points.Select(p => station.transform.rotation * p).ToList();

            stations.Add(copy.GetInstanceID(), copy);
            
            //what to do wth rail?
        }
    }
}
