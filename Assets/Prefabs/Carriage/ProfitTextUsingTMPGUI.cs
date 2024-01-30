using System.Collections;
using System.Collections.Generic;
using System.Timers;
using TMPro;
using UnityEngine;

namespace Trains
{
    public class ProfitTextUsingTMPGUI : MonoBehaviour
    {
        [SerializeField] private AnimationCurve curve;
        private TextMeshProUGUI tmpGui;
        private bool animIsRunning;

        public void PlayAnim()
        {
            gameObject.SetActive(true);
            StartCoroutine(AnimateText_Coroutine(3));
        }

        IEnumerator AnimateText_Coroutine(float timeSeconds)
        {
            animIsRunning = true;
            float animTime = 0;
            Vector3 startPos = GetScreenPos();
            tmpGui.transform.position = startPos;
            float dist = 50;

            while (animTime < timeSeconds)
            {
                float nextVal = curve.Evaluate(animTime / timeSeconds) * dist;

                tmpGui.transform.position = startPos + nextVal * Vector3.up;
                animTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            OnAnimationEnd();
            animIsRunning = false;
        }

        public void OnAnimationEnd()
        {
            gameObject.SetActive(false);
        }

        private void Awake()
        {
            tmpGui = GetComponentInChildren<TextMeshProUGUI>();
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (animIsRunning) return;
        }

        private Vector3 GetScreenPos()
        {
            Vector3 carPos =
                transform.parent    //canvas
                .transform.parent   //carriage
                .position;
            Vector3 screenPos = Camera.main.WorldToViewportPoint(carPos);
            return new Vector2(1920 * screenPos.x, 1080 * screenPos.y + 20);
        }
    }
}
