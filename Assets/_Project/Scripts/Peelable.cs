using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peelable : MonoBehaviour
{
    [SerializeField] private float power;
    [SerializeField] private float speedAmount;
    [SerializeField] private CollectableType collectableType;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController.instance.OnPeelableDetection(speedAmount, power);
    }

    private void OnTriggerStay(Collider other)
    {
        power -= PlayerController.instance.tool.power;
        if(power <= 0)
        {
            CollectablesPooler.Instance.GetCollectable(collectableType, transform.position);
            gameObject.SetActive(false);
        }
    }
}
