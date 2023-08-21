using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PeelableManager : MonoBehaviour
{
    public List<Peelable> peelableParts;
    private List<MeshRenderer> renderers = new List<MeshRenderer>();
    [SerializeField] private Transform[] blockHolders;
    [SerializeField] private Material[] mats;
    [SerializeField] private float[] powers;
    [SerializeField] private float[] speeds;
    [SerializeField] private int zoneIndex;
    [SerializeField] private CollectableShape[] collectableShapes;
    [SerializeField] private Color[] dustColors;
    [SerializeField] private int[] blocksNumbers;
    [SerializeField] private int[] prices;
    [SerializeField] private Color movedPieceColor;
    private int currentBlockNumber = 1;

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
        peelableParts.Clear();
        for (int i = 0; i < blockHolders.Length; i++)
        {
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
            }
        }

        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(gameObject);

        for (int i = 0; i < peelableParts.Count; i++)
        {
            peelableParts[i].SetPeelableEditor(i);
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

    [ContextMenu("Set Peelable Materials")]
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

    [ContextMenu("Set Power and Speed")]
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

    [ContextMenu("Set Zone Index")]
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

    [ContextMenu("Set Collectable Shape")]
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

    [ContextMenu("Set Dust Colors")]
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

    [ContextMenu("Set Blocks Numbers")]
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

    [ContextMenu("Set Prices")]
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

    [ContextMenu("Set Moved Color")]
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
