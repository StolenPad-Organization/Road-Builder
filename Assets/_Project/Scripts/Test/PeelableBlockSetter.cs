using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PeelableBlockSetter : MonoBehaviour
{
    [SerializeField] private int startingIndex;
    public List<Peelable> peelableParts;
    private List<MeshRenderer> renderers = new List<MeshRenderer>();
    [SerializeField] private Transform blockHolder;
    public PeelableBlockHolder peelableBlockHolder;
    [SerializeField] private Material mat;
    [SerializeField] private float power;
    [SerializeField] private float speed;
    [SerializeField] private int zoneIndex;
    [SerializeField] private CollectableShape collectableShape;
    [SerializeField] private Color dustColor;
    [SerializeField] private int blocksNumber;
    [SerializeField] private int price;
    [SerializeField] private Color movedPieceColor;
    [SerializeField] private Transform copyBlockHolder;

    void Start()
    {

    }

    void Update()
    {

    }

#if UNITY_EDITOR
    [ContextMenu("Set Peelable Parts")]
    private void SetPeelableParts()
    {
        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(gameObject);

        for (int i = 0; i < peelableParts.Count; i++)
        {
            peelableParts[i].SetPeelableEditor(i + startingIndex);
        }

        SetPeelableMaterials();
        SetPowerSpeed();
        SetZoneIndex();
        SetCollectableShape();
        SetDustColors();
        SetBlocksNumbers();
        SetPrices();
        SetMovedColor();
        SetPeelableCopy();
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

    private void SetPeelableMaterials()
    {
        foreach (var item in peelableParts)
        {
            item.SetMaterialEditor(mat);
        }
    }

    private void SetPowerSpeed()
    {
        foreach (var item in peelableParts)
        {
            item.SetPowerSpeed(power, speed);
        }
    }

    private void SetZoneIndex()
    {
        foreach (var item in peelableParts)
        {
            item.zoneIndex = zoneIndex;
        }
    }

    private void SetCollectableShape()
    {
        foreach (var item in peelableParts)
        {
            item.collectableShape = collectableShape;
            if (collectableShape != CollectableShape.Original)
                item.diffirentCollectable = true;
            else
                item.diffirentCollectable = false;
        }
    }

    private void SetDustColors()
    {
        foreach (var item in peelableParts)
        {
            item.dustColor = dustColor;
        }
    }

    private void SetBlocksNumbers()
    {
        foreach (var item in peelableParts)
        {
            item.blockNumber = blocksNumber;
        }
    }

    private void SetPrices()
    {
        foreach (var item in peelableParts)
        {
            item.price = price;
        }
    }

    private void SetMovedColor()
    {
        foreach (var item in peelableParts)
        {
            item.movedPieceColor = movedPieceColor;
        }
    }

    [ContextMenu("Set Peelable Copies")]
    private void SetPeelableCopy()
    {
        for (int i = 0; i < copyBlockHolder.childCount; i++)
        {
            copyBlockHolder.GetChild(i).gameObject.GetComponent<PeelableCopy>().SetPeelableCopy(peelableParts[i], mat);
        }
    }
#endif
}
