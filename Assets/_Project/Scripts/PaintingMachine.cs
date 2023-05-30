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
    [SerializeField] private GameObject fullWarning;
    [SerializeField] private GameObject emptyWarning;

    void Start()
    {
        OnSpawn();
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
            GameManager.instance.currentStage.currentZone.StartPaintStage();
        }
    }

    public void FillPaint()
    {
        if (paintValue >= paintCapacity)
        {
            if (!fullWarning.activeInHierarchy)
            {
                fullWarning.SetActive(true);
            }
            return; 
        }
        paintValue += paintFillRate;
        if (emptyWarning.activeInHierarchy)
        {
            emptyWarning.SetActive(false);
        }
    }

    public bool UsePaint()
    {
        if (paintValue <= 0)
        {
            paintValue = 0;
            if (!emptyWarning.activeInHierarchy)
            {
                emptyWarning.SetActive(true);
            }
            return false;
        }
        paintValue -= paintConsumeRate;
        painteffectRemainingTime = painteffectDuration;
        if (fullWarning.activeInHierarchy)
        {
            fullWarning.SetActive(false);
        }
        return true;
    }

    public void OnSpawn()
    {
        used = true;
        transform.DOMove(Vector3.right * 1.75f, 2.0f).OnComplete(()=>
        {
            used = false;
            PlayerController.instance.arrowController.PointToObject(gameObject);
        });
    }
}
