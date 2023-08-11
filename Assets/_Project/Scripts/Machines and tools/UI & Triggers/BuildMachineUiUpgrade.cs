using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum MachineUpgradeType
{
    AsphaltMachineUpgrade,
    RollingMachineUpgrade,
    none
}

public class BuildMachineUiUpgrade : MonoBehaviour
{
    [Header("Properties")]
    [SerializeField] private MachineUpgradeType upgradeType;
    private MachineUpgradeData upgradeData;
    public int loops;
    public int level;
    [SerializeField] private TextMeshProUGUI levelText;
    public float value;
    public float addValue;
    [SerializeField] private Image[] steps;
    public int stepIndex;
    [SerializeField] private Sprite[] lockedStepSprites;
    [SerializeField] private Sprite[] unlockedStepSprites;
    public BuildMachineUpgradeMenu upgradeManager;
    [SerializeField] private Button buyButton;
    [SerializeField] private GameObject maxButton;

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

    public void LoadUpgrade(MachineUpgradeType _upgradeType)
    {
        upgradeType = _upgradeType; 
        upgradeData = Database.Instance.GetMachineUpgradeData(upgradeType);
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
        for (int i = 0; i < stepIndex + 1; i++)
        {
            if (i == 0)
                steps[i].sprite = unlockedStepSprites[0];
            else if (i == steps.Length - 1)
                steps[i].sprite = unlockedStepSprites[2];
            else
                steps[i].sprite = unlockedStepSprites[1];
        }

        upgradeManager.UpgradeMachine();
        CheckButton();
    }

    public void Buy()
    {
        UIManager.instance.UpdateMoney(-cost);
        cost += (costAddPrecentage * cost) / 100;
        costText.text = UIManager.instance.ReturnNumberText(cost);
        stepIndex++;
        if (stepIndex < steps.Length)
        {
            if (stepIndex == 0)
                steps[stepIndex].sprite = unlockedStepSprites[0];
            else if (stepIndex == steps.Length - 1)
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
        if (UIManager.instance.money < cost)
        {
            buyButton.interactable = false;
        }
        else
        {
            buyButton.interactable = true;
        }

        if (level >= 9)
        {
            buyButton.interactable = false;
            maxButton.SetActive(true);
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

            Database.Instance.SetMachineUpgradeData(upgradeType, upgradeData);
        }
    }
}
