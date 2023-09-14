using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public PlayerMovementController movementController;
    [SerializeField] private List<Peelable> collectables;
    [SerializeField] private float collectablesLimit;
    [SerializeField] private float collectableOffest;
    [SerializeField] private float angleCollectableOffest;
    [SerializeField] private Transform collectableParent;
    [SerializeField] private Vector3 collectableParentPos;
    [SerializeField] private Vector3 collectableParentAnglePos;
    [SerializeField] private GameObject model;
    public GameObject fullWarning;
    public BuildMachine asphaltMachine;
    public PaintMachine paintMachine;
    public WheelBarrow wheelBarrow;
    public Transform wheelBarrowFollowTransform;
    public ArrowController arrowController;
    [SerializeField] private ParticleSystem paintFootprintsEffect;
    [SerializeField] private float footPrintsDuration;
    [SerializeField] private ParticleSystem waterPodsEffect;
    [SerializeField] private ParticleSystem paintPodsEffect;

    [Header("Scrape Tools")]
    public ScrapeTool scrapeTool;
    [SerializeField] private ScrapeTool[] scrapeToolsPrefabs;
    [SerializeField] private ScrapeTool[] normalScrapeToolsPrefabs;
    [SerializeField] private ScrapeTool[] angleScrapeToolsPrefabs;
    public int scrapeToolIndex;
    public Transform scrapeToolHolder;
    float lastToolUsingTime;
    [SerializeField] private float toolCoolDown;
    public Transform hidePos;
    public Transform showPos;
    [SerializeField] private ParticleSystem dustVFX;
    [SerializeField] private ParticleSystem dustTrailVFX;
    [SerializeField] private Transform rightCheck;
    [SerializeField] private Transform leftCheck;
    [SerializeField] private GameObject UpgradeCamera;

    [Header("Machines Colliders")]
    [SerializeField] private GameObject buildCollider;
    [SerializeField] private GameObject paintCollider;
    [SerializeField] private GameObject cementCollider;

    [Header("Tilt")]
    [SerializeField] private float lerp;
    private Vector3 pos;
    private Quaternion rot;

    [Header("Stricted Haptic Control")]
    [SerializeField] private float HapticCoolDown;
    float lastHapticTime;
    public bool canDoStrictedHaptic;

    [Header("Block Warning")]
    [SerializeField] private GameObject powerWarning;
    private bool isToolBlocked;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ChangeScrapeTool(scrapeToolIndex);
    }

    void Update()
    {
        if (!isToolBlocked)
        {
            if (!movementController.canRotate)
            {
                lastToolUsingTime = toolCoolDown;
            }
                
            if (lastToolUsingTime <= 0)
            {
                lastToolUsingTime = toolCoolDown;
                //complete making the tool hides after not using it for a while
                if (scrapeTool.showing)
                {
                    scrapeTool.ShowTool(false);
                }
            }
            else
            {
                lastToolUsingTime -= Time.deltaTime;
            }
        }

        if (lastHapticTime <= 0)
        {
            lastHapticTime = HapticCoolDown;
            canDoStrictedHaptic = true;
        }
        else
        {
            lastHapticTime -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (scrapeTool.toolAngleController != null) return;
        TiltCollectables();
    }

    private void TiltCollectables()
    {
        pos = collectableParent.position;
        rot = collectableParent.rotation;
        if (scrapeTool.toolAngleController != null)
            rot = Quaternion.Euler(Vector3.right * 90);
        for (int i = 0; i < collectables.Count; i++)
        {
            if (collectables[i].readyToTilt)
            {
                if (collectables[i].peelableCopy.transform.parent == collectableParent)
                    collectables[i].peelableCopy.transform.SetParent(GameManager.instance.currentZone.collectableParent);
                if(scrapeTool.toolAngleController != null)
                    pos += Vector3.up * angleCollectableOffest;
                else
                    pos += Vector3.up * collectableOffest;
                collectables[i].peelableCopy.transform.position = Vector3.Lerp(collectables[i].peelableCopy.transform.position, pos, lerp);
                //colectedMoney[i].transform.Rotate(rotDir * rb.velocity.magnitude);
                collectables[i].peelableCopy.transform.rotation = Quaternion.Lerp(collectables[i].peelableCopy.transform.rotation, rot, lerp);
                pos = collectables[i].peelableCopy.transform.position;
                rot = collectables[i].peelableCopy.transform.rotation;
            }
        }
    }

    public Vector3 GetClosestCheckDirection(Vector3 pos)
    {
        //used to decide if collectable spawns right or left of the tool to make a path 
        if (Vector3.Distance(pos, rightCheck.transform.position) < Vector3.Distance(pos, leftCheck.transform.position))
            return transform.right;
        else
            return transform.right * -1;
    }

    public void OnPeelableDetection(float amount, float _power,Color _dustColor)
    {
        if (!movementController.insideAngleTrigger)
        {
            if (scrapeTool.power >= _power)
                amount = 100;
        }

        movementController.SetSpeedMultiplayer(amount);
        lastToolUsingTime = toolCoolDown;
        if (!scrapeTool.showing)
        {
            scrapeTool.ShowTool(true);
        }

        // set dust color
        var main = dustVFX.main;
        main.startColor = _dustColor;
        main = dustTrailVFX.main;
        main.startColor = _dustColor;
    }

    public void OnBlockWallDetection(bool entered)
    {
        isToolBlocked = entered;
    }

    public void SetScrapingMovementSpeed(float amount, float _power)
    {
        if (!movementController.insideAngleTrigger)
        {
            if (scrapeTool.power >= _power)
                amount = 100;
        }
            
        movementController.SetSpeedMultiplayer(amount);
        lastToolUsingTime = toolCoolDown;
    }

    public void EmitDust()
    {
        dustVFX.Play();
    }

    public void OnCollect(Peelable collectable)
    {
        lastToolUsingTime = toolCoolDown;
        if (!scrapeTool.showing && !fullWarning.activeInHierarchy)
            scrapeTool.ShowTool(true);

        if (wheelBarrow != null)
        {
            if (wheelBarrow.collectables.Count < wheelBarrow.collectablesLimit)
            {
                wheelBarrow.OnCollect(collectable);
                return;
            }
        }
        if (collectables.Count >= collectablesLimit) return;
        if (scrapeTool.toolAngleController != null)
            collectable.Collect(collectables.Count, angleCollectableOffest, collectableParent);
        else
            collectable.Collect(collectables.Count, collectableOffest, collectableParent);
        collectables.Add(collectable);
        GameManager.instance.currentZone.AddCollectableData(true, collectable.index);
        if (collectables.Count >= collectablesLimit)
        {
            fullWarning.SetActive(true);
            arrowController.GetNewTarget();
        }
    }

    public void SellCollectable(Transform sellPoint)
    {
        if (wheelBarrow != null)
        {
            if (wheelBarrow.collectables.Count > 0)
            {
                wheelBarrow.SellCollectable(sellPoint);
                if(fullWarning.activeInHierarchy)
                    fullWarning.SetActive(false);
                return;
            }
        }
        if (collectables.Count == 0) return;
        Peelable collectable = collectables[collectables.Count - 1];
        collectables.Remove(collectable);
        collectable.Sell(sellPoint);
        GameManager.instance.currentZone.RemoveCollectableData(true, collectable.index);
        if (fullWarning.activeInHierarchy)
            fullWarning.SetActive(false);
    }

    public void ChangeScrapeTool(int index)
    {
        RBManagerJobs rbManager = RBManagerJobs.Instance;
        rbManager.SetTarget(transform);

        if (scrapeTool != null)
            Destroy(scrapeTool.gameObject);
        if (index >= scrapeToolsPrefabs.Length)
            index = scrapeToolsPrefabs.Length - 1;
        scrapeTool = Instantiate(scrapeToolsPrefabs[index], scrapeToolHolder);
        scrapeTool.ShowTool(false);

        if(scrapeTool.toolAngleController != null)
        {
            rbManager.SetTarget(scrapeTool.toolAngleController.toolHead);
        }

        if (scrapeTool.toolAngleController != null)
            collectableParent.localPosition = collectableParentAnglePos;
        else
            collectableParent.localPosition = collectableParentPos;

        rbManager.SetRadius(scrapeTool.rbRadius);
    }

    public void UpgradeCollectablesLimit(float value)
    {
        collectablesLimit = value;
    }

    public void ReadyForPaint()
    {
        fullWarning.SetActive(false);
        RemovePeelingAndCollectingTools();
    }

    public void GetOnAsphaltMachine(Transform playerSeat, BuildMachine _asphaltMachine)
    {
        fullWarning.SetActive(false);
        RemovePeelingAndCollectingTools();
        asphaltMachine = _asphaltMachine;
        movementController.canMove = false;

        StartCoroutine(AshphaltMachinePick(playerSeat, _asphaltMachine));
    }

    private IEnumerator AshphaltMachinePick(Transform playerSeat, BuildMachine _asphaltMachine)
    {
        yield return new WaitForSeconds(0.75f);
        if (asphaltMachine.drivable)
        {
            movementController.ToggleMovementAnimation(false);
            buildCollider.SetActive(true);
        }
        else
            cementCollider.SetActive(true);
        model.transform.SetParent(playerSeat);
        model.transform.DOLocalJump(Vector3.zero, 2, 1, 0.7f);
        model.transform.DOScale(1, 0.7f);
        model.transform.DOLocalRotate(Vector3.zero, 0.7f);
        transform.rotation = _asphaltMachine.transform.rotation;
        transform.DOMove(_asphaltMachine.transform.position, 0.7f).OnComplete(() =>
        {
            _asphaltMachine.transform.SetParent(transform);
            movementController.canMove = true;
            movementController.SetRotationSpeed(_asphaltMachine.rotationSpeed);
            GameManager.instance.currentZone.StartAsphaltStage();
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        });
    }

    public void GetOffAsphaltMachine()
    {
        if (asphaltMachine == null) return;
        asphaltMachine.gameObject.SetActive(false);
        asphaltMachine.transform.SetParent(null);
        model.transform.SetParent(transform);
        model.transform.DOLocalJump(Vector3.zero, 2, 1, 0.7f);
        model.transform.DOScale(1, 0.7f);
        model.transform.DOLocalRotate(Vector3.zero, 0.7f);
        asphaltMachine = null;
        cementCollider.SetActive(false);
        buildCollider.SetActive(false);
        movementController.ToggleMovementAnimation(true);
        movementController.SetRotationSpeed();
    }

    public void ActivateWheelBarrow(WheelBarrow _wheelBarrow)
    {
        wheelBarrow = _wheelBarrow;
    }

    public void RemovePeelingAndCollectingTools()
    {
        RBManagerJobs.Instance.SetTarget(transform);
        int x = collectables.Count;
        Peelable c;
        for (int i = 0; i < x; i++)
        {
            c = collectables[0];
            collectables.Remove(c);
            c.gameObject.SetActive(false);
            c.peelableCopy.gameObject.SetActive(false);
            GameManager.instance.currentZone.RemoveCollectableData(true, c.index);
        }
        if (wheelBarrow != null)
        {
            x = wheelBarrow.collectables.Count;
            for (int i = 0; i < x; i++)
            {
                GameManager.instance.currentZone.RemoveCollectableData(false, wheelBarrow.collectables[0].index);
                wheelBarrow.collectables.Remove(wheelBarrow.collectables[0]);
            }
        }

        if (wheelBarrow != null)
            wheelBarrow.gameObject.SetActive(false);
        collectableParent.gameObject.SetActive(false);
        scrapeToolHolder.gameObject.SetActive(false);
    }

    public void LoadCollectables(List<CollectableData> collectableDatas)
    {
        Peelable collectable;
        for (int i = 0; i < collectableDatas.Count; i++)
        {
            collectable = GameManager.instance.currentZone.peelableManager.ReturnPeelableWithIndex(collectableDatas[i].index);
            if (scrapeTool.toolAngleController != null)
                collectable.LoadCollectable(collectables.Count, angleCollectableOffest, collectableParent);
            else
                collectable.LoadCollectable(collectables.Count, collectableOffest, collectableParent);
            collectables.Add(collectable);
        }
        if (collectables.Count >= collectablesLimit)
        {
            fullWarning.SetActive(true);
            arrowController.GetNewTarget();
        }
    }

    public void ResetForNextZone()
    {
        //ChangeScrapeTool(scrapeToolIndex);
        scrapeToolHolder.gameObject.SetActive(true);
        wheelBarrow = null;
        collectableParent.gameObject.SetActive(true);
        paintMachine = null;
        TogglePaintCollider(false);
        movementController.ToggleMovementAnimation(true);
        movementController.SetRotationSpeed();
    }

    public void TogglePaintCollider(bool activate)
    {
        paintCollider.SetActive(activate);
        if(activate)
            movementController.SetRotationSpeed(paintMachine.rotationSpeed);
    }

    public bool MovementCheck()
    {
        return movementController.MovementCheck();
    }

    public void ShowWarning(bool show)
    {
        powerWarning.SetActive(show);
    }

    public void ActivateFootPrints(bool activate)
    {
        if (activate)
        {
            paintFootprintsEffect.Play();
            CancelInvoke(nameof(DisableFootPrints));
            Invoke(nameof(DisableFootPrints), footPrintsDuration);
        }
        else
        {
            paintFootprintsEffect.Stop();
        }
    }

    private void DisableFootPrints()
    {
        ActivateFootPrints(false);
    }

    public void ActivateUpgradeCamera(bool activate)
    {
        UpgradeCamera.SetActive(activate);
    }

    public void SwitchTools(bool toAngle)
    {
        if (toAngle)
            scrapeToolsPrefabs = angleScrapeToolsPrefabs;
        else
            scrapeToolsPrefabs = normalScrapeToolsPrefabs;
        ChangeScrapeTool(scrapeToolIndex);
    }

    public void ActivateWaterPodsEffect(bool activate)
    {
        waterPodsEffect.gameObject.SetActive(activate);
    }

    public void ActivatePaintPodsEffect(bool activate)
    {
        paintPodsEffect.gameObject.SetActive(activate);
    }
}
