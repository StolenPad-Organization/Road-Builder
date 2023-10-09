using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UltimateJoystickExample.Spaceship;
using Unity.VisualScripting;

public class AISellPoint : MonoBehaviour
{
    public int MaxCollectables;
    public int Collectables;
    public List<Peelable> Peelables;
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI TextCount;
    [SerializeField] private GameObject FullText;

    [SerializeField] private float sellRate;
    private float currentSellRate;
    private int sellCount;
    [SerializeField] private float nextSell;
    public Action OnSellPointAvailble;
    public bool IsFull;
    private void OnValidate()
    {
        TextCount.text = Collectables + "/" + MaxCollectables;
    }
    public bool CheckCanGetPeelable()
    {
        return MaxCollectables > Collectables;
    }
    public void AddCollectable(Peelable peelable)
    {
        Collectables++;
        Peelables.Add(peelable);
        TextCount.text = Collectables + "/" + MaxCollectables;
        if(MaxCollectables <= Collectables)
        {
            TextCount.transform.parent.gameObject.SetActive(false);
            FullText.SetActive(true);   
            IsFull = true;
        }
        peelable.transform.localPosition = Vector3.zero;
        peelable.peelableCopy.transform.localPosition = Vector3.zero;
    }
    public void GetCollectablesFromPoint()
    {
        if (nextSell <= 0)
        {
            if (Peelables.Count <= 0) return;

            GameManager.instance.player.OnCollect(Peelables[0]);
            Peelables.RemoveAt(0);

            Collectables--;
            TextCount.text = Collectables + "/" + MaxCollectables;

            if (IsFull)
            {
                if (MaxCollectables > Collectables)
                {
                    TextCount.transform.parent.gameObject.SetActive(true);
                    FullText.SetActive(false);
                    OnSellPointAvailble?.Invoke();
                    IsFull = false;
                }
            }
           

            sellCount++;
            nextSell = currentSellRate - (sellCount * 0.005f);
            if (nextSell < 0.01f) nextSell = 0.01f;
        }
        else
        {
            nextSell -= Time.deltaTime;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            GameManager.instance.player.ActivateUpgradeCamera(true);

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetCollectablesFromPoint();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        GameManager.instance.player.ActivateUpgradeCamera(false);
    }


}
