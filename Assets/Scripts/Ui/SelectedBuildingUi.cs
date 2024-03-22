using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Trains
{
    public class SelectedBuildingUi : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI buildingType;
        [SerializeField] TextMeshProUGUI consumeType;
        [SerializeField] TextMeshProUGUI produceType;

        public void Set(string buildingType, string consumeType, string produceType)
        {
            this.buildingType.text = buildingType;
            this.consumeType.text = consumeType;
            this.produceType.text = produceType;
        }

        internal void Set(string buildingName, object consumeType, object produceType)
        {
            throw new NotImplementedException();
        }
    }
}
