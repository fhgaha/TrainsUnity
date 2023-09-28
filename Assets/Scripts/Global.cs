using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class Global : MonoBehaviour
    {
        public static Global Instance { get; private set; }
        [field: SerializeField] public RailContainer RailContainer { get; set; }
        [field: SerializeField] public StationContainer StationContainer { get; set; }
        [field: SerializeField] public TrainContainer TrainContainer { get; set; }
        [field: SerializeField] public RailBuilder RailBuilder { get; set; }
        [field: SerializeField] public StationBuilder StationBuilder { get; set; }

        public float DriveDistance { get; private set; } = 1f;

        void Awake()
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
        }

        private void Start()
        {
            //if (Instance == null)
            //{
            //    DontDestroyOnLoad(gameObject);
            //    Instance = this;
            //}
            //else if (Instance == this)
            //{
            //    throw new System.Exception($"Attempting to create instance of {this.GetType()} signleton when such instance already exists");
            //    //Destroy(gameObject);
            //}
        }
    }
}
