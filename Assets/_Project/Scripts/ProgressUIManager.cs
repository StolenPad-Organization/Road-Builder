using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressUIManager : MonoBehaviour
{
    [SerializeField] private Image barFillImage;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void UpdateProgressBar(float value)
    {
        barFillImage.fillAmount = value;
    }
}
