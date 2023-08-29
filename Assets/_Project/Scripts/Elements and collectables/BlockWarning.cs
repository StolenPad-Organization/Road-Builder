using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockWarning : MonoBehaviour
{
    private bool colliding;
    private void OnTriggerEnter(Collider other)
    {
        colliding = true;
        PlayerController.instance.ShowWarning(true);
        PlayerController.instance.scrapeTool.ShowTool(true);
        PlayerController.instance.OnBlockWallDetection(true);
        PlayerController.instance.scrapeTool.ChangeToolMaterial(true);
    }

    private void OnTriggerExit(Collider other)
    {
        colliding = false;
        PlayerController.instance.ShowWarning(false);
        PlayerController.instance.OnBlockWallDetection(false);
        PlayerController.instance.scrapeTool.ChangeToolMaterial(false);
    }

    private void OnDisable()
    {
        if (colliding)
        {
            colliding = false;
            PlayerController.instance.ShowWarning(false);
            PlayerController.instance.OnBlockWallDetection(false);
            if(PlayerController.instance.scrapeTool != null)
                PlayerController.instance.scrapeTool.ChangeToolMaterial(false);
        }
    }
}
