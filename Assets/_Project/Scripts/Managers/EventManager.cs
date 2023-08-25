using System;
using UnityEngine;

public static class EventManager
{
    #region Player Events
    public static Action<int, Transform> AddMoney;
    public static Action<int, Transform> AddUpgradePoint;
    public static Action PlayUpgradeEffect;
    #endregion

    #region HapticEvents
    public static Action<vibrationTypes> invokeHaptic;
    public static Action<bool> SwitchHaptic;
    #endregion

    #region Ending Events

    #endregion

    #region Level Events
    public static Action TriggerWin;
    public static Action TriggerLose;
    public static Action loadNextScene;
    public static Action loadSameScene;
    public static Action loadOpeningScene;
    #endregion
}