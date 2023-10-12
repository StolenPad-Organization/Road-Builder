using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintMachineUpgradeMenuTrigger : MonoBehaviour
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
        uIManager.ShowPaintMachineUpgradeMenu();
        GameManager.instance.player.ActivateUpgradeCamera(true);
    }

    private void OnTriggerExit(Collider other)
    {
        uIManager.HidePaintMachineUpgradeMenu();
        GameManager.instance.player.ActivateUpgradeCamera(false);
    }

    private void PlayUpgradeEffect()
    {
        upgradeEffect.Play();
    }
}
