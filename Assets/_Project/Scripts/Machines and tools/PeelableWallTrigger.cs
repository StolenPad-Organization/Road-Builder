using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeelableWallTrigger : MonoBehaviour
{
    private PlayerController playerController;
    [SerializeField] private Transform distanceTarget;
    void Start()
    {
        playerController = PlayerController.instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerController.scrapeTool.gameObject.activeInHierarchy)
        {
            playerController.movementController.canRotate = false;
            playerController.scrapeTool.ShowTool(true);
            playerController.movementController.SetAnimation();
            playerController.transform.localEulerAngles = Vector3.zero;
            playerController.scrapeTool.toolAngleController.SetDistanceTarget(distanceTarget);

            playerController.scrapeTool.WaterEffectFillController.Fill();
            playerController.movementController.insideAngleTrigger = true;

            playerController.ActivateWaterPodsEffect(true);
            playerController.SetWalkType(0);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerController.scrapeTool.gameObject.activeInHierarchy)
        {
            playerController.movementController.canRotate = true;
            playerController.scrapeTool.ShowTool(false);
            playerController.movementController.SetAnimation();
            playerController.scrapeTool.toolAngleController.ResetTool();

            playerController.scrapeTool.WaterEffectFillController.Empty();
            playerController.movementController.insideAngleTrigger = false;

            playerController.ActivateWaterPodsEffect(false);
            playerController.SetWalkType();
        }
    }

    private void OnDisable()
    {
        if (playerController != null)
        {
            playerController.movementController.canRotate = true;
            playerController.movementController.SetAnimation();
            playerController.movementController.insideAngleTrigger = false;
            playerController.ActivateWaterPodsEffect(false);
            playerController.SetWalkType();
        }
    }
}
