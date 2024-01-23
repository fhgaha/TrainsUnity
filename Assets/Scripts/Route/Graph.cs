using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class Node
    {
        public Vector3 Pos;
        public List<Node> Neighbours;
        public IPlayer Owner;

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
        public static bool operator ==(Node a, Node b) => a is not null && a.Equals(b);
        public static bool operator !=(Node a, Node b) => a is not null && !a.Equals(b);
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

        public List<Vector3> RunDijkstraGetPath(Node from, Node to)
        {
            RailContainer rc = Global.Instance.RailContainer;
            (_, Dictionary<Node, Node> prev) = Dijkstra(from, to);
            List<Node> S = new();
            Node u = to;
            if (prev[u] is not null || u == from)
            {
                while (u is not null)
                {
                    S.Insert(0, u);
                    u = prev[u];
                }
            }

            List<Vector3> points = rc.GetRoadsAsPoints(S);
            return points;
        }

        public (Dictionary<Node, float> dist, Dictionary<Node, Node> prev) Dijkstra(Node source, Node target)
        {
            Dictionary<Node, float> dist = new(AllNodes.Count); //<node, distance between source and that node>
            Dictionary<Node, Node> prev = new(AllNodes.Count);  //<We came to this node, from this node>
            List<Node> unvisited = new(AllNodes.Count);         //unvisited nodes indeces

            foreach (Node v in AllNodes)
            {
                dist[v] = float.PositiveInfinity;
                prev[v] = null;
                unvisited.Add(v);
            }
            dist[source] = 0;

            while (unvisited.Count > 0)
            {
                Node u = GetMinDistUnvisitedNode(dist, unvisited);
                if (u is null || u == target)
                {
                    //prev[u] = u;
                    return (dist, prev);
                }
                unvisited.Remove(u);

                foreach (Node v in u.Neighbours)
                {
                    if (!unvisited.Contains(v)) continue;

                    float alt = dist[u] + GetEdgeLength(u, v);
                    if (alt < dist[v])
                    {
                        dist[v] = alt;
                        prev[v] = u;
                    }
                }
            }
            return (dist, prev);
        }

        private float GetEdgeLength(Node u, Node v)
        {
            foreach (var e in AllEdges)
            {
                if (e.Contains(u, v)) return e.Length;
            }

            return float.PositiveInfinity;
        }

        private static Node GetMinDistUnvisitedNode(Dictionary<Node, float> dist, List<Node> unvisited)
        {
            float minDist = float.PositiveInfinity;
            Node u = unvisited[0];
            foreach (var n in unvisited)
            {
                if (dist[n] < minDist)
                {
                    minDist = dist[n];
                    u = n;
                }
            }
            return u;
        }

    }
}
