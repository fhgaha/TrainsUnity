using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Collections;

namespace Trains
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class RoadSegment : MonoBehaviour
    {
        [field: SerializeField] public List<Vector3> Points { get; set; }
        [field: SerializeField] public bool IsBlueprint { get; set; } = true;
        public string OwnerAsString = "---";
        public IPlayer Owner
        {
            get => owner;
            set
            {
                OwnerAsString = value?.GetType().Name;
                owner = value;
            }
        }
        private IPlayer owner;

        public Vector3 Start = Vector3.zero;
        public Vector3 End = Vector3.zero;

        [SerializeField] private Mesh2D shape2D;
        [SerializeField] private Material allowedMaterial;
        [SerializeField] private Material defaultMaterial;
        [SerializeField] private Material forbiddenMaterial;

        private Mesh mesh;
        private MeshCollider meshCollider;

        public event EventHandler OnColliderSharedMeshUpdated;
        public override string ToString() => $"RoadSegm {GetInstanceID()}";

        private void Awake()
        {
            Points = new List<Vector3>();
            mesh = new Mesh { name = "Segment" };
            meshCollider = GetComponent<MeshCollider>();
            gameObject.layer = LayerMask.NameToLayer("Road");
            GetComponent<MeshFilter>().mesh = mesh;

            OnColliderSharedMeshUpdated += OnColliderSharedMeshUpdatedHandle;
        }

        Dictionary<int, Collider> detected = new();

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out RoadSegment rs)
                 && IsBlueprint
                 && Owner is HumanPlayer)
            {
                //Debug.Log($"RoadSegment.OnTriggerEnter, {transform.GetInstanceID()} detected {rs.transform.GetInstanceID()}");
                //detected[other.GetInstanceID()] = other;
                //    ReliableOnTriggerExit.NotifyTriggerEnter(other, gameObject, OnTriggerExit);
                //BecomeRed();

            }
        }

        //does not get called cause collider mesh is new every time?
        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out RoadSegment rs)
                && IsBlueprint
                && Owner is HumanPlayer)
            {
                //Debug.LogWarning($"RoadSegment.OnTriggerExit, {transform.GetInstanceID()} detected {rs.transform.GetInstanceID()}");
                //    ReliableOnTriggerExit.NotifyTriggerExit(other, gameObject);
                //BecomeGreen();
            }
        }

        //private void OnTriggerStay(Collider other)
        //{
        //    if (IsBlueprint && Owner is HumanPlayer)
        //        if (other.TryGetComponent(out RoadSegment rs))
        //        {
        //            Debug.Log($"RoadSegment.OnTriggerStay, {transform.GetInstanceID()} detected {rs.transform.GetInstanceID()}");
        //        }
        //}

        private void OnColliderSharedMeshUpdatedHandle(object sender, EventArgs e)
        {
            if (IsBlueprint)
            {
                foreach (KeyValuePair<int, Collider> pair in detected)
                {
                    //OnTriggerExit(pair.Value);
                }
                detected.Clear();
            }
        }

        public void DestroyRigBodyCopyAndPlace(RoadSegment from)
        {
            Destroy(GetComponent<Rigidbody>());

            CopyPointsByValue(from);
            SetMesh(from.GetMesh());
            TrySetCollider(from.GetMesh());
            name = $"Road Segment {GetInstanceID()}";
            Start = from.Start;
            End = from.End;
            Owner = from.Owner;

            IsBlueprint = false;
            meshCollider.isTrigger = false;
            meshCollider.convex = false;
        }

        public void GenerateMeshAndSetPoints(List<Vector3> pts, IPlayer owner)
        {
            GenerateMesh(pts);
            TrySetCollider(GetComponent<MeshFilter>().mesh);
            name = $"Road Segment {GetInstanceID()}";
            SetPointsAndOwner(pts, owner);
        }

        public void SetPointsAndOwner(List<Vector3> pts, IPlayer owner)
        {
            Points = pts;
            Start = pts[0];
            End = pts[^1];
            Owner = owner;
        }

        public void PlaceFromStationBuilder()
        {
            IsBlueprint = false;
            meshCollider.isTrigger = false;
            //meshCollider.convex = false;
        }

        public static float GetApproxLength(List<Vector3> points)
        {
            return points.Count * Global.Instance.DriveDistance;

            //float dist = 0f;
            //for (int i = 0; i < points.Count - 1; i++)
            //{
            //	dist += Vector3.Distance(points[i], points[i + 1]);
            //}
            //return dist;
        }

        public void CopyPointsByValue(RoadSegment from)
        {
            Points.Clear();
            from.Points.ForEach(p => Points.Add(p));
        }

        public bool TryUpdateCollider()
        {
            if (mesh.vertexCount > 0)
            {
                ///generate box shape for mesh collider


                meshCollider.sharedMesh = mesh;


                //if (meshCollider.sharedMesh is null)
                //{
                //    meshCollider.sharedMesh = mesh;
                //}
                //else
                //{
                //    meshCollider.sharedMesh.SetVertices(mesh.vertices);
                //    meshCollider.sharedMesh.SetNormals(mesh.normals);
                //    meshCollider.sharedMesh.SetUVs(0, mesh.uv);
                //    meshCollider.sharedMesh.SetTriangles(mesh.triangles, 0);

                //    meshCollider.sharedMesh.RecalculateNormals();
                //    meshCollider.sharedMesh.RecalculateBounds();
                //    meshCollider.sharedMesh.RecalculateTangents();
                //    meshCollider.sharedMesh.RecalculateUVDistributionMetrics();
                //}


                //OnColliderSharedMeshUpdated?.Invoke(this, EventArgs.Empty);
                return true;
            }
            return false;

            //if (meshCollider.sharedMesh is null) meshCollider.sharedMesh = mesh;
            //if (mesh.vertexCount > 0)
            //{
            //    //meshCollider.sharedMesh.SetVertices(mesh.vertices);
            //    //meshCollider.sharedMesh.SetNormals(mesh.normals);
            //    //meshCollider.sharedMesh.SetUVs(0, mesh.uv);
            //    //meshCollider.sharedMesh.SetTriangles(mesh.triangles, 0);

            //    meshCollider.sharedMesh.vertices = mesh.vertices;
            //    meshCollider.sharedMesh.normals = mesh.normals;
            //    meshCollider.sharedMesh.uv = mesh.uv;
            //    meshCollider.sharedMesh.triangles = mesh.triangles;
            //    return true;
            //}
            //return false;
        }

        public bool TrySetCollider(Mesh mesh)
        {
            if (mesh.vertexCount > 0)
            {
                meshCollider.sharedMesh = mesh;
                return true;
            }
            return false;
        }

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

        public void UpdateMeshAndCollider(List<Vector3> pts)
        {
            //https://forum.unity.com/threads/when-assigning-mesh-collider-errors-about-doesnt-have-read-write-enabled.1248541/

            if (pts.Count <= 0) return;

            GenerateMesh(pts);
            Points = pts;
            TryUpdateCollider();
        }

        public void GenerateMesh(List<Vector3> pts)
        {
            mesh.Clear();

            (List<Vector3> verts, List<Vector3> normals, List<Vector2> uvs, List<int> triIndeces) = GenerateMeshValues(pts);

            mesh.SetVertices(verts);
            mesh.SetNormals(normals);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(triIndeces, 0);
        }

        public (List<Vector3> verts, List<Vector3> normals, List<Vector2> uvs, List<int> triIndeces) GenerateMeshValues(List<Vector3> pts)
        {
            //oriented points from rail builder points
            List<OrientedPoint> ops = new();
            for (int i = 0; i < pts.Count - 1; i++)
            {
                ops.Add(new OrientedPoint(pos: pts[i], forward: pts[i + 1] - pts[i]));
            }

            if (pts.Count >= 2)
            {
                Vector3 dir = pts[^1] - pts[^2];
                //!myFix: last point was ignored lets try to add it some value
                ops.Add(new OrientedPoint(pos: pts[^1], forward: dir));

                //!myFix: draw one end of mesh one DriveDistance longer. This causes meshes to overlap, but not gaps anymore
                ops.Add(new OrientedPoint(pos: ops[^1].pos + Global.Instance.DriveDistance * dir, forward: dir));
            }

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

            //last set of triangles is ignored?

            return (verts, normals, uvs, triIndeces);
        }

        public float GetApproxLength() => GetApproxLength(Points);

        public bool IsPointSnappedOnEnding(Vector3 point) => Start == point || End == point;

        public void BecomeGreen() => GetComponent<MeshRenderer>().material = allowedMaterial;

        public void BecomeRed() => GetComponent<MeshRenderer>().material = forbiddenMaterial;

        public void BecomeDefaultColor() => GetComponent<MeshRenderer>().material = defaultMaterial;
    }
}