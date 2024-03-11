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
        [field: SerializeField] private bool debug = true;
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
        public int LengthIndeces => loco.LengthIndeces + cars.Sum(c => c.LengthIndeces);
        [SerializeField] private bool keepMoving = true;

        [SerializeField] private SpeedData speed = new();
        private SpeedData speedx1 = new() { slowSpeed = 20, maxSpeed = 50, speedStep = 0.1f, curSpeed = 0 };
        private SpeedData speedx5 = new() { slowSpeed = 20 * 5, maxSpeed = 50 * 5, speedStep = 0.1f * 5, curSpeed = 0 * 5 };
        private SpeedData speedx10 = new() { slowSpeed = 20 * 10, maxSpeed = 50 * 10, speedStep = 0.1f * 10, curSpeed = 0 * 10 };

        private TrainCargoHandler cargoHandler;
        private LocomotiveMove loco;
        private List<Carriage> cars = new();
        private List<Vector3> curPath, pathFwd, pathBack;
        private bool isCoroutineRunning = false;
        private int curTargetIdx;
        private int slowDownDistIndeces = 30;    //assumed distance train will cover wile slowing from max to min speed

        private void Awake()
        {
            cargoHandler = gameObject.GetOrAddComponent<TrainCargoHandler>().Confgure(this);
        }

        private void Update()
        {
            speed = debug ? speedx10 : speedx1;
        }

        public void Configure(Route route, GameObject locoPrefab, GameObject carriagePrefab, IPlayer owner)
        {
            Route = route;
            pathFwd = route.PathForward;
            pathBack = route.PathBack;
            CurPath = route.PathForward;
            Owner = owner;

            //this thing already sustracts from StationFrom, it should happen only on load
            //List<CarCargo> cargoes = Route.GetCargoToLoad(3);

            //mock
            var cargoes = new List<CarCargo> { new CarCargo(), new CarCargo(), new CarCargo() };

            CreateLocoAndCars(locoPrefab, carriagePrefab, cargoes);
            StartCoroutine(Move_Routine());
        }

        private void CreateLocoAndCars(GameObject locoPrefab, GameObject carriagePrefab, List<CarCargo> cargoes)
        {
            //instantiate train objs
            loco = Instantiate(locoPrefab, transform).GetComponent<LocomotiveMove>();
            for (int i = 0; i < cargoes.Count; i++)
                cars.Add(Instantiate(carriagePrefab, transform).GetComponent<Carriage>());

            //set pos' and rots
            //reversed order cause last car should have its back on first point of path
            Quaternion startRot = Quaternion.LookRotation((pathFwd[5] - pathFwd[0]).normalized);
            int lenIdcs = cars[0].LengthIndeces - cars[0].FrontToSupportFrontLengthIndeces;

            for (int i = cars.Count - 1; i > 0; i--)
            {
                cars[i].Configure(cars[i - 1].Back, pathFwd[lenIdcs], startRot);
                lenIdcs += cars[0].LengthIndeces;
            }
            cars[0].Configure(loco.Back, pathFwd[lenIdcs], startRot);
            loco.Configure(pathFwd[cars[0].LengthIndeces * cars.Count + loco.LengthIndeces], startRot);
        }

        public IEnumerator Move_Routine()
        {
            if (isCoroutineRunning) yield break;
            isCoroutineRunning = true;

            curTargetIdx = LengthIndeces + 1;

            //do this once to set cars into in-between positions
            MoveToCurPt();
            yield return cargoHandler.Load_Rtn(cars);

            while (keepMoving)
            {
                var distance = Vector3.Distance(loco.transform.position, CurPath[curTargetIdx]);
                if (distance * distance < 0.1f)
                    curTargetIdx++;

                if (ReachedEnd())
                {
                    speed.curSpeed = 0;

                    yield return cargoHandler.Unload_Rtn(cars);

                    if (!LoopThroughStations)
                    {
                        //dont move
                        curTargetIdx--;
                        yield return new WaitUntil(() => LoopThroughStations);
                    }

                    //reverse train and load
                    curTargetIdx = LengthIndeces + 1;
                    UpdateRoute();
                    FlipTrain();
                    //do this once to set cars into in-between positions
                    MoveToCurPt();

                    yield return cargoHandler.Load_Rtn(cars);

                }

                MoveToCurPt();
                yield return new WaitForEndOfFrame();

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

            foreach (Carriage car in cars)
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
            float minCarDot = cars.Select(c => Vector3.Dot(c.transform.forward, loco.transform.forward)).Min();
            dot = Math.Min(dot, minCarDot);

            float t = Mathf.InverseLerp(0.96f, 1f, dot);
            float reqSpeed = Mathf.Lerp(speed.slowSpeed, speed.maxSpeed, t);

            var remainingIdcs = curPath.Count - curTargetIdx;
            if (remainingIdcs < 30)
            {
                t = Mathf.InverseLerp(30, 0, remainingIdcs);
                speed.curSpeed = Mathf.Lerp(speed.slowSpeed, 0, t);
            }
            else
            {
                if (speed.curSpeed < reqSpeed) speed.curSpeed += speed.speedStep;
                else
                if (speed.curSpeed > reqSpeed) speed.curSpeed -= speed.speedStep;
            }

            loco.transform.SetPositionAndRotation(
                position: Vector3.MoveTowards(loco.transform.position, curPath[curTargetIdx], speed.curSpeed * Time.deltaTime),
                rotation: Quaternion.Lerp(loco.transform.rotation, Quaternion.LookRotation(nextLocoDir), speed.curSpeed * Time.deltaTime)
            );

            SetCarriagesPosRot();
        }

        private void SetCarriagesPosRot()
        {
            for (int i = 0; i < cars.Count; i++)
            {
                //count length of cars before
                var trainLengthIndeces = curTargetIdx - loco.LengthIndeces;
                for (int j = 0; j < cars.Count; j++)
                    if (j < i) trainLengthIndeces -= cars[j].LengthIndeces;

                //calculate where back support should be placed
                int backPosIdx = trainLengthIndeces - cars[i].FrontToSupportFrontLengthIndeces - cars[i].SupportLengthIndeces;
                Vector3 backPos;
                if (backPosIdx < 0)
                {
                    //set rot the same as leader's
                    Vector3 fromFrontToBack = loco.SupportBack.transform.position - loco.SupportFront.transform.position;
                    backPos = cars[i].Leader.position + fromFrontToBack;
                }
                else
                {
                    backPos = CurPath[backPosIdx];
                }

                //rot is calculated based on backPos
                cars[i].UpdateManually(backPos, speed.curSpeed);
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
