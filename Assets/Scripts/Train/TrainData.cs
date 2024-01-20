using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    //Open scriptable object in editor right after play causes error "ArgumentNullException: Value cannot be null." Its probably a Unity bug
    [CreateAssetMenu()]
    public class TrainData : ScriptableObject
    {
        [field: SerializeField] public Route Route { get; set; } 
        [field: SerializeField] public Cargo Cargo { get; set; }

        internal void Configure(Route route, Cargo cargo)
        {
            Route = route;
            Cargo = cargo;
        }
    }
}
