using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum UpgradeType
{
    ToolUpgrade,
    LoadUpgrade,
    ToolLengthUpgrade,
    ToolWidthUpgrade
}
public class Upgrade : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private UpgradeType upgradeType;
    private UpgradeData upgradeData;
    public int loops;
    public int level;
    [SerializeField] private TextMeshProUGUI levelText;
    public float value;
    public float addValue;
    [SerializeField] private Image[] steps;
    public int stepIndex;
    [SerializeField] private Sprite[] lockedStepSprites;
    [SerializeField] private Sprite[] unlockedStepSprites;
    public UpgradeManager upgradeManager;
    [SerializeField] private Button buyButton;

    [Header("Upgrade & Cost")]
    [SerializeField] private int cost;
    [SerializeField] private int costAddPrecentage;
    [SerializeField] private TextMeshProUGUI costText;

    void Start()
    {
        //LoadUpgrade();
    }

    void Update()
    {
        
    }

    public void LoadUpgrade()
    {
        upgradeData = Database.Instance.GetUpgradeData(upgradeType);
        if (upgradeData != null)
        {
            loops = upgradeData.Loops;
            level = upgradeData.Level;
            cost = upgradeData.Cost;
            value = upgradeData.Value;
            stepIndex = upgradeData.StepIndex;
        }

        costText.text = UIManager.instance.ReturnNumberText(cost);
        levelText.text = "Lvl " + level;
        for (int i = 0; i < stepIndex+1; i++)
        {
            if (i == 0)
                steps[i].sprite = unlockedStepSprites[0];
            else if (i == steps.Length - 1)
                steps[i].sprite = unlockedStepSprites[2];
            else
                steps[i].sprite = unlockedStepSprites[1];
        }

        switch (upgradeType)
        {
            case UpgradeType.ToolUpgrade:
                upgradeManager.LoadShovel();
                break;
            case UpgradeType.LoadUpgrade:
                upgradeManager.OnLoadUpgrade();
                break;
            case UpgradeType.ToolLengthUpgrade:
                upgradeManager.OnToolLengthUpgrade();
                break;
            case UpgradeType.ToolWidthUpgrade:
                upgradeManager.OnToolWidthUpgrade();
                break;
            default:
                break;
        }
    }

    public void Buy()
    {
        if (PlayerController.instance.canDoStrictedHaptic)
        {
            EventManager.invokeHaptic.Invoke(vibrationTypes.LightImpact);
            PlayerController.instance.canDoStrictedHaptic = false;
        }
        UIManager.instance.UpdateMoney(-cost);
        cost += (costAddPrecentage * cost)/100;
        costText.text = UIManager.instance.ReturnNumberText(cost);
        stepIndex++;
        if(stepIndex < steps.Length)
        {
            if(stepIndex == 0)
                steps[stepIndex].sprite = unlockedStepSprites[0];
            else if(stepIndex == steps.Length -1)
                steps[stepIndex].sprite = unlockedStepSprites[2];
            else
                steps[stepIndex].sprite = unlockedStepSprites[1];
        }
        else
        {
            loops++;
            stepIndex = 0;
            for (int i = 0; i < steps.Length; i++)
            {
                if (i == 0)
                    steps[i].sprite = lockedStepSprites[0];
                else if (i == steps.Length - 1)
                    steps[i].sprite = lockedStepSprites[2];
                else
                    steps[i].sprite = lockedStepSprites[1];
            }
            steps[0].sprite = unlockedStepSprites[0];
        }
        level++;
        levelText.text = "Lvl " + level;
        value += addValue;

        upgradeManager.CheckButtons();
    }

    public void CheckButton()
    {
        if(UIManager.instance.money < cost)
        {
            buyButton.interactable = false;
        }
        else
        {
            buyButton.interactable = true;
        }
    }

    public void SaveUpgradeData()
    {
        if (upgradeData != null)
        {
            upgradeData.Loops = loops;
            upgradeData.Level = level;
            upgradeData.Cost = cost;
            upgradeData.Value = value;
            upgradeData.StepIndex = stepIndex;

            Database.Instance.SetUpgradeData(upgradeType, upgradeData);
        }
    }
}
