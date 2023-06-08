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
    [SerializeField] private GameObject fullWarning;
    [SerializeField] private GameObject emptyWarning;
    public bool drivable;

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
        if (asphaltCount >= asphaltCapacity)
        {
            if (!fullWarning.activeInHierarchy)
            {
                fullWarning.SetActive(true);
            }
            return;
        }
        asphaltObjects[asphaltCount].SetActive(true);
        asphaltCount++;
        if (emptyWarning.activeInHierarchy)
        {
            emptyWarning.SetActive(false);
        }
    }

    public bool UseAsphalt()
    {
        if (asphaltCount <= 0)
        {
            if (!emptyWarning.activeInHierarchy)
            {
                emptyWarning.SetActive(true);
            }
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
        if (fullWarning.activeInHierarchy)
        {
            fullWarning.SetActive(false);
        }
        return true;
    }

    public void OnSpawn()
    {
        used = true;
        transform.DOMove(GameManager.instance.currentZone.machinesPosition.position, 2.0f).OnComplete(() => 
        {
            used = false;
            PlayerController.instance.arrowController.PointToObject(gameObject);
        });
    }
}
