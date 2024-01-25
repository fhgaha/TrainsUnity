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

        public Station Add(Station original)
        {
            Station copy = Instantiate(original, transform);
            Destroy(copy.GetComponent<StationRotator>());
            copy.name = $"Station {copy.GetInstanceID()}";
            copy.SetUpRoadSegment(copy.Owner);
            copy.CopyInfoFrom(original);
            copy.BecomeDefaultColor();

            Stations.Add(copy.GetInstanceID(), copy);

            OnStationAdded?.Invoke(this, EventArgs.Empty);

            //what to do wth rail?
            Global.Instance.RailContainer.AddDontCreateInstance(copy.segment);
            return copy;
        }
    }
}
