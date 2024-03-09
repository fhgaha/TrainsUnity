using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    [Serializable]
    public class SpeedData
    {
        [SerializeField] public float slowSpeed;
        [SerializeField] public float maxSpeed;
        [SerializeField] public float speedStep;
        [SerializeField] public float curSpeed;
    }

}
