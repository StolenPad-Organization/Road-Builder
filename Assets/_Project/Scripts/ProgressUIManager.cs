using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressUIManager : MonoBehaviour
{
    [SerializeField] private Image barFillImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private Sprite[] icons;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ChangeIcon(int index)
    {
        iconImage.sprite = icons[index];
    }

    public void UpdateProgressBar(float value)
    {
        barFillImage.fillAmount = value;
    }
}
