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
    //        PlayerController.instance.OnCollect(peelable);
    //    }
    //}

    public void Collect(int index, float collectableOffest, Transform collectableParent)
    {
        copyCollider.enabled = false;
        peelable.collected = true;

        //peelableCollider.enabled = false;
        //rb.isKinematic = true;
        //rb.useGravity = false;
        transform.SetParent(collectableParent);
        if(PlayerController.instance.scrapeTool.toolAngleController!=null)
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
            if (PlayerController.instance.canDoStrictedHaptic)
            {
                EventManager.invokeHaptic.Invoke(vibrationTypes.LightImpact);
                PlayerController.instance.canDoStrictedHaptic = false;
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
        // set transform and settings for the peelable as it's collected
        peelable.peeled = true;
        peelable.collected = true;

        //peelableCollider.enabled = false;
        //rb.isKinematic = true;
        //rb.useGravity = false;

        transform.SetParent(collectableParent);
        Vector3 rot = Vector3.zero;
        if (PlayerController.instance.scrapeTool.toolAngleController != null)
            rot = Vector3.right * 90;
        transform.localEulerAngles = rot;
        transform.localPosition = Vector3.up * index * collectableOffest;
        peelable.readyToTilt = true;
        //peelable.peelableRenderer.material.color = peelable.movedPieceColor;
        //GetComponent<Renderer>().material.color = peelable.movedPieceColor;
    }

    public void LoadCopy()
    {
        if (peelable.peeled && !peelable.collected && !peelable.sold)
            ActivateCollision();
        if (peelable.moved)
        {
            transform.position = peelable.transform.position;
            transform.localEulerAngles = peelable.transform.eulerAngles;
            //GetComponent<Renderer>().material.color = peelable.movedPieceColor;
        }
    }
}
