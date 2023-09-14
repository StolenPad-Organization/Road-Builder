using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    string levelToLoad;
    AsyncOperation loadOperation;

    [SerializeField] private TextMeshProUGUI loadText;
    [SerializeField] private Image loadImage;

    [SerializeField] private GameObject loadPanel;

    public void LoadLevel()
    {
        levelToLoad = EventManager.GetCurrentLevel.Invoke();
        StartCoroutine(LoadAsync());
    }

    IEnumerator LoadAsync()
    {
        loadPanel.SetActive(true);
        loadOperation = SceneManager.LoadSceneAsync(levelToLoad);
        loadOperation.allowSceneActivation = false;
        while (!loadOperation.isDone)
        {
            loadText.text = (int)(loadOperation.progress * 100) + "%";
            loadImage.fillAmount = loadOperation.progress;
            if(loadOperation.progress >= 0.9f)
                loadOperation.allowSceneActivation = true;
            yield return null;
        }
    }
}
