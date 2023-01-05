using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EleCellLogin;
using SimpleJSON;

public class ShopPanel : MonoBehaviour
{
    List<List<string>> IteamLists;
    [Header("抽抽")]
    public GameObject DrawEndPanel;

    public Transform drawEndTran;
    
    public Image DrawEndPerfab;
    
    public DrawChancePanel DrawChancePanel;


    [Header("卡池")]
    public GameObject CardPoolPerfab;
    public Transform CreatPoolTran;

    public PageView pageView;


    private void Awake()
    {
        IteamLists = ReadCsv.MyReadCSV.Read("Csv/Iteam");

        GameServer.instance.LoadCardPool_Server(new EleCellJsonCallback(delegate (string err, JSONClass message) {

            for (int i = 0; i < message[0].Count; i++)
            {
                var tmp = Instantiate(CardPoolPerfab.gameObject, CreatPoolTran);

                tmp.GetComponent<DrawPoolPanel>().Init(message[0][i]);
            }
            pageView.Init();
            MainManager.instance.ServerData_Json.Add(MainManager.ServerData.CardPool, message);
            Debug.Log("儲存資料" + MainManager.instance.ServerData_Json[MainManager.ServerData.CardPool][0]);

        }));

        GameServer.instance.LoadItem_Server("100000001", new EleCellJsonCallback(delegate (string err, JSONClass message)
        {
            MainManager.instance.ServerData_Json.Add(MainManager.ServerData.Item, message);
            Debug.Log("儲存資料"+MainManager.instance.ServerData_Json[MainManager.ServerData.Item][0]);

        }));

        GameServer.instance.LoadCharater_Server("100000001", new EleCellJsonCallback(delegate (string err, JSONClass message) {
            MainManager.instance.ServerData_Json.Add(MainManager.ServerData.Charater, message);
            Debug.Log(MainManager.instance.ServerData_Json[MainManager.ServerData.Charater][0][3]);
        }));
    }




    public SO_Iteam FindItem(string itemNo)
    {
        var tmp = IteamLists.Find(x => x[0] == itemNo);
        if (tmp[0] != 0.ToString())
        {
            return Resources.Load<SO_Iteam>("Iteam/PlayerItem/" + tmp[1]);
        }

        Debug.Log("沒找到對應編號道具");
        return null;
    }
}
