using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Trains
{
    public class Carriage : MonoBehaviour, ILocoCarriageMove
    {
        [field: SerializeField] public Transform Leader { get; set; }
        [field: SerializeField] public Transform Front { get; private set; }   //same as transform
        [field: SerializeField] public Transform Back { get; private set; }    //same as joint
        [field: SerializeField] public Transform SupportFront { get; private set; }
        [field: SerializeField] public Transform SupportBack { get; private set; }
        [field: SerializeField] public CarriageCargo Cargo { get; set; }
        public int LengthIndeces => (int)(Vector3.Distance(Front.position, Back.position) / DubinsMath.driveDistance);
        public int SupportLengthIndeces => (int)(Vector3.Distance(SupportFront.position, SupportBack.position) / DubinsMath.driveDistance);
        public int FrontToSupportFrontLengthIndeces => (int)(Vector3.Distance(Front.position, SupportFront.position) / DubinsMath.driveDistance);

        [SerializeField] private ProfitText profitText;

        public Carriage Configure(Transform leader, Vector3 pos, Quaternion rot)
        {
            Leader = leader;
            transform.SetPositionAndRotation(pos, rot);
            return this;
        }

        public void UpdateManually(Vector3 backPos, float rotSpeed)
        {
            Vector3 dir = (Leader.position - backPos).normalized;
            transform.SetPositionAndRotation(
                Leader.position - (Front.position - SupportFront.position),
                Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir, Vector3.up), rotSpeed * Time.deltaTime)
            );
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawLine(SupportFront.position, SupportFront.position + Vector3.up * 5);
            Gizmos.DrawLine(SupportBack.position, SupportBack.position + Vector3.up * 5);
        }

        public void PlayProfitAnim(string text)
        {
            profitText.PlayAnim(text);
        }
    }
}
