using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatManager : MonoBehaviour
{
    [SerializeField] private bool isMenuOpen;
    [SerializeField] private GameObject menu;
    [SerializeField] private ZoneManager currentZone;
    private bool canSkipZone;
    public void OpenCheatMenu()
    {
        if (!isMenuOpen)
            menu.gameObject.SetActive(true);
        else
            menu.gameObject.SetActive(false);

        isMenuOpen = !isMenuOpen;
    }

    public void NextStage()
    {
        currentZone = GameManager.instance.currentZone;
        switch (currentZone.zoneState)
        {
            case ZoneState.PeelingStage:
                if(currentZone.buildMachine != null)
                {
                    currentZone.ShowAsphaltMachine();
                }
                else if (currentZone.paintingMachine != null)
                {
                    currentZone.ShowPaintMachine();
                }
                else
                {
                    NextZone();
                }
                break;
            case ZoneState.BuildingStage:
                if (currentZone.paintingMachine != null)
                {
                    currentZone.ShowPaintMachine();
                }
                else
                {
                    NextZone();
                }
                break;
            case ZoneState.PaintingStage:
                NextZone();
                break;
            default:
                break;
        }
    }

    public void NextZone()
    {
        currentZone = GameManager.instance.currentZone;
        StartCoroutine(currentZone.CompleteZone());
        StartCoroutine(cleanZone());
    }

    IEnumerator cleanZone()
    {
        yield return new WaitForSeconds(1.25f); 
        currentZone.LoadZone();
    }

    public void AddMoney()
    {
        UIManager.instance.UpdateMoney(1000);
    }
}
