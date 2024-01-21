using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class CarriageMove : MonoBehaviour
    {
        public Transform leader;
        [SerializeField] private Transform supportFront;
        [SerializeField] private Transform supportBack;

        private List<Vector3> path;

        public CarriageMove Configure(List<Vector3> path, Transform leader)
        {
            this.leader = leader;
            this.path = path;
            return this;
        }

        public void UpdateManually(Vector3 v)
        {
            Vector3 dir = (leader.position - v).normalized;
            transform.SetPositionAndRotation(
                leader.position,
                Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), 10 * Time.deltaTime)
            );
        }
    }
}
