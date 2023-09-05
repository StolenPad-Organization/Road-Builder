using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class Database : MonoBehaviour
{
    public static Database Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Database>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(Database).Name;
                    instance = obj.AddComponent<Database>();
                }
            }

            return instance;
        }
        set
        {
            instance = value;
        }
    }
    private static Database instance = null;
    [SerializeField] private DataList updaterData;
    private DataList data;
    private DataList dataBackup;

    public bool EditorMode = false;
    public bool ABMode = false;

    private void Awake()
    {
        #if UNITY_EDITOR
            EditorMode = true;
        #endif
        if(instance == null)
        {
            instance = this;
            data = LoadData();
            dataBackup = LoadDataBackup();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        if (!EditorMode)
        {
            CheckUpdate();
        }
    }

    private DataList LoadData()
    {
        DataList data = null;
        if (!EditorMode)
        {
            if (File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + "Database.txt"))
            {
                data = ScriptableObject.CreateInstance<DataList>();
                var json = File.ReadAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + "Database.txt");
                JsonUtility.FromJsonOverwrite(json, data);
            }
            else
            {
                data = Resources.Load<DataList>("Data");
                var json = JsonUtility.ToJson(data, true);
                File.WriteAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + "Database.txt", json);
            }
        }
        else
        {
            data = Resources.Load<DataList>("Data");
        }

        return data;
    }

    private DataList LoadDataBackup()
    {
        DataList data = null;
        data = Resources.Load<DataList>("Data Backup");

        return data;
    }

    public void SaveData()
    {
        var json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + Path.DirectorySeparatorChar + "Database.txt", json);
    }

    private void CheckUpdate()
    {
        //if(data.CharacterItemData.Count < updaterData.CharacterItemData.Count)
        //{
        //    for (int i = 0; i < data.CharacterItemData.Count; i++)
        //    {
        //        updaterData.CharacterItemData[i] = data.CharacterItemData[i];
        //    }
        //    data.CharacterItemData = updaterData.CharacterItemData;
        //}
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveData();
        }
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }

    public LevelData GetLevelData()
    {
        return data.LevelData;
    }

    public void SetLevelData(LevelData value)
    {
        data.LevelData = value;
    }

    public PlayerData GetPlayerData()
    {
        return data.PlayerData;
    }

    public void SetPlayerData(PlayerData value)
    {
        data.PlayerData = value;
    }

    public LevelProgressData GetLevelProgressData(int index)
    {
        return data.LevelProgressDatas[index];
    }

    public void SetLevelProgressData(LevelProgressData value, int index)
    {
        data.LevelProgressDatas[index] = value;
    }

    public UpgradeData GetUpgradeData(UpgradeType upgradeType)
    {
        for (int i = 0; i < data.UpgradeDatas.Count; i++)
        {
            if (data.UpgradeDatas[i].upgradeType == upgradeType)
                return data.UpgradeDatas[i];
        }

        return null;
    }

    public void SetUpgradeData(UpgradeType upgradeType, UpgradeData value)
    {
        for (int i = 0; i < data.UpgradeDatas.Count; i++)
        {
            if (data.UpgradeDatas[i].upgradeType == upgradeType)
                data.UpgradeDatas[i] = value;
        }
    }

    public BuildMachineUpgradeData GetBuildMachineUpgradeData(BuildMachineUpgradeType upgradeType)
    {
        for (int i = 0; i < data.BuildMachineUpgradeDatas.Count; i++)
        {
            if (data.BuildMachineUpgradeDatas[i].machineUpgradeType == upgradeType)
                return data.BuildMachineUpgradeDatas[i];
        }

        return null;
    }

    public void SetBuildMachineUpgradeData(BuildMachineUpgradeType upgradeType, BuildMachineUpgradeData value)
    {
        for (int i = 0; i < data.BuildMachineUpgradeDatas.Count; i++)
        {
            if (data.BuildMachineUpgradeDatas[i].machineUpgradeType == upgradeType)
                data.BuildMachineUpgradeDatas[i] = value;
        }
    }

    public PaintMachineUpgradeData GetPaintMachineUpgradeData(PaintMachineUpgradeType upgradeType)
    {
        for (int i = 0; i < data.PaintMachineUpgradeDatas.Count; i++)
        {
            if (data.PaintMachineUpgradeDatas[i].machineUpgradeType == upgradeType)
                return data.PaintMachineUpgradeDatas[i];
        }

        return null;
    }

    public void SetPaintMachineUpgradeData(PaintMachineUpgradeType upgradeType, PaintMachineUpgradeData value)
    {
        for (int i = 0; i < data.PaintMachineUpgradeDatas.Count; i++)
        {
            if (data.PaintMachineUpgradeDatas[i].machineUpgradeType == upgradeType)
                data.PaintMachineUpgradeDatas[i] = value;
        }
    }

    public bool GetVibrationData()
    {
        return data.useHaptic;
    }

    public void SetVibrationData(bool _useHaptic)
    {
        data.useHaptic = _useHaptic;
    }

    public void ResetAllData()
    {
        data.LevelData = dataBackup.LevelData;
        data.PlayerData = dataBackup.PlayerData;
        data.UpgradeDatas = dataBackup.UpgradeDatas;
        data.LevelProgressDatas = dataBackup.LevelProgressDatas;
        data.BuildMachineUpgradeDatas = dataBackup.BuildMachineUpgradeDatas;
        data.PaintMachineUpgradeDatas = dataBackup.PaintMachineUpgradeDatas;
        data.useHaptic = dataBackup.useHaptic;
    }
}