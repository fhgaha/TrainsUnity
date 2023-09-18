using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Trains
{
    public class InputManager : MonoBehaviour
    {
        //public event EventHandler OnBuildRailActivated;

        [SerializeField] private UiMain ui;
        [SerializeField] private RailBuilder rb;
        [SerializeField] private StationBuilder sb;
        [SerializeField] private RailContainer rc;
        [SerializeField] private StationContainer sc;

        private void Start()
        {
            ui.OnBuildRailActivated += Ui_OnBuildRailActivated;
            ui.OnBuildStationActivated += Ui_OnBuildStationActivated;

        }

        private void Ui_OnBuildRailActivated(object sender, Toggle toggle) => rb.gameObject.SetActive(toggle.isOn);
        private void Ui_OnBuildStationActivated(object sender, Toggle toggle) => sb.gameObject.SetActive(toggle.isOn);

        //if rb is active send mouse position to rb
        //if rb is active send lmb/rmb clicks to rb
    }
}
