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

        EnterButton.onClick.AddListener(() => {

            StartCoroutine(LoadScene());
        
        
        });
    }


    IEnumerator LoadScene()
    {

        var async = SceneManager.LoadSceneAsync("GamePlay");

        while (!async.isDone)
        {
            yield return null;
        }

        //async.allowSceneActivation = true;
        yield return null;

    }


}
