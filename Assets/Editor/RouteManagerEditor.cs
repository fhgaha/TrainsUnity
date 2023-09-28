using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Trains
{
    [CustomEditor(typeof(RouteManager))]
    public class RouteManagerEditor : Editor
    {
        private bool showDebug;

        public override void OnInspectorGUI()
        {
            RouteManager rm = (RouteManager)target;
            showDebug = EditorGUILayout.Toggle("Show Debug", showDebug);

            if (showDebug)
            {
                rm.DebugDraw();
            }
            else
            {
                rm.DebugErase();
            }

            base.OnInspectorGUI();
        }
    }
}
