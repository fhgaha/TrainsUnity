using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class DetectorWithChildren : MonoBehaviour
    {
        public List<Collider> Children { get; private set; }
        private List<Collider> detected = new();

        private void Awake()
        {
            Children = GetComponentsInChildren<Collider>().ToList();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!Children.Contains(other) && !detected.Contains(other))
            {
                // Your code here
                Debug.Log($"DetectorWithChildren enter");

                detected.Add(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!Children.Contains(other) && detected.Contains(other))
            {
                detected.Remove(other);

                if (detected.Count == 0)
                {
                    // Your code here
                    Debug.Log($"DetectorWithChildren exit");
                }
            }
        }
    }
}
