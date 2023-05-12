using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintableManager : MonoBehaviour
{
    [SerializeField] private Paintable[] paintableParts;
    void Start()
    {

    }

    void Update()
    {

    }

    [ContextMenu("Set Paintable Parts")]
    private void SetPaintableParts()
    {
        foreach (var part in paintableParts)
        {
            part.SetPaintableEditor();
        }
    }
}
