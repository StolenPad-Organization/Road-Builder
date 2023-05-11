using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SellManager : MonoBehaviour
{
    public static SellManager instance;

    [SerializeField] private Transform sellPoint;
    [SerializeField] private bool selling;
    [SerializeField] private float sellRate;
    [SerializeField] private float nextSell;
    [SerializeField] MoneySpot[] moneySpots;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
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
}
