using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class Train : MonoBehaviour
    {
        [field: SerializeField] public TrainData Data { get; private set; }
    }
}
