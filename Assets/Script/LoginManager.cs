using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EleCellLogin;
public class LoginManager : LoadBase
{
    public Button[] buttons;

    public static LoginManager instance;
    private void Awake()
    {
        instance = this;

        buttons[0].onClick.AddListener(() => {
            //if(!PlayerPrefs.HasKey("log"))
            {
                GameServer.rlogin(null, Application.systemLanguage.ToString(), loginFinished);

            }
            //else
            {
                //Debug.Log(PlayerPrefs.HasKey("log"));
               
            }

            //StartCoroutine(LoadScene("Menu"));



        });
        
        
        buttons[1].onClick.AddListener(() => {


            StartCoroutine(LoadScene("Menu"));


        });
    }
    public void Load()
    {
        StartCoroutine(LoadScene("Menu"));

    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            PlayerPrefs.DeleteAll();
        }
    }
    void loginFinished(string error, string message)
	{
#if UNITY_EDITOR
		if (error != null)
			Debug.Log(error);
		if (message != null)
			Debug.Log(message);

		Debug.Log(GameServer.AccessToken);
#endif

		if (error != null)
		{

			if (error == "Device not matched")
			{
				//TextEffect.Notice(Language.text("deviceErr"));
				//parentPanel.SetActive(true);
				MjInit.instance.normalLogin.SetActive(false);
				MjInit.instance.newAccount.SetActive(true);
			}
			else if (error == "Incorrect ID/Password")
			{
				//TextEffect.Notice(Language.text("passErr"));
				//parentPanel.SetActive(true);
			}
			else
			{
				MjInit.OfflineStart("login:" + error);
			}

			return;
		}

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

        MjInit.StartGame(message);
	}

}
