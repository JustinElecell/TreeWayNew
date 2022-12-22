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
        //test: Debug.Log("初始未完成");
        async.allowSceneActivation = false;
        while (async.progress<0.9f)
        {
            Debug.Log("讀取中");
            yield return null;
            //goto test;
        }


        Debug.Log("初始化完成");
        async.allowSceneActivation = true;


        //yield return null;

    }


}
