using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyPosition : MonoBehaviour
{
    [SerializeField] private GameObject[] oldObjects;
    [SerializeField] private GameObject[] newObjects;

#if UNITY_EDITOR
    [ContextMenu("Set New Objects Position")]
    private void SetPosition()
    {
        for (int i = 0; i < newObjects.Length; i++)
        {
            newObjects[i].transform.SetPositionAndRotation(oldObjects[i].transform.position, oldObjects[i].transform.rotation);
            UnityEditor.EditorUtility.SetDirty(newObjects[i]);
        }
    }
#endif
}
