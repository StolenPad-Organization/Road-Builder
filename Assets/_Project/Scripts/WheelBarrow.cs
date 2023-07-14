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
    Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        agent.updatePosition = false;
    }

    void Start()
    {
        playerTransform = PlayerController.instance.wheelBarrowFollowTransform;
    }

    void FixedUpdate()
    {
        if (used)
        {
            agent.SetDestination(playerTransform.position);
            transform.position = Vector3.SmoothDamp(transform.position, agent.nextPosition, ref velocity, 0.1f);
        }
    }

    public void OnCollect(Collectable collectable)
    {
        if (collectables.Count >= collectablesLimit) return;
        collectable.Collect(collectables.Count, collectableOffest, collectableParent);
        collectables.Add(collectable);
        GameManager.instance.currentZone.AddCollectableData(false, collectable.collectableType, collectable.peelable);
    }
    public void SellCollectable(Transform sellPoint)
    {
        if (collectables.Count == 0) return;
        Collectable collectable = collectables[collectables.Count - 1];
        collectables.Remove(collectable);
        collectable.Sell(sellPoint);
        GameManager.instance.currentZone.RemoveCollectableData(false, collectable.collectableType, collectable.peelable);
    }

    private void OnTriggerEnter(Collider other)
    {
        ActivateWheelBarrow();
    }

    public void ActivateWheelBarrow()
    {
        used = true;
        PlayerController.instance.ActivateWheelBarrow(this);
    }

    public void LoadCollectables(List<CollectableData> collectableDatas)
    {
        Collectable collectable;
        for (int i = 0; i < collectableDatas.Count; i++)
        {
            collectable = CollectablesPooler.Instance.GetCollectable(collectableDatas[i].CollectableType, Vector3.down * 10);
            collectable.LoadCollectable(collectables.Count, collectableOffest, collectableParent, collectableDatas[i].Peelable);
            collectables.Add(collectable);
        }
    }
}
