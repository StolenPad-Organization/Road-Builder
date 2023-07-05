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
    public AsphaltMachine asphaltMachine;
    [SerializeField] private GameObject asphaltBlock;
    public AsphaltAmmo asphaltAmmo;
    public PaintingMachine paintingMachine;
    [SerializeField] private GameObject paintBlock;
    public PaintAmmo paintAmmo;
    [SerializeField] private WheelBarrow wheelBarrow;
    public SellManager sellManager;
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] private Transform collectableParent;
    public Transform machinesPosition;
    [SerializeField] private GameObject zoneCollider;

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
        zoneState = ZoneState.BuildingStage;
        asphaltMachine.gameObject.SetActive(true);
        player.arrowController.PointToObject(asphaltMachine.gameObject);
    }
    public void StartAsphaltStage()
    {
        upgrades.SetActive(false);
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
        if (currentAsphalt == maxAsphalt)
            ShowPaintMachine();
    }
    private void ShowPaintMachine()
    {
        zoneState = ZoneState.PaintingStage;
        PlayerController.instance.GetOffAsphaltMachine();
        paintingMachine.gameObject.SetActive(true);
        player.arrowController.PointToObject(paintingMachine.gameObject);
    }
    public void StartPaintStage()
    {
        //PlayerController.instance.GetOffAsphaltMachine();
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
        if (currentPaint == maxPaint)
            StartCoroutine(CompleteZone());
    }
    private IEnumerator CompleteZone()
    {
        yield return new WaitForSeconds(1.0f);
        paintingMachine.transform.SetParent(null);
        paintingMachine.gameObject.SetActive(false);
        paintAmmo.gameObject.SetActive(false);

        zoneState = ZoneState.Complete;
        GameManager.instance.SaveLevel();
        // Unlock next Zone
        GameManager.instance.UnlockNextZone();
    }

    public void UnlockZone()
    {
        zoneData.ZoneState = ZoneState.PeelingStage;
        LoadZone();
        if(zoneCollider != null)
        {
            PlayerController.instance.arrowController.PointToObject(zoneCollider);
        }
    }

    private void LoadZone()
    {
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
                break;
            case ZoneState.BuildingStage:
                if (zoneCollider != null)
                    zoneCollider.gameObject.SetActive(false);
                // load buildable and building machine
                player.RemovePeelingAndCollectingTools();
                buildableManager.LoadBuildables(zoneData.BuildableDatas, true);
                upgrades.SetActive(false);
                removableBlock.SetActive(false);
                asphaltMachine.gameObject.SetActive(true);
                asphaltBlock.SetActive(true);
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
                break;
            case ZoneState.Locked:
                upgrades.SetActive(false);
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
        //check for stage
        upgradeManager.shovelUpgrade.SaveUpgradeData();
        upgradeManager.loadUpgrade.SaveUpgradeData();

        Database.Instance.SetPlayerData(playerData);

        return zoneData;
    }

    public void SavePeelable(int index, bool isPeeled, bool isCollected)
    {
        zoneData.PeelableDatas[index].IsPeeled = isPeeled;
        zoneData.PeelableDatas[index].IsCollected = isCollected;
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
}
