using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    public PlayerController playerController;
    public Upgrade shovelUpgrade;
    public Upgrade loadUpgrade;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        //playerController = PlayerController.instance;
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
            playerController.scrapeToolIndex = shovelUpgrade.loops;
            playerController.ChangeScrapeTool(playerController.scrapeToolIndex);
        }
    }

    public void OnLoadUpgrade()
    {
        playerController.UpgradeCollectablesLimit(loadUpgrade.value);
    }

    public void LoadShovel()
    {
        playerController.scrapeToolIndex = shovelUpgrade.loops;
        playerController.ChangeScrapeTool(playerController.scrapeToolIndex);
        playerController.scrapeTool.UpdateShovelScale(shovelUpgrade.stepIndex);
        playerController.scrapeTool.power = shovelUpgrade.value;
    }
}
