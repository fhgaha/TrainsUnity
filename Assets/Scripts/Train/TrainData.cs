using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    //Script to create scriptable objects
    [CreateAssetMenu()]
    public class TrainData : ScriptableObject
    {
        [field: SerializeField] public Route Route { get; private set; }
        [field: SerializeField] public Cargo Cargo { get; private set; }
    }
}
