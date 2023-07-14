using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BuildableManager : MonoBehaviour
{
    [SerializeField] private List<Buildable> buildableParts;
    private List<MeshRenderer> renderers = new List<MeshRenderer>();
    [SerializeField] private Transform[] blockHolders;
    [SerializeField] private Material[] mats;
    [SerializeField] private Material[] copyMats;
    [SerializeField] private float copyCenter = 0.0f;
    [SerializeField] private float copySize = 1.5f;

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

    [ContextMenu("Set Buildable Copy")]
    private void SetBuildableCopy()
    {
        Undo.IncrementCurrentGroup();
        Undo.SetCurrentGroupName("created copies");
        int undoId = Undo.GetCurrentGroup();
        for (int i = 0; i < blockHolders.Length; i++)
        {
            renderers.Clear();
            renderers = GetRenderers(blockHolders[i], renderers);
            foreach (var item in renderers)
            {
                var copy = item.gameObject.GetComponent<Buildable>().SetBuildableCopy(copyMats[i], copyCenter, copySize);
                Undo.RegisterCreatedObjectUndo(copy, "created Copy");
            }
        }
        Undo.CollapseUndoOperations(undoId);
    }

    [ContextMenu("Remove Buildable Copy")]
    private void RemoveBuildableCopy()
    {
        for (int i = 0; i < blockHolders.Length; i++)
        {
            renderers.Clear();
            renderers = GetRenderers(blockHolders[i], renderers);
            foreach (var item in renderers)
            {
                item.gameObject.GetComponent<Buildable>().RemoveCopy();
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

    public Buildable ReturnNearestBuildable()
    {
        Buildable target = null;
        for (int i = 0; i < buildableParts.Count; i++)
        {
            if (target == null)
            {
                if (!buildableParts[i].buildableRenderer.enabled)
                    target = buildableParts[i];
            }
            else
            {
                if (!buildableParts[i].buildableRenderer.enabled
                    && Vector3.Distance(buildableParts[i].transform.position, PlayerController.instance.transform.position)
                    < Vector3.Distance(target.transform.position, PlayerController.instance.transform.position))
                {
                    target = buildableParts[i];
                }
            }

        }

        return target;
    }
}
