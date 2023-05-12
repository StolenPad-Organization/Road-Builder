using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsphaltMachine : MonoBehaviour
{
    [SerializeField] private Transform playerSeat;
    public Transform partsSpawnPoint;
    [SerializeField] private GameObject playerTrigger;
    [SerializeField] private GameObject partsTrigger;
    private bool used;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!used)
        {
            PlayerController.instance.GetOnAsphaltMachine(playerSeat, this);
            used = true;
            playerTrigger.SetActive(false);
            partsTrigger.SetActive(true);
        }
    }
}
