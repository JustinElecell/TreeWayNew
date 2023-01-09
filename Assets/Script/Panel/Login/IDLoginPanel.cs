using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EleCellLogin;
using TextLocalization;

public class IDLoginPanel : MonoBehaviour
{
    public InputField UidInput;
    public InputField PassInput;

    public Button LoginButton;
    private void OnEnable()
    {
        LoginButton.onClick.AddListener(() => {
            if (UidInput.text.Length < 9)
            {
                NoticePanel.instance.Notic("密碼或ID錯誤");
                return;
            }
            if (PassInput.text.Length < 1) 
            {
                NoticePanel.instance.Notic("密碼或ID錯誤");

                return;
                
            }

            GameServer.rlogin(UidInput.text + PassInput.text, Application.systemLanguage.ToString(), new EleCellProfileCallback(delegate (string err, string message) {

                if(err==null)
                {
                    string[] nick = GameServer.playerInfo.nick.Split('|');

                    MjSave.instance.playerName = nick[0];
                    //if (nick.Length > 1 && MjConfig.Instance.titleBack.ContainsKey(nick[1]))
                    //    MjSave.instance.playerTitle = nick[1];
                    //else
                    //    MjSave.instance.playerTitle = null;

                    MjSave.instance.playerID = GameServer.playerInfo.pid;

                    Debug.Log(MjSave.instance.playerName);
                    Debug.Log(MjSave.instance.playerID);

                    MjSave.instance.fbID = GameServer.playerInfo.fbID;
                    MjSave.instance.googleID = GameServer.playerInfo.googleID;

                    LoginManager.instance.StartGame(message);


                    LoginManager.instance.interPanel.Init(MjSave.instance.playerID);
                    this.gameObject.SetActive(false);
                    //LoginManager.instance.Load();

                }
                else
                {
                    NoticePanel.instance.Notic("密碼或ID錯誤");
                }

            }));

        });
    }
}
