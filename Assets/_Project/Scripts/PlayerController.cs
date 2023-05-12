using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    
    [SerializeField] private PlayerMovementController movementController;
    [SerializeField] private List<Collectable> collectables;
    [SerializeField] private float collectablesLimit;
    [SerializeField] private float collectableOffest;
    [SerializeField] private Transform collectableParent;

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
}
