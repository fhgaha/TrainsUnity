using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public enum CargoType { Passengers, Mail, Wood };

    public class CarriageCargo
    {
        public CargoType CargoType { get; set; }
        public int CargoAmnt { get; set; } = 0;
    }
}
