using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private LevelData levelData;
    public LevelProgressData levelProgressData;
    [SerializeField] private ZoneManager[] zones;
    public ZoneManager currentZone;
    private bool gameStarted;

    private void Awake()
    {
        instance = this;
    }

    IEnumerator Start()
    {
        YsoCorp.GameUtils.YCManager.instance.OnGameStarted(1);
        Application.targetFrameRate = 60;
        UIManager.instance.transitionAnim.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        levelData = Database.Instance.GetLevelData();
        levelProgressData = Database.Instance.GetLevelProgressData(levelData.LevelValue - 1);
        currentZone = zones[levelProgressData.ZoneIndex];
        LoadLevel();
        yield return new WaitForSeconds(0.1f);
        gameStarted = true;
        UIManager.instance.transitionAnim.enabled = true;
    }

    private void LoadLevel()
    {
        //load zones
        for (int i = 0; i < zones.Length; i++)
        {
            zones[i].InitZone(levelProgressData.ZoneDatas[i]);
        }
    }

    public void UnlockNextZone()
    {
        PlayerController.instance.ResetForNextZone();
        if (levelProgressData.ZoneIndex + 1 < levelProgressData.ZoneDatas.Count)
        {
            levelProgressData.ZoneIndex++;
            currentZone = zones[levelProgressData.ZoneIndex];
            currentZone.UnlockZone();
        }
        else
        {
            WinLevel();
        }
    }
    public void WinLevel()
    {
        YsoCorp.GameUtils.YCManager.instance.OnGameFinished(true);
        PlayerData playerData = Database.Instance.GetPlayerData();
        playerData.ResetData();
        Database.Instance.SetPlayerData(playerData);

        PaintMachineUpgradeData paintMachineUpgradeData = Database.Instance.GetPaintMachineUpgradeData(PaintMachineUpgradeType.CapacityUpgrade);
        paintMachineUpgradeData.ResetData();
        Database.Instance.SetPaintMachineUpgradeData(PaintMachineUpgradeType.CapacityUpgrade, paintMachineUpgradeData);

        paintMachineUpgradeData = Database.Instance.GetPaintMachineUpgradeData(PaintMachineUpgradeType.LengthUpgrade);
        paintMachineUpgradeData.ResetData();
        Database.Instance.SetPaintMachineUpgradeData(PaintMachineUpgradeType.LengthUpgrade, paintMachineUpgradeData);

        paintMachineUpgradeData = Database.Instance.GetPaintMachineUpgradeData(PaintMachineUpgradeType.WidthUpgrade);
        paintMachineUpgradeData.ResetData();
        Database.Instance.SetPaintMachineUpgradeData(PaintMachineUpgradeType.WidthUpgrade, paintMachineUpgradeData);

        BuildMachineUpgradeData machineUpgradeData = Database.Instance.GetBuildMachineUpgradeData(BuildMachineUpgradeType.AsphaltMachineUpgrade);
        machineUpgradeData.ResetData();
        Database.Instance.SetBuildMachineUpgradeData(BuildMachineUpgradeType.AsphaltMachineUpgrade, machineUpgradeData);

        machineUpgradeData = Database.Instance.GetBuildMachineUpgradeData(BuildMachineUpgradeType.RollingMachineUpgrade);
        machineUpgradeData.ResetData();
        Database.Instance.SetBuildMachineUpgradeData(BuildMachineUpgradeType.RollingMachineUpgrade, machineUpgradeData);

        machineUpgradeData = Database.Instance.GetBuildMachineUpgradeData(BuildMachineUpgradeType.GrilliageRollingMachineUpgrade);
        machineUpgradeData.ResetData();
        Database.Instance.SetBuildMachineUpgradeData(BuildMachineUpgradeType.GrilliageRollingMachineUpgrade, machineUpgradeData);

        UpgradeData upgradeData = Database.Instance.GetUpgradeData(UpgradeType.LoadUpgrade);
        upgradeData.ResetData();
        Database.Instance.SetUpgradeData(UpgradeType.LoadUpgrade, upgradeData);

        upgradeData = Database.Instance.GetUpgradeData(UpgradeType.ToolUpgrade);
        upgradeData.ResetData();
        Database.Instance.SetUpgradeData(UpgradeType.ToolUpgrade, upgradeData);

        upgradeData = Database.Instance.GetUpgradeData(UpgradeType.ToolLengthUpgrade);
        upgradeData.ResetData();
        Database.Instance.SetUpgradeData(UpgradeType.ToolLengthUpgrade, upgradeData);

        upgradeData = Database.Instance.GetUpgradeData(UpgradeType.ToolWidthUpgrade);
        upgradeData.ResetData();
        Database.Instance.SetUpgradeData(UpgradeType.ToolWidthUpgrade, upgradeData);

        Database.Instance.SaveData();
        EventManager.TriggerWin.Invoke();
    }

    private void OnApplicationQuit()
    {
        SaveLevel();
    }

    private void OnApplicationPause()
    {
        if(gameStarted)
            SaveLevel();
    }

    public void SaveLevel()
    {
        levelProgressData.ZoneDatas[levelProgressData.ZoneIndex] = currentZone.SaveZone();
        Database.Instance.SetLevelProgressData(levelProgressData, levelData.LevelValue - 1);
        Database.Instance.SaveData();
    }
}
