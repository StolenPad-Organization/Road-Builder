using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject model;
    [SerializeField] private float checkDistance;
    private Vector3 lookpos;

    [SerializeField] private float checkTargetCooldown;
    private float nextTargetCheck;
    GameManager gameManager;

    void Start()
    {
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

    private void GetNewTarget()
    {
        switch (gameManager.currentZone.zoneState)
        {
            case ZoneState.PeelingStage:
                if(PlayerController.instance.fullWarning.activeInHierarchy)
                    PointToObject(gameManager.currentZone.sellManager.gameObject);
                else
                    PointToObject(gameManager.currentZone.peelableManager.ReturnNearestPeelable().gameObject);
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
                    PointToObject(gameManager.currentZone.paintableManager.ReturnNearestPaintable().gameObject);
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

            if(Vector3.Distance(transform.position, target.transform.position) <= checkDistance)
            {
                model.SetActive(false);
                target = null;
            }
        }
    }

    public void PointToObject(GameObject _target)
    {
        target = _target;
        model.SetActive(true);
    }
}
