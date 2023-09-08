using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class Movement : MonoBehaviour
    {
        public void Move(float speed)
        {
            transform.position += speed * Time.deltaTime * transform.forward;
        }

        public void Turn(float angleSpeed)
        {
            if (Input.GetKey(KeyCode.A))
            {
                transform.Rotate(0, -angleSpeed * Time.deltaTime, 0);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Rotate(0, angleSpeed * Time.deltaTime, 0);
            }
        }
    }
}
