using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.NiceVibrations;

public class HapticManager : MonoBehaviour
{
    [SerializeField] private bool useHaptic;
    private void OnEnable()
    {
        EventManager.invokeHaptic += VibrateDevice;
        EventManager.SwitchHaptic += SwitchHaptic;
    }

    private void OnDisable()
    {
        EventManager.invokeHaptic -= VibrateDevice;
        EventManager.SwitchHaptic -= SwitchHaptic;
    }

    private void SwitchHaptic(bool activate)
    {
        useHaptic = activate;
    }

    private void VibrateDevice(vibrationTypes hapticTypes)
    {
        if (!useHaptic) return;
        switch (hapticTypes)
        {
            case vibrationTypes.Failure:
                MMVibrationManager.Haptic(HapticTypes.Failure);
                break;
            case vibrationTypes.HeavyImpact:
                MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
                break;
            case vibrationTypes.LightImpact:
                MMVibrationManager.Haptic(HapticTypes.LightImpact);
                break;
            case vibrationTypes.MediumImpact:
                MMVibrationManager.Haptic(HapticTypes.MediumImpact);
                break;
            case vibrationTypes.RigidImpact:
                MMVibrationManager.Haptic(HapticTypes.RigidImpact);
                break;
            case vibrationTypes.Selection:
                MMVibrationManager.Haptic(HapticTypes.Selection);
                break;
            case vibrationTypes.SoftImpact:
                MMVibrationManager.Haptic(HapticTypes.SoftImpact);
                break;
            case vibrationTypes.Success:
                MMVibrationManager.Haptic(HapticTypes.Success);
                break;
            case vibrationTypes.Warning:
                MMVibrationManager.Haptic(HapticTypes.Warning);
                break;
        }
    }
}