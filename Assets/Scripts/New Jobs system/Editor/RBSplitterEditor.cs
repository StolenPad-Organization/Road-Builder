using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RBSplitter))]

public class RBSplitterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RBSplitter myScript = (RBSplitter)target;

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Split Box Collider"))
        {
            myScript.SplitBoxCollider();
        }
        if (GUILayout.Button("Clear"))
        {
            myScript.Clear();
        }
        GUILayout.EndHorizontal();
    }
}
