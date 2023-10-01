using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Trains
{
    public class SelectedStationsList : MonoBehaviour
    {
        [SerializeField] private StationInfo stationInfoPrefab;
        [SerializeField] private RectTransform content;

        public void UpdateItems(Dictionary<int, Toggle> selectedStations)
        {
            foreach (RectTransform child in content)
            {
                Destroy(child.gameObject);
            }

            foreach (var s in selectedStations)
            {
                StationInfo item = Instantiate(stationInfoPrefab, content.transform);
                item.Set(false, s.Key.ToString(), null);
            }
        }
    }
}
