using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadFirstScene : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LoadFirstSceneDelay());
    }
    private IEnumerator LoadFirstSceneDelay()
    {
        yield return new WaitForSeconds(0.5f);
        EventManager.loadOpeningScene?.Invoke();
    }
}
