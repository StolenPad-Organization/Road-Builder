using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private LevelData levelData;
    private LevelProgressData levelProgressData;
    [SerializeField] private StageManager[] stages;
    public StageManager currentStage;
    private bool gameStarted;

    private void Awake()
    {
        instance = this;
    }

    IEnumerator Start()
    {
        Application.targetFrameRate = 60;
        yield return new WaitForSeconds(0.1f);
        levelData = Database.Instance.GetLevelData();
        levelProgressData = Database.Instance.GetLevelProgressData(levelData.LevelValue - 1);
        currentStage = stages[levelProgressData.StageIndex];
        LoadLevel();
        yield return new WaitForSeconds(0.1f);
        gameStarted = true;
    }

    void Update()
    {
        
    }

    private void LoadLevel()
    {
        for (int i = 0; i < stages.Length; i++)
        {
            stages[i].LoadStage(levelProgressData.StageDatas[i]);
        }
    }

    public void UnlockNextZone()
    {
        PlayerController.instance.ResetForNextZone();
        currentStage.UnlockNextZone();
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
        levelProgressData.StageDatas[levelProgressData.StageIndex] = currentStage.SaveStage();
        Database.Instance.SetLevelProgressData(levelProgressData, levelData.LevelValue - 1);
        Database.Instance.SaveData();
    }
}
