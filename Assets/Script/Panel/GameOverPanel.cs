using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverPanel : MonoBehaviour
{
    public void LoadMenu()
    {
        StartCoroutine(LoadScene());
    }
    IEnumerator LoadScene()
    {
        Debug.Log("測試");

        var async = SceneManager.LoadSceneAsync("Menu");

        while (!async.isDone)
        {
            yield return null;
        }

        //async.allowSceneActivation = true;
        yield return null;

    }

}
