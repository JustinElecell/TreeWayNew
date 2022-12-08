using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClassPlanel : MonoBehaviour
{
    public Button EnterButton;

    private void Start()
    {
        Debug.Log("測試");

        EnterButton.onClick.AddListener(() => {
            Debug.Log("測試");

            StartCoroutine(LoadScene());
        
        
        });
    }


    IEnumerator LoadScene()
    {
        Debug.Log("測試");

        var async = SceneManager.LoadSceneAsync("GamePlay");

        while (!async.isDone)
        {
            yield return null;
        }

        //async.allowSceneActivation = true;
        yield return null;

    }


}
