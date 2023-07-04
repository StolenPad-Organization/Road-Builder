using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScrapeTool : MonoBehaviour
{
    public float power;
    [SerializeField] private GameObject shovelHead;
    [SerializeField] private float scaleFactor;
    public bool showing;
    

    public void UpdateShovelScale(int scaleMultiplier)
    {
        shovelHead.transform.DOScaleX(1 + (scaleFactor * scaleMultiplier), 0.5f);
    }

    public void ShowTool(bool show)
    {
        showing = show;
        if (showing)
        {
            transform.SetParent(PlayerController.instance.showPos);
            PlayerController.instance.movementController.ToggleMovementAnimation(true);
        }
        else
        {
            transform.SetParent(PlayerController.instance.hidePos);
            PlayerController.instance.movementController.ToggleMovementAnimation(true);
        }
        transform.DOLocalMove(Vector3.zero, 0.3f);
        transform.DOLocalRotate(Vector3.zero, 0.3f);
    }

    private void OnDisable()
    {
        transform.DOKill();
    }

    private void OnDestroy()
    {
        transform.DOKill();
    }
}
