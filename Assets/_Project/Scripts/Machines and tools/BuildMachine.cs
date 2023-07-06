using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BuildMachine : MonoBehaviour
{
    public MachineUpgradeType machineUpgradeType;
    [SerializeField] private Transform playerSeat;
    public Transform partsSpawnPoint;
    [SerializeField] private GameObject playerTrigger;
    [SerializeField] private GameObject partsTrigger;
    [SerializeField] private bool used;
    public int asphaltCount;
    [SerializeField] private int asphaltCapacity;
    [SerializeField] private GameObject[] asphaltObjects;
    [SerializeField] private int consumeRate;
    [SerializeField] private int consumeValue;
    [SerializeField] private GameObject fullWarning;
    [SerializeField] private GameObject emptyWarning;
    public bool drivable;
    [SerializeField] private Transform scalingObject;
    [SerializeField] private float scalingRate;
    [SerializeField] private float minScale;
    [SerializeField] private float maxScale;
    [SerializeField] private Transform scalingObjectHolder;
    [SerializeField] private Vector2 movingYLimits;
    [SerializeField] private Animator anim;
    [SerializeField] private BuildMachineUpgrade[] buildMachineUpgrades;
    [SerializeField] private int upgradeIndex;
    [SerializeField] private bool hasUpgrade;

    void Start()
    {
        asphaltCapacity = asphaltObjects.Length;
        //SetUpgrade(upgradeIndex);
        //OnSpawn();
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
        if (Input.GetKeyDown(KeyCode.C))
        {
            SetUpgrade(Random.Range(0,3));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!used && other.CompareTag("Player"))
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
        scalingObject.localScale = new Vector3(1, minScale, minScale) + (new Vector3(0,1,1) * scalingRate * asphaltCount);
        float t = Mathf.InverseLerp(minScale, maxScale, scalingObject.localScale.y);
        Vector3 newPos = scalingObjectHolder.transform.localPosition;
        newPos.y = Mathf.Lerp(movingYLimits.x, movingYLimits.y, t);
        scalingObjectHolder.transform.localPosition = newPos;
    }

    public void OnSpawn()
    {
        playerTrigger.SetActive(true);

        //used = true;
        //anim.SetBool("Run", true);
        //transform.DOMove(GameManager.instance.currentZone.machinesPosition.position, 2.0f).OnComplete(() => 
        //{
            used = false;
            PlayerController.instance.arrowController.PointToObject(gameObject);
        //    anim.SetBool("Run", false);
        //});
    }

    public void SetUpgrade(int index)
    {
        if (!hasUpgrade) return;
        buildMachineUpgrades[upgradeIndex].gameObject.SetActive(false);
        if (index > 0)
            anim.SetLayerWeight(index, 0);
        upgradeIndex = index;
        buildMachineUpgrades[upgradeIndex].gameObject.SetActive(true);
        playerSeat = buildMachineUpgrades[upgradeIndex].playerSeat;
        consumeRate = buildMachineUpgrades[upgradeIndex].consumeRate;
        anim.SetLayerWeight(index, 1);
        if (scalingObject != null)
        {
            scalingObject = buildMachineUpgrades[upgradeIndex].scalingObject;
            scalingObjectHolder = buildMachineUpgrades[upgradeIndex].scalingObjectHolder;
            SetObjectScale();
        }
        if (used)
            PlayerController.instance.GetOnAsphaltMachine(playerSeat, this);
    }
}
