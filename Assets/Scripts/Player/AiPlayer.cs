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
            Build_C();
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


        //other alternative is to use update for that somehow. Hey, itll be better forperformance!
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

        public void BuildManyRndSegments()
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

        private Coroutine BuildSegm(Vector3 from, Vector3 to) => StartCoroutine(rb.BuildRoad_Routine(from, to));

    }
}
