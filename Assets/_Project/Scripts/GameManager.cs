using HomaGames.HomaBelly;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private LevelData levelData;
    private LevelProgressData levelProgressData;
    [SerializeField] private ZoneManager[] zones;
    public ZoneManager currentZone;
    private bool gameStarted;

    private void Awake()
    {
        instance = this;
    }

    IEnumerator Start()
    {
        DefaultAnalytics.GameplayStarted();
        Application.targetFrameRate = 60;
        yield return new WaitForSeconds(0.1f);
        DefaultAnalytics.LevelStarted(1);
        levelData = Database.Instance.GetLevelData();
        levelProgressData = Database.Instance.GetLevelProgressData(levelData.LevelValue - 1);
        currentZone = zones[levelProgressData.ZoneIndex];
        LoadLevel();
        yield return new WaitForSeconds(0.1f);
        gameStarted = true;
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
        Debug.Log("Level Complete , YOU WIN!");
        DefaultAnalytics.LevelCompleted();
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
