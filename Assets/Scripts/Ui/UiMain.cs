using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Trains
{
    public class StationSelectedEventArgs : EventArgs
    {
        public int from, to;
    }

    public enum UiState
    {
        None, BuildRailIsActive, BuildStationIsActive, SelectStationsIsActive
    }

    public class UiMain : MonoBehaviour
    {
        public UiState State { get; private set; }

        public event EventHandler<Toggle> OnBuildRailActivated;
        public event EventHandler<Toggle> OnBuildStationActivated;
        public event EventHandler<StationSelectedEventArgs> OnStationsSelected;

        //toggle-buttons
        [SerializeField] private Toggle buildRail;
        [SerializeField] private Toggle buildStation;
        [SerializeField] private Toggle selectStations;

        [SerializeField] private UiStationSelector stationSelector;

        private void Awake()
        {
            GetComponentInChildren<Canvas>().enabled = true;
        }

        private void Start()
        {
            //mode buttons
            buildRail.onValueChanged.AddListener(delegate { BuildRailValueChanged(buildRail); });
            buildStation.onValueChanged.AddListener(delegate { BuildStationValueChanged(buildStation); });
            selectStations.onValueChanged.AddListener(delegate { SelectStationsValueChanged(selectStations); });

            stationSelector.OnStationsSelected += (sender, btn) => OnStationsSelected?.Invoke(this, new StationSelectedEventArgs { from = 0, to = 0 });
        }

        //TODO: would be nice if on rmb rb became untoggled

        private void BuildRailValueChanged(Toggle buildRail)
        {
            State = buildRail.isOn ? UiState.BuildRailIsActive : UiState.None;
            OnBuildRailActivated?.Invoke(this, buildRail);
        }

        private void BuildStationValueChanged(Toggle buildStation)
        {
            //Debug.Log($"BuildStationValueChanged: {buildStation.isOn}");
            State = buildStation.isOn ? UiState.BuildStationIsActive : UiState.None;
            OnBuildStationActivated?.Invoke(this, buildStation);
        }

        private void SelectStationsValueChanged(Toggle selectStations)
        {
            State = selectStations.isOn ? UiState.SelectStationsIsActive : UiState.None;
            stationSelector.gameObject.SetActive(selectStations.isOn);
        }
    }
}
