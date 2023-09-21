using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public struct Node
    {
        public Vector3 Pos;
        public List<Node> Neighbours;

        public Node(Vector3 pos)
        {
            Pos = pos;
            Neighbours = new();
        }

        public Node AddNeigh(Node n)
        {
            if (Neighbours.Contains(n)) throw new Exception($"{this}: Trying add the same neighbour {n} second time");
            Neighbours.Add(n);
            return this;
        }
        public Node RemoveNeigh(Node n)
        {
            Neighbours.Remove(n);
            return this;
        }

        #region boilerplate 
        public static bool operator ==(Node a, Node b) => a.Equals(b);
        public static bool operator !=(Node a, Node b) => !a.Equals(b);
        public override int GetHashCode() => HashCode.Combine(Pos);
        //i have decided to use pos only, since i dont want to be two nodes with different neighbours but same pos ever exist
        public override bool Equals(object obj) => obj is Node node && Pos.Equals(node.Pos);    
        public override string ToString() => $"Node: {Pos}";
        #endregion boilerplate 
    }

    public class Edge
    {
        public Node Node1, Node2;
        public float Length;

        public bool Contains(Node node1, Node node2)
            => (Node1 == node1 && Node2 == node2)
            || (Node1 == node2 && Node2 == node1);

        public override string ToString() => $"Edge: {Node1}, {Node2}, length: {Length}";
    }

    public class Graph
    {
        public List<Node> AllNodes = new();
        public List<Edge> AllEdges = new();

        public (float[], int[]) Dijkstra(int sourceIdx, int targetIdx)
        {
            float[] dist = new float[AllNodes.Count];      //distances
            int[] prev = new int[AllNodes.Count];          //node indeces of total found shortest path
            List<int> Q = new();                           //unvisited nodes indeces

            for (int v = 0; v < AllNodes.Count; v++)
            {
                dist[v] = float.PositiveInfinity;
                prev[v] = -1;
                Q.Add(v);
            }
            dist[sourceIdx] = 0;

            while (Q.Count > 0)
            {
                int u = GetMinDistNodeIdx(dist, Q);
                if (u == targetIdx) return (dist, prev);
                Q.Remove(u);

                for (int v = 0; v < AllNodes[u].Neighbours.Count; v++)
                {
                    if (!Q.Contains(v)) continue;

                    float alt = dist[v] + GetEdgeLength(u, v);
                    if (alt < dist[v])
                    {
                        dist[v] = alt;
                        prev[v] = u;
                    }
                }
            }
            return (dist, prev);
        }

        private static int GetMinDistNodeIdx(float[] dist, List<int> Q)
        {
            int u = -1;
            float minDist = float.PositiveInfinity;
            for (int i = 0; i < Q.Count; i++)
            {
                int someIdx = Q[i];
                float someDist = dist[someIdx];
                if (someDist < minDist)
                {
                    minDist = someDist;
                    u = i;
                }
            }
            return u;
        }

        public float GetEdgeLength(int u, int v)
        {
            foreach (Edge edge in AllEdges)
            {
                if (edge.Contains(AllNodes[u], AllNodes[v])) return edge.Length;
            }
            return float.PositiveInfinity;
        }

        private void CopyArrayElements(Node[] copyFrom, Node[] copyTo)
        {
            for (int i = 0; i < copyFrom.Length; i++)
            {
                copyTo[i] = copyFrom[i];
            }
        }
    }
}
