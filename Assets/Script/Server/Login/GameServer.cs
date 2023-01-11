using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using ElecellConnection;
using SimpleJSON;

using GameSecurity;
using UnityEngine.Events;
using GooglePlayGames.BasicApi;
using GooglePlayGames;

namespace EleCellLogin
{


    public class GameServer : MonoBehaviour
    {

        private static int pictureCacheDay = 14;

        private static float loginTimeOut = 15f;

        private static GameServer _instance;

        static bool needInit = true;

        static string gameVersion;
        public bool firstStartFlag=false;
        public static GameServer instance
        {
            get
            {
                if (needInit)
                {
                    GameObject GO = new GameObject();
                    _instance = GO.AddComponent<GameServer>();
                    GO.name = "EleCell GameServer";
                    DontDestroyOnLoad(GO);
                    needInit = false;


#if UNITY_ANDROID && !UNITY_EDITOR
					if (PlayerPrefs.HasKey("push")){
						gcm = PlayerPrefs.GetString("push");
					}

#endif

#if UNITY_EDITOR
                    _instance.imei = "10002";
#endif

#if UNITY_IOS
					if (PlayerPrefs.HasKey("push")){
						apn = PlayerPrefs.GetString("push");
					}

					_instance.imei = "";
#endif

                    if (_instance.imei == null) _instance.imei = "";
                }
                return _instance;
            }
        }

        void OnDestroy()
        {
            needInit = true;
        }

#if UNITY_ANDROID
        private static string gcm = "";
#endif

#if UNITY_IOS
		private static string apn = "";
#endif

        private JSONClass stageScoreJSON;
        private List<Ranking[]> leaderboardCache = new List<Ranking[]>();
        private JSONClass leaderboardCacheIndex = new JSONClass();

        private Mail[] mailCache = new Mail[0];
        private QnA[] QnACache = new QnA[0];
        private ExtraLevel[] ExtraLvlCache = new ExtraLevel[0];

        public static int stageHiScore(string key)
        {
            if (_instance.stageScoreJSON == null) return 0;
            return _instance.stageScoreJSON[key].AsInt;
        }

        public static Ranking[] readLeaderboard(string key)
        {
            if (_instance.leaderboardCacheIndex[key] == null) return null;
            return _instance.leaderboardCache[_instance.leaderboardCacheIndex[key]["leaderboard"].AsInt];
        }

        public static Ranking[] readLeaderboardTop(string key)
        {
            if (_instance.leaderboardCacheIndex[key] == null) return null;
            return _instance.leaderboardCache[_instance.leaderboardCacheIndex[key]["leaderboardTop"].AsInt];
        }

        public static Ranking[] readLeaderboardFriends(string key)
        {
            if (_instance.leaderboardCacheIndex[key] == null) return null;
            return _instance.leaderboardCache[_instance.leaderboardCacheIndex[key]["leaderboardFriends"].AsInt];
        }

        public static int readMyRank(string key)
        {
            if (_instance.leaderboardCacheIndex[key] == null) return 0;
            return _instance.leaderboardCacheIndex[key]["myRank"].AsInt;
        }

        public static int readMyScore(string key)
        {
            if (_instance.leaderboardCacheIndex[key] == null) return 0;
            return _instance.leaderboardCacheIndex[key]["myScore"].AsInt;
        }

        public static long readMyLongScore(string key)
        {
            if (_instance.leaderboardCacheIndex[key] == null) return 0;
            return _instance.leaderboardCacheIndex[key]["myScore"].AsLong;
        }
        public static double readMyDoubleScore(string key)
        {
            if (_instance.leaderboardCacheIndex[key] == null) return 0;
            return _instance.leaderboardCacheIndex[key]["myScore"].AsDouble;
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        //public static void GetIMEI(System.Action<bool> callback = null)
        //{
        //    AndroidJavaObject androidUtil = null;
        //    AndroidJavaObject activityContext = null;
        //    var state = AN_ActivityCompat.CheckSelfPermission(SA.Android.Manifest.AMM_ManifestPermission.READ_PHONE_STATE);
        //    if (state == AN_PackageManager.PermissionState.Denied)
        //    {
        //        AN_PermissionsUtility.TryToResolvePermission(SA.Android.Manifest.AMM_ManifestPermission.READ_PHONE_STATE, (result) =>
        //        {
        //            if (result)
        //                GetIMEI(callback);
        //            else
        //                callback?.Invoke(false);
        //        });
        //        return;
        //    }
        //    else if (state == AN_PackageManager.PermissionState.Granted)
        //    {
        //        using (AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        //        {
        //            activityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
        //        }
        //        using (AndroidJavaClass pluginClass = new AndroidJavaClass("com.elecell.util.Main"))
        //        {
        //            if (pluginClass != null)
        //            {
        //                androidUtil = pluginClass.CallStatic<AndroidJavaObject>("instance");
        //                androidUtil.Call("SetContext", activityContext);
        //                _instance.imei = androidUtil.Call<string>("GetIMEI");
        //                callback?.Invoke(true);
        //                return;
        //            }
        //        }
        //    }
        //    callback?.Invoke(false);
        //}
#endif

        public static string IMEI { get { return _instance.imei; } }
        string imei;

        public static Mail[] Mail
        {
            get { return _instance.mailCache; }
        }

        public static QnA[] QnA
        {
            get { return _instance.QnACache; }
        }
        public static ExtraLevel[] ExtraLevel
        {
            get { return _instance.ExtraLvlCache; }
        }

        static string gameID;
        static string _accessToken;

        public static string GameID
        {
            get { return gameID; }
        }

        public static string AccessToken
        {
            get { return _accessToken; }
        }

        private PlayerInfo _pinfo;
        public static PlayerInfo playerInfo
        {
            get { return _instance._pinfo; }
        }

        private PlayerFacebookInfo _pFbInfo;//玩家Facebook資料
        public static PlayerFacebookInfo PlayerFacebookInfo { get { return _instance._pFbInfo; } }

        private PlayerGooglePlayGameInfo _pGpgInfo;//玩家google資料
        public static PlayerGooglePlayGameInfo PlayerGoogleInfo { get { return _instance._pGpgInfo; } }

        private JSONClass _pStats = new JSONClass();
        public static JSONClass playerStats
        {
            get { return _instance._pStats; }
        }

        private static Vector2 profilePicSize = new Vector2(256f, 256f);
        public static void SetProfilePicSize(Vector2 size)
        {
            profilePicSize = size;
        }

        private bool profilePicReady = false;
        private Texture2D _profilePic;
        public static Texture2D ProfilePic
        {
            get
            {
                if (_instance.profilePicReady)
                    return _instance._profilePic;

                if (_instance._pFbInfo.avatarTexture != null)
                    return _instance._pFbInfo.avatarTexture;

                if (_instance._pGpgInfo.avatarTexture != null)
                    return _instance._pGpgInfo.avatarTexture;

                return null;
            }
        }

        private Reward[] _checkin;
        public static Reward[] CheckinList { get { return _instance._checkin; } }

        private Mission _lastMission;
        public static Mission LastMission { get { return _instance._lastMission; } }

        private Battle _lastBattle;
        public static Battle LastBattle { get { return _instance._lastBattle; } }

        private Pet _lastPet;
        public static Pet LastPet { get { return _instance._lastPet; } }

        private Pet[] _petList;
        public static Pet[] PetList { get { return _instance._petList; } }

        private Build[] _buildList;
        public static Build[] BuildList { get { return _instance._buildList; } }

        private Hero _lastHero;
        public static Hero LastHero { get { return _instance._lastHero; } }

        private Hero[] _heroList;
        public static Hero[] HeroList { get { return _instance._heroList; } }

        private Deck[] _deckList;
        public static Deck[] DeckList { get { return _instance._deckList; } }

        private Friend[] _friendList;
        public static Friend[] FriendList { get { return _instance._friendList; } }

        private Friend[] _inviteList;
        public static Friend[] InviteList { get { return _instance._inviteList; } }

        private Friend[] _suggestedFriend;
        public static Friend[] SuggestedFriend { get { return _instance._suggestedFriend; } }

        private Friend[] _friendSearched;
        public static Friend[] FriendSearched { get { return _instance._friendSearched; } }

        private ExchangeRate[] _exchangeRate;
        public static ExchangeRate[] ExchangeRate { get { return _instance._exchangeRate; } }

        private JSONClass _itemList;
        public static JSONClass ItemList { get { return _instance._itemList; } }

        private JSONClass _galleryList;
        public static JSONClass GalleryList { get { return _instance._galleryList; } }

        private JSONClass _priceList;
        public static JSONClass PriceList { get { return _instance._priceList; } }

        private JSONClass _alchemyRecipes;
        public static JSONNode AlchemyRecipes { get { return _instance._alchemyRecipes["array"]; } }

        private JSONClass _bReq = new JSONClass();
        public static JSONClass BuildReq { get { return _instance._bReq; } }

        private JSONClass _achieve;
        public static JSONClass Achieve { get { return _instance._achieve; } }

        private JSONClass _dailyDone;
        public static JSONClass DailyDone { get { return _instance._dailyDone; } }

        private Trophy[] _trophy;
        public static Trophy[] Trophy { get { return _instance._trophy; } }

        private DailyMission[] _daily;
        public static DailyMission[] Daily { get { return _instance._daily; } }

        private Guild[] guildSearched = new Guild[0];
        public static Guild[] GuildSearched { get { return _instance.guildSearched; } }

        private Guild guildDetails;
        public static Guild GuildDetails { get { return _instance.guildDetails; } }

        private EleCellProfileCallback trophyAlert;
        public static void AddTrophyListener(EleCellProfileCallback listener)
        {
            _instance.trophyAlert = listener;
        }

        public static void setGame(string id, string version)
        {
            gameID = id;

            gameVersion = version;
            Debug.Log(id+" : "+version);
        }

        EleCellProfileCallback accountBindcallback;

        private bool loginInitiated = false;

        // quick start game
        public static void qlogin(string nick, string language, EleCellProfileCallback callback)
        {
            if (gameID == null)
            {
                callback("GameServer.login : Game ID not set", null);
                return;
            }
            Debug.Log(gameID);
            lang = language;
            instance.loginInitiated = true;
            instance.qloginWWW(null, null, nick, language, null, callback);
        }
        // recover account
        public static void rlogin(string pass, string language, EleCellProfileCallback callback)
        {
            Debug.Log(gameID);
            if (gameID == null)
            {
                callback("GameServer.login : Game ID not set", null);
                return;
            }

            lang = language;
            instance.loginInitiated = true;
            Debug.Log(lang);
            instance.qloginWWW(null, null, "Player", language, pass, callback);

        }

        public static void rloginExample(string pass, string language, UnityAction<string , string> callback)
        {
            Debug.Log(gameID);
            if (gameID == null)
            {
                callback("GameServer.login : Game ID not set", null);
                return;
            }

            lang = language;
            instance.loginInitiated = true;
            Debug.Log(lang);
            //instance.qloginWWW(null, null, "Player", language, pass, callback);

        }

        //        public static void Release()
        //        {
        //            if (_instance == null) return;

        //            //if (SA_FB.IsLoggedIn) SA_FB.LogOut();
        //#if UNITY_ANDROID
        //            if (AN_GoogleSignIn.GetLastSignedInAccount() != null)
        //            {
        //                AN_GoogleSignInOptions gso = new AN_GoogleSignInOptions.Builder(AN_GoogleSignInOptions.DEFAULT_GAMES_SIGN_IN).Build();
        //                AN_GoogleSignInClient client = AN_GoogleSignIn.GetClient(gso);
        //                client.SignOut(() => { });
        //            }
        //#endif
        //            Destroy(_instance.gameObject);
        //            _instance = null;
        //        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //PlayerPrefs.DeleteAll();

                //GameServer.instance.DrawTest(new EleCellProfileCallback(delegate (string err, string message) { }));


            }
        }


        private static bool FBinit = false;
        //private static bool GoogleInit = false;
        static string lang;
        private int _choice;
        private EleCellProfileCallback loginCB;
        private EleCellProfileCallback iosGameCenterCB;

        private static bool gameCenterConnected = false;
        public static bool GameCenterReady { get { return gameCenterConnected; } }
        public static void SetLang(string l)
        {
            lang = l;
        }

        public static void AddGameCenterListener(EleCellProfileCallback cb)
        {
            _instance.iosGameCenterCB = cb;
        }

        public static void loginFB(string language, EleCellProfileCallback callback)
        {
            loginFB(language, -1, callback);
        }


        public static void loginFB(string language, int choice, EleCellProfileCallback callback)
        {
            if (gameID == null)
            {
                callback("GameServer.login : Game ID not set", null);
                return;
            }

            _instance._choice = choice;
            lang = language;
            _instance.loginCB = callback;

            instance.loginInitiated = true;

            //if (!SA_FB.IsInitialized) SA_FB.Init(_instance.OnFacebookInit);
            //else _instance.OnFacebookInit();
        }

        public static void loginGoogle(string language, EleCellProfileCallback callback)
        {
            loginGoogle(language, -1, callback);
        }

        public static void loginGoogle(string language, int choice, EleCellProfileCallback callback)
        {
#if UNITY_IOS
			callback("GameServer.loginGoogle : iOS not supported yet",null);
			return;
#endif

            if (gameID == null)
            {
                callback("GameServer.login : Game ID not set", null);
                return;
            }

            _instance._choice = choice;
            lang = language;
            _instance.loginCB = callback;

            instance.loginInitiated = true;

            //if (instance.gUserInfo!=null){
            //	instance.StartCoroutine(instance.loginWWW(null,instance.gUserInfo.playerId,instance.gUserInfo.name,language,choice, callback));
            //	return;
            //}

            //#if Obsolete
            //            if (!GoogleInit)
            //            {
            //                GoogleInit = true;
            //                GooglePlayConnection.ActionConnectionResultReceived += instance.OnGoogleConnectionComplete;
            //                GooglePlayManager.ActionFriendsListLoaded += instance.OnFriendListLoaded;
            //                //	GooglePlayManager.ActionAchievementsLoaded +=  instance.OnAchievmentsLoaded; 
            //            }

            //            GooglePlayConnection.Instance.Connect();
            //#endif
            //可以使用GMS系統

            _instance._pGpgInfo = new PlayerGooglePlayGameInfo
            {
                id = PlayGamesPlatform.Instance.GetUserId(),
                name = PlayGamesPlatform.Instance.GetUserDisplayName(),
                avatarTexture = null
            };
            
            _instance.qloginWWW(null, _instance._pGpgInfo.id, _instance._pGpgInfo.name, language, null, _instance.loginCB);

        }

        //初始化GooglePlayGame
        void OnGooglePlayGameInit()
        {
            ////初始化登入系統
            //AN_GoogleSignInOptions.Builder builder = new AN_GoogleSignInOptions.Builder(AN_GoogleSignInOptions.DEFAULT_GAMES_SIGN_IN);
            //builder.RequestId();
            //builder.RequestEmail();
            //builder.RequestProfile();
            ////
            //AN_GoogleSignInClient client = AN_GoogleSignIn.GetClient(builder.Build());
            //client.SignIn((gameResult) =>
            //{
            //    if (gameResult.IsSucceeded)
            //    {
            //        if (gameResult.Account.GetId() == "editor_id")
            //            loginCB("GooglePlayerGameFail", null);
            //        else
            //        {
            //            _pGpgInfo = new PlayerGooglePlayGameInfo
            //            {
            //                id = gameResult.Account.GetId(),
            //                name = gameResult.Account.GetDisplayName(),
            //                avatarTexture = null
            //            };
            //            qloginWWW(null, _pGpgInfo.id, _pGpgInfo.name, lang, null, loginCB);
            //        }
            //    }
            //    else if (gameResult.StatusCode == AN_CommonStatusCodes.SIGN_IN_REQUIRED)
            //    {
            //        builder = new AN_GoogleSignInOptions.Builder(AN_GoogleSignInOptions.DEFAULT_SIGN_IN);
            //        builder.RequestId();
            //        builder.RequestEmail();
            //        builder.RequestProfile();
            //        //
            //        client = AN_GoogleSignIn.GetClient(builder.Build());
            //        client.SignIn((result) =>
            //        {
            //            if (result.IsSucceeded)
            //            {
            //                if (result.Account.GetId() == "editor_id")
            //                    loginCB("GooglePlayerGameFail", null);
            //                else
            //                {
            //                    _pGpgInfo = new PlayerGooglePlayGameInfo
            //                    {
            //                        id = result.Account.GetId(),
            //                        name = result.Account.GetDisplayName(),
            //                        avatarTexture = null
            //                    };
            //                    qloginWWW(null, _pGpgInfo.id, _pGpgInfo.name, lang, null, loginCB);
            //                }
            //            }
            //            else loginCB("GooglePlayerGameFail", null);
            //        });
            //    }
            //    else loginCB("GooglePlayerGameFail", null);
            //});
        }

        //初始化Facebook
        void OnFacebookInit()
        {
            //if (!SA_FB.IsLoggedIn)
            //{
            //    //準備進行登入
            //    SA_FB.Login((result) =>
            //    {
            //        //取得玩家資訊
            //        if (result.IsSucceeded) SA_FB.GetLoggedInUserInfo(FacebookHandleResult);
            //        else loginCB("FacebookFail", null);
            //    });
            //}
            //else SA_FB.GetLoggedInUserInfo(FacebookHandleResult);
        }

        //取得Facebook帳號資訊
        //void FacebookHandleResult(SA_FB_GetUserResult result)
        //{
        //    if (result.IsSucceeded)
        //    {
        //        _pFbInfo = new PlayerFacebookInfo
        //        {
        //            id = result.User.Id,
        //            name = result.User.Name,
        //            avatarTexture = null
        //        };
        //        //將已經有進行FB登入的FB好友申請加入為遊戲內的好友
        //        StartCoroutine(GetFacebookFriend());
        //        StartCoroutine(profileAvatarDownload(_pFbInfo.id));
        //        qloginWWW(_pFbInfo.id, null, _pFbInfo.name, lang, null, loginCB);
        //    }
        //    else loginCB("FacebookFail", null);
        //}

        public static void bindFB(EleCellProfileCallback callback)
        {
            //if (gameID == null) { callback("GameServer.bindFB : Game ID not set", null); return; }
            //if (_accessToken == null) { callback("GameServer.bindFB : No access token", null); return; }

            //_instance.accountBindcallback = callback;
            //if (!SA_FB.IsInitialized)
            //{
            //    //初始化Facebook
            //    SA_FB.Init(_instance.BindFBInit);
            //}
            //else _instance.BindFBInit();
        }

        void BindFBInit()
        {
            ////登入Facebook
            //if (SA_FB.IsLoggedIn)
            //{
            //    //取得玩家資訊
            //    SA_FB.GetLoggedInUserInfo(playerInfo =>
            //    {
            //        if (playerInfo.IsSucceeded)
            //        {
            //            _instance._pFbInfo = new PlayerFacebookInfo
            //            {
            //                id = playerInfo.User.Id,
            //                name = playerInfo.User.Name,
            //                avatarTexture = null
            //            };
            //            _instance.bindFbCB(null, playerInfo.User.Id);
            //        }
            //        else accountBindcallback("FacebookFail", null);
            //    });
            //}
            //else
            //{
            //    SA_FB.Login((loginResult) =>
            //    {
            //        if (loginResult.IsSucceeded)
            //        {
            //            //取得玩家資訊
            //            SA_FB.GetLoggedInUserInfo(playerInfo =>
            //            {
            //                if (playerInfo.IsSucceeded)
            //                {
            //                    _instance._pFbInfo = new PlayerFacebookInfo
            //                    {
            //                        id = playerInfo.User.Id,
            //                        name = playerInfo.User.Name,
            //                        avatarTexture = null
            //                    };
            //                    _instance.bindFbCB(null, playerInfo.User.Id);
            //                }
            //                else accountBindcallback("FacebookFail", null);
            //            });
            //        }
            //        else accountBindcallback("FacebookFail", null);
            //    });
            //}
        }



        void bindFbCB(string error, string message)
        {
            if (error == null)
            {
                TcpForm form = new TcpForm();
                form.AddField("game", gameID);
                form.AddField("accessToken", AccessToken);
                form.AddField("fb", message);

                NetworkManager.instance.Query("updateFB", form, new GenericCallback<JSONClass>(delegate (JSONClass json)
                {
                    if (json["error"] != null)
                    {
                        accountBindcallback("FacebookFail", null);
                    }
                    else
                    {
                        _pinfo.fbID = json["fbID"];
                        StartCoroutine(GetFacebookFriend());
                        StartCoroutine(profileAvatarDownload(message));
                        accountBindcallback(null, json.ToString());
                    }
                }));
            }
            else accountBindcallback("FacebookFail", null);
        }

        void OnFriendUpdated(string err, string message)
        {
        }

        public static void bindG(EleCellProfileCallback callback)
        {
            //if (gameID == null) { callback("GameServer.bindG : Game ID not set", null); return; }
            //if (_accessToken == null) { callback("GameServer.bindG : No access token", null); return; }

            //_instance.accountBindcallback = callback;

            //AN_GoogleSignInOptions.Builder builder = new AN_GoogleSignInOptions.Builder(AN_GoogleSignInOptions.DEFAULT_GAMES_SIGN_IN);
            //builder.RequestId();
            //builder.RequestEmail();
            //builder.RequestProfile();
            //AN_GoogleSignInClient client = AN_GoogleSignIn.GetClient(builder.Build());

            //client.SignIn((gameResult) =>
            //{
            //    if (gameResult.IsSucceeded)
            //    {
            //        if (gameResult.Account.GetId() == "editor_id")
            //            _instance.bindGCB("GooglePlayerGameFail", null);
            //        else
            //        {
            //            //載入玩家資訊
            //            _instance._pGpgInfo = new PlayerGooglePlayGameInfo
            //            {
            //                id = gameResult.Account.GetId(),
            //                name = gameResult.Account.GetDisplayName(),
            //                avatarTexture = null
            //            };
            //            _instance.bindGCB(null, gameResult.Account.GetId());
            //        }
            //    }
            //    else if (gameResult.StatusCode == AN_CommonStatusCodes.SIGN_IN_REQUIRED)
            //    {
            //        builder = new AN_GoogleSignInOptions.Builder(AN_GoogleSignInOptions.DEFAULT_SIGN_IN);
            //        builder.RequestId();
            //        builder.RequestEmail();
            //        builder.RequestProfile();
            //        client = AN_GoogleSignIn.GetClient(builder.Build());

            //        client.SignIn((result) =>
            //        {
            //            if (result.IsSucceeded)
            //            {
            //                if (result.Account.GetId() == "editor_id")
            //                    _instance.bindGCB("GooglePlayerGameFail", null);
            //                else
            //                {
            //                    //載入玩家資訊
            //                    _instance._pGpgInfo = new PlayerGooglePlayGameInfo
            //                    {
            //                        id = result.Account.GetId(),
            //                        name = result.Account.GetDisplayName(),
            //                        avatarTexture = null
            //                    };
            //                    _instance.bindGCB(null, result.Account.GetId());
            //                }
            //            }
            //            else _instance.bindGCB("GooglePlayerGameFail", null);
            //        });
            //    }
            //    else _instance.bindGCB("GooglePlayerGameFail", null);
            //});
        }

        void bindGCB(string error, string message)
        {
            if (error == null)
            {
                TcpForm form = new TcpForm();
                form.AddField("game", gameID);
                form.AddField("accessToken", _accessToken);
                form.AddField("google", message);

                NetworkManager.instance.Query("updateG", form,
                    new GenericCallback<JSONClass>(delegate (JSONClass json)
                    {
                        if (json["error"] != null)
                        {
                            accountBindcallback(json["error"], null);
                        }
                        else
                        {
                            accountBindcallback(null, json.ToString());
                        }
                    })
                );
            }
        }

        IEnumerator updateGFriendWWW(string[] friendList, EleCellProfileCallback callback)
        {
            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            while (_accessToken == null) yield return null;
            form.AddField("accessToken", _accessToken);
            form.AddField("param", string.Join(",", friendList));
            form.AddField("action", "google");

            NetworkManager.instance.Query("friend", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json)
                {

                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        callback(null, json.ToString());
                    }
                }));
        }

        IEnumerator profileAvatarDownload(string fbid)
        {
#if !UNITY_WEBPLAYER && !UNITY_WP8
            System.IO.FileInfo finfo = new System.IO.FileInfo(Application.persistentDataPath + "/" + fbid + ".png");

            if (finfo != null && finfo.Exists && PlayerPrefs.HasKey(fbid))
            {
                System.DateTime lastTime = System.DateTime.Parse(PlayerPrefs.GetString(fbid), System.Globalization.DateTimeFormatInfo.CurrentInfo);
                System.TimeSpan timePassed = System.DateTime.Now - lastTime;
                double dayPassed = timePassed.TotalDays;

                if (dayPassed <= (double)pictureCacheDay)
                {
                    byte[] picFile = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/" + fbid + ".png");
                    _pFbInfo.avatarTexture = new Texture2D((int)profilePicSize.x, (int)profilePicSize.y);
                    _pFbInfo.avatarTexture.LoadImage(picFile);
                    yield break;
                }
                else
                {
                    System.IO.File.Delete(Application.persistentDataPath + "/" + fbid + ".png");
                }
            }
#endif
            //if (SA_FB.IsInitialized)
            //{
            //    SA_FB.CurrentUser.GetProfileImage(SA_FB_ProfileImageSize.normal, (result) =>
            //    {
            //        if (result != null)
            //        {
            //            _pFbInfo.avatarTexture = result;
            //            System.IO.File.WriteAllBytes(Application.persistentDataPath + "/" + fbid + ".png", _pFbInfo.avatarTexture.EncodeToPNG());
            //            PlayerPrefs.SetString(fbid, System.DateTime.Now.ToString(System.Globalization.DateTimeFormatInfo.CurrentInfo));
            //        }
            //    });
            //}
            //            WWW imageRequest = new WWW("http://graph.facebook.com/" + fbid + "/picture?width=" + ((int)profilePicSize.x).ToString() + "&height=" + ((int)profilePicSize.y).ToString());
            //            yield return imageRequest;
            //#if !UNITY_WEBPLAYER && !UNITY_WP8
            //            if (imageRequest.error == null)
            //            {
            //                _pFbInfo.avatarTexture = imageRequest.texture;
            //                System.IO.File.WriteAllBytes(Application.persistentDataPath + "/" + fbid + ".png", _pFbInfo.avatarTexture.EncodeToPNG());
            //                PlayerPrefs.SetString(fbid, System.DateTime.Now.ToString(System.Globalization.DateTimeFormatInfo.CurrentInfo));
            //            }
            //#endif
        }

        public static void inviteFB(string _title, EleCellFbInviteCallback callback)
        {
            //Debug.Log(_title);
            instance.fbInviteCB = callback;

            //if (!SA_FB.IsLoggedIn)
            //{
            //    //loginFB(lang,instance.loginFbInGame);
            //    _instance.loginCB = _instance.OnReadyToInvite;
            //    debugText = _title;

            //    if (!SA_FB.IsInitialized) SA_FB.Init(_instance.OnFacebookInit);
            //    else _instance.OnFacebookInit();

            //    return;
            //}

            _instance.OnReadyToInvite(null, null);
        }

        void OnReadyToInvite(string error, string message)
        {
            if (error == null) StartCoroutine(GetFacebookFriend());
        }

        IEnumerator GetFacebookFriend()
        {
            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            yield return new WaitWhile(() => _accessToken == null);//等待Server回傳accessToken
            form.AddField("accessToken", _accessToken);
            form.AddField("action", "fb");
            JSONClass jsonParam = new JSONClass();
            int isGetFriends = 0;//0:wait, 1:succeeded, 2:filed
            //SA_FB.GraphAPI.GetFriends(-1, (friendsResult) =>
            //{
            //    if (friendsResult.IsSucceeded)
            //    {
            //        for (int i = 0; i < friendsResult.Users.Count; i++)
            //        {
            //            jsonParam["param"][i] = friendsResult.Users[i].Id;
            //        }
            //        if (jsonParam["param"].Count > 0)
            //        {
            //            form.AddField(jsonParam);
            //            isGetFriends = 1;
            //        }
            //        else isGetFriends = 2;
            //    }
            //});
            yield return new WaitWhile(() => isGetFriends <= 0);
            if (isGetFriends == 1)
            {
                NetworkManager.instance.Query("friend", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json)
                {
                    if (json["error"] == null)
                    {
                        //成功
                    }
                }));
            }
        }

        void qloginWWW(string fb, string google, string nick, string lang, string pass, EleCellProfileCallback callback)
        {
            if (!loginInitiated)
                return;

            loginInitiated = false;

            string device = SystemInfo.deviceUniqueIdentifier;

#if UNITY_IOS && !UNITY_EDITOR
			//IOSNotificationController.instance.ApplicationIconBadgeNumber(-1);
			//device = UnityEngine.iOS.Device.advertisingIdentifier;
#endif

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("gameVer", gameVersion);
            form.AddField("lang", lang);
            form.AddField("model", SystemInfo.deviceModel);

            if (pass != null)
            {
                form.AddField("id", pass.Substring(0, 9));
                form.AddField("pass", pass.Substring(9));
            }
            else if (PlayerPrefs.HasKey("log"))
            {
                int pid = 100000001;
                string id = "";
                try
                {
                    id = postData.Decrypt(PlayerPrefs.GetString("log")).Substring(0, 9);
                }
                catch
                {
                }
                int.TryParse(id, out pid);
                form.AddField("id", pid);
            }
            form.AddField("nick", nick);
            form.AddField("device", device);

            // if (choice > -1) form.AddField("choice",choice);
#if UNITY_ANDROID
            form.AddField("gcm", gcm);
            //form.AddField("imei", imei);
#endif
#if UNITY_IOS
			form.AddField("apn",apn);
#endif
            if (fb != null)
            {
                form.AddField("fb", fb);
            }
            else if (google != null)
                form.AddField("google", google);

            //form.AddField("dump",dump.FinalizeR());

            NetworkManager.instance.Query("qlogin", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json)
                {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                          //Debug.Log("Json[build]:\n" + json["build"].ToString());

                        _accessToken = json["accessToken"];
                        _accessToken = _accessToken.Replace("\"", "");

                        if (_secondsPassed != null) _secondsPassed.release();

                        _secondsPassed = new LogTime(1f, 1f);
                        if (System.DateTime.TryParseExact(json["time"], "MM-dd-yyyy HH:mm:ss", System.Globalization.DateTimeFormatInfo.CurrentInfo, System.Globalization.DateTimeStyles.None, out _instance._serverTime))
                        {
                        }
                        else
                        {
                            _serverTime = System.DateTime.Now;
                            callback("Failed to parse server time", json["time"]);
                            return;
                        }

                        Debug.Log(_instance._serverTime);
                        _pinfo = new PlayerInfo();
                        _pinfo.id = json["pid"].AsInt;
                        _pinfo.pid = ((string)json["pid"]).Replace("\"", "");
                        if (_pinfo.pid.Length == 9)
                            _pinfo.pid = _pinfo.pid.Substring(0, 3) + '.' + _pinfo.pid.Substring(3, 3) + '.' + _pinfo.pid.Substring(6, 3);
                        _pinfo.nick = ((string)json["nick"]).Replace("\"", "");
                        _pinfo.googleID = ((json["googleID"] == null) ? "" : ((string)json["googleID"]).Replace("\"", ""));
                        _pinfo.fbID = ((json["fbID"] == null) ? "" : ((string)json["fbID"]).Replace("\"", ""));
                        _pinfo.checkinRow = json["checkinRow"].AsInt;
                        _pinfo.checkinTotal = json["checkinTotal"].AsInt;
                        _pinfo.canCheckin = (_pinfo.checkinRow == 0) ? true : (json["canCollect"].AsFloat > 0f);

                        _pinfo.friendCount = json["frdCount"].AsInt;
                        _pinfo.pass = (string)json["pass"];

                        _pinfo.nextCheckin = _pinfo.checkinRow + 1;
                        if (_pinfo.nextCheckin > 7) _pinfo.nextCheckin = 1;

                        // pet
                        _petList = new Pet[json["pet"].Count];
                        for (int i = 0; i < _petList.Length; i++)
                        {
                            _petList[i].id = json["pet"][i]["petID"].AsInt;
                            _petList[i].type = json["pet"][i]["type"];
                            _petList[i].rank = json["pet"][i]["rank"].AsInt;
                            _petList[i].gender = json["pet"][i]["gender"].AsInt;
                            _petList[i].status = json["pet"][i]["status"].AsInt;
                            _petList[i].name = json["pet"][i]["name"];
                            _petList[i].level = json["pet"][i]["level"].AsInt;
                            _petList[i].xp = json["pet"][i]["xp"].AsInt;
                            _petList[i].cooldown1 = GameTime(json["pet"][i]["cooldown1"]);
                            _petList[i].cooldown2 = GameTime(json["pet"][i]["cooldown2"]);
                        }

                        //gallery
                        _galleryList = (JSONClass)json["gallery"];

                        //item
                        _itemList = (JSONClass)json["item"];

                        Debug.Log(_itemList.ToString());
                        //itemprice
                        _priceList = (JSONClass)json["itemprice"];

                        // exchange rate				
                        _exchangeRate = new ExchangeRate[json["exchange"].Count];
                        for (int i = 0; i < _exchangeRate.Length; i++)
                        {
                            _exchangeRate[i].from = json["exchange"][i]["fromCur"];
                            _exchangeRate[i].to = json["exchange"][i]["toCur"];
                            _exchangeRate[i].pay = json["exchange"][i]["pay"].AsDouble;
                            _exchangeRate[i].receive = json["exchange"][i]["receive"].AsDouble;
                            _exchangeRate[i].remarks = json["exchange"][i]["remarks"];
                        }

                        //checkin 
                        _checkin = new Reward[json["checkin"].Count];
                        for (int i = 0; i < _checkin.Length; i++)
                        {
                            string[] reward = ((string)json["checkin"][i]["reward"]).Split(',');
                            if (reward.Length > 2)
                            {
                                _checkin[i].rewardType = int.Parse(reward[0]);
                                _checkin[i].amount = double.Parse(reward[2]);

                                if (_checkin[i].rewardType == 0)
                                {
                                    _checkin[i].rewardEnum = int.Parse(reward[1]);
                                }
                                else
                                {
                                    _checkin[i].item = reward[1];
                                }
                            }
                        }

                        //trophyDef
                        _trophy = new Trophy[json["trophy"].Count];
                        for (int i = 0; i < _trophy.Length; i++)
                        {
                            _trophy[i].TrophyName = json["trophy"][i]["name"];
                            _trophy[i].category = json["trophy"][i]["category"].AsInt;
                            _trophy[i].field = json["trophy"][i]["field"];
                            _trophy[i].count = json["trophy"][i]["count"].AsInt;
                            _trophy[i].collected = (json["trophy"][i]["collected"].AsInt > 0);

                            string r = json["trophy"][i]["reward"];
                            string[] rs = null;
                            if (r != null && r.Length > 0)
                            {
                                rs = r.Split(',');
                            }

                            if (rs != null && rs.Length > 2)
                            {
                                _trophy[i].reward.rewardType = int.Parse(rs[0]);
                                if (_trophy[i].reward.rewardType == 0)
                                    _trophy[i].reward.rewardEnum = int.Parse(rs[1]);
                                else
                                    _trophy[i].reward.item = rs[1];
                                _trophy[i].reward.amount = int.Parse(rs[2]);
                            }
                            else
                            {
                                _trophy[i].reward.rewardType = -1;
                            }
                        }

                        //achieve
                        if (json["achieve"].Count > 0)
                            _achieve = (JSONClass)json["achieve"];
                        else
                            _achieve = new JSONClass();

                        //dailyDef
                        _daily = new DailyMission[json["daily"].Count];
                        for (int i = 0; i < _daily.Length; i++)
                        {
                            _daily[i].TrophyName = json["daily"][i]["name"];
                            _daily[i].category = json["daily"][i]["category"].AsInt;
                            _daily[i].field = json["daily"][i]["field"];
                            _daily[i].count = json["daily"][i]["count"].AsInt;
                            _daily[i].collected = (json["daily"][i]["collected"].AsInt > 0);

                            string r = json["daily"][i]["reward"];
                            string[] rs = null;
                            if (r != null && r.Length > 0)
                            {
                                rs = r.Split(',');
                            }

                            if (rs != null && rs.Length > 2)
                            {
                                _daily[i].reward.rewardType = int.Parse(rs[0]);
                                if (_daily[i].reward.rewardType == 0)
                                    _daily[i].reward.rewardEnum = int.Parse(rs[1]);
                                else
                                    _daily[i].reward.item = rs[1];
                                _daily[i].reward.amount = int.Parse(rs[2]);
                            }
                            else
                            {
                                _daily[i].reward.rewardType = -1;
                            }
                        }

                        //dailyDone
                        if (json["dailyDone"].Count > 0)
                            _dailyDone = (JSONClass)json["dailyDone"];
                        else
                            _dailyDone = new JSONClass();

                        //recipes

                        if (json["recipes"].Count > 0)
                        {
                            _alchemyRecipes = new JSONClass();
                            _alchemyRecipes["array"] = json["recipes"];
                        }

                        //hero
                        _heroList = new Hero[json["hero"].Count];
                        for (int i = 0; i < _heroList.Length; i++)
                        {
                            _heroList[i].id = json["hero"][i]["hid"].AsInt;
                            _heroList[i].type = json["hero"][i]["type"];
                            _heroList[i].rank = json["hero"][i]["rank"].AsInt;
                            _heroList[i].status = json["hero"][i]["status"].AsInt;
                            _heroList[i].level = json["hero"][i]["level"].AsInt;
                            _heroList[i].xp = json["hero"][i]["xp"].AsInt;

                            _heroList[i].stats = new int[6];
                            _heroList[i].skill = new int[6];
                            _heroList[i].stats[0] = json["hero"][i]["sta1"].AsInt;
                            _heroList[i].stats[1] = json["hero"][i]["sta2"].AsInt;
                            _heroList[i].stats[2] = json["hero"][i]["sta3"].AsInt;
                            _heroList[i].stats[3] = json["hero"][i]["sta4"].AsInt;
                            _heroList[i].stats[4] = json["hero"][i]["sta5"].AsInt;
                            _heroList[i].stats[5] = json["hero"][i]["sta6"].AsInt;
                            _heroList[i].skill[0] = json["hero"][i]["sk1"].AsInt;
                            _heroList[i].skill[1] = json["hero"][i]["sk2"].AsInt;
                            _heroList[i].skill[2] = json["hero"][i]["sk3"].AsInt;
                            _heroList[i].skill[3] = json["hero"][i]["sk4"].AsInt;
                            _heroList[i].skill[4] = json["hero"][i]["sk5"].AsInt;
                            _heroList[i].skill[5] = json["hero"][i]["sk6"].AsInt;
                        }

                        //deck
                        _deckList = new Deck[json["deck"].Count];
                        for (int i = 0; i < _deckList.Length; i++)
                        {
                            _deckList[i].id = json["deck"][i]["deckID"].AsInt;
                            _deckList[i].hero = json["deck"][i]["hid"].AsInt;

                            _deckList[i].pets = new int[6];

                            _deckList[i].pets[0] = json["deck"][i]["pet1"].AsInt;
                            _deckList[i].pets[1] = json["deck"][i]["pet2"].AsInt;
                            _deckList[i].pets[2] = json["deck"][i]["pet3"].AsInt;
                            _deckList[i].pets[3] = json["deck"][i]["pet4"].AsInt;
                            _deckList[i].pets[4] = json["deck"][i]["pet5"].AsInt;
                            _deckList[i].pets[5] = json["deck"][i]["pet6"].AsInt;
                        }

                        //friend
                        _friendList = new Friend[json["friend"].Count];
                        for (int i = 0; i < _friendList.Length; i++)
                        {
                            _friendList[i].id = json["friend"][i]["pid"].AsInt;
                            _friendList[i].fbid = json["friend"][i]["fbid"];
                            _friendList[i].nick = json["friend"][i]["nick"];
                            _friendList[i].level = json["friend"][i]["playerLvl"].AsInt;
                            _friendList[i].friendship = json["friend"][i]["friendship"].AsInt;
                            _friendList[i].cooldown = GameTime(json["friend"][i]["cooldown"]);
                            _friendList[i].lastSeen = GameTime(json["friend"][i]["loginTime"]);

                            if (json["friend"][i]["type"] != null)
                            {
                                _friendList[i].hero.type = json["friend"][i]["type"];
                                _friendList[i].hero.level = json["friend"][i]["level"].AsInt;
                            }

                            _friendList[i].pets = new Pet[6];

                            for (int k = 0; k < 6; k++)
                            {
                                if (json["friend"][i]['p' + (k + 1).ToString()] != null)
                                {
                                    _friendList[i].pets[k].type = json["friend"][i]['p' + (k + 1).ToString()];
                                    _friendList[i].pets[k].level = json["friend"][i]['p' + (k + 1).ToString() + 'l'].AsInt;
                                }
                            }
                        }

                        _inviteList = new Friend[json["invite"].Count];
                        for (int i = 0; i < _inviteList.Length; i++)
                        {
                            _inviteList[i].id = json["invite"][i]["pid"].AsInt;
                            _inviteList[i].fbid = json["invite"][i]["fbid"];
                            _inviteList[i].nick = json["invite"][i]["nick"];
                            _inviteList[i].level = json["invite"][i]["playerLvl"].AsInt+1;
                            _inviteList[i].lastSeen = GameTime(json["invite"][i]["loginTime"]);


                            if (json["invite"][i]["type"] != null)
                            {
                                _inviteList[i].hero.type = json["invite"][i]["type"];
                                _inviteList[i].hero.level = json["invite"][i]["level"].AsInt;
                            }

                            _inviteList[i].pets = new Pet[6];

                            for (int k = 0; k < 6; k++)
                            {
                                if (json["invite"][i]['p' + (k + 1).ToString()] != null)
                                {
                                    _inviteList[i].pets[k].type = json["invite"][i]['p' + (k + 1).ToString()];
                                    _inviteList[i].pets[k].level = json["invite"][i]['p' + (k + 1).ToString() + 'l'].AsInt;
                                }
                            }
                        }

                        //build
                        _buildList = new Build[json["build"].Count];

                        for (int i = 0; i < _buildList.Length; i++)
                        {
                            int j = json["build"][i]["pos"].AsInt;

                            _buildList[j].id = json["build"][i]["bID"].AsInt;
                            _buildList[j].type = json["build"][i]["type"];
                            _buildList[j].rank = json["build"][i]["rank"].AsInt;
                            _buildList[j].status = json["build"][i]["status"].AsInt;
                            _buildList[j].task = json["build"][i]["task"].AsInt;
                            _buildList[j].upgrade = json["build"][i]["upgrade"].AsInt;

                            if (_buildList[j].task > 0)
                            {
                                _buildList[j].rID = json["build"][i]["rID"].AsInt;
                                _buildList[j].multiple = json["build"][i]["multiple"].AsInt;
                                _buildList[j].taskFinish = GameTime(json["build"][i]["taskFinish"]);
                            }
                            if (_buildList[j].upgrade > 0)
                            {
                                _buildList[j].brID = json["build"][i]["brID"].AsInt;
                                _buildList[j].upgradeFinish = GameTime(json["build"][i]["upgradeFinish"]);
                            }


                            _buildList[j].cooldown = GameTime(json["build"][i]["cooldown"]);

                        }

                        //buildReq
                        if (json["buildReq"].Count > 0)
                        {
                            _bReq = new JSONClass();
                            _bReq["array"] = json["buildReq"];
                        }

                        if (json["guild"] != null)
                        {
                            _pinfo.guildID = json["guild"]["guildID"].AsInt;
                            _pinfo.guildRole = json["guild"]["role"].AsInt;
                            _pinfo.guildName = json["guild"]["guildName"];
                            _pinfo.guildLevel = json["guild"]["level"].AsInt;
                            _pinfo.guildWeekly = json["guild"]["weekly"].AsInt;
                            _pinfo.guildTime = GameTime(json["guild"]["updateTime"]);
                        }

                        // stats
                        _pStats = (JSONClass)json["stats"];
                        Debug.Log(_pStats["coin"].AsInt);
                        SaveManager.instance.SaveCoin(_pStats["coin"].AsInt);


                        Debug.Log(_pStats);
                        // lang callback
                        if (json["lang"] == null)
                            callback(null, "");
                        else
                            callback(null, ((string)json["lang"]).Replace("\\\"", "\""));

                        if (_pinfo.fbID.Length > 0)
                            StartCoroutine(profileAvatarDownload(_pinfo.fbID));
#if UNITY_ANDROID
                        //如果有使用GooglePlayGame登入, 則彈跳出動態小視窗
                        //if (!string.IsNullOrEmpty(_pinfo.googleID))
                        //{
                        //    if (AN_GoogleSignIn.GetLastSignedInAccount() != null)
                        //    {
                        //        AN_GoogleSignInOptions.Builder builder = new AN_GoogleSignInOptions.Builder(AN_GoogleSignInOptions.DEFAULT_GAMES_SIGN_IN);
                        //        AN_GoogleSignInClient client = AN_GoogleSignIn.GetClient(builder.Build());
                        //        client.SignOut(() => { });

                        //        builder.RequestId();
                        //        builder.RequestEmail();
                        //        builder.RequestProfile();
                        //        client = AN_GoogleSignIn.GetClient(builder.Build());
                        //        //
                        //        client.SilentSignIn((result) =>
                        //        {
                        //            if (result.IsSucceeded)
                        //            {
                        //                _pGpgInfo = new PlayerGooglePlayGameInfo
                        //                {
                        //                    id = result.Account.GetId(),
                        //                    name = result.Account.GetDisplayName(),
                        //                    avatarTexture = null
                        //                };

                        //                AN_GamesClient _gamesClient = AN_Games.GetGamesClient();
                        //                _gamesClient.SetViewForPopups(AN_MainActivity.Instance);
                        //                _gamesClient.SetGravityForPopups(AN_Gravity.TOP | AN_Gravity.CENTER_HORIZONTAL);
                        //            }
                        //        });
                        //    }
                        //}
#endif
                        string id = _pinfo.pid.Replace(".", "");
                        id += EncryptionHash.Md5Sum(UnityEngine.Random.Range(1, 999999999).ToString());
                        id = postData.Encrypt(id);
                        PlayerPrefs.SetString("log", id);
                    }
                })
            );
        }

        public static void updatePKey(EleCellProfileCallback callback)
        {
            instance.StartCoroutine(instance.updatePKeyWWW(callback));
        }

        IEnumerator updatePKeyWWW(EleCellProfileCallback callback)
        {
            WWW wwwGet = new WWW("http://www.elecell.com/api/pkey.php");

            float timer = 0f;

            while (!wwwGet.isDone && timer < 8f)
            {
                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            if (!wwwGet.isDone)
            {
                callback("Time out", null);
            }
            else if (wwwGet.error != null)
            {
                callback(wwwGet.error, null);
            }
            else
            {
                RSAEncrypt.init(postData.Decrypt(wwwGet.text));
                callback(null, null);
            }
        }

        public static void checkInObsolete(EleCellProfileCallback callback)
        {
            if (gameID == null)
            {
                callback("GameServer.checkIn : Game ID not set", null);
                return;
            }
            if (_accessToken == null)
            {
                callback("GameServer.checkIn : No access token", null);
                return;
            }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);

            NetworkManager.instance.Query("checkInObsolete", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        _instance._pinfo.checkinRow = json["checkinRow"].AsInt;
                        _instance._pinfo.checkinTotal = json["checkinTotal"].AsInt;
                        _instance._pinfo.checkinRewardO = json["checkinReward"].AsDouble;
                        _instance._pinfo.canCheckin = false;

                        _instance._pinfo.nextCheckin = _instance._pinfo.checkinRow + 1;
                        if (_instance._pinfo.nextCheckin > 7) _instance._pinfo.nextCheckin = 1;

#if UNITY_EDITOR
                        callback(null, json.ToString());
#else
						callback(null, null);
#endif
                    }
                })
            );
        }

        public static void checkInMJ(int multiplier, EleCellProfileCallback callback)
        {
            if (gameID == null) { callback("GameServer.checkIn : Game ID not set", null); return; }
            if (_accessToken == null) { callback("GameServer.checkIn : No access token", null); return; }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            form.AddField("multiplier", multiplier);

            NetworkManager.instance.Query("checkinMJ", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        _instance._pinfo.checkinRow = json["checkinRow"].AsInt;
                        _instance._pinfo.checkinTotal = json["checkinTotal"].AsInt;
                        _instance._pinfo.checkinRewardO = json["checkinReward"].AsDouble;
                        _instance._pinfo.canCheckin = false;

                        _instance._pinfo.nextCheckin = _instance._pinfo.checkinRow + 1;
                        if (_instance._pinfo.nextCheckin > 7) _instance._pinfo.nextCheckin = 1;

#if UNITY_EDITOR
                        callback(null, json.ToString());
#else
						callback(null, null);
#endif
                    }
                })
            );
        }

        public static void checkIn(EleCellProfileCallback callback, bool doublePay)
        {
            if (gameID == null)
            {
                callback("GameServer.checkIn : Game ID not set", null);
                return;
            }
            if (_accessToken == null)
            {
                callback("GameServer.checkIn : No access token", null);
                return;
            }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            if (doublePay) form.AddField("doublePay", "1");

            NetworkManager.instance.Query("checkin", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        _instance._pinfo.checkinRow = json["checkinRow"].AsInt;
                        _instance._pinfo.checkinTotal = json["checkinTotal"].AsInt;
                        _instance._pinfo.checkinReward = (string)json["checkinReward"];
                        _instance._pinfo.canCheckin = (_instance._pinfo.checkinRow == 0) ? true : (json["canCollect"].AsFloat > 0);

                        string[] reward = _instance._pinfo.checkinReward.Split(',');
                        if (reward.Length > 2)
                        {
                            if (reward[0] == "0")
                            {
                                string currency = "xp";
                                if (reward[1] == "0") currency = "coin";
                                else if (reward[1] == "1") currency = "credit";
                                else if (reward[1] == "2") currency = "gem";
                                else if (reward[1] == "3") currency = "tickC";
                                else if (reward[1] == "4") currency = "tickR";
                                else if (reward[1] == "5") currency = "tickG";

                                int amt = 0;
                                if (int.TryParse(reward[2], out amt))
                                {
                                    _instance._pStats[currency].AsInt = _instance._pStats[currency].AsInt + amt;
                                }
                            }
                            else if (reward[0] == "1")
                            {
                                int qty = 0;
                                if (int.TryParse(reward[2], out qty))
                                {
                                    _instance._itemList[reward[1]].AsInt = _instance._itemList[reward[1]].AsInt + qty;
                                }
                            }
                        }

#if UNITY_EDITOR
                        callback(null, json.ToString());
#else
						callback(null, null);
#endif
                    }
                })
            );
        }

        public static void getMail(EleCellProfileCallback callback)
        {
            if (gameID == null)
            {
                callback("GameServer.getMail : Game ID not set", null);
                return;
            }
            if (_accessToken == null)
            {
                callback("GameServer.getMail : No access token", null);
                return;
            }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);

            NetworkManager.instance.Query("getMail", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {

                        _instance.mailCache = new Mail[json["array"].Count];

                        for (int i = 0; i < _instance.mailCache.Length; i++)
                        {
                            _instance.mailCache[i].mid = json["array"][i]["mid"].AsInt;
                            _instance.mailCache[i].sender = json["array"][i]["sender"];
                            _instance.mailCache[i].senderID = json["array"][i]["senderID"].AsInt;

                            _instance.mailCache[i].subject = json["array"][i]["subject"];
                            _instance.mailCache[i].message = json["array"][i]["message"];
                            _instance.mailCache[i].url = json["array"][i]["url"];

                            _instance.mailCache[i].isRead = (json["array"][i]["isRead"].AsInt > 0);
                            _instance.mailCache[i].isCollected = (json["array"][i]["isCollected"].AsInt > 0);

                            _instance.mailCache[i].date = json["array"][i]["createTime"];

                            string r = json["array"][i]["reward"];
                            string[] rs = null;
                            if (r != null && r.Length > 0)
                            {
                                rs = r.Split(',');
                            }

                            if (rs != null && rs.Length > 2)
                            {
                                _instance.mailCache[i].reward.rewardType = int.Parse(rs[0]);
                                if (_instance.mailCache[i].reward.rewardType == 0)
                                    _instance.mailCache[i].reward.rewardEnum = int.Parse(rs[1]);
                                else
                                    _instance.mailCache[i].reward.item = rs[1];
                                _instance.mailCache[i].reward.amount = double.Parse(rs[2]);
                            }
                            else
                            {
                                _instance.mailCache[i].reward.rewardType = -1;
                            }
                        }
#if UNITY_EDITOR
                        callback(null, json.ToString());
#else
						callback(null, null);
#endif
                    }
                })
            );
        }

        public static void updateMail(int MailID, MailAction action, EleCellProfileCallback callback)
        {
            if (gameID == null)
            {
                callback("GameServer.updateMail : Game ID not set", null);
                return;
            }
            if (_accessToken == null)
            {
                callback("GameServer.updateMail : No access token", null);
                return;
            }

            if (action == MailAction.SetRead)
            {
                for (int i = 0; i < _instance.mailCache.Length; i++)
                    if (_instance.mailCache[i].mid == MailID) _instance.mailCache[i].isRead = true;
            }
            if (action == MailAction.Delete)
            {
                int k = 0;
                Mail[] tmp = new Mail[_instance.mailCache.Length - 1];
                for (int i = 0; i < _instance.mailCache.Length; i++)
                    if (_instance.mailCache[i].mid != MailID)
                    {
                        tmp[k] = _instance.mailCache[i];
                        k++;
                    }

                _instance.mailCache = tmp;
            }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            form.AddField("mid", MailID.ToString());
            form.AddField("action", action.ToString());

            NetworkManager.instance.Query("updateMail", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        if (action == MailAction.CollectAttachment)
                        {

                            for (int i = 0; i < _instance.mailCache.Length; i++)
                                if (_instance.mailCache[i].mid == MailID)
                                {
                                    _instance.mailCache[i].isCollected = true;

                                    if (_instance.mailCache[i].reward.rewardType == 1)
                                    {
                                        double rewardCount = 1;
                                        for (int k = 0; k < _instance.mailCache.Length; k++)
                                        {
                                            if (_instance.mailCache[k].mid == MailID)
                                            {
                                                rewardCount = _instance.mailCache[k].reward.amount; ;
                                                break;
                                            }
                                        }
                                        _instance._itemList[_instance.mailCache[i].reward.item].AsInt = _instance._itemList[_instance.mailCache[i].reward.item].AsInt + (int)rewardCount;
                                    }
                                    else if (_instance.mailCache[i].reward.rewardType == 2)
                                    {
                                        _instance._lastPet = new Pet();
                                        _instance._lastPet.id = json["petID"].AsInt;
                                        _instance._lastPet.type = json["type"];
                                        _instance._lastPet.rank = json["rank"].AsInt;
                                        _instance._lastPet.gender = json["gender"].AsInt;
                                        _instance._lastPet.status = json["status"].AsInt;
                                        _instance._lastPet.name = json["name"];
                                        _instance._lastPet.level = json["level"].AsInt;
                                        _instance._lastPet.xp = json["xp"].AsInt;

                                        if (_instance._petList.Length == 0)
                                        {
                                            _instance._petList = new Pet[1];
                                            _instance._petList[0] = _instance._lastPet;
                                        }
                                        else
                                        {
                                            Pet[] tempList = new Pet[_instance._petList.Length + 1];
                                            tempList[0] = _instance._lastPet;
                                            for (int k = 1; k < tempList.Length; k++)
                                                tempList[k] = _instance._petList[k - 1];

                                            _instance._petList = tempList;
                                        }
                                    }
                                    break;
                                }
                        }
#if UNITY_EDITOR
                        callback(null, json.ToString());
#else
						callback(null, null);
#endif
                    }
                })
            );
        }

        public static void sendMail(int receiver, string subject, string message, int coin, EleCellProfileCallback callback)
        {
            if (gameID == null)
            {
                callback("GameServer.updateMail : Game ID not set", null);
                return;
            }
            if (_accessToken == null)
            {
                callback("GameServer.updateMail : No access token", null);
                return;
            }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            form.AddField("receiver", receiver);
            form.AddField("subject", subject);
            form.AddField("message", message);
            form.AddField("coin", coin);

            NetworkManager.instance.Query("sendMail", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        instance._pStats["coin"].AsInt = instance._pStats["coin"].AsInt - Mathf.CeilToInt((float)coin * 1.05f);

                        callback(null, json["receiver"]);
                    }
                })
            );
        }

        public static void sendMail(int receiver, string subject, string message, long coin, EleCellProfileCallback callback)
        {
            if (gameID == null)
            {
                callback("GameServer.updateMail : Game ID not set", null);
                return;
            }
            if (_accessToken == null)
            {
                callback("GameServer.updateMail : No access token", null);
                return;
            }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            form.AddField("receiver", receiver);
            form.AddField("subject", subject);
            form.AddField("message", message);
            form.AddField("coin", coin);

            NetworkManager.instance.Query("sendMail", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        instance._pStats["coin"].AsInt = instance._pStats["coin"].AsInt - Mathf.CeilToInt((float)coin * 1.05f);

                        callback(null, json["receiver"]);
                    }
                })
            );
        }

        public static void getQnA(EleCellProfileCallback callback)
        {
            if (gameID == null)
            {
                callback("GameServer.getQnA : Game ID not set", null);
                return;
            }
            if (_accessToken == null)
            {
                callback("GameServer.getQnA : No access token", null);
                return;
            }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);

            NetworkManager.instance.Query("QnAquery", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        _instance.QnACache = new QnA[json["array"].Count];

                        for (int i = 0; i < _instance.QnACache.Length; i++)
                        {
                            _instance.QnACache[i].message = json["array"][i]["message"];
                            _instance.QnACache[i].isQuery = (json["array"][i]["QnA"].AsInt == 0);
                        }

#if UNITY_EDITOR
                        callback(null, json.ToString());
#else
						callback(null, null);
#endif
                    }
                })
            );
        }

        public static void sendQnA(string message, EleCellProfileCallback callback)
        {
            if (gameID == null) { callback("GameServer.sendQnA : Game ID not set", null); return; }
            if (_accessToken == null) { callback("GameServer.sendQnA : No access token", null); return; }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            form.AddField("hash", EncryptionHash.Md5Sum(gameID + _accessToken));
            form.AddField("message", message);

            NetworkManager.instance.Query("QnAquery", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                        callback(json["error"], null);
                    else
                        callback(null, null);
                })
            );
        }

        public static void readChat(EleCellJsonCallback callback, ChatChannel channel, int newerThan = 0)
        {
            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);

            switch (channel)
            {
                case ChatChannel.System:
                    form.AddField("channel", 0);
                    break;
                case ChatChannel.World:
                    form.AddField("channel", 1);
                    break;
                case ChatChannel.Guild:
                    if (playerInfo.guildID == 0)
                    {
                        Debug.LogError("Not in guild to read guild chat");
                        return;
                    }

                    form.AddField("channel", playerInfo.guildID);
                    break;
                default:
                    if (playerInfo.guildID == 0)
                        form.AddField("channel", "0,1");
                    else
                        form.AddField("channel", "0,1," + playerInfo.guildID.ToString());
                    break;
            }

            form.AddField("newerThan", newerThan);

            NetworkManager.instance.Query("chat", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        callback(null, json);
                    }
                })
            );
        }

        public static void sendChat(string msg, ChatChannel channel, EleCellProfileCallback callback)
        {
            if (channel == ChatChannel.All || channel == ChatChannel.System)
            {
                Debug.LogError("Invalid channel for sending chat");
                return;
            }

            if (channel == ChatChannel.Guild && playerInfo.guildID == 0)
            {
                Debug.LogError("Not in guild to send guild chat");
                return;
            }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            form.AddField("msg", msg);

            if (channel == ChatChannel.Guild)
                form.AddField("channel", playerInfo.guildID);
            else
                form.AddField("channel", 1);

            NetworkManager.instance.Query("chat", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                        callback(json["error"], null);
                    else
                        callback(null, (string)json["chatID"]);
                })
            );
        }

        public static void retreiveLeaderboard(string key, EleCellProfileCallback callback)
        {
            if (gameID == null)
            {
                callback("GameServer.retreiveLeaderboard : Game ID not set", null);
                return;
            }
            if (_accessToken == null)
            {
                callback("GameServer.retreiveLeaderboard : No access token", null);
                return;
            }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            form.AddField("scoreKey", key);

            NetworkManager.instance.Query("retreiveLeaderboard", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        int r = 0;

                        double s = 0;

                        Ranking[] board = new Ranking[json["me"].Count];
                        for (int i = 0; i < board.Length; i++)
                        {
                            board[i].rank = json["me"][i]["rank"].AsInt;
                            board[i].nick = json["me"][i]["nick"];
                            board[i].score = json["me"][i][key].AsDouble;
                            board[i].isMe = (json["me"][i]["isMe"].AsInt == 1);
                            if (board[i].isMe) { r = board[i].rank; s = board[i].score; }
                        }
                        _instance.leaderboardCache.Add(board);
                        _instance.leaderboardCacheIndex[key]["leaderboard"].AsInt = _instance.leaderboardCache.Count - 1;
                        _instance.leaderboardCacheIndex[key]["myRank"].AsInt = r;
                        _instance.leaderboardCacheIndex[key]["myScore"] = s.ToString();

                        board = new Ranking[json["top"].Count];
                        for (int i = 0; i < board.Length; i++)
                        {
                            board[i].rank = json["top"][i]["rank"].AsInt;
                            board[i].nick = json["top"][i]["nick"];
                            board[i].score = json["top"][i][key].AsDouble;
                            board[i].isMe = (json["top"][i]["isMe"].AsInt == 1);
                        }
                        _instance.leaderboardCache.Add(board);
                        _instance.leaderboardCacheIndex[key]["leaderboardTop"].AsInt = _instance.leaderboardCache.Count - 1;

                        board = new Ranking[json["frd"].Count];
                        for (int i = 0; i < board.Length; i++)
                        {
                            board[i].rank = json["frd"][i]["rank"].AsInt;
                            board[i].nick = json["frd"][i]["nick"];
                            board[i].score = json["frd"][i][key].AsDouble;
                            board[i].isMe = (json["frd"][i]["isMe"].AsInt == 1);
                        }
                        _instance.leaderboardCache.Add(board);
                        _instance.leaderboardCacheIndex[key]["leaderboardFriends"].AsInt = _instance.leaderboardCache.Count - 1;

                        callback(null, null);
                    }
                })
            );
        }

        //public static void shareToFB(string _link, string _title, string _caption, string _description, string _pictureLink)
        //{
        //    debugText = _link + "|" + _pictureLink;
        //    if (!SA_FB.IsLoggedIn)
        //    {
        //        _instance.loginCB = _instance.OnReadyToShare;

        //        if (!SA_FB.IsInitialized) SA_FB.Init(_instance.OnFacebookInit);
        //        else _instance.OnFacebookInit();

        //        return;
        //    }

        //    _instance.OnReadyToShare(null, null);
        //}

        //void OnReadyToShare(string error, string message)
        //{
        //    if (error == null)
        //    {
        //        string[] arg = debugText.Split('|');
        //        var client = UM_SocialService.SharingClient;
        //        var builder = new UM_ShareDialogBuilder();
        //        builder.AddImage(Resources.Load<Texture2D>(arg[1]));
        //        builder.SetUrl(arg[0]);
        //        client.ShareToFacebook(builder, (result) =>
        //        {
        //            if (result.IsSucceeded)
        //            {
        //                Debug.Log("Sharing started ");
        //            }
        //            else
        //            {
        //                Debug.Log("Failed to share: " + result.Error.FullMessage);
        //            }
        //        });
        //    }
        //}

        public static void LoadFbProfilePic(string fbid, EleCellPictureCallback callback)
        {
            _instance.StartCoroutine(_instance.LoadFbProfilePicWWW(fbid, callback));
        }

        IEnumerator LoadFbProfilePicWWW(string fbid, EleCellPictureCallback callback)
        {
#if !UNITY_WEBPLAYER && !UNITY_WP8
            System.IO.FileInfo finfo = new System.IO.FileInfo(Application.persistentDataPath + "/" + fbid + ".png");

            if (finfo != null && finfo.Exists && PlayerPrefs.HasKey(fbid))
            {
                System.DateTime lastTime = System.DateTime.Parse(PlayerPrefs.GetString(fbid), System.Globalization.DateTimeFormatInfo.CurrentInfo);
                System.TimeSpan timePassed = System.DateTime.Now - lastTime;
                double dayPassed = timePassed.TotalDays;

                if (dayPassed <= (double)pictureCacheDay)
                {

                    byte[] picFile = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/" + fbid + ".png");
                    Texture2D pic = new Texture2D((int)profilePicSize.x, (int)profilePicSize.y);
                    pic.LoadImage(picFile);

                    callback(null, pic);

                    yield break;
                }
                else
                {
                    System.IO.File.Delete(Application.persistentDataPath + "/" + fbid + ".png");
                }
            }
#endif
            WWW imageRequest = new WWW("http://graph.facebook.com/" + fbid + "/picture?width=" + ((int)profilePicSize.x).ToString() + "&height=" + ((int)profilePicSize.y).ToString());

            yield return imageRequest;
#if !UNITY_WEBPLAYER && !UNITY_WP8
            if (imageRequest.error == null)
            {
                //profilePicReady = true;
                callback(null, imageRequest.texture);

                System.IO.File.WriteAllBytes(Application.persistentDataPath + "/" + fbid + ".png", imageRequest.texture.EncodeToPNG());
                PlayerPrefs.SetString(fbid, System.DateTime.Now.ToString(System.Globalization.DateTimeFormatInfo.CurrentInfo));

            }
#endif
        }

        private static string playStoreUrl;

        public static void PromptRating(string androidurl, string iosAppID, string _title, string _text)
        {

            //string appIdentifier = Application.identifier;

            //var dialog = new AN_AlertDialog(AN_DialogTheme.Default);
            //dialog.Title = _title;
            //dialog.Message = _text;

            //dialog.SetNegativeButton("Later", () =>
            //{

            //});

            //dialog.SetPositiveButton("Yes", () =>
            //{
            //    System.Uri uri = new System.Uri("market://details?id=" + appIdentifier);
            //    AN_Intent viewIntent = new AN_Intent(AN_Intent.ACTION_VIEW, uri);
            //    AN_MainActivity.Instance.StartActivity(viewIntent);
            //});

            //dialog.Show();
        }

        void loginFbInGame(string error, string message)
        {
            // empty callback
        }

        private EleCellFbInviteCallback fbInviteCB;
        /*
		void FbAppReqComplete(FBResult result){
			if (fbInviteCB == null) { 
				return;
			}
			
			JSONClass json;
			json = (JSONClass) JSONNode.Parse(result.Text);
			
			if (json == null) 
				fbInviteCB("Fail to send request",null);
			else {
				string[] frd = new string[json["to"].Count];
				for (int i = 0; i< frd.Length; i++)
					frd[i] = json["to"][i];
				
				StartCoroutine(updateFriendWWW(frd,OnFriendUpdated));
				fbInviteCB(null,frd);
			}
			fbInviteCB = null;
		}
		*/

        private string _saveString;
        private bool _cloudsaving = false;
        private EleCellProfileCallback cloudSaveCB;

        public static void uploadSave(string saveString, EleCellProfileCallback callback)
        {
            if (gameID == null)
            {
                callback("GameServer.uploadSave : Game ID not set", null);
                return;
            }
            if (_accessToken == null)
            {
                callback("GameServer.uploadSave : No access token", null);
                return;
            }

            instance._saveString = saveString;
            instance.cloudSaveCB = callback;
            instance._cloudsaving = true;
        }

        IEnumerator uploadSaveWWW(string message, EleCellProfileCallback callback)
        {
            WWWForm form = new WWWForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            form.AddField("hash", EncryptionHash.Md5Sum(gameID + _accessToken));
            form.AddField("save", message + "%&");

            while (!WWWPool.instance.Available) yield return null; WWW wwwPost = WWWPool.instance.GetWWW("http://www.elecell.com/cloudSave2.php", form);

            float timer = 0f;

            while (timer < loginTimeOut && !wwwPost.isDone)
            {
                yield return null;
                timer += Time.deltaTime;
            }

            if (!wwwPost.isDone)
            {
                callback("WWW timeout", null);
            }
            else if (wwwPost.error != null)
            {
                callback(wwwPost.error, null);
            }
            else
            {
                JSONClass json = (JSONClass)JSONNode.Parse(wwwPost.text);

                if (json["error"] != null)
                {
                    callback(json["error"], wwwPost.text);
                }
                else
                {
                    callback(null, wwwPost.text);
                }
            }
            WWWPool.instance.Release(wwwPost);
        }

        public static void downloadSave(EleCellProfileCallback callback)
        {
            if (gameID == null)
            {
                callback("GameServer.downloadSave : Game ID not set", null);
                return;
            }
            if (_accessToken == null)
            {
                callback("GameServer.downloadSave : No access token", null);
                return;
            }
            instance.StartCoroutine(instance.downloadSaveWWW(callback));
        }

        IEnumerator downloadSaveWWW(EleCellProfileCallback callback)
        {

            WWWForm form = new WWWForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            form.AddField("hash", EncryptionHash.Md5Sum(gameID + _accessToken));

            while (!WWWPool.instance.Available) yield return null; WWW wwwPost = WWWPool.instance.GetWWW("http://www.elecell.com/cloudSave2.php", form);

            float timer = 0f;

            while (timer < loginTimeOut && !wwwPost.isDone)
            {
                yield return null;
                timer += Time.deltaTime;
            }

            if (!wwwPost.isDone)
            {
                callback("WWW timeout", null);
            }
            else if (wwwPost.error != null)
            {
                callback(wwwPost.error, null);
            }
            else
            {
                JSONClass json = (JSONClass)JSONNode.Parse(wwwPost.text);

                if (json["error"] != null)
                {
                    callback(json["error"], wwwPost.text);
                }
                else
                {
                    //					Debug.Log(wwwPost.text);
                    //					Debug.Log(json.ToString());
                    string save = (string)json["save"];
                    Debug.Log(save);
                    if (save.Length > 4 && save.Substring(save.Length - 2) == "%&")
                    {
                        save = save.Substring(0, save.Length - 2);
                        callback(null, save);
                    }
                    else
                        callback("invalid save", save);
                }
            }
            WWWPool.instance.Release(wwwPost);
        }

        public static void getLanguage(EleCellProfileCallback callback)
        {
            if (gameID == null)
            {
                callback("EleCellLogin.PlayerProfile.getLanguage : Game ID not set", null);
                return;
            }
            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("lang", lang);
            //Debug.Log("game id: " +gameID);
            NetworkManager.instance.Query("language", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                        callback((string)json["error"], null);
                    else
                        callback(null, (string)json["lang"]);
                })
            );
        }

        public static void changeNick(string nick, EleCellProfileCallback callback)
        {
            if (gameID == null)
            {
                callback("GameServer.changeNick : Game ID not set", null);
                return;
            }
            if (_accessToken == null)
            {
                Debug.Log("GameServer.changeNick : No access token");
                return;
            }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("nick", nick);
            form.AddField("accessToken", _accessToken);

            NetworkManager.instance.Query("rename", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        _instance._pinfo.nick = json["nick"];
                        callback(null, _instance._pinfo.nick);
                    }
                })
            );
        }

        public static void startChoice(string choice, EleCellProfileCallback callback)
        {
            if (gameID == null) { callback("GameServer.startChoice : Game ID not set", null); return; }
            if (_accessToken == null) { Debug.Log("GameServer.startChoice : No access token"); return; }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("choice", choice);
            form.AddField("accessToken", _accessToken);

            NetworkManager.instance.Query("startChoice", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
#if UNITY_EDITOR
                        callback(null, json.ToString());
#else
						callback(null,null);
#endif
                    }
                })
            );
        }

        public static void friendQuery(FriendAction action, string param, EleCellProfileCallback callback)
        {
            if (gameID == null)
            {
                callback("GameServer.friendQuery : Game ID not set", null); return;
            }
            if (_accessToken == null)
            {
                callback("GameServer.friendQuery : No access token", null); return;
            }

            if (action == FriendAction.add)
            {
                int id = 0;
                int.TryParse(param, out id);

                for (int i = 0; i < instance._inviteList.Length; i++)
                {
                    if (instance._inviteList[i].id == id)
                    {
                        callback("GameServer.friendQuery : cannot resend friend request", null); return;
                    }
                }
            }
            else if (action == FriendAction.cxlAdd)
            {
                int id = 0;
                int.TryParse(param, out id);
                bool invited = false;

                for (int i = 0; i < instance._inviteList.Length; i++)
                {
                    if (instance._inviteList[i].id == id)
                    {
                        invited = true;
                    }
                }

                if (!invited)
                {
                    callback("GameServer.friendQuery : cannot cancel non-existent friend request", null); return;
                }
            }
            else if (action == FriendAction.confirm)
            {
                int id = 0;
                int.TryParse(param, out id);
                bool invited = false;

                for (int i = 0; i < instance._friendList.Length; i++)
                {
                    if (instance._friendList[i].id == id && instance._friendList[i].friendship < 2)
                    {
                        invited = true;
                    }
                }

                if (!invited)
                {
                    callback("GameServer.friendQuery : cannot confirm friend without friend request", null); return;
                }
            }
            else if (action == FriendAction.delete)
            {
                int id = 0;
                int.TryParse(param, out id);
                bool inList = false;

                for (int i = 0; i < instance._friendList.Length; i++)
                {
                    if (instance._friendList[i].id == id && (instance._friendList[i].friendship == 2 || instance._friendList[i].friendship == 0))
                    { // delete friend or reject fr
                        inList = true;
                    }
                }

                if (!inList)
                {
                    callback("GameServer.friendQuery : cannot delete friend not on friend list", null); return;
                }
            }

            TcpForm form = new TcpForm();

            if (action == FriendAction.suggest) param = "0";

            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            form.AddField("action", action.ToString());
            form.AddField("param", param);

            NetworkManager.instance.Query("friend", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        if (action == FriendAction.suggest)
                        {
                            _instance._suggestedFriend = new Friend[json["array"].Count];
                            for (int i = 0; i < _instance._suggestedFriend.Length; i++)
                            {
                                _instance._suggestedFriend[i].id = json["array"][i]["pid"].AsInt;
                                _instance._suggestedFriend[i].fbid = json["array"][i]["fbid"];
                                _instance._suggestedFriend[i].nick = json["array"][i]["nick"];
                                _instance._suggestedFriend[i].level = json["array"][i]["playerLvl"].AsInt;

                                _instance._suggestedFriend[i].lastSeen = GameTime(json["array"][i]["loginTime"]);
                                if (json["array"][i]["type"] != null)
                                {
                                    _instance._suggestedFriend[i].hero.type = json["array"][i]["type"];
                                    _instance._suggestedFriend[i].hero.level = json["array"][i]["level"].AsInt;
                                }

                                _instance._suggestedFriend[i].pets = new Pet[6];

                                for (int k = 0; k < 6; k++)
                                {
                                    if (json["array"][i]['p' + (k + 1).ToString()] != null)
                                    {
                                        _instance._suggestedFriend[i].pets[k].type = json["array"][i]['p' + (k + 1).ToString()];
                                        _instance._suggestedFriend[i].pets[k].level = json["array"][i]['p' + (k + 1).ToString() + 'l'].AsInt;
                                    }
                                }
                            }
                        }
                        else if (action == FriendAction.search)
                        {
                            _instance._friendSearched = new Friend[json["array"].Count];
                            for (int i = 0; i < _instance._friendSearched.Length; i++)
                            {
                                _instance._friendSearched[i].id = json["array"][i]["pid"].AsInt;
                                _instance._friendSearched[i].fbid = json["array"][i]["fbid"];
                                _instance._friendSearched[i].nick = json["array"][i]["nick"];
                                _instance._friendSearched[i].level = json["array"][i]["playerLvl"].AsInt;

                                _instance._friendSearched[i].lastSeen = GameTime(json["array"][i]["loginTime"]);

                                if (json["array"][i]["type"] != null)
                                {
                                    _instance._friendSearched[i].hero.type = json["array"][i]["type"];
                                    _instance._friendSearched[i].hero.level = json["array"][i]["level"].AsInt;
                                }

                                _instance._friendSearched[i].pets = new Pet[6];

                                for (int k = 0; k < 6; k++)
                                {
                                    if (json["array"][i]['p' + (k + 1).ToString()] != null)
                                    {
                                        _instance._friendSearched[i].pets[k].type = json["array"][i]['p' + (k + 1).ToString()];
                                        _instance._friendSearched[i].pets[k].level = json["array"][i]['p' + (k + 1).ToString() + 'l'].AsInt;
                                    }
                                }
                            }
                        }
                        else if (action == FriendAction.add)
                        {
                            Friend addedFriend = new Friend();

                            addedFriend.id = json["pid"].AsInt;

                            addedFriend.fbid = json["fbid"];
                            addedFriend.nick = json["nick"];
                            addedFriend.level = json["playerLvl"].AsInt;

                            addedFriend.lastSeen = GameTime(json["loginTime"]);

                            if (json["type"] != null)
                            {
                                addedFriend.hero.type = json["type"];
                                addedFriend.hero.level = json["level"].AsInt;
                            }

                            addedFriend.pets = new Pet[6];

                            for (int k = 0; k < 6; k++)
                            {
                                if (json['p' + (k + 1).ToString()] != null)
                                {
                                    addedFriend.pets[k].type = json['p' + (k + 1).ToString()];
                                    addedFriend.pets[k].level = json['p' + (k + 1).ToString() + 'l'].AsInt;
                                }
                            }

                            if (_instance._inviteList == null)
                            {
                                _instance._inviteList = new Friend[1];
                                _instance._inviteList[0] = addedFriend;
                            }
                            else
                            {
                                Friend[] tempList = new Friend[_instance._inviteList.Length + 1];
                                for (int i = 0; i < _instance._inviteList.Length; i++)
                                    tempList[i] = _instance._inviteList[i];

                                tempList[_instance._inviteList.Length] = addedFriend;

                                _instance._inviteList = tempList;
                            }

                            if (_instance._friendSearched != null)
                                for (int i = 0; i < _instance._friendSearched.Length; i++)
                                {
                                    if (_instance._friendSearched[i].id == addedFriend.id)
                                    {
                                        _instance._friendSearched[i].friendship = 1;
                                        break;
                                    }
                                }
                        }
                        else if (action == FriendAction.cxlAdd)
                        {
                            int id = 0;
                            int.TryParse(param, out id);

                            Friend[] tempList = new Friend[_instance._inviteList.Length - 1];
                            int idx = 0;

                            for (int i = 0; i < _instance._inviteList.Length; i++)
                            {
                                if (_instance._inviteList[i].id != id)
                                {
                                    tempList[idx] = _instance._inviteList[i];
                                    idx++;
                                }
                            }

                            _instance._inviteList = tempList;
                        }
                        else if (action == FriendAction.confirm)
                        {
                            int id = 0;
                            int.TryParse(param, out id);

                            for (int i = 0; i < _instance._friendList.Length; i++)
                            {
                                if (_instance._friendList[i].id == id)
                                {
                                    _instance._friendList[i].friendship = 2;
                                }
                            }

                            bool invited = false;
                            for (int i = 0; i < _instance._inviteList.Length; i++)
                            {
                                if (_instance._inviteList[i].id == id)
                                {
                                    invited = true;
                                }
                            }
                            if (invited)
                            {
                                Friend[] tempList = new Friend[_instance._inviteList.Length - 1];
                                int idx = 0;

                                for (int i = 0; i < _instance._inviteList.Length; i++)
                                {
                                    if (_instance._inviteList[i].id != id)
                                    {
                                        tempList[idx] = _instance._inviteList[i];
                                        idx++;
                                    }
                                }

                                _instance._inviteList = tempList;
                            }
                        }
                        else if (action == FriendAction.delete)
                        {
                            int id = 0;
                            int.TryParse(param, out id);

                            Friend[] tempList = new Friend[_instance._friendList.Length - 1];
                            int idx = 0;

                            for (int i = 0; i < _instance._friendList.Length; i++)
                            {
                                if (_instance._friendList[i].id != id)
                                {
                                    tempList[idx] = _instance._friendList[i];
                                    idx++;
                                }
                            }

                            _instance._friendList = tempList;

                        }
#if UNITY_EDITOR
                        callback(null, json.ToString());
#else
						callback(null,null);
#endif
                    }
                })
            );
        }

        public static void drawPet(int version, string currency, int amount, int pool, EleCellProfileCallback callback)
        {
            if (gameID == null)
            {
                callback("GameServer.drawPet : Game ID not set", null); return;
            }
            if (_accessToken == null)
            {
                callback("GameServer.drawPet : No access token", null); return;
            }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            form.AddField("version", version);
            form.AddField("currency", currency);
            form.AddField("amount", amount);
            form.AddField("pool", pool);

            NetworkManager.instance.Query("drawPet", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        _instance._lastPet = new Pet();
                        _instance._lastPet.id = json["petID"].AsInt;
                        _instance._lastPet.type = json["type"];
                        _instance._lastPet.rank = json["rank"].AsInt;
                        _instance._lastPet.gender = json["gender"].AsInt;
                        _instance._lastPet.status = json["status"].AsInt;
                        _instance._lastPet.name = json["name"];
                        _instance._lastPet.level = json["level"].AsInt;
                        _instance._lastPet.xp = json["xp"].AsInt;

                        char star = '1';
                        if (_instance._lastPet.rank > 8) star = '5';
                        else if (_instance._lastPet.rank > 6) star = '4';
                        else if (_instance._lastPet.rank > 4) star = '3';
                        else if (_instance._lastPet.rank > 2) star = '2';
                        _instance._achieve["pet" + star].AsInt = _instance._achieve["pet" + star].AsInt + 1;

                        _instance.CheckTrophy(1, "pet" + star);

                        if (_instance._petList.Length == 0)
                        {
                            _instance._petList = new Pet[1];
                            _instance._petList[0] = _instance._lastPet;
                        }
                        else
                        {
                            Pet[] tempList = new Pet[_instance._petList.Length + 1];
                            tempList[0] = _instance._lastPet;
                            for (int i = 1; i < tempList.Length; i++)
                                tempList[i] = _instance._petList[i - 1];

                            _instance._petList = tempList;
                        }

#if UNITY_EDITOR
                        callback(null, json.ToString());
#else
						callback(null,null);
#endif
                    }
                })
            );
        }

        public static void drawHero(int version, string currency, int amount, int pool, EleCellProfileCallback callback)
        {
            if (gameID == null)
            {
                callback("GameServer.drawHero : Game ID not set", null); return;
            }
            if (_accessToken == null)
            {
                callback("GameServer.drawHero : No access token", null); return;
            }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            form.AddField("version", version);
            form.AddField("currency", currency);
            form.AddField("amount", amount);
            form.AddField("pool", pool);

            NetworkManager.instance.Query("drawHero", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        _instance._lastHero = new Hero();
                        _instance._lastHero.id = json["hid"].AsInt;
                        _instance._lastHero.type = json["type"];
                        _instance._lastHero.rank = json["rank"].AsInt;
                        _instance._lastHero.status = json["status"].AsInt;
                        _instance._lastHero.level = json["level"].AsInt;
                        _instance._lastHero.xp = json["xp"].AsInt;

                        _instance._lastHero.stats = new int[6];
                        _instance._lastHero.skill = new int[6];
                        _instance._lastHero.stats[0] = json["sta1"].AsInt;
                        _instance._lastHero.stats[1] = json["sta2"].AsInt;
                        _instance._lastHero.stats[2] = json["sta3"].AsInt;
                        _instance._lastHero.stats[3] = json["sta4"].AsInt;
                        _instance._lastHero.stats[4] = json["sta5"].AsInt;
                        _instance._lastHero.stats[5] = json["sta6"].AsInt;
                        _instance._lastHero.skill[0] = json["sk1"].AsInt;
                        _instance._lastHero.skill[1] = json["sk2"].AsInt;
                        _instance._lastHero.skill[2] = json["sk3"].AsInt;
                        _instance._lastHero.skill[3] = json["sk4"].AsInt;
                        _instance._lastHero.skill[4] = json["sk5"].AsInt;
                        _instance._lastHero.skill[5] = json["sk6"].AsInt;

                        Hero[] temp = new Hero[_instance._heroList.Length + 1];
                        for (int i = 0; i < _instance._heroList.Length; i++) temp[i] = _instance._heroList[i];
                        temp[_instance._heroList.Length] = _instance._lastHero;

                        _instance._heroList = temp;

#if UNITY_EDITOR
                        callback(null, json.ToString());
#else
						callback(null,null);
#endif
                    }
                })
            );
        }

        public static void editDeck(int deckID, Deck deck, EleCellProfileCallback callback)
        {
            if (gameID == null)
            {
                callback("GameServer.editDeck : Game ID not set", null); return;
            }
            if (_accessToken == null)
            {
                callback("GameServer.editDeck : No access token", null); return;
            }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            form.AddField("deck", deckID);

            string value = deck.hero.ToString() + ',' + deck.pets[0].ToString() + ',' + deck.pets[1].ToString() + ',' + deck.pets[2].ToString() + ',' + deck.pets[3].ToString() + ',' + deck.pets[4].ToString() + ',' + deck.pets[5].ToString();

            form.AddField("value", value);
            //Debug.Log (gameID + _accessToken + " " + deckID.ToString () + " " + value);

            NetworkManager.instance.Query("deck", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        deck.id = deckID;

                        for (int i = 0; i < _instance._deckList.Length; i++)
                        {
                            if (_instance._deckList[i].id == deckID) _instance._deckList[i] = deck;
                        }

#if UNITY_EDITOR
                        callback(null, json.ToString());
#else
						callback(null,null);
#endif
                    }
                })
            );
        }

        public static void petQuery(PetQuery query, EleCellProfileCallback callback)
        {
            if (gameID == null)
            {
                callback("GameServer.petQuery : Game ID not set", null); return;
            }
            if (_accessToken == null)
            {
                callback("GameServer.petQuery : No access token", null); return;
            }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);

            switch (query.action)
            {
                case PetAction.updateType:
                    form.AddField("action", "updateType");
                    form.AddField("petID", query.petID);
                    form.AddField("newValue", query.type);
                    break;
                case PetAction.updateStatus:
                    form.AddField("action", "updateStatus");
                    form.AddField("petID", query.petID);
                    form.AddField("newValue", query.status);
                    break;
                case PetAction.harvest:
                    form.AddField("action", "harvest");
                    form.AddField("petID", query.petID);
                    form.AddField("newValue", query.status);
                    break;
            }

            NetworkManager.instance.Query("pet", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        if (query.action == PetAction.harvest)
                        {
                            int cooldown = json["cooldown"].AsInt;

                            int pos = _instance.GetPetPos(query.petID);
                            if (pos < _instance._petList.Length)
                            {
                                if (query.status == 1)
                                {
                                    _instance._petList[pos].cooldown1 = ServerTime.AddSeconds(cooldown);
                                }
                                else if (query.status == 2)
                                {
                                    _instance._petList[pos].cooldown2 = ServerTime.AddSeconds(cooldown);
                                }
                            }
                            string item = (string)json["item"];
                            _instance._itemList[item].AsInt = _instance._itemList[item].AsInt + json["count"].AsInt;

                            _instance._achieve[item].AsInt = _instance._achieve[item].AsInt + json["count"].AsInt;
                            _instance.CheckTrophy(1, item);

                            _instance._pStats["xp"].AsInt = _instance._pStats["xp"].AsInt + json["xp"].AsInt;
                        }
                        else if (query.action == PetAction.updateStatus)
                        {
                            if (query.status == 99)
                            {
                                Pet[] tempList = new Pet[_instance._petList.Length - 1];

                                int idx = 0;

                                for (int i = 0; i < _instance._petList.Length; i++)
                                {
                                    if (_instance._petList[i].id != query.petID)
                                    {
                                        tempList[idx] = _instance._petList[i];
                                        idx++;
                                    }
                                }

                                _instance._petList = tempList;
                            }
                        }

                        callback(null, json.ToString());
                    }
                })
            );
        }

        public static void achieveQuery(EleCellProfileCallback callback)
        {
            if (gameID == null)
            {
                callback("GameServer.achieveQuery : Game ID not set", null); return;
            }
            if (_accessToken == null)
            {
                callback("GameServer.achieveQuery : No access token", null); return;
            }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);

            NetworkManager.instance.Query("achieve", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        if (json["achieve"].Count > 0)
                            _instance._achieve = (JSONClass)json["achieve"];
                        else
                            instance._achieve = new JSONClass();

                        callback(null, null);
                    }
                })
            );
        }

        public static void dailyDoneQuery(EleCellProfileCallback callback)
        {
            if (gameID == null)
            {
                callback("GameServer.dailyDoneQuery : Game ID not set", null); return;
            }
            if (_accessToken == null)
            {
                callback("GameServer.dailyDoneQuery : No access token", null); return;
            }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);

            NetworkManager.instance.Query("dailyDone", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        if (json["dailyDone"].Count > 0)
                            _instance._dailyDone = (JSONClass)json["dailyDone"];
                        else
                            instance._dailyDone = new JSONClass();

                        callback(null, null);
                    }
                })
            );
        }

        public static void buildQuery(BuildQuery query, EleCellProfileCallback callback)
        {
            if (gameID == null) { callback("GameServer.buildQuery : Game ID not set", null); return; }
            if (_accessToken == null) { callback("GameServer.buildQuery : No access token", null); return; }
            instance.buildQueryWWW(query, callback);
        }

        void buildQueryWWW(BuildQuery query, EleCellProfileCallback callback)
        {
            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);

            switch (query.action)
            {
                case BuildAction.upgrade:
                    form.AddField("action", "upgrade");
                    form.AddField("bID", query.bID);
                    form.AddField("newValue", query.type);
                    break;
                case BuildAction.destroy:
                    form.AddField("action", "destroy");
                    form.AddField("bID", query.bID);
                    break;
                case BuildAction.cancel:
                    form.AddField("action", "cancel");
                    form.AddField("bID", query.bID);
                    break;
                case BuildAction.complete:
                    form.AddField("action", "complete");
                    form.AddField("bID", query.bID);
                    form.AddField("newValue", query.status);
                    break;
                case BuildAction.task:
                    form.AddField("action", "task");
                    form.AddField("bID", query.bID);
                    form.AddField("newValue", query.type);
                    form.AddField("newValue2", query.status);
                    break;
                case BuildAction.cxltask:
                    form.AddField("action", "cxltask");
                    form.AddField("bID", query.bID);
                    break;
                case BuildAction.comtask:
                    form.AddField("action", "comtask");
                    form.AddField("bID", query.bID);
                    form.AddField("newValue", query.status);
                    break;
                case BuildAction.collect:
                    form.AddField("action", "collect");
                    form.AddField("bID", query.bID);
                    break;
            }

            NetworkManager.instance.Query("build", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        Debug.Log(query.bID);
                        Debug.Log(query.type);
                        Debug.Log(query.status);
                        //bil add  金庫 無金幣顯示無金幣
                        if (query.status == 0)
                        {
                            Debug.Log("收集中,晚點來");
                        }

                        callback(json["error"], null);
                    }
                    else
                    {
                        if (query.action == BuildAction.upgrade)
                        {
                            int pos = GetBuildPos(query.bID);
                            _buildList[pos].upgrade = json["buID"].AsInt;
                            _buildList[pos].brID = json["brID"].AsInt;
                            _buildList[pos].status = 2;
                            _buildList[pos].upgradeFinish = ServerTime.AddSeconds((double)GetUpgradeSec(_buildList[pos].brID));
                        }
                        else if (query.action == BuildAction.destroy)
                        {
                            int pos = GetBuildPos(query.bID);
                            _buildList[pos].upgrade = json["buID"].AsInt;
                            _buildList[pos].brID = json["brID"].AsInt;
                            _buildList[pos].status = 3;
                            _buildList[pos].upgradeFinish = ServerTime.AddSeconds((double)GetUpgradeSec(_buildList[pos].brID));
                        }
                        else if (query.action == BuildAction.cancel)
                        {
                            int pos = GetBuildPos(query.bID);
                            _buildList[pos].upgrade = 0;
                            _buildList[pos].brID = json["brID"].AsInt;
                            _buildList[pos].status = 1;

                            callback(null, (string)json["refund"]);
                            return;
                        }
                        else if (query.action == BuildAction.complete)
                        {
                            int pos = GetBuildPos(query.bID);
                            _buildList[pos].upgrade = 0;
                            _buildList[pos].brID = json["brID"].AsInt;

                            if (_buildList[pos].status == 3)
                            {
                                _buildList[pos].type = "landEmpt";
                                _buildList[pos].rank = 1;
                            }
                            else
                            { // == 2 upgrade
                                int idx = GetBuildUpgrade(_buildList[pos].brID);
                                _buildList[pos].type = _bReq["array"][idx]["type"];
                                _buildList[pos].rank = _bReq["array"][idx]["rank"].AsInt;
                            }

                            _pStats["xp"].AsInt = _pStats["xp"].AsInt + json["xp"].AsInt;

                            _buildList[pos].status = 1;
                        }
                        else if (query.action == BuildAction.task)
                        {
                            int pos = GetBuildPos(query.bID);
                            _buildList[pos].task = json["btID"].AsInt;
                            _buildList[pos].rID = json["rID"].AsInt;

                            JSONNode task = GetTask(_buildList[pos].rID);

                            if (task["itmC1"].AsInt > 0)
                            {
                                _itemList[(string)task["item1"]].AsInt = _itemList[(string)task["item1"]].AsInt - task["itmC1"].AsInt * query.status;
                                if (_itemList[(string)task["item1"]].AsInt < 1)
                                    _itemList.Remove((string)task["item1"]);
                            }
                            if (task["itmC2"].AsInt > 0)
                            {
                                _itemList[(string)task["item2"]].AsInt = _itemList[(string)task["item2"]].AsInt - task["itmC2"].AsInt * query.status;
                                if (_itemList[(string)task["item2"]].AsInt < 1)
                                    _itemList.Remove((string)task["item2"]);
                            }
                            if (task["itmC3"].AsInt > 0)
                            {
                                _itemList[(string)task["item3"]].AsInt = _itemList[(string)task["item3"]].AsInt - task["itmC3"].AsInt * query.status;
                                if (_itemList[(string)task["item3"]].AsInt < 1)
                                    _itemList.Remove((string)task["item3"]);
                            }
                            if (task["itmC4"].AsInt > 0)
                            {
                                _itemList[(string)task["item4"]].AsInt = _itemList[(string)task["item4"]].AsInt - task["itmC4"].AsInt * query.status;
                                if (_itemList[(string)task["item4"]].AsInt < 1)
                                    _itemList.Remove((string)task["item4"]);
                            }

                            _buildList[pos].taskFinish = ServerTime.AddSeconds((double)task["sec"].AsInt * query.status);
                        }
                        else if (query.action == BuildAction.comtask)
                        {
                            int pos = GetBuildPos(query.bID);
                            _buildList[pos].task = 0;

                            string item = (string)json["item"];
                            _itemList[item].AsInt = _itemList[item].AsInt + json["count"].AsInt;

                            _achieve[item].AsInt = _achieve[item].AsInt + json["count"].AsInt;
                            CheckTrophy(1, item);
                        }
                        else if (query.action == BuildAction.collect)
                        {
                            int pos = GetBuildPos(query.bID);
                            _buildList[pos].cooldown = ServerTime;

                            _pStats[json["currency"]].AsInt = _pStats[json["currency"]].AsInt + json["reward"].AsInt;
                            CheckTrophy(0, json["currency"]);
                        }

                        callback(null, json.ToString());
                    }
                })
            );
        }

        public static void breedQuery(BuildQuery query, EleCellProfileCallback callback)
        {
            if (gameID == null) { callback("GameServer.breedQuery : Game ID not set", null); return; }
            if (_accessToken == null) { callback("GameServer.breedQuery : No access token", null); return; }
            instance.breedQueryWWW(query, callback);
        }

        void breedQueryWWW(BuildQuery query, EleCellProfileCallback callback)
        {
            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);

            switch (query.action)
            {
                case BuildAction.task:
                    form.AddField("action", "task");
                    form.AddField("bID", query.bID);
                    form.AddField("newValue", query.type);
                    form.AddField("newValue2", query.status);
                    break;
                case BuildAction.comtask:
                    form.AddField("action", "comtask");
                    form.AddField("bID", query.bID);
                    form.AddField("newValue", query.status);
                    break;
                case BuildAction.destroy:
                    form.AddField("action", "destroy");
                    form.AddField("bID", query.bID);
                    form.AddField("newValue", query.type);
                    break;
            }

            NetworkManager.instance.Query("breed", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        if (query.action == BuildAction.task)
                        {
                            int pos = GetBuildPos(query.bID);
                            _buildList[pos].task = json["btID"].AsInt;

                            int petA = query.status;
                            int petB = int.Parse(query.type);

                            for (int i = 0; i < _petList.Length; i++)
                            {
                                if (_petList[i].id == petA || _petList[i].id == petB)
                                    _petList[i].status = query.bID + 100;
                            }

                            _buildList[pos].taskFinish = ServerTime.AddSeconds(300);
                        }
                        else if (query.action == BuildAction.comtask)
                        {
                            int pos = GetBuildPos(query.bID);
                            _buildList[pos].task = 0;

                            if (json["petID"].AsInt >= 0)
                            {
                                _lastPet = new Pet();
                                _lastPet.id = json["petID"].AsInt;
                                _lastPet.type = json["type"];
                                _lastPet.rank = json["rank"].AsInt;
                                _lastPet.gender = json["gender"].AsInt;
                                _lastPet.status = json["status"].AsInt;
                                _lastPet.name = json["name"];
                                _lastPet.level = json["level"].AsInt;
                                _lastPet.xp = json["xp"].AsInt;

                                char star = '1';
                                if (_lastPet.rank > 8) star = '5';
                                else if (_lastPet.rank > 6) star = '4';
                                else if (_lastPet.rank > 4) star = '3';
                                else if (_lastPet.rank > 2) star = '2';
                                _achieve["breed" + star].AsInt = _achieve["breed" + star].AsInt + 1;

                                CheckTrophy(1, "breed" + star);

                                _achieve["breed"].AsInt = _achieve["breed"].AsInt + 1;

                                CheckTrophy(1, "breed");
                            }
                        }
                        else if (query.action == BuildAction.destroy)
                        {
                            int pos = GetPetPos(query.status);
                            if (pos < _petList.Length)
                            {
                                _petList[pos].status = 4;
                            }
                            string item = (string)json["item"];
                            _itemList[item].AsInt = _itemList[item].AsInt + json["count"].AsInt;
                        }

                        callback(null, json.ToString());
                    }
                })
            );
        }

        public static void itemQuery(ItemQuery query, EleCellProfileCallback callback)
        {
            if (gameID == null)
            {
                callback("GameServer.itemQuery : Game ID not set", null); return;
            }
            if (_accessToken == null)
            {
                callback("GameServer.itemQuery : No access token", null); return;
            }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);

            switch (query.action)
            {
                case ItemAction.sell:
                    form.AddField("action", "sell");
                    form.AddField("item", query.field);
                    form.AddField("qty", query.value);
                    break;
                case ItemAction.buy:
                    form.AddField("action", "buy");
                    form.AddField("item", query.field);
                    form.AddField("qty", query.value);
                    break;
                case ItemAction.use:
                    form.AddField("action", "use");
                    form.AddField("item", query.field);
                    form.AddField("qty", query.value);
                    break;
            }

            NetworkManager.instance.Query("item", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        if (query.action == ItemAction.sell)
                        {
                            _instance._pStats["coin"].AsInt = _instance._pStats["coin"].AsInt + json["coin"].AsInt;

                            _instance._itemList[query.field].AsInt = _instance._itemList[query.field].AsInt - query.value;
                            if (_instance._itemList[query.field].AsInt < 1)
                                _instance._itemList.Remove(query.field);

                            callback(null, (string)json["coin"]);

                            _instance.CheckTrophy(0, "coin");
                        }
                        else if (query.action == ItemAction.buy)
                        {
                            _instance._itemList[query.field].AsInt = _instance._itemList[query.field].AsInt + query.value;

                            callback(null, (string)json["receive"]);
                        }
                        else if (query.action == ItemAction.use)
                        {
                            _instance._itemList[query.field].AsInt = _instance._itemList[query.field].AsInt - query.value;
                            if (_instance._itemList[query.field].AsInt < 1)
                                _instance._itemList.Remove(query.field);

#if UNITY_EDITOR
                            callback(null, json.ToString());
#else
							callback(null,null);
#endif
                        }
                    }
                })
            );
        }

        public void AddItem(string itemName,int level,EleCellProfileCallback callback)
        {

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", AccessToken);

            form.AddField("pid", MjSave.instance.playerID.Replace(".", ""));

            form.AddField("SaveType", "item");
            form.AddField("item", itemName);
            form.AddField("qty", level);

            NetworkManager.instance.Query("saveManager", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        Debug.Log(json["error"]);
                        //callback(json["error"], null);
                    }
                    else
                    {

#if UNITY_EDITOR
                        //callback(null, json.ToString());
                        Debug.Log(json);
#else
						callback(null,null);
#endif
                        
                    }
                })
            );
        }

        public void AddItemBase(string itemName, int level, EleCellProfileCallback callback)
        {

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", AccessToken);

            form.AddField("pid", "100000001");

            form.AddField("SaveType", "itemBase");
            form.AddField("item", itemName);
            form.AddField("qty", level);

            NetworkManager.instance.Query("saveManager", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        Debug.Log(json["error"]);
                        //callback(json["error"], null);
                    }
                    else
                    {

#if UNITY_EDITOR
                        //callback(null, json.ToString());
                        Debug.Log(json);
#else
						callback(null,null);
#endif

                    }
                })
            );
        }

        public void Test(EleCellProfileCallback callback)
        {
            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", AccessToken);
            
            form.AddField("pid", MjSave.instance.playerID.Replace(".", ""));
            
            form.AddField("type", "players");

            form.AddField("playerLvl", playerStats["playerLvl"].AsInt + 1);


            NetworkManager.instance.Query("saveManager", form, 
                new GenericCallback<JSONClass>(delegate (JSONClass json){
                    if (json["error"] != null)
                    {
                        Debug.Log(json["error"]);
                        //callback(json["error"], null);
                    }
                    else
                    {

    #if UNITY_EDITOR
                        //callback(null, json.ToString());
                        Debug.Log(json);
    #else
						    callback(null,null);
    #endif

                    }

                })
            );

        }
        public void DrawTest(int drawMax,string poolName, EleCellJsonCallback callback)
        {
            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", AccessToken);
            
            form.AddField("targetID", "100000001");
            form.AddField("pid", MjSave.instance.playerID.Replace(".", ""));

            form.AddField("drawMax", drawMax);
            form.AddField("PoolType", poolName);
            NetworkManager.instance.Query("Draw", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        Debug.Log(json["error"]);
                    }
                    else
                    {
                        Debug.Log(json);
                        callback(null,json);
                    }
                })
            );
        }
        public void SaveCharaterBase_Server(string charaterNo,EleCellJsonCallback callback)
        {

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", AccessToken);

            //form.AddField("pid", MjSave.instance.playerID.Replace(".", ""));
            form.AddField("pid", "100000001");

            form.AddField("SaveType", "CharaterBase");

            form.AddField("qty", 0);
            form.AddField("type", charaterNo);

            NetworkManager.instance.Query("saveManager", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        Debug.Log(json["error"]);
                    }
                    else
                    {
                        Debug.Log(json);
                        callback(null, json);
                    }
                })
            );

        }
        public void LoadCardPool_Server(EleCellJsonCallback callback)
        {
            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", AccessToken);

            form.AddField("pid", MjSave.instance.playerID.Replace(".", ""));

            form.AddField("type", "CardPool");


            NetworkManager.instance.Query("loadManager", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        Debug.Log(json["error"]);
                    }
                    else
                    {
                        callback(null, json);
                    }
                })
            );
        }

        public void LoadItem_Server(string Target,EleCellJsonCallback callback)
        {
            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", AccessToken);

            form.AddField("type", "item");

            form.AddField("pid", Target);

            NetworkManager.instance.Query("loadManager", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        Debug.Log(json["error"]);
                    }
                    else
                    {
                        Debug.Log(json);
                        callback(null, json);
                    }
                })
            );
        }

        public void LoadCharater_Server(string Target, EleCellJsonCallback callback)
        {
            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", AccessToken);

            form.AddField("type", "LoadCharater");

            form.AddField("pid", Target);

            NetworkManager.instance.Query("loadManager", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        Debug.Log(json["error"]);
                    }
                    else
                    {
                        callback(null, json);
                    }
                })
            );
            
        }

        public void LevelUp_Server(int Cost, string type , EleCellJsonCallback callback)
        {
            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", AccessToken);

            form.AddField("cost", Cost);
            form.AddField("type", type);

            form.AddField("pid", MjSave.instance.playerID.Replace(".", ""));

            NetworkManager.instance.Query("charaLevelUp", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        Debug.Log(json["error"]);
                    }
                    else
                    {
                        callback(null, json);
                    }
                })
            );
        }
        public static void missionQuery(int missionID, EleCellProfileCallback callback)
        {
            if (gameID == null)
            {
                callback("GameServer.missionQuery : Game ID not set", null); return;
            }
            if (_accessToken == null)
            {
                callback("GameServer.missionQuery : No access token", null); return;
            }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);

            if (missionID > 0) form.AddField("missionID", missionID);


            NetworkManager.instance.Query("mission", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        if (missionID == 0)
                        {
                            _instance._lastMission = new Mission();
                            _instance._lastMission.missionID = json["missionID"].AsInt;
                            _instance._lastMission.mName = json["mName"];
                            _instance._lastMission.objective = json["objective"];
                            _instance._lastMission.count = json["count"].AsInt;
                            _instance._lastMission.complete = json["complete"].AsInt;
                            _instance._lastMission.xp = json["xp"].AsInt;
                            _instance._lastMission.coin = json["coin"].AsInt;
                        }
                        else
                        {
                            _instance._pStats["playerLvl"].AsInt = json["level"].AsInt;
                            _instance._pStats["xp"].AsInt = _instance._pStats["xp"].AsInt + json["missionXP"].AsInt;
                            _instance._pStats["xplow"].AsInt = json["xplow"].AsInt;
                            _instance._pStats["xphi"].AsInt = json["xp"].AsInt;

                            _instance.CheckTrophy(0, "playerLvl");
                        }

#if UNITY_EDITOR
                        callback(null, json.ToString());
#else
						callback(null,null);
#endif

                    }
                })
            );
        }

        public static void trophyQuery(string trophy, EleCellProfileCallback callback)
        {
            if (gameID == null)
            {
                callback("GameServer.itemQuery : Game ID not set", null); return;
            }
            if (_accessToken == null)
            {
                callback("GameServer.itemQuery : No access token", null); return;
            }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            form.AddField("action", trophy);

            NetworkManager.instance.Query("trophy", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        for (int i = 0; i < _instance._trophy.Length; i++)
                        {
                            if (_instance._trophy[i].TrophyName == trophy)
                            {
                                _instance._trophy[i].collected = true;

                                if (_instance._trophy[i].reward.rewardType == 0)
                                {
                                    string currency = "xp";
                                    if (_instance._trophy[i].reward.rewardEnum == 0) currency = "coin";
                                    else if (_instance._trophy[i].reward.rewardEnum == 1) currency = "credit";
                                    else if (_instance._trophy[i].reward.rewardEnum == 2) currency = "gem";
                                    else if (_instance._trophy[i].reward.rewardEnum == 3) currency = "tickC";
                                    else if (_instance._trophy[i].reward.rewardEnum == 4) currency = "tickR";
                                    else if (_instance._trophy[i].reward.rewardEnum == 5) currency = "tickG";

                                    _instance._pStats[currency].AsDouble = _instance._pStats[currency].AsDouble + _instance._trophy[i].reward.amount;
                                }
                                else if (_instance._trophy[i].reward.rewardType == 1)
                                {
                                    _instance._itemList[_instance._trophy[i].reward.item].AsInt = _instance._itemList[_instance._trophy[i].reward.item].AsInt + (int)_instance._trophy[i].reward.amount;
                                }
                                break;
                            }
                        }

                        callback(null, trophy);
                    }
                })
            );
        }

        public static void dailyQuery(string trophy, EleCellProfileCallback callback)
        {
            if (gameID == null)
            {
                callback("GameServer.dailyQuery : Game ID not set", null); return;
            }
            if (_accessToken == null)
            {
                callback("GameServer.dailyQuery : No access token", null); return;
            }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            form.AddField("action", trophy);

            NetworkManager.instance.Query("daily", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        for (int i = 0; i < _instance._daily.Length; i++)
                        {
                            if (_instance._daily[i].TrophyName == trophy)
                            {
                                _instance._daily[i].collected = true;

                                if (_instance._daily[i].reward.rewardType == 0)
                                {
                                    string currency = "xp";
                                    if (_instance._daily[i].reward.rewardEnum == 0) currency = "coin";
                                    else if (_instance._daily[i].reward.rewardEnum == 1) currency = "credit";
                                    else if (_instance._daily[i].reward.rewardEnum == 2) currency = "gem";
                                    else if (_instance._daily[i].reward.rewardEnum == 3) currency = "tickC";
                                    else if (_instance._daily[i].reward.rewardEnum == 4) currency = "tickR";
                                    else if (_instance._daily[i].reward.rewardEnum == 5) currency = "tickG";

                                    _instance._pStats[currency].AsDouble = _instance._pStats[currency].AsDouble + _instance._daily[i].reward.amount;
                                }
                                else if (_instance._daily[i].reward.rewardType == 1)
                                {
                                    _instance._itemList[_instance._daily[i].reward.item].AsInt = _instance._itemList[_instance._daily[i].reward.item].AsInt + (int)_instance._daily[i].reward.amount;
                                }
                                break;
                            }
                        }

                        callback(null, trophy);
                    }
                })
            );
        }

        /// <summary>
        /// list, cxlJon, leave  	
        /// </summary>
        public static void guildQuery(GuildAction action, EleCellProfileCallback callback)
        {
            if (gameID == null) { callback("GameServer.guildQuery : Game ID not set", null); return; }
            if (_accessToken == null) { callback("GameServer.guildQuery : No access token", null); return; }
            if (action != GuildAction.list && action != GuildAction.cxlJoin && action != GuildAction.leave) return;

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            form.AddField("action", action.ToString());

            NetworkManager.instance.Query("guild", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        if (action == GuildAction.list)
                        {
                            _instance.guildSearched = new Guild[json["array"].Count];
                            for (int i = 0; i < _instance.guildSearched.Length; i++)
                            {
                                _instance.guildSearched[i].guildID = json["array"][i]["guildID"].AsInt;
                                _instance.guildSearched[i].guildName = json["array"][i]["guildName"];
                                _instance.guildSearched[i].motto = json["array"][i]["motto"];
                                _instance.guildSearched[i].level = json["array"][i]["level"].AsInt;
                                _instance.guildSearched[i].xp = json["array"][i]["xp"].AsInt;
                                _instance.guildSearched[i].status = json["array"][i]["status"].AsInt;
                                _instance.guildSearched[i].weekly = json["array"][i]["weekly"].AsInt;
                                _instance.guildSearched[i].lastWeek = json["array"][i]["lastWeek"].AsInt;
                                _instance.guildSearched[i].memberCount = json["array"][i]["memberCount"].AsInt;
                                _instance.guildSearched[i].guildMaster = json["array"][i]["guildMaster"];
                            }
                        }
                        else if (action == GuildAction.cxlJoin)
                        {
                            _instance._pinfo.guildID = 0;
                        }
                        else if (action == GuildAction.leave)
                        {
                            _instance._pinfo.guildRole = 99;
                            _instance._pinfo.guildTime = ServerTime;
                        }
#if UNITY_EDITOR
                        callback(null, json.ToString());
#else
						callback(null, null);
#endif
                    }
                })
            );
        }

        /// <summary>
        /// search, create
        /// </summary>
        public static void guildQuery(GuildAction action, string guildName, EleCellProfileCallback callback)
        {
            if (gameID == null) { callback("GameServer.guildQuery : Game ID not set", null); return; }
            if (_accessToken == null) { callback("GameServer.guildQuery : No access token", null); return; }
            if (action != GuildAction.search && action != GuildAction.create) return;

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            form.AddField("action", action.ToString());
            form.AddField("guildName", guildName);


            NetworkManager.instance.Query("guild", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        if (action == GuildAction.search)
                        {
                            _instance.guildSearched = new Guild[json["array"].Count];
                            for (int i = 0; i < _instance.guildSearched.Length; i++)
                            {
                                _instance.guildSearched[i].guildID = json["array"][i]["guildID"].AsInt;
                                _instance.guildSearched[i].guildName = json["array"][i]["guildName"];
                                _instance.guildSearched[i].motto = json["array"][i]["motto"];
                                _instance.guildSearched[i].level = json["array"][i]["level"].AsInt;
                                _instance.guildSearched[i].xp = json["array"][i]["xp"].AsInt;
                                _instance.guildSearched[i].status = json["array"][i]["status"].AsInt;
                                _instance.guildSearched[i].weekly = json["array"][i]["weekly"].AsInt;
                                _instance.guildSearched[i].lastWeek = json["array"][i]["lastWeek"].AsInt;
                                _instance.guildSearched[i].memberCount = json["array"][i]["memberCount"].AsInt;
                                _instance.guildSearched[i].guildMaster = json["array"][i]["guildMaster"];
                            }
                        }
                        else if (action == GuildAction.create)
                        {
                            _instance._pinfo.guildID = json["guild"]["guildID"].AsInt;
                            _instance._pinfo.guildLevel = json["guild"]["level"].AsInt;
                            _instance._pinfo.guildName = json["guild"]["guildName"];
                            _instance._pinfo.guildRole = Guild.GuildMaster;
                            _instance._pinfo.guildWeekly = 0;

                            _instance.guildDetails.guildID = json["guild"]["guildID"].AsInt;
                            _instance.guildDetails.guildName = json["guild"]["guildName"];
                            _instance.guildDetails.motto = json["guild"]["motto"];
                            _instance.guildDetails.level = json["guild"]["level"].AsInt;
                            _instance.guildDetails.xp = json["guild"]["xp"].AsInt;
                            _instance.guildDetails.status = json["guild"]["status"].AsInt;
                            _instance.guildDetails.weekly = json["guild"]["weekly"].AsInt;
                            _instance.guildDetails.lastWeek = json["guild"]["lastWeek"].AsInt;
                            _instance.guildDetails.memberCount = json["guild"]["memberCount"].AsInt;
                            _instance.guildDetails.guildMaster = playerInfo.nick;

                            _instance.guildDetails.members = new GuildMember[]{
                                new GuildMember {
                                    id = playerInfo.id,
                                    level = playerStats["playerLvl"].AsInt,
                                    nick = playerInfo.nick,
                                    role = Guild.GuildMaster,
                                    loginTime = ServerTime
                                }
                            };
                        }
#if UNITY_EDITOR
                        callback(null, json.ToString());
#else
						callback(null, null);
#endif
                    }
                })
            );
        }

        /// <summary>
        /// memberList, disband, join
        /// </summary>
        public static void guildQuery(GuildAction action, int guildID, EleCellProfileCallback callback)
        {
            if (gameID == null) { callback("GameServer.guildQuery : Game ID not set", null); return; }
            if (_accessToken == null) { callback("GameServer.guildQuery : No access token", null); return; }
            if (action != GuildAction.memberList && action != GuildAction.disband && action != GuildAction.join) return;

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            form.AddField("action", action.ToString());
            form.AddField("guildID", guildID);

            NetworkManager.instance.Query("guild", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        if (action == GuildAction.disband)
                        {
                            _instance._pinfo.guildRole = 99;
                            _instance._pinfo.guildTime = ServerTime;
                        }
                        else if (action == GuildAction.join)
                        {
                            _instance._pinfo.guildRole = 0;
                            _instance._pinfo.guildID = guildID;
                            for (int i = 0; i < GuildSearched.Length; i++)
                            {
                                if (guildID == GuildSearched[i].guildID)
                                {
                                    _instance._pinfo.guildName = GuildSearched[i].guildName;
                                    _instance._pinfo.guildLevel = GuildSearched[i].level;
                                    _instance._pinfo.guildWeekly = GuildSearched[i].weekly;
                                    break;
                                }
                            }
                            _instance._pinfo.guildTime = ServerTime;
                        }
                        else if (action == GuildAction.memberList)
                        {
                            _instance.guildDetails.guildID = json["guild"]["guildID"].AsInt;
                            _instance.guildDetails.guildName = json["guild"]["guildName"];
                            _instance.guildDetails.motto = json["guild"]["motto"];
                            _instance.guildDetails.level = json["guild"]["level"].AsInt;
                            _instance.guildDetails.xp = json["guild"]["xp"].AsInt;
                            _instance.guildDetails.status = json["guild"]["status"].AsInt;
                            _instance.guildDetails.weekly = json["guild"]["weekly"].AsInt;
                            _instance.guildDetails.lastWeek = json["guild"]["lastWeek"].AsInt;
                            _instance.guildDetails.memberCount = json["guild"]["memberCount"].AsInt;
                            _instance.guildDetails.guildMaster = json["guild"]["guildMaster"];

                            _instance.guildDetails.members = new GuildMember[json["members"].Count];
                            for (int i = 0; i < _instance.guildDetails.members.Length; i++)
                            {
                                _instance.guildDetails.members[i] = new GuildMember
                                {
                                    id = json["members"][i]["pid"].AsInt,
                                    level = json["members"][i]["playerLvl"].AsInt,
                                    nick = json["members"][i]["nick"],
                                    role = json["members"][i]["role"].AsInt,
                                    score = json["members"][i]["score"].AsInt,
                                    loginTime = GameTime(json["members"][i]["loginTime"])
                                };
                            };
                        }
#if UNITY_EDITOR
                        callback(null, json.ToString());
#else
						callback(null, null);
#endif
                    }
                })
            );
        }

        /// <summary>
        /// motto
        /// </summary>
        public static void guildQuery(GuildAction action, int guildID, string motto, EleCellProfileCallback callback)
        {
            if (gameID == null) { callback("GameServer.guildQuery : Game ID not set", null); return; }
            if (_accessToken == null) { callback("GameServer.guildQuery : No access token", null); return; }
            if (action != GuildAction.motto) return;

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            form.AddField("action", action.ToString());
            form.AddField("guildID", guildID);
            form.AddField("motto", motto);

            NetworkManager.instance.Query("guild", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        _instance.guildDetails.motto = motto;
                        for (int i = 0; i < GuildSearched.Length; i++)
                        {
                            if (guildID == GuildSearched[i].guildID)
                            {
                                GuildSearched[i].motto = motto;
                                break;
                            }
                        }
#if UNITY_EDITOR
                        callback(null, json.ToString());
#else
						callback(null, null);
#endif
                    }
                })
            );
        }

        /// <summary>
        /// transfer, approve, reject, promote, demote, kick
        /// </summary>
        public static void guildQuery(GuildAction action, int guildID, int member, EleCellProfileCallback callback)
        {
            if (gameID == null) { callback("GameServer.guildQuery : Game ID not set", null); return; }
            if (_accessToken == null) { callback("GameServer.guildQuery : No access token", null); return; }
            if (action != GuildAction.transfer && action != GuildAction.approve && action != GuildAction.reject && action != GuildAction.promote && action != GuildAction.demote && action != GuildAction.kick) return;

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            form.AddField("action", action.ToString());
            form.AddField("guildID", guildID);
            form.AddField("member", member);

            NetworkManager.instance.Query("guild", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        if (action == GuildAction.transfer)
                        {
                            _instance._pinfo.guildRole = 2;
                            for (int i = 0; i < _instance.guildDetails.members.Length; i++)
                            {
                                if (_instance.guildDetails.members[i].id == member)
                                {
                                    _instance.guildDetails.members[i].role = 5;
                                    _instance.guildDetails.guildMaster = _instance.guildDetails.members[i].nick;
                                    break;
                                }
                            }
                        }
                        else if (action == GuildAction.approve)
                        {
                            for (int i = 0; i < _instance.guildDetails.members.Length; i++)
                            {
                                if (_instance.guildDetails.members[i].id == member)
                                {
                                    _instance.guildDetails.members[i].role = 1;
                                    break;
                                }
                            }
                        }
                        else if (action == GuildAction.reject)
                        {
                            _instance.guildDetails.members = _instance.guildDetails.members.Where(x => x.id != member).ToArray();
                        }
                        else if (action == GuildAction.promote)
                        {
                            for (int i = 0; i < _instance.guildDetails.members.Length; i++)
                            {
                                if (_instance.guildDetails.members[i].id == member)
                                {
                                    _instance.guildDetails.members[i].role = 2;
                                    break;
                                }
                            }
                        }
                        else if (action == GuildAction.demote)
                        {
                            for (int i = 0; i < _instance.guildDetails.members.Length; i++)
                            {
                                if (_instance.guildDetails.members[i].id == member)
                                {
                                    _instance.guildDetails.members[i].role = 1;
                                    break;
                                }
                            }
                        }
                        else if (action == GuildAction.kick)
                        {
                            _instance.guildDetails.members = _instance.guildDetails.members.Where(x => x.id != member).ToArray();
                        }
#if UNITY_EDITOR
                        callback(null, json.ToString());
#else
						callback(null, null);
#endif
                    }
                })
            );
        }

        // deprecated
        public static void galleryQuery(string item, EleCellProfileCallback callback)
        {
            if (gameID == null) { callback("GameServer.galleryQuery : Game ID not set", null); return; }
            if (_accessToken == null) { callback("GameServer.galleryQuery : No access token", null); return; }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            form.AddField("action", item);

            NetworkManager.instance.Query("gallery", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        _instance._galleryList[item].AsInt = _instance._galleryList[item].AsInt + 1;
                        callback(null, (string)_instance._galleryList[item]);
                    }
                })
            );
        }


        int GetPetPos(int petID)
        {
            for (int i = 0; i < _petList.Length; i++)
            {
                if (_petList[i].id == petID) return i;
            }
            return 0;
        }

        int GetBuildPos(int bID)
        {
            for (int i = 0; i < _buildList.Length; i++)
            {
                if (_buildList[i].id == bID) return i;
            }
            return 0;
        }

        int GetUpgradeSec(int brID)
        {
            for (int i = 0; i < _bReq["array"].Count; i++)
            {
                if (_bReq["array"][i]["brID"].AsInt == brID)
                {
                    return _bReq["array"][i]["buildSec"].AsInt;
                }
            }
            return 0;
        }

        JSONNode GetTask(int rID)
        {
            for (int i = 0; i < _alchemyRecipes["array"].Count; i++)
            {
                if (_alchemyRecipes["array"][i]["rID"].AsInt == rID)
                {
                    return _alchemyRecipes["array"][i];
                }
            }
            return null;
        }

        public int GetBuildUpgrade(int brID)
        {
            for (int i = 0; i < _bReq["array"].Count; i++)
            {
                if (_bReq["array"][i]["brID"].AsInt == brID)
                {
                    return i;
                }
            }
            return 0;
        }

        public static void startBattle(int map, string[] item, int friendID, EleCellProfileCallback callback)
        {
            if (gameID == null) { callback("GameServer.startBattle : Game ID not set", null); return; }
            if (_accessToken == null) { callback("GameServer.startBattle : No access token", null); return; }
            if (map < 1) { callback("GameServer.startBattle : invalid map ID", null); return; }

            if (friendID > 0)
            {
                for (int i = 0; i < instance._friendList.Length; i++)
                {
                    if (instance._friendList[i].id == friendID)
                    {
                        if (instance._friendList[i].cooldown > ServerTime)
                        {
                            callback("GameServer.startBattle : friend still in cooldown", null); return;
                        }
                        break;
                    }
                }
            }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            form.AddField("action", "create");
            form.AddField("friend", friendID);
            form.AddField("map", map);
            if (item != null) form.AddField("item", string.Join(",", item));

            NetworkManager.instance.Query("battle", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        _instance._lastBattle = new Battle();
                        _instance._lastBattle.id = json["battleID"].AsInt;
                        _instance._lastBattle.friend = json["friend"].AsInt;
                        _instance._lastBattle.map = json["mapID"].AsInt;
                        _instance._lastBattle.config = (string)json["config"];
                        _instance._lastBattle.reward = (string)json["reward"];
                        _instance._lastBattle.xp = json["xp"].AsInt;

                        if (_instance._lastBattle.friend > 0)
                        {
                            bool inList = false;

                            for (int i = 0; i < _instance._friendList.Length; i++)
                            {
                                if (_instance._friendList[i].id == _instance._lastBattle.friend)
                                {
                                    inList = true;
                                    _instance._friendList[i].cooldown = ServerTime.AddHours(3);
                                    break;
                                }
                            }
                            _instance._pStats["credit"].AsInt = _instance._pStats["credit"].AsInt + (inList ? 10 : 5);
                        }

#if UNITY_EDITOR
                        callback(null, json.ToString());
#else
						callback(null,null);
#endif
                    }
                })
            );
        }

        public static void reportBattle(string result, EleCellProfileCallback callback)
        {
            if (gameID == null) { callback("GameServer.reportBattle : Game ID not set", null); return; }
            if (_accessToken == null) { callback("GameServer.reportBattle : No access token", null); return; }
            if (instance._lastBattle.id < 1) { callback("GameServer.reportBattle : invalid battle ID", null); return; }


            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            form.AddField("action", "report");
            form.AddField("battleID", _instance._lastBattle.id);
            //result = postData.Encrypt(result);
            form.AddField("result", result);

            NetworkManager.instance.Query("battle", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        _instance._pStats["playerLvl"].AsInt = json["level"].AsInt;
                        _instance._pStats["xp"].AsInt = _instance._pStats["xp"].AsInt + json["battleXP"].AsInt;
                        _instance._pStats["xplow"].AsInt = json["xplow"].AsInt;
                        _instance._pStats["xphi"].AsInt = json["xp"].AsInt;

                        _instance.CheckTrophy(0, "playerLvl");
                        callback(null, json.ToString());
                    }
                })
            );
        }

        public static void reportBattle(JSONClass result, EleCellJsonCallback callback)
        {
            if (gameID == null) { callback("GameServer.reportBattle : Game ID not set", null); return; }
            if (_accessToken == null) { callback("GameServer.reportBattle : No access token", null); return; }
            //if (instance._lastBattle.id < 1) {	callback("GameServer.reportBattle : invalid battle ID", null); return;	}

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            form.AddField("action", "report");
            form.AddField("battleID", _instance._lastBattle.id);
            //result = postData.Encrypt(result);

            for (int i = 0; i < result.Count; i++)
            {
                if (result[i].Count > 0)
                    form.AddField(result.getKeyAt(i), (JSONClass)result[i]);
                else
                    form.AddField(result.getKeyAt(i), result[i]);
            }

            NetworkManager.instance.Query("battle", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        _instance._pStats["playerLvl"].AsInt = json["level"].AsInt+1;
                        _instance._pStats["xp"].AsInt = _instance._pStats["xp"].AsInt + json["battleXP"].AsInt;
                        _instance._pStats["xplow"].AsInt = json["xplow"].AsInt;
                        _instance._pStats["xphi"].AsInt = json["xp"].AsInt;

                        _instance.CheckTrophy(0, "playerLvl");
                        callback(null, json);
                    }
                })
            );
        }

        //public void PlayerSteta(JSONClass result, EleCellJsonCallback callback)
        //{
        //    if (gameID == null) { callback("GameServer.reportBattle : Game ID not set", null); return; }
        //    if (_accessToken == null) { callback("GameServer.reportBattle : No access token", null); return; }

        //    TcpForm form = new TcpForm();
        //    form.AddField("game", gameID);
        //    form.AddField("accessToken", _accessToken);
        //    form.AddField("action", "report");
        //    form.AddField("battleID", _instance._lastBattle.id);

        //    for (int i = 0; i < result.Count; i++)
        //    {
        //        if (result[i].Count > 0)
        //            form.AddField(result.getKeyAt(i), (JSONClass)result[i]);
        //        else
        //            form.AddField(result.getKeyAt(i), result[i]);
        //    }

        //    NetworkManager.instance.Query("battle", form,
        //        new GenericCallback<JSONClass>(delegate (JSONClass json) {
        //            if (json["error"] != null)
        //            {
        //                callback(json["error"], null);
        //            }
        //            else
        //            {
        //                _instance._pStats["playerLvl"].AsInt = json["level"].AsInt+1;
        //                _instance._pStats["xp"].AsInt = _instance._pStats["xp"].AsInt + json["battleXP"].AsInt;
        //                _instance._pStats["xplow"].AsInt = json["xplow"].AsInt;
        //                _instance._pStats["xphi"].AsInt = json["xp"].AsInt;

        //                _instance.CheckTrophy(0, "playerLvl");
        //                callback(null, json);
        //            }
        //        })
        //    );
        //}

        public static void buyHeroStats(int heroID, int stats, int price, EleCellProfileCallback callback)
        {
            if (gameID == null)
            {
                callback("GameServer.buyHeroStats : Game ID not set", null); return;
            }
            if (_accessToken == null)
            {
                callback("GameServer.buyHeroStats : No access token", null); return;
            }

            if (heroID < 1)
            {
                callback("GameServer.buyHeroStats : invalid Hero ID", null); return;
            }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            form.AddField("hid", heroID);
            form.AddField("coin", price);
            if (stats > 5)
            {
                form.AddField("action", "buySkill");
                form.AddField("stats", "sk" + (stats - 5).ToString());
            }
            else
            {
                form.AddField("action", "buyStats");
                form.AddField("stats", "sta" + (stats + 1).ToString());
            }

            NetworkManager.instance.Query("hero", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        for (int i = 0; i < _instance._heroList.Length; i++)
                        {
                            if (_instance._heroList[i].id == heroID)
                            {
                                if (stats > 5)
                                    _instance._heroList[i].skill[stats - 6]++;
                                else
                                    _instance._heroList[i].stats[stats]++;

                                break;
                            }
                        }
                        _instance._pStats["coin"].AsInt = _instance._pStats["coin"].AsInt - price;

#if UNITY_EDITOR
                        callback(null, json.ToString());
#else
						callback(null,null);
#endif
                    }
                })
            );
        }

        public static void statsQuery(StatsQuery query, EleCellProfileCallback callback)
        { // not to be used after the Sheep games
            if (gameID == null) { callback("GameServer.statsQuery : Game ID not set", null); return; }
            if (_accessToken == null) { callback("GameServer.statsQuery : No access token", null); return; }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);

            switch (query.action)
            {
                case StatsAction.spend:
                    form.AddField("action", "spend");
                    break;
                case StatsAction.set:
                    form.AddField("action", "set");
                    break;
                case StatsAction.earn:
                    form.AddField("action", "earn");
                    break;
            }
            form.AddField("field", query.field);
            form.AddField("value", query.value);

            NetworkManager.instance.Query("statsUpdate", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        switch (query.action)
                        {
                            case StatsAction.spend:
                                if (_instance._pStats[query.field].AsInt - (int)query.value >= 0)
                                {
                                   
#if UNITY_EDITOR
                                    Debug.Log("Server spend coin : " + _instance._pStats[query.field].AsInt + " - " + query.value + " = " + (_instance._pStats[query.field].AsInt - (int)query.value)  );
#endif
                                    _instance._pStats[query.field].AsInt = _instance._pStats[query.field].AsInt - (int)query.value;
                                }
                                break;
                            case StatsAction.set:
                                _instance._pStats[query.field].AsInt = (int)query.value;
                                _instance.CheckTrophy(0, query.field);
                                break;
                            case StatsAction.earn:
                                _instance._pStats[query.field].AsInt = _instance._pStats[query.field].AsInt + (int)query.value;
                                _instance.CheckTrophy(0, query.field);
                                break;
                        }
#if UNITY_EDITOR
                        callback(null, json.ToString());
#else
						callback(null,null);
#endif
                    }
                })
            );
        }

        public static void statsEasyQuery(StatsQuery query, EleCellEasyCallback callback)
        { // not to be used after the Sheep games
            if (gameID == null) { callback("GameServer.statsQuery : Game ID not set", null); return; }
            if (_accessToken == null) { callback("GameServer.statsQuery : No access token", null); return; }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);

            switch (query.action)
            {
                case StatsAction.spend:
                    form.AddField("action", "spend");
                    break;
                case StatsAction.set:
                    form.AddField("action", "set");
                    break;
                case StatsAction.earn:
                    form.AddField("action", "earn");
                    break;
            }
            form.AddField("field", query.field);
            form.AddField("value", query.value);

            NetworkManager.instance.Query("statsUpdate", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        switch (query.action)
                        {
                            case StatsAction.spend:
                                _instance._pStats[query.field].AsInt = _instance._pStats[query.field].AsInt - (int)query.value;
                                break;
                            case StatsAction.set:
                                _instance._pStats[query.field].AsInt = (int)query.value;
                                _instance.CheckTrophy(0, query.field);
                                break;
                            case StatsAction.earn:
                                _instance._pStats[query.field].AsInt = _instance._pStats[query.field].AsInt + (int)query.value;
                                _instance.CheckTrophy(0, query.field);
                                break;
                        }


                        InGameCurrency igc_field = InGameCurrency.tickC;
                        switch (query.field)
                        {
                            case "coin":
                                igc_field = InGameCurrency.coin;
                                break;
                            case "gem":
                                igc_field = InGameCurrency.gem;
                                break;
                        }

#if UNITY_EDITOR
                        callback(null, json.ToString(), igc_field, (int)query.value);
#else
callback(null,null, igc_field,  (int)query.value);
#endif
                    }
                })
            );
        }



        public static void currencyExchange(string from, string to, double pay, EleCellProfileCallback callback)
        {
            if (gameID == null)
            {
                callback("GameServer.currencyExchange : Game ID not set", null); return;
            }
            if (_accessToken == null)
            {
                callback("GameServer.currencyExchange : No access token", null); return;
            }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            form.AddField("from", from);
            form.AddField("to", to);
            form.AddField("pay", pay);

            NetworkManager.instance.Query("exchange", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        callback(null, (string)json["receive"]);
                    }
                })
            );
        }

        public static void addXP(int xp, EleCellProfileCallback callback)
        {
            if (gameID == null)
            {
                callback("GameServer.addXP : Game ID not set", null); return;
            }
            if (_accessToken == null)
            {
                callback("GameServer.addXP : No access token", null); return;
            }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            form.AddField("xp", xp);

            NetworkManager.instance.Query("sheepXp", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        _instance._pStats["playerLvl"].AsInt = json["level"].AsInt+1;
                        _instance._pStats["xp"].AsInt = _instance._pStats["xp"].AsInt + xp;
                        _instance._pStats["xplow"].AsInt = json["xplow"].AsInt;
                        _instance._pStats["xphi"].AsInt = json["xp"].AsInt;

                        _instance.CheckTrophy(0, "playerLvl");
#if UNITY_EDITOR
                        callback(null, json.ToString());
#else
						callback(null,null);
#endif
                    }
                })
            );
        }

        public static void getTransactionID(string sku, EleCellProfileCallback callback)
        {
            if (gameID == null) { callback("GameServer.getTransactionID : Game ID not set", null); return; }
            if (_accessToken == null) { callback("GameServer.getTransactionID : No access token", null); return; }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            form.AddField("sku", sku);


#if UNITY_ANDROID && !UNITY_EDITOR
			NetworkManager.instance.Query("iap-gp", form,
#elif UNITY_IOS || UNITY_EDITOR
            NetworkManager.instance.Query("iap-ios", form,
#endif
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        callback(null, (string)json["payload"]);
                    }
                })
            );

        }

        public static void verifyTransaction(string transactionID, string sku, string receipt, string signature, EleCellProfileCallback callback)
        {
            if (gameID == null) { callback("GameServer.getTransactionID : Game ID not set", null); return; }
            if (_accessToken == null) { callback("GameServer.getTransactionID : No access token", null); return; }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            form.AddField("sku", sku);
            form.AddField("tid", int.Parse(transactionID));
            form.AddField("receipt", receipt);
            form.AddField("signature", signature);


#if UNITY_ANDROID && !UNITY_EDITOR
			NetworkManager.instance.Query("iap-gp", form,
#elif UNITY_IOS || UNITY_EDITOR
            NetworkManager.instance.Query("iap-ios", form,
#endif
                new GenericCallback<JSONClass>(delegate (JSONClass json) {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        callback(null, (string)json["sku"]);
                    }
                })
            );

        }

        private float saveTimer = 0f;

        public static void LoadServerTime(EleCellProfileCallback callback)
        {
            NetworkManager.instance.Query("serverTime", new TcpForm(), new GenericCallback<JSONClass>(delegate (JSONClass json) {
                if (json["error"] != null)
                {
                    callback(json["error"], null);
                }
                else
                {
                    if (_instance._secondsPassed != null) _instance._secondsPassed.release();

                    _instance._secondsPassed = new LogTime(1f, 1f);
                    if (System.DateTime.TryParseExact(json["time"], "MM-dd-yyyy HH:mm:ss", System.Globalization.DateTimeFormatInfo.CurrentInfo, System.Globalization.DateTimeStyles.None, out _instance._serverTime))
                    {
                        callback(null, json["time"]);
                    }
                    else
                    {
                        _instance._serverTime = System.DateTime.Now;
                        callback("Failed to parse server time", json["time"]);
                    }

                }
            }));
        }

        private System.DateTime _serverTime;
        private LogTime _secondsPassed;
        public static System.DateTime ServerTime { get { return _instance._serverTime.AddSeconds(_instance._secondsPassed); } }

        void LateUpdate()
        {
            saveTimer += Time.deltaTime * 0.5f;

            if (saveTimer < 1f) return;

            saveTimer = 0f;

            if (_cloudsaving)
            {
                _cloudsaving = false;
                Debug.Log(_saveString);
                StartCoroutine(uploadSaveWWW(_saveString, cloudSaveCB));
            }
        }

        public static string debugText = "Debug Mode"; // void OnGUI () { GUI.Label (new Rect (10,40,500,40), debugText);	}

        void CheckTrophy(int category, string field)
        {
            if (trophyAlert == null) return;

            for (int i = 0; i < _trophy.Length; i++)
            {
                if (!_trophy[i].collected && _trophy[i].category == category && _trophy[i].field == field)
                {
                    bool meet = ((category == 0) ? _pStats[field].AsInt : _achieve[field].AsInt) >= _trophy[i].count;
                    if (meet) trophyAlert(null, _trophy[i].TrophyName);
                    return;
                }
            }
        }

        public static void EmptyCallback(string e, string m)
        {
#if UNITY_EDITOR
            if (e != null) Debug.Log("error: " + e);
            if (m != null) Debug.Log("message: " + m);
#endif
        }

        static System.DateTime GameTime(string timeString)
        {
            System.DateTime d;
            if (System.DateTime.TryParseExact(timeString, "yyyy-MM-dd HH:mm:ss", System.Globalization.DateTimeFormatInfo.CurrentInfo, System.Globalization.DateTimeStyles.None, out d))
                return d;
            else
                return ServerTime;
        }

        /// <summary>
        /// 重置隨機影片獎勵次數
        /// </summary>
        public static void ResetRandomReward()
        {
            int vedioLimit = ItemList["RandomReward"].AsInt;
            if (vedioLimit > 0)
            {
                itemQuery(new ItemQuery(ItemAction.use, "RandomReward", vedioLimit), new EleCellProfileCallback(delegate (string err, string message)
                {
                    if (err == null)
                    {
                        Debug.Log("use success");
                        Debug.Log("now Count:" + ItemList["RandomReward"].AsInt);
                    }
                }));
            }
        }

        public static void adGetGem(EleCellJsonCallback callback)//
        {
            if (gameID == null) { callback("GameServer.reportBattle : Game ID not set", null); return; }
            if (_accessToken == null) { callback("GameServer.reportBattle : No access token", null); return; }

            TcpForm form = new TcpForm();
            form.AddField("game", gameID);
            form.AddField("accessToken", _accessToken);
            form.AddField("action", "adGetGem");

            NetworkManager.instance.Query("battle", form,
                new GenericCallback<JSONClass>(delegate (JSONClass json)
                {
                    if (json["error"] != null)
                    {
                        callback(json["error"], null);
                    }
                    else
                    {
                        //json["gem"].AsInt 目前Server 寶石數量
                        //json["getGem"].AsInt 本次廣告寶石數量
                        //Debug.Log("GetGem gem = " + json["gem"].AsInt + "   getGem = " + json["getGem"].AsInt);
                        _instance._pStats["gem"].AsInt = json["gem"].AsInt;
                        callback(null, json);
                    }
                })
            );
        }
    }

    public enum ChatChannel
    {
        System,
        World,
        Guild,
        All
    }
}