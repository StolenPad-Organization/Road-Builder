using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum StageType
{
    Locked,
    PeelingStage,
    BuildingStage,
    PaintingStage,
    Complete
}
public class Stage : MonoBehaviour
{
    public int maxProgress;
    public int currentProgress;

    public void OnStageStart()
    {
        //
    }

    public void OnStageProgress()
    {
        UIManager.instance.UpdateProgressBar((float)currentProgress / (float)maxProgress);
        if (currentProgress == maxProgress)
            OnStageComplete();
    }

    private void OnStageComplete()
    {
        //call next stage from zone manager
    }
}
