using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyPooler : MonoBehaviour
{
    public static MoneyPooler instance;

    [SerializeField] private Money moneyPrefab;
    [SerializeField] private int poolSize = 10;

    private Queue<Money> moneyPool = new Queue<Money>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            Money money = Instantiate(moneyPrefab);
            money.gameObject.SetActive(false);
            moneyPool.Enqueue(money);
        }
    }

    public Money GetMoney()
    {
        if (moneyPool.Count == 0)
        {
            Money money = Instantiate(moneyPrefab);
            money.gameObject.SetActive(false);
            moneyPool.Enqueue(money);
        }

        Money pooledmoney = moneyPool.Dequeue();
        pooledmoney.gameObject.SetActive(true);
        return pooledmoney;
    }

    public void ReturnMoney(Money money)
    {
        money.transform.position = transform.position;
        money.transform.rotation = transform.rotation;

        money.gameObject.SetActive(false);
        moneyPool.Enqueue(money);
    }
}
