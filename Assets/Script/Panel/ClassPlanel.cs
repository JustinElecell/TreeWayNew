using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ClassPlanel : LoadBase
{
    public Button EnterButton;

    private void Start()
    {

        EnterButton.onClick.AddListener(() => {

            StartCoroutine(LoadScene("GamePlay"));
        
        
        });
    }

}
