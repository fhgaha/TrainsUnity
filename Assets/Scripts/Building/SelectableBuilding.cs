using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Trains
{
    [RequireComponent(typeof(BoxCollider))]
    public class SelectableBuilding : MonoBehaviour
    {
        [SerializeField] Outline outline;
        [SerializeField] MeshRenderer rend;


        //collider should be top among nested colliders
        private void OnMouseEnter()
        {
            //Debug.Log($"{this}: mouse entered");
            //if (EventSystem.current.IsPointerOverGameObject()) return;

            rend.materials = new[] { rend.materials[0], rend.materials[0], rend.materials[0] };
            outline.enabled = true;
        }

        private void OnMouseExit()
        {
            //Debug.Log($"{this}: mouse exited");
            //if (EventSystem.current.IsPointerOverGameObject()) return;

            rend.materials = new[] { rend.materials[0] };
            outline.enabled = false;
        }
    }
}
