using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Money : MonoBehaviour
{
    [SerializeField] private Collider moneyCollider;
    [SerializeField] private int value;
    private MoneySpot target;
    void Start()
    {
        
    }

    public void Spawn()
    {
        target = SellManager.instance.GetMoneySpot();
        if (target == null) return;
        target.used = true;
        moneyCollider.enabled = false;
        transform.DOJump(target.transform.position, 1, 1, 0.4f).OnComplete(() =>
        {
            moneyCollider.enabled = true;
        });
    }

    private void Collect()
    {
        target.used = false;
        MoneyPooler.instance.ReturnMoney(this);
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Collect();
    }
}
