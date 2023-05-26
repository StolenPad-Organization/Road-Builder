using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject model;
    [SerializeField] private float checkDistance;
    private Vector3 lookpos;

    void Start()
    {
        
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
