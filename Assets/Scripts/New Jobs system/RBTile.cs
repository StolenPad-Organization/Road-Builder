using System.Collections.Generic;
using UnityEngine;

public class RBTile : GridElement
{
    public List<RBHandler> rbPositions = new List<RBHandler>();
    public List<RBTile> adjustantTiles = new List<RBTile>();

    private void Start()
    {
        foreach (RBHandler rbHandler in rbPositions)
        {
            rbHandler.CheckSwitch(false);
        }
    }
    public void RemoveHandler(RBHandler handler)
    {
        rbPositions.Remove(handler);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RBManagerJobs.Instance.SubscribeNewTile(adjustantTiles);
        }
        if (other.CompareTag("parts"))
        {
            var rbh = other.GetComponent<Peelable>();
            if(!rbh.collected && !rbh.sold)
                rbh.rbHandler.SwitchTile(this);
        }

    }
#if UNITY_EDITOR
    public void AddRbHandler(RBHandler handler)
    {
        rbPositions.Add(handler);
    }
    public void MoveUp()
    {
        foreach (RBHandler handler in rbPositions)
        {
            handler.transform.position = handler.transform.position + Vector3.up * 3f;  // Move 1 unit up, you can customize this

        }
    }

    public void ResetPosition()
    {
        foreach (RBHandler handler in rbPositions)
        {
            handler.transform.position = handler.transform.position - Vector3.up * 3f;  // Move 1 unit up, you can customize this

        }
    }
#endif
}
