using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelSelectController : MonoBehaviour
{
    [SerializeField] private LevelUIController[] levels;
    private int level;
    [SerializeField] private TextMeshProUGUI moneyText;
    public int money;
    [SerializeField] private TextMeshProUGUI upgradePointsText;
    public int upgradePoints;
    [SerializeField] private PlayerUIController player;
    [SerializeField] private GameObject playButton;
    [SerializeField] private int maxLevels;

    IEnumerator Start()
    {
        level = Database.Instance.GetLevelData().LevelTextValue;
        UpdateMoney(Database.Instance.GetPlayerData().Money);
        UpdateUpgradePoints(Database.Instance.GetPlayerData().UpgradePoints);
        if (level <= maxLevels)
        {
            if (level - 1 >= 1)
                player.transform.position = levels[level - 2].transform.position;
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(player.FollowPath(levels[level - 1].levelPath));
        }
        else
        {
            player.transform.position = levels[maxLevels-1].transform.position;
        }
    }

    public void ShowPlayButton()
    {
        playButton.SetActive(true);
    }

    public void UpdateMoney(int amount)
    {
        money += amount;
        moneyText.text = ReturnNumberText(money);
    }

    public void UpdateUpgradePoints(int amount)
    {
        upgradePoints += amount;
        upgradePointsText.text = ReturnNumberText(upgradePoints);
    }

    public string ReturnNumberText(int num)
    {
        int a, b;
        a = num / 1000;
        b = num % 1000;
        if (a == 0)
        {
            return b.ToString();
        }
        else
        {
            if (b / 100 != 0)
                b /= 100;
            return a + "." + b + " k";
        }
    }
}
