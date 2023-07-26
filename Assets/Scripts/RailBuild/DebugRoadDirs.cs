using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class DebugRoadDirs : MonoBehaviour
    {
        [SerializeField] private RailBuilder rb;
        [SerializeField] private GameObject startFirst;
        [SerializeField] private GameObject startSecond;
        [SerializeField] private GameObject endFirst;
        [SerializeField] private GameObject endSecond;

        private void Update()
        {
            var points = rb.points;
            if (points == null || points.Count < 2) return;

            var dirFromStart = (points[0] - points[1]).normalized;
            var dirFromEnd = (points[points.Count - 1] - points[points.Count - 2]).normalized;
            
            PlaceCube(ref startFirst, Color.green, points[0]);
            PlaceCube(ref startSecond, Color.yellow, points[0] + dirFromStart * 10f);
            PlaceCube(ref endFirst, Color.red, points.Last());
            PlaceCube(ref endSecond, Color.magenta, points.Last() + dirFromEnd * 10f);
        }

        private void PlaceCube(ref GameObject cube, Color color, Vector3 pos)
        {
            cube = CreateCube(cube, color);
            cube.transform.position = pos;
            cube.transform.SetParent(this.transform);
        }

        private GameObject CreateCube(GameObject go, Color color)
        {
            if (go != null) return go;  //dont create new if one already exists

            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.GetComponent<MeshRenderer>().material.color = color;
            cube.GetComponent<BoxCollider>().enabled = false;
            cube.transform.localScale = new Vector3(3f, 3f, 3f);
            return cube;
        }
    }
}
