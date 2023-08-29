using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadFirstScene : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LoadFirstSceneDelay());
    }
    private IEnumerator LoadFirstSceneDelay()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("Level Select");
    }
}
