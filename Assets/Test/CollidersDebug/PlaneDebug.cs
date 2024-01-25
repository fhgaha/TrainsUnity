using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class PlaneDebug : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log($"OnTriggerEnter: {other.gameObject.name}");
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log($"OnCollisionEnter: {collision.gameObject.name}");
        }
    }
}
