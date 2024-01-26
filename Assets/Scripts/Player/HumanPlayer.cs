using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Trains
{
    public enum PlayerState
    {
        None, BuildingRoads, BuildingStations
    }

    public class HumanPlayer : MonoBehaviour, IPlayer
    {
        public int Id { get ; set ; }
        public Color Color { get; set; } = Color.blue;

        [SerializeField] private RailBuilder rb;
        [SerializeField] private StationBuilder sb;
        [SerializeField] private Camera cam;

        public float MoneyBalance { get; set; } = 1000;
        private PlayerState state;

        private void Awake()
        {
            Id = GetInstanceID();
        }

        private void Start()
        {
            EventManager.Instance.OnBuildRailPressed += OnBuildRailPressed;
            //TODO temporary, probably stick main player to current camera
            Global.Instance.MainPlayer = this;

            state = PlayerState.None;
            rb.Configure(this);
            rb.gameObject.SetActive(false);
            sb.Configure(this);
        }

        private void OnBuildRailPressed(object sender, Toggle toggle)
        {
            if (toggle.isOn)
            {
                if (state == PlayerState.BuildingRoads) return;

                rb.gameObject.SetActive(true);
                state = PlayerState.BuildingRoads;
            }
            else
            {
                if (state == PlayerState.None) return;

                rb.gameObject.SetActive(false);
                state = PlayerState.None;
            }
        }

        //private void Update()
        //{
        //    if (!rb.gameObject.activeInHierarchy) return;

        //    rb.Tick();
        //}
    }
}
