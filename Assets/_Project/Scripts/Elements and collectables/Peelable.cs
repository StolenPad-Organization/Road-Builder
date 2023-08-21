using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;

public class Peelable : MonoBehaviour
{
    public int index;
    public float power;
    private float initialPower;
    [SerializeField] private float speedAmount;
    [SerializeField] private CollectableType collectableType;
    public MeshFilter peelableMeshFilter;
    public MeshRenderer peelableRenderer;
    public Rigidbody rb;
    public MeshCollider peelableCollider;
    public bool diffirentCollectable;
    private Vector3 peelablePosition;
    private Vector3 peelableRotation;
    public int zoneIndex;
    public CollectableShape collectableShape;
    public Color dustColor;
    public int blockNumber;
    public Color movedPieceColor;

    public bool peeled;
    public bool collected;
    public bool sold;
    public bool moved;

    [Header("Collectable Properties")]
    public bool readyToTilt;
    public int price;

    void Start()
    {
        initialPower = power;
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (zoneIndex != GameManager.instance.levelProgressData.ZoneIndex || other.gameObject.layer != LayerMask.NameToLayer("ScrapTool")) return;
        PlayerController.instance.OnPeelableDetection(speedAmount, initialPower, dustColor);
        if (!peeled)
        {
            if(!moved)
                StartCoroutine(PlayEffect());
            rb.constraints = RigidbodyConstraints.None;
            moved = true;
            peelableRenderer.material.color = movedPieceColor;
            rb.AddForce(Vector3.up * 4, ForceMode.Impulse);
        }
        else if(!collected)
        {
            PlayerController.instance.OnCollect(this);
        }
        EventManager.invokeHaptic.Invoke(vibrationTypes.MediumImpact);
    }

    private void OnTriggerStay(Collider other)
    {
        if (zoneIndex != GameManager.instance.levelProgressData.ZoneIndex || other.gameObject.layer != LayerMask.NameToLayer("ScrapTool")) return;
        if (!peeled)
        {
            power -= PlayerController.instance.scrapeTool.power;
            if (peelableRenderer.material.HasFloat("_Power"))
                peelableRenderer.material.SetFloat("_Power", 1 - (power / initialPower));
            //PlayerController.instance.scrapeTool.ShakeTool();
            if (power <= 0)
            {
                peeled = true;
                GameManager.instance.currentZone.OnBlockRemove();
                SavePeelable();
            }
            PlayerController.instance.SetScrapingMovementSpeed(speedAmount, initialPower);
        }
        else
        {
            PlayerController.instance.OnPeelableDetection(speedAmount, initialPower, dustColor);
        }
    }

    private IEnumerator PlayEffect()
    {
        ParticleSystem effect = ScrapingEffectPooler.instance.GetEffect();
        effect.transform.position = transform.position;
        effect.GetComponent<ParticleSystemRenderer>().material.color = dustColor;
        effect.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        ScrapingEffectPooler.instance.ReturnEffect(effect);
    }

    public void Collect(int index, float collectableOffest, Transform collectableParent)
    {
        collected = true;

        peelableCollider.enabled = false;
        rb.isKinematic = true;
        rb.useGravity = false;

        transform.SetParent(collectableParent);
        transform.DOLocalRotate(Vector3.zero, 0.4f);
        transform.DOLocalJump(Vector3.up * index * collectableOffest, 1f + (index * 0.1f), 1, 0.4f).OnComplete(() => readyToTilt = true);
        SavePeelable();
    }
    public void Sell(Transform sellPoint)
    {
        collected = false;
        sold = true;
        readyToTilt = false;
        transform.SetParent(sellPoint);
        transform.DOLocalJump(Vector3.zero, 3, 1, 0.6f).OnComplete(() =>
        {
            EventManager.invokeHaptic.Invoke(vibrationTypes.LightImpact);
            Money money = MoneyPooler.instance.GetMoney();
            money.transform.position = GameManager.instance.currentZone.sellManager.transform.position;
            money.Spawn(price);
            gameObject.SetActive(false);
        });
    }

#if UNITY_EDITOR
    public void SetPeelableEditor(int _index)
    {
        index = _index;
        peelableMeshFilter = GetComponent<MeshFilter>();
        peelableRenderer = GetComponent<MeshRenderer>();
        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(gameObject);
    }

    public void SetMaterialEditor(Material mat)
    {
        peelableRenderer.material = mat;
        EditorUtility.SetDirty(peelableRenderer);
        EditorUtility.SetDirty(gameObject);
    }

    public void SetPowerSpeed(float _power, float _speed)
    {
        power = _power;
        speedAmount = _speed;
    }
#endif

    public void SavePeelable()
    {
        peelablePosition = transform.position;
        peelableRotation = transform.eulerAngles;
        GameManager.instance.currentZone.SavePeelable(index, peeled, collected, sold, moved, peelablePosition, peelableRotation);
    }

    public void loadPeelable(PeelableData peelableData)
    {
        moved = peelableData.IsMoved;
        peeled = peelableData.IsPeeled;
        sold = peelableData.IsSold;
        if (sold)
        {
            gameObject.SetActive(false);
            return;
        }
        peelablePosition = peelableData.PeelablePosition;
        peelableRotation = peelableData.PeelableRotation;
        if (moved)
        {
            transform.position = peelablePosition;
            transform.localEulerAngles = peelableRotation;
            rb.constraints = RigidbodyConstraints.None;
            peelableRenderer.material.color = movedPieceColor;
        }
        if(peeled)
            power = 0;
    }
    public void LoadCollectable(int index, float collectableOffest, Transform collectableParent)
    {
        // set transform and settings for the peelable as it's collected
        peeled = true;
        collected = true;

        peelableCollider.enabled = false;
        rb.isKinematic = true;
        rb.useGravity = false;

        transform.SetParent(collectableParent);
        transform.localEulerAngles = Vector3.zero;
        transform.localPosition = Vector3.up * index * collectableOffest;
        readyToTilt = true;
        peelableRenderer.material.color = movedPieceColor;
    }
}
