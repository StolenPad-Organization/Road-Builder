using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Mathematics;

public class BuildMachine : MonoBehaviour
{
    public BuildMachineUpgradeType machineUpgradeType;
    [SerializeField] private Transform playerSeat;
    public Transform partsSpawnPoint;
    public Transform[] partsSpawnPoints;
    [SerializeField] private GameObject playerTrigger;
    [SerializeField] private GameObject partsTrigger;
    [SerializeField] private bool used;
    public int asphaltCount;
    [SerializeField] private int asphaltCapacity;
    [SerializeField] private GameObject[] asphaltObjects;
    [SerializeField] private GameObject asphaltScalingObject;
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
    public bool hasUpgrade;
    private Vector3[] ammoInitialScales;
    public float Speed;
    [Header("Upgrades")]
    [SerializeField] private float[] speedUpgrades;
    [SerializeField] private Vector3[] CollisionUpgrades;
    [SerializeField] private int[] consumeRateUpgrade;

    [Header("Machine Icon")]
    [SerializeField] private MachineIconController machineIcon;

    void Start()
    {
        asphaltCapacity = asphaltObjects.Length;
        //SetUpgrade(upgradeIndex);
        //OnSpawn();
        ammoInitialScales = new Vector3[asphaltObjects.Length];
        for (int i = 0; i < asphaltObjects.Length; i++)
        {
            ammoInitialScales[i] = asphaltObjects[i].transform.localScale;
        }
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
        if (!used && other.CompareTag("Player"))
        {
            PlayerController.instance.GetOnAsphaltMachine(playerSeat, this);
            used = true;
            playerTrigger.SetActive(false);
            partsTrigger.SetActive(true);
            if (machineIcon != null)
                machineIcon.Fade();
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
        {
            if(asphaltScalingObject != null)
            {
                asphaltScalingObject.transform.localScale = calculateScale();
            }
            else
            {
                asphaltObjects[asphaltCount].SetActive(true);
                asphaltObjects[asphaltCount].transform.DOKill();
                asphaltObjects[asphaltCount].transform.localScale = Vector3.zero;
                asphaltObjects[asphaltCount].transform.DOScale(ammoInitialScales[asphaltCount], 0.15f);
            }
        }
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
        if (consumeRate <= consumeValue)
        {
            consumeValue = 0;
            asphaltCount--;
            if (scalingObject == null)
            {
                if (asphaltScalingObject != null)
                    asphaltScalingObject.transform.localScale = calculateScale();
                else
                    asphaltObjects[asphaltCount].transform.DOScale(0, 0.5f);
            }
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

    private Vector3 calculateScale()
    {
        if(hasUpgrade)
            return Vector3.Lerp(Vector3.zero, Vector3.one * 1.65f, Mathf.InverseLerp(0, asphaltObjects.Length, asphaltCount));
        else
            return Vector3.Lerp(Vector3.zero, Vector3.one * 0.95f, Mathf.InverseLerp(0.0f, asphaltObjects.Length, asphaltCount));
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
        if(machineIcon != null)
            machineIcon.OnSpawn();
    }

    public void SetUpgrade(int index, int level)
    {
        if (!hasUpgrade) return;
        buildMachineUpgrades[upgradeIndex].gameObject.SetActive(false);
        if (index > 0)
            anim.SetLayerWeight(index, 0);
        upgradeIndex = index;
        buildMachineUpgrades[upgradeIndex].gameObject.SetActive(true);
        playerSeat = buildMachineUpgrades[upgradeIndex].playerSeat;
        anim.SetLayerWeight(index, 1);
        if (scalingObject != null)
        {
            scalingObject = buildMachineUpgrades[upgradeIndex].scalingObject;
            scalingObjectHolder = buildMachineUpgrades[upgradeIndex].scalingObjectHolder;
            SetObjectScale();
        }
        else
        {
            // set next upgrade asphalt objects
        }
        if (used)
            PlayerController.instance.GetOnAsphaltMachine(playerSeat, this);

        Speed = speedUpgrades[level-1];
        partsTrigger.transform.localScale = CollisionUpgrades[level-1];
        consumeRate = consumeRateUpgrade[level-1];
    }

    public Transform GetNearestSpawnPoint(Vector3 pos)
    {
        Transform target = null;
        if (partsSpawnPoints.Length < 0) return null;
        target = partsSpawnPoints[0];
        float closestDistance = math.distance(target.transform.position, pos);
        for (int i = 1; i < partsSpawnPoints.Length; i++)
        {
            var tmpDistance = math.distance(partsSpawnPoints[i].position, pos);
            if (tmpDistance < closestDistance)
            {
                target = partsSpawnPoints[i];
                closestDistance = tmpDistance;
            }
        }
        return target;
    }
}
