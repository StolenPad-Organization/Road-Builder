using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceWalkTrigger : MonoBehaviour
{
    private bool isPlayerInside;

    private void OnTriggerEnter(Collider other)
    {
        isPlayerInside = true;
    }
    private void OnTriggerExit(Collider other)
    {
        isPlayerInside = false;
    }

    private void OnDisable()
    {
        if(isPlayerInside)
            PlayerController.instance.RemoveWalkTrigger();
    }
}
