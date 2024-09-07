using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraMovement : MonoBehaviour
{
    public float speed = 30f;
    public float rotationSpeed = 30f;
    public float zoomSpeed = 100f;
    public float zoomMin = 30f;
    public float zoomMax = 90f;
    public Camera targetCamera;

    private void Start()
    {
        if(targetCamera == null)
        {
            targetCamera = GetComponent<Camera>();
        }
        targetCamera.fieldOfView = (zoomMax + zoomMin) / 2;
    }

    void Update()
    {
        var translation = Input.GetAxis("Vertical") * speed;
        var horizontalMovement = Input.GetAxis("Horizontal") * speed;
        var rotation = 0f;

        if (Input.GetKey(KeyCode.Q))
        {
            rotation = -rotationSpeed;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            rotation = rotationSpeed;
        }

        translation *= Time.deltaTime;
        horizontalMovement *= Time.deltaTime;
        rotation *= Time.deltaTime;
        var zoom = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;

        transform.Translate(horizontalMovement, 0, translation);
        transform.Rotate(0, rotation, 0);
        targetCamera.fieldOfView = Mathf.Clamp(targetCamera.fieldOfView - zoom, zoomMin, zoomMax);
    }
}
