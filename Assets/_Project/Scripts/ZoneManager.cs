using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ZoneState
{
    Locked,
    PeelingStage,
    BuildingStage,
    PaintingStage,
    Complete
}

public class ZoneManager : MonoBehaviour
{
    //public static ZoneManager instance;

    [SerializeField] private PlayerController player;
    public ZoneState zoneState;
    [SerializeField] private GameObject removableBlock;
    [SerializeField] private GameObject upgrades;
    public BuildMachine buildMachine;
    [SerializeField] private GameObject asphaltBlock;
    public BuildAmmo asphaltAmmo;
    public PaintMachine paintingMachine;
    [SerializeField] private GameObject paintBlock;
    public PaintAmmo paintAmmo;
    [SerializeField] private WheelBarrow wheelBarrow;
    public SellManager sellManager;
    [SerializeField] private UpgradeManager upgradeManager;
    public BuildMachineUpgradeMenu buildMachineUpgradeMenu;
    [SerializeField] private Transform collectableParent;
    public Transform machinesPosition;
    [SerializeField] private GameObject zoneCollider;
    [SerializeField] private GameObject machineUpgradeTrigger;
    [SerializeField] private bool hideMachine;
    public float groundYRef = -1;

    [Header("Blocks Progress")]
    [SerializeField] private int maxBlocks;
    [SerializeField] private int currentBlocks;
    public PeelableManager peelableManager;

    [Header("Asphalt Progress")]
    [SerializeField] private int maxAsphalt;
    [SerializeField] private int currentAsphalt;
    public BuildableManager buildableManager;

    [Header("Paint Progress")]
    [SerializeField] private int maxPaint;
    [SerializeField] private int currentPaint;
    public PaintableManager paintableManager;

    [Header("Data Progress")]
    private LevelData levelData;
    private PlayerData playerData;
    private ZoneData zoneData;

    //private void Awake()
    //{
    //    instance = this;
    //}

    private void Start()
    {
        maxBlocks = peelableManager.peelableParts.Count;
        maxAsphalt = buildableManager.buildableParts.Count;
        maxPaint = paintableManager.paintableParts.Count;
    }

    public void InitZone(ZoneData _zoneData)
    {
        zoneData = _zoneData;
        player = PlayerController.instance;
        levelData = Database.Instance.GetLevelData();
        playerData = Database.Instance.GetPlayerData();
        //yield return new WaitForEndOfFrame();
        LoadZone();
        upgradeManager.playerController = player;
        upgradeManager.shovelUpgrade.upgradeManager = upgradeManager;
        upgradeManager.shovelUpgrade.LoadUpgrade();
        upgradeManager.loadUpgrade.upgradeManager = upgradeManager;
        upgradeManager.loadUpgrade.LoadUpgrade();
        upgradeManager.CheckButtons();
    }

    void Update()
    {
        
    }

    public void OnBlockRemove()
    {
        currentBlocks++;
        CheckBlocks();
    }
    private void CheckBlocks()
    {
        UIManager.instance.UpdateProgressBar((float)currentBlocks / (float)maxBlocks);
        if (currentBlocks == maxBlocks)
            ShowAsphaltMachine();
    }
    private void ShowAsphaltMachine()
    {
        EventManager.invokeHaptic.Invoke(vibrationTypes.Success);
        zoneState = ZoneState.BuildingStage;
        buildMachine.gameObject.SetActive(true);
        player.arrowController.PointToObject(buildMachine.gameObject);

        buildMachine.OnSpawn();
    }
    public void StartAsphaltStage()
    {
        UIManager.instance.ChangeProgressBarIcon(1);
        UIManager.instance.UpdateProgressBar(0);
        upgrades.SetActive(false);
        if (buildMachine.hasUpgrade)
            machineUpgradeTrigger.SetActive(true);
        removableBlock.SetActive(false);
        //PlayerController.instance.RemovePeelingAndCollectingTools();
        asphaltBlock.SetActive(true);
        asphaltAmmo.gameObject.SetActive(true);
        player.arrowController.PointToObject(asphaltAmmo.gameObject);
    }

    public void OnRoadBuild()
    {
        currentAsphalt++;
        CheckAsphalt();
    }
    private void CheckAsphalt()
    {
        UIManager.instance.UpdateProgressBar((float)currentAsphalt / (float)maxAsphalt);
        if (currentAsphalt == maxAsphalt)
            ShowPaintMachine();
    }
    private void ShowPaintMachine()
    {
        EventManager.invokeHaptic.Invoke(vibrationTypes.Success);
        zoneState = ZoneState.PaintingStage;
        PlayerController.instance.GetOffAsphaltMachine();
        paintingMachine.gameObject.SetActive(true);
        player.arrowController.PointToObject(paintingMachine.gameObject);

        paintingMachine.OnSpawn();
    }
    public void StartPaintStage()
    {
        UIManager.instance.ChangeProgressBarIcon(2);
        //PlayerController.instance.GetOffAsphaltMachine();
        machineUpgradeTrigger.SetActive(false);
        asphaltAmmo.gameObject.SetActive(false);
        paintBlock.SetActive(true);
        paintAmmo.gameObject.SetActive(true);
        player.arrowController.PointToObject(paintAmmo.gameObject);
    }

    public void OnRoadPaint()
    {
        currentPaint++;
        CheckPaint();
    }
    private void CheckPaint()
    {
        UIManager.instance.UpdateProgressBar((float)currentPaint / (float)maxPaint);
        if (currentPaint == maxPaint)
            StartCoroutine(CompleteZone());
    }
    private IEnumerator CompleteZone()
    {
        EventManager.invokeHaptic.Invoke(vibrationTypes.Success);
        yield return new WaitForSeconds(1.0f);
        paintingMachine.transform.SetParent(null);
        paintingMachine.gameObject.SetActive(false);
        paintAmmo.gameObject.SetActive(false);

        zoneState = ZoneState.Complete;

        if (hideMachine)
        {
            paintingMachine.gameObject.SetActive(false);
            paintAmmo.gameObject.SetActive(false);
            buildMachine.gameObject.SetActive(false);
            asphaltAmmo.gameObject.SetActive(false);
            wheelBarrow.gameObject.SetActive(false);
            upgrades.SetActive(false);
        }

        GameManager.instance.SaveLevel();
        // Unlock next Zone
        GameManager.instance.UnlockNextZone();
    }

    public void UnlockZone()
    {
        zoneData.ZoneState = ZoneState.PeelingStage;
        LoadZone();
        if (hideMachine)
        {
            paintingMachine.gameObject.SetActive(true);
            paintAmmo.gameObject.SetActive(true);
            buildMachine.gameObject.SetActive(true);
            asphaltAmmo.gameObject.SetActive(true);
            wheelBarrow.gameObject.SetActive(true);
            upgrades.SetActive(true);
        }
        if (zoneCollider != null)
        {
            PlayerController.instance.arrowController.PointToObject(zoneCollider);
        }
    }

    private void LoadZone()
    {
        if (GameManager.instance.currentZone == this && buildMachine.machineUpgradeType != MachineUpgradeType.none)
        {
            buildMachineUpgradeMenu.buildMachineUpgrade.LoadUpgrade(buildMachine.machineUpgradeType);
            buildMachineUpgradeMenu.UpgradeMachine();
            buildMachineUpgradeMenu.CheckButtons();
        }
        // Load player position and rotation
        player.transform.position = playerData.PlayerPosition;
        player.transform.eulerAngles = playerData.PlayerRotation;

        //load level State
        zoneState = zoneData.ZoneState;

        switch (zoneState)
        {
            case ZoneState.PeelingStage:
                if (zoneCollider != null)
                    zoneCollider.gameObject.SetActive(false);
                // load collectables and peelables
                upgrades.SetActive(true);
                CollectablesPooler.Instance.collectableParent = collectableParent;
                peelableManager.LoadPeelables(zoneData.PeelableDatas);
                if (playerData.HasWheelBarrow)
                {
                    wheelBarrow.transform.position = playerData.WheelBarrowPosition;
                    wheelBarrow.transform.eulerAngles = playerData.WheelBarrowRotation;
                    wheelBarrow.ActivateWheelBarrow();
                    wheelBarrow.LoadCollectables(playerData.wheelBarrowCollectables);
                }
                player.LoadCollectables(playerData.playerCollectables);
                sellManager.LoadMoeny(zoneData.MoneyDatas);
                UIManager.instance.ChangeProgressBarIcon(0);
                break;
            case ZoneState.BuildingStage:
                if (zoneCollider != null)
                    zoneCollider.gameObject.SetActive(false);
                // load buildable and building machine
                player.RemovePeelingAndCollectingTools();
                buildableManager.LoadBuildables(zoneData.BuildableDatas, true);
                upgrades.SetActive(false);
                removableBlock.SetActive(false);
                buildMachine.gameObject.SetActive(true);
                asphaltBlock.SetActive(true);
                if (buildMachine.hasUpgrade)
                    machineUpgradeTrigger.SetActive(true);
                LoadUpgradePoints(zoneData.UpgradePointDatas);
                UIManager.instance.ChangeProgressBarIcon(1);

                buildMachine.OnSpawn();
                break;
            case ZoneState.PaintingStage:
                if (zoneCollider != null)
                    zoneCollider.gameObject.SetActive(false);
                // load paintable and painting machine and buildables
                player.RemovePeelingAndCollectingTools();
                buildableManager.LoadBuildables(zoneData.BuildableDatas, false);
                paintableManager.LoadPaintables(zoneData.PaintableDatas, true);
                upgrades.SetActive(false);
                removableBlock.SetActive(false);
                asphaltBlock.SetActive(true);
                paintingMachine.gameObject.SetActive(true);
                paintBlock.gameObject.SetActive(true);

                UIManager.instance.ChangeProgressBarIcon(2);

                paintingMachine.OnSpawn();
                break;
            case ZoneState.Complete:
                if (zoneCollider != null)
                    zoneCollider.gameObject.SetActive(false);
                upgrades.SetActive(false);
                removableBlock.SetActive(false);
                asphaltBlock.SetActive(true);
                paintBlock.gameObject.SetActive(true);
                buildableManager.LoadBuildables(zoneData.BuildableDatas, false);
                paintableManager.LoadPaintables(zoneData.PaintableDatas, false);
                if (hideMachine)
                {
                    paintingMachine.gameObject.SetActive(false);
                    paintAmmo.gameObject.SetActive(false);
                    buildMachine.gameObject.SetActive(false);
                    asphaltAmmo.gameObject.SetActive(false);
                    wheelBarrow.gameObject.SetActive(false);
                    upgrades.SetActive(false);
                }
                break;
            case ZoneState.Locked:
                upgrades.SetActive(false);
                if (hideMachine)
                {
                    paintingMachine.gameObject.SetActive(false);
                    paintAmmo.gameObject.SetActive(false);
                    buildMachine.gameObject.SetActive(false);
                    asphaltAmmo.gameObject.SetActive(false);
                    wheelBarrow.gameObject.SetActive(false);
                    upgrades.SetActive(false);
                }
                break;
            default:
                break;
        }
    }

    public ZoneData SaveZone()
    {
        // save all levelprogress data and player data
        zoneData.ZoneState = zoneState;
        playerData.PlayerPosition = player.transform.position;
        playerData.PlayerRotation = player.transform.eulerAngles;
        if(player.wheelBarrow != null)
        {
            playerData.HasWheelBarrow = true;
            playerData.WheelBarrowPosition = player.wheelBarrow.transform.position;
            playerData.WheelBarrowRotation = player.wheelBarrow.transform.eulerAngles;
        }
        if (zoneState == ZoneState.Complete)
            playerData.HasWheelBarrow = false;
        playerData.Money = UIManager.instance.money;
        playerData.UpgradePoints = UIManager.instance.upgradePoints;
        //check for stage
        upgradeManager.shovelUpgrade.SaveUpgradeData();
        upgradeManager.loadUpgrade.SaveUpgradeData();

        buildMachineUpgradeMenu.buildMachineUpgrade.SaveUpgradeData();

        Database.Instance.SetPlayerData(playerData);

        return zoneData;
    }

    private void LoadUpgradePoints(List<UpgradePointData> upgradePointDatas)
    {
        UpgradePoint upgradePoint;
        for (int i = 0; i < upgradePointDatas.Count; i++)
        {
            upgradePoint = UpgradePointsPooler.instance.GetUpgradePoint();
            upgradePoint.LoadUpgradePoint(upgradePointDatas[i].Price, upgradePointDatas[i].Pos);
        }
    }

    public void SavePeelable(int index, bool isPeeled, bool isCollected, Vector3 collectablePosition, Vector3 collectableRotation)
    {
        zoneData.PeelableDatas[index].IsPeeled = isPeeled;
        zoneData.PeelableDatas[index].IsCollected = isCollected;
        zoneData.PeelableDatas[index].CollectablePosition = collectablePosition;
        zoneData.PeelableDatas[index].CollectableRotation = collectableRotation;
    }

    public void SaveBuildable(int index, bool isBuilded)
    {
        zoneData.BuildableDatas[index].IsBuilded = isBuilded;
    }

    public void SavePaintable(int index, bool IsPainted)
    {
        zoneData.PaintableDatas[index].IsPainted = IsPainted;
    }

    public void AddCollectableData(bool IsPlayer, CollectableType collectableType, Peelable peelable)
    {
        if (IsPlayer)
        {
            playerData.playerCollectables.Add(new CollectableData(collectableType, peelable));
        }
        else
        {
            playerData.wheelBarrowCollectables.Add(new CollectableData(collectableType, peelable));
        }
    }

    public void RemoveCollectableData(bool IsPlayer, CollectableType collectableType, Peelable peelable)
    {
        CollectableData collectableData = null;
        if (IsPlayer)
        {
            for (int i = 0; i < playerData.playerCollectables.Count; i++)
            {
                if(playerData.playerCollectables[i].CollectableType == collectableType && playerData.playerCollectables[i].Peelable == peelable)
                {
                    collectableData = playerData.playerCollectables[i];
                    break;
                }
            }
            if(collectableData != null)
                playerData.playerCollectables.Remove(collectableData);
        }
        else
        {
            for (int i = 0; i < playerData.wheelBarrowCollectables.Count; i++)
            {
                if (playerData.wheelBarrowCollectables[i].CollectableType == collectableType && playerData.wheelBarrowCollectables[i].Peelable == peelable)
                {
                    collectableData = playerData.wheelBarrowCollectables[i];
                    break;
                }
            }
            if (collectableData != null)
                playerData.wheelBarrowCollectables.Remove(collectableData);
        }
    }

    public void AddMoneyData(int index, int price)
    {
        zoneData.MoneyDatas.Add(new MoneyData(index, price));
    }

    public void RemoveMoneyData(int index, int price)
    {
        MoneyData moneyData = null;
        for (int i = 0; i < zoneData.MoneyDatas.Count; i++)
        {
            if(zoneData.MoneyDatas[i].Index == index && zoneData.MoneyDatas[i].Price == price)
            {
                moneyData = zoneData.MoneyDatas[i];
                break;
            }   
        }
        if (moneyData != null)
            zoneData.MoneyDatas.Remove(moneyData);
    }

    public void AddUpgradePointData(Vector3 pos, int price)
    {
        zoneData.UpgradePointDatas.Add(new UpgradePointData(pos, price));
    }

    public void RemoveUpgradePointData(Vector3 pos, int price)
    {
        UpgradePointData upgradePointData = null;
        for (int i = 0; i < zoneData.UpgradePointDatas.Count; i++)
        {
            if (zoneData.UpgradePointDatas[i].Pos == pos && zoneData.UpgradePointDatas[i].Price == price)
            {
                upgradePointData = zoneData.UpgradePointDatas[i];
                break;
            }
        }
        if (upgradePointData != null)
            zoneData.UpgradePointDatas.Remove(upgradePointData);
    }
}
