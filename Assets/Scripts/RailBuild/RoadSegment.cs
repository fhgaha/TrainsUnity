using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Trains
{
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshRenderer))]
	public class RoadSegment : MonoBehaviour
	{
		public List<Vector3> points = new();
		[SerializeField] private Mesh2D _shape2D;
		private Mesh _mesh;

		private void Awake()
		{
			_mesh = new Mesh { name = "Segment" };
			GetComponent<MeshFilter>().sharedMesh = _mesh;
		}

		public Mesh GetMesh() => _mesh;
		public void SetMesh(Mesh mesh)
		{
			_mesh.SetVertices(mesh.vertices);
			_mesh.SetNormals(mesh.normals);
			_mesh.SetUVs(0, mesh.uv);
			_mesh.SetTriangles(mesh.triangles, 0);
		}

		public void GenerateMesh(List<Vector3> pts)
		{
			_mesh.Clear();

			//oriented points from rail builder points
			List<OrientedPoint> ops = new();
			for (int i = 0; i < pts.Count - 1; i++)
			{
				ops.Add(new OrientedPoint(pos: pts[i], forward: pts[i + 1] - pts[i]));
			}

			//verts, normals and uvs
			float uSpan = _shape2D.CalcUspan();
			List<Vector3> verts = new();
			List<Vector3> normals = new();
			List<Vector2> uvs = new();
			for (int ring = 0; ring < ops.Count; ring++)
			{
				float t = ring / (ops.Count - 1f);
				for (int i = 0; i < _shape2D.VertexCount; i++)
				{
					verts.Add(ops[ring].LocalToWorldPos(_shape2D.vertices[i].point));
					normals.Add(ops[ring].LocalToWorldVect(_shape2D.vertices[i].normal));
					uvs.Add(new Vector2(_shape2D.vertices[i].u, t * GetApproxLength(pts) / uSpan));
				}
			}

			//triangles
			List<int> triIndeces = new();
			for (int ring = 0; ring < ops.Count - 1; ring++)
			{
				int rootIndex = ring * _shape2D.VertexCount;
				int rootIndexNext = (ring + 1) * _shape2D.VertexCount;

				for (int line = 0; line < _shape2D.LineCount; line += 2)
				{
					int lineIndexA = _shape2D.lineIndices[line];
					int lineIndexB = _shape2D.lineIndices[line + 1];

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

			_mesh.SetVertices(verts);
			_mesh.SetNormals(normals);
			_mesh.SetUVs(0, uvs);
			_mesh.SetTriangles(triIndeces, 0);
		}

		private float GetApproxLength(List<Vector3> points)
		{
			return points.Count * 0.2f;

			float dist = 0f;
			for (int i = 0; i < points.Count - 1; i++)
			{
				dist += Vector3.Distance(points[i], points[i + 1]);
			}
			return dist;
		}
	}
}