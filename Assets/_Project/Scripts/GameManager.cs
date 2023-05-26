using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelState
{
    PeelingStage,
    BuildingStage,
    PaintingStage
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private PlayerController player;
    [SerializeField] private LevelState levelState;
    [SerializeField] private GameObject removableBlock;
    [SerializeField] private GameObject upgrades;
    [SerializeField] private AsphaltMachine asphaltMachine;
    [SerializeField] private GameObject asphaltBlock;
    [SerializeField] private AsphaltAmmo asphaltAmmo;
    [SerializeField] private PaintingMachine paintingMachine;
    [SerializeField] private GameObject paintBlock;
    [SerializeField] private PaintAmmo paintAmmo;
    [SerializeField] private WheelBarrow wheelBarrow;
    [SerializeField] private SellManager sellManager;
    [SerializeField] private UpgradeManager upgradeManager;

    [Header("Blocks Progress")]
    [SerializeField] private int maxBlocks;
    [SerializeField] private int currentBlocks;
    [SerializeField] private PeelableManager peelableManager;

    [Header("Asphalt Progress")]
    [SerializeField] private int maxAsphalt;
    [SerializeField] private int currentAsphalt;
    [SerializeField] private BuildableManager buildableManager;

    [Header("Paint Progress")]
    [SerializeField] private int maxPaint;
    [SerializeField] private int currentPaint;
    [SerializeField] private PaintableManager paintableManager;

    [Header("level Progress")]
    private LevelData levelData;
    private LevelProgressData levelProgressData;
    private PlayerData playerData;

    private void Awake()
    {
        instance = this;
    }

    IEnumerator Start()
    {
        player = PlayerController.instance;
        levelData = Database.Instance.GetLevelData();
        levelProgressData = Database.Instance.GetLevelProgressData(levelData.LevelValue - 1);
        playerData = Database.Instance.GetPlayerData();
        yield return new WaitForEndOfFrame();
        LoadStage();
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
        levelState = LevelState.BuildingStage;
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
        levelState = LevelState.PaintingStage;
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
            Win();
    }
    private void Win()
    {
        Debug.Log("YOU WIN!");
        // Unlock next level and reset this level progress (if the player can play again this level)
    }

    private void LoadStage()
    {
        // Load player position and rotation
        player.transform.position = playerData.PlayerPosition;
        player.transform.eulerAngles = playerData.PlayerRotation;

        //load level State
        levelState = levelProgressData.LevelState;

        switch (levelState)
        {
            case LevelState.PeelingStage:
                // load collectables and peelables
                peelableManager.LoadPeelables(levelProgressData.PeelableDatas);
                if (playerData.HasWheelBarrow)
                {
                    wheelBarrow.transform.position = playerData.WheelBarrowPosition;
                    wheelBarrow.transform.eulerAngles = playerData.WheelBarrowRotation;
                    wheelBarrow.ActivateWheelBarrow();
                    wheelBarrow.LoadCollectables(playerData.wheelBarrowCollectables);
                }
                player.LoadCollectables(playerData.playerCollectables);
                sellManager.LoadMoeny(levelProgressData.MoneyDatas);
                break;
            case LevelState.BuildingStage:
                // load buildable and building machine
                player.RemovePeelingAndCollectingTools();
                buildableManager.LoadBuildables(levelProgressData.BuildableDatas);
                upgrades.SetActive(false);
                removableBlock.SetActive(false);
                asphaltMachine.gameObject.SetActive(true);
                asphaltBlock.SetActive(true);
                break;
            case LevelState.PaintingStage:
                // load paintable and painting machine and buildables
                player.RemovePeelingAndCollectingTools();
                buildableManager.LoadBuildables(levelProgressData.BuildableDatas);
                paintableManager.LoadPaintables(levelProgressData.PaintableDatas);
                upgrades.SetActive(false);
                removableBlock.SetActive(false);
                asphaltBlock.SetActive(true);
                paintingMachine.gameObject.SetActive(true);
                paintBlock.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    private void OnApplicationQuit()
    {
        SaveStage();
        Database.Instance.SaveData();
    }

    private void SaveStage()
    {
        // save all levelprogress data and player data
        levelProgressData.LevelState = levelState;
        playerData.PlayerPosition = player.transform.position;
        playerData.PlayerRotation = player.transform.eulerAngles;
        if(player.wheelBarrow != null)
        {
            playerData.HasWheelBarrow = true;
            playerData.WheelBarrowPosition = player.wheelBarrow.transform.position;
            playerData.WheelBarrowRotation = player.wheelBarrow.transform.eulerAngles;
        }
        playerData.Money = UIManager.instance.money;
        //check for stage
        upgradeManager.shovelUpgrade.SaveUpgradeData();
        upgradeManager.loadUpgrade.SaveUpgradeData();

        Database.Instance.SetLevelProgressData(levelProgressData, levelData.LevelValue - 1);
        Database.Instance.SetPlayerData(playerData);
    }

    public void SavePeelable(int index, bool isPeeled, bool isCollected)
    {
        levelProgressData.PeelableDatas[index].IsPeeled = isPeeled;
        levelProgressData.PeelableDatas[index].IsCollected = isCollected;
    }

    public void SaveBuildable(int index, bool isBuilded)
    {
        levelProgressData.BuildableDatas[index].IsBuilded = isBuilded;
    }

    public void SavePaintable(int index, bool IsPainted)
    {
        levelProgressData.PaintableDatas[index].IsPainted = IsPainted;
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
        levelProgressData.MoneyDatas.Add(new MoneyData(index, price));
    }

    public void RemoveMoneyData(int index, int price)
    {
        MoneyData moneyData = null;
        for (int i = 0; i < levelProgressData.MoneyDatas.Count; i++)
        {
            if(levelProgressData.MoneyDatas[i].Index == index && levelProgressData.MoneyDatas[i].Price == price)
            {
                moneyData = levelProgressData.MoneyDatas[i];
                break;
            }   
        }
        if (moneyData != null)
            levelProgressData.MoneyDatas.Remove(moneyData);
    }
}
