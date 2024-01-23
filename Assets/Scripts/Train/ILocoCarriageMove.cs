using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public interface ILocoCarriageMove
    {
        public Transform Front { get; }
        public Transform Back { get; }
        public Transform SupportFront { get; }
        public Transform SupportBack { get; }

        public int LengthIndeces => (int)(Vector3.Distance(Front.position, Back.position) / DubinsMath.driveDistance);
        public int SupportLengthIndeces => (int)(Vector3.Distance(SupportFront.position, SupportBack.position) / DubinsMath.driveDistance);
        public int FrontToSupportFrontLengthIndeces => (int)(Vector3.Distance(Front.position, SupportFront.position) / DubinsMath.driveDistance);

    }
}
