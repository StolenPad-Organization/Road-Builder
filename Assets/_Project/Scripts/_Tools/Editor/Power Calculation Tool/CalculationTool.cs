using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CalculationTool : EditorWindow
{
    private int powerPurchasePercentage = 80;
    private int initialPowerCost = 100;
    private int powerCostAddPercentage = 20;
    private int startingPower = 0;
    private int startingMoney = 0;
    private int peelableNumber = 0;
    private int peelableCost = 0;
    private int totalCost = 0;
    private int endPower = 0;
    private int endMoney = 0;

    [MenuItem("My Tools/Calculator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(CalculationTool), false, "Calculator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Enter Block Info:", EditorStyles.boldLabel);

        powerPurchasePercentage = EditorGUILayout.IntField("Power Purchase Percentage:", powerPurchasePercentage);
        startingPower = EditorGUILayout.IntField("Starting Power:", startingPower);
        startingMoney = EditorGUILayout.IntField("Starting Money:", startingMoney);
        peelableNumber = EditorGUILayout.IntField("Peelable Number:", peelableNumber);
        peelableCost = EditorGUILayout.IntField("Peelable Cost:", peelableCost);

        if (GUILayout.Button("Calculate"))
        {
            totalCost = peelableNumber * peelableCost;
            endMoney = startingMoney + totalCost;
            CalculatePower();
        }

        EditorGUILayout.LabelField("Total Cost:", totalCost.ToString());
        EditorGUILayout.LabelField("End Money:", endMoney.ToString());
        EditorGUILayout.LabelField("End Power:", endPower.ToString());
    }

    private void CalculatePower()
    {
        endPower = startingPower;
        int powerCost = initialPowerCost;
        for (int i = 1; i < endPower; i++)
        {
            powerCost += Mathf.RoundToInt(powerCost*(powerCostAddPercentage /100f));
        }
        int additionalPower = 0;
        while (endMoney >= powerCost)
        {
            endMoney -= powerCost;
            powerCost += Mathf.RoundToInt(powerCost * (powerCostAddPercentage / 100f));
            additionalPower++;
        }
        endPower += Mathf.RoundToInt(additionalPower * (powerPurchasePercentage / 100f));
    }
}
