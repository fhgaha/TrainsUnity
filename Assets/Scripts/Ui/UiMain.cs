using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Trains
{
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
        [SerializeField] private TextMeshProUGUI cash;
        [SerializeField] private RectTransform selectedBuilding;

        private void Awake()
        {
            GetComponentInChildren<Canvas>().enabled = true;

            selectedBuilding.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            buildRail.onValueChanged.AddListener(delegate { BuildRailValueChanged(buildRail); });
            buildStation.onValueChanged.AddListener(delegate { BuildStationValueChanged(buildStation); });
            selectStations.onValueChanged.AddListener(delegate { SelectStationsValueChanged(selectStations); });
            stationSelector.OnStationsSelectedAcceptPressed += NotifyStationsSelectedAcceptPressed;
            sc.OnStationAdded += StationContainer_OnStationAdded;
            SelectableBuilding.OnBuildingSelected += ShowSelectedPanel;
        }

        private void OnDisable()
        {
            buildRail.onValueChanged.RemoveAllListeners();
            selectStations.onValueChanged.RemoveAllListeners();
            selectStations.onValueChanged.RemoveAllListeners();
            stationSelector.OnStationsSelectedAcceptPressed -= NotifyStationsSelectedAcceptPressed;
            sc.OnStationAdded -= StationContainer_OnStationAdded;
            SelectableBuilding.OnBuildingSelected += ShowSelectedPanel;
        }

        public void UpdateCashText(object sender, string value)
        {
            if (sender == Global.Instance.MainPlayer)
                cash.text = $"$ {value}";
        }

        public void ShowSelectedPanel(object sender, EventArgs e)
        {
            var sb = (SelectableBuilding)sender;
            Debug.Log($"-- {sb.transform.parent.name}");
            selectedBuilding.gameObject.SetActive(sb.Selected);
        }

        private void StationContainer_OnStationAdded(object sender, EventArgs e)
        {
            stationSelector.SetStationIcons(sc.Stations);
        }

        private void BuildRailValueChanged(Toggle buildRail)
        {
            State = buildRail.isOn ? UiState.BuildRailIsActive : UiState.None;
            OnBuildRailActivated?.Invoke(this, buildRail);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                switch (State)
                {
                    case UiState.BuildStationIsActive:
                        State = UiState.None;
                        buildStation.isOn = false;
                        OnBuildStationActivated?.Invoke(this, buildStation);
                        break;
                    case UiState.BuildRailIsActive:
                        State = UiState.None;
                        buildRail.isOn = false;
                        OnBuildRailActivated?.Invoke(this, buildRail);
                        break;
                }
            }
        }

        //
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
