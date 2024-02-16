using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class RoadSubDetector : MonoBehaviour
    {
        DetChild mainChild;

        public void Configure(DetChild mainChild)
        {
            this.mainChild = mainChild;
        }



    }
}
