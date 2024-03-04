using System.Collections;
using System.Collections.Generic;
using System.Timers;
using TMPro;
using UnityEngine;

namespace Trains
{
    public class ProfitText : MonoBehaviour
    {
        [SerializeField] private AnimationCurve curve;
        private TextMeshProUGUI tmpGui;

        public void PlayAnim(string text)
        {
            tmpGui.text = text;
            gameObject.SetActive(true);
            StartCoroutine(AnimateText_Coroutine(3));
        }

        IEnumerator AnimateText_Coroutine(float timeSeconds)
        {
            float passedSeconds = 0;
            Vector3 startPos = GetScreenPos();
            tmpGui.transform.position = startPos;
            float maxDist = 50;

            while (passedSeconds < timeSeconds)
            {
                float yVal = curve.Evaluate(passedSeconds / timeSeconds) * maxDist;
                //tmpGui.transform.position = startPos + yVal * Vector3.up;
                tmpGui.transform.position = GetScreenPos() + yVal * Vector3.up;
                passedSeconds += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            OnAnimationEnd();
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
