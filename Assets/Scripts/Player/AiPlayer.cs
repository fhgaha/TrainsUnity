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
            Build_I_fromLeft_to_I_fromRight_sameZ();  
            //Build_T_fromLooseEndToConnection();
            //Build_T_fromConnectionToLooseEnd();
        }

        [ContextMenu("Build simple road")] //works even on disabled gameonject
        public void Build_I()
        {
            StartCoroutine(Build());
            IEnumerator Build()
            {
                yield return BuildSegm(new Vector3(0, 0, 0), new Vector3(30, 0, 30));
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

        public void Build_I_fromLeft_to_I_fromRight_sameZ()
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
                yield return BuildSegm(new Vector3(-20, 0, 5), new Vector3(20, 0, 5));
                yield return BuildSegm(new Vector3(-5, 0, -20), new Vector3(10, 0, 5));
            }
        }

        public void Build_T_fromConnectionToLooseEnd()
        {
            StartCoroutine(Build());
            IEnumerator Build()
            {
                yield return BuildSegm(new Vector3(-20, 0, 5), new Vector3(20, 0, 5));
                yield return BuildSegm(new Vector3(10, 0, 5), new Vector3(-5, 0, -20));
            }
        }



        private Coroutine BuildSegm(Vector3 p1, Vector3 p2) => StartCoroutine(rb.BuildRoad_Routine(p1, p2));

       
    }
}
