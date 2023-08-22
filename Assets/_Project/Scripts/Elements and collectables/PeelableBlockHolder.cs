using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeelableBlockHolder : MonoBehaviour
{
    [SerializeField] private List<Peelable> peelableParts = new List<Peelable>();
    [SerializeField] private List<RBHandler> rbHandlers = new List<RBHandler>();
    void Start()
    {
        
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
        foreach (var item in rbHandlers)
        {
            item.CheckSwitch(state);
            if (state)
                RBManager.Instance.AddAgent(item);
            else if (hide)
            {
                item.CheckSwitch(!state);
                item.gameObject.SetActive(false);
            }
        }
    }
}
