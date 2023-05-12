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
        foreach (var part in buildableParts)
        {
            part.SetBuildableEditor();
        }
    }
}
