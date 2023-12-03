using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
	[CreateAssetMenu]
	public class Mesh2D : ScriptableObject
	{
		[System.Serializable]
		public class Vertex
		{
			public Vector2 point;
			public Vector2 normal;
			public float u;    //first coordinate of uv. vs are generated.
		}

		public Vertex[] vertices;
		public int[] lineIndices;
		public int VertexCount => vertices.Length;
		public int LineCount => lineIndices.Length;


		/*
		1/sqrt(2) (or cos 45 deg?) = 0.70710678118 - its a diagonal normal direction

		shape looks like that:
		 _		  _
		| \______/ |
		|__________|
		*/

		public List<Vertex> testVertexSet = new()
		{
			new Vertex{point = new Vector2(3, 0),   normal = new Vector2(0, 1),                             u = 0.171f},
			new Vertex{point = new Vector2(3, 0),   normal = new Vector2(-0.70710678118f, 0.70710678118f),  u = 0.171f},
			new Vertex{point = new Vector2(4, 1),   normal = new Vector2(-0.70710678118f, 0.70710678118f),  u = 0.116f},
			new Vertex{point = new Vector2(4, 1),   normal = new Vector2(0, 1),                             u = 0.116f},
			new Vertex{point = new Vector2(5, 1),   normal = new Vector2(0, 1),                             u = 0.077f},
			new Vertex{point = new Vector2(5, 1),   normal = new Vector2(1, 0),                             u = 0.077f},
			new Vertex{point = new Vector2(5, -1),  normal = new Vector2(1, 0),                             u = 0f},
			new Vertex{point = new Vector2(5, -1),  normal = new Vector2(0, -1),                            u = 1f},
			new Vertex{point = new Vector2(-5, -1), normal = new Vector2(0, -1),                            u = 0.613f},
			new Vertex{point = new Vector2(-5, -1), normal = new Vector2(-1, 0),                            u = 0.574f},
			new Vertex{point = new Vector2(-5, 1),  normal = new Vector2(0, 1),                             u = 0.497f},
			new Vertex{point = new Vector2(-5, 1),  normal = new Vector2(-1, 0),                            u = 0.497f},
			new Vertex{point = new Vector2(-4, 1),  normal = new Vector2(0, 1),                             u = 0.458f},
			new Vertex{point = new Vector2(-4, 1),  normal = new Vector2(0.70710678118f, 0.70710678118f),   u = 0.458f},
			new Vertex{point = new Vector2(-3, 0),  normal = new Vector2(0.70710678118f, 0.70710678118f),   u = 0.403f},
			new Vertex{point = new Vector2(-3, 0),  normal = new Vector2(0, 1),                             u = 0.403f},
		};

		public List<int> testIndexSet = new() { 15, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 };


		/*
		  _______		  
		 /		 \
		/_________\
		*/

		public List<Vertex> trapezoidVertexSet = new()
		{
			CreateVertex(new Vector2(1, 1),     new Vector2(0, 1),                              0.1f),
			CreateVertex(new Vector2(1, 1),     new Vector2(0.70710678118f, 0.70710678118f),    0.1f),
			CreateVertex(new Vector2(2, 0),    new Vector2(0.70710678118f, 0.70710678118f),    0f),
			CreateVertex(new Vector2(2, 0),    new Vector2(0, -1),                             1f),
			CreateVertex(new Vector2(-2, 0),   new Vector2(0, -1),                             0.6f),
			CreateVertex(new Vector2(-2, 0),   new Vector2(-0.70710678118f, 0.70710678118f),   0.6f),
			CreateVertex(new Vector2(-1, 1),    new Vector2(-0.70710678118f, 0.70710678118f),   0.5f),
			CreateVertex(new Vector2(-1, 1),    new Vector2(0, 1),                              0.5f),
		};

		public List<int> trapezoidIndexSet = new() { 7, 0, 1, 2, 3, 4, 5, 6 };



		public float CalcUspan()
		{
			float dist = 0;
			for (int i = 0; i < LineCount; i += 2)
			{
				Vector2 a = vertices[lineIndices[i]].point;
				Vector2 b = vertices[lineIndices[i + 1]].point;
				dist += (a - b).magnitude;
			}
			return dist;
		}

		private static Vertex CreateVertex(Vector2 point, Vector2 normal, float u)
		{
			float scale = 1f;
			return new Vertex() { point = new Vector2(point.x * scale, point.y * scale), normal = normal, u = u };
		}
	}
}
