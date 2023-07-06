using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMachineUpgradeMenuTrigger : MonoBehaviour
{
    private UIManager uIManager;
    void Start()
    {
        uIManager = UIManager.instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        uIManager.ShowMachineUpgradeMenu();
    }

    private void OnTriggerExit(Collider other)
    {
        uIManager.HideMachineUpgradeMenu();
    }
}
