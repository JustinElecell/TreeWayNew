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
    public Button[] CharaterDrawButton;
    public Button[] IteamDrawButton;

    public Transform drawEndTran;
    
    public Image DrawEndPerfab;


    [Header("卡池")]
    public GameObject CardPoolPerfab;
    public Transform CreatPoolTran;

    public PageView pageView;

    private void Awake()
    {
        ButtonInit(); 
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
    private void OnEnable()
    {
        CreatPoolTran.transform.DetachChildren();


    }

    void ButtonInit()
    {
        //單抽
        IteamDrawButton[0].onClick.AddListener(() => {

            GameServer.instance.DrawTest(1,new EleCellJsonCallback(delegate (string err, JSONClass message) {
                Debug.Log(message);
                //drawEndTran.DetachChildren();

                //Debug.Log(message[0][0]["type"]);
                //var tmp = Instantiate(DrawEndPerfab, drawEndTran);
                //tmp.sprite = FindItem(message[0][0]["type"]).IteamImage;

                //DrawEndPanel.SetActive(true);



            }));

        });
        //10抽
        IteamDrawButton[1].onClick.AddListener(() => {

            GameServer.instance.DrawTest(10, new EleCellJsonCallback(delegate (string err, JSONClass message) {
                Debug.Log(message);
                //drawEndTran.DetachChildren();

                //for (int i=0;i< message[0].Count;i++)
                //{
                //    var tmp = Instantiate(DrawEndPerfab, drawEndTran);
                //    tmp.sprite = FindItem(message[0][i]["type"]).IteamImage;

                //}
                //DrawEndPanel.SetActive(true);


            }));

        });
    }


    SO_Iteam FindItem(string itemNo)
    {
        for (int i = 0; i < IteamLists.Count; i++)
        {
            if(IteamLists[i][0]== itemNo)
            {
                return Resources.Load<SO_Iteam>("Iteam/PlayerItem/" + IteamLists[i][1]);

            }

        }
        Debug.Log("沒找到對應編號道具");
        return null;
    }
}
