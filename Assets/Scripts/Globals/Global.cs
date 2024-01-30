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
        public IPlayer MainPlayer { get; set; }

        public float DriveDistance { get; private set; } = 1f;
        public int MapWidth = 1024;
        public int MarHeght = 640;

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
    }


}
