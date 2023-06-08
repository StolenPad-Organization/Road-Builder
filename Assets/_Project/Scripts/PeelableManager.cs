using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeelableManager : MonoBehaviour
{
    [SerializeField] private Peelable[] peelableParts;

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
        for (int i = 0; i < peelableParts.Length; i++)
        {
            peelableParts[i].SetPeelableEditor(i);
        }
    }
#endif

    public void LoadPeelables(List<PeelableData> PeelableDatas)
    {
        for (int i = 0; i < peelableParts.Length; i++)
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
}
