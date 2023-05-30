using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    private StageData stageData;
    [SerializeField] private ZoneManager[] zones;
    public ZoneManager currentZone;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void LoadStage(StageData _stageData)
    {
        stageData = _stageData;
        currentZone = zones[stageData.ZoneIndex];

        //load zones
        for (int i = 0; i < zones.Length; i++)
        {
            zones[i].InitZone(stageData.ZoneDatas[i]);
        }
    }

    public void UnlockNextZone()
    {
        stageData.ZoneIndex++;
        currentZone = zones[stageData.ZoneIndex];
        currentZone.UnlockZone();
    }

    public StageData SaveStage()
    {
        stageData.ZoneDatas[stageData.ZoneIndex] = currentZone.SaveZone();
        return stageData;
    }
}
