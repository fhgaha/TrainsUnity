using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class CarriageMove : MonoBehaviour
    {
        [SerializeField] private Transform supportFront;
        [SerializeField] private Transform supportBack;
        public Transform leader;
        public int LengthIndeces
        {
            get
            {
                var distBetweenSupports = Vector3.Distance(supportFront.position, supportBack.position);
                return (int)(distBetweenSupports / DubinsMath.driveDistance);
            }
        }

        private List<Vector3> path;

        public CarriageMove Configure(List<Vector3> path, Transform leader)
        {
            this.leader = leader;
            this.path = path;
            return this;
        }

        public void UpdateManually(Vector3 backPos)
        {
            Vector3 dir = (leader.position - backPos).normalized;
            transform.SetPositionAndRotation(
                leader.position,
                Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir, Vector3.up), 10 * Time.deltaTime)
            );
        }
    }
}
