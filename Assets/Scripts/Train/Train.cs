using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    //https://gist.github.com/codeimpossible/2704498b7b78240ccb08e5234b6a557c
    public class Train : MonoBehaviour
    {
        [field: SerializeField] public TrainData Data { get; private set; }
        public List<Vector3> CurPath
        {
            get => curPath;
            set
            {
                CurPathAsString = value == Data.Route.PathForward ? nameof(Data.Route.PathForward) : nameof(Data.Route.PathBack);
                curPath = value;
            }
        }
        public bool LoopThroughPoints { get; private set; } = true;

        public float SlowSpeed = 5;
        public float MaxSpeed = 20;
        public float SpeedStep = 0.1f;
        public float CurSpeed = 0;
        public float RotSpeed = 10;

        public string CurPathAsString;

        private LocomotiveMove loco;
        private List<CarriageMove> wagons = new();
        private bool isCoroutineRunning = false;
        private List<Vector3> curPath;
        private bool KeepMoving = true;
        private int curTargetIdx;

        public void Configure(TrainData data, GameObject locoPrefab, GameObject carriagePrefab)
        {
            Data = data;

            loco = Instantiate(locoPrefab, transform).GetComponent<LocomotiveMove>();
            loco.Configure(data.Route.PathForward[loco.LengthIndeces]);
            CarriageMove wagon1 = Instantiate(carriagePrefab, transform).GetComponent<CarriageMove>().Configure(data.Route.PathForward, loco.Joint);
            wagons.Add(wagon1);

            //StartCoroutine(wagon1.Move_Routine(3, 3, 11));
            StartCoroutine(Move_Routine(3, 3, 21));
        }

        private void Update()
        {
            //foreach (var car in wagons)
            //{
            //    //unreadable
            //    //TODO better LengthIndices
            //    int wagon1BackPosIdx = curTargetIdx - loco.LengthIndeces - car.LengthIndeces;
            //    Vector3 wagon1BackPos;
            //    if (wagon1BackPosIdx < 0)
            //    {
            //        Vector3 displasement = loco.SupportBack.transform.position - loco.SupportFront.transform.position;
            //        wagon1BackPos = loco.SupportFront.transform.position + displasement;
            //    }
            //    else
            //    {
            //        wagon1BackPos = CurPath[wagon1BackPosIdx];
            //    }
            //    car.UpdateManually(wagon1BackPos);
            //}
        }

        public IEnumerator Move_Routine(float unloadTime, float loadTime, int idx)
        {
            if (isCoroutineRunning == true) yield break;
            isCoroutineRunning = true;

            CurPath = Data.Route.PathForward;
            curTargetIdx = GetTrainLengthIndeces() + 1;

            new WaitForSeconds(loadTime);

            while (KeepMoving)
            {
                var distance = Vector3.Distance(loco.transform.position, CurPath[curTargetIdx]);
                if (distance * distance < 0.1f)
                {
                    curTargetIdx++;
                }

                if (curTargetIdx >= CurPath.Count)
                {
                    //reached end
                    if (LoopThroughPoints)
                    {
                        curTargetIdx = GetTrainLengthIndeces() + 1;
                        CurPath = CurPath == Data.Route.PathForward ? Data.Route.PathBack : Data.Route.PathForward;
                        yield return new WaitForSeconds(unloadTime);

                        //flip train instantly
                        //TODO LocoFlipRotOnStation
                        loco.transform.position = CurPath[curTargetIdx];
                        loco.transform.Rotate(0, 180, 0, Space.Self);
                        yield return new WaitForSeconds(loadTime);
                    }
                    else
                    {
                        //dont move
                        curTargetIdx--;
                    }
                }
                else
                {
                    //not reached end
                    //loco.MoveFurther(curTargetIdx);

                    Vector3 frontPt = curPath[curTargetIdx];
                    Vector3 backPt = curPath[curTargetIdx - loco.LengthIndeces];
                    Vector3 dir = (frontPt - backPt).normalized;
                    var dot = Vector3.Dot(loco.transform.forward, (frontPt - backPt).normalized);
                    float t = Mathf.InverseLerp(0.96f, 1f, dot);
                    float reqSpeed = Mathf.Lerp(SlowSpeed, MaxSpeed, t);
                    if (ReachingDestination()) reqSpeed = SlowSpeed;
                    CurSpeed += (CurSpeed > reqSpeed ? -1 : 1) * SpeedStep;

                    //TODO move this to loco
                    loco.transform.SetPositionAndRotation(
                        position: Vector3.MoveTowards(loco.transform.position, curPath[curTargetIdx], CurSpeed * Time.deltaTime),
                        rotation: Quaternion.Lerp(loco.transform.rotation, Quaternion.LookRotation(dir), CurSpeed * Time.deltaTime)
                    );


                    //carriages
                    foreach (var car in wagons)
                    {
                        //unreadable
                        //TODO better LengthIndices
                        int wagon1BackPosIdx = curTargetIdx - loco.LengthIndeces - car.LengthIndeces - 5;
                        Vector3 wagon1BackPos;
                        if (wagon1BackPosIdx < 0)
                        {
                            Vector3 fromFrontToBack = loco.SupportBack.transform.position - loco.SupportFront.transform.position;
                            wagon1BackPos = car.Leader.position + fromFrontToBack;
                        }
                        else
                        {
                            wagon1BackPos = CurPath[wagon1BackPosIdx];
                        }
                        car.UpdateManually(wagon1BackPos);
                    }








                    yield return new WaitForEndOfFrame();
                }
            }
            isCoroutineRunning = false;

        }

        private bool ReachingDestination()
        {
            float threshold = 30;
            float dist = (curPath.Count - curTargetIdx) * DubinsMath.driveDistance;
            return dist < threshold;
        }

        private int GetTrainLengthIndeces()
        {
            return loco.LengthIndeces;
        }
    }
}
