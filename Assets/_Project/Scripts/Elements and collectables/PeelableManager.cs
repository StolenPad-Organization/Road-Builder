using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PeelableManager : MonoBehaviour
{
    [SerializeField] private List<Peelable> peelableParts;
    private List<MeshRenderer> renderers = new List<MeshRenderer>();
    [SerializeField] private Transform[] blockHolders;
    [SerializeField] private Material[] mats;
    [SerializeField] private float[] powers;
    [SerializeField] private float[] speeds;

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
                if (!item.GetComponent<BoxCollider>())
                    item.gameObject.AddComponent<BoxCollider>();

                item.gameObject.GetComponent<BoxCollider>().isTrigger = true;
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
#endif

    public void LoadPeelables(List<PeelableData> PeelableDatas)
    {
        for (int i = 0; i < peelableParts.Count; i++)
        {
            if (PeelableDatas[i].IsPeeled)
            {
                if (!PeelableDatas[i].IsCollected)
                {
                    peelableParts[i].LoadCollectable();
                }
                peelableParts[i].gameObject.SetActive(false);
                GameManager.instance.currentZone.OnBlockRemove();
            }
        }
    }

    public Peelable ReturnNearestPeelable()
    {
        Peelable target = null; 
        for (int i = 0; i < peelableParts.Count; i++)
        {
            if(target == null)
            {
                if (peelableParts[i].gameObject.activeInHierarchy)
                    target = peelableParts[i];
            }
            else
            {
                if (peelableParts[i].gameObject.activeInHierarchy
                    && Vector3.Distance(peelableParts[i].transform.position,PlayerController.instance.transform.position) 
                    < Vector3.Distance(target.transform.position, PlayerController.instance.transform.position))
                {
                    target = peelableParts[i];
                }
            }
            
        }

        return target;
    }
}
