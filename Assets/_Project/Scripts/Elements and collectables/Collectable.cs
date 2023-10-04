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
    [SerializeField] private MeshCollider collectableMeshCollider;
    [SerializeField] private MeshCollider collectableMeshColliderTrigger;
    [SerializeField] private Rigidbody rb;
    public Peelable peelable;
    public int price;
    [SerializeField] private GameObject[] collectableShapes;
    [SerializeField] private GameObject originalModel;
    public bool readyToTilt;

    void Start()
    {
        
    }

    private void OnEnable()
    {
        
    }

    public void Spawn(Vector3 collectablePosition, Vector3 collectableRotation)
    {
        loadCollectableShape();
        //collectableCollider.enabled = false;
        //transform.DOJump(collectablePosition, 0.25f, 1, 0.4f).OnComplete(() =>
        //{
        //    collectableCollider.enabled = true;
        //});
        //transform.DOLocalRotate(collectableRotation, 0.4f);

        transform.position = collectablePosition;
        transform.eulerAngles = collectableRotation;

        collectableMeshColliderTrigger.gameObject.SetActive(false);

        rb.isKinematic = false;
        rb.useGravity = true;
        collectableCollider.enabled = true;
        collectableMeshCollider.enabled = true;

        Invoke(nameof(ActivateTrigger), 2.0f);
    }

    private void ActivateTrigger()
    {
        collectableMeshColliderTrigger.gameObject.SetActive(true);
    }

    public void Collect(int index, float collectableOffest, Transform collectableParent)
    {
        collectableMeshColliderTrigger.gameObject.SetActive(false);
        rb.isKinematic = true;
        rb.useGravity = false;
        collectableCollider.enabled = false;
        collectableMeshCollider.enabled = false;

        transform.SetParent(collectableParent);
        transform.DOLocalRotate(Vector3.zero, 0.4f);
        transform.DOLocalJump(Vector3.up * index * collectableOffest, 1, 1, 0.4f).OnComplete(() => readyToTilt = true);
        //peelable.OnCollect();
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    GameManager.instance.player.OnCollect(this);
    //}

    public void Sell(Transform sellPoint)
    {
        readyToTilt = false;
        transform.SetParent(sellPoint);
        transform.DOLocalJump(Vector3.zero, 3, 1, 0.6f).OnComplete(() => 
        {
            if (GameManager.instance.player.canDoStrictedHaptic)
            {
                EventManager.invokeHaptic.Invoke(vibrationTypes.LightImpact);
                GameManager.instance.player.canDoStrictedHaptic = false;
            }
            CollectablesPooler.Instance.ReturnCollectable(this);
            Money money = MoneyPooler.instance.GetMoney();
            money.transform.position = GameManager.instance.currentZone.sellManager.transform.position;
            money.Spawn(price);
        });
    }

    public void ClearCollectable()
    {
        foreach (var item in collectableShapes)
        {
            item.SetActive(false);
        }
        originalModel.SetActive(true);
    }

    public void LoadCollectable(int index, float collectableOffest, Transform collectableParent, Peelable _peelable, bool collected = false)
    {
        if (collected)
        {
            collectableMeshColliderTrigger.gameObject.SetActive(false);
            rb.isKinematic = true;
            rb.useGravity = false;
            collectableCollider.enabled = false;
            collectableMeshCollider.enabled = false;
        }
        else
        {
            ActivateTrigger();
        }
        collectableCollider.enabled = false;
        transform.SetParent(collectableParent);
        transform.localPosition = Vector3.up * index * collectableOffest;
        peelable = _peelable;
        loadCollectableShape();
        readyToTilt = true;
    }

    public void loadCollectableShape()
    {
        //if (!peelable.diffirentCollectable)
        //{
            collectableMeshFilter.mesh = peelable.peelableMeshFilter.mesh;
            collectableRenderer.material = peelable.peelableRenderer.material;
            collectableMeshFilter.gameObject.transform.localScale = Vector3.one;
            collectableMeshFilter.gameObject.transform.localEulerAngles = Vector3.zero;

        collectableMeshCollider.sharedMesh = collectableMeshFilter.mesh;
        collectableMeshColliderTrigger.sharedMesh = collectableMeshFilter.mesh;
        //}
        //else
        //{
        //    originalModel.SetActive(false);
        //    collectableShapes[(int)peelable.collectableShape -1].SetActive(true);
        //}
    }
}
