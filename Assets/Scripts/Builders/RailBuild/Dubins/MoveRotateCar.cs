using UnityEngine;

namespace Trains
{
    //Move and rotate the car we want to reach with a Dubins Path
    //Attach the script to the target car
    public class MoveRotateCar : MonoBehaviour 
    {
        //The scene's camera
        public Camera cameraObj;
	
	
	    void Update() 
	    {
            //Move the target car with the mouse
            MoveCar();

            //Rotate the target car around y
            RotateCar();
        }


        //Move the car with the mouse
        void MoveCar()
        {
            //Fire a ray from the mouse position
            Ray ray = cameraObj.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out RaycastHit hit))
			{
				//Where the the ray hot the ground?
				Vector3 hitCoordinate = hit.point;

				hitCoordinate.y = 0f;

				//Move the car to that position
				transform.position = hitCoordinate;
			}
		}


        //Rotate the car around its axis
        void RotateCar()
        {
            float rotationSpeed = 80f;

            //Rotate counter clock-wise
            if (Input.GetKey(KeyCode.Q))
            {
                transform.Rotate(-Vector3.up * Time.deltaTime * rotationSpeed);
            }
            else if (Input.GetKey(KeyCode.E))
            {
                transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed);
            }
        }
    }
}
