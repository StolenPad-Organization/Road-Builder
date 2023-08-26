using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellManager : MonoBehaviour
{
    [SerializeField] private Transform sellPoint;
    [SerializeField] private bool selling;
    [SerializeField] private float sellRate;
    private float currentSellRate;
    private int sellCount;
    [SerializeField] private float nextSell;
    [SerializeField] MoneySpot[] moneySpots;

    void Start()
    {
        for (int i = 0; i < moneySpots.Length; i++)
        {
            moneySpots[i].index = i;
        }
    }

    void Update()
    {
        if (selling)
        {
            if(nextSell <= 0)
            {
                PlayerController.instance.SellCollectable(sellPoint);
                sellCount++;
                nextSell = currentSellRate - (sellCount * 0.005f);
                if (nextSell < 0.01f) nextSell = 0.01f;
            }
            else
            {
                nextSell -= Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        currentSellRate = sellRate;
        selling = true;
        nextSell = 0;
        PlayerController.instance.ActivateUpgradeCamera(true);
    }
    private void OnTriggerExit(Collider other)
    {
        selling = false;
        PlayerController.instance.ActivateUpgradeCamera(false);
    }

    public MoneySpot GetMoneySpot()
    {
        for (int i = 0; i < moneySpots.Length; i++)
        {
            if (!moneySpots[i].used)
            {
                return moneySpots[i];
            }
        }
        return moneySpots[Random.Range(0,moneySpots.Length)];
    }

    public void LoadMoeny(List<MoneyData> moneyDatas)
    {
        Money money;
        for (int i = 0; i < moneyDatas.Count; i++)
        {
            money = MoneyPooler.instance.GetMoney();
            money.LoadMoeny(moneyDatas[i].Price, moneySpots[moneyDatas[i].Index]);
        }
    }
}
