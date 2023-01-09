using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EleCellLogin;
public class CreatePanel : MonoBehaviour
{
    public Text ID, Pass;
    public Button LoginButton;
    public void Init(string id ,string pass,string message)
    {
        ID.text = id;
        Pass.text = pass;
        this.gameObject.SetActive(true);
        LoginButton.onClick.AddListener(() => {

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

        
        });
    }
}
