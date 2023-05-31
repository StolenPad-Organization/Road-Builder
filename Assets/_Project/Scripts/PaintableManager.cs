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
        for (int i = 0; i < paintableParts.Length; i++)
        {
            paintableParts[i].SetPaintableEditor(i);
        }
    }

    public void LoadPaintables(List<PaintableData> paintableDatas, bool check)
    {
        for (int i = 0; i < paintableParts.Length; i++)
        {
            if (paintableDatas[i].IsPainted)
            {
                paintableParts[i].LoadPaintable(check);
            }
        }
    }
}
