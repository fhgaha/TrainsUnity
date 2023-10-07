using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class RegisterHelper : MonoBehaviour
    {
        private RoadSegment segment;
        private RailContainer railContainer;

        public void Configure(RoadSegment segment, RailContainer railContainer)
        {
            this.segment = segment;
            this.railContainer = railContainer;
        }

        public void RegisterI(Vector3 start, Vector3 end) => RouteManager.Instance.RegisterI(start, end, segment.GetApproxLength());

        public void RegisterII(Vector3 newNodePos, Vector3 nodeWeConnectedToPos) => RouteManager.Instance.RegisterII(newNodePos, nodeWeConnectedToPos, segment.GetApproxLength());

        public void RegisterT(Vector3 start, Vector3 connection, RoadSegment otherRoad, List<Vector3> pts)
        {
            (RoadSegment segment1, RoadSegment segment2) = SplitSegment(otherRoad, connection);

            RouteManager.Instance.RegisterT(
                start, connection, RoadSegment.GetApproxLength(pts),
                otherRoad.Start, otherRoad.End,
                segment1.GetApproxLength(), segment2.GetApproxLength()
            );

            railContainer.Remove(otherRoad);
            railContainer.AddDontCreateInstance(segment1);
            railContainer.AddDontCreateInstance(segment2);
        }

        public void RegisterH(
            RoadSegment oldRoad1, Vector3 oldRoad1Connection,
            RoadSegment oldRoad2, Vector3 oldRoad2Connection,
            RoadSegment newSegm
        )
        {
            //oldRoad2.points does not contain oldRoad2Connection
            (RoadSegment ae, RoadSegment eb) = SplitSegment(oldRoad1, oldRoad1Connection);
            (RoadSegment cf, RoadSegment fd) = SplitSegment(oldRoad2, oldRoad2Connection);

            RouteManager.Instance.RegisterH(
                oldRoad1Connection, oldRoad2Connection, newSegm.GetApproxLength(),
                oldRoad1.Start, ae.GetApproxLength(),
                oldRoad1.End, eb.GetApproxLength(),
                oldRoad2.Start, cf.GetApproxLength(),
                oldRoad2.End, fd.GetApproxLength()
            );

            railContainer.Remove(oldRoad1);
            railContainer.Remove(oldRoad2);
            railContainer.AddDontCreateInstance(ae);
            railContainer.AddDontCreateInstance(eb);
            railContainer.AddDontCreateInstance(cf);
            railContainer.AddDontCreateInstance(fd);
        }

        public void RegisterC(RoadSegment newRoad) => RegisterC(newRoad.Start, newRoad.End, newRoad.GetApproxLength());
        public void RegisterC(Vector3 start, Vector3 end, float length) => RouteManager.Instance.RegisterC(start, end, length);

        public void RegisterIT(RoadSegment roadMidConnected, RoadSegment newRoad, Vector3 connection)
        {
            (RoadSegment ad, RoadSegment db) = SplitSegment(roadMidConnected, connection);
            Vector3 end = connection == newRoad.End ? newRoad.Start : newRoad.End;

            RouteManager.Instance.RegisterIT(
                connection, ad.GetApproxLength(), db.GetApproxLength(), newRoad.GetApproxLength(),
                roadMidConnected.Start, roadMidConnected.End, end
            );

            railContainer.Remove(roadMidConnected);
            railContainer.AddDontCreateInstance(ad);
            railContainer.AddDontCreateInstance(db);
        }

        private (RoadSegment, RoadSegment) SplitSegment(RoadSegment roadToSplit, Vector3 splitPt)
        {
            RoadSegment segment1 = Instantiate(roadToSplit, railContainer.transform);
            RoadSegment segment2 = Instantiate(roadToSplit, railContainer.transform);

            (List<Vector3> splitted1, List<Vector3> splitted2) = SplitPointsInTwoSets(roadToSplit.Points, splitPt);

            segment1.ConfigureFrom(splitted1);
            segment2.ConfigureFrom(splitted2);

            return (segment1, segment2);
        }

        private (List<Vector3>, List<Vector3>) SplitPointsInTwoSets(List<Vector3> originalPts, Vector3 splitPt)
        {
            List<Vector3> newPts1 = new();
            List<Vector3> newPts2 = new();

            bool sendToFirst = true;
            foreach (var p in originalPts)
            {
                if (p == splitPt)
                {
                    sendToFirst = false;
                    newPts1.Add(p);
                }

                if (sendToFirst)
                    newPts1.Add(p);
                else
                    newPts2.Add(p);
            }
            return (newPts1, newPts2);
        }

        private float GetPartialLength(Vector3 from, Vector3 to, List<Vector3> pts)
        {
            if (from != pts[0] && from != pts[^1]) throw new Exception($"Point {from} is not start or end of points {pts}");

            List<Vector3> foundPts = new();
            for (int i = 0; i < pts.Count; i++)
            {
                if (pts[i] != from) break;

                foundPts.Add(pts[i]);
                if (pts[i] == to) break;
            }

            for (int i = pts.Count - 1; i >= 0; i--)
            {
                if (pts[i] != from) break;

                foundPts.Add(pts[i]);
                if (pts[i] == to) break;
            }
            float startToConnectionLength = RoadSegment.GetApproxLength(foundPts);
            return startToConnectionLength;
        }
    }
}
