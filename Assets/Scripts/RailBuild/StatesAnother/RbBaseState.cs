using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public abstract class RbBaseState
    {
        public abstract void EnterState(RbStateMachine machine);
        public abstract void UpdateState(RbStateMachine machine, bool wasHit, Vector3 hitPoint, bool lmbPressed, bool rmbPressed);
    }
}
