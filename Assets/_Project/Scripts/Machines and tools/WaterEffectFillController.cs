using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterEffectFillController : MonoBehaviour
{
    private Tween disloveTween;
    [SerializeField] GameObject splashObject;
    void Start()
    {
        
    }

    void Update()
    {
        if (splashObject != null)
            splashObject.transform.eulerAngles = Vector3.right * -90f;
    }

    public void Fill()
    {
        if (disloveTween != null)
            disloveTween.Kill();
        var render = GetComponent<ParticleSystemRenderer>();
        Material mat = render.material;
        mat.SetFloat("Disolve", 0.0f);
        float t = 0.0f;
        disloveTween = DOTween.To(() => t, x => t = x, 0.5f, 0.5f)
           .OnUpdate(() => mat.SetFloat("ReverseDisolve", t)).OnComplete(()=>
           {
               if(splashObject != null)
                    splashObject.SetActive(true);
           });
    } 

    public void Empty()
    {
        if (disloveTween != null)
            disloveTween.Kill();
        var render = GetComponent<ParticleSystemRenderer>();
        Material mat = render.material;
        mat.SetFloat("Disolve", 0.5f);
        float t = 0.5f;
        disloveTween = DOTween.To(() => t, x => t = x, 0.0f, 1.0f)
           .OnUpdate(() => mat.SetFloat("ReverseDisolve", t));

        if (splashObject != null)
            splashObject.SetActive(false);
    }
}
