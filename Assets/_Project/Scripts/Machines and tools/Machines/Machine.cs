using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Machine : MonoBehaviour
{
    protected bool used;

    private void OnTriggerEnter(Collider other)
    {
        if (!used && other.CompareTag("Player"))
        {
            used = true;
            TriggerEffect();
        }
    }

    protected abstract void TriggerEffect();

    public abstract void FillMachine();

    public abstract bool UseMachine();

    public abstract void OnSpawn();

    public abstract void SetUpgrade(int index, int level);
}
