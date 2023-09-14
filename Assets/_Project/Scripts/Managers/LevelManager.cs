﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private int levelCount;
    #endregion

    private void OnEnable ()
    {
        EventManager.loadOpeningScene += LoadOpeningLevel;
        EventManager.loadNextScene += LoadNextLevel;
        EventManager.loadSameScene += LoadSameLevel;
        EventManager.GetCurrentLevel += FindCorrectSceneForAB;
    }

    private void OnDisable ()
    {
        EventManager.loadOpeningScene -= LoadOpeningLevel;
        EventManager.loadNextScene -= LoadNextLevel;
        EventManager.loadSameScene -= LoadSameLevel;
        EventManager.GetCurrentLevel -= FindCorrectSceneForAB;
    }

    void LoadOpeningLevel ()
    {
        LevelData data = Database.Instance.GetLevelData();
        if (data.LevelValue > levelCount)
        {
            data.LevelValue = 1;
        }

        Database.Instance.SetLevelData(data);
        Database.Instance.SaveData();
        SceneManager.LoadScene(FindCorrectSceneForAB());
    }

    void LoadNextLevel ()
    {
        LevelData data = Database.Instance.GetLevelData();
        data.LevelValue++;
        data.LevelTextValue++;

        //#if !UNITY_EDITOR
        //            ElephantManager.instance.SendAdjustLevelEvent(data.LevelTextValue);
        //#endif

        if (data.LevelValue > levelCount)
        {
            data.LevelValue = 1;
            Database.Instance.ResetAllData();
        }

        Database.Instance.SetLevelData(data);
        Database.Instance.SaveData();
        SceneManager.LoadScene("Level Select");
    }

    void LoadSameLevel ()
    {
        //SceneManager.LoadScene(FindCorrectSceneForAB());

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private string FindCorrectSceneForAB ()
    {
        LevelData data = Database.Instance.GetLevelData();
        string sceneName = "Level ";
        //if (data.LevelTextValue == 1)
        //{
        //    if (RemoteManager.Instance.remoteHardTutorial == 0)
        //    {
        //        sceneName = sceneName + "1 A";
        //    }
        //    else if (RemoteManager.Instance.remoteHardTutorial == 1)
        //    {
        //        sceneName = sceneName + "1 B";
        //    }
        //    else if (RemoteManager.Instance.remoteHardTutorial == 2)
        //    {
        //        sceneName = sceneName + "1 C";
        //    }
        //}
        //else 
        //if (data.LevelTextValue != 1)
        //{
        //    sceneName = sceneName + data.LevelValue;
        //}

        sceneName = sceneName + data.LevelValue;

        //if (RemoteManager.Instance.remoteArtType == 1)
        //    sceneName = sceneName + " AB";

        return sceneName;
    }
}