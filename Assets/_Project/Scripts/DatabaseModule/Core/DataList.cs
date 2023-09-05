using System.Collections.Generic;
using UnityEngine;

public class DataList : ScriptableObject
{
    public LevelData LevelData;
    public PlayerData PlayerData;
    public List<UpgradeData> UpgradeDatas;
    public List<LevelProgressData> LevelProgressDatas;
    public List<BuildMachineUpgradeData> BuildMachineUpgradeDatas;
    public List<PaintMachineUpgradeData> PaintMachineUpgradeDatas;
    public bool useHaptic;
}