using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Trains
{
    public class ProfitText : MonoBehaviour
    {
        public void PlayAnim()
        {
            Vector3 rot = Camera.main.transform.eulerAngles;
            //rot.x = 0;
            rot.z = 0;
            transform.eulerAngles = rot;
            gameObject.SetActive(true); 
        }




        public void OnAnimationEnd()
        {
            gameObject.SetActive(false);
        }

        private void Awake()
        {
            gameObject.SetActive(false);
        }
    }
}
