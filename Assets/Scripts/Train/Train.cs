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
        //google Steering Behavior Arrival

        [field: SerializeField] public Route Route { get; private set; }
        [field: SerializeField] public bool LoopThroughStations { get; private set; } = true;
        public IPlayer Owner { get; private set; }
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
        public int LengthIndeces => loco.LengthIndeces + carriages.Sum(c => c.LengthIndeces);

        [SerializeField] private bool keepMoving = true;
        [SerializeField] private float slowSpeed = 20;
        [SerializeField] private float maxSpeed = 50;
        [SerializeField] private float speedStep = 0.1f;
        [SerializeField] private float curSpeed = 0;
        //[SerializeField] private float rotSpeed = 10;

        private LocomotiveMove loco;
        private List<Carriage> carriages = new();
        private List<Vector3> curPath, pathFwd, pathBack;
        private bool isCoroutineRunning = false;

        private int curTargetIdx;
        private int slowDownDistIndeces = 30;    //assumed distance train will cover wile slowing from max to min speed

        public void Configure(Route route, GameObject locoPrefab, GameObject carriagePrefab, IPlayer owner)
        {
            Route = route;
            pathFwd = route.PathForward;
            pathBack = route.PathBack;
            CurPath = route.PathForward;
            Owner = owner;

            //instantiate train objs
            loco = Instantiate(locoPrefab, transform).GetComponent<LocomotiveMove>();
            Carriage car1 = Instantiate(carriagePrefab, transform).GetComponent<Carriage>();
            carriages.Add(car1);
            Carriage car2 = Instantiate(carriagePrefab, transform).GetComponent<Carriage>();
            carriages.Add(car2);

            //set pos' and rots
            //reversed order cause last car should have its back on first point of path
            Quaternion startRot = Quaternion.LookRotation((pathFwd[5] - pathFwd[0]).normalized);
            car2.Configure(car1.Back, pathFwd[car2.LengthIndeces - car2.FrontToSupportFrontLengthIndeces], startRot);
            car1.Configure(loco.Back, pathFwd[car2.LengthIndeces + car1.LengthIndeces - car1.FrontToSupportFrontLengthIndeces], startRot);
            loco.Configure(pathFwd[car2.LengthIndeces + car1.LengthIndeces + loco.LengthIndeces], startRot);

            car2.Cargo = new CarriageCargo { CargoType = CargoType.Passengers, Amnt = 5 };
            car1.Cargo = new CarriageCargo { CargoType = CargoType.Mail, Amnt = 8 };

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
                    curSpeed = 0;

                    //decimal worth = Owner.AddProfitForDeliveredCargo(Data.Cargo);
                    //Data.Route.StationTo.UnloadCargo(this);
                    //string carText = $"+{(int)worth / 2}$";

                    StartCoroutine(CarsPlayDelayedAnims_Coroutine(0.4f));
                    IEnumerator CarsPlayDelayedAnims_Coroutine(float delay)
                    {
                        foreach (var car in carriages)
                        {
                            decimal worth = Owner.AddProfitForDeliveredCargo(car.Cargo);
                            Route.StationTo.UnloadCargoFrom(car);
                            car.PlayProfitAnim($"+{(int)worth}$");
                            yield return new WaitForSeconds(delay);
                        }
                    }

                    yield return new WaitForSeconds(unloadTime);

                    if (!LoopThroughStations)
                    {
                        //dont move
                        curTargetIdx--;
                        yield return new WaitUntil(() => LoopThroughStations);
                    }

                    curTargetIdx = LengthIndeces + 1;
                    UpdateRoute();
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

        private void UpdateRoute()
        {
            //reverse route
            Route = Route.Reversed();
            Route = RouteManager.Instance.CreateRoute(
                new List<int>() {
                    Route.StationFrom.GetInstanceID(),
                    Route.StationTo.GetInstanceID()
                });
            pathFwd = Route.PathForward;
            pathBack = Route.PathBack;
            CurPath = pathFwd;
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

            foreach (Carriage car in carriages)
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
