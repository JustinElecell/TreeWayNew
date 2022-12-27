using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EleCellLogin;
using TextLocalization;
public class LoginManager : LoadBase
{
    public Button[] buttons;

    public static LoginManager instance;

    // 版本控制
    public string version;                      // 版本標示訊息字串
    public static int versionCode = 0;          // 版本號
    bool keyUpdated = false;

    private void Awake()
    {
        instance = this;

        buttons[0].onClick.AddListener(() => {
            GameServer.rlogin(null, Application.systemLanguage.ToString(), loginFinished);
        });
          
        buttons[1].onClick.AddListener(() => {
            StartCoroutine(LoadScene("Menu"));
        });
        
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
    public static void StartGame(string languagefile)
    {
        if (languagefile.Length > 0)
        {
            Language.appendLanguage(languagefile);
            //if (MjSave.instance.wallpaper != null)
            //    MjTextureController.instance.SetWallpaper(MjSave.instance.wallpaper);
        }

        MjSave.UpdateStats(GameServer.playerStats);

        LoginManager.instance.Load();
        //if (MjLogin.newAccount)
        //{
        //    MjLogin.newAccount = false;
        //    instance.StartCoroutine(instance.showPass());
        //    //instance.showPass2();
        //}
        //else
        //{
        //    //instance.StartCoroutine(instance.StartGameC());
        //    instance.StartGameC2();
        //}
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
