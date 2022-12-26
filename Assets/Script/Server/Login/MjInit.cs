using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using EleCellLogin;
using TextLocalization;

/****************************************************************************
 * 
 * 建立獨體
 * 設定時間寬度
 * 加載音效
 * 設定語係
 * 設定標題畫面
 * Start()
 * 版本確認
 * 
 * 2018/3/14 Coinmouse註解
 ****************************************************************************/

/// <summary>
/// 執行遊戲初始化之類別，進行版本控制、登入相關操作及語言切換之用。
/// </summary>
public class MjInit : MonoBehaviour
{
    public static MjInit instance;              // 獨體 (獨體應該對外封閉修改)，且此獨體只使用於內部，等於是多餘的，但外部卻需要使用StartGame (static)的函示

    // 版本控制
    public string version;                      // 版本標示訊息字串
    public static int versionCode = 0;          // 版本號



    private string languageFile;                // 語言檔案


    public GameObject offlineAlert;             // 伺服器關閉警示視窗
    public GameObject oldVerAlert;              // 版本老舊警示視窗

    public GameObject normalLogin;              // 正常登入視窗
    public GameObject newAccount;               // 建立帳號視窗
    public GameObject passPanel;                // 建立帳號後視窗(拍照)



    public Texture[] logoMats;                  // 遊戲標題，多語言版

    bool keyUpdated = false;
    public static bool audioLoaded = false;

    void Awake()
    {
        Debug.Log("mj init awake");

        instance = this;                        // 建立獨體

        Time.timeScale = 1f;                    // 指定時間寬度，1表示接近真實時間，0.5表示慢於真實時間...
        Application.targetFrameRate = 120;       // 指定FPS

        // 加載音效場景，並疊加在主場景中
        //if (!audioLoaded)
        //{
        //    if (!SceneManager.GetSceneByName("audio").isLoaded)
        //        SceneManager.LoadScene("audio", LoadSceneMode.Additive);
        //    audioLoaded = true;
        //}

        // 檢查系統語言並進行設定
        //if (Application.systemLanguage == SystemLanguage.Japanese)
        //    SettingData.instance.language = "jp";
        //else if (Application.systemLanguage == SystemLanguage.Chinese || Application.systemLanguage == SystemLanguage.ChineseTraditional || Application.systemLanguage == SystemLanguage.ChineseSimplified)
        //    SettingData.instance.language = "zh-tw";
        //else
        //    SettingData.instance.language = "en";

        // 讀取紀錄
        //MjSave.load();

        // 放入標題圖片
        //if (SettingData.instance.language == "zh-tw")
        //{
        //    logoTexture.mainTexture = logoMats[0];
        //}
        //else if (SettingData.instance.language == "en")
        //{
        //    logoTexture.mainTexture = logoMats[1];
        //}
        //else if (SettingData.instance.language == "jp")
        //{
        //    logoTexture.mainTexture = logoMats[2];
        //}
        //else
        //{
        //    logoTexture.mainTexture = logoMats[0];
        //}

        version = Application.version;

        // 設定遊戲版本
        GameServer.setGame("TreeWayTest", version);
        //versionLabel.SetText("Version " + version);


        // 廣告key
        //string appkey = "1735289";          // 設定Apple廣告ID

#if UNITY_ANDROID
        //appkey = "1735288";                  // 設定android廣告ID

        /*
		if (!TapjoyUnity.Tapjoy.IsConnected)
			TapjoyUnity.Tapjoy.Connect();
		*/
#endif

        // 廣告控制器初始化(未使用廣告必須關閉)


    }

    void OnIapInited(string error, string message)
    {
    }

    void Start()
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
                normalLogin.transform.parent.gameObject.SetActive(false);       // 關閉正常登入畫面
                oldVerAlert.SetActive(true);                                    // 顯示版本老舊警示訊息
                //GoUtil.Delay(DownloadNewVersion, 2f);                          // 下載新版本(延遲兩秒後)
            }
        }
    }

    // 設定廣告、設定語言、確認玩家登入方式
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


    public static void OfflineStart(string error)
    {
        //Loading.Hide();
        instance.offlineAlert.SetActive(true);
    }

    // 顯示玩家UID及密碼視窗
    IEnumerator showPass()
    {
        //Loading.Hide();
        passPanel.SetActive(true);
        yield return null;
        while (passPanel.activeSelf) yield return null;
        yield return null;
        
        StartGameC2();

    }

    // 設定金流(儲值種類)，並進入HUD介面
    private void StartGameC2()
    {
        //IapController.instance.Init(P5UI_210_Mall.OnTransactionCompleted);

        LoadLevel();
        //Loading.Hide();
    }

    //void OnGUI(){		GUI.Label(new Rect(0,0,100,100),itemLoading.ToString() + recipesLoading.ToString());	}

    void OnDisable()
    {
        // AdsController.hideBanner();
        instance = null;
    }

    // 開啟HUD介面(正式進入)
    public void LoadLevel()
    {
        //if (GameServer.instance.firstStartFlag)
        //{
        //    hud_first.Init();

        //}
        //else
        //{
        //    hud.Init();
        //    Destroy(gameObject);


        //}
    }

    public void CloseFirstHUD()
    {
        //hud.Init();
        //Destroy(hud_first.gameObject);
        Destroy(gameObject);

    }
    public void Restart()
    {
        //GameServer.Release();
        //TextEffect.Release();
        instance = null;
        Application.LoadLevel(2);
    }

    public void DownloadNewVersion()
    {
        Application.OpenURL("http://www.elecell.com/getGame.php?game=CandyMahjong");
    }
}