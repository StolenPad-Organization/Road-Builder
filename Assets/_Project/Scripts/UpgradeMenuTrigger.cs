using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeMenuTrigger : MonoBehaviour
{
    private UIManager uIManager;
    void Start()
    {
        uIManager = UIManager.instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        uIManager.ShowUpgradeMenu();
        PlayerController.instance.ActivateUpgradeCamera(true);
    }

    private void OnTriggerExit(Collider other)
    {
        uIManager.HideUpgradeMenu();
        PlayerController.instance.ActivateUpgradeCamera(false);
    }
}
