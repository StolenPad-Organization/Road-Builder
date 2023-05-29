using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AsphaltMachine : MonoBehaviour
{
    [SerializeField] private Transform playerSeat;
    public Transform partsSpawnPoint;
    [SerializeField] private GameObject playerTrigger;
    [SerializeField] private GameObject partsTrigger;
    private bool used;
    [SerializeField] private int asphaltCount;
    [SerializeField] private int asphaltCapacity;
    [SerializeField] private GameObject[] asphaltObjects;
    [SerializeField] private int consumeRate;
    [SerializeField] private int consumeValue;

    void Start()
    {
        asphaltCapacity = asphaltObjects.Length;
        OnSpawn();
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!used)
        {
            PlayerController.instance.GetOnAsphaltMachine(playerSeat, this);
            used = true;
            playerTrigger.SetActive(false);
            partsTrigger.SetActive(true);
        }
    }

    public void FillAsphalt()
    {
        if (asphaltCount >= asphaltCapacity) return;
        asphaltObjects[asphaltCount].SetActive(true);
        asphaltCount++;
    }

    public bool UseAsphalt()
    {
        if (asphaltCount <= 0)
        { 
            return false; 
        }
        if (consumeRate == consumeValue)
        {
            consumeValue = 0;
            asphaltCount--;
            asphaltObjects[asphaltCount].SetActive(false);
        }
        else
        {
            consumeValue++;
        }    
        return true;
    }

    public void OnSpawn()
    {
        used = true;
        transform.DOMove(Vector3.zero, 2.0f).OnComplete(() => 
        {
            used = false;
            PlayerController.instance.arrowController.PointToObject(gameObject);
        });
    }
}
