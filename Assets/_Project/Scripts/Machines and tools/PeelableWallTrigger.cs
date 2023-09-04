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
            playerController.transform.localEulerAngles = Vector3.zero;
            playerController.scrapeTool.toolAngleController.SetDistanceTarget(distanceTarget);
            playerController.scrapeTool.toolAngleController.enabled = true;

            playerController.scrapeTool.WaterEffectFillController.Fill();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerController.scrapeTool.gameObject.activeInHierarchy)
        {
            playerController.movementController.canRotate = true;
            playerController.scrapeTool.toolAngleController.ResetTool();
            playerController.scrapeTool.toolAngleController.enabled = false;

            playerController.scrapeTool.WaterEffectFillController.Empty();
        }
    }
}
