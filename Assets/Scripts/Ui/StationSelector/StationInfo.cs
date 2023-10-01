using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Trains
{
    public class StationInfo : MonoBehaviour
    {
        private RectTransform mark;
        private TextMeshProUGUI townName;
        private RectTransform freightInfo;

        private void Awake()
        {
            mark = transform.GetChild(0).GetComponent<RectTransform>();
            townName = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            freightInfo = transform.GetChild(2).GetComponent<RectTransform>();
        }

        public void Set(bool markOn, string townName, string freigthInfo)
        {
            //set mark
            this.townName.text = townName;
            //set fright info
        }
    }
}
