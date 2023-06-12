using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PaintableManager : MonoBehaviour
{
    [SerializeField] private List<Paintable> paintableParts;
    private List<MeshRenderer> renderers = new List<MeshRenderer>();
    [SerializeField] private Transform[] blockHolders;
    [SerializeField] private Material[] mats;

    void Start()
    {

    }

    void Update()
    {

    }
#if UNITY_EDITOR
    [ContextMenu("Set Paintable Parts")]
    private void SetPaintableParts()
    {
        paintableParts.Clear();
        for (int i = 0; i < blockHolders.Length; i++)
        {
            renderers.Clear();
            renderers = GetRenderers(blockHolders[i], renderers);
            foreach (var item in renderers)
            {
                item.enabled = false;
                if (!item.GetComponent<Paintable>())
                    item.gameObject.AddComponent<Paintable>();
                if (!item.GetComponent<BoxCollider>())
                    item.gameObject.AddComponent<BoxCollider>();

                item.gameObject.GetComponent<BoxCollider>().isTrigger = true;
                paintableParts.Add(item.GetComponent<Paintable>());
            }
        }

        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(gameObject);

        for (int i = 0; i < paintableParts.Count; i++)
        {
            paintableParts[i].SetPaintableEditor(i);
        }
    }
    private List<MeshRenderer> GetRenderers(Transform t, List<MeshRenderer> renderers)
    {
        foreach (Transform item in t)
        {
            if (item.TryGetComponent<MeshRenderer>(out MeshRenderer rd))
            {
                renderers.Add(rd);
            }
            else
            {
                GetRenderers(item, renderers);
            }
        }
        return renderers;
    }

    [ContextMenu("Set Paintable Materials")]
    private void SetPaintableeMaterials()
    {
        for (int i = 0; i < blockHolders.Length; i++)
        {
            renderers.Clear();
            renderers = GetRenderers(blockHolders[i], renderers);
            foreach (var item in renderers)
            {
                item.gameObject.GetComponent<Paintable>().SetMaterialEditor(mats[i]);
            }
        }
    }
#endif

    public void LoadPaintables(List<PaintableData> paintableDatas, bool check)
    {
        for (int i = 0; i < paintableParts.Count; i++)
        {
            if (paintableDatas[i].IsPainted)
            {
                paintableParts[i].LoadPaintable(check);
            }
        }
    }
}
