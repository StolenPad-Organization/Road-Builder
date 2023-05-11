using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    public float power;
    [SerializeField] private PlayerMovementController movementController;
    [SerializeField] private List<Collectable> collectables;
    [SerializeField] private int collectablesLimit;
    [SerializeField] private float collectableOffest;
    [SerializeField] private Transform collectableParent;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void OnPeelableDetection(float amount, float _power)
    {
        if (power > _power)
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
}
