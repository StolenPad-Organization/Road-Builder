using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePointsPooler : MonoBehaviour
{
    public static UpgradePointsPooler instance;

    [SerializeField] private UpgradePoint upgradePointPrefab;
    [SerializeField] private int poolSize = 10;

    private Queue<UpgradePoint> upgradePointPool = new Queue<UpgradePoint>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            UpgradePoint upgradePoint = Instantiate(upgradePointPrefab);
            upgradePoint.gameObject.SetActive(false);
            upgradePointPool.Enqueue(upgradePoint);
        }
    }

    public UpgradePoint GetUpgradePoint()
    {
        if (upgradePointPool.Count == 0)
        {
            UpgradePoint upgradePoint = Instantiate(upgradePointPrefab);
            upgradePoint.gameObject.SetActive(false);
            upgradePointPool.Enqueue(upgradePoint);
        }

        UpgradePoint pooledupgradePoint = upgradePointPool.Dequeue();
        pooledupgradePoint.gameObject.SetActive(true);
        return pooledupgradePoint;
    }

    public void ReturnUpgradePoint(UpgradePoint upgradePoint)
    {
        upgradePoint.transform.position = transform.position;
        upgradePoint.transform.rotation = transform.rotation;

        upgradePoint.gameObject.SetActive(false);
        upgradePointPool.Enqueue(upgradePoint);
    }
}
