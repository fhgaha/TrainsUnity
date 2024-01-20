using System;
using UnityEngine;

namespace Trains
{
    [Serializable]
    public class Freight
    {
        [field: SerializeField] public int WoodAmnt { get; set; }

        public Freight(int woodAmnt)
        {
            WoodAmnt = woodAmnt;
        }
    }
}