using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class StationContainer : MonoBehaviour
    {
        public event EventHandler OnStationAdded;

        public Dictionary<int, Station> Stations { get; private set; } = new();
        [SerializeField] private RailContainer railContainer;

        public void Add(Station station)
        {
            Station copy = Instantiate(station, transform);
            Destroy(copy.GetComponent<StationRotator>());
            copy.name = $"Station {copy.GetInstanceID()}";
            copy.SetUpRoadSegment();
            copy.segment.CopyPoints(station.segment);
            copy.segment.Start = station.segment.Start;
            copy.segment.End = station.segment.End;

            Stations.Add(copy.GetInstanceID(), copy);

            OnStationAdded?.Invoke(this, EventArgs.Empty);

            //what to do wth rail?
            Global.Instance.RailContainer.AddDontCreateInstance(copy.segment);
        }
    }
}
