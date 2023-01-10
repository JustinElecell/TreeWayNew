using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EleCellLogin;
using TextLocalization;
using GooglePlayGames.BasicApi;
using GooglePlayGames;

public class LoginManager : LoadBase
{
    public Button[] buttons;

    public CreatePanel CreatePanel;
    public GameObject LoginPanel;
    public GameObject MainPanel;
    public InterPanel interPanel;
    public static LoginManager instance;

    // 版本控制
    public string version;                      // 版本標示訊息字串
    public static int versionCode = 0;          // 版本號
    bool keyUpdated = false;

    private void Awake()
    {
        instance = this;

        Time.timeScale = 1f;                    // 指定時間寬度，1表示接近真實時間，0.5表示慢於真實時間...
        Application.targetFrameRate = 120;       // 指定FPS
        version = Application.version;

        GameServer.setGame("TreeWayTest", version);

    }




    private void Start()
    {
        StartCoroutine(VersionChecker("TreeWayTest"));     // 版本確認
        keyUpdated = false;                                                     // 版本更新flag
        GameServer.updatePKey(pKeyUpdated);                                    // 版本更新


        if (PlayerPrefs.HasKey("log"))
        {
            StartCoroutine(StartLoginCheck());

        }
        else
        {
            //建立帳號
            buttons[0].onClick.AddListener(() => {
                buttons[0].enabled = false;
                GameServer.rlogin(null, Application.systemLanguage.ToString(), new EleCellProfileCallback(delegate (string err, string message) {

                    MainPanel.SetActive(false);
                    //Debug.Log(GameServer.playerInfo.pass);
                    CreatePanel.Init(GameServer.playerInfo.id.ToString(), GameServer.playerInfo.pass,message);

                }));
            });

            //登入帳號
            buttons[1].onClick.AddListener(() => {

                MainPanel.SetActive(false);
                LoginPanel.SetActive(true);
                //if (RecoverPanel.instance.id.value.Length < 9) return;
                //if (RecoverPanel.instance.pass.value.Length < 1) return;

                //parentPanel.SetActive(false);
                //Loading.Show();
                //GameServer.rlogin(RecoverPanel.instance.id.value + RecoverPanel.instance.pass.value, SystemSettings.SettingData.instance.language, loginFinished);
                //StartCoroutine(LoadScene("Menu"));
            });

            // google登入
            buttons[2].onClick.AddListener(() => {
                Debug.Log("TEST");
                //PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication);

                PlayGamesPlatform.Instance.Authenticate((SignInStatus status) => {
                    if (status == SignInStatus.Success)
                    {
                        NoticePanel.instance.Notic("連接成功");
                        // Continue with Play Games Services
                    }
                    else
                    {
                        NoticePanel.instance.Notic("連接失敗:" + status);

                        // Disable your integration with Play Games Services or show a login button
                        // to ask users to sign-in. Clicking it should call
                    }


                });
            });
        }



    }

    IEnumerator StartLoginCheck()
    {
        yield return new WaitForSeconds(1f);
        GameServer.rlogin(null, Application.systemLanguage.ToString(), loginFinished);

    }


    void pKeyUpdated(string error, string message)
    {
        if (error == null)
            ElecellConnection.TcpNetwork.instance.setkey();

        keyUpdated = true;

        //if (PlayerPrefs.HasKey("log"))
        //{
        //    normalLogin.SetActive(true);
        //    newAccount.SetActive(false);
        //}
        //else
        //{
        //    normalLogin.SetActive(false);
        //    newAccount.SetActive(true);
        //}
    }

    // 連接server端並進行版本控制(同時進行音效的更新)
    IEnumerator VersionChecker(string gameID)
    {
        WWWForm form = new WWWForm();                   // PHP Post表格資料
        form.AddField("game", gameID);                  // 新增game欄位
        WWW wwwPost;                                    // 宣告WWW

        Debug.Log("測試版本");
        wwwPost = new WWW("http://www.elecell.com/versionCheck.php", form);     // 建立WWW請求

        yield return null;

        //SoundSettings.updateVolume();                                           // 更新音量
        //DarkTonic.MasterAudio.MasterAudio.ChangePlaylistByName("menuTheme");   // 改變當前播放清單(變更為主題)

        yield return wwwPost;

        // 更新版本結果
        if (wwwPost.error == null)
        {
            int currentVer = 0;
            int.TryParse(wwwPost.text, out currentVer);                          // 取得server端版本號
            Debug.Log(currentVer);
            if (currentVer > versionCode)
            {                                      // server端版本高於本機端
                //normalLogin.transform.parent.gameObject.SetActive(false);       // 關閉正常登入畫面
                //oldVerAlert.SetActive(true);                                    // 顯示版本老舊警示訊息
                //GoUtil.Delay(DownloadNewVersion, 2f);                          // 下載新版本(延遲兩秒後)
            }
        }
    }
    public void StartGame(string languagefile)
    {
        if (languagefile.Length > 0)
        {
            Language.appendLanguage(languagefile);
            //if (MjSave.instance.wallpaper != null)
            //    MjTextureController.instance.SetWallpaper(MjSave.instance.wallpaper);
        }

        MjSave.UpdateStats(GameServer.playerStats);
        interPanel.Init(MjSave.instance.playerID);
        MainPanel.SetActive(false);

    }

    public void Load()
    {
        StartCoroutine(LoadScene("Menu"));

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

        Debug.Log(GameServer.playerStats["pid"]);

        if (error != null)
		{

			if (error == "Device not matched")
			{
				//TextEffect.Notice(Language.text("deviceErr"));
				//parentPanel.SetActive(true);
				//MjInit.instance.normalLogin.SetActive(false);
				//MjInit.instance.newAccount.SetActive(true);
			}
			else if (error == "Incorrect ID/Password")
			{
				//TextEffect.Notice(Language.text("passErr"));
				//parentPanel.SetActive(true);
			}
			else
			{
				//MjInit.OfflineStart("login:" + error);
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

        StartGame(message);
	}
    public void DownloadNewVersion()
    {
        Application.OpenURL("http://www.elecell.com/getGame.php?game=CandyMahjong");
    }
}
