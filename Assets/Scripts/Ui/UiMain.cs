using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Trains
{
    public enum UiState
    {
        Default, BuildRailIsPressed,
    }

    public class UiMain : MonoBehaviour
    {
        public UiState State { get; private set; }

        public event EventHandler<Toggle> OnBuildRailActivated;

        //toggle-buttons
        [SerializeField] private Toggle buildRail;

        private void Start()
        {
            buildRail.onValueChanged.AddListener(delegate { BuildRailValueChanged(buildRail); });
            //subscriptions to other toggle events
        }

        //TODO: would be nice if on rmb rb became untoggled

        private void BuildRailValueChanged(Toggle buildRail)
        {
            if (buildRail.isOn)
                State = UiState.BuildRailIsPressed;
            else
                State = UiState.Default;

            OnBuildRailActivated?.Invoke(this, buildRail);
        }
    }
}
