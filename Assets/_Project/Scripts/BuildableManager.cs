using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildableManager : MonoBehaviour
{
    [SerializeField] private List<Buildable> buildableParts;
    private List<Renderer> renderers = new List<Renderer>();
    [SerializeField] private Transform[] blockHolders;
    [SerializeField] private Material[] mats;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

#if UNITY_EDITOR
    [ContextMenu("Set Buildable Parts")]
    private void SetBuildableParts()
    {
        buildableParts.Clear();
        for (int i = 0; i < blockHolders.Length; i++)
        {
            renderers.Clear();
            renderers = GetRenderers(blockHolders[i], renderers);
            foreach (var item in renderers)
            {
                item.enabled = false;
                if (!item.GetComponent<Buildable>())
                    item.gameObject.AddComponent<Buildable>();
                if (!item.GetComponent<BoxCollider>())
                    item.gameObject.AddComponent<BoxCollider>();

                item.gameObject.GetComponent<BoxCollider>().isTrigger = true;
                buildableParts.Add(item.GetComponent<Buildable>());
            }
        }

        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(gameObject);

        for (int i = 0; i < buildableParts.Count; i++)
        {
            buildableParts[i].SetBuildableEditor(i);
        }
    }
    private List<Renderer> GetRenderers(Transform t, List<Renderer> renderers)
    {
        foreach (Transform item in t)
        {
            if (item.TryGetComponent<Renderer>(out Renderer rd))
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

    [ContextMenu("Set Buildable Materials")]
    private void SetBuildableMaterials()
    {
        for (int i = 0; i < blockHolders.Length; i++)
        {
            renderers.Clear();
            renderers = GetRenderers(blockHolders[i], renderers);
            foreach (var item in renderers)
            {
                item.gameObject.GetComponent<Buildable>().SetMaterialEditor(mats[i]);
            }
        }
    }
#endif

    public void LoadBuildables(List<BuildableData> buildableDatas, bool check)
    {
        for (int i = 0; i < buildableParts.Count; i++)
        {
            if (buildableDatas[i].IsBuilded)
            {
                buildableParts[i].LoadBuildable(check);
            }
        }
    }
}
