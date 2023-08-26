using HomaGames.HomaBelly;
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
        Application.targetFrameRate = 30;
        UIManager.instance.transitionAnim.gameObject.SetActive(true);
        DefaultAnalytics.GameplayStarted();
        yield return new WaitForSeconds(0.1f);
        DefaultAnalytics.LevelStarted(1);
        levelData = Database.Instance.GetLevelData();
        levelProgressData = Database.Instance.GetLevelProgressData(levelData.LevelValue - 1);
        currentZone = zones[levelProgressData.ZoneIndex];
        LoadLevel();
        yield return new WaitForSeconds(0.1f);
        gameStarted = true;
        UIManager.instance.transitionAnim.enabled = true;
    }

    void Update()
    {
        
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
        DefaultAnalytics.LevelCompleted();

        PlayerData playerData = Database.Instance.GetPlayerData();
        playerData.ResetData();
        Database.Instance.SetPlayerData(playerData);

        MachineUpgradeData machineUpgradeData = Database.Instance.GetMachineUpgradeData(MachineUpgradeType.AsphaltMachineUpgrade);
        machineUpgradeData.ResetData();
        Database.Instance.SetMachineUpgradeData(MachineUpgradeType.AsphaltMachineUpgrade, machineUpgradeData);

        machineUpgradeData = Database.Instance.GetMachineUpgradeData(MachineUpgradeType.RollingMachineUpgrade);
        machineUpgradeData.ResetData();
        Database.Instance.SetMachineUpgradeData(MachineUpgradeType.RollingMachineUpgrade, machineUpgradeData);

        machineUpgradeData = Database.Instance.GetMachineUpgradeData(MachineUpgradeType.GrilliageRollingMachineUpgrade);
        machineUpgradeData.ResetData();
        Database.Instance.SetMachineUpgradeData(MachineUpgradeType.GrilliageRollingMachineUpgrade, machineUpgradeData);

        UpgradeData upgradeData = Database.Instance.GetUpgradeData(UpgradeType.LoadUpgrade);
        upgradeData.ResetData();
        Database.Instance.SetUpgradeData(UpgradeType.LoadUpgrade, upgradeData);

        upgradeData = Database.Instance.GetUpgradeData(UpgradeType.ToolUpgrade);
        upgradeData.ResetData();
        Database.Instance.SetUpgradeData(UpgradeType.ToolUpgrade, upgradeData);

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
