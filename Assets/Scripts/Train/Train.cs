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

        [SerializeField] public bool keepMoving = true;
        private LocomotiveMove loco;
        private List<CarriageMove> carriages = new();
        private bool isCoroutineRunning = false;
        private List<Vector3> curPath;
        
        private int curTargetIdx;
        private int slowDownDistIndeces = 30;    //assumed distance train will cover wile slowing from max to min speed
        private int trainLengthIndeces;

        public void Configure(TrainData data, GameObject locoPrefab, GameObject carriagePrefab)
        {
            Data = data;

            loco = Instantiate(locoPrefab, transform).GetComponent<LocomotiveMove>();
            Vector3 locoStartDir = (data.Route.PathForward[loco.LengthIndeces] - data.Route.PathForward[0]).normalized;
            Quaternion locoStartRot = Quaternion.LookRotation(locoStartDir, Vector3.up);
            loco.Configure(data.Route.PathForward[loco.LengthIndeces], locoStartRot);

            CarriageMove wagon1 = Instantiate(carriagePrefab, transform).GetComponent<CarriageMove>().Configure(data.Route.PathForward, loco.Back);
            carriages.Add(wagon1);

            CarriageMove wagon2 = Instantiate(carriagePrefab, transform).GetComponent<CarriageMove>().Configure(data.Route.PathForward, wagon1.Back);
            carriages.Add(wagon2);

            StartCoroutine(Move_Routine(3, 3, 21));
        }

        public IEnumerator Move_Routine(float unloadTime, float loadTime, int idx)
        {
            if (isCoroutineRunning == true) yield break;
            isCoroutineRunning = true;

            CurPath = Data.Route.PathForward;
            curTargetIdx = GetTrainLengthIndeces() + 1;

            new WaitForSeconds(loadTime);

            while (keepMoving)
            {
                var distance = Vector3.Distance(loco.transform.position, CurPath[curTargetIdx]);
                if (distance * distance < 0.1f)
                {
                    curTargetIdx++;
                }

                if (ReachedEnd())
                {
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
                    MoveToCurPt();

                    yield return new WaitForEndOfFrame();
                }

                if (!keepMoving) yield return new WaitUntil(() => keepMoving);
            }
            isCoroutineRunning = false;

        }

        private void MoveToCurPt()
        {
            //not reached end
            int farIdx = curTargetIdx + slowDownDistIndeces;
            if (farIdx >= curPath.Count)
            {
                farIdx = curTargetIdx;
            }

            Vector3 frontPt = curPath[curTargetIdx];
            Vector3 backPt = curPath[curTargetIdx - loco.SupportLengthIndeces];

            Vector3 locoDir = (frontPt - backPt).normalized;
            var locoToFarPtDir = (curPath[farIdx] - loco.transform.position).normalized;
            var dot = Vector3.Dot(locoToFarPtDir, locoDir);
            float t = Mathf.InverseLerp(0.96f, 1f, dot);
            float reqSpeed = Mathf.Lerp(SlowSpeed, MaxSpeed, t);
            if (ReachingDestination()) reqSpeed = SlowSpeed;

            if (CurSpeed < reqSpeed) CurSpeed += SpeedStep;
            else
            if (CurSpeed > reqSpeed) CurSpeed -= SpeedStep;

            loco.transform.SetPositionAndRotation(
                position: Vector3.MoveTowards(loco.transform.position, curPath[curTargetIdx], CurSpeed * Time.deltaTime),
                rotation: Quaternion.Lerp(loco.transform.rotation, Quaternion.LookRotation(locoDir), CurSpeed * Time.deltaTime)
            );


            //carriages
            for (int i = 0; i < carriages.Count; i++)
            {
                //count length of cars before
                trainLengthIndeces = curTargetIdx - loco.LengthIndeces;
                for (int j = 0; j < carriages.Count; j++)
                {
                    if (j < i) trainLengthIndeces -= carriages[j].LengthIndeces;
                }

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
                carriages[i].UpdateManually(backPos, CurSpeed);
            }
        }

        private bool ReachingDestination()
        {
            float threshold = 30;
            float dist = (curPath.Count - curTargetIdx) * DubinsMath.driveDistance;
            return dist < threshold;
        }

        private bool ReachedEnd()
        {
            var d = Vector3.Distance(loco.Front.position, CurPath[^1]);
            return curTargetIdx >= CurPath.Count || d * d < 0.1f;
        }

        private int GetTrainLengthIndeces()
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
