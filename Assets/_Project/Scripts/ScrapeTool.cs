using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScrapeTool : MonoBehaviour
{
    public float power;
    [SerializeField] private GameObject shovelHead;
    [SerializeField] private float scaleFactor;

    public void UpdateShovelScale(int scaleMultiplier)
    {
        shovelHead.transform.DOScaleX(1 + (scaleFactor * scaleMultiplier), 0.5f);
    }
}
