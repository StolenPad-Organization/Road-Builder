using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterEffectFillController : MonoBehaviour
{
    private Tween disloveTween;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Fill()
    {
        var render = GetComponent<ParticleSystemRenderer>();
        Material mat = render.material;
        mat.SetFloat("Disolve", 0.0f);
        float t = 0.0f;
        disloveTween = DOTween.To(() => t, x => t = x, 0.5f, 1.0f)
           .OnUpdate(() => mat.SetFloat("ReverseDisolve", t));
    } 

    public void Empty()
    {
        var render = GetComponent<ParticleSystemRenderer>();
        Material mat = render.material;
        mat.SetFloat("Disolve", 0.5f);
        float t = 0.5f;
        disloveTween = DOTween.To(() => t, x => t = x, 0.0f, 1.0f)
           .OnUpdate(() => mat.SetFloat("ReverseDisolve", t));
    }
}
