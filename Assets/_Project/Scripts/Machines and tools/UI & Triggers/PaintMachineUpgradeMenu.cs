using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintMachineUpgradeMenu : MonoBehaviour
{
    public static PaintMachineUpgradeMenu instance;

    public PaintMachineUiUpgrade capacityUpgrade;
    public PaintMachineUiUpgrade lengthUpgrade;
    public PaintMachineUiUpgrade widthUpgrade;

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        //buildMachineUpgrade.LoadUpgrade(GameManager.instance.currentZone.buildMachine.machineUpgradeType);
        CheckButtons();
    }

    public void UpgradeMachine()
    {
        GameManager.instance.currentZone.paintingMachine.SetPaintCapacity(capacityUpgrade.value);
        GameManager.instance.currentZone.paintingMachine.toolAngleController.CalculateLength(lengthUpgrade.level);
        GameManager.instance.currentZone.paintingMachine.toolAngleController.CalculateWidth(widthUpgrade.level);
    }

    public void CheckButtons()
    {
        capacityUpgrade.CheckButton();
        lengthUpgrade.CheckButton();
        widthUpgrade.CheckButton();
    }
}
