using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockWarning : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PlayerController.instance.ShowWarning(true);
        PlayerController.instance.scrapeTool.ShowTool(true);
        PlayerController.instance.OnBlockWallDetection(true);
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController.instance.ShowWarning(false);
        PlayerController.instance.OnBlockWallDetection(false);
    }
}
