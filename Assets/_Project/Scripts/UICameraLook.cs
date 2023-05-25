using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICameraLook : MonoBehaviour
{
    Camera mainCamera;
    Vector3 rotation;
    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        transform.LookAt(mainCamera.transform);
        rotation = transform.eulerAngles;
        rotation.y += 180;
        rotation.x *= -1;
        transform.eulerAngles = rotation;
    }
}
