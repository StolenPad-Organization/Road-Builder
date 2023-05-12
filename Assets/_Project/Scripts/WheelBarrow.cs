using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WheelBarrow : MonoBehaviour
{
    public List<Collectable> collectables;
    public float collectablesLimit;
    [SerializeField] private float collectableOffest;
    [SerializeField] private Transform collectableParent;
    [SerializeField] private bool used;
    [SerializeField] private NavMeshAgent agent;
    private Transform playerTransform;

    void Start()
    {
        playerTransform = PlayerController.instance.wheelBarrowFollowTransform;
    }

    void Update()
    {
        if (used)
        {
            agent.SetDestination(playerTransform.position);
        }
    }

    public void OnCollect(Collectable collectable)
    {
        if (collectables.Count >= collectablesLimit) return;
        collectable.Collect(collectables.Count, collectableOffest, collectableParent);
        collectables.Add(collectable);
    }
    public void SellCollectable(Transform sellPoint)
    {
        if (collectables.Count == 0) return;
        Collectable collectable = collectables[collectables.Count - 1];
        collectables.Remove(collectable);
        collectable.Sell(sellPoint);
    }

    private void OnTriggerEnter(Collider other)
    {
        used = true;
        PlayerController.instance.ActivateWheelBarrow(this);
    }
}
