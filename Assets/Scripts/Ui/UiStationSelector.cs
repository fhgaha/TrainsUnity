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
        [SerializeField] private Button accept;
        [SerializeField] private Button cancel;

        private Dictionary<int, Toggle> stations = new();

        private void Awake()
        {
            accept.onClick.AddListener(delegate { OnAcceptPressed(); });
            cancel.onClick.AddListener(delegate { OnCancelPressed(); });
        }

        private void OnAcceptPressed()
        {
            //send which stations were selected
            if (stations.All(s => !s.Value.isOn)) return;

            List<int> list = new();
            foreach (var s in stations)
            {
                if (s.Value.isOn)
                {
                    list.Add(s.Key);
                }
            }
            OnStationsSelected?.Invoke(this, new StationSelectorEventArgs { selectedIds = list }); ;
        }

        private void OnCancelPressed()
        {
            foreach (var s in stations)
            {
                s.Value.GetComponent<Toggle>().isOn = false;
            }
        }

        public void SetStationIcons(Dictionary<int, Station> stations)
        {
            foreach (Transform child in minimap.transform)
            {
                Destroy(child.gameObject);
            }
            this.stations = new();

            float hOffset = 0;
            foreach (var s in stations)
            {
                //set relative position for s.Pos with some scale lets say 1/100
                //for now lets place them in row

                RectTransform toggle = Instantiate(stTogglePrefab, minimap.transform);
                toggle.anchoredPosition = new Vector2(0, -hOffset);
                hOffset += toggle.rect.height;

                this.stations.Add(s.Key, toggle.GetComponent<Toggle>());
            }
        }
    }
}
