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
            for(int i=0;i<5;i++)
            {
                if(MainManager.instance.skillIteams[i]!=null)
                {
                    StartCoroutine(LoadScene("GamePlay"));
                    return;
                }
            }
            NoticePanel.instance.Notic("已裝備數為0");
        
        });
    }

}
