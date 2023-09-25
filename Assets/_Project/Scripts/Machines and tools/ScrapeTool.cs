using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum ScrapeToolType
{
    Normal,
    Angle
}

public class ScrapeTool : MonoBehaviour
{
    public ScrapeToolType scrapeToolType;
    public float power;
    [SerializeField] private GameObject shovelHead;
    [SerializeField] private float scaleFactor;
    public bool showing;
    private bool canShake;
    private Vector3 currentScale;
    private Tween shakeTween = null;
    [SerializeField] private GameObject showCollider;
    [SerializeField] private GameObject hideCollider;
    [SerializeField] private Material originalMaterial;
    [SerializeField] private Material redMaterial;
    [SerializeField] private Renderer[] renderers;
    public ToolAngleController toolAngleController;
    public WaterEffectFillController WaterEffectFillController;
    public float rbRadius;

    public void UpdateShovelScale(int scaleMultiplier)
    {
        shovelHead.transform.DOScaleX(1 + (scaleFactor * scaleMultiplier), 0.5f).OnComplete(()=> 
        {
            currentScale = shovelHead.transform.localScale;
            canShake = true;
        });
    }

    public void ShowTool(bool show)
    {
        showing = show;
        if (showing)
        {
            transform.SetParent(PlayerController.instance.showPos);
            PlayerController.instance.movementController.ToggleMovementAnimation(true);
            if (toolAngleController == null)
                PlayerController.instance.movementController.canRecoverSpeed = false;
            else
                PlayerController.instance.movementController.canRecoverSpeed = true;
            showCollider.SetActive(true);
            hideCollider.SetActive(false);

            RBManagerJobs.Instance.SwitchJob(true);
        }
        else
        {
            transform.SetParent(PlayerController.instance.hidePos);
            PlayerController.instance.movementController.ToggleMovementAnimation(true);
            PlayerController.instance.movementController.canRecoverSpeed = true;
            showCollider.SetActive(false);
            hideCollider.SetActive(true);
            if (toolAngleController != null)
                toolAngleController.OnPick();

            RBManagerJobs.Instance.SwitchJob(false);
        }
        transform.DOLocalMove(Vector3.zero, 0.3f);
        transform.DOLocalRotate(Vector3.zero, 0.3f);
    }

    public void ShakeTool()
    {
        if (shakeTween != null || !canShake) return;
        shakeTween = shovelHead.transform.DOScale(currentScale * 2f, 0.1f).OnComplete(() =>
        {
            PlayerController.instance.EmitDust();
            shakeTween = shovelHead.transform.DOScale(currentScale, 0.1f).OnComplete(() => shakeTween = null);
        });
    }

    private void OnDisable()
    {
        transform.DOKill();
        shovelHead.transform.DOKill();
    }

    private void OnDestroy()
    {
        transform.DOKill();
        shovelHead.transform.DOKill();
    }

    public void ChangeToolMaterial(bool turnRed)
    {
        if (turnRed)
        {
            foreach (var item in renderers)
            {
                item.material = redMaterial;
            }
        }
        else
        {
            foreach (var item in renderers)
            {
                item.material = originalMaterial;
            }
        }
    }
}
