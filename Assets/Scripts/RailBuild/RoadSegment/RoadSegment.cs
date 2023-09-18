using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Trains
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class RoadSegment : MonoBehaviour
    {
        public Vector3 StartGlobal => Points[0] + transform.position;
        public Vector3 EndGlobal => Points[^1] + transform.position;
        
        [field: SerializeField] public List<Vector3> Points { get; set; }
        public RoadSegmentData data;
        [SerializeField] private Mesh2D shape2D;
        private Mesh mesh;
        private MeshCollider meshCollider;

        private void Awake()
        {
            Points = new List<Vector3>();
            mesh = new Mesh { name = "Segment" };
            meshCollider = GetComponent<MeshCollider>();
            GetComponent<MeshFilter>().mesh = mesh;
        }

        public void CopyPoints(RoadSegment segm) => segm.Points.ForEach(p => Points.Add(p));
        public void UpdateCollider() => meshCollider.sharedMesh = mesh;
        public void SetCollider(Mesh mesh) => meshCollider.sharedMesh = mesh;
        public Mesh GetMesh() => mesh;
        public void SetMesh(Mesh mesh)
        {
            if (mesh == null)
            {
                this.mesh.Clear(false);
                return;
            }

            this.mesh.SetVertices(mesh.vertices);
            this.mesh.SetNormals(mesh.normals);
            this.mesh.SetUVs(0, mesh.uv);
            this.mesh.SetTriangles(mesh.triangles, 0);
        }

        public void GenerateMeshSafely(List<Vector3> pts)
        {
            //https://forum.unity.com/threads/when-assigning-mesh-collider-errors-about-doesnt-have-read-write-enabled.1248541/
            
            if (pts.Count <= 0) return;
            
            GenerateMesh(pts);
            Points = pts;
        }

        public void GenerateMesh(List<Vector3> pts)
        {
            mesh.Clear();

            //oriented points from rail builder points
            List<OrientedPoint> ops = new();
            for (int i = 0; i < pts.Count - 1; i++)
            {
                ops.Add(new OrientedPoint(pos: pts[i], forward: pts[i + 1] - pts[i]));
            }

            //this fixes gaps but makes meshes to overlap
            if (pts.Count >= 2) ops.Add(new OrientedPoint(pos: pts[^1], forward: pts[^1] - pts[^2]));

            //verts, normals and uvs
            float uSpan = shape2D.CalcUspan();
            List<Vector3> verts = new();
            List<Vector3> normals = new();
            List<Vector2> uvs = new();
            for (int ring = 0; ring < ops.Count; ring++)
            {
                float t = ring / (ops.Count - 1f);
                for (int i = 0; i < shape2D.VertexCount; i++)
                {
                    verts.Add(ops[ring].LocalToWorldPos(shape2D.vertices[i].point));
                    normals.Add(ops[ring].LocalToWorldVect(shape2D.vertices[i].normal));
                    uvs.Add(new Vector2(shape2D.vertices[i].u, t * GetApproxLength(pts) / uSpan));
                }
            }

            //triangles
            List<int> triIndeces = new();
            for (int ring = 0; ring < ops.Count - 1; ring++)
            {
                int rootIndex = ring * shape2D.VertexCount;
                int rootIndexNext = (ring + 1) * shape2D.VertexCount;

                for (int line = 0; line < shape2D.LineCount; line += 2)
                {
                    int lineIndexA = shape2D.lineIndices[line];
                    int lineIndexB = shape2D.lineIndices[line + 1];

                    int currentA = rootIndex + lineIndexA;
                    int currentB = rootIndex + lineIndexB;
                    int nextA = rootIndexNext + lineIndexA;
                    int nextB = rootIndexNext + lineIndexB;

                    triIndeces.Add(currentA);
                    triIndeces.Add(nextA);
                    triIndeces.Add(nextB);

                    triIndeces.Add(currentA);
                    triIndeces.Add(nextB);
                    triIndeces.Add(currentB);
                }
            }

            mesh.SetVertices(verts);
            mesh.SetNormals(normals);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(triIndeces, 0);
        }

        private float GetApproxLength(List<Vector3> points)
        {
            return points.Count * 0.2f;

            //float dist = 0f;
            //for (int i = 0; i < points.Count - 1; i++)
            //{
            //	dist += Vector3.Distance(points[i], points[i + 1]);
            //}
            //return dist;
        }

        //private void OnCollisionEnter(Collision collision) => Debug.Log("OnCollisionEnter from RoadSegment");

        //private void OnTriggerEnter(Collider other) => Debug.Log("OnTriggerEnter from RoadSegment");

    }
}