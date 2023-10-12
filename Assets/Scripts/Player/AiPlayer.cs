using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public interface IPlayer { }

    public class AiPlayer : MonoBehaviour, IPlayer
    {
        [SerializeField] private RailBuilder rb;
        [SerializeField] private StationBuilder sb;
        [SerializeField] private Camera cam;

        private PlayerState state;

        private void Start()
        {
            rb.Parent = this;
            rb.gameObject.SetActive(true);

            //Build_I();
            //Build_I_II();
            //Build_I_fromLeftTo_I_fromRight_sameZ();
            //Build_T_fromLooseEndToConnection();
            //Build_T_fromConnectionToLooseEnd();
            //Build_H();
            //Build_C();
            //Build_IT();

            //BuildAndDestroySeveralTimes_I();
            //BuildAndDestroySeveralTimes_I_II();
            //BuildAndDestroySeveralTimes_I_fromLeftTo_I_fromRight_sameZ();
            //BuildAndDestroySeveralTimes_T_fromLooseEndToConnection();
            //BuildAndDestroySeveralTimes_T_fromConnectionToLooseEnd();
            //BuildAndDestroySeveralTimes_H();
            //BuildAndDestroySeveralTimes_C();
            //BuildAndDestroySeveralTimes_IT();

            StartCoroutine(BuildAndDestroyAll());
            IEnumerator BuildAndDestroyAll()
            {
                yield return BuildAndDestroySeveralTimes_I_Coroutine();
                yield return BuildAndDestroySeveralTimes_I_II_Coroutine();
                yield return BuildAndDestroySeveralTimes_I_fromLeftTo_I_fromRight_sameZ_Coroutine();
                yield return BuildAndDestroySeveralTimes_T_fromLooseEndToConnection_Coroutine();
                yield return BuildAndDestroySeveralTimes_T_fromConnectionToLooseEnd_Coroutine();
                yield return BuildAndDestroySeveralTimes_H_Coroutine();
                yield return BuildAndDestroySeveralTimes_C_Coroutine();
                yield return BuildAndDestroySeveralTimes_IT_Coroutine();
            }
        }

        [ContextMenu("Build simple road")] //works even on disabled gameonject
        public void Build_I()
        {
            StartCoroutine(Build());
            IEnumerator Build()
            {
                yield return BuildSegm(new(0, 0, 0), new(30, 0, 30));
            }
        }

        public void Build_I_II()
        {
            StartCoroutine(Build());
            IEnumerator Build()
            {
                yield return BuildSegm(Vector3.zero, new(20, 0, 20));
                yield return BuildSegm(new(20, 0, 20), new(20, 0, -20));
            }
        }

        public void Build_I_fromLeftTo_I_fromRight_sameZ()
        {
            StartCoroutine(Build());
            IEnumerator Build()
            {
                yield return BuildSegm(new(-20, 0, 5), new(5, 0, 5));
                yield return BuildSegm(new(50, 0, 5), new(5, 0, 5));
            }
        }

        public void Build_T_fromLooseEndToConnection()
        {
            StartCoroutine(Build());
            IEnumerator Build()
            {
                yield return BuildSegm(new(-20, 0, 5), new(20, 0, 5));
                yield return BuildSegm(new(-5, 0, -20), new(10, 0, 5));
            }
        }

        public void Build_T_fromConnectionToLooseEnd()
        {
            StartCoroutine(Build());
            IEnumerator Build()
            {
                yield return BuildSegm(new(-20, 0, 5), new(20, 0, 5));
                yield return BuildSegm(new(10, 0, 5), new(-5, 0, -20));
            }
        }

        public void Build_H()
        {
            StartCoroutine(Build());
            IEnumerator Build()
            {
                yield return BuildSegm(new(-50, 0, 30), new(50, 0, 30));
                yield return BuildSegm(new(-50, 0, -30), new(50, 0, -30));
                yield return BuildSegm(new(10, 0, -30), new(-10, 0, 30));
            }
        }

        public void Build_C()
        {
            StartCoroutine(Build());
            IEnumerator Build()
            {
                yield return BuildSegm(new(-50, 0, 30), new(50, 0, 30));
                yield return BuildSegm(new(-50, 0, -30), new(50, 0, -30));
                yield return BuildSegm(new(-50, 0, -30), new(-50, 0, 30));
                yield return BuildSegm(new(50, 0, -30), new(50, 0, 30));
            }
        }

        public void Build_IT()
        {
            StartCoroutine(Build());
            IEnumerator Build()
            {
                yield return BuildSegm(new(-50, 0, -30), new(50, 0, -30));
                yield return BuildSegm(new(10, 0, 30), new(50, 0, 30));
                yield return BuildSegm(new(10, 0, 30), new(-20, 0, -30));
            }
        }



        public void BuildAndDestroySeveralTimes_I()
        {
            var segm = (start: new Vector3(0, 0, 0), end: new Vector3(30, 0, 30));

            StartCoroutine(BuildAndDestroy());
            IEnumerator BuildAndDestroy()
            {
                for (int i = 0; i < 50; i++)
                {
                    yield return BuildSegm(segm);
                    UnbuildSegm(segm);

                    Debug.Log($"--Cicle {i + 1} finished");
                }

                Debug.Log("----BuildAndDestroySeveralTimes_C finished");
            }
        }

        public IEnumerator BuildAndDestroySeveralTimes_I_Coroutine()
        {
            var segm = (start: new Vector3(0, 0, 0), end: new Vector3(30, 0, 30));

            for (int i = 0; i < 50; i++)
            {
                yield return BuildSegm(segm);
                UnbuildSegm(segm);

                Debug.Log($"--Cicle {i + 1} finished");
            }

            Debug.Log("----BuildAndDestroySeveralTimes_C finished");
        }

        public void BuildAndDestroySeveralTimes_I_II()
        {
            Vector3 a = new(0, 0, 0);
            Vector3 b = new(20, 0, 20);
            Vector3 c = new(20, 0, -20);

            var firstSegm = (start: a, end: b);
            var secondSegm = (start: b, end: c);

            StartCoroutine(BuildAndDestroy());
            IEnumerator BuildAndDestroy()
            {
                for (int i = 0; i < 50; i++)
                {
                    yield return BuildSegm(firstSegm);
                    yield return BuildSegm(secondSegm);

                    UnbuildSegm(firstSegm);
                    UnbuildSegm(secondSegm);

                    Debug.Log($"--Cicle {i + 1} finished");
                }

                Debug.Log("----BuildAndDestroySeveralTimes_C finished");
            }
        }

        public IEnumerator BuildAndDestroySeveralTimes_I_II_Coroutine()
        {
            Vector3 a = new(0, 0, 0);
            Vector3 b = new(20, 0, 20);
            Vector3 c = new(20, 0, -20);

            var firstSegm = (start: a, end: b);
            var secondSegm = (start: b, end: c);

            for (int i = 0; i < 50; i++)
            {
                yield return BuildSegm(firstSegm);
                yield return BuildSegm(secondSegm);

                UnbuildSegm(firstSegm);
                UnbuildSegm(secondSegm);

                Debug.Log($"--Cicle {i + 1} finished");
            }

            Debug.Log("----BuildAndDestroySeveralTimes_C finished");
        }

        public void BuildAndDestroySeveralTimes_I_fromLeftTo_I_fromRight_sameZ()
        {
            var fromLeft = (start: new Vector3(-20, 0, 5), end: new Vector3(5, 0, 5));
            var fromRight = (start: new Vector3(50, 0, 5), end: new Vector3(5, 0, 5));

            StartCoroutine(BuildAndDestroy());
            IEnumerator BuildAndDestroy()
            {
                for (int i = 0; i < 50; i++)
                {
                    yield return BuildSegm(fromLeft);
                    yield return BuildSegm(fromRight);

                    UnbuildSegm(fromLeft);
                    UnbuildSegm(fromRight);

                    Debug.Log($"--Cicle {i + 1} finished");
                }

                Debug.Log("----BuildAndDestroySeveralTimes_C finished");
            }
        }

        public IEnumerator BuildAndDestroySeveralTimes_I_fromLeftTo_I_fromRight_sameZ_Coroutine()
        {
            var fromLeft = (start: new Vector3(-20, 0, 5), end: new Vector3(5, 0, 5));
            var fromRight = (start: new Vector3(50, 0, 5), end: new Vector3(5, 0, 5));

            for (int i = 0; i < 50; i++)
            {
                yield return BuildSegm(fromLeft);
                yield return BuildSegm(fromRight);
                
                UnbuildSegm(fromLeft);
                UnbuildSegm(fromRight);

                Debug.Log($"--Cicle {i + 1} finished");
            }

            Debug.Log("----BuildAndDestroySeveralTimes_C finished");
        }

        public void BuildAndDestroySeveralTimes_T_fromLooseEndToConnection()
        {
            Vector3 lineLeft = new Vector3(-20, 0, 5);
            Vector3 lineRight = new Vector3(20, 0, 5);
            Vector3 looseEnd = new Vector3(-5, 0, -20);
            Vector3 connection = new Vector3(10, 0, 5);

            var line = (start: lineLeft, end: lineRight);
            var looseEndToMid = (start: looseEnd, end: connection);

            StartCoroutine(BuildAndDestroy());
            IEnumerator BuildAndDestroy()
            {
                for (int i = 0; i < 50; i++)
                {
                    yield return BuildSegm(line);
                    yield return BuildSegm(looseEndToMid);

                    UnbuildSegm(looseEndToMid);
                    UnbuildSegm(lineLeft, connection);
                    UnbuildSegm(connection, lineRight);

                    Debug.Log($"--Cicle {i + 1} finished");
                }

                Debug.Log("----BuildAndDestroySeveralTimes_C finished");
            }
        }

        public IEnumerator BuildAndDestroySeveralTimes_T_fromLooseEndToConnection_Coroutine()
        {
            Vector3 lineLeft = new Vector3(-20, 0, 5);
            Vector3 lineRight = new Vector3(20, 0, 5);
            Vector3 looseEnd = new Vector3(-5, 0, -20);
            Vector3 connection = new Vector3(10, 0, 5);

            var line = (start: lineLeft, end: lineRight);
            var looseEndToMid = (start: looseEnd, end: connection);

            for (int i = 0; i < 50; i++)
            {
                yield return BuildSegm(line);
                yield return BuildSegm(looseEndToMid);

                UnbuildSegm(looseEndToMid);
                UnbuildSegm(lineLeft, connection);
                UnbuildSegm(connection, lineRight);

                Debug.Log($"--Cicle {i + 1} finished");
            }

            Debug.Log("----BuildAndDestroySeveralTimes_C finished");
        }

        public void BuildAndDestroySeveralTimes_T_fromConnectionToLooseEnd()
        {
            Vector3 lineLeft = new Vector3(-20, 0, 5);
            Vector3 lineRight = new Vector3(20, 0, 5);
            Vector3 looseEnd = new Vector3(-5, 0, -20);
            Vector3 connection = new Vector3(10, 0, 5);

            var line = (start: lineLeft, end: lineRight);
            var connectionToLooseEnd = (start: connection, end: looseEnd);

            StartCoroutine(BuildAndDestroy());
            IEnumerator BuildAndDestroy()
            {
                for (int i = 0; i < 50; i++)
                {
                    yield return BuildSegm(line);
                    yield return BuildSegm(connectionToLooseEnd);

                    UnbuildSegm(connectionToLooseEnd);
                    UnbuildSegm(lineLeft, connection);
                    UnbuildSegm(connection, lineRight);

                    Debug.Log($"--Cicle {i + 1} finished");
                }

                Debug.Log("----BuildAndDestroySeveralTimes_C finished");
            }
        }

        public IEnumerator BuildAndDestroySeveralTimes_T_fromConnectionToLooseEnd_Coroutine()
        {
            Vector3 lineLeft = new Vector3(-20, 0, 5);
            Vector3 lineRight = new Vector3(20, 0, 5);
            Vector3 looseEnd = new Vector3(-5, 0, -20);
            Vector3 connection = new Vector3(10, 0, 5);

            var line = (start: lineLeft, end: lineRight);
            var connectionToLooseEnd = (start: connection, end: looseEnd);

            for (int i = 0; i < 50; i++)
            {
                yield return BuildSegm(line);
                yield return BuildSegm(connectionToLooseEnd);

                UnbuildSegm(connectionToLooseEnd);
                UnbuildSegm(lineLeft, connection);
                UnbuildSegm(connection, lineRight);

                Debug.Log($"--Cicle {i + 1} finished");
            }

            Debug.Log("----BuildAndDestroySeveralTimes_C finished");
        }

        public void BuildAndDestroySeveralTimes_H()
        {
            Vector3 topLeft = new(-50, 0, 30);
            Vector3 topRight = new(50, 0, 30);
            Vector3 botLeft = new(-50, 0, -30);
            Vector3 botRight = new(50, 0, -30);
            Vector3 topMid = new(-10, 0, 30);
            Vector3 botMid = new(10, 0, -30);

            var topSegm = (start: topLeft, end: topRight);
            var botSegm = (start: botLeft, end: botRight);
            var midSegm = (start: botMid, end: topMid);

            StartCoroutine(BuildAndDestroy());
            IEnumerator BuildAndDestroy()
            {
                for (int i = 0; i < 50; i++)
                {
                    yield return BuildSegm(topSegm);
                    yield return BuildSegm(botSegm);
                    yield return BuildSegm(midSegm);

                    UnbuildSegm(midSegm);
                    UnbuildSegm(topLeft, topMid);
                    UnbuildSegm(topMid, topRight);
                    UnbuildSegm(botLeft, botMid);
                    UnbuildSegm(botMid, botRight);

                    Debug.Log($"--Cicle {i + 1} finished");
                }

                Debug.Log("----BuildAndDestroySeveralTimes_C finished");
            }
        }

        public IEnumerator BuildAndDestroySeveralTimes_H_Coroutine()
        {
            Vector3 topLeft = new(-50, 0, 30);
            Vector3 topRight = new(50, 0, 30);
            Vector3 botLeft = new(-50, 0, -30);
            Vector3 botRight = new(50, 0, -30);
            Vector3 topMid = new(-10, 0, 30);
            Vector3 botMid = new(10, 0, -30);

            var topSegm = (start: topLeft, end: topRight);
            var botSegm = (start: botLeft, end: botRight);
            var midSegm = (start: botMid, end: topMid);

            for (int i = 0; i < 50; i++)
            {
                yield return BuildSegm(topSegm);
                yield return BuildSegm(botSegm);
                yield return BuildSegm(midSegm);

                UnbuildSegm(midSegm);
                UnbuildSegm(topLeft, topMid);
                UnbuildSegm(topMid, topRight);
                UnbuildSegm(botLeft, botMid);
                UnbuildSegm(botMid, botRight);

                Debug.Log($"--Cicle {i + 1} finished");
            }

            Debug.Log("----BuildAndDestroySeveralTimes_C finished");
        }

        public void BuildAndDestroySeveralTimes_C()
        {
            Vector3 topLeft = new(-50, 0, 30);
            Vector3 topRight = new(50, 0, 30);
            Vector3 botLeft = new(-50, 0, -30);
            Vector3 botRight = new(50, 0, -30);

            var topSegm = (start: topLeft, end: topRight);
            var botSegm = (start: botLeft, end: botRight);
            var leftSegm = (start: botLeft, end: topLeft);
            var rightSegm = (start: botRight, end: topRight);

            StartCoroutine(BuildAndDestroy());
            IEnumerator BuildAndDestroy()
            {
                for (int i = 0; i < 50; i++)
                {
                    yield return BuildSegm(topSegm);
                    yield return BuildSegm(botSegm);
                    yield return BuildSegm(leftSegm);
                    yield return BuildSegm(rightSegm);

                    UnbuildSegm(topSegm);
                    UnbuildSegm(botSegm);
                    UnbuildSegm(leftSegm);
                    UnbuildSegm(rightSegm);

                    Debug.Log($"--Cicle {i + 1} finished");
                }

                Debug.Log("----BuildAndDestroySeveralTimes_C finished");
            }
        }

        public IEnumerator BuildAndDestroySeveralTimes_C_Coroutine()
        {
            Vector3 topLeft = new(-50, 0, 30);
            Vector3 topRight = new(50, 0, 30);
            Vector3 botLeft = new(-50, 0, -30);
            Vector3 botRight = new(50, 0, -30);

            var topSegm = (start: topLeft, end: topRight);
            var botSegm = (start: botLeft, end: botRight);
            var leftSegm = (start: botLeft, end: topLeft);
            var rightSegm = (start: botRight, end: topRight);

            for (int i = 0; i < 50; i++)
            {
                yield return BuildSegm(topSegm);
                yield return BuildSegm(botSegm);
                yield return BuildSegm(leftSegm);
                yield return BuildSegm(rightSegm);

                UnbuildSegm(topSegm);
                UnbuildSegm(botSegm);
                UnbuildSegm(leftSegm);
                UnbuildSegm(rightSegm);

                Debug.Log($"--Cicle {i + 1} finished");
            }

            Debug.Log("----BuildAndDestroySeveralTimes_C finished");
        }

        public void BuildAndDestroySeveralTimes_IT()
        {
            Vector3 topLeft = new(10, 0, 30);
            Vector3 topRight = new(50, 0, 30);
            Vector3 botLeft = new(-50, 0, -30);
            Vector3 botRight = new(50, 0, -30);
            Vector3 botMid = new(-20, 0, -30);

            var topSegm = (start: topLeft, end: topRight);
            var botSegm = (start: botLeft, end: botRight);
            var midSegm = (start: topLeft, end: botMid);

            StartCoroutine(BuildAndDestroy());
            IEnumerator BuildAndDestroy()
            {
                for (int i = 0; i < 50; i++)
                {
                    yield return BuildSegm(topSegm);
                    yield return BuildSegm(botSegm);
                    yield return BuildSegm(midSegm);

                    UnbuildSegm(topSegm);
                    UnbuildSegm(midSegm);
                    UnbuildSegm(botLeft, botMid);
                    UnbuildSegm(botMid, botRight);

                    Debug.Log($"--Cicle {i + 1} finished");
                }

                Debug.Log("----BuildAndDestroySeveralTimes_C finished");
            }
        }

        public IEnumerator BuildAndDestroySeveralTimes_IT_Coroutine()
        {
            Vector3 topLeft = new(10, 0, 30);
            Vector3 topRight = new(50, 0, 30);
            Vector3 botLeft = new(-50, 0, -30);
            Vector3 botRight = new(50, 0, -30);
            Vector3 botMid = new(-20, 0, -30);

            var topSegm = (start: topLeft, end: topRight);
            var botSegm = (start: botLeft, end: botRight);
            var midSegm = (start: topLeft, end: botMid);

            for (int i = 0; i < 50; i++)
            {
                yield return BuildSegm(topSegm);
                yield return BuildSegm(botSegm);
                yield return BuildSegm(midSegm);

                UnbuildSegm(topSegm);
                UnbuildSegm(midSegm);
                UnbuildSegm(botLeft, botMid);
                UnbuildSegm(botMid, botRight);

                Debug.Log($"--Cicle {i + 1} finished");
            }

            Debug.Log("----BuildAndDestroySeveralTimes_C finished");
        }

        

        private void BuildManyRndSegments()
        {
            StartCoroutine(Build());
            IEnumerator Build()
            {
                for (int i = 0; i < 100_000; i++)
                {
                    yield return BuildSegm(GetRndVector(), GetRndVector());
                }
            }

            Vector3 GetRndVector() => new(Random.value * 400 - 200, 0, Random.value * 400 - 200);
        }

        private Coroutine BuildSegm((Vector3 start, Vector3 end) segm) => StartCoroutine(rb.BuildRoad_Routine(segm.start, segm.end));
        private Coroutine BuildSegm(Vector3 from, Vector3 to) => StartCoroutine(rb.BuildRoad_Routine(from, to));
        private void UnbuildSegm((Vector3 start, Vector3 end) segm) => rb.RemoveRoad(segm.start, segm.end);
        private void UnbuildSegm(Vector3 start, Vector3 end) => rb.RemoveRoad(start, end);
    }
}
