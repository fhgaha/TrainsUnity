using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Trains
{
    public class ProfitNumber : MonoBehaviour
    {
        //[SerializeField] private Animation anim;
        private TextMeshPro tmp;
        private Animator anim;

        public void PlayAnim()
        {
            //tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, 1);
            //anim start

            gameObject.SetActive(true);
        }

        public void OnAnimationEnd()
        {
            //tmp.color = new Color(tmp.color.r, tmp.color.g, tmp.color.b, 0);
            gameObject.SetActive(false);
        }

        private void Awake()
        {
            tmp = GetComponent<TextMeshPro>();
            anim = GetComponent<Animator>();

            gameObject.SetActive(false);
        }
    }
}
