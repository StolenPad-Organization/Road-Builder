using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildableManager : MonoBehaviour
{
    [SerializeField] private Buildable[] buildableParts;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    [ContextMenu("Set Buildable Parts")]
    private void SetBuildableParts()
    {
        for (int i = 0; i < buildableParts.Length; i++)
        {
            buildableParts[i].SetBuildableEditor(i);
        }
    }

    public void LoadBuildables(List<BuildableData> buildableDatas)
    {
        for (int i = 0; i < buildableParts.Length; i++)
        {
            if (buildableDatas[i].IsBuilded)
            {
                buildableParts[i].LoadBuildable();
            }
        }
    }
}
