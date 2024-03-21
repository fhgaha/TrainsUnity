using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Trains
{
    [RequireComponent(typeof(BoxCollider))]
    public class SelectableBuilding : MonoBehaviour
    {
        public static event EventHandler OnBuildingSelected;
        static SelectableBuilding curSelected;

        [Header("To Set")]
        [SerializeField] Outline outline;
        [SerializeField] MeshRenderer rend;

        [Header("To Display")]
        public bool Selected = false;
        [SerializeField] bool hovered = false;

        List<SelectableBuilding> selectables;

        private void Start()
        {
            selectables = BuildingContainer.Instance.Buildings.Select(b => b.GetComponentInChildren<SelectableBuilding>()).Where(s => s != null).ToList();
        }

        //collider should be top among nested colliders
        private void OnMouseEnter()
        {
            if (Selected) return;

            Hover();
        }

        private void OnMouseExit()
        {
            if (Selected) return;

            Dehover();
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (hovered)
                {
                    if (!Selected)
                    {
                        foreach (SelectableBuilding b in selectables.Where(s => s != this))
                        {
                            b.Deselect();
                            b.Dehover();
                        }

                        Select();
                    }
                    else if (curSelected == this)
                    {
                        Deselect();
                    }
                    else
                    {
                        Deselect();
                        Dehover();
                    }
                }
                else if (!hovered && curSelected)
                    return;
            }

            if (Selected && (Input.GetMouseButtonUp(1) || Input.GetKeyUp(KeyCode.Escape)))
            {
                //hovered = false;
                Deselect();
                Dehover();
            }

        }

        private void Hover()
        {
            rend.materials = new[] { rend.materials[0], rend.materials[0], rend.materials[0] };
            hovered = true;
        }

        private void Dehover()
        {
            rend.materials = new[] { rend.materials[0] };
            hovered = false;
        }

        private void Select()
        {
            curSelected = this;
            Selected = true;
            outline.enabled = true;
            OnBuildingSelected?.Invoke(this, EventArgs.Empty);
        }

        private void Deselect()
        {
            if (curSelected == this) curSelected = null;
            Selected = false;
            outline.enabled = false;
            OnBuildingSelected?.Invoke(this, EventArgs.Empty);
        }

    }
}
