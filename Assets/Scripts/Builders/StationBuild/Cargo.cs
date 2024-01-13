using System;
using UnityEngine;

namespace Trains
{
    [Serializable]
    public class Cargo
    {
        [field: SerializeField] public int Passengers { get; set; } = 10;
        [field: SerializeField] public int Mail { get; set; } = 20;
        [field: SerializeField] public Freight Freight { get; set; } = new(30);
    }
}
