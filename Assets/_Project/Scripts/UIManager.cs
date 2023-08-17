using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] private TextMeshProUGUI moneyText;
    public int money;
    [SerializeField] private TextMeshProUGUI upgradePointsText;
    public int upgradePoints;
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] private BuildMachineUpgradeMenu buildMachineUpgradeMenu;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private ProgressUIManager progressBar;

    [Header("Money UI Effect")]
    [SerializeField] private Transform moneyTarget;
    [SerializeField] private Transform canvasSpace;
    [SerializeField] private GameObject moneyPrefab;
    [SerializeField] private float VFXDuration;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateMoney(Database.Instance.GetPlayerData().Money);
        UpdateUpgradePoints(Database.Instance.GetPlayerData().UpgradePoints);
        levelText.text = "Level " + Database.Instance.GetLevelData().LevelTextValue;
    }

    private void OnEnable()
    {
        EventManager.AddMoney += AddMoney;
    }

    private void OnDisable()
    {
        EventManager.AddMoney -= AddMoney;
    }

    void Update()
    {
        
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

    private void AddMoney(int amount, Transform target)
    {
        StartCoroutine(sendMoney(amount, target));
    }

    IEnumerator sendMoney(int amount, Transform target)
    {
        Vector3 pos = target.position;
        Vector3 fpos = ReturnWorldToCanvasPosition(pos);

        int x = Random.Range(2, 4);
        for (int i = 0; i < x; i++)
        {
            if (i > 0)
            {
                fpos = ReturnWorldToCanvasPosition(pos + Random.insideUnitSphere);
            }
            GameObject moneyClone = Instantiate(moneyPrefab, fpos, Quaternion.identity, canvasSpace);
            moneyClone.transform.DOScale(0.85f, VFXDuration);
            moneyClone.transform.DOMove(moneyTarget.position, VFXDuration).OnComplete(() =>
            {
                Destroy(moneyClone);
                //money += amount / x;
                UpdateMoney(amount / x);
            });
            yield return new WaitForSeconds(0.12f);
        }
        //money += amount % x;
        UpdateMoney(amount % x);
    }

    private Vector3 ReturnWorldToCanvasPosition(Vector3 pos)
    {
        pos = Camera.main.WorldToScreenPoint(pos);
        pos.z = 0;
        return pos;
    }
}
