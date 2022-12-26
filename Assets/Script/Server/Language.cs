using UnityEngine;
using System;
using System.Collections;
using SimpleJSON;

namespace TextLocalization {

	public class Language {

		private static Language _instance;
        public static Action OnLanguageUpdate;
		public static Language instance{
			get {
				if (_instance == null){
					_instance = new Language();
					_instance.dialogJSON = (JSONClass) JSONNode.Parse(((TextAsset) Resources.Load("Text/dialog")).text);
				}
				return _instance;
			}
		}
		
		private JSONClass txtJSON;
		private JSONClass dialogJSON;
		
		public static void setLanguage(string text){
			instance.txtJSON = (JSONClass) JSONNode.Parse(text);
            onUpdate();
		}
		
		public static void loadFromResources(string file){
			string text = ((TextAsset) Resources.Load(file)).text;
			instance.txtJSON = (JSONClass) JSONNode.Parse(text);
            onUpdate();
		}

        static void onUpdate()
        {
            if (OnLanguageUpdate != null)
                OnLanguageUpdate();
        }
		
		public static string text(string txt){
			if (instance.txtJSON == null) return txt;
			
			string t = instance.txtJSON[txt];
			if (t==null) return txt;
			return t;
		}
		
		public static void appendLanguage(string text){
			instance.txtJSON.AppendParse(text);
			#if UNITY_EDITOR
			Debug.Log(instance.txtJSON.ToString());
			#endif
		}
		
		//public static Dialog dialog(string id, GenericCallback<bool> callback = null){
		//	string d = instance.dialogJSON[id][SettingData.instance.language];
		//	if (d==null) d = id;
			
		//	return new Dialog(d,callback);
		//}
		//public static Dialog dialog(string id,string NN, GenericCallback<bool> callback = null){
		//	string d = instance.dialogJSON[id][SettingData.instance.language];
		//	if (d==null) d = id;
			
		//	return new Dialog(d.Replace("NN",NN),callback);
		//}
		//public static Dialog dialog(string id,string NN,string PP, GenericCallback<bool> callback = null){
		//	string d = instance.dialogJSON[id][SettingData.instance.language];
		//	if (d==null) d = id;
		
		//	return new Dialog(d.Replace("NN",NN).Replace("PP",PP),callback);
		//}
		
		public static string text(params string[] txt){
			if (instance.txtJSON == null) return string.Join("",txt);
			
			string result = "";
			string t;
			for (int i = 0 ; i< txt.Length; i++) {
				t = instance.txtJSON[txt[i]];
				if (t==null) 
					result += txt[i];
				else
					result += t;
			}
			return result;
		}
		
		public static string timer(int seconds){
			int minute = seconds / 60;
			seconds -= minute * 60;
			int hr = minute / 60;
			minute -= hr * 60;
			
			return hr.ToString().PadLeft(2,'0')+':'+minute.ToString().PadLeft(2,'0')+':'+seconds.ToString().PadLeft(2,'0');
		}

		public static string shortTimer(int seconds){
			int minute = seconds / 60;
			seconds -= minute * 60;
			
			return minute.ToString()+" : "+seconds.ToString().PadLeft(2,'0');
		}

		
		public static string duration(int seconds){
			if (seconds < 61) return seconds.ToString() + " sec";
			
			int minute = seconds / 60;
			seconds -= minute * 60;
			
			if (minute < 60) {
				if (seconds == 0)
					return minute.ToString() + " min";	
				else
					return minute.ToString() + " min " + seconds.ToString() + " sec";	
			}
			
			int hr = minute / 60;
			minute -= hr * 60;
			
			if (hr < 25) {
				if (minute == 0)
					return hr.ToString() + " hr";	
				else
					return hr.ToString() + " hr " + minute.ToString() + " min";	
			}
			
			int day = hr / 24;
			hr -= day * 24;
			
			if (hr == 0)
				return day.ToString() + " day";	
			else
				return day.ToString() + " day " + hr.ToString() + " hr";	
		}

		public static string timeSince(System.DateTime lastSeen){
			System.TimeSpan duration = EleCellLogin.GameServer.ServerTime - lastSeen;
			if (duration.TotalDays >= 1)
				return ((int) duration.TotalDays).ToString() + text ("daysAgo");
			if (duration.TotalHours >= 1)
				return ((int) duration.TotalHours).ToString() + text ("hoursAgo");
			if (duration.TotalMinutes >= 1)
				return ((int) duration.TotalMinutes).ToString() + text ("minutesAgo");
			return text ("lessMinutes");
		}
	}

}

public static class StringFunction {

	public static string MjCoinString(this GameSecurity.safeDouble number){
		double num = (double) number;

		if (num <= 10000)
			return (num).ToString("N0");
		if (num < 1000000)
			return (num/1000).ToString("N2") + 'K';
		if (num < 1000000000)
			return (num/1000).ToString("N2") + 'M';
		if (num < 1000000000000)
			return (num/1000).ToString("N2") + 'B';
		if (num < 1000000000000000) return (num/1000000000000).ToString("N2") + 'T';
		if (num < 1000000000000000000) return (num/1000000000000000).ToString("N2") + "aa";
		if (num < 1E+21) return (num/1000000000000000000).ToString("N2") + "bb";
		if (num < 1E+24) return (num/1E+21).ToString("N2") + "cc";
		if (num < 1E+27) return (num/1E+24).ToString("N2") + "dd";
		if (num < 1E+30) return (num/1E+27).ToString("N2") + "ee";
		if (num < 1E+33) return (num/1E+30).ToString("N2") + "ff";
		if (num < 1E+36) return (num/1E+33).ToString("N2") + "gg";
		if (num < 1E+39) return (num/1E+36).ToString("N2") + "hh";
		if (num < 1E+42) return (num/1E+39).ToString("N2") + "ii";
		if (num < 1E+45) return (num/1E+42).ToString("N2") + "jj";
		if (num < 1E+48) return (num/1E+45).ToString("N2") + "kk";
		if (num < 1E+51) return (num/1E+48).ToString("N2") + "ll";
		if (num < 1E+54) return (num/1E+51).ToString("N2") + "mm";
		if (num < 1E+57) return (num/1E+54).ToString("N2") + "nn";
		if (num < 1E+60) return (num/1E+57).ToString("N2") + "oo";
		if (num < 1E+63) return (num/1E+60).ToString("N2") + "pp";
		if (num < 1E+66) return (num/1E+63).ToString("N2") + "qq";
		if (num < 1E+69) return (num/1E+66).ToString("N2") + "rr";
		if (num < 1E+72) return (num/1E+69).ToString("N2") + "ss";
		if (num < 1E+75) return (num/1E+72).ToString("N2") + "tt";
		if (num < 1E+78) return (num/1E+75).ToString("N2") + "uu";
		if (num < 1E+81) return (num/1E+78).ToString("N2") + "vv";
		if (num < 1E+84) return (num/1E+81).ToString("N2") + "ww";
		if (num < 1E+87) return (num/1E+84).ToString("N2") + "xx";
		if (num < 1E+90) return (num/1E+87).ToString("N2") + "yy";
		if (num < 1E+93) return (num/1E+90).ToString("N2") + "zz";

		return num.ToString ("G3");
    }

	public static string MjCoinString(this double num){
		if (num <= 10000)
			return (num).ToString("N0");
		if (num < 1000000)
			return (num/1000).ToString("N2") + 'K';
		if (num < 1000000000)
			return (num/1000000).ToString("N2") + 'M';
		if (num < 1000000000000)
			return (num/1000000000).ToString("N2") + 'B';
		if (num < 1000000000000000) return (num/1000000000000).ToString("N2") + 'T';
		if (num < 1000000000000000000) return (num/1000000000000000).ToString("N2") + "aa";
		if (num < 1E+21) return (num/1000000000000000000).ToString("N2") + "bb";
		if (num < 1E+24) return (num/1E+21).ToString("N2") + "cc";
		if (num < 1E+27) return (num/1E+24).ToString("N2") + "dd";
		if (num < 1E+30) return (num/1E+27).ToString("N2") + "ee";
		if (num < 1E+33) return (num/1E+30).ToString("N2") + "ff";
		if (num < 1E+36) return (num/1E+33).ToString("N2") + "gg";
		if (num < 1E+39) return (num/1E+36).ToString("N2") + "hh";
		if (num < 1E+42) return (num/1E+39).ToString("N2") + "ii";
		if (num < 1E+45) return (num/1E+42).ToString("N2") + "jj";
		if (num < 1E+48) return (num/1E+45).ToString("N2") + "kk";
		if (num < 1E+51) return (num/1E+48).ToString("N2") + "ll";
		if (num < 1E+54) return (num/1E+51).ToString("N2") + "mm";
		if (num < 1E+57) return (num/1E+54).ToString("N2") + "nn";
		if (num < 1E+60) return (num/1E+57).ToString("N2") + "oo";
		if (num < 1E+63) return (num/1E+60).ToString("N2") + "pp";
		if (num < 1E+66) return (num/1E+63).ToString("N2") + "qq";
		if (num < 1E+69) return (num/1E+66).ToString("N2") + "rr";
		if (num < 1E+72) return (num/1E+69).ToString("N2") + "ss";
		if (num < 1E+75) return (num/1E+72).ToString("N2") + "tt";
		if (num < 1E+78) return (num/1E+75).ToString("N2") + "uu";
		if (num < 1E+81) return (num/1E+78).ToString("N2") + "vv";
		if (num < 1E+84) return (num/1E+81).ToString("N2") + "ww";
		if (num < 1E+87) return (num/1E+84).ToString("N2") + "xx";
		if (num < 1E+90) return (num/1E+87).ToString("N2") + "yy";
		if (num < 1E+93) return (num/1E+90).ToString("N2") + "zz";
		
		return num.ToString ("G3");
    }
	/* 
	public static string MjCoinString(this long num){
		if (num < 10000)
			return (num*100).ToString();

        char man = (SettingData.instance.language == "en") ? 'K' : ((SettingData.instance.language == "jp") ? '万' : '萬');
        char oku = (SettingData.instance.language == "en") ? 'M' : ((SettingData.instance.language == "jp") ? '億' : '億');

        if (SettingData.instance.language != "en")
        {

            if (num < 1000000)
                return (num / 100).ToString() + man;

            long i = num / 1000000;
            long j = (num % 1000000) / 100;

            return (j > 0 ? (i.ToString() + oku + (j.ToString() + man)) : (i.ToString() + oku));
        }
        else
        {
            if (num < 10000)
                return (num / 10).ToString() + man;

            long i = num / 10000;
            long j = (num % 10000) / 10;

            return (j > 0 ? (i.ToString() + oku + (j.ToString() + man)) : (i.ToString() + oku));
        }
    }
	*/
	public static string MjStringChange(this double num){
		/*
		if (MainHall.godMode == 2) {
			return num.ChangeString();
		}
		*/

		if (num == 0)
			return "±0";
		
		if (num > 0)
			return ("+" + num.MjCoinString ());
		
		return "-" + (-num).MjCoinString ();
	}
	/*
	public static string MjStringChange(this long num){
		if (MainHall.godMode == 2) {
			return num.ChangeString();
		}

		if (num == 0)
			return "±0";

		if (num > 0)
			return ("+" + num.MjCoinString ());

		return "-" + (-num).MjCoinString ();
	}
	*/
	public static string ChangeString(this int num){
		if (num == 0)
			return "±0";
		
		if (num > 0)
			return ("+" + num.ToString ());
		
		return num.ToString ();
	}

	public static string ChangeString(this double num){
		if (num == 0)
			return "±0";

		if (num > 0)
			return ("+" + num.ToString ());

		return num.ToString ();
	}

	static System.Collections.Generic.Dictionary<char,char> fullWidthDict = new System.Collections.Generic.Dictionary<char, char>(){
		{'0','０'},{'1','１'},{'2','２'},{'3','３'},{'4','４'},{'5','５'},{'6','６'},{'7','７'},{'8','８'},{'9','９'}
	};

	static System.Collections.Generic.Dictionary<int,char> chiNumDict = new System.Collections.Generic.Dictionary<int, char>(){
		{1,'一'},{2,'二'},{3,'三'},{4,'四'},{5,'五'},{6,'六'},{7,'七'},{8,'八'},{9,'九'},{10,'十'},{100,'百'},{1000,'千'},{10000,'萬'}
	};

	public static string toChineseNum (this int num){
		if (num == 0) return "零";

		string s = "";
		if (num >= 10000) {
			s += (num/10000).toChineseNum();
			s += chiNumDict[10000];
			num %= 10000;
		}
		if (num >= 1000) {
			s += chiNumDict[num/1000];
			s += chiNumDict[1000];
			num %= 1000;
		}
		if (num >= 100) {
			s += chiNumDict[num/100];
			s += chiNumDict[100];
			num %= 100;
		}
		if (num >= 10) {
			if (num >= 20 || s != "")
				s += chiNumDict[num/10];
			s += chiNumDict[10];
			num %= 10;
		}

		if (num > 0)
			s += chiNumDict[num];

		return s;
	}

	public static string toFullWidthString (this int num){
		string s = num.ToString ();
		char[] c = new char[s.Length];
		
		for (int i = 0; i < s.Length; i++) {
			if (fullWidthDict.ContainsKey(s[i]))
				c[i] = fullWidthDict[s[i]];
			else
				c[i] = s[i];
		}
		
		return new string (c);
	}
}

public struct Dialog{
	public string content;
	public GenericCallback<bool> callback;
	
	public Dialog(string _content){
		content = "[000000]" + _content;
		callback = null;
	}
	
	public Dialog(string _content, GenericCallback<bool> _callback){
		content = "[000000]" + _content;
		callback = _callback;
	}
}