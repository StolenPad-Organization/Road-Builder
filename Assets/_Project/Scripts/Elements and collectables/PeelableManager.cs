using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.Mathematics;
using System.Linq;

public class PeelableManager : MonoBehaviour
{
    public Transform angleTrigger;
    public List<Peelable> peelableParts;
    private List<MeshRenderer> renderers = new List<MeshRenderer>();
    [SerializeField] private Transform[] blockHolders;
    [SerializeField] private Transform[] copyBlockHolders;
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
    public float percentageToComplete = 100;

    void Start()
    {
        for (int i = 0; i < peelableBlockHolders.Count; i++)
        {
            peelableBlockHolders[i].SetPartsCount(percentageToComplete);
        }
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
        if (angleTrigger == null || zoneState != ZoneState.PeelingStage) return;
        Vector3 scale = angleTrigger.localScale;
        scale.z = Mathf.Lerp(0.55f, 0.8f, t);
        angleTrigger.localScale = scale;
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
        SetPeelableCopy();
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

    [ContextMenu("Set Peelable Copies")]
    private void SetPeelableCopy()
    {
        for (int i = 0; i < copyBlockHolders.Length; i++)
        {
            renderers.Clear();
            renderers = GetRenderers(copyBlockHolders[i], renderers);
            for (int j = 0; j < renderers.Count; j++)
            {
                if (renderers[j].gameObject.GetComponent<MeshCollider>())
                    DestroyImmediate(renderers[j].gameObject.GetComponent<MeshCollider>());
                if (renderers[j].gameObject.GetComponent<PeelableCopy>())
                    DestroyImmediate(renderers[j].gameObject.GetComponent<PeelableCopy>());

                renderers[j].gameObject.AddComponent<PeelableCopy>().SetPeelableCopy(peelableBlockHolders[i].peelableParts[j]);
            }
        }
    }
#endif

    public void LoadPeelables(List<PeelableData> PeelableDatas, int currentPeelableBlock)
    {
        for (int i = 0; i < peelableParts.Count; i++)
        {
            if (!PeelableDatas[i].IsCollected)
            {
                peelableParts[i].loadPeelable(PeelableDatas[i]);
            }
            if (PeelableDatas[i].IsPeeled)
            {
                GameManager.instance.currentZone.OnBlockRemove(peelableParts[i].blockNumber, true);
            }
        }
        //currentBlockNumber = currentPeelableBlock;
        CheckCurrentBlock();

        SetBlockHoldersStates();
    }

    public void SetBlockHoldersStates()
    {
        //RBManager.Instance.Clear();
        //for (int i = 0; i < peelableBlockHolders.Count; i++)
        //{
        //    if (i + 1 < currentBlockNumber)
        //    {
        //        peelableBlockHolders[i].SetRBHandlersState(false, true);
        //    }
        //    else if (i + 1 > currentBlockNumber)
        //    {
        //        peelableBlockHolders[i].SetRBHandlersState(false);
        //    }
        //    else if (i + 1 == currentBlockNumber)
        //    {
        //        peelableBlockHolders[i].SetRBHandlersState(true);
        //    }
        //    peelableBlockHolders[i].CheckCountLoad();
        //}
    }

    public void ShowCopyOnly()
    {
        for (int i = 0; i < peelableBlockHolders.Count; i++)
        {
            peelableBlockHolders[i].SetRBHandlersState(false);
        }
    }

    public void CheckBlock(int _blockNumber)
    {
        peelableBlockHolders[_blockNumber - 1].RemovePart();
        //CheckCurrentBlock();
    }

    public void CheckCurrentBlock()
    {
        for (int i = 0; i < peelableBlockHolders.Count; i++)
        {
            if (!peelableBlockHolders[i].CheckCount())
            {
                currentBlockNumber = i + 1;
                break;
            }
        }
        //if (peelableBlockHolders[currentBlockNumber - 1].CheckCount())
        //{
        //    currentBlockNumber++;
        //    if (currentBlockNumber > blocksNumbers.Length)
        //        return;
        //    //SetBlockHoldersStates();
        //}
    }

    public Peelable ReturnNearestPeelable()
    {
        CheckCurrentBlock();
        var parts = peelableParts.Where(t =>!t.peeled && !t.sold && !t.collected && t.blockNumber == currentBlockNumber).ToList();
        float3 playerPos = GameManager.instance.player.transform.position;
        Peelable target = null;
        if (parts.Count <= 0) return null;
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
    public Peelable ReturnNearestPeelable(Vector3 position)
    {
        CheckCurrentBlock();
        var parts = peelableParts.Where(t => !t.peeled && !t.sold && !t.collected && t.blockNumber == currentBlockNumber).ToList();
        Peelable target = null;
        if (parts.Count <= 0) return null;
        target = parts[0];
        float closestDistance = math.distance(target.transform.position, position);
        for (int i = 1; i < parts.Count(); i++)
        {
            var tmpDistance = math.distance(parts[i].transform.position, position);
            if (tmpDistance < closestDistance)
            {
                target = parts[i];
                closestDistance = tmpDistance;
            }
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

#if UNITY_EDITOR
    [ContextMenu("Set Blocks")]
    private void SetBlocks()
    {
        peelableParts.Clear();
        foreach (var item in peelableBlockHolders)
        {
            peelableParts.AddRange(item.peelableParts);
        }
    }
#endif
}
