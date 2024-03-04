using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Trains
{
    public class CameraSystem : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera cam;

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 100;
        [SerializeField] private float moveStep = 1;
        [SerializeField] private float dragPanSpeed = 0.12f;

        [Header("Rotation")]
        [SerializeField] private float rotSpeed = 200;
        [SerializeField] private float rotStep = 1;

        [Header("Edge Scroll")]
        [SerializeField] private bool edgeScrollEnabled = false;
        [SerializeField] private int edgeScrollSize = 20;

        [Header("Zoom")]
        [SerializeField] private float zoomSpeed = 10;
        [SerializeField] private float zoomStep = 3;
        [SerializeField] private float followOffsetMin = 30;
        [SerializeField] private float followOffsetMax = 200;
        [SerializeField] private AnimationCurve curve;

        private CinemachineTransposer camTransp;
        private bool dragPanActive = false;
        private Vector2 lastMousePos;
        private Vector3 inputDir = Vector3.zero;
        private Vector3 followOffset;

        void Awake()
        {
            camTransp = cam.GetCinemachineComponent<CinemachineTransposer>();
            followOffset = camTransp.m_FollowOffset;
        }

        void Update()
        {
            Move();
            Rotate();
            Zoom();
        }

        void Move()
        {
            inputDir = Vector3.zero;

            if (Input.GetKey(KeyCode.W)) inputDir.z += moveStep;
            if (Input.GetKey(KeyCode.A)) inputDir.x -= moveStep;
            if (Input.GetKey(KeyCode.S)) inputDir.z -= moveStep;
            if (Input.GetKey(KeyCode.D)) inputDir.x += moveStep;

            DragPan();
            EdgeScroll();

            Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
            transform.position += moveSpeed * Time.deltaTime * moveDir;
        }

        void Rotate()
        {
            float rotDir = 0;
            if (Input.GetKey(KeyCode.Q)) rotDir += rotStep;
            if (Input.GetKey(KeyCode.E)) rotDir -= rotStep;
            transform.eulerAngles += new Vector3(0, rotSpeed * rotDir * Time.deltaTime, 0);
        }

        void EdgeScroll()
        {
            if (!edgeScrollEnabled) return;

            if (Input.mousePosition.x < edgeScrollSize) inputDir.x -= moveStep;
            if (Input.mousePosition.y < edgeScrollSize) inputDir.z -= moveStep;
            if (Input.mousePosition.x > Screen.width - edgeScrollSize) inputDir.x += moveStep;
            if (Input.mousePosition.y > Screen.height - edgeScrollSize) inputDir.z += moveStep;
        }

        void DragPan()
        {
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                dragPanActive = true;
                lastMousePos = Input.mousePosition;
            }
            if (Input.GetKeyUp(KeyCode.Mouse1)) dragPanActive = false;

            if (dragPanActive)
            {
                Vector2 mouseMoveDelta = (Vector2)Input.mousePosition - lastMousePos;
                mouseMoveDelta *= -1;
                inputDir.x = mouseMoveDelta.x;
                inputDir.z = mouseMoveDelta.y;
                inputDir *= dragPanSpeed;
                lastMousePos = Input.mousePosition;
            }
        }

        void Zoom()
        {
            if (Input.mouseScrollDelta.y > 0) followOffset.y -= zoomStep;
            if (Input.mouseScrollDelta.y < 0) followOffset.y += zoomStep;

            followOffset.y = Mathf.Clamp(followOffset.y, followOffsetMin, followOffsetMax);
            Vector3 res = Vector3.Lerp(camTransp.m_FollowOffset, followOffset, zoomSpeed * Time.deltaTime);
            camTransp.m_FollowOffset = res;
        }
    }
}
