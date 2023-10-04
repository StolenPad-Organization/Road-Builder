using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WheelBarrow : MonoBehaviour
{
    public List<Peelable> collectables;
    public float collectablesLimit;
    [SerializeField] private float collectableOffest;
    [SerializeField] private float angleCollectableOffest;
    [SerializeField] private Transform collectableParent;
    [SerializeField] private bool used;
    [SerializeField] private NavMeshAgent agent;
    private Transform playerTransform;
    Vector3 velocity = Vector3.zero;
    public PlayerController player;

    private void Awake()
    {
        agent.updatePosition = false;
    }

    void Start()
    {
        playerTransform = GameManager.instance.player.wheelBarrowFollowTransform;
    }

    void FixedUpdate()
    {
        if (used)
        {
            agent.SetDestination(playerTransform.position);
            transform.position = Vector3.SmoothDamp(transform.position, agent.nextPosition, ref velocity, 0.1f);
        }
    }

    public void OnCollect(Peelable collectable)
    {
        if (collectables.Count >= collectablesLimit) return;
        if(GameManager.instance.player.scrapeTool.toolAngleController != null)
            collectable.Collect(collectables.Count, angleCollectableOffest, collectableParent, player);
        else
            collectable.Collect(collectables.Count, collectableOffest, collectableParent, player);
        collectables.Add(collectable);
        GameManager.instance.currentZone.AddCollectableData(false, collectable.index);
    }
    public void SellCollectable(Transform sellPoint)
    {
        if (collectables.Count == 0) return;
        Peelable collectable = collectables[collectables.Count - 1];
        collectables.Remove(collectable);
        collectable.Sell(sellPoint);
        GameManager.instance.currentZone.RemoveCollectableData(false, collectable.index);
    }

    private void OnTriggerEnter(Collider other)
    {
        ActivateWheelBarrow(other.GetComponent<PlayerController>());
    }

    public void ActivateWheelBarrow(PlayerController _player)
    {
        used = true;
        player = _player;
        player.ActivateWheelBarrow(this);
    }

    public void LoadCollectables(List<CollectableData> collectableDatas)
    {
        Peelable collectable;
        for (int i = 0; i < collectableDatas.Count; i++)
        {
            collectable = GameManager.instance.currentZone.peelableManager.ReturnPeelableWithIndex(collectableDatas[i].index);
            if (GameManager.instance.player.scrapeTool.toolAngleController != null)
                collectable.LoadCollectable(collectables.Count, angleCollectableOffest, collectableParent);
            else
                collectable.LoadCollectable(collectables.Count, collectableOffest, collectableParent);
            collectables.Add(collectable);
        }
    }
}
