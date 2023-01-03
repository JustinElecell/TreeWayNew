using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EleCellLogin;
using SimpleJSON;

public class DrawPoolPanel : MonoBehaviour
{
    public Button[] draw;
    public Button chanceButton;
    public Text TypeText;

    
    public void Init(JSONNode data)
    {
        draw[0].onClick.AddListener(() => {
            GameServer.instance.DrawTest(1, new EleCellJsonCallback(delegate (string err, JSONClass message) {
                Debug.Log(message);
            }));

        });

        draw[1].onClick.AddListener(() => {
            GameServer.instance.DrawTest(10, new EleCellJsonCallback(delegate (string err, JSONClass message) {
                Debug.Log(message);

            }));
        });

        chanceButton.onClick.AddListener(() => {
            MainManager.instance.DrawChancePanel.Init();
        
        });
        TypeText.text = data["Type"];
    }

}
