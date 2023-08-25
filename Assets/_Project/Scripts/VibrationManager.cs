using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MoreMountains.NiceVibrations;
using UnityEngine.UI;
using DG.Tweening;
using Stolenpad.Utils;

public class VibrationManager : SingletonMB<VibrationManager>
{
    public Image vibrationImage;
    public Sprite activeVibrationSprite;
    public Sprite desactiveVibrationSprite;
    [SerializeField] private RectTransform vibrationPanel;
    [SerializeField] private RectTransform settingsImage;
    [SerializeField] private float duration;
    private Tween tweenVibration;
    private Tween tweenSettings;
    private bool isOpened;
    private Coroutine vibrationCoroutine;
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        EventManager.SwitchHaptic.Invoke(Database.Instance.GetVibrationData());
        SetVibrationImage();
    }

    public void SetVibrationState()
    {
        if (Database.Instance.GetVibrationData())
        {
            Database.Instance.SetVibrationData(false);
        }
        else
        {
            Database.Instance.SetVibrationData(true);
        }
        SetVibrationImage();
    }

    private void SetVibrationImage()
    {
        if (Database.Instance.GetVibrationData())
        {
            vibrationImage.sprite = activeVibrationSprite;
            MMVibrationManager.Haptic(HapticTypes.HeavyImpact);
        }
        else
        {
            vibrationImage.sprite = desactiveVibrationSprite;
        }
        Invoke(nameof(CloseVibration), 0.5f);
    }

    public void OpenVibration()
    {
        if (isOpened) return;
        isOpened = true;
        tweenSettings.Kill();
        tweenVibration.Kill();
        tweenSettings = settingsImage.DORotate(Vector3.forward * 20f, duration/4f);
        tweenVibration = vibrationPanel.DOAnchorPosX(-266, duration)
            .OnComplete(() =>
            {
                //CloseVibrationInvoke();
                vibrationCoroutine = StartCoroutine(CloseVibrationCoroutine());
            });
    }

    public void CloseVibration()
    {
        if (!isOpened) return;
        isOpened = false;
        StopCoroutine(vibrationCoroutine);
        tweenSettings.Kill();
        tweenVibration.Kill();
        tweenSettings = settingsImage.DORotate(Vector3.zero, duration/4f);
        tweenVibration = vibrationPanel.DOAnchorPosX(-105, duration)
            .OnComplete(() =>
            {
                
            });
    }

    private void CloseVibrationInvoke()
    {
        Invoke(nameof(CloseVibration), 2f);
    }

    private IEnumerator CloseVibrationCoroutine()
    {
        yield return new WaitForSeconds(2f);
        CloseVibration();
    }
}
