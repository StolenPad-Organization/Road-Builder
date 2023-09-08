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
            playerController.movementController.SetAnimation();
            playerController.transform.localEulerAngles = Vector3.zero;
            playerController.paintMachine.toolAngleController.SetDistanceTarget(distanceTarget);
            playerController.movementController.insideAngleTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerController.paintMachine != null)
        {
            playerController.movementController.canRotate = true;
            playerController.movementController.SetAnimation();
            playerController.paintMachine.toolAngleController.ResetTool();
            playerController.movementController.insideAngleTrigger = false;
        }
    }

    private void OnDisable()
    {
        if (playerController != null)
        {
            playerController.movementController.canRotate = true;
            playerController.movementController.SetAnimation();
            playerController.movementController.insideAngleTrigger = false;
        }
    }
}
