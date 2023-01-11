using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    public static SceneLoadManager instance;
    public bool DelectFlag = false;
    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }


    public void Load(bool flag,string loadSceneName)
    {
        StartCoroutine(LoadScene(flag,loadSceneName));

    }


    IEnumerator LoadScene(bool flag,string LoadSceneName)
    {

        var async = SceneManager.LoadSceneAsync(LoadSceneName);

        async.allowSceneActivation = false;
        while (async.progress < 0.9f)
        {
            Debug.Log("讀取中: " + async.progress * 100 + "%");
            yield return null;
        }


        Debug.Log("初始化完成");
        DelectFlag = flag;


        async.allowSceneActivation = true;

    }
}
