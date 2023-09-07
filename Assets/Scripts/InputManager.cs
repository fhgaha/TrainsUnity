using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class InputManager : MonoBehaviour
    {
        public event EventHandler OnBuildRailActivated;

        [SerializeField] private UiMain ui;
        [SerializeField] private RailBuilder rb;

        private void Start()
        {
            ui.OnBuildRailActivated += Ui_OnBuildRailActivated;
        }

        private void Ui_OnBuildRailActivated(object sender, UnityEngine.UI.Toggle toggle)
        {
            rb.gameObject.SetActive(toggle.isOn);
        }

        //if rb is active send mouse position to rb
        //if rb is active send lmb/rmb clicks to rb
    }
}
