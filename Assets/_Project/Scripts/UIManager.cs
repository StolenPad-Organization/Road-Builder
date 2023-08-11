using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] private TextMeshProUGUI moneyText;
    public int money;
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] private BuildMachineUpgradeMenu buildMachineUpgradeMenu;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private ProgressUIManager progressBar;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateMoney(Database.Instance.GetPlayerData().Money);
        levelText.text = "Level " + Database.Instance.GetLevelData().LevelTextValue;
    }

    void Update()
    {
        
    }

    public void UpdateMoney(int amount)
    {
        money += amount;
        moneyText.text = ReturnNumberText(money);
    }

    public void ShowUpgradeMenu()
    {
        upgradeManager.gameObject.SetActive(true);
    }

    public void HideUpgradeMenu()
    {
        upgradeManager.gameObject.SetActive(false);
    }

    public void UpdateProgressBar(float value)
    {
        progressBar.UpdateProgressBar(value);
    }

    public void ChangeProgressBarIcon(int index)
    {
        progressBar.ChangeIcon(index);
    }

    public string ReturnNumberText(int num)
    {
        int a, b;
        a = num / 1000;
        b = num % 1000;
        if(a == 0)
        {
            return b.ToString();
        }
        else
        {
            if(b/100 != 0)
                b /= 100;
            return a + "." + b +" k";
        }
    }

    public void ShowMachineUpgradeMenu()
    {
        buildMachineUpgradeMenu.gameObject.SetActive(true);
    }

    public void HideMachineUpgradeMenu()
    {
        buildMachineUpgradeMenu.gameObject.SetActive(false);
    }
}
