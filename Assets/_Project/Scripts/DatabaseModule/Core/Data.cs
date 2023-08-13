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
}

[System.Serializable]
public class PeelableData
{
    public bool IsPeeled;
    public bool IsCollected;
    public Vector3 CollectablePosition;
    public Vector3 CollectableRotation;
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
public class ObjectData
{
    public bool IsPeeled;
    public bool IsCollected;
    public Vector3 CollectablePosition;
    public Vector3 CollectableRotation;

    public bool IsBuilded;

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
    public int stageIndex;
    public ZoneState ZoneState;
    public List<StageData> StagesDatas;

    public List<MoneyData> MoneyDatas;
    public List<UpgradePointData> UpgradePointDatas;
}

[System.Serializable]
public class StageData
{
    public List<ObjectData> ObjectDatas;
}

[System.Serializable]
public class CollectableData
{
    public CollectableType CollectableType;
    public Peelable Peelable;

    public CollectableData(CollectableType collectableType, Peelable peelable)
    {
        CollectableType = collectableType;
        Peelable = peelable;
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
    public int StepIndex;
    public int Cost;
}

[System.Serializable]
public class MachineUpgradeData
{
    public MachineUpgradeType machineUpgradeType;
    public int Loops;
    public int Level;
    public float Value;
    public int StepIndex;
    public int Cost;
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