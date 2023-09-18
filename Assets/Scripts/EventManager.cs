using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Trains
{
    public class EventManager : MonoBehaviour
    {
        //public event EventHandler OnBuildRailActivated;

        [SerializeField] private UiMain ui;
        [SerializeField] private RailBuilder railBuild;
        [SerializeField] private StationBuilder stBuild;
        [SerializeField] private RailContainer railCont;
        [SerializeField] private StationContainer stCont;
        [SerializeField] private TrainContainer trCont;

        private void Start()
        {
            ui.OnBuildRailActivated += (sender, toggle) => railBuild.gameObject.SetActive(toggle.isOn);
            ui.OnBuildStationActivated += (sender, toggle) => stBuild.gameObject.SetActive(toggle.isOn);
            ui.OnStationsSelected += (sender, e) => trCont.SendTrain(e.from, e.to);
        }

        //if rb is active send mouse position to rb
        //if rb is active send lmb/rmb clicks to rb
    }
}
