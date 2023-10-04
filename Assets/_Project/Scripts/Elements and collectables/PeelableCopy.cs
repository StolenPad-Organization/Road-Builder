using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeelableCopy : MonoBehaviour
{
    [SerializeField] public Peelable peelable;
    [SerializeField] private BoxCollider copyCollider;

#if UNITY_EDITOR
    public void SetPeelableCopy(Peelable _peelable)
    {
        peelable = _peelable;
        peelable.peelableCopy = this;
        if(copyCollider == null)
        {
            if (gameObject.GetComponent<BoxCollider>())
                copyCollider = gameObject.GetComponent<BoxCollider>();
            else
                copyCollider = gameObject.AddComponent<BoxCollider>();

            copyCollider.enabled = false;
            copyCollider.isTrigger = true;
        }

        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.EditorUtility.SetDirty(gameObject);
        UnityEditor.EditorUtility.SetDirty(peelable);
    }

    public void SetPeelableCopy(Peelable _peelable, Material mat)
    {
        peelable = _peelable;
        peelable.peelableCopy = this;
        if (copyCollider == null)
        {
            if (gameObject.GetComponent<BoxCollider>())
                copyCollider = gameObject.GetComponent<BoxCollider>();
            else
                copyCollider = gameObject.AddComponent<BoxCollider>();

            copyCollider.enabled = false;
            copyCollider.isTrigger = true;
        }

        var rd = GetComponent<Renderer>();
        rd.sharedMaterial = mat;

        rd.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        rd.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
        rd.motionVectorGenerationMode = MotionVectorGenerationMode.ForceNoMotion;
        rd.allowOcclusionWhenDynamic = false;

        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.EditorUtility.SetDirty(rd);
        UnityEditor.EditorUtility.SetDirty(gameObject);
        UnityEditor.EditorUtility.SetDirty(peelable);
    }
#endif

    public void ActivateCollision()
    {
        copyCollider.enabled = true;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (peelable.zoneIndex != GameManager.instance.levelProgressData.ZoneIndex || other.gameObject.layer != LayerMask.NameToLayer("ScrapTool")) return;
    //    if (other.CompareTag("Collector"))
    //    {
    //        GameManager.instance.player.OnCollect(peelable);
    //    }
    //}

    public void Collect(int index, float collectableOffest, Transform collectableParent, PlayerController player)
    {
        copyCollider.enabled = false;
        peelable.collected = true;

        transform.SetParent(collectableParent);
        if(player.scrapeTool.toolAngleController!=null)
            transform.DOLocalRotate(Vector3.right * 90, 0.4f);
        else
            transform.DOLocalRotate(Vector3.zero, 0.4f);
        transform.DOLocalJump(Vector3.up * index * collectableOffest, 1f + (index * 0.1f), 1, 0.4f).OnComplete(() => peelable.readyToTilt = true);
        peelable.SavePeelable();
    }

    public void Sell(Transform sellPoint)
    {
        peelable.collected = false;
        peelable.sold = true;
        peelable.readyToTilt = false;
        transform.SetParent(sellPoint);
        transform.DOLocalJump(Vector3.zero, 3, 1, 0.6f).OnComplete(() =>
        {
            if (GameManager.instance.player.canDoStrictedHaptic)
            {
                EventManager.invokeHaptic.Invoke(vibrationTypes.LightImpact);
                GameManager.instance.player.canDoStrictedHaptic = false;
            }
            Money money = MoneyPooler.instance.GetMoney();
            money.transform.position = GameManager.instance.currentZone.sellManager.transform.position;
            money.Spawn(peelable.price);
            gameObject.SetActive(false);
        });
    }

    public void LoadCollectable(int index, float collectableOffest, Transform collectableParent)
    {
        copyCollider.enabled = false;

        peelable.peeled = true;
        peelable.collected = true;;

        transform.SetParent(collectableParent);
        Vector3 rot = Vector3.zero;
        //if (GameManager.instance.player.scrapeTool.toolAngleController != null)
        //    rot = Vector3.right * 90;
        transform.localEulerAngles = rot;
        transform.localPosition = Vector3.up * index * collectableOffest;
        peelable.readyToTilt = true;
    }

    public void LoadCopy()
    {
        if (peelable.peeled && !peelable.collected && !peelable.sold)
            ActivateCollision();
        if (peelable.moved)
        {
            transform.position = peelable.transform.position;
            transform.localEulerAngles = peelable.transform.eulerAngles;
        }
    }
}
