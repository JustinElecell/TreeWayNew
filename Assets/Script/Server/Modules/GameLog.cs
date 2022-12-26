using UnityEngine;
using System.Collections;
using GameSecurity;

public class GameLog {

	public static void AddLog(string key, string text){
		if (PlayerPrefs.HasKey(key)){
			try {
				PlayerPrefs.SetString(key,StringCipher.Encrypt(StringCipher.Decrypt(PlayerPrefs.GetString(key)) + '|' + text));
			}
			catch {
				PlayerPrefs.SetString(key, StringCipher.Encrypt(text));
			}
		}
		else {
			PlayerPrefs.SetString(key, StringCipher.Encrypt(text));
		}
	}
	
	public static string GetLog(string key){
		if (PlayerPrefs.HasKey(key)){
			try {
				return StringCipher.Decrypt(PlayerPrefs.GetString(key));
			}
			catch {
				PlayerPrefs.DeleteKey(key);
				return null;
			}
		}
		else {
			return null;
		}
	}
	
	public static void ClearLog(string key){
		if (PlayerPrefs.HasKey(key))
			PlayerPrefs.DeleteKey(key);
	}
}
