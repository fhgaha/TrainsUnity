using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Trains
{
    //public class StationSelectedEventArgs : EventArgs
    //{
    //    public int from, to;
    //}

    public enum UiState
    {
        None, BuildRailIsActive, BuildStationIsActive, SelectStationsIsActive
    }

    public class UiMain : MonoBehaviour
    {
        public UiState State { get; private set; }

        public event EventHandler<Toggle> OnBuildRailActivated;
        public event EventHandler<Toggle> OnBuildStationActivated;
        public event EventHandler<StationSelectorEventArgs> OnStationsSelectedAcceptPressed;

        //external
        [SerializeField] private StationContainer sc;

        //toggle-buttons
        [SerializeField] private Toggle buildRail;
        [SerializeField] private Toggle buildStation;
        [SerializeField] private Toggle selectStations;

        [SerializeField] private UiStationSelector stationSelector;

        private void Awake()
        {
            GetComponentInChildren<Canvas>().enabled = true;

            //mode buttons
            buildRail.onValueChanged.AddListener(delegate { BuildRailValueChanged(buildRail); });
            buildStation.onValueChanged.AddListener(delegate { BuildStationValueChanged(buildStation); });
            selectStations.onValueChanged.AddListener(delegate { SelectStationsValueChanged(selectStations); });

            stationSelector.OnStationsSelectedAcceptPressed += NotifyStationsSelectedAcceptPressed;
            sc.OnStationAdded += StationContainer_OnStationAdded;
        }

        private void StationContainer_OnStationAdded(object sender, EventArgs e)
        {
            stationSelector.SetStationIcons(sc.Stations);
        }

        private void Start()
        {
            //var stations = stationContainer.Stations;
            //stationSelector.SetTooglesForStations(stations);

        }

        //TODO: would be nice if on rmb rb became untoggled

        private void BuildRailValueChanged(Toggle buildRail)
        {
            State = buildRail.isOn ? UiState.BuildRailIsActive : UiState.None;
            OnBuildRailActivated?.Invoke(this, buildRail);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                if (State == UiState.BuildStationIsActive)
                {
                    //TODO should be in BuildStationValueChanged()
                    State = UiState.None;
                    buildStation.isOn = false;
                    OnBuildStationActivated?.Invoke(this, buildStation);
                }
            }
        }

        private void BuildStationValueChanged(Toggle buildStation)
        {
            State = buildStation.isOn ? UiState.BuildStationIsActive : UiState.None;
            OnBuildStationActivated?.Invoke(this, buildStation);
        }

        private void SelectStationsValueChanged(Toggle selectStations)
        {
            State = selectStations.isOn ? UiState.SelectStationsIsActive : UiState.None;
            stationSelector.gameObject.SetActive(selectStations.isOn);
        }

        private void NotifyStationsSelectedAcceptPressed(object sender, StationSelectorEventArgs e)
        {
            OnStationsSelectedAcceptPressed?.Invoke(this, e);
        }
    }
}
