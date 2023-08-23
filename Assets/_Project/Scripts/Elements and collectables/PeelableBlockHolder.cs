using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeelableBlockHolder : MonoBehaviour
{
    [SerializeField] private List<Peelable> peelableParts = new List<Peelable>();
    private int partsCount;
    [SerializeField] private GameObject blockCollider;
    [SerializeField] private GameObject blockTrigger;
    void Start()
    {
        partsCount = peelableParts.Count;
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
                peelableParts[i].rbHandler.CheckSwitch(true);
                peelableParts[i].rbHandler.gameObject.SetActive(false);
                continue;
            }
            if (peelableParts[i].collected)
            {
                peelableParts[i].rbHandler.CheckSwitch(true);
                continue;
            }
            if (peelableParts[i].sold)
            {
                peelableParts[i].rbHandler.CheckSwitch(true);
                peelableParts[i].rbHandler.gameObject.SetActive(false);
            }
            else
            {
                peelableParts[i].rbHandler.CheckSwitch(state);
                if (state)
                {
                    RBManager.Instance.AddAgent(peelableParts[i].rbHandler);
                }
            }
        }
        if(!hide)
            ActivateBlock(!state);
        else
            ActivateBlock(false);
    }

    public void CheckCountLoad()
    {
        for (int i = 0; i < peelableParts.Count; i++)
        {
            if (peelableParts[i].peeled || peelableParts[i].collected || peelableParts[i].sold)
                partsCount--;
        }
    }

    public bool CheckCount()
    {
        partsCount--;
        return partsCount <= 0;
    }
}
