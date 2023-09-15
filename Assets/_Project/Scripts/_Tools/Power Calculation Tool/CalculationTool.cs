using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CalculationTool : EditorWindow
{
    private float startingPower = 0;
    private float peelableNumber = 0;
    private float peelableCost = 0;
    private float totalCost = 0;
    private float endPower = 0;
    private float endMoney = 0;

    [MenuItem("My Tools/Calculator")]
    public static void ShowWindow()
    {
        GetWindow(typeof(CalculationTool), false, "Calculator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Enter Block Info:", EditorStyles.boldLabel);

        peelableNumber = EditorGUILayout.FloatField("Peelable Number:", peelableNumber);
        peelableCost = EditorGUILayout.FloatField("Peelable Cost:", peelableCost);

        if (GUILayout.Button("Calculate"))
        {
            totalCost = peelableNumber * peelableCost;
        }

        EditorGUILayout.LabelField("Total Cost:", totalCost.ToString());
    }
}
