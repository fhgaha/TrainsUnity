using System.Collections;
using System.Collections.Generic;
using System.Timers;
using TMPro;
using UnityEngine;

namespace Trains
{
    public class ProfitTextUsingTMPGUI : MonoBehaviour
    {
        private TextMeshProUGUI tmpGui;
        private bool animIsRunning;

        public void PlayAnim()
        {
            gameObject.SetActive(true);
            
            Debug.Log($"{gameObject} {gameObject.activeSelf} {gameObject.activeInHierarchy}");
            Debug.Log($"    {transform.parent.gameObject.activeSelf} {transform.parent.gameObject.activeInHierarchy}");


            StartCoroutine(Coroutine());
        }

        IEnumerator Coroutine()
        {
            animIsRunning = true;
            float animTime = 0;
            tmpGui.transform.position = GetScreenPos();

            while (animTime < 3)
            {
                tmpGui.transform.position += 1 * Vector3.up;
                animTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            //yield return new WaitForSeconds(3);
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

            tmpGui.transform.position = GetScreenPos();
            //Debug.Log($"{transform.parent.transform.position} -> {screenPos } -> {transform.position }");
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
