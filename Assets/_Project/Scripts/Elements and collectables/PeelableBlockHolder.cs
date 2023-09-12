using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PeelableBlockHolder : MonoBehaviour
{
    public List<Peelable> peelableParts = new List<Peelable>();
    private int partsCount;
    [SerializeField] private GameObject blockCollider;
    [SerializeField] private GameObject blockTrigger;
    void Start()
    {
        //partsCount = peelableParts.Count;
    }

    public void AddPeelablePart(Peelable peelable)
    {
        peelableParts.Add(peelable);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
    }

    private void ActivateBlock(bool activate)
    {
        if (activate)
        {
            if(blockCollider !=null)
                blockCollider.SetActive(true);
            if (blockTrigger != null)
                blockTrigger.SetActive(true);
        }
        else
        {
            if (blockCollider != null)
                blockCollider.SetActive(false);
            if (blockTrigger != null)
                blockTrigger.SetActive(false);
        }
    }

    public void SetRBHandlersState(bool state, bool hide = false)
    {
        for (int i = 0; i < peelableParts.Count; i++)
        {
            if (hide)
            {
                peelableParts[i].rbHandler.CheckSwitch(false);
                if (!peelableParts[i].sold && !peelableParts[i].collected)
                {
                    peelableParts[i].peelableCopy.ActivateCollision();
                    continue;
                }
            }
            
            if (peelableParts[i].sold)
            {
                peelableParts[i].rbHandler.CheckSwitch(true);
                peelableParts[i].rbHandler.gameObject.SetActive(false);
            }
            else
            {
                if (peelableParts[i].collected) continue;
                peelableParts[i].rbHandler.CheckSwitch(state);
                if (state)
                {
                    RBManager.Instance.AddAgent(peelableParts[i].rbHandler);
                }
            }
        }
        if (!hide)
            ActivateBlock(!state);
        else
            ActivateBlock(false);
    }

    private void GetPeelableRest()
    {
        RBManager.Instance.Clear();
        var parts = peelableParts.Where(t => !t.sold && !t.collected);
        parts.Select(t => t.rbHandler).ToList().ForEach(t => t.CheckSwitch(false));
        parts.Select(t => t.peelableCopy).ToList().ForEach(t => t.ActivateCollision());
    }

    public void CheckCountLoad()
    {
        for (int i = 0; i < peelableParts.Count; i++)
        {
            if (peelableParts[i].peeled || peelableParts[i].collected || peelableParts[i].sold)
                partsCount--;
        }
    }

    public void RemovePart()
    {
        partsCount--;
    }

    public bool CheckCount()
    {
        return partsCount <= 0;
    }

    public void SetPartsCount(float percentage)
    {
        partsCount = Mathf.RoundToInt(peelableParts.Count * (percentage / 100));
    }
}
