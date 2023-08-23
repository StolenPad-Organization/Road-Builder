using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeelableBlockHolder : MonoBehaviour
{
    [SerializeField] private List<Peelable> peelableParts = new List<Peelable>();
    [SerializeField] private List<RBHandler> rbHandlers = new List<RBHandler>();
    private int partsCount;
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

    public void AddRBHandlers()
    {
        foreach (var item in peelableParts)
        {
            rbHandlers.Add(item.GetComponent<RBHandler>());
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
    }

    public void SetRBHandlersState(bool state, bool hide = false)
    {
        for (int i = 0; i < rbHandlers.Count; i++)
        {
            if (peelableParts[i].collected)
            {
                rbHandlers[i].CheckSwitch(!state);
                return;
            }
            rbHandlers[i].CheckSwitch(state);
            if (state)
                RBManager.Instance.AddAgent(rbHandlers[i]);
            else if (hide || peelableParts[i].sold)
            {
                rbHandlers[i].CheckSwitch(!state);
                rbHandlers[i].gameObject.SetActive(false);
            }
        }
    }

    public bool CheckCount()
    {
        partsCount--;
        return partsCount <= 0;
    }
}
