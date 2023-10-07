using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    //https://discussions.unity.com/t/is-there-any-trick-that-allow-us-to-use-coroutines-with-anonymous-functions-in-c/82448/4
    public static class MonoBehaviourExtension
    {
        //Usage in a behavior is this.StartCoroutine( ()=> { your code here… } );
        public static void StartMyCoroutine(this MonoBehaviour mb, Action funcs) => mb.StartCoroutine(CoroutineRunnerSimple(new Action[] { funcs }));

        public static void StartMyCoroutine(this MonoBehaviour mb, params Action[] funcs) => mb.StartCoroutine(CoroutineRunnerSimple(funcs));

        private static IEnumerator CoroutineRunnerSimple(Action[] funcs)
        {
            foreach (Action func in funcs)
            {
                func?.Invoke();

                // yield return new WaitForSeconds(.01f);
                // Thanks bunny83
                yield return null;
            }
        }
    }
}
