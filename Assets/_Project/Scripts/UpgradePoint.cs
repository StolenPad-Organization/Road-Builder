using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UpgradePoint : MonoBehaviour
{
    [SerializeField] private Collider upgradePointCollider;
    [SerializeField] private int value;
    [SerializeField] private ParticleSystem upgradePointPickingVfx;
    private Vector3 ipos;

    void Start()
    {

    }

    public void Spawn(int price , Vector3 pos)
    {
        ipos = pos;
        value = price;
        upgradePointCollider.enabled = false;
        transform.position = ipos;
        transform.DOJump(ipos, 4, 1, 1.5f).OnComplete(() =>
        {
            upgradePointCollider.enabled = true;
        });
        GameManager.instance.currentZone.AddUpgradePointData(ipos, value);
    }

    private void Collect()
    {
        EventManager.AddUpgradePoint.Invoke(value, transform);
        UpgradePointsPooler.instance.ReturnUpgradePoint(this);
        //UIManager.instance.UpdateUpgradePoints(value);
        GameManager.instance.currentZone.RemoveUpgradePointData(ipos, value);
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        upgradePointPickingVfx.transform.SetParent(null);
        upgradePointPickingVfx.transform.position = transform.position + (Vector3.up * 0.25f);
        upgradePointPickingVfx.gameObject.SetActive(true);
        upgradePointPickingVfx.Play();

        Collect();
    }

    public void LoadUpgradePoint(int price, Vector3 pos)
    {
        ipos = pos;
        value = price;
        transform.position = ipos;
    }
}
