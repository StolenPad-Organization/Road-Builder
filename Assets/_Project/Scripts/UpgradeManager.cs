using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    private PlayerController playerController;
    [SerializeField] private Upgrade shovelUpgrade;

    private void Start()
    {
        playerController = PlayerController.instance;
    }

    public void OnShovelUpgrade()
    {
        if(playerController.scrapeToolIndex == shovelUpgrade.loops)
        {
            playerController.scrapeTool.UpdateShovelScale(shovelUpgrade.stepIndex);
            playerController.scrapeTool.power = shovelUpgrade.value;
        }
        else
        {
            playerController.scrapeToolIndex++;
            playerController.ChangeScrapeTool(playerController.scrapeToolIndex);
        }
    }
}
