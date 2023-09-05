using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public int LevelValue = 1;
    public int LevelTextValue = 1;
}

[System.Serializable]
public class PlayerData
{
    public int Money = 0;
    public int UpgradePoints = 0;
    public Vector3 PlayerPosition;
    public Vector3 PlayerRotation;
    public bool HasWheelBarrow;
    public Vector3 WheelBarrowPosition;
    public Vector3 WheelBarrowRotation;
    public List<CollectableData> wheelBarrowCollectables;
    public List<CollectableData> playerCollectables;

    public void ResetData()
    {
        Money = 0;
        UpgradePoints = 0;
        PlayerPosition = Vector3.zero;
        PlayerRotation = Vector3.zero;
        HasWheelBarrow = false;
        WheelBarrowPosition = Vector3.zero;
        WheelBarrowRotation = Vector3.zero;
        wheelBarrowCollectables.Clear();
        playerCollectables.Clear();
    }
}

[System.Serializable]
public class PeelableData
{
    public bool IsPeeled;
    public bool IsCollected;
    public bool IsSold;
    public bool IsMoved;
    public Vector3 PeelablePosition;
    public Vector3 PeelableRotation;
}

[System.Serializable]
public class BuildableData
{
    public bool IsBuilded;
}

[System.Serializable]
public class PaintableData
{
    public bool IsPainted;
}

[System.Serializable]
public class LevelProgressData
{
    public int ZoneIndex = 0;
    public List<ZoneData> ZoneDatas;
}

[System.Serializable]
public class ZoneData
{
    public ZoneState ZoneState;
    public int currentPeelableBlock;
    public List<PeelableData> PeelableDatas;
    public List<BuildableData> BuildableDatas;
    public List<PaintableData> PaintableDatas;
    public List<MoneyData> MoneyDatas;
    public List<UpgradePointData> UpgradePointDatas;
}

[System.Serializable]
public class CollectableData
{
    public int index;

    public CollectableData(int _index)
    {
        index = _index;
    }
}

[System.Serializable]
public class MoneyData
{
    public int Index;
    public int Price;

    public MoneyData(int index, int price)
    {
        Index = index;
        Price = price;
    }
}

[System.Serializable]
public class UpgradeData
{
    public UpgradeType upgradeType;
    public int Loops;
    public int Level;
    public float Value;
    public float OriginalValue;
    public int StepIndex;
    public int Cost;

    public void ResetData()
    {
        Loops = 0;
        Level = 1;
        switch (upgradeType)
        {
            case UpgradeType.ToolUpgrade:
                Value = OriginalValue;
                break;
            case UpgradeType.LoadUpgrade:
                if(Database.Instance.GetLevelData().LevelTextValue % 2 == 0)
                    Value = OriginalValue/2;
                else
                    Value = OriginalValue;
                break;
            default:
                break;
        }
        
        StepIndex = 0;
        Cost = 20;
    }
}

[System.Serializable]
public class BuildMachineUpgradeData
{
    public BuildMachineUpgradeType machineUpgradeType;
    public int Loops;
    public int Level;
    public float Value;
    public int StepIndex;
    public int Cost;

    public void ResetData()
    {
        Loops = 0;
        Level = 1;
        Value = 1;
        StepIndex = 0;
        Cost = 1;
    }
}

[System.Serializable]
public class PaintMachineUpgradeData
{
    public PaintMachineUpgradeType machineUpgradeType;
    public int Loops;
    public int Level;
    public float Value;
    public int StepIndex;
    public int Cost;

    public void ResetData()
    {
        Loops = 0;
        Level = 1;
        Value = 1;
        StepIndex = 0;
        Cost = 1;
    }
}

[System.Serializable]
public class UpgradePointData
{
    public Vector3 Pos;
    public int Price;

    public UpgradePointData(Vector3 pos, int price)
    {
        Pos = pos;
        Price = price;
    }
}