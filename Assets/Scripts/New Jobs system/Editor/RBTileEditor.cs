using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RBTile))]
public class RBTileEditor : Editor
{
    private bool isUp = false;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        RBTile myTile = (RBTile)target;

        GUILayout.Space(20);

        // Check if already up
        if (!isUp)
        {
            if (GUILayout.Button("Up"))
            {
                myTile.MoveUp();
              
                isUp = true;
            }
        }

        // Check if up was previously executed
        if (isUp)
        {
            if (GUILayout.Button("Down"))
            {
                myTile.ResetPosition();

                isUp = false;
            }
        }
    }
}
