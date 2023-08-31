using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintableWallTrigger : MonoBehaviour
{
    private PlayerController playerController;
    [SerializeField] private Transform distanceTarget;
    void Start()
    {
        playerController = PlayerController.instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(playerController.paintMachine != null)
        {
            playerController.movementController.canRotate = false;
            playerController.transform.localEulerAngles = Vector3.zero;
            playerController.paintMachine.paintToolController.SetDistanceTarget(distanceTarget);
            playerController.paintMachine.paintToolController.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerController.paintMachine != null)
        {
            playerController.movementController.canRotate = true;
            playerController.paintMachine.paintToolController.ResetTool();
            playerController.paintMachine.paintToolController.enabled = false;
        }
    }
}
