using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu_UI : MonoBehaviour
{

    private AsyncOperation asyncLoad;

    void Start()
    {
        StartCoroutine(PreloadGameScene("Game"));
    }

    public void StartGame()
    {
        if (asyncLoad != null)
        {
            asyncLoad.allowSceneActivation = true;
        }
    }

    public void BackToDeskTop()
    {
        Application.Quit();
    }

    public void StartTraining()
    {
        SceneManager.LoadScene("Training");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator PreloadGameScene(string sceneName)
    {
        asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                break;
            }
            yield return null;
        }
    }
}
