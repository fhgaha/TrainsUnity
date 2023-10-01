using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Trains
{
    public class StationSelectorEventArgs : EventArgs
    {
        public List<int> selectedIds;
    }


    public class UiStationSelector : MonoBehaviour
    {
        public EventHandler<StationSelectorEventArgs> OnStationsSelected;

        [SerializeField] private RectTransform stTogglePrefab;

        [SerializeField] private GameObject minimap;
        [SerializeField] private SelectedStationsList selectedStationsList;
        [SerializeField] private Button accept;
        [SerializeField] private Button cancel;

        private Dictionary<int, Toggle> stationIcons = new();       //<id, toggle>
        private Dictionary<int, Toggle> selectedStations = new();

        private void Awake()
        {
            accept.onClick.AddListener(OnAcceptPressed);
            cancel.onClick.AddListener(OnCancelPressed);
        }

        private void Start()
        {
            gameObject.SetActive(false);
        }

        private void OnAcceptPressed()
        {
            if (stationIcons.All(s => !s.Value.isOn)) return;

            OnStationsSelected?.Invoke(this, new StationSelectorEventArgs { selectedIds = selectedStations.Keys.ToList() });
        }

        private void OnCancelPressed()
        {
            foreach (var s in stationIcons)
            {
                s.Value.GetComponent<Toggle>().isOn = false;
            }

            selectedStations.Clear();
        }

        public void SetStationIcons(Dictionary<int, Station> stations)
        {
            foreach (Transform child in minimap.transform)
            {
                Destroy(child.gameObject);
            }
            stationIcons = new();
            selectedStations = new();

            var mapWidth = Global.Instance.MapWidth;
            var mapHeight = Global.Instance.MarHeght;

            var miniWidth = minimap.GetComponent<RectTransform>().rect.width;
            var miniHeight = minimap.GetComponent<RectTransform>().rect.height;

            foreach ((int id, Station s) in stations)
            {
                RectTransform toggle = Instantiate(stTogglePrefab, minimap.transform);
                toggle.anchoredPosition = new Vector2(s.transform.position.x * miniWidth / mapWidth, s.transform.position.z * miniHeight / mapHeight);

                NamedToggle nt = toggle.GetComponent<NamedToggle>();
                nt.Id = id;
                nt.OnToggleChanged += OnTogglePressed;

                this.stationIcons.Add(id, toggle.GetComponent<Toggle>());
            }
        }

        public void OnTogglePressed(int id, bool isOn)
        {
            //Debug.Log($"id: {toggleLabelText}, val: {isOn}");

            //set stations one by one on each check and uncheck then on accept send them in that order

            if (isOn)
            {
                //i do this instead of simply adding new value to selectedStation so the added value would be always at the end of selectedStations
                //even if it were hashed in that dictionary before (i.e. was added and then removed)

                Dictionary<int, Toggle> newDict = new();
                foreach (var s in selectedStations)
                {
                    newDict[s.Key] = s.Value;
                }
                newDict.Add(id, stationIcons[id]);
                selectedStations = newDict;
            }
            else
            {
                selectedStations.Remove(id);
            }

            selectedStationsList.UpdateItems(selectedStations);

            //foreach (var s in selectedStations)
            //{
            //    Debug.Log($"id: {s.Key}, toggle: {s.Value}");
            //}
            //Debug.Log("============");
        }
    }
}
