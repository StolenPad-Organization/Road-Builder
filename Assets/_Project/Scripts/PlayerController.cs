using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [SerializeField] private PlayerMovementController movementController;
    [SerializeField] private List<Collectable> collectables;
    [SerializeField] private float collectablesLimit;
    [SerializeField] private float collectableOffest;
    [SerializeField] private Transform collectableParent;
    [SerializeField] private GameObject model;
    public AsphaltMachine asphaltMachine;

    [Header("Scrape Tools")]
    public ScrapeTool scrapeTool;
    [SerializeField] private ScrapeTool[] scrapeToolsPrefabs;
    public int scrapeToolIndex;
    [SerializeField] private Transform scrapeToolHolder;

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
        
    }

    public void OnPeelableDetection(float amount, float _power)
    {
        if (scrapeTool.power > _power)
            amount = 100;
        movementController.SetSpeedMultiplayer(amount);
    }

    public void OnCollect(Collectable collectable)
    {
        if (collectables.Count >= collectablesLimit) return;
        collectable.Collect(collectables.Count, collectableOffest, collectableParent);
        collectables.Add(collectable);
    }

    public void SellCollectable(Transform sellPoint)
    {
        if (collectables.Count == 0) return;
        Collectable collectable = collectables[collectables.Count - 1];
        collectables.Remove(collectable);
        collectable.Sell(sellPoint);
    }

    public void ChangeScrapeTool(int index)
    {
        if(scrapeTool != null)
            Destroy(scrapeTool.gameObject);
        scrapeTool = Instantiate(scrapeToolsPrefabs[index], scrapeToolHolder);
    }

    public void UpgradeCollectablesLimit(float value)
    {
        collectablesLimit = value;
    }

    public void GetOnAsphaltMachine(Transform playerSeat, AsphaltMachine _asphaltMachine)
    {
        asphaltMachine = _asphaltMachine;
        movementController.canMove = false;
        model.transform.SetParent(playerSeat);
        model.transform.DOLocalJump(Vector3.zero, 2, 1, 0.7f);
        model.transform.DOScale(1, 0.7f);
        model.transform.DOLocalRotate(Vector3.zero, 0.7f);
        transform.rotation = _asphaltMachine.transform.rotation;
        transform.DOMove(_asphaltMachine.transform.position, 0.7f).OnComplete(() =>
        {
            _asphaltMachine.transform.SetParent(transform);
            movementController.canMove = true; 
        });
    }
}
