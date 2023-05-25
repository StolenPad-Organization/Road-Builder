using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PaintingMachine : MonoBehaviour
{
    public Transform partsSpawnPoint;
    private bool used;
    [SerializeField] private float paintCapacity;
    [SerializeField] private float paintValue;
    [SerializeField] private float paintFillRate;
    [SerializeField] private float paintConsumeRate;
    public float paintDuration;
    public float paintDelay;
    [SerializeField] private ParticleSystem paintEffect;
    [SerializeField] private float painteffectDuration;
    [SerializeField] private float painteffectRemainingTime;

    void Start()
    {
        
    }

    void Update()
    {
        if(painteffectRemainingTime <= 0)
        {
            if(paintEffect.isPlaying)
                paintEffect.Stop();
        }
        else
        {
            painteffectRemainingTime -= Time.deltaTime;
            if (!paintEffect.isPlaying)
                paintEffect.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!used)
        {
            PlayerController.instance.paintMachine = this;
            used = true;
            transform.SetParent(PlayerController.instance.transform);
            transform.DOLocalJump(Vector3.zero, 1.5f, 1, 0.5f);
            transform.DOLocalRotate(Vector3.zero, 0.5f);
            GameManager.instance.StartPaintStage();
        }
    }

    public void FillPaint()
    {
        if (paintValue >= paintCapacity) return;
        paintValue += paintFillRate;
    }

    public bool UsePaint()
    {
        if (paintValue <= 0)
        {
            paintValue = 0;
            return false;
        }
        paintValue -= paintConsumeRate;
        painteffectRemainingTime = painteffectDuration;
        return true;
    }
}
