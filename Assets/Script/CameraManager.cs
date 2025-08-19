// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
// using System;
// using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    //  private float defaultSize = 5;
    // private float zoom;
    // private float zoomMultiplier = 4f;
    // private float minZoom = 2f;
    // private float maxZoom = 8f;
    // private float velocity = 0f;
    // private float smoothTime = 0.25f;

    private Camera mainCamera;
    // [SerializeField] private Camera cam;


    // private Vector3 _origin;
    // private Vector3 _difference;

    // private bool _isDragging;

    private void Start()
    {
        // cam = Camera.main;
        // zoom = cam.orthographicSize;

        // Fit game
        // mainCamera = Camera.main;
        // transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, -10);
        // Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(Vector3.zero) * 100;
        // Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(mainCamera.rect.width, mainCamera.rect.height)) * 100;
        // Vector3 screenSize = topRight - bottomLeft;
        // float screenRatio = screenSize.x / screenSize.y;
        // float desiredRatio = transform.localScale.x / transform.localScale.y;

        // if (screenRatio > desiredRatio)
        // {
        //     float height = screenSize.y;
        //     transform.localScale = new Vector3(height * desiredRatio, height);
        // } else
        // {
        //     float width = screenSize.x;
        //     transform.localScale = new Vector3(width, width / desiredRatio);
        // }


        // float ratio = (float)Screen.width / Screen.height;
        // GetComponent<Camera>().orthographicSize = (0.6f / ratio) * 5;
        // Debug.Log(ratio);
        // Debug.Log((0.6f / ratio) * 5);
    }

    // private void Update()
    // {
        // float scroll = Input.GetAxis("Mouse ScrollWheel");
        // zoom -= scroll * zoomMultiplier;
        // zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        // cam.orthographicSize = Mathf.SmoothDamp(cam.orthographicSize, zoom, ref velocity, smoothTime);
    // }


    // private void Awake()
    // {
    //     mainCamera = Camera.main;
    // }

    // public void OnDrag(InputAction.CallbackContext ctx)
    // {
    //     Debug.Log("??");
    //     if(ctx.started) _origin = GetMousePosition;
    //     _isDragging = ctx.started || ctx.performed;
    // }

    // private void LateUpdate()
    // {
    //     if(!_isDragging) return;

    //     _difference = GetMousePosition - transform.position;
    //     transform.position = _origin - _difference;
    // }

    // private Vector3 GetMousePosition => mainCamera.ScreenToWorldPoint((Vector3)Mouse.current.position.ReadValue());
}
