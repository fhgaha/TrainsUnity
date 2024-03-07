using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class Global : MonoBehaviour
    {
        public static event EventHandler OnTick;
        public static event EventHandler OnTick_3;
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

        private float tickTimer = 0;
        private float tickDuration = 1;
        private int tick = 0;

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

        private void Update()
        {
            CalcTicks();
        }

        private void CalcTicks()
        {
            tickTimer += Time.deltaTime;
            if (tickTimer >= tickDuration)
            {
                tickTimer -= tickDuration;
                tick = tick >= int.MaxValue ? 0 : tick + 1;
                OnTick?.Invoke(this, EventArgs.Empty);

                if (tick % 3 == 0)
                    OnTick_3?.Invoke(this, EventArgs.Empty);
            }
        }
    }


}
