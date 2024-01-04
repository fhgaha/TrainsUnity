using System.Collections;
using UnityEngine;

namespace Trains
{
    public class ComplexRailBuilder
    {
        private RailBuilder rb;

        public ComplexRailBuilder(RailBuilder rb)
        {
            this.rb = rb;
        }

        public void Build(params Vector3[] pts)
        {
            rb.StartCoroutine(Build_Routine(pts));
        }

        public IEnumerator Build_Routine(params Vector3[] pts)
        {
            for (int i = 0; i < pts.Length - 1; i++)
            {
                yield return BuildSegm(pts[i], pts[i + 1]);
            }
        }


        //Tests----------------------------------

        public void BuildAndDestroyAllOnce()
        {
            rb.StartCoroutine(BuildAndDestroy());
            IEnumerator BuildAndDestroy()
            {
                yield return BuildAndDestroyOnce_I_Coroutine();
                yield return BuildAndDestroyOnce_I_II_Coroutine();
                yield return BuildAndDestroyOnce_I_fromLeftTo_I_fromRight_sameZ_Coroutine();
                yield return BuildAndDestroyOnce_T_fromLooseEndToConnection_Coroutine();
                yield return BuildAndDestroyOnce_T_fromConnectionToLooseEnd_Coroutine();
                yield return BuildAndDestroyOnce_H_Coroutine();
                yield return BuildAndDestroyOnce_C_Coroutine();
                yield return BuildAndDestroyOnce_IT_Coroutine();
            }
        }

        public void BuildAndDestroyAllSeveralTimes()
        {
            rb.StartCoroutine(BuildAndDestroy());
            IEnumerator BuildAndDestroy()
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

        #region simple
        public void Build_I()
        {
            rb.StartCoroutine(Build());
            IEnumerator Build()
            {
                yield return BuildSegm(new(0, 0, 0), new(30, 0, 30));
            }
        }

        public void Build_I_II()
        {
            rb.StartCoroutine(Build());
            IEnumerator Build()
            {
                yield return BuildSegm(Vector3.zero, new(20, 0, 20));
                yield return BuildSegm(new(20, 0, 20), new(20, 0, -20));
            }
        }

        public void Build_I_fromLeftTo_I_fromRight_sameZ()
        {
            rb.StartCoroutine(Build());
            IEnumerator Build()
            {
                yield return BuildSegm(new(-20, 0, 5), new(5, 0, 5));
                yield return BuildSegm(new(50, 0, 5), new(5, 0, 5));
            }
        }

        public void Build_T_fromLooseEndToConnection()
        {
            rb.StartCoroutine(Build());
            IEnumerator Build()
            {
                yield return BuildSegm(new(-20, 0, 5), new(20, 0, 5));
                yield return BuildSegm(new(-5, 0, -20), new(10, 0, 5));
            }
        }

        public void Build_T_fromConnectionToLooseEnd()
        {
            rb.StartCoroutine(Build());
            IEnumerator Build()
            {
                yield return BuildSegm(new(-20, 0, 5), new(20, 0, 5));
                yield return BuildSegm(new(10, 0, 5), new(-5, 0, -20));
            }
        }

        public void Build_H()
        {
            rb.StartCoroutine(Build());
            IEnumerator Build()
            {
                yield return BuildSegm(new(-50, 0, 30), new(50, 0, 30));
                yield return BuildSegm(new(-50, 0, -30), new(50, 0, -30));
                yield return BuildSegm(new(10, 0, -30), new(-10, 0, 30));
            }
        }

        public void Build_C()
        {
            rb.StartCoroutine(Build());
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
            rb.StartCoroutine(Build());
            IEnumerator Build()
            {
                yield return BuildSegm(new(-50, 0, -30), new(50, 0, -30));
                yield return BuildSegm(new(10, 0, 30), new(50, 0, 30));
                yield return BuildSegm(new(10, 0, 30), new(-20, 0, -30));
            }
        }
        #endregion

        #region build and destroy once
        public IEnumerator BuildAndDestroyOnce_I_Coroutine()
        {
            var segm = (start: new Vector3(0, 0, 0), end: new Vector3(30, 0, 30));

            yield return BuildSegm(segm);
            UnbuildSegm(segm);
        }

        public IEnumerator BuildAndDestroyOnce_I_II_Coroutine()
        {
            Vector3 a = new(0, 0, 0);
            Vector3 b = new(20, 0, 20);
            Vector3 c = new(20, 0, -20);

            var firstSegm = (start: a, end: b);
            var secondSegm = (start: b, end: c);

            yield return BuildSegm(firstSegm);
            yield return BuildSegm(secondSegm);

            UnbuildSegm(firstSegm);
            UnbuildSegm(secondSegm);
        }

        public IEnumerator BuildAndDestroyOnce_I_fromLeftTo_I_fromRight_sameZ_Coroutine()
        {
            var fromLeft = (start: new Vector3(-20, 0, 5), end: new Vector3(5, 0, 5));
            var fromRight = (start: new Vector3(50, 0, 5), end: new Vector3(5, 0, 5));

            yield return BuildSegm(fromLeft);
            yield return BuildSegm(fromRight);

            UnbuildSegm(fromLeft);
            UnbuildSegm(fromRight);
        }

        public IEnumerator BuildAndDestroyOnce_T_fromLooseEndToConnection_Coroutine()
        {
            Vector3 lineLeft = new Vector3(-20, 0, 5);
            Vector3 lineRight = new Vector3(20, 0, 5);
            Vector3 looseEnd = new Vector3(-5, 0, -20);
            Vector3 connection = new Vector3(10, 0, 5);

            var line = (start: lineLeft, end: lineRight);
            var looseEndToMid = (start: looseEnd, end: connection);

            yield return BuildSegm(line);
            yield return BuildSegm(looseEndToMid);

            UnbuildSegm(looseEndToMid);
            UnbuildSegm(lineLeft, connection);
            UnbuildSegm(connection, lineRight);
        }

        public IEnumerator BuildAndDestroyOnce_T_fromConnectionToLooseEnd_Coroutine()
        {
            Vector3 lineLeft = new Vector3(-20, 0, 5);
            Vector3 lineRight = new Vector3(20, 0, 5);
            Vector3 looseEnd = new Vector3(-5, 0, -20);
            Vector3 connection = new Vector3(10, 0, 5);

            var line = (start: lineLeft, end: lineRight);
            var connectionToLooseEnd = (start: connection, end: looseEnd);

            yield return BuildSegm(line);
            yield return BuildSegm(connectionToLooseEnd);

            UnbuildSegm(connectionToLooseEnd);
            UnbuildSegm(lineLeft, connection);
            UnbuildSegm(connection, lineRight);
        }

        public IEnumerator BuildAndDestroyOnce_H_Coroutine()
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

            yield return BuildSegm(topSegm);
            yield return BuildSegm(botSegm);
            yield return BuildSegm(midSegm);

            UnbuildSegm(midSegm);
            UnbuildSegm(topLeft, topMid);
            UnbuildSegm(topMid, topRight);
            UnbuildSegm(botLeft, botMid);
            UnbuildSegm(botMid, botRight);
        }

        public IEnumerator BuildAndDestroyOnce_C_Coroutine()
        {
            Vector3 topLeft = new(-50, 0, 30);
            Vector3 topRight = new(50, 0, 30);
            Vector3 botLeft = new(-50, 0, -30);
            Vector3 botRight = new(50, 0, -30);

            var topSegm = (start: topLeft, end: topRight);
            var botSegm = (start: botLeft, end: botRight);
            var leftSegm = (start: botLeft, end: topLeft);
            var rightSegm = (start: botRight, end: topRight);

            yield return BuildSegm(topSegm);
            yield return BuildSegm(botSegm);
            yield return BuildSegm(leftSegm);
            yield return BuildSegm(rightSegm);

            UnbuildSegm(topSegm);
            UnbuildSegm(botSegm);
            UnbuildSegm(leftSegm);
            UnbuildSegm(rightSegm);
        }

        public IEnumerator BuildAndDestroyOnce_IT_Coroutine()
        {
            Vector3 topLeft = new(10, 0, 30);
            Vector3 topRight = new(50, 0, 30);
            Vector3 botLeft = new(-50, 0, -30);
            Vector3 botRight = new(50, 0, -30);
            Vector3 botMid = new(-20, 0, -30);

            var topSegm = (start: topLeft, end: topRight);
            var botSegm = (start: botLeft, end: botRight);
            var midSegm = (start: topLeft, end: botMid);

            yield return BuildSegm(topSegm);
            yield return BuildSegm(botSegm);
            yield return BuildSegm(midSegm);

            UnbuildSegm(topSegm);
            UnbuildSegm(midSegm);
            UnbuildSegm(botLeft, botMid);
            UnbuildSegm(botMid, botRight);
        }
        #endregion

        #region build and destroy multiple times
        public IEnumerator BuildAndDestroySeveralTimes_I_Coroutine()
        {
            Debug.Log("----BuildAndDestroySeveralTimes_I started");

            var segm = (start: new Vector3(0, 0, 0), end: new Vector3(30, 0, 30));

            for (int i = 0; i < 50; i++)
            {
                yield return BuildSegm(segm);
                UnbuildSegm(segm);

                Debug.Log($"--Cycle {i + 1} finished");
            }

            Debug.Log("----BuildAndDestroySeveralTimes_I finished");
        }

        public IEnumerator BuildAndDestroySeveralTimes_I_II_Coroutine()
        {
            Debug.Log("----BuildAndDestroySeveralTimes_I_II started");

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

                Debug.Log($"--Cycle {i + 1} finished");
            }

            Debug.Log("----BuildAndDestroySeveralTimes_I_II finished");
        }

        public IEnumerator BuildAndDestroySeveralTimes_I_fromLeftTo_I_fromRight_sameZ_Coroutine()
        {
            Debug.Log("----BuildAndDestroySeveralTimes_I_fromLeftTo_I_fromRight_sameZ started");

            var fromLeft = (start: new Vector3(-20, 0, 5), end: new Vector3(5, 0, 5));
            var fromRight = (start: new Vector3(50, 0, 5), end: new Vector3(5, 0, 5));

            for (int i = 0; i < 50; i++)
            {
                yield return BuildSegm(fromLeft);
                yield return BuildSegm(fromRight);

                UnbuildSegm(fromLeft);
                UnbuildSegm(fromRight);

                Debug.Log($"--Cycle {i + 1} finished");
            }

            Debug.Log("----BuildAndDestroySeveralTimes_I_fromLeftTo_I_fromRight_sameZ finished");
        }

        public IEnumerator BuildAndDestroySeveralTimes_T_fromLooseEndToConnection_Coroutine()
        {
            Debug.Log("----BuildAndDestroySeveralTimes_T_fromLooseEndToConnection start");

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

                Debug.Log($"--Cycle {i + 1} finished");
            }

            Debug.Log("----BuildAndDestroySeveralTimes_T_fromLooseEndToConnection finished");
        }

        public IEnumerator BuildAndDestroySeveralTimes_T_fromConnectionToLooseEnd_Coroutine()
        {
            Debug.Log("----BuildAndDestroySeveralTimes_T_fromConnectionToLooseEnd started");

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

                Debug.Log($"--Cycle {i + 1} finished");
            }

            Debug.Log("----BuildAndDestroySeveralTimes_T_fromConnectionToLooseEnd finished");
        }

        public IEnumerator BuildAndDestroySeveralTimes_H_Coroutine()
        {
            Debug.Log("----BuildAndDestroySeveralTimes_H started");

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

                Debug.Log($"--Cycle {i + 1} finished");
            }

            Debug.Log("----BuildAndDestroySeveralTimes_H finished");
        }

        public IEnumerator BuildAndDestroySeveralTimes_C_Coroutine()
        {
            Debug.Log("----BuildAndDestroySeveralTimes_C started");

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

                Debug.Log($"--Cycle {i + 1} finished");
            }

            Debug.Log("----BuildAndDestroySeveralTimes_C finished");
        }

        public IEnumerator BuildAndDestroySeveralTimes_IT_Coroutine()
        {
            Debug.Log("----BuildAndDestroySeveralTimes_IT started");

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

                Debug.Log($"--Cycle {i + 1} finished");
            }

            Debug.Log("----BuildAndDestroySeveralTimes_IT finished");
        }
        #endregion

        private void BuildManyRndSegments()
        {
            rb.StartCoroutine(Build());
            IEnumerator Build()
            {
                for (int i = 0; i < 100_000; i++)
                {
                    yield return BuildSegm(GetRndVector(), GetRndVector());
                }
            }

            Vector3 GetRndVector() => new(Random.value * 400 - 200, 0, Random.value * 400 - 200);
        }

        private Coroutine BuildSegm((Vector3 start, Vector3 end) segm) => rb.StartCoroutine(rb.BuildRoad_Routine(segm.start, segm.end));
        private Coroutine BuildSegm(Vector3 from, Vector3 to) => rb.StartCoroutine(rb.BuildRoad_Routine(from, to));
        private void UnbuildSegm((Vector3 start, Vector3 end) segm) => rb.RemoveRoad(segm.start, segm.end);
        private void UnbuildSegm(Vector3 start, Vector3 end) => rb.RemoveRoad(start, end);
    }
}
