using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintAmmo : MonoBehaviour
{
    [SerializeField] private Transform fillPoint;
    [SerializeField] private bool filling;
    [SerializeField] private float fillRate;
    [SerializeField] private float nextFill;
    [SerializeField] private ParticleSystem effect;
    private Tween disloveTween;

    void Start()
    {

    }

    void Update()
    {
        if (filling)
        {
            if (nextFill <= 0)
            {
                if (PlayerController.instance.paintMachine != null)
                {
                    PlayerController.instance.paintMachine.FillPaint();
                }
                nextFill = fillRate;
            }
            else
            {
                nextFill -= Time.deltaTime;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        filling = true;
        PlayerController.instance.paintMachine.OnFillStart();
        nextFill = 0;
        disloveTween.Kill();
        effect.Play();
        var render = effect.GetComponent<ParticleSystemRenderer>();
        Material mat = render.material;
        mat.SetFloat("Disolve", 0.0f);
        float t = 0.0f;
        disloveTween = DOTween.To(() => t, x => t = x, 0.5f, 1.5f)
           .OnUpdate(() => mat.SetFloat("ReverseDisolve", t));

        //var main = effect.main;
        //main.loop = true;
    }
    private void OnTriggerExit(Collider other)
    {
        filling = false;
        PlayerController.instance.paintMachine.OnFillEnd();
        //var main = effect.main;
        //main.loop = false;
        disloveTween.Kill();
        var render = effect.GetComponent<ParticleSystemRenderer>();
        Material mat = render.material;
        float t = 0.0f;
        disloveTween = DOTween.To(() => t, x => t = x, 0.5f, 1.5f)
           .OnUpdate(() => mat.SetFloat("Disolve", t)).OnComplete(()=> effect.Stop());
    }
}
