using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsphaltAmmo : MonoBehaviour
{
    [SerializeField] private Transform fillPoint;
    [SerializeField] private bool filling;
    [SerializeField] private float fillRate;
    [SerializeField] private float nextFill;

    void Start()
    {
        
    }

    void Update()
    {
        if (filling)
        {
            if (nextFill <= 0)
            {
                if(PlayerController.instance.asphaltMachine != null)
                {
                    PlayerController.instance.asphaltMachine.FillAsphalt();
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
        nextFill = 0;
    }
    private void OnTriggerExit(Collider other)
    {
        filling = false;
    }
}
