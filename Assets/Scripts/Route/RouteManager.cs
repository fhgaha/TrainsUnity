using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Trains
{
    public class RouteManager : MonoBehaviour
    {
        public static RouteManager Instance { get; private set; }
        private Graph graph = new();

        public List<Vector3> CreateRoute(List<int> selectedIds)
        {
            List<Vector3> finalPath = new();
            StationContainer sc = Global.Instance.StationContainer;
            TrainContainer tc = Global.Instance.TrainContainer;

            Station[] stations = selectedIds.Select(s => sc.Stations[s]).ToArray();
            for (int i = 0; i < stations.Length - 1; i++)
            {
                //lats take only enties 1 for now
                Node from = graph.AllNodes.First(n => n.Pos == stations[i].Entry1);
                Node to = graph.AllNodes.First(n => n.Pos == stations[i + 1].Entry1);
                var points = graph.RunDijkstraGetPath(from, to);
                if (points.Count == 0) return new List<Vector3>();

                finalPath.AddRange(points);
            }

            //if we want to go back to start point
            if (finalPath.Count >= 2)
            {
                var finish = graph.AllNodes.First(n => finalPath[^1] == n.Pos);
                var start = graph.AllNodes.First(n => finalPath[0] == n.Pos);
                var pathBack = graph.RunDijkstraGetPath(finish, start);
                finalPath.AddRange(pathBack);
            }

            return finalPath;
        }

        void Awake()
        {
            if (Instance == null)
            {
                DontDestroyOnLoad(gameObject);
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        //Connection options:
        //I  - simplest road, new road no start or end snapped
        //II - connection to a road end, new road start (or end) snapped
        //T  - connection to the middle a road, new road start (or end) snapped
        //IT - connection from one road end to another road middle, new road start and end snapped
        //H  - connection from one road middle to another road middle, new road start and end snapped
        //C  - connection from one road end to another road end, new road start and end snapped


        //A     B
        //-------

        //A: create, add neigh B
        //B: create, add neigh A
        //AB: create, set length
        public void RegisterI(Vector3 start, Vector3 end, float length)
        {
            Debug.Log($"RegisterI called");

            Node a = CreateNode(start);
            Node b = CreateNode(end);
            a.AddNeigh(b);
            b.AddNeigh(a);
            Edge ab = CreateEdge(a, b, length);

            RegisterNode(a); RegisterNode(b);
            RegisterEdge(ab);

            //DebugDraw();
        }

        //public void UnregisterI(Vector3 start, Vector3 end)
        //{
        //    Node a = GetNode(start);
        //    Node b = GetNode(end);

        //    RemoveNode(a);
        //    RemoveNode(b);
        //}


        //A     B     C       BC is new road
        //------*------

        //C: create, add neigh B 
        //B: add neigh C
        //BC: create, set length
        public void RegisterII(Vector3 newPos, Vector3 otherPos, float length)
        {
            Debug.Log($"RegisterII called");

            Node b = GetNode(otherPos);
            Node c = CreateNode(newPos);
            b.AddNeigh(c);
            c.AddNeigh(b);
            Edge bc = CreateEdge(b, c, length);

            RegisterNode(c);
            RegisterEdge(bc);

            //DebugDraw();
        }


        // D       B        C           AB is new road
        //---------*---------
        //         |
        //         |
        //         A

        //A: create, add neigh B
        //B: create, add neigh A, C, D
        //C: remove neigh D, add neigh B
        //D: remove neigh C, add neigh B
        //CD: remove
        //AB, BC, BD: create, set length
        public void RegisterT(
            Vector3 newSegmStart, Vector3 connection, float newSegmLength,
            Vector3 edgeToRemoveStart, Vector3 edgeToRemoveEnd,
            float newEdge1Length, float newEdge2Length
        )
        {
            Debug.Log($"RegisterT called");

            Node a = CreateNode(newSegmStart);
            Node b = CreateNode(connection);
            Node c = GetNode(edgeToRemoveStart);
            Node d = GetNode(edgeToRemoveEnd);
            a.AddNeigh(b);
            b.AddNeigh(a).AddNeigh(c).AddNeigh(d);
            c.RemoveNeigh(d).AddNeigh(b);
            d.RemoveNeigh(c).AddNeigh(b);
            RemoveEdge(c, d);
            Edge ab = CreateEdge(a, b, newSegmLength);
            Edge bc = CreateEdge(b, c, newEdge1Length);
            Edge bd = CreateEdge(b, d, newEdge2Length);

            RegisterNode(a); RegisterNode(b);
            RegisterEdge(ab); RegisterEdge(bc); RegisterEdge(bd);

            //DebugDraw();
        }

        // A       E        B           EF is new road
        //---------*---------
        //         |
        //         |
        //---------*---------
        //C        F        D

        //A: remove neigh B, add neigh E
        //B: remove neigh A, add neigh E
        //C: remove neigh D, add neigh F
        //D: remove neigh C, add neigh F
        //E: create, add neigh A,B,F
        //F: create, add neigh C,D,E
        //AB, CD: remove
        //AE, EB, CF, FD, EF: create, set length
        public void RegisterH(
            Vector3 ePos, Vector3 fPos, float efLength,
            Vector3 aPos, float aeLength,
            Vector3 bPos, float ebLength,
            Vector3 cPos, float cfLength,
            Vector3 dPos, float fdLength
        )
        {
            Debug.Log($"RegisterH called");

            Node a = GetNode(aPos);
            Node b = GetNode(bPos);
            Node c = GetNode(cPos);
            Node d = GetNode(dPos);
            Node e = CreateNode(ePos);
            Node f = CreateNode(fPos);
            a.RemoveNeigh(b).AddNeigh(e);
            b.RemoveNeigh(a).AddNeigh(e);
            c.RemoveNeigh(d).AddNeigh(f);
            d.RemoveNeigh(c).AddNeigh(f);
            e.AddNeigh(a).AddNeigh(b).AddNeigh(f);
            f.AddNeigh(c).AddNeigh(d).AddNeigh(e);
            RemoveEdge(a, b); RemoveEdge(c, d);
            Edge ae = CreateEdge(a, e, aeLength);
            Edge eb = CreateEdge(e, b, ebLength);
            Edge cf = CreateEdge(c, f, cfLength);
            Edge fd = CreateEdge(f, d, fdLength);
            Edge ef = CreateEdge(e, f, efLength);

            RegisterNode(e); RegisterNode(f);
            RegisterEdge(ae); RegisterEdge(eb); RegisterEdge(cf); RegisterEdge(fd); RegisterEdge(ef);

            //DebugDraw();
        }


        //      A         B           AC is new road
        //      *---------
        //      |
        //      |
        //      *---------
        //      C         D

        //A: add neigh C
        //C: add neigh A
        //AC: create, set length
        public void RegisterC(Vector3 aPos, Vector3 cPos, float newSegmLength)
        {
            Debug.Log($"RegisterC called");

            Node a = GetNode(aPos);
            Node c = GetNode(cPos);
            a.AddNeigh(c);
            c.AddNeigh(a);
            Edge ac = CreateEdge(a, c, newSegmLength);

            RegisterEdge(ac);

            //DebugDraw();
        }


        //        C             DC is new road
        //        *------
        //       /   
        //      /      
        //-----*---------
        //A    D        B

        //A: remove neigh B, add neigh D
        //B: remove neigh A, add neigh D
        //C: add neigh D
        //AB: remove
        //AD, DB, DC: create, set length
        public void RegisterIT(
            Vector3 dPos, float adLength, float dbLength, float dcLength,
            Vector3 aPos, Vector3 bPos, Vector3 cPos
        )
        {
            Debug.Log($"RegisterIT called");

            Node a = GetNode(aPos);
            Node b = GetNode(bPos);
            Node c = GetNode(cPos);
            Node d = CreateNode(dPos);
            a.RemoveNeigh(b).AddNeigh(d);
            b.RemoveNeigh(a).AddNeigh(d);
            c.AddNeigh(d);
            d.AddNeigh(a).AddNeigh(b).AddNeigh(c);
            RemoveEdge(a, b);
            Edge ad = CreateEdge(a, d, adLength);
            Edge db = CreateEdge(d, b, dbLength);
            Edge dc = CreateEdge(d, c, dcLength);

            RegisterNode(d);
            RegisterEdge(ad); RegisterEdge(db); RegisterEdge(dc);

            //DebugDraw();
        }

        private Node CreateNode(Vector3 pos) => new Node(pos);
        private Edge CreateEdge(Node n, Node m, float length) => new Edge { Node1 = n, Node2 = m, Length = length };
        private Node GetNode(Vector3 pos) => graph.AllNodes.First(n => n.Pos == pos);
        private Edge GetEdge(Node a, Node b) => graph.AllEdges.First(e => e.Contains(a, b));

        public void RegisterEdge(Edge edge)
        {
            if (graph.AllEdges.Contains(edge)
                //|| graph.AllEdges.Any(edge => edge.Contains(n, m))
                )
            {
                throw new Exception($"Attempting to register edge {edge} that already registered. Operation terminated.");
            }
            graph.AllEdges.Add(edge);
        }

        public void RegisterNode(Node n)
        {
            if (graph.AllNodes.Contains(n))
            {
                throw new Exception($"Attempting to register node {n} that already registered. Operation terminated.");
            }
            graph.AllNodes.Add(n);
        }

        public void RemoveNode(Node n) => graph.AllNodes.Remove(n);

        private void RemoveEdge(Node c, Node d)
        {
            Edge cd = graph.AllEdges.First(e => e.Contains(c, d));
            graph.AllEdges.Remove(cd);
        }

        public void UnregisterEdge(Vector3 start, Vector3 end)
        {
            Node a = GetNode(start);
            Node b = GetNode(end);

            a.RemoveNeigh(b);
            b.RemoveNeigh(a);
            RemoveEdge(a, b);

            if (a.Neighbours.Count == 0) RemoveNode(a);
            if (b.Neighbours.Count == 0) RemoveNode(b);
        }

        public void DebugDraw()
        {
            //return;

            DebugErase();

            foreach (var n in graph.AllNodes)
            {
                DrawRayWithText(n.Pos, 20 * Vector3.up, Color.yellow, float.PositiveInfinity, n.ToString());
            }

            foreach (var e in graph.AllEdges)
            {
                DebugDrawEdge(e);
            }

            //draw station labels
            foreach (var s in Global.Instance.StationContainer.Stations)
            {
                DebugDrawText(s.Value.transform.position, 30 * Vector3.up, $"Station: {s.Key}");
            }
        }

        private void DebugDrawEdge(Edge e)
        {
            var line = new GameObject();
            line.transform.parent = transform;
            line.name = "DebugLine " + line.GetInstanceID();
            var lr = line.AddComponent<LineRenderer>();
            lr.SetPositions(new[] { e.Node1.Pos + 5 * Vector3.up, e.Node2.Pos + 5 * Vector3.up });
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = Color.green;
            lr.endColor = Color.green;
        }

        private void DrawRayWithText(Vector3 start, Vector3 dir, Color color, float duration, string text)
        {
            Debug.DrawRay(start, dir, color, duration);
            DebugDrawText(start, dir / 2, text);
        }

        private void DebugDrawText(Vector3 start, Vector3 dir, string text)
        {
            GameObject textObj = new();
            textObj.transform.parent = transform;
            textObj.transform.position = start + dir;
            textObj.name = $"DebugText {textObj.GetInstanceID()}";

            TextMesh tm = textObj.AddComponent<TextMesh>();
            tm.fontSize = 56;
            tm.color = Color.black;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.text = text;
        }

        public void DebugErase()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
