using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Upgrade : MonoBehaviour
{
    [Header("Properties")]
    public int loops;
    public int level;
    [SerializeField] private TextMeshProUGUI levelText;
    public float value;
    [SerializeField] private float addValue;
    [SerializeField] private Image[] steps;
    public int stepIndex;
    [SerializeField] private Color lockedStepColor;
    [SerializeField] private Color unlockedStepColor;

    [Header("Upgrade & Cost")]
    [SerializeField] private int cost;
    [SerializeField] private int costAddPrecentage;
    [SerializeField] private TextMeshProUGUI costText;

    void Start()
    {
        costText.text = cost.ToString();
        levelText.text = "Lvl " + level;
    }

    void Update()
    {
        
    }

    public void Buy()
    {
        cost += (costAddPrecentage * cost)/100;
        costText.text = cost.ToString();
        stepIndex++;
        if(stepIndex < steps.Length)
        {
            steps[stepIndex].color = unlockedStepColor;
        }
        else
        {
            loops++;
            stepIndex = 0;
            foreach (var step in steps)
            {
                step.color = lockedStepColor;
            }
            steps[0].color = unlockedStepColor;
        }
        level++;
        levelText.text = "Lvl " + level;
        value += addValue;
    }

    private void CheckButton()
    {

    }
}
