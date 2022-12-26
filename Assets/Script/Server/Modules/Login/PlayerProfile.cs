using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GameSecurity;
using SimpleJSON;
using ElecellConnection;

namespace EleCellLogin {

    public delegate void EleCellEasyCallback(string err, string message, InGameCurrency field = InGameCurrency.Null, int val = 0);
    public delegate void EleCellLootCallback(string err, string message, Reward reward);
	public delegate void EleCellProfileCallback(string err, string message);
	public delegate void EleCellJsonCallback(string err, JSONClass message);
	public delegate void EleCellFbInviteCallback(string err, string[] friends);
	public delegate void EleCellPictureCallback(string err, Texture2D pic);

    /// <summary>
    /// Google Game Play資料
    /// </summary>
    public struct PlayerGooglePlayGameInfo
    {
        public string id;
        public string name;
        public Texture2D avatarTexture;
    }

    /// <summary>
    /// Facebook資料
    /// </summary>
    public struct PlayerFacebookInfo
    {
        public string id;
        public string name;
        public Texture2D avatarTexture;
    }

    public struct PlayerInfo{
		public int id;
		public string pid;
		public string nick;
		public string pass;
		public string googleID;
		public string fbID;
		public int checkinRow;
		public int checkinTotal;
		public double checkinRewardO;
		public string checkinReward;
		public bool canCheckin;
		public int nextCheckin;
		public int friendCount;

		public int guildID;
		public string guildName;
		public int guildRole;
		public int guildLevel;
		public int guildWeekly;
		public System.DateTime guildTime;
	}
	
	public struct Ranking{
		public int rank;
		public string nick;
		public double score;
		public bool isMe;
	}
	
	public struct Mail{
		public int mid;
		public string sender;
		public int senderID;
		public string subject;
		public string message;
		public string url;
		public Reward reward;		
		public bool isRead;
		public bool isCollected;
		public string date;
	}
	
	public struct Reward{
		public int rewardType;              // 0 = 貨幣(含以下分類), 1 = 道具
		public int rewardEnum;              // 0 = 金幣, 1 = 賭神幣, 2 = 寶石, 3...
		public double amount;               // 獎勵數量
		public string item;                 // 獎勵道具
		
		public Reward(int a){
			rewardType = -1;
			rewardEnum = 0;
			amount = 0;
			item = "";
		}
		
		public Reward(int type, int enm, double count){
			rewardType = type;
			rewardEnum = enm;
			amount = count;
			item = "";
		}
		
		public Reward(int type, string _item, double count){
			rewardType = type;
			rewardEnum = 0;
			amount = count;
			item = _item;
		}
	}
	
	public enum MailAction{
		SetRead,
		Delete,
		CollectAttachment
	}
	
	public struct QnA{
		public string message;
		public bool isQuery;
	}
	
	public struct ExtraLevel{
		public string levelID;
		public string levelData;
		public float remainingSeconds;
		public int taken;
	}
	
	public struct Battle{
		public int id;
		public int friend;
		public int map;
		public string config;
		public string reward;
		public int xp;
	}
	
	public struct Hero{
		public int id;
		public string type;
		public int rank;
		public int status;
		
		public int[] stats;
		public int[] skill;
		
		public int level;
		public int xp;		
	}
	
	public struct Deck{
		public int id;
		public int hero;
		public int[] pets;
	}
	
	public struct Pet{
		public int id;
		public string type;
		public int rank;
		public int gender;
		public int status;
		public string name;
		public int level;
		public int xp;
		
		public System.DateTime cooldown1;
		public System.DateTime cooldown2;
	}
	
	public struct Friend{
		public int id;
		public string fbid;
		public string nick;
		public int level;
		public int friendship; // 0 = invited, 1 = fb friend, 2 = in-game friend
		public System.DateTime cooldown;
		public System.DateTime lastSeen;
		
		public Hero hero;
		public Pet[] pets;
	}

	public struct ExchangeRate{
		public string from;             // 商品需求幣
		public string to;               // 商品代號
		public double pay;              // 商品價格
		public double receive;          // 單次購買數量

		public string remarks;
	}
	
	public struct Mission{
		public int missionID;
		public string mName;
		public string objective;
		public int count;
		public int complete;
		public int xp;
		public double coin;
	}
	
	public struct PetQuery{
		public PetAction action;
		public int petID;
		public string type;
		public int status;
		
		public PetQuery(PetAction _action){
			action = _action;
			petID = 0;
			type = "";
			status = 0;
		}
		public PetQuery(PetAction _action, int _petID, string _type){
			action = _action;
			petID = _petID;
			type = _type;
			status = 0;
		}
		public PetQuery(PetAction _action, int _petID, int _status){
			action = _action;
			petID = _petID;
			type = "";
			status = _status;
		}
	}
	
	public struct Build{
		public int id;
		public string type;
		public int rank;
		//		public int pos; 
		public int status;
		public int task;
		public int upgrade;	
		
		public int rID;
		public int multiple;
		public System.DateTime taskFinish;
		public int brID;
		public System.DateTime upgradeFinish;

		public System.DateTime cooldown;
	}

	public struct Guild{
		public const int Applicant = 0;
		public const int Member = 1;
		public const int Officer = 2;
		public const int GuildMaster = 5;
		public const int Left = 99;

		public int guildID;
		public string guildName;
		public string motto;
		public int level;
		public int xp;
		public int status;	
		public int weekly;
		public int lastWeek;

		public int memberCount;
		public string guildMaster;

		public GuildMember[] members;
	}

	public struct GuildMember {
		public int id;
		public string nick;
		public int level;
		public int role;
		public int score;
		public System.DateTime loginTime;
	}
	
    /// <summary>
    /// 成就之類別
    /// </summary>
	public struct Trophy : System.IComparable<Trophy>{
		public string TrophyName;           // 成就名稱
		public int category;                // 0 = pStats, 1 = achieve
		public string field;                // 成就任務內容
		public int count;                   // 成就的完成次數
		public Reward reward;               // 成就獎勵
		public bool collected;              // 成就獎勵是否領取
		
        /// <summary>
        /// 成就是否已完成
        /// </summary>
		public bool achieved {
			get {
				if (collected) return true;
				return ((category == 0)? GameServer.playerStats[field].AsInt : GameServer.Achieve[field].AsInt) >= count;
			}
		}

		public string progress {
			get {
				if (category == 0) {
					return ((string)GameServer.playerStats [field] + '/' + count.ToString ());
				} else {
					if (GameServer.Achieve[field] == null)
						return ("0/" + count.ToString ());
					return ((string)GameServer.Achieve[field] + '/' + count.ToString ());
				}
			}
		}

		public int CompareTo(Trophy other){
			if (collected) {
				if (other.collected)
					return 0;
				return 1;
			} else {
				if (other.collected)
					return -1;

				int a = 0;
				int b = 0;

				if (category == 0)
					a = -1000 * GameServer.playerStats [field].AsInt / count;
				else if (GameServer.Achieve [field] != null)
					a = -1000 * GameServer.Achieve [field].AsInt / count;
				else
					a = count;

				if (other.category == 0)
					b = -1000 * GameServer.playerStats [other.field].AsInt / other.count;
				else if (GameServer.Achieve [other.field] != null)
					b = -1000 * GameServer.Achieve [other.field].AsInt / other.count;
				else
					b = other.count;

				return a - b;
			}
		}
	}

    /// <summary>
    /// 每日任務之類別
    /// </summary>
	public struct DailyMission : System.IComparable<DailyMission>{
		public string TrophyName;               // 任務名稱
		public int category;                    // 0 = pStats, 1 = achieve
		public string field;                    // 任務實體
		public int count;                       // 任務次數
		public Reward reward;                   // 任務獎勵
		public bool collected;                  // 任務是否領取

        /// <summary>
        /// 確認任務是否完全完成
        /// </summary>
		public bool achieved {
			get {
				if (collected) return true;
				return ((category == 0)? GameServer.playerStats[field].AsInt : GameServer.DailyDone[field].AsInt) >= count;
			}
		}

        /// <summary>
        /// 任務進度
        /// </summary>
		public string progress {
			get {
				if (category == 0) {
					return ((string)GameServer.playerStats [field] + '/' + count.ToString ());
				} else {
					if (GameServer.DailyDone[field] == null)
						return ("0/" + count.ToString ());
					return ((string)GameServer.DailyDone[field] + '/' + count.ToString ());
				}
			}
		}

        /// <summary>
        /// 與其他任務做Sort用
        /// </summary>
        /// <param name="other">其他任務</param>
        /// <returns></returns>
		public int CompareTo(DailyMission other){
			if (collected) {
				if (other.collected)
					return 0;
				return 1;
			} else {
				if (other.collected)
					return -1;

				int a = 0;
				int b = 0;

				if (category == 0)
					a = -1000 * GameServer.playerStats [field].AsInt / count;
				else if (GameServer.DailyDone [field] != null)
					a = -1000 * GameServer.DailyDone [field].AsInt / count;
				else
					a = count;

				if (other.category == 0)
					b = -1000 * GameServer.playerStats [other.field].AsInt / other.count;
				else if (GameServer.DailyDone [other.field] != null)
					b = -1000 * GameServer.DailyDone [other.field].AsInt / other.count;
				else
					b = other.count;

				return a - b;
			}
		}
	}
	
	public struct BuildQuery{
		public BuildAction action;
		public int bID;
		public string type;
		public int status;
		
		public BuildQuery(BuildAction _action){
			action = _action;
			bID = 0;
			type = "";
			status = 0;
		}
		public BuildQuery(BuildAction _action, int _bID, string _type, int _status = 0){
			action = _action;
			bID = _bID;
			type = _type;
			status = _status;
		}
		public BuildQuery(BuildAction _action, int _bID, int _status = 0){
			action = _action;
			bID = _bID;
			type = "";
			status = _status;
		}
	}
	
	public struct StatsQuery{
		public StatsAction action;
		public string field;
		// public int value;
		public double value;
		
		public StatsQuery(StatsAction _action, string _field, double _value){
			action = _action;
			field = _field;
			value = _value;
		}
	}
	
	public struct ItemQuery{
		public ItemAction action;
		public string field;
		public int value;
		
		public ItemQuery(ItemAction _action, string _item = null, int _qty = 1){
			action = _action;
			field = _item;
			value = _qty;
		}
	}
	
	public struct AlchemyQuery{
		public AlchemyAction action;
		public string field;
		public int value;
		
		public AlchemyQuery(AlchemyAction _action, string _item = null, int _qty = 1){
			action = _action;
			field = _item;
			value = _qty;
		}
	}
	
	public enum PetAction{
		list,
		updateType,
		updateStatus,
		harvest
	}
	
	public enum BuildAction{
		list,
		listReq,
		upgrade,
		destroy,
		cancel,
		complete,
		task,
		cxltask,
		comtask,
		collect
	}

	public enum GuildAction {
		list,
		search,
		create,
		memberList,
		disband,
		join,
		cxlJoin,
		leave,
		transfer,
		approve,
		reject,
		promote,
		demote,
		kick,
		motto
	}

	public enum FriendAction{
		suggest,
		search,
		add,
		cxlAdd,
		confirm,
		delete
	}
	
	public enum AlchemyAction{
		list
	}
	
	public enum ItemAction{
		list,
		listPrice,
		sell,
		buy,
		use
	}
	
	public enum StatsAction{
		spend,
		set,
		earn
	}
	
	public enum TrophyAction{
		listDef,
		listAchieve,
		collect
	}
}

public delegate void GenericCallback<T>(T value);
public delegate void GenericTrigger();