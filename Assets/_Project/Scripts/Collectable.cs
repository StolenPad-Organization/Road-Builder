using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Collectable : MonoBehaviour
{
    public CollectableType collectableType;
    [SerializeField] private Collider collectableCollider;
    void Start()
    {
        
    }

    private void OnEnable()
    {
        Spawn();
    }

    private void Spawn()
    {
        collectableCollider.enabled = false;
        transform.DOJump(transform.position, 1, 1, 0.4f).OnComplete(() =>
        {
            collectableCollider.enabled = true;
        });
    }

    public void Collect(int index, float collectableOffest, Transform collectableParent)
    {
        collectableCollider.enabled = false;
        transform.SetParent(collectableParent);
        transform.DOLocalJump(Vector3.up * index * collectableOffest, collectableOffest * index * 3, 1, 0.4f);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController.instance.OnCollect(this);
    }

    public void Sell(Transform sellPoint)
    {
        transform.SetParent(sellPoint);
        transform.DOLocalJump(Vector3.zero, 3, 1, 0.6f).OnComplete(() => 
        {
            CollectablesPooler.Instance.ReturnCollectable(this);
            Money money = MoneyPooler.instance.GetMoney();
            money.transform.position = SellManager.instance.transform.position;
            money.Spawn();
        });
    }
}
