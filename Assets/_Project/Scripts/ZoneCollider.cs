using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ZoneCollider : MonoBehaviour
{
    [SerializeField] private GameObject[] models;

    public void OpenPath()
    {
        foreach (var item in models)
        {
            item.transform.SetParent(null);
            item.transform.DOMoveY(item.transform.position.y - 50, 1.5f).SetSpeedBased(true);
        }
    }
}
