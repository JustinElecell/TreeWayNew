using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using EleCellLogin;
using SimpleJSON;
public class MainManager : MonoBehaviour
{
    public static MainManager instance;

    public enum ServerData
    {
        CardPool,
        Charater,
        Item
    }

    public Dictionary<ServerData, JSONClass> ServerData_Json = new Dictionary<ServerData, JSONClass>();
    public List<List<string>> CardPoolData = new List<List<string>>();

    public bool TestFlag;
    public List<string> TargetCharater;
    public int targetCharaterLevel;
    public int targetRank;

    public int targetSkillNo = 1;
    public SO_Iteam[] skillIteams;

    public Text ID;

    [Header("Panel")]
    public ShopPanel shopPanel;
    public IteamPanel itemPanel;
    public CharaterPanel charaterPanel;
    public NoticePanel noticePanel;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);

            GameServer.instance.LoadItem_Server("100000001", new EleCellJsonCallback(delegate (string err, JSONClass message)
            {
                MainManager.instance.ServerData_Json.Add(MainManager.ServerData.Item, message);

            }));

            GameServer.instance.LoadCharater_Server("100000001", new EleCellJsonCallback(delegate (string err, JSONClass message)
            {
                MainManager.instance.ServerData_Json.Add(MainManager.ServerData.Charater, message);
            }));

        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        Init();

    }
    void Init()
    {
        CardPoolData = ReadCsv.MyReadCSV.Read("Csv/CardPool");
        if (MjSave.instance.playerID != "")
        {
            ID.text = "ID : " + MjSave.instance.playerID.ToString();
        }
        else
        {
            ID.text = "ID : Null";
        }


        shopPanel.Init();
        charaterPanel.Init();


        for (int i = 0; i < 5; i++)
        {
            if (SaveManager.instance.saveData.skillItemList[i] != "")
            {
                var tmp = Resources.Load<SO_Iteam>("Iteam/PlayerItem/" + SaveManager.instance.saveData.skillItemList[i]);
                MainManager.instance.skillIteams[i] = tmp;

            }

        }
    }




}
