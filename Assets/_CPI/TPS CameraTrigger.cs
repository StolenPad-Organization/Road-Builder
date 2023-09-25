using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSCameraTrigger : MonoBehaviour
{
    [SerializeField] private GameObject tpsCameraObject;
    private void OnTriggerEnter(Collider other)
    {
        tpsCameraObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        tpsCameraObject.SetActive(false);
    }
}
