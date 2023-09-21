using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PaintMachine : MonoBehaviour
{
    public Transform partsSpawnPoint;
    [SerializeField] private bool used;
    [SerializeField] private float paintCapacity;
    public float paintValue;
    [SerializeField] private float paintFillRate;
    [SerializeField] private float paintConsumeRate;
    public float paintDuration;
    public float paintDelay;
    [SerializeField] private GameObject paintvfx;
    [SerializeField] private float painteffectDuration;
    [SerializeField] private float painteffectRemainingTime;
    [SerializeField] private GameObject fullWarning;
    [SerializeField] private GameObject emptyWarning;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject playerTrigger;
    [SerializeField] private GameObject partsTrigger;
    [SerializeField] private GameObject paintScalingObject;
    private bool effectActivated;
    public ToolAngleController toolAngleController;
    public bool hasUpgrade;
    public float rotationSpeed;

    [Header("Machine Icon")]
    [SerializeField] private MachineIconController machineIcon;

    void Start()
    {
        //OnSpawn();
    }

    void Update()
    {
        if(painteffectRemainingTime <= 0)
        {
            //if(paintEffect.isPlaying)
            //    paintEffect.Stop();
            if (effectActivated)
                ShowEffect(false);
                //paintvfx.SetActive(false);
        }
        else
        {
            painteffectRemainingTime -= Time.deltaTime;
            //if (!paintEffect.isPlaying)
            //    paintEffect.Play();
            if (!effectActivated)
                ShowEffect(true);
            //paintvfx.SetActive(true);
        }

        if (used)
        {
            if (!anim.GetBool("Run") && PlayerController.instance.MovementCheck())
            {
                anim.SetBool("Run", true);
            }

            if (anim.GetBool("Run") && !PlayerController.instance.MovementCheck())
            {
                anim.SetBool("Run", false);
            }
        }
    }

    private void ShowEffect(bool show)
    {
        effectActivated = show;
        if (show)
            paintvfx.transform.DOScale(0.8f, 0.15f);
        else
            paintvfx.transform.DOScale(0, 0.15f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!used && other.CompareTag("Player"))
        {
            PlayerController.instance.paintMachine = this;
            used = true;
            
            if(machineIcon != null)
                machineIcon.Fade();
            PlayerController.instance.movementController.canMove = false;
            PlayerController.instance.SetWalkType(0);

            StartCoroutine(PaintMachineEquipe());
        }
    }

    IEnumerator PaintMachineEquipe()
    {
        yield return new WaitForSeconds(0.75f);
        transform.SetParent(PlayerController.instance.transform);
        transform.DOLocalJump(Vector3.zero, 1.5f, 1, 0.5f);
        transform.DOLocalRotate(Vector3.zero, 0.5f);
        GameManager.instance.currentZone.StartPaintStage();
        if (toolAngleController == null)
            PlayerController.instance.TogglePaintCollider(true);
        else
            toolAngleController.OnPick();
        playerTrigger.SetActive(false);
        partsTrigger.SetActive(true);
        PlayerController.instance.movementController.canMove = true;
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
        paintScalingObject.transform.localScale = calculateScale();
    }

    private Vector3 calculateScale()
    {
        return Vector3.Lerp(Vector3.zero, Vector3.one * 0.72f, Mathf.InverseLerp(0.0f, paintCapacity, paintValue));
    }

    public bool UsePaint()
    {
        if (paintValue <= 0)
        {
            paintValue = 0;
            if (!emptyWarning.activeInHierarchy)
            {
                emptyWarning.SetActive(true);
                PlayerController.instance.arrowController.GetNewTarget();
            }
            return false;
        }
        paintValue -= paintConsumeRate;
        painteffectRemainingTime = painteffectDuration;
        if (fullWarning.activeInHierarchy)
        {
            fullWarning.SetActive(false);
        }
        paintScalingObject.transform.localScale = calculateScale();
        return true;
    }

    public void OnSpawn()
    {
        //machineCollider.enabled = true;
        playerTrigger.SetActive(true);

        //used = true;
        //anim.SetBool("Run", true);
        //transform.DOMove(GameManager.instance.currentZone.machinesPosition.position + Vector3.right * 1.75f, 2.0f).OnComplete(()=>
        //{
        used = false;
            PlayerController.instance.arrowController.PointToObject(gameObject);
        //anim.SetBool("Run", false);
        //});
        if (machineIcon != null)
            machineIcon.OnSpawn();
    }

    public void OnFillStart()
    {
        anim.SetBool("Fill", true);
    }

    public void OnFillEnd()
    {
        anim.SetBool("Fill", false);
    }

    public void SetPaintCapacity(float value)
    {
        paintCapacity = value;
    }
}
