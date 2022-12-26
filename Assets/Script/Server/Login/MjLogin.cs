using UnityEngine;
using System.Collections;
using EleCellLogin;

public class MjLogin : MonoBehaviour {
	
	public enum LoginType{
		Normal,
		FB,
		Google,
		Register,
		Recover
	}
	
	public LoginType type;
	public static bool newAccount = false;
	
	public GameObject parentPanel;

	void Start(){
		
		#if !UNITY_ANDROID
		if (type == LoginType.Google) gameObject.SetActive(false);
		#endif
	}
	
	public void OnPress(bool pressed){
        //if (pressed){
        //	if (type != LoginType.Normal)  mySprite.color = Color.gray;
        //	return;
        //}

        //if (type != LoginType.Normal) mySprite.color = Color.white;
        //DarkTonic.MasterAudio.MasterAudio.PlaySoundAndForget("Click");

        //if (type == LoginType.Normal || type == LoginType.Recover)
        //	newAccount = false;
        //else
        //	newAccount = true;

        //if (type == LoginType.Normal){
        //	if (MjSave.instance.playerName == null) MjSave.instance.playerName = "DSCPlayer";
        //	parentPanel.SetActive(false);
        //	Loading.Show();
        //	GameServer.qlogin(MjSave.instance.playerName,SystemSettings.SettingData.instance.language, loginFinished);
        //}

        //if (type == LoginType.FB){
        //	MjSave.instance.playerName = "DSCPlayer" + Random.Range(1001,9999).ToString();				
        //	parentPanel.SetActive(false);
        //	Loading.Show();				
        //	GameServer.loginFB(SystemSettings.SettingData.instance.language, loginFinished);
        //}

        //if (type == LoginType.Google){
        //	MjSave.instance.playerName = "DSCPlayer" + Random.Range(1001,9999).ToString();			
        //	parentPanel.SetActive(false);
        //	Loading.Show();			
        //	GameServer.loginGoogle(SystemSettings.SettingData.instance.language, loginFinished);
        //}		

        //if (type == LoginType.Recover)
        //{
        //    if (RecoverPanel.instance.id.value.Length < 9) return;
        //    if (RecoverPanel.instance.pass.value.Length < 1) return;

        //    parentPanel.SetActive(false);
        //    Loading.Show();
        //    GameServer.rlogin(RecoverPanel.instance.id.value + RecoverPanel.instance.pass.value, SystemSettings.SettingData.instance.language, loginFinished);
        //}

        //if (type == LoginType.Register){

        //	GameServer.instance.firstStartFlag = true;

        //	MjSave.instance.playerName = "DSCPlayer" + Random.Range(1001,9999).ToString();			
        //	parentPanel.SetActive(false);
        //	Loading.Show();	
        //	if (PlayerPrefs.HasKey("log")) PlayerPrefs.DeleteKey("log");
        //	GameServer.qlogin(MjSave.instance.playerName,SystemSettings.SettingData.instance.language, loginFinished);
        //}

    }
	
	void loginFinished(string error, string message){
		//#if UNITY_EDITOR
		//if (error!=null)
		//	Debug.Log (error);
		//if (message!=null)
		//	Debug.Log (message);
		
		//Debug.Log (GameServer.AccessToken);
		//#endif
		
		//if (error!=null) {
		//	Loading.Hide();
			
		//	if (error == "Device not matched"){
		//		TextEffect.Notice(Language.text("deviceErr"));
		//		parentPanel.SetActive(true);
		//		MjInit.instance.normalLogin.SetActive(false);
		//		MjInit.instance.newAccount.SetActive(true);
		//	}
		//	else if (error == "Incorrect ID/Password"){
		//		TextEffect.Notice(Language.text("passErr"));
		//		parentPanel.SetActive(true);
		//	}
		//	else {
		//		MjInit.OfflineStart("login:"+error);
		//	}
			
		//	return;
		//}
		
		//string[] nick = GameServer.playerInfo.nick.Split('|');
		
		//MjSave.instance.playerName = nick[0];
		//if (nick.Length > 1 && MjConfig.Instance.titleBack.ContainsKey(nick[1]))
		//	MjSave.instance.playerTitle = nick[1];
		//else
		//	MjSave.instance.playerTitle = null;
		
		//MjSave.instance.playerID = GameServer.playerInfo.pid;
		//MjSave.instance.fbID = GameServer.playerInfo.fbID;
		//MjSave.instance.googleID = GameServer.playerInfo.googleID;

		//MjInit.StartGame(message);
	}
}
