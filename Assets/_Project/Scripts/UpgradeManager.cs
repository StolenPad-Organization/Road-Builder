using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    public ScrapeToolType scrapeToolType;
    public PlayerController playerController;
    public Upgrade shovelUpgrade;
    public Upgrade loadUpgrade;
    public Upgrade lengthUpgrade;
    public Upgrade widthUpgrade;
    public Upgrade loadUpgrade2;
    [SerializeField] private GameObject normalUpgradesHolder;
    [SerializeField] private GameObject angleUpgradesHolder;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        //playerController = GameManager.instance.player;
    }

    private void OnEnable()
    {
        CheckButtons();
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
        scrapeToolType = playerController.scrapeTool.scrapeToolType;

        if (scrapeToolType == ScrapeToolType.Normal)
        {
            playerController.scrapeTool.UpdateShovelScale(shovelUpgrade.stepIndex);
            playerController.scrapeTool.power = shovelUpgrade.value;
        }
        else
        {
            normalUpgradesHolder.SetActive(false);
            angleUpgradesHolder.SetActive(true);
            loadUpgrade = loadUpgrade2;
        }
    }

    public void OnToolLengthUpgrade()
    {
        if (scrapeToolType == ScrapeToolType.Normal) return;
        playerController.scrapeTool.toolAngleController.CalculateLength(lengthUpgrade.level-1);
    }

    public void OnToolWidthUpgrade()
    {
        if (scrapeToolType == ScrapeToolType.Normal) return;
        playerController.scrapeTool.toolAngleController.CalculateWidth(widthUpgrade.level-1);
    }

    public void CheckButtons()
    {
        shovelUpgrade.CheckButton();
        loadUpgrade.CheckButton();
        lengthUpgrade.CheckButton();
        widthUpgrade.CheckButton();
    }
}
