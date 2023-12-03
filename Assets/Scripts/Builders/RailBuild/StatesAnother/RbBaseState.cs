using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public abstract class RbBaseState
    {
        public abstract void OnEnter(RbStateMachine machine);
        public abstract void UpdateState(RbStateMachine machine, bool wasHit, Vector3 hitPoint, bool lmbPressed, bool rmbPressed);
        public abstract void OnExit(RbStateMachine machine);
    }
}
