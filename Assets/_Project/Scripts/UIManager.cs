using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [SerializeField] private TextMeshProUGUI moneyText;
    private int money;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateMoney(0);
    }

    void Update()
    {
        
    }

    public void UpdateMoney(int amount)
    {
        money += amount;
        moneyText.text = money.ToString();
    }
}
