using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup rawardGroup;
    [SerializeField] private Transform rawardPanel;

    private int index = -1;
    private WinLoseUI winLoseUI;

    [SerializeField] private TextMeshProUGUI rewardNameText;
    [SerializeField] private Button claimButton;
    [Header("Money Reward")]
    [SerializeField] private GameObject moneyreward;
    [SerializeField] private TextMeshProUGUI moneyrewardText;
    [Header("UpgradePoints Reward")]
    [SerializeField] private GameObject upgradePointsreward;
    [SerializeField] private TextMeshProUGUI upgradePointsrewardText;
    [Header("Skin/Mount Reward")]
    [SerializeField] private GameObject skinreward;
    [SerializeField] private Image skinImage;
    [Header("Rewards")]
    [SerializeField] private RewardOption[] rewardOptions;

    void Start()
    {

    }

    private void OnEnable()
    {
        claimButton.onClick.AddListener(ShowNextReward);
    }
    private void OnDisable()
    {
        claimButton.onClick.RemoveAllListeners();
    }

    public void ShowPanel(WinLoseUI winLose)
    {
        winLoseUI = winLose;
        rawardGroup.alpha = 0f;
        rawardGroup.gameObject.SetActive(true);
        rawardPanel.localScale = Vector3.zero;
        rawardGroup.DOFade(1f, 0.2f);
        rawardPanel.gameObject.SetActive(true);
        ShowNextReward();
    }

    public void ShowNextReward()
    {
        index++;
        Debug.Log("Button Pressed , Index :" + index);
        rawardPanel.DOScale(Vector3.zero, 0.3f).OnComplete(() =>
        {
            if (index >= rewardOptions.Length)
            {
                winLoseUI.ShowWinPanel();
                return;
            }
            showReward();
            rawardPanel.DOScale(Vector3.one * 1.5f, 0.3f);
        });
    }

    private void showReward()
    {
        moneyreward.SetActive(false);
        upgradePointsreward.SetActive(false);
        skinreward.SetActive(false);

        switch (rewardOptions[index].rewardType)
        {
            case RewardType.money:
                moneyreward.SetActive(true);
                moneyrewardText.text = UIManager.instance.ReturnNumberText(rewardOptions[index].value);
                break;
            case RewardType.upgradePoint:
                upgradePointsreward.SetActive(true);
                upgradePointsrewardText.text = UIManager.instance.ReturnNumberText(rewardOptions[index].value);
                break;
            case RewardType.Skin:
                skinreward.SetActive(true);
                skinImage.sprite = rewardOptions[index].rewardIcon;
                break;
            case RewardType.Mount:
                skinreward.SetActive(true);
                skinImage.sprite = rewardOptions[index].rewardIcon;
                break;
            default:
                break;
        }
        rewardNameText.text = rewardOptions[index].rewardName;
    }
}
