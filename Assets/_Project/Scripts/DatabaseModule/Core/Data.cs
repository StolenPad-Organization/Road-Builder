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
    public Vector3 PlayerPosition;
    public Vector3 PlayerRotation;
}

[System.Serializable]
public class PeelableData
{
    public bool IsPeeled;
    public bool IsCollected;
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
    public LevelState LevelState;
    public List<PeelableData> PeelableDatas;
    public List<BuildableData> BuildableDatas;
    public List<PaintableData> PaintableDatas;
}