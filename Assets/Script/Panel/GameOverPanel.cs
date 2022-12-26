using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverPanel : LoadBase
{
    public void LoadMenu()
    {
        StartCoroutine(LoadScene("Menu"));
    }


}
