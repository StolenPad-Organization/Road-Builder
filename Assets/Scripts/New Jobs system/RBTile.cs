using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RBTile : GridElement
{
    public List<RBHandler> rbPositions = new List<RBHandler>();
    public List<RBTile> adjustantTiles = new List<RBTile>();
    public float3 MinPosition;
    public float3 MaxPosition;  
    private void Start()
    {
        foreach (RBHandler rbHandler in rbPositions)
        {
            rbHandler.CheckSwitch(false);
        }
    }
    [ContextMenu("Debug bounds")]
    public void DebugBounds()
    {
        BoxCollider b = GetComponent<BoxCollider>();

        // get min
        var min = b.bounds.min;
        var max = b.bounds.max; 
        var center = b.center;

        //AI min position 
        MinPosition = float3.zero;
            
        MinPosition.z =-(b.bounds.size.z/2);
        MinPosition.x = center.x;

        MaxPosition = float3.zero;

        MaxPosition.z =(b.bounds.size.z/2);
        MaxPosition.x = center.x;

        MaxPosition += (float3)transform.position;
        MinPosition += (float3)transform.position;


        //Debug.Log("Minposition : " + MinPosition);
        //Debug.Log("Maxposition : " + MaxPosition);
    }   
    public void RemoveHandler(RBHandler handler)
    {
        rbPositions.Remove(handler);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("AI"))
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(MinPosition, 1f);
        Gizmos.color = Color.black;

        Gizmos.DrawWireSphere(MaxPosition, 1f);
    }
#endif
}
