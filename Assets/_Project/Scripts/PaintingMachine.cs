using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PaintingMachine : MonoBehaviour
{
    public Transform partsSpawnPoint;
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
            PlayerController.instance.paintMachine = this;
            used = true;
            transform.SetParent(PlayerController.instance.transform);
            transform.DOLocalJump(Vector3.zero, 1.5f, 1, 0.5f);
            transform.DOLocalRotate(Vector3.zero, 0.5f);
        }
    }
}
