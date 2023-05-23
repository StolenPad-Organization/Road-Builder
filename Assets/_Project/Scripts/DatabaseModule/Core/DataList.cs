using System.Collections.Generic;
using UnityEngine;

public class DataList : ScriptableObject
{
    public LevelData LevelData;
    public PlayerData PlayerData;
    public List<LevelProgressData> LevelProgressDatas;
    public List<UpgradeData> upgradeDatas;
}