using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public PlayerMovementController movementController;
    [SerializeField] private List<Collectable> collectables;
    [SerializeField] private float collectablesLimit;
    [SerializeField] private float collectableOffest;
    [SerializeField] private Transform collectableParent;
    [SerializeField] private GameObject model;
    public GameObject fullWarning;
    public BuildMachine asphaltMachine;
    public PaintMachine paintMachine;
    public WheelBarrow wheelBarrow;
    public Transform wheelBarrowFollowTransform;
    public ArrowController arrowController;

    [Header("Scrape Tools")]
    public ScrapeTool scrapeTool;
    [SerializeField] private ScrapeTool[] scrapeToolsPrefabs;
    public int scrapeToolIndex;
    public Transform scrapeToolHolder;
    float lastToolUsingTime;
    [SerializeField] private float toolCoolDown;
    public Transform hidePos;
    public Transform showPos;
    [SerializeField] private ParticleSystem dustVFX;
    [SerializeField] private Transform rightCheck;
    [SerializeField] private Transform leftCheck;

    [Header("Machines Colliders")]
    [SerializeField] private GameObject buildCollider;
    [SerializeField] private GameObject paintCollider;
    [SerializeField] private GameObject cementCollider;

    [Header("Tilt")]
    [SerializeField] private float lerp;
    private Vector3 pos;
    private Quaternion rot;

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
        if(lastToolUsingTime <= 0)
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

    private void FixedUpdate()
    {
        TiltCollectables();
    }

    private void TiltCollectables()
    {
        pos = collectableParent.position;
        rot = collectableParent.rotation;
        for (int i = 0; i < collectables.Count; i++)
        {
            if (collectables[i].readyToTilt)
            {
                if (collectables[i].transform.parent == collectableParent)
                    collectables[i].transform.SetParent(null);
                pos += Vector3.up * collectableOffest;
                collectables[i].transform.position = Vector3.Lerp(collectables[i].transform.position, pos, lerp);
                //colectedMoney[i].transform.Rotate(rotDir * rb.velocity.magnitude);
                collectables[i].transform.rotation = Quaternion.Lerp(collectables[i].transform.rotation, rot, lerp);
                pos = collectables[i].transform.position;
                rot = collectables[i].transform.rotation;
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

    public void OnPeelableDetection(float amount, float _power)
    {
        if (scrapeTool.power > _power)
            amount = 100;
        movementController.SetSpeedMultiplayer(amount);
        lastToolUsingTime = toolCoolDown;
        if (!scrapeTool.showing)
        {
            scrapeTool.ShowTool(true);
        }
    }

    public void EmitDust()
    {
        dustVFX.Play();
    }

    public void OnCollect(Collectable collectable)
    {
        lastToolUsingTime = toolCoolDown;
        if (!scrapeTool.showing)
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
        collectable.Collect(collectables.Count, collectableOffest, collectableParent);
        collectables.Add(collectable);
        GameManager.instance.currentZone.AddCollectableData(true, collectable.collectableType, collectable.peelable);
        if (collectables.Count >= collectablesLimit)
        {
            fullWarning.SetActive(true);
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
        Collectable collectable = collectables[collectables.Count - 1];
        collectables.Remove(collectable);
        collectable.Sell(sellPoint);
        GameManager.instance.currentZone.RemoveCollectableData(true, collectable.collectableType, collectable.peelable);
        if (fullWarning.activeInHierarchy)
            fullWarning.SetActive(false);
    }

    public void ChangeScrapeTool(int index)
    {
        if(scrapeTool != null)
            Destroy(scrapeTool.gameObject);
        if (index >= scrapeToolsPrefabs.Length)
            index = scrapeToolsPrefabs.Length - 1;
        scrapeTool = Instantiate(scrapeToolsPrefabs[index], scrapeToolHolder);
        scrapeTool.ShowTool(false);
    }

    public void UpgradeCollectablesLimit(float value)
    {
        collectablesLimit = value;
    }

    public void GetOnAsphaltMachine(Transform playerSeat, BuildMachine _asphaltMachine)
    {
        fullWarning.SetActive(false);
        RemovePeelingAndCollectingTools();
        asphaltMachine = _asphaltMachine;
        movementController.canMove = false;
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
    }

    public void ActivateWheelBarrow(WheelBarrow _wheelBarrow)
    {
        wheelBarrow = _wheelBarrow;
    }

    public void RemovePeelingAndCollectingTools()
    {
        int x = collectables.Count;
        for (int i = 0; i < x; i++)
        {
            GameManager.instance.currentZone.RemoveCollectableData(true, collectables[0].collectableType, collectables[0].peelable);
            CollectablesPooler.Instance.ReturnCollectable(collectables[0]);
            collectables.Remove(collectables[0]);
        }
        if (wheelBarrow != null)
        {
            x = wheelBarrow.collectables.Count;
            for (int i = 0; i < x; i++)
            {
                GameManager.instance.currentZone.RemoveCollectableData(false, wheelBarrow.collectables[0].collectableType, wheelBarrow.collectables[0].peelable);
                CollectablesPooler.Instance.ReturnCollectable(wheelBarrow.collectables[0]);
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
        Collectable collectable;
        for (int i = 0; i < collectableDatas.Count; i++)
        {
            collectable = CollectablesPooler.Instance.GetCollectable(collectableDatas[i].CollectableType, Vector3.down * 10);
            collectable.LoadCollectable(collectables.Count, collectableOffest, collectableParent, collectableDatas[i].Peelable);
            collectables.Add(collectable);
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
    }

    public void TogglePaintCollider(bool activate)
    {
        paintCollider.SetActive(activate);
    }

    public bool MovementCheck()
    {
        return movementController.MovementCheck();
    }
}
