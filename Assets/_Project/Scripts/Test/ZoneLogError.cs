using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneLogError : MonoBehaviour
{
    private void OnEnable()
    {
        Debug.LogError("Player Reached Level 2 Zone 3");
    }
}
