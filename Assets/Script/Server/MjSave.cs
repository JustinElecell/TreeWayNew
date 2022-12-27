using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using GameSecurity;
using System;
using EleCellLogin;

public class MjSave {
	
	public static bool cloudLoading;
	
	private static MjSave _instance;
	
	public static MjSave instance{
		get{
			if (_instance == null)
				_instance = new MjSave();
			
			return _instance;
		}
	}
	
	public string playerID;
	public string playerName;
	public string playerTitle;
	
	public string fbID;
	public string googleID;
	
	public int tutorialStage;
	
	public safeInt level;
	public safeInt xp;
	public safeDouble coin;
	public safeInt credit;
	public safeInt gem;
	public safeInt ticketC;
	public safeInt ticketG;
	
	public safeDouble topCoin;
	public safeInt topPoint;

	public safeInt win;

	public string xpUp;
	public string coinUp;

	public string wallpaper;
	public string tileback;

	//public LogTime totalPlayingTime;
	
	public DateTime timeStamp;
	public DateTime saveTime;
	
	private JSONClass saveJSON;
	
	public MjSave(){
		playerID = "";
		playerName = "";
		fbID = "";
		googleID = "";
		level = 1;
		coin = 1000;
		credit = 1000;
		gem = 10;
		xp = 0;

		ticketC = 0;
		ticketG = 0;

		topCoin = 0;
		topPoint = 0;

		win = 0;

		xpUp = null;
		coinUp = null;

		wallpaper = null;
		tileback = null;

		//totalPlayingTime = new LogTime();
		//totalPlayingTime.renameLooper("Total Playing Time");
		
		saveJSON = new JSONClass();
	}
	
	public static bool canAfford(ItemCost cost){
		switch (cost.currency){
		case InGameCurrency.coin: return instance.coin >= cost.amount;
		case InGameCurrency.credit: return instance.credit >= cost.amount;
		case InGameCurrency.gem: return instance.gem >= cost.amount;
			
		}
		return false;
	}
	
	//public static void pay(ItemCost cost){
	//	switch (cost.currency){
	//	case InGameCurrency.coin: instance.coin -= cost.amount; break;
	//	case InGameCurrency.credit: instance.credit -= (int) cost.amount; break;
	//	case InGameCurrency.gem: instance.gem -= (int) cost.amount; break;
	//	case InGameCurrency.tickC: instance.ticketC -= (int)  cost.amount; break;
	//	case InGameCurrency.tickG: instance.ticketG -= (int) cost.amount; break;
	//	}

	//	if (MjHUD.instance != null)
	//		MjHUD.instance.updateHUD();		
	//}

	//public static void pay(InGameCurrency currency, double amount){
	//	switch (currency){
	//	case InGameCurrency.coin: instance.coin -= amount; break;
	//	case InGameCurrency.credit: instance.credit -= (int) amount; break;
	//	case InGameCurrency.gem: instance.gem -= (int) amount; break;
	//	case InGameCurrency.tickC: instance.ticketC -= (int)  amount; break;
	//	case InGameCurrency.tickG: instance.ticketG -= (int) amount; break;
	//	}

	//	if (MjHUD.instance != null)
	//		MjHUD.instance.updateHUD();		
	//}
	
	//public static void earnCurrency(InGameCurrency currency, int amount){
	//	switch (currency){
	//	case InGameCurrency.coin: instance.coin += amount; break;
	//	case InGameCurrency.credit: instance.credit += amount; break;
	//	case InGameCurrency.gem: instance.gem += amount; break;
	//	case InGameCurrency.tickC: instance.ticketC += amount; break;
	//	case InGameCurrency.tickG: instance.ticketG += amount; break;
	//	}

	//	if (MjHUD.instance != null)
	//		MjHUD.instance.updateHUD();		

	//}

	//public static void earnCurrency(InGameCurrency currency, double amount){
	//	switch (currency){
	//	case InGameCurrency.coin: instance.coin += amount; break;
	//	case InGameCurrency.credit: instance.credit += (int) amount; break;
	//	}

	//	if (MjHUD.instance != null)
	//		MjHUD.instance.updateHUD();		

	//}
	
	public static void save(){
		JSONClass json = new JSONClass();
		
		//json["timestamp"] = DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss");
		
		json["saveVersion"].AsInt = 1;
		
		json["playerID"] = instance.playerID;
		json["playerName"] = instance.playerName;
		json["fbID"] = instance.fbID;
		json["googleID"] = instance.googleID;
		
		json["level"].AsInt = instance.level;
		json["xp"].AsInt = instance.xp;
		json["coin"] = instance.coin.ToString();
		json["credit"].AsInt = instance.credit;
		json["gem"].AsInt = instance.gem;
		json["ticketC"].AsInt = instance.ticketC;
		json["ticketG"].AsInt = instance.ticketG;

		instance.saveTime = DateTime.Now;
		json["timeStamp"] = GameServer.ServerTime.ToString("MM-dd-yyyy HH:mm:ss");
		
		//json["sound"].AsFloat = SettingData.instance.sfxVolume;
		//json["music"].AsFloat = SettingData.instance.musicVolume;
		//json["sensitivity"].AsFloat = SettingData.instance.sensitivity;
		//json["lang"] = SettingData.instance.language;
		
		//json["cloudSave"].AsInt = SettingData.instance.cloudSave;
		//json["nametag"].AsInt = SettingData.instance.nametag;
		
		json["hash"] = EncryptionHash.Md5Sum(instance.playerID + instance.playerName + instance.coin.ToString() + instance.gem.ToString() );

		json["wallpaper"] = instance.wallpaper;
		json["tileback"] = instance.tileback;

		try{
			//PlayerPrefs.SetString("data",StringCipher.Encrypt(json.ToString()));
			PlayerPrefs.SetString("data",json.ToString());
			
			#if UNITY_EDITOR
			Debug.Log("Game saved at: " + DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss"));
			#endif
		}
		catch {
			#if UNITY_EDITOR
			Debug.Log("Failed to save");
			#endif
		}	
	}
	
	public static void UpdateStats(JSONClass json){
		if (json == null) return;
		
		instance.level = json["playerLvl"].AsInt+1;
		instance.xp = json["xp"].AsInt ;
        instance.coin = double.Parse(json["coin"]);
		instance.credit= json["credit"].AsInt ;
		instance.gem = json["gem"].AsInt ;
		instance.ticketC = json["tickC"].AsInt;
		instance.ticketG = json["tickG"].AsInt;
		
		instance.tutorialStage = json["tutorialStage"].AsInt;
		instance.topPoint = json["topPoint"].AsInt;
		instance.topCoin = double.Parse(json["topCoin"]);

		instance.win = json["win"].AsInt;
	}

	//public void FetchItem(){
	//	coinUp = null;
	//	xpUp = null;

	//	for (int i = 0; i < P5UI_210_Mall.coinUp.Count; i++) {
	//		if (GameServer.ItemList[P5UI_210_Mall.coinUp[i]] != null && GameServer.ItemList[P5UI_210_Mall.coinUp[i]].AsInt > 0) {
	//			coinUp = P5UI_210_Mall.coinUp[i];
	//			break;
	//		}				
	//	}

	//	for (int i = 0; i < P5UI_210_Mall.xpUp.Count; i++) {
	//		if (GameServer.ItemList[P5UI_210_Mall.xpUp[i]] != null && GameServer.ItemList[P5UI_210_Mall.xpUp[i]].AsInt > 0) {
	//			xpUp = P5UI_210_Mall.xpUp[i];
	//			break;
	//		}				
	//	}
	//}
	
	public static void load(){
		if (!PlayerPrefs.HasKey("data")) return;
		
		string saveString = PlayerPrefs.GetString("data");
		
		string decryptedString;
		
		JSONClass json = new JSONClass();

		if (saveString [0] == '{') {
			json = (JSONClass) JSONNode.Parse(saveString);
		} else {
			try {
				decryptedString = StringCipher.Decrypt(saveString);

				json = (JSONClass) JSONNode.Parse(decryptedString);

				if (json["saveVersion"].AsInt > 0){ // if hash check is implemented:
					instance.playerID = json["playerID"] ;
					instance.playerName = json["playerName"] ;

					if ( (String) json["hash"] != EncryptionHash.Md5Sum(instance.playerID + instance.playerName + (string)json["coin"] + (string)json["gem"] ) ){
						throw new Exception();
					}
					else {
						PlayerPrefs.SetString("backup",saveString);
					}
				}
				else 
					PlayerPrefs.SetString("backup",saveString);
			}
			catch {
				if (PlayerPrefs.HasKey("backup")){
					saveString = PlayerPrefs.GetString("backup");
					try {
						decryptedString = StringCipher.Decrypt(saveString);
						json = (JSONClass) JSONNode.Parse(decryptedString);
						PlayerPrefs.SetString("data",saveString);
					}
					catch {
						return;
					}				
				}
				else 
					return;
			}
		}
		
		if (json != null && json ["lang"] != null) {
			instance.playerID = json ["playerID"];
			instance.playerName = json ["playerName"];
			instance.fbID = json ["fbID"];
			instance.googleID = json ["googleID"];
			
			if (instance.fbID == null)
				instance.fbID = "";
			if (instance.googleID == null)
				instance.googleID = "";
			

			//SettingData.instance.sfxVolume = json ["sound"].AsFloat;
			//SettingData.instance.musicVolume = json ["music"].AsFloat;
			//SettingData.instance.language = json ["lang"];
			//SettingData.instance.sensitivity = json["sensitivity"].AsFloat;
			//SettingData.instance.cloudSave = json ["cloudSave"].AsInt;
			
			//if (json["nametag"]!=null) SettingData.instance.nametag = json["nametag"].AsInt;

			instance.wallpaper = json["wallpaper"];
			instance.tileback = json["tileback"];

			instance.tutorialStage = json ["tutorialStage"].AsInt;
			
			instance.saveJSON = json;
			
			#if UNITY_EDITOR
			Debug.Log (json.ToString ());
			Debug.Log (json.ToString ().Length);
			#endif
		}
	}

	public static void SetGameSpeed(float speed){
		//Time.timeScale = speed + 1f;
	}

	public static void Reset(){
		_instance = new MjSave ();
	}
}
