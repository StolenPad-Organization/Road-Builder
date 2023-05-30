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

    public void Spawn(int price)
    {
        value = price;
        target = SellManager.instance.GetMoneySpot();
        if (target == null) return;
        target.used = true;
        moneyCollider.enabled = false;
        transform.DOJump(target.transform.position, 1, 1, 0.4f).OnComplete(() =>
        {
            moneyCollider.enabled = true;
        });
        GameManager.instance.currentStage.currentZone.AddMoneyData(target.index, value);
    }

    private void Collect()
    {
        target.used = false;
        MoneyPooler.instance.ReturnMoney(this);
        UIManager.instance.UpdateMoney(value);
        GameManager.instance.currentStage.currentZone.RemoveMoneyData(target.index, value);
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Collect();
    }

    public void LoadMoeny(int price, MoneySpot moneySpot)
    {
        value = price;
        target = moneySpot;
        target.used = true;
        transform.position = target.transform.position;
    }
}
