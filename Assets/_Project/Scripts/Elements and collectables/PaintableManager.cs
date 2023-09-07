using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class PaintableManager : MonoBehaviour
{
    public Transform angleTrigger;
    public List<Paintable> paintableParts;
    private List<MeshRenderer> renderers = new List<MeshRenderer>();
    [SerializeField] private Transform[] blockHolders;
    [SerializeField] private Material[] mats;

    void Start()
    {

    }

    private void OnEnable()
    {
        EventManager.OnToolLengthUpgrade += updateTriggerSize;
    }

    private void OnDisable()
    {
        EventManager.OnToolLengthUpgrade -= updateTriggerSize;
    }

    private void updateTriggerSize(float t, ZoneState zoneState)
    {
        if (angleTrigger == null || zoneState != ZoneState.PaintingStage) return;
        Vector3 scale = angleTrigger.localScale;
        scale.z = Mathf.Lerp(0.55f, 0.8f, t);
        angleTrigger.localScale = scale;
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

    public Paintable ReturnNearestPaintable()
    {
        var parts = paintableParts.Where(t => !t.paintableRenderer.enabled).ToList();
        Paintable target = null;
        float3 playerPos = PlayerController.instance.transform.position;
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
