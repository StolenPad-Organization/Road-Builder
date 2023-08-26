using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Money : MonoBehaviour
{
    [SerializeField] private Collider moneyCollider;
    [SerializeField] private int value;
    private MoneySpot target;
    [SerializeField] private ParticleSystem moneyPickingVfx;

    void Start()
    {
        
    }

    public void Spawn(int price)
    {
        value = price;
        target = GameManager.instance.currentZone.sellManager.GetMoneySpot();
        if (target == null) return;
        target.used = true;
        moneyCollider.enabled = false;
        transform.DOJump(target.transform.position, 1, 1, 0.4f).OnComplete(() =>
        {
            moneyCollider.enabled = true;
        });
        GameManager.instance.currentZone.AddMoneyData(target.index, value);
    }

    private void Collect()
    {
        if (PlayerController.instance.canDoStrictedHaptic)
        {
            EventManager.invokeHaptic.Invoke(vibrationTypes.LightImpact);
            PlayerController.instance.canDoStrictedHaptic = false;
        }
        target.used = false;
        EventManager.AddMoney.Invoke(value, transform);
        MoneyPooler.instance.ReturnMoney(this);
        //UIManager.instance.UpdateMoney(value);
        GameManager.instance.currentZone.RemoveMoneyData(target.index, value);
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        moneyPickingVfx.transform.SetParent(null);
        moneyPickingVfx.transform.position = transform.position + (Vector3.up * 0.25f);
        moneyPickingVfx.gameObject.SetActive(true);
        moneyPickingVfx.Play();

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
