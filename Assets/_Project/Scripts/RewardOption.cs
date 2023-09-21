using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public enum RewardType
{
    money,
    upgradePoint,
    Skin,
    Mount
}

[System.Serializable]
public class RewardOption
{
    public RewardType rewardType;
    [TextArea]
    public string rewardName;
    public int value;
    public Sprite rewardIcon;

    public void ClaimReward()
    {   
        // switch rewardtype
        // add value to player data 
        // + money or + upgrade points + true to skin or mount bool
    }
}
