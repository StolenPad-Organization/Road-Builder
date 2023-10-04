using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject model;
    [SerializeField] private float checkDistance;
    [SerializeField] private Transform checkOrigin;
    private Vector3 lookpos;

    [SerializeField] private float checkTargetCooldown;
    private float nextTargetCheck;
    GameManager gameManager;
    public PlayerController player;

    void Start()
    {
        checkOrigin = transform;
        nextTargetCheck = checkTargetCooldown;
        gameManager = GameManager.instance;
    }

    private void Update()
    {
        if(nextTargetCheck <= 0)
        {
            nextTargetCheck = checkTargetCooldown;
            if(target == null)
            {
                GetNewTarget();
            }
        }
        else
        {
            nextTargetCheck -= Time.deltaTime;
        }
    }

    public void GetNewTarget()
    {
        switch (gameManager.currentZone.zoneState)
        {
            case ZoneState.PeelingStage:
                if(player.fullWarning.activeInHierarchy)
                    PointToObject(gameManager.currentZone.sellManager.gameObject);
                else
                {
                    if (player.scrapeTool.toolAngleController != null)
                        checkOrigin = player.scrapeTool.toolAngleController.toolHead;
                    else
                        checkOrigin = transform;
                    PointToObject(gameManager.currentZone.peelableManager.ReturnNearestPeelable().gameObject, false);
                }
                break;
            case ZoneState.BuildingStage:
                if(gameManager.currentZone.buildMachine.asphaltCount <= 0)
                    PointToObject(gameManager.currentZone.asphaltAmmo.gameObject);
                else
                    PointToObject(gameManager.currentZone.buildableManager.ReturnNearestBuildable().gameObject);
                break;
            case ZoneState.PaintingStage:
                if(gameManager.currentZone.paintingMachine.paintValue <= 0)
                    PointToObject(gameManager.currentZone.paintAmmo.gameObject);
                else
                {
                    if (player.paintMachine.toolAngleController != null)
                        checkOrigin = player.paintMachine.toolAngleController.toolHead;
                    else
                        checkOrigin = transform;
                    PointToObject(gameManager.currentZone.paintableManager.ReturnNearestPaintable().gameObject, false);
                }
                break;
            default:
                break;
        }
    }

    void LateUpdate()
    {
        if(target != null)
        {
            lookpos = target.transform.position;
            lookpos.y = 0;
            transform.LookAt(lookpos);

            if(Vector3.Distance(checkOrigin.position, target.transform.position) <= checkDistance)
            {
                model.SetActive(false);
                target = null;
            }
        }
    }

    public void PointToObject(GameObject _target, bool resetOrigin = true)
    {
        if (_target == null) return;
        if (resetOrigin)
            checkOrigin = transform;
        target = _target;
        model.SetActive(true);
    }
}
