using System.Collections.Generic;
using UnityEngine;

public class DataList : ScriptableObject
{
    public LevelData LevelData;
    public PlayerData PlayerData;
    public List<UpgradeData> upgradeDatas;
    public List<LevelProgressData> LevelProgressDatas;
}