using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Trains
{
    public class UiStationSelector : MonoBehaviour
    {
        public EventHandler<Button> OnStationsSelected;

        [SerializeField] private Toggle station1;
        [SerializeField] private Toggle station2;
        [SerializeField] private Button accept;
        [SerializeField] private Button cancel;

        private void Start()
        {
            accept.onClick.AddListener(delegate { AcceptPressed(); });
            cancel.onClick.AddListener(delegate { CancelPressed(); });

            gameObject.SetActive(false);
        }

        private void AcceptPressed()
        {
            if (station1.isOn && station2.isOn)
            {
                OnStationsSelected?.Invoke(this, accept);
                //gameObject.SetActive(false);
            }
        }

        private void CancelPressed()
        {
            station1.isOn = false;
            station2.isOn = false;
            //gameObject.SetActive(false);
        }
    }
}
