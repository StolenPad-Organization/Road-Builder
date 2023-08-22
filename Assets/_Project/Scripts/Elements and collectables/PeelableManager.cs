using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PeelableManager : MonoBehaviour
{
    public List<Peelable> peelableParts;
    private List<MeshRenderer> renderers = new List<MeshRenderer>();
    [SerializeField] private Transform[] blockHolders;
    public List<PeelableBlockHolder> peelableBlockHolders;
    [SerializeField] private Material[] mats;
    [SerializeField] private float[] powers;
    [SerializeField] private float[] speeds;
    [SerializeField] private int zoneIndex;
    [SerializeField] private CollectableShape[] collectableShapes;
    [SerializeField] private Color[] dustColors;
    public int currentBlockNumber = 1;
    [SerializeField] private int[] blocksNumbers;
    [SerializeField] private int[] prices;
    [SerializeField] private Color movedPieceColor;

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
        peelableBlockHolders.Clear();
        PeelableBlockHolder peelableBlockHolder;
        peelableParts.Clear();
        for (int i = 0; i < blockHolders.Length; i++)
        {
            if (blockHolders[i].gameObject.GetComponent<PeelableBlockHolder>())
                DestroyImmediate(blockHolders[i].gameObject.GetComponent<PeelableBlockHolder>());
            peelableBlockHolder = blockHolders[i].gameObject.AddComponent<PeelableBlockHolder>();
            peelableBlockHolders.Add(peelableBlockHolder);
            renderers.Clear();
            renderers = GetRenderers(blockHolders[i], renderers);
            foreach (var item in renderers)
            {
                if (!item.GetComponent<Peelable>())
                    item.gameObject.AddComponent<Peelable>();

                if (!item.GetComponent<MeshCollider>())
                    item.GetComponent<Peelable>().peelableCollider = item.gameObject.AddComponent<MeshCollider>();
                else
                    item.GetComponent<Peelable>().peelableCollider = item.gameObject.GetComponent<MeshCollider>();

                if (!item.GetComponent<Rigidbody>())
                    item.GetComponent<Peelable>().rb = item.gameObject.AddComponent<Rigidbody>();

                item.gameObject.GetComponent<MeshCollider>().convex = true;
                item.gameObject.GetComponent<MeshCollider>().isTrigger = false;
                item.GetComponent<Peelable>().rb.constraints = RigidbodyConstraints.FreezeAll;

                peelableParts.Add(item.GetComponent<Peelable>());
                peelableBlockHolder.AddPeelablePart(item.GetComponent<Peelable>());
            }
        }

        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(gameObject);

        for (int i = 0; i < peelableParts.Count; i++)
        {
            peelableParts[i].SetPeelableEditor(i);
        }

        SetPeelableMaterials();
        SetPowerSpeed();
        SetZoneIndex();
        SetCollectableShape();
        SetDustColors();
        SetBlocksNumbers();
        SetPrices();
        SetMovedColor();
    }

    [ContextMenu("Set Blocks RBHandlers")]
    private void SetBlocksRBHandlers()
    {
        foreach (var item in peelableBlockHolders)
        {
            item.AddRBHandlers();
        }
    }

    private List<MeshRenderer> GetRenderers(Transform t,List<MeshRenderer> renderers)
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
        for (int i = 0; i < blockHolders.Length; i++)
        {
            renderers.Clear();
            renderers = GetRenderers(blockHolders[i], renderers);
            foreach (var item in renderers)
            {
                item.gameObject.GetComponent<Peelable>().SetMaterialEditor(mats[i]);
            }
        }
    }

    private void SetPowerSpeed()
    {
        for (int i = 0; i < blockHolders.Length; i++)
        {
            renderers.Clear();
            renderers = GetRenderers(blockHolders[i], renderers);
            foreach (var item in renderers)
            {
                item.gameObject.GetComponent<Peelable>().SetPowerSpeed(powers[i],speeds[i]);
            }
        }
    }

    private void SetZoneIndex()
    {
        for (int i = 0; i < blockHolders.Length; i++)
        {
            renderers.Clear();
            renderers = GetRenderers(blockHolders[i], renderers);
            foreach (var item in renderers)
            {
                item.gameObject.GetComponent<Peelable>().zoneIndex = zoneIndex;
            }
        }
    }

    private void SetCollectableShape()
    {
        for (int i = 0; i < blockHolders.Length; i++)
        {
            renderers.Clear();
            renderers = GetRenderers(blockHolders[i], renderers);
            foreach (var item in renderers)
            {
                item.gameObject.GetComponent<Peelable>().collectableShape = collectableShapes[i];
                if (collectableShapes[i] != CollectableShape.Original)
                    item.gameObject.GetComponent<Peelable>().diffirentCollectable = true;
                else
                    item.gameObject.GetComponent<Peelable>().diffirentCollectable = false;
            }
        }
    }

    private void SetDustColors()
    {
        for (int i = 0; i < blockHolders.Length; i++)
        {
            renderers.Clear();
            renderers = GetRenderers(blockHolders[i], renderers);
            foreach (var item in renderers)
            {
                item.gameObject.GetComponent<Peelable>().dustColor = dustColors[i];
            }
        }
    }

    private void SetBlocksNumbers()
    {
        for (int i = 0; i < blockHolders.Length; i++)
        {
            renderers.Clear();
            renderers = GetRenderers(blockHolders[i], renderers);
            foreach (var item in renderers)
            {
                item.gameObject.GetComponent<Peelable>().blockNumber = blocksNumbers[i];
            }
        }
    }

    private void SetPrices()
    {
        for (int i = 0; i < blockHolders.Length; i++)
        {
            renderers.Clear();
            renderers = GetRenderers(blockHolders[i], renderers);
            foreach (var item in renderers)
            {
                item.gameObject.GetComponent<Peelable>().price = prices[i];
            }
        }
    }

    private void SetMovedColor()
    {
        for (int i = 0; i < blockHolders.Length; i++)
        {
            renderers.Clear();
            renderers = GetRenderers(blockHolders[i], renderers);
            foreach (var item in renderers)
            {
                item.gameObject.GetComponent<Peelable>().movedPieceColor = movedPieceColor;
            }
        }
    }
#endif

    public void LoadPeelables(List<PeelableData> PeelableDatas)
    {
        for (int i = 0; i < peelableParts.Count; i++)
        {
            if (!PeelableDatas[i].IsCollected)
            {
                peelableParts[i].loadPeelable(PeelableDatas[i]);
            }
            if (PeelableDatas[i].IsPeeled)
            {
                GameManager.instance.currentZone.OnBlockRemove();
            }
        }
        SetBlockHoldersStates();
    }

    public void SetBlockHoldersStates()
    {
        for (int i = 0; i < peelableBlockHolders.Count; i++)
        {
            if (i + 1 < currentBlockNumber)
            {
                peelableBlockHolders[i].SetRBHandlersState(false, true);
            }
            else if (i + 1 > currentBlockNumber)
            {
                peelableBlockHolders[i].SetRBHandlersState(false);
            }
            else
            {
                peelableBlockHolders[i].SetRBHandlersState(true);
            }
        }
    }

    public Peelable ReturnNearestPeelable()
    {
        Peelable target = null; 
        for (int i = 0; i < peelableParts.Count; i++)
        {
            if (peelableParts[i].blockNumber != currentBlockNumber) 
                continue;
            if(target == null)
            {
                if (!peelableParts[i].collected && !peelableParts[i].sold)
                    target = peelableParts[i];
            }
            else
            {
                if (!peelableParts[i].collected && !peelableParts[i].sold
                    && Vector3.Distance(peelableParts[i].transform.position,PlayerController.instance.transform.position) 
                    < Vector3.Distance(target.transform.position, PlayerController.instance.transform.position))
                {
                    target = peelableParts[i];
                }
            }
        }

        if(target == null)
        {
            currentBlockNumber++;
            if (currentBlockNumber > blocksNumbers.Length)
                return null;
            else
                return ReturnNearestPeelable();
        }

        return target;
    }

    public Peelable ReturnPeelableWithIndex(int index)
    {
        Peelable peelable = null;
        for (int i = 0; i < peelableParts.Count; i++)
        {
            if (peelableParts[i].index == index)
                peelable = peelableParts[i];
        }
        return peelable;
    }

    public void SaveAllPeelabes()
    {
        for (int i = 0; i < peelableParts.Count; i++)
        {
            peelableParts[i].SavePeelable();
        }
    }
}
