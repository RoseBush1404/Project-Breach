using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace Breach.Controler.Cam
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private float screenRange = 20f;
        [SerializeField] private float minZoom = 10f;
        [SerializeField] private float maxZoom = 4f;
        [SerializeField] private float startingZoom = 5.5f;

        private GameObject parent;
        private CinemachineVirtualCamera virtualCamera;

        private void Start()
        {
            parent = gameObject.GetComponentInParent<Camera>().gameObject;
            virtualCamera = parent.GetComponentInChildren<CinemachineVirtualCamera>();
            virtualCamera.m_Lens.OrthographicSize = startingZoom;
        }

        void Update()
        {
            CameraPosition();
            ScrollWheelInput();
        }

        private void ScrollWheelInput()
        {
            float wheelInput = Input.GetAxis("Mouse ScrollWheel");
            virtualCamera.m_Lens.OrthographicSize = Mathf.Clamp(virtualCamera.m_Lens.OrthographicSize - wheelInput, maxZoom, minZoom);
        }

        private void CameraPosition()
        {
            if (Input.mousePosition.x >= Screen.width - screenRange) //move sceen right
            {
                transform.position = new Vector2(parent.transform.position.x + (virtualCamera.m_Lens.OrthographicSize / 3), transform.position.y);
            }
            else if (Input.mousePosition.x <= 0 + screenRange) //move screen left
            {
                transform.position = new Vector2(parent.transform.position.x - (virtualCamera.m_Lens.OrthographicSize / 3), transform.position.y);
            }
            else
            {
                transform.position = new Vector2(parent.transform.position.x, transform.position.y);
            }

            if (Input.mousePosition.y >= Screen.height - screenRange) //move screen up
            {
                transform.position = new Vector2(transform.position.x, parent.transform.position.y + (virtualCamera.m_Lens.OrthographicSize / 3));
            }
            else if (Input.mousePosition.y <= 0 + screenRange) //move screen down
            {
                transform.position = new Vector2(transform.position.x, parent.transform.position.y - (virtualCamera.m_Lens.OrthographicSize / 3));
            }
            else
            {
                transform.position = new Vector2(transform.position.x, parent.transform.position.y);
            }
        }
    }
}