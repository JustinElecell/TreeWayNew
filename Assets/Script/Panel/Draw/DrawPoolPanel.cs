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

    Transform drawEndTran;
    Image drawEndPerfab;
    
    public void Init(JSONNode data)
    {
        drawEndTran = MainManager.instance.shopPanel.drawEndTran;
        drawEndPerfab = MainManager.instance.shopPanel.DrawEndPerfab;

        draw[0].onClick.AddListener(() => {
            GameServer.instance.DrawTest(1, data["Type"],new EleCellJsonCallback(delegate (string err, JSONClass message) {
                Debug.Log(message);
            }));

        });

        draw[1].onClick.AddListener(() => {
            ResetDrawEnd();
            Debug.Log(data["Type"]);
            GameServer.instance.DrawTest(10, data["Type"], new EleCellJsonCallback(delegate (string err, JSONClass message) {
                for(int i=0;i<message[0].Count;i++)
                {

                    var tmp = Instantiate(drawEndPerfab, drawEndTran);
                    if (message[0][i].AsInt>1000)
                    {

                        tmp.sprite = MainManager.instance.shopPanel.FindItem(message[0][i]).IteamImage;
                    }
                    else
                    {
                        //Debug.Log("Image/Charater/" + message[0][i].ToString());
                        tmp.sprite = Resources.Load<Sprite>("Image/Charater/" + message[0][i].AsInt.ToString());
                    }

                }
                MainManager.instance.shopPanel.DrawEndPanel.SetActive(true);

            }));
        });

        chanceButton.onClick.AddListener(() => {
            MainManager.instance.shopPanel.DrawChancePanel.Init(data);
        
        });
        TypeText.text = data["Type"];
    }

    void ResetDrawEnd()
    {
        for(int i=0; i< drawEndTran.childCount;i++)
        {
            Destroy(drawEndTran.GetChild(i).gameObject);
        }
    }

}
