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
        [SerializeField] private RouteManager routeMngr;

        private void Start()
        {
            ui.OnBuildRailActivated += (sender, toggle) => railBuild.gameObject.SetActive(toggle.isOn);
            ui.OnBuildStationActivated += (sender, toggle) => stBuild.gameObject.SetActive(toggle.isOn);
            ui.OnStationsSelected += (sender, e) => CreateRouteAndSendTrain(e);
        }

        private void CreateRouteAndSendTrain(StationSelectorEventArgs e)
        {
            var path = routeMngr.CreateRoute(e.selectedIds);
            trCont.SendTrain(path);
        }

    }
}
