using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class BuildableManager : MonoBehaviour
{
    public List<Buildable> buildableParts;
    private List<MeshRenderer> renderers = new List<MeshRenderer>();
    [SerializeField] private Transform[] blockHolders;
    [SerializeField] private Material[] mats;
    [SerializeField] private Material[] copyMats;
    [SerializeField] private float copyCenter = 0.0f;
    [SerializeField] private float copySize = 1.5f;

    [Header("Upgrade Points Reward")]
    [SerializeField] private bool hasReward;
    private int rewardRate;
    private int rewardprogress;
    [SerializeField] private int totalRewards;

    public float percentageToComplete = 100;

    void Start()
    {
        if(totalRewards > 0)
            rewardRate = Mathf.RoundToInt(buildableParts.Count * 0.7f) / totalRewards;
    }

    public void OnBuild(Vector3 pos)
    {
        if (!hasReward) return;
        rewardprogress++;
        if(rewardprogress >= rewardRate)
        {
            rewardprogress = 0;

            UpgradePoint upgradePoint = UpgradePointsPooler.instance.GetUpgradePoint();
            upgradePoint.Spawn(1, pos + (Vector3.up * 0.5f));
        }
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
        var parts = buildableParts.Where(t => !t.buildableRenderer.enabled).ToList();
        Buildable target = null;
        float3 playerPos = GameManager.instance.player.transform.position;
        if (parts.Count < 0) return null;
        target = parts[0];
        float closestDistance = math.distance(target.transform.position, playerPos);
        for (int i = 1; i < parts.Count(); i++)
        {
            var tmpDistance = math.distance(parts[i].transform.position, playerPos);
            if (tmpDistance < closestDistance)
            {
                target = parts[i];
                closestDistance = tmpDistance;
            }
        }
        return target;
    }
}
