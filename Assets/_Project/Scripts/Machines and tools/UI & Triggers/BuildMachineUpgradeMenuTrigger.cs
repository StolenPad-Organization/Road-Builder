using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMachineUpgradeMenuTrigger : MonoBehaviour
{
    private UIManager uIManager;
    [SerializeField] private ParticleSystem upgradeEffect;
    void Start()
    {
        uIManager = UIManager.instance;
    }

    private void OnEnable()
    {
        EventManager.PlayUpgradeEffect += PlayUpgradeEffect;
    }
    private void OnDisable()
    {
        EventManager.PlayUpgradeEffect -= PlayUpgradeEffect;
    }

    private void OnTriggerEnter(Collider other)
    {
        uIManager.ShowMachineUpgradeMenu();
    }

    private void OnTriggerExit(Collider other)
    {
        uIManager.HideMachineUpgradeMenu();
    }

    private void PlayUpgradeEffect()
    {
        upgradeEffect.Play();
    }
}
