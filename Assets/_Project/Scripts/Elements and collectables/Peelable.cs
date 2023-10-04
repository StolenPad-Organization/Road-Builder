using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;

public class Peelable : MonoBehaviour
{
    public int index;
    public float power;
    public float initialPower;
    public float speedAmount;
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
    public Material effectMat;
    public int blockNumber;
    public Color movedPieceColor;
    public RBHandler rbHandler;
    public PeelableCopy peelableCopy;

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

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (zoneIndex != GameManager.instance.levelProgressData.ZoneIndex || other.gameObject.layer != LayerMask.NameToLayer("ScrapTool")) return;
    //    if (other.CompareTag("Collector")) return;
    //    CollectPeeled();
    //}

    public void CollectPeeled()
    {
        if (!peeled)
        {
            if (!moved)
            {
                PlayEffect();
                rb.constraints = RigidbodyConstraints.None;
                moved = true;
                //peelableRenderer.material.color = movedPieceColor;
                rb.AddForce(Vector3.up * 4, ForceMode.Impulse);
            }
         
        }
        //else if (!collected)
        //{
        //    GameManager.instance.player.OnCollect(this);
        //}
        //if (GameManager.instance.player.canDoStrictedHaptic)
        //{
        //    EventManager.invokeHaptic.Invoke(vibrationTypes.MediumImpact);
        //    GameManager.instance.player.canDoStrictedHaptic = false;
        //}
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (zoneIndex != GameManager.instance.levelProgressData.ZoneIndex || other.gameObject.layer != LayerMask.NameToLayer("ScrapTool")) return;
    //    if (other.CompareTag("Collector")) return;
    //    PeeledStay();
    //}

    public void PeeledStay(float _power)
    {
        if (!peeled)
        {
          power -= _power;

            //GameManager.instance.player.scrapeTool.ShakeTool();
            if (power <= 0)
            {
                peeled = true;
                peelableCopy.ActivateCollision();
                GameManager.instance.currentZone.OnBlockRemove(blockNumber);
                SavePeelable();
            }
            //GameManager.instance.player.SetScrapingMovementSpeed(speedAmount, initialPower);
        }
        //else
        //{
        //    GameManager.instance.player.SetScrapingMovementSpeed(speedAmount, initialPower);
        //}
    }

    public void PlayEffect()
    {
        ParticleSystem effect = ScrapingEffectPooler.instance.GetEffect();
        effect.transform.position = transform.position;
        if(effectMat != null)
            effect.GetComponent<ParticleSystemRenderer>().material = effectMat;
        else
            effect.GetComponent<ParticleSystemRenderer>().material.color = dustColor;
        effect.gameObject.SetActive(true);
        ScrapingEffectPooler.instance.ReturnEffect(effect);
    }

    public void Collect(int index, float collectableOffest, Transform collectableParent, PlayerController player)
    {
        //collected = true;

        //peelableCollider.enabled = false;
        //rb.isKinematic = true;
        //rb.useGravity = false;

        //transform.SetParent(collectableParent);
        //transform.DOLocalRotate(Vector3.zero, 0.4f);
        //transform.DOLocalJump(Vector3.up * index * collectableOffest, 1f + (index * 0.1f), 1, 0.4f).OnComplete(() => readyToTilt = true);
        //SavePeelable();
        RBManagerJobs.Instance.RemoveAgent(rbHandler);
        rbHandler.RemoveFromTile();
        rbHandler.CheckSwitch(false);
        peelableCopy.Collect(index, collectableOffest, collectableParent, player);
    }

    public void Sell(Transform sellPoint)
    {
        //collected = false;
        //sold = true;
        //readyToTilt = false;
        //transform.SetParent(sellPoint);
        //transform.DOLocalJump(Vector3.zero, 3, 1, 0.6f).OnComplete(() =>
        //{
        //    EventManager.invokeHaptic.Invoke(vibrationTypes.LightImpact);
        //    Money money = MoneyPooler.instance.GetMoney();
        //    money.transform.position = GameManager.instance.currentZone.sellManager.transform.position;
        //    money.Spawn(price);
        //    gameObject.SetActive(false);
        //});

        peelableCopy.Sell(sellPoint);
    }

    public void SetRBHandler(RBHandler _rbHandler)
    {
        rbHandler = _rbHandler;
#if UNITY_EDITOR

        UnityEditor.EditorUtility.SetDirty(this);
#endif
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
        peelableRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        peelableRenderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        peelableRenderer.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
        peelableRenderer.allowOcclusionWhenDynamic = false;
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
            rbHandler.RemoveFromTile();
            peelableCopy.gameObject.SetActive(false);
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
            //peelableRenderer.material.color = movedPieceColor;
        }
        if(peeled)
            power = 0;

        peelableCopy.LoadCopy();
    }
    public void LoadCollectable(int index, float collectableOffest, Transform collectableParent)
    {
        //peeled = true;
        //collected = true;

        //peelableCollider.enabled = false;
        //rb.isKinematic = true;
        //rb.useGravity = false;

        //transform.SetParent(collectableParent);
        //transform.localEulerAngles = Vector3.zero;
        //transform.localPosition = Vector3.up * index * collectableOffest;
        //readyToTilt = true;
        //peelableRenderer.material.color = movedPieceColor;

        rbHandler.RemoveFromTile();
        rbHandler.CheckSwitch(false);
        peelableCopy.LoadCollectable(index, collectableOffest, collectableParent);
    }
}
