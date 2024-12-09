using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 10f;

    [SerializeField]
    private Vector2 minBound, maxBound;

    [SerializeField]
    private float zoomSpeed = 10f;

    [SerializeField]
    private float minZoom = 10f, maxZoom = 30f;

    [SerializeField]
    private float edgeBorder = 10f;

    private float currentZoom = 20f;

    //private Vector3 lastMousePosition;

    private void Update()
    {
        HandleMovement();
        HandleZoom();
    }

    private void HandleMovement()
    {
        Vector3 moveDirection = Vector3.zero;

        if(Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - edgeBorder)
        {
            moveDirection += Vector3.forward;
        }

        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= edgeBorder)
        {
            moveDirection += Vector3.back;
        }

        if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= edgeBorder)
        {
            moveDirection += Vector3.left;
        }

        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - edgeBorder)
        {
            moveDirection += Vector3.right;
        }

        Vector3 newPosition = transform.position + moveDirection * moveSpeed * Time.deltaTime;

        newPosition.x = Mathf.Clamp(newPosition.x,minBound.x,maxBound.x);
        newPosition.z = Mathf.Clamp(newPosition.z,minBound.y,maxBound.y);

        newPosition.y = transform.position.y;

        transform.position = newPosition;
    }

    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scroll * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        Camera.main.fieldOfView = currentZoom;
    }
}
