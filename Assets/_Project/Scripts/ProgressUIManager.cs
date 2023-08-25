using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressUIManager : MonoBehaviour
{
    [SerializeField] private Image barFillImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private Sprite[] icons;
    [SerializeField] private Image[] starImage;
    [SerializeField] private Sprite fullstar;
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
        CheckStars(value);
    }

    private void CheckStars(float value)
    {
        if (value >= 0.34f && value < 0.655f)
        {
            starImage[0].sprite = fullstar;
        }
        else if (value >= 0.655f && value < 0.95f)
        {
            starImage[0].sprite = fullstar;
            starImage[1].sprite = fullstar;
        }
        else if (value >= 0.95f)
        {
            starImage[0].sprite = fullstar;
            starImage[1].sprite = fullstar;
            starImage[2].sprite = fullstar;
        }
    }
}
