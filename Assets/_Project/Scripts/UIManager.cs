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
        moneyText.text = money.ToString();
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
}
