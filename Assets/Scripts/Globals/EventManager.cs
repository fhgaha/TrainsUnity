using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Trains
{
    public class EventManager : MonoBehaviour
    {
        public event EventHandler<Toggle> OnBuildRailPressed;
        public static EventManager Instance { get; private set; }

        [SerializeField] private UiMain ui;
        [SerializeField] private RailBuilder railBuild;
        [SerializeField] private StationBuilder stBuild;
        [SerializeField] private RailContainer railCont;
        [SerializeField] private StationContainer stCont;
        [SerializeField] private TrainContainer trCont;
        [SerializeField] private RouteManager routeMngr;

        private void Awake()
        {
            if (Instance == null)
            {
                DontDestroyOnLoad(gameObject);
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            //ui.OnBuildRailActivated += (sender, toggle) => railBuild.gameObject.SetActive(toggle.isOn);
            ui.OnBuildRailActivated += (sender, toggle) => OnBuildRailPressed?.Invoke(this, toggle);
            ui.OnBuildStationActivated += (sender, toggle) => stBuild.gameObject.SetActive(toggle.isOn);
            ui.OnStationsSelectedAcceptPressed += (sender, e) => CreateRouteAndSendTrain(e);
        }

        private void CreateRouteAndSendTrain(StationSelectorEventArgs e)
        {
            Route r = routeMngr.CreateRoute(e.selectedIds);
            trCont.SendTrain(r, e.selectedBy);
        }

    }
}
