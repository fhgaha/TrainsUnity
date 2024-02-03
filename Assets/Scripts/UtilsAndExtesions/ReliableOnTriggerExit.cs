
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://forum.unity.com/threads/fix-ontriggerexit-will-now-be-called-for-disabled-gameobjects-colliders.657205/
// OnTriggerExit is not called if the triggering object is destroyed, set inactive, or if the collider is disabled. This script fixes that
//
// Usage: Wherever you read OnTriggerEnter() and want to consistently get OnTriggerExit
// In OnTriggerEnter() call ReliableOnTriggerExit.NotifyTriggerEnter(other, gameObject, OnTriggerExit);
// In OnTriggerExit call ReliableOnTriggerExit.NotifyTriggerExit(other, gameObject);
//
// Algorithm: Each ReliableOnTriggerExit is associated with a collider, which is added in OnTriggerEnter via NotifyTriggerEnter
// Each ReliableOnTriggerExit keeps track of OnTriggerEnter calls
// If ReliableOnTriggerExit is disabled or the collider is not enabled, call all pending OnTriggerExit calls

//Usage example:
//    private void OnTriggerEnter(Collider other)
//    {
//        ReliableOnTriggerExit.NotifyTriggerEnter(other, gameObject, OnTriggerExit);
//        Debug.Log("OnTriggerEnter");
//    }

//    private void OnTriggerExit(Collider other)
//    {
//        ReliableOnTriggerExit.NotifyTriggerExit(other, gameObject);
//        Debug.Log("OnTriggerExit");
//    }
//}
public class ReliableOnTriggerExit : MonoBehaviour
{
    public delegate void _OnTriggerExit(Collider c);

    Collider thisCollider;
    bool ignoreNotifyTriggerExit = false;

    // Target callback
    Dictionary<GameObject, _OnTriggerExit> waitingForOnTriggerExit = new Dictionary<GameObject, _OnTriggerExit>();

    public static void NotifyTriggerEnter(Collider otherColl, GameObject caller, _OnTriggerExit onTriggerExit)
    {
        ReliableOnTriggerExit curComp = null;
        ReliableOnTriggerExit[] otherComps = otherColl.gameObject.GetComponents<ReliableOnTriggerExit>();

        foreach (ReliableOnTriggerExit otherComp in otherComps)
        {
            if (otherComp.thisCollider == otherColl)
            {
                curComp = otherComp;
                break;
            }
        }

        if (curComp == null)
        {
            curComp = otherColl.gameObject.AddComponent<ReliableOnTriggerExit>();
            curComp.thisCollider = otherColl;
        }

        // Unity bug? (!!!!): Removing a Rigidbody while the collider is in contact will call OnTriggerEnter twice,
        // so I need to check to make sure it isn't in the list twice
        // In addition, force a call to NotifyTriggerExit so the number of calls to OnTriggerEnter and OnTriggerExit match up
        if (curComp.waitingForOnTriggerExit.ContainsKey(caller) == false)
        {
            curComp.waitingForOnTriggerExit.Add(caller, onTriggerExit);
            curComp.enabled = true;
        }
        else
        {
            curComp.ignoreNotifyTriggerExit = true;
            curComp.waitingForOnTriggerExit[caller].Invoke(otherColl);
            curComp.ignoreNotifyTriggerExit = false;
        }
    }

    public static void NotifyTriggerExit(Collider otherCol, GameObject caller)
    {
        if (otherCol == null) return;

        ReliableOnTriggerExit curComp = null;
        ReliableOnTriggerExit[] otherComps = otherCol.gameObject.GetComponents<ReliableOnTriggerExit>();

        foreach (ReliableOnTriggerExit otherComp in otherComps)
        {
            if (otherComp.thisCollider == otherCol)
            {
                curComp = otherComp;
                break;
            }
        }

        if (curComp != null && curComp.ignoreNotifyTriggerExit == false)
        {
            curComp.waitingForOnTriggerExit.Remove(caller);
            if (curComp.waitingForOnTriggerExit.Count == 0)
            {
                curComp.enabled = false;
            }
        }
    }

    private void OnDisable()
    {
        if (gameObject.activeInHierarchy == false)
            CallCallbacks();
    }

    private void Update()
    {
        if (thisCollider == null)
        {
            // Will GetOnTriggerExit with null, but is better than no call at all
            CallCallbacks();

            Component.Destroy(this);
        }
        else if (thisCollider.enabled == false)
        {
            CallCallbacks();
        }
    }

    void CallCallbacks()
    {
        ignoreNotifyTriggerExit = true;
        foreach (var v in waitingForOnTriggerExit)
        {
            if (v.Key == null)
            {
                continue;
            }

            v.Value.Invoke(thisCollider);
        }
        ignoreNotifyTriggerExit = false;
        waitingForOnTriggerExit.Clear();
        enabled = false;
    }
}