using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellManager : MonoBehaviour
{
    [SerializeField] private Transform sellPoint;
    [SerializeField] private bool selling;
    [SerializeField] private float sellRate;
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
                nextSell = sellRate;
            }
            else
            {
                nextSell -= Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        selling = true;
        nextSell = 0;
    }
    private void OnTriggerExit(Collider other)
    {
        selling = false;
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
        return null;
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
