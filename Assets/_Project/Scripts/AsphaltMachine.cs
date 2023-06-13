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
    [SerializeField] private Transform scalingObject;
    [SerializeField] private float scalingRate;
    [SerializeField] private Animator anim;

    void Start()
    {
        asphaltCapacity = asphaltObjects.Length;
        OnSpawn();
    }

    void Update()
    {
        if (used)
        {
            if (!anim.GetBool("Run") && PlayerController.instance.MovementCheck())
            {
                anim.SetBool("Run", true);
            }

            if (anim.GetBool("Run") && !PlayerController.instance.MovementCheck())
            {
                anim.SetBool("Run", false);
            }
        }
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
        if (scalingObject == null)
            asphaltObjects[asphaltCount].SetActive(true);
        else
            SetObjectScale();
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
            if (scalingObject == null)
                asphaltObjects[asphaltCount].SetActive(false);
            else
                SetObjectScale();
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

    private void SetObjectScale()
    {
        scalingObject.localScale = Vector3.one + (new Vector3(0,1,1) * scalingRate * asphaltCount);
    }

    public void OnSpawn()
    {
        used = true;
        anim.SetBool("Run", true);
        transform.DOMove(GameManager.instance.currentZone.machinesPosition.position, 2.0f).OnComplete(() => 
        {
            used = false;
            PlayerController.instance.arrowController.PointToObject(gameObject);
            anim.SetBool("Run", false);
        });
    }
}
