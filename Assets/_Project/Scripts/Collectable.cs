using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Collectable : MonoBehaviour
{
    public CollectableType collectableType;
    [SerializeField] private Collider collectableCollider;
    [SerializeField] private MeshFilter collectableMeshFilter;
    [SerializeField] private MeshRenderer collectableRenderer;
    public Peelable peelable;
    public int price;
    void Start()
    {
        
    }

    private void OnEnable()
    {
        
    }

    public void Spawn()
    {
        loadCollectableShape();
        collectableCollider.enabled = false;
        transform.DOJump(transform.position, 1, 1, 0.4f).OnComplete(() =>
        {
            collectableCollider.enabled = true;
        });
    }

    public void Collect(int index, float collectableOffest, Transform collectableParent)
    {
        collectableCollider.enabled = false;
        transform.SetParent(collectableParent);
        transform.DOLocalJump(Vector3.up * index * collectableOffest, 1, 1, 0.4f);
        peelable.OnCollect();
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController.instance.OnCollect(this);
    }

    public void Sell(Transform sellPoint)
    {
        transform.SetParent(sellPoint);
        transform.DOLocalJump(Vector3.zero, 3, 1, 0.6f).OnComplete(() => 
        {
            CollectablesPooler.Instance.ReturnCollectable(this);
            Money money = MoneyPooler.instance.GetMoney();
            money.transform.position = GameManager.instance.currentZone.sellManager.transform.position;
            money.Spawn(price);
        });
    }

    public void LoadCollectable(int index, float collectableOffest, Transform collectableParent, Peelable _peelable)
    {
        collectableCollider.enabled = false;
        transform.SetParent(collectableParent);
        transform.localPosition = Vector3.up * index * collectableOffest;
        peelable = _peelable;
        loadCollectableShape();
    }

    public void loadCollectableShape()
    {
        if (!peelable.diffirentCollectable)
        {
            collectableMeshFilter.mesh = peelable.peelableMeshFilter.mesh;
            collectableRenderer.material = peelable.peelableRenderer.material;
            collectableMeshFilter.gameObject.transform.localScale = Vector3.one * 0.75f;
            collectableMeshFilter.gameObject.transform.localEulerAngles = Vector3.zero;
        }
    }
}
