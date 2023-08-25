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

    [Header("Upgrade Points UI Effect")]
    [SerializeField] private Transform upgradePointTarget;
    [SerializeField] private GameObject upgradePointPrefab;

    [Header("State UI")]
    [SerializeField] private TextMeshProUGUI StepText;
    [SerializeField] private string peeling;
    [SerializeField] private string building;
    [SerializeField] private string painting;

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
        EventManager.AddUpgradePoint += AddUpgradePoint;
    }

    private void OnDisable()
    {
        EventManager.AddMoney -= AddMoney;
        EventManager.AddUpgradePoint -= AddUpgradePoint;
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
                UpdateMoney(amount / x);
            });
            yield return new WaitForSeconds(0.12f);
        }
        UpdateMoney(amount % x);
    }

    private void AddUpgradePoint(int amount, Transform target)
    {
        StartCoroutine(sendUpgradePoint(amount, target));
    }

    IEnumerator sendUpgradePoint(int amount, Transform target)
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
            GameObject upgradePointClone = Instantiate(upgradePointPrefab, fpos, Quaternion.identity, canvasSpace);
            upgradePointClone.transform.DOScale(0.85f, VFXDuration);
            upgradePointClone.transform.DOMove(upgradePointTarget.position, VFXDuration).OnComplete(() =>
            {
                Destroy(upgradePointClone);
                UpdateUpgradePoints(amount / x);
            });
            yield return new WaitForSeconds(0.12f);
        }
        UpdateUpgradePoints(amount % x);
    }

    private Vector3 ReturnWorldToCanvasPosition(Vector3 pos)
    {
        pos = Camera.main.WorldToScreenPoint(pos);
        pos.z = 0;
        return pos;
    }

    public void UpdateStepText()
    {
        switch (GameManager.instance.currentZone.zoneState)
        {
            case ZoneState.PeelingStage:
                StepText.text = peeling;
                break;
            case ZoneState.BuildingStage:
                StepText.text = building;
                break;
            case ZoneState.PaintingStage:
                StepText.text = painting;
                break;
            default:
                break;
        }
    }
}
