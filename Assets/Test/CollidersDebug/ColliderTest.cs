using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class ColliderTest : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"OnTriggerEnter {other}");
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log($"OnCollisionEnter {collision}");
        }

    }
}
