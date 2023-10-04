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
    public float rotationSpeed;

    [Header("Upgrades")]
    [SerializeField] private float[] speedUpgrades;
    [SerializeField] private Vector3[] CollisionUpgrades;
    [SerializeField] private int[] consumeRateUpgrade;

    [Header("Machine Icon")]
    [SerializeField] private MachineIconController machineIcon;

    void Start()
    {
        //SetUpgrade(upgradeIndex);
        //OnSpawn();

        initAmmo();
    }

    private void initAmmo()
    {
        if(asphaltObjects.Length != 0)
        {
            for (int i = 0; i < asphaltObjects.Length; i++)
            {
                if (asphaltObjects[i].transform.localScale == Vector3.zero) return;
            }
        }

        asphaltCapacity = asphaltObjects.Length;
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
            if (!anim.GetBool("Run") && GameManager.instance.player.MovementCheck())
            {
                anim.SetBool("Run", true);
            }

            if (anim.GetBool("Run") && !GameManager.instance.player.MovementCheck())
            {
                anim.SetBool("Run", false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!used && other.CompareTag("Player"))
        {
            GameManager.instance.player.GetOnAsphaltMachine(playerSeat, this);
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
                GameManager.instance.player.arrowController.GetNewTarget();
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
            GameManager.instance.player.arrowController.PointToObject(gameObject);
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
            if (buildMachineUpgrades[upgradeIndex].asphaltObjects.Length > 0)
            {
                asphaltObjects = buildMachineUpgrades[upgradeIndex].asphaltObjects;
                initAmmo();
                if (asphaltCount > asphaltObjects.Length)
                    asphaltCount = asphaltObjects.Length;
                for (int i = 0; i < asphaltCount; i++)
                {
                    asphaltObjects[i].SetActive(true);
                    asphaltObjects[i].transform.DOKill();
                    asphaltObjects[i].transform.localScale = ammoInitialScales[i];
                }
            }
        }

        if(buildMachineUpgrades[upgradeIndex].partsSpawnPoints.Length > 0)
            partsSpawnPoints = buildMachineUpgrades[upgradeIndex].partsSpawnPoints;

        if (used)
            GameManager.instance.player.GetOnAsphaltMachine(playerSeat, this);

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
