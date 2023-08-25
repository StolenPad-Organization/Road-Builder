using System.Collections.Generic;
using UnityEngine;

public class DataList : ScriptableObject
{
    public LevelData LevelData;
    public PlayerData PlayerData;
    public List<UpgradeData> UpgradeDatas;
    public List<LevelProgressData> LevelProgressDatas;
    public List<MachineUpgradeData> MachineUpgradeDatas;
    public bool useHaptic;
}