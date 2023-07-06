using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMachineUpgradeMenu : MonoBehaviour
{
    public static BuildMachineUpgradeMenu instance;

    public BuildMachineUiUpgrade buildMachineUpgrade;

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
        GameManager.instance.currentZone.buildMachine.SetUpgrade(buildMachineUpgrade.level-1);
    }

    public void CheckButtons()
    {
        buildMachineUpgrade.CheckButton();
    }
}
