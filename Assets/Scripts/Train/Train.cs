using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    //https://gist.github.com/codeimpossible/2704498b7b78240ccb08e5234b6a557c
    public class Train : MonoBehaviour
    {
        [field: SerializeField] public TrainData Data { get; private set; }
        [field: SerializeField] public bool LoopThroughStations { get; private set; } = true;
        public List<Vector3> CurPath
        {
            get => curPath;
            set
            {
                CurPathAsString = value == pathFwd ? nameof(pathFwd) : nameof(pathBack);
                curPath = value;
            }
        }
        public string CurPathAsString;
        public int LengthIndeces
        {
            get
            {
                int lenIndcs = loco.LengthIndeces;
                carriages.ForEach(c => lenIndcs += c.LengthIndeces);
                return lenIndcs;
            }
        }

        [SerializeField] private bool keepMoving = true;
        [SerializeField] private float slowSpeed = 20;
        [SerializeField] private float maxSpeed = 50;
        [SerializeField] private float speedStep = 0.1f;
        [SerializeField] private float curSpeed = 0;
        //[SerializeField] private float rotSpeed = 10;

        private LocomotiveMove loco;
        private List<CarriageMove> carriages = new();
        private List<Vector3> curPath, pathFwd, pathBack;
        private bool isCoroutineRunning = false;

        private int curTargetIdx;
        private int slowDownDistIndeces = 30;    //assumed distance train will cover wile slowing from max to min speed

        public void Configure(TrainData data, GameObject locoPrefab, GameObject carriagePrefab)
        {
            Data = data;
            pathFwd = data.Route.PathForward;
            pathBack = data.Route.PathBack;
            CurPath = data.Route.PathForward;

            loco = Instantiate(locoPrefab, transform).GetComponent<LocomotiveMove>();
            CarriageMove car1 = Instantiate(carriagePrefab, transform).GetComponent<CarriageMove>();
            carriages.Add(car1);
            CarriageMove car2 = Instantiate(carriagePrefab, transform).GetComponent<CarriageMove>();
            carriages.Add(car2);

            //set positions in reverse order
            Quaternion startRot = Quaternion.LookRotation((pathFwd[5] - pathFwd[0]).normalized);
            car2.Configure(car1.Back, pathFwd[car2.LengthIndeces - car2.FrontToSupportFrontLengthIndeces], startRot);
            car1.Configure(loco.Back, pathFwd[car2.LengthIndeces + car1.LengthIndeces - car1.FrontToSupportFrontLengthIndeces], startRot);
            loco.Configure(pathFwd[car2.LengthIndeces + car1.LengthIndeces + loco.LengthIndeces], startRot);

            StartCoroutine(Move_Routine(4, 3));
        }

        public IEnumerator Move_Routine(float unloadTime, float loadTime)
        {
            if (isCoroutineRunning) yield break;
            isCoroutineRunning = true;

            curTargetIdx = LengthIndeces + 1;

            //do this once to set cars into in-between positions
            MoveToCurPt();
            yield return new WaitForSeconds(loadTime);

            while (keepMoving)
            {
                var distance = Vector3.Distance(loco.transform.position, CurPath[curTargetIdx]);
                if (distance * distance < 0.1f)
                    curTargetIdx++;

                if (ReachedEnd())
                {
                    if (!LoopThroughStations)
                    {
                        //dont move
                        curTargetIdx--;
                        yield return new WaitForSeconds(1);     //without this causes stack overflow
                        continue;
                    }

                    curTargetIdx = LengthIndeces + 1;
                    curSpeed = 0;

                    carriages.ForEach(c => c.PlayProfitAnim());
                    //wait till animations stopped playing

                    yield return new WaitForSeconds(unloadTime);

                    //reverse route
                    Data.Route = Data.Route.Reversed();
                    Data.Route = RouteManager.Instance.CreateRoute(
                        new List<int>{
                                Data.Route.StationFrom.GetInstanceID(),
                                Data.Route.StationTo.GetInstanceID()
                        });
                    pathFwd = Data.Route.PathForward;
                    pathBack = Data.Route.PathBack;
                    CurPath = pathFwd;

                    FlipTrain();
                    //do this once to set cars into in-between positions
                    MoveToCurPt();
                    yield return new WaitForSeconds(loadTime);
                }
                else
                {
                    MoveToCurPt();
                    yield return new WaitForEndOfFrame();
                }

                if (!keepMoving) yield return new WaitUntil(() => keepMoving);
            }
            isCoroutineRunning = false;
        }

        private bool ReachedEnd()
        {
            var d = Vector3.Distance(loco.Front.position, CurPath[^1]);
            return curTargetIdx >= CurPath.Count || d * d < 0.1f;
        }

        private void FlipTrain()
        {
            loco.transform.position = CurPath[LengthIndeces];
            loco.transform.Rotate(0, 180, 0, Space.Self);

            foreach (CarriageMove car in carriages)
            {
                car.transform.position = car.Leader.position;
                car.transform.Rotate(0, 180, 0, Space.Self);
            }
        }

        private void MoveToCurPt()
        {
            int farIdx = curTargetIdx + slowDownDistIndeces;
            if (farIdx >= curPath.Count) farIdx = curTargetIdx;

            Vector3 frontPt = curPath[curTargetIdx];
            Vector3 backPt = curPath[curTargetIdx - loco.SupportLengthIndeces];
            Vector3 nextLocoDir = (frontPt - backPt).normalized;
            Vector3 locoToFarPtDir = (curPath[farIdx] - loco.transform.position).normalized;

            float dot = Vector3.Dot(locoToFarPtDir, loco.transform.forward);
            float minCarDot = carriages.Select(c => Vector3.Dot(c.transform.forward, loco.transform.forward)).Min();
            dot = Math.Min(dot, minCarDot);

            float t = Mathf.InverseLerp(0.96f, 1f, dot);
            float reqSpeed = Mathf.Lerp(slowSpeed, maxSpeed, t);

            var remainingIdcs = curPath.Count - curTargetIdx;
            if (remainingIdcs < 30)
            {
                t = Mathf.InverseLerp(30, 0, remainingIdcs);
                curSpeed = Mathf.Lerp(slowSpeed, 0, t);
            }
            else
            {
                if (curSpeed < reqSpeed) curSpeed += speedStep;
                else
                if (curSpeed > reqSpeed) curSpeed -= speedStep;
            }

            loco.transform.SetPositionAndRotation(
                position: Vector3.MoveTowards(loco.transform.position, curPath[curTargetIdx], curSpeed * Time.deltaTime),
                rotation: Quaternion.Lerp(loco.transform.rotation, Quaternion.LookRotation(nextLocoDir), curSpeed * Time.deltaTime)
            );

            SetCarriagesPosRot();
        }

        private void SetCarriagesPosRot()
        {
            for (int i = 0; i < carriages.Count; i++)
            {
                //count length of cars before
                var trainLengthIndeces = curTargetIdx - loco.LengthIndeces;
                for (int j = 0; j < carriages.Count; j++)
                    if (j < i) trainLengthIndeces -= carriages[j].LengthIndeces;

                //calculate where back support should be placed
                int backPosIdx = trainLengthIndeces - carriages[i].FrontToSupportFrontLengthIndeces - carriages[i].SupportLengthIndeces;
                Vector3 backPos;
                if (backPosIdx < 0)
                {
                    //set rot the same as leader's
                    Vector3 fromFrontToBack = loco.SupportBack.transform.position - loco.SupportFront.transform.position;
                    backPos = carriages[i].Leader.position + fromFrontToBack;
                }
                else
                {
                    backPos = CurPath[backPosIdx];
                }

                //rot is calculated based on backPos
                carriages[i].UpdateManually(backPos, curSpeed);
            }
        }

        private int GetLocoLengthIndeces()
        {
            return loco.LengthIndeces;
        }

        private void OnDrawGizmosSelected()
        {
            ShowGizmos();
        }

        private void ShowGizmos()
        {
            if (CurPath == null || CurPath.Count == 0) return;

            for (int i = 0; i < CurPath.Count; i++)
            {
                Gizmos.color = Color.yellow;
                if (i < curTargetIdx) Gizmos.color = Color.red;
                if (i > curTargetIdx) Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(CurPath[i], 1f);
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(CurPath[curTargetIdx], CurPath[curTargetIdx] + Vector3.up * 5);
        }
    }
}
