using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EleCellLogin;
using SimpleJSON;

public class DrawPoolPanel : BasePanel
{
    public Button[] draw;
    public Button chanceButton;
    public Text TypeText;

    Transform drawEndTran;
    Image drawEndPerfab;

    private void OnEnable()
    {
        
    }
    public void Init(JSONNode data)
    {
        drawEndTran = MainManager.instance.shopPanel.drawEndTran;
        drawEndPerfab = MainManager.instance.shopPanel.DrawEndPerfab;

        draw[0].onClick.AddListener(() => {
            if (SaveManager.instance.saveData.coin >= 1000)
            {

                GameServer.instance.DrawTest(1, data["Type"], new EleCellJsonCallback(delegate (string err, JSONClass message)
                {

                    for (int i = 0; i < MainManager.instance.itemPanel.Panels.Length; i++)
                    {
                        DerstorChild(MainManager.instance.itemPanel.Panels[i]);
                    }

                    DerstorChild(drawEndTran);
                    DerstorChild(MainManager.instance.charaterPanel.createrTran);

                    for (int i = 0; i < message[0].Count; i++)
                    {
                        var tmp = Instantiate(drawEndPerfab, drawEndTran);
                        if (message[0][i].AsInt > 1000)
                        {

                            tmp.sprite = MainManager.instance.shopPanel.FindItem(message[0][i]).IteamImage;
                        }
                        else
                        {
                            //Debug.Log("Image/Charater/" + message[0][i].ToString());
                            tmp.sprite = Resources.Load<Sprite>("Image/Charater/" + message[0][i].AsInt.ToString());
                        }

                    }
                    MainManager.instance.itemPanel.Init();
                    MainManager.instance.charaterPanel.Init();

                    SaveManager.instance.SaveCoin(SaveManager.instance.saveData.coin - 1000);
                    MainManager.instance.shopPanel.ResetPanel();
                    MainManager.instance.charaterPanel.ResetPanel();
                    MainManager.instance.shopPanel.DrawEndPanel.SetActive(true);

                }));
            }
            else
            {
                MainManager.instance.noticePanel.Notic("金幣不足");
            }

        });

        draw[1].onClick.AddListener(() => {

            if(SaveManager.instance.saveData.coin>=10000)
            {

                GameServer.instance.DrawTest(10, data["Type"], new EleCellJsonCallback(delegate (string err, JSONClass message) {

                    Debug.Log("長度" + message[0].Count);

                    for (int i = 0; i < MainManager.instance.itemPanel.Panels.Length; i++)
                    {
                        DerstorChild(MainManager.instance.itemPanel.Panels[i]);
                    }
                    DerstorChild(drawEndTran);
                    DerstorChild(MainManager.instance.charaterPanel.createrTran);

                    for (int i = 0; i < message[0].Count; i++)
                    {

                        var tmp = Instantiate(drawEndPerfab, drawEndTran);
                        if (message[0][i].AsInt > 1000)
                        {

                            tmp.sprite = MainManager.instance.shopPanel.FindItem(message[0][i]).IteamImage;
                        }
                        else
                        {
                            //Debug.Log("Image/Charater/" + message[0][i].ToString());
                            tmp.sprite = Resources.Load<Sprite>("Image/Charater/" + message[0][i].AsInt.ToString());
                        }

                    }
                    MainManager.instance.itemPanel.Init();
                    MainManager.instance.charaterPanel.Init();

                    SaveManager.instance.SaveCoin(SaveManager.instance.saveData.coin - 1000);
                    MainManager.instance.shopPanel.ResetPanel();
                    MainManager.instance.charaterPanel.ResetPanel();
                    MainManager.instance.shopPanel.DrawEndPanel.SetActive(true);
                }));
            }
            else
            {
                MainManager.instance.noticePanel.Notic("金幣不足");
            }

        });

        chanceButton.onClick.AddListener(() => {
            MainManager.instance.shopPanel.DrawChancePanel.Init(data);
        
        });
        TypeText.text = data["Type"];
    }


}
