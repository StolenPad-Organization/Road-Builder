using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockWarning : MonoBehaviour
{
    private bool colliding;
    private void OnTriggerEnter(Collider other)
    {
        colliding = true;
        GameManager.instance.player.ShowWarning(true);
        GameManager.instance.player.scrapeTool.ShowTool(true);
        GameManager.instance.player.OnBlockWallDetection(true);
        GameManager.instance.player.scrapeTool.ChangeToolMaterial(true);
    }

    private void OnTriggerExit(Collider other)
    {
        colliding = false;
        GameManager.instance.player.ShowWarning(false);
        GameManager.instance.player.OnBlockWallDetection(false);
        GameManager.instance.player.scrapeTool.ChangeToolMaterial(false);
    }

    private void OnDisable()
    {
        if (colliding)
        {
            colliding = false;
            GameManager.instance.player.ShowWarning(false);
            GameManager.instance.player.OnBlockWallDetection(false);
            if(GameManager.instance.player.scrapeTool != null)
                GameManager.instance.player.scrapeTool.ChangeToolMaterial(false);
        }
    }
}
