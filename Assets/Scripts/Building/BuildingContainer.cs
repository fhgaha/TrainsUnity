using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class BuildingContainer : MonoBehaviour
    {
        public static BuildingContainer Instance { get; private set; }
        public List<Building> Buildings { get; private set; }
        //[SerializeField] private bool printDebugInfo = true;

        private void Awake()
        {
            if (Instance == null)
            {
                DontDestroyOnLoad(gameObject);
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }

            Buildings = GetComponentsInChildren<Building>().ToList();
        }
    }
}
