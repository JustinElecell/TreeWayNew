using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EleCellLogin;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class SettingPanel : MonoBehaviour
{
    public Button mailBoxButton;
    public GameObject MailPanel;
    public Text NickName;

    public Button GoogleAccessButton;
    public Button LogOut;

    private void Start()
    {


        mailBoxButton.onClick.AddListener(()=> {
            MailPanel.gameObject.SetActive(true);


        });


        GoogleAccessButton.onClick.AddListener(() => {
            PlayGamesPlatform.Instance.ManuallyAuthenticate((SignInStatus status) => {
                if (status == SignInStatus.Success)
                {
                    ////NoticePanel.instance.Notic("連接成功"+ PlayGamesPlatform.Instance.GetUserId());
                    //GameServer.loginGoogle(Application.systemLanguage.ToString(), new EleCellProfileCallback(delegate (string err, string message) {

                    //    Debug.Log(message);
                    //    Debug.Log(err);
                    //    if(err.Length<=0)
                    //    {
                    //        string[] nick = GameServer.playerInfo.nick.Split('|');

                    //        MjSave.instance.playerName = nick[0];


                    //        MjSave.instance.playerID = GameServer.playerInfo.pid;

                    //        Debug.Log(MjSave.instance.playerName);
                    //        Debug.Log(MjSave.instance.playerID);

                    //        MjSave.instance.fbID = GameServer.playerInfo.fbID;
                    //        MjSave.instance.googleID = GameServer.playerInfo.googleID;

                        
                    //        if (GameServer.playerInfo.googleID != "")
                    //        {
                    //            NickName.text = "Google：" + GameServer.playerInfo.nick;
                    //            GoogleAccessButton.gameObject.SetActive(false);
                    //        }
                    //        else
                    //        {
                    //            NickName.gameObject.SetActive(false);
                    //            GoogleAccessButton.gameObject.SetActive(true);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        NoticePanel.instance.Notic("連接失敗:" + err);
                    //    }

                    //}));
                }
                else
                {
                    NoticePanel.instance.Notic("連接失敗:" + status);
                }


            });

        });

        LogOut.onClick.AddListener(() => {


            SceneLoadManager.instance.Load(true,"Login");

        });


        if (GameServer.playerInfo.googleID != "")
        {
            NickName.text = "Google：" + GameServer.playerInfo.nick;
            GoogleAccessButton.gameObject.SetActive(false);
        }
        else
        {
            NickName.gameObject.SetActive(false);
            GoogleAccessButton.gameObject.SetActive(true);
        }



    }



}
