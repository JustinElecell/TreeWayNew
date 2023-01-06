using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.UI;
using System;
public class DrawChancePanel : MonoBehaviour
{
    public Transform createTran;
    public Text TextPerfab;

    public List<int> PickUpList = new List<int>();
    public List<int> NoPickUpList = new List<int>();

    public List<int> NothingList = new List<int>();
    public Dictionary<String, List<int>> KeyNothingList = new Dictionary<string, List<int>>();
    public List<List<string>> CardPoolData = new List<List<string>>();

    int Up, Down = 0;


    public void Init(JSONNode data)
    {
        CardPoolData = MainManager.instance.CardPoolData;

        if (data["Type"].Value == "Charater")
        {
            DrawCharater(data);

        }
        else
        {
            DrawItem(data["Type"].Value, data);
        }

        gameObject.SetActive(true);
    }
    


    void DrawCharater(JSONNode data)
    {
        for (int i = 1; i <= 3; i++)
        {
            if (data["PickupNo" + i.ToString()].AsInt != 0)
            {
                PickUpList.Add(data["PickupNo" + i.ToString()].AsInt);
            }
        }
        if (PickUpList.Count != 0)
        {
            var pickUpText = Instantiate(TextPerfab, createTran);
            Instantiate(TextPerfab, createTran);

            var tmpPickUpChance = (data["Round1"].AsFloat / 100f) * ((PickUpList.Count * 10f + 10f) / 100f) * (1f / PickUpList.Count);

            pickUpText.text = "◆◆◆ Pick up ◆◆◆";

            for (int i = 0; i < PickUpList.Count; i++)
            {
                var tmp = Instantiate(TextPerfab, createTran);

                tmp.text = FindIteamName(PickUpList[i]) + "---------" + (tmpPickUpChance * 100f).ToString("0.00") + "%";
            }
        }

        // 裝備的部分
        for (int i = 0; i < MainManager.instance.ServerData_Json[MainManager.ServerData.Charater][0].Count; i++)
        {

            var tmp = PickUpList.Find(x => x == MainManager.instance.ServerData_Json[MainManager.ServerData.Charater][0][i]["type"].AsInt);

            if (tmp == 0)
            {
                NoPickUpList.Add(MainManager.instance.ServerData_Json[MainManager.ServerData.Charater][0][i]["type"].AsInt);
            }

        }

        if (NoPickUpList.Count != 0)
        {
            Instantiate(TextPerfab, createTran);
            var tmpText = Instantiate(TextPerfab, createTran);
            tmpText.text = "◆◆◆◆◆◆◆◆◆";
            NothingList.Sort();
            var tmpNoPickUpChance = (data["Round1"].AsFloat / 100f) * (1 - ((PickUpList.Count * 10f + 10f) / 100f)) * (1f / (MainManager.instance.ServerData_Json[MainManager.ServerData.Charater][0].Count - PickUpList.Count));

            for (int i = 0; i < NoPickUpList.Count; i++)
            {
                var tmp = Instantiate(TextPerfab, createTran);
                tmp.text = FindIteamName(NoPickUpList[i]) + "---------" + (tmpNoPickUpChance * 100).ToString("0.00") + "%";
            }
        }

        for (int i = 0; i < MainManager.instance.ServerData_Json[MainManager.ServerData.Item][0].Count; i++)
        {
            var tmp = PickUpList.Find(x => x == MainManager.instance.ServerData_Json[MainManager.ServerData.Item][0][i]["type"].AsInt);

            if (tmp == 0)
            {
                NothingList.Add(MainManager.instance.ServerData_Json[MainManager.ServerData.Item][0][i]["type"].AsInt);
            }
        }

        if (NothingList.Count != 0)
        {
            Instantiate(TextPerfab, createTran);
            var tmpText = Instantiate(TextPerfab, createTran);
            tmpText.text = "◆◆◆◆◆◆◆◆◆";
            NothingList.Sort();
            var tmpNothigChance = (1f - (data["Round1"].AsFloat / 100f)) * (1f / MainManager.instance.ServerData_Json[MainManager.ServerData.Item][0].Count);

            for (int i = 0; i < NothingList.Count; i++)
            {
                var tmp = Instantiate(TextPerfab, createTran);
                tmp.text = FindIteamName(NothingList[i]) + "---------" + (tmpNothigChance * 100).ToString("0.00") + "%";
            }
        }
    }

    void DrawItem(string type,JSONNode data)
    {
        for (int i = 1; i <= 3; i++)
        {
            if (data["PickupNo" + i.ToString()].AsInt != 0)
            {
                PickUpList.Add(data["PickupNo" + i.ToString()].AsInt);
            }
        }
        if (PickUpList.Count != 0)
        {
            var pickUpText = Instantiate(TextPerfab, createTran);
            Instantiate(TextPerfab, createTran);

            var tmpPickUpChance = (data["Round2"].AsFloat / 100f) * ((PickUpList.Count * 10f + 10f) / 100f) * (1f / PickUpList.Count);

            pickUpText.text = "◆◆◆ Pick up ◆◆◆";

            for (int i = 0; i < PickUpList.Count; i++)
            {
                var tmp = Instantiate(TextPerfab, createTran);

                tmp.text = FindIteamName(PickUpList[i]) + "---------" + (tmpPickUpChance * 100f).ToString("0.00") + "%";
            }
        }

        switch(type)
        {
            case "Invoked":
                Up = 5000;
                Down = 6000;
                break;
            case "Magic":
                Up = 3000;
                Down = 4000;
                break;
            case "Weapon":
                Up = 1000;
                Down = 2000;
                break;
        }

        // 裝備的部分
        for (int i = 0; i < MainManager.instance.ServerData_Json[MainManager.ServerData.Item][0].Count; i++)
        {
            if(MainManager.instance.ServerData_Json[MainManager.ServerData.Item][0][i]["type"].AsInt > Up && 
                MainManager.instance.ServerData_Json[MainManager.ServerData.Item][0][i]["type"].AsInt < Down)
            {
                var tmp = PickUpList.Find(x => x == MainManager.instance.ServerData_Json[MainManager.ServerData.Item][0][i]["type"].AsInt);
                if (tmp == 0)
                {
                    NoPickUpList.Add(MainManager.instance.ServerData_Json[MainManager.ServerData.Item][0][i]["type"].AsInt);
                }
            }
        }

        if (NoPickUpList.Count != 0)
        {
            Instantiate(TextPerfab, createTran);
            var tmpText = Instantiate(TextPerfab, createTran);
            tmpText.text = "◆◆◆◆◆◆◆◆◆";
            NothingList.Sort();
            var tmpNoPickUpChance = (data["Round2"].AsFloat / 100f) * (1 - ((PickUpList.Count * 10f + 10f) / 100f)) * (1f / (NoPickUpList.Count));

            for (int i = 0; i < NoPickUpList.Count; i++)
            {
                var tmp = Instantiate(TextPerfab, createTran);
                tmp.text = FindIteamName(NoPickUpList[i]) + "---------" + (tmpNoPickUpChance * 100).ToString("0.00") + "%";
            }
        }

        KeyNothingList.Add("召喚",new List<int>());
        KeyNothingList.Add("魔法", new List<int>());
        KeyNothingList.Add("武具", new List<int>());


        for (int i = 0; i < MainManager.instance.ServerData_Json[MainManager.ServerData.Item][0].Count; i++)
        {
            var tmp = PickUpList.Find(x => x == MainManager.instance.ServerData_Json[MainManager.ServerData.Item][0][i]["type"].AsInt);
            var tmp2 = NoPickUpList.Find(x => x == MainManager.instance.ServerData_Json[MainManager.ServerData.Item][0][i]["type"].AsInt);
            if (tmp == 0&&tmp2==0)
            {
                // 召喚
                if (MainManager.instance.ServerData_Json[MainManager.ServerData.Item][0][i]["type"].AsInt > 5000 &&
                    MainManager.instance.ServerData_Json[MainManager.ServerData.Item][0][i]["type"].AsInt < 6000)
                {
                    KeyNothingList["召喚"].Add(MainManager.instance.ServerData_Json[MainManager.ServerData.Item][0][i]["type"].AsInt);
                    //typeCount["召喚"]++;

                }
                //魔法
                else if (MainManager.instance.ServerData_Json[MainManager.ServerData.Item][0][i]["type"].AsInt > 3000 &&
                    MainManager.instance.ServerData_Json[MainManager.ServerData.Item][0][i]["type"].AsInt < 4000)
                {
                    //typeCount["魔法"]++;
                    KeyNothingList["魔法"].Add(MainManager.instance.ServerData_Json[MainManager.ServerData.Item][0][i]["type"].AsInt);

                }
                //武具
                else if (MainManager.instance.ServerData_Json[MainManager.ServerData.Item][0][i]["type"].AsInt > 1000 &&
                    MainManager.instance.ServerData_Json[MainManager.ServerData.Item][0][i]["type"].AsInt < 2000)
                {
                    //typeCount["武具"]++;
                    KeyNothingList["武具"].Add(MainManager.instance.ServerData_Json[MainManager.ServerData.Item][0][i]["type"].AsInt);

                }

                //Debug.Log(MainManager.instance.ServerData_Json[MainManager.ServerData.Charater][0][i]["type"].AsInt);
                //NothingList.Add(MainManager.instance.ServerData_Json[MainManager.ServerData.Item][0][i]["type"].AsInt);
            }

        }

        CreatItem("召喚", data["Round2"].AsFloat);
        CreatItem("魔法", data["Round2"].AsFloat);
        CreatItem("武具", data["Round2"].AsFloat);

    }

    void CreatItem(string targetItem,float roundNo)
    {
        if (KeyNothingList[targetItem].Count != 0)
        {
            Instantiate(TextPerfab, createTran);
            var tmpText = Instantiate(TextPerfab, createTran);
            tmpText.text = "◆◆◆◆◆◆◆◆◆";
            var tmpNothigChance = (1f - (roundNo / 100f)) * 0.5 * (1f / KeyNothingList[targetItem].Count);


            for (int i = 0; i < KeyNothingList[targetItem].Count; i++)
            {
                var tmp = Instantiate(TextPerfab, createTran);
                tmp.text = FindIteamName(KeyNothingList[targetItem][i]) + "---------" + (tmpNothigChance * 100).ToString("0.00") + "%";
            }
        }
    }

    string FindIteamName(int ItemNo)
    {
        Debug.Log(CardPoolData.Count);
        var tmp = CardPoolData.Find(x => int.Parse(x[0]) == ItemNo);
        if(tmp!=null)
        {
            return tmp[1];
        }
        else
        {
            Debug.Log("沒找到道具");
            return "沒找到道具";
        }
    }

    private void OnDisable()
    {
        for (int i = 0; i < createTran.childCount; i++)
        {
            Destroy(createTran.GetChild(i).gameObject);
        }
        //createTran.DetachChildren();
        PickUpList.Clear();
        NoPickUpList.Clear();
        NothingList.Clear();
        KeyNothingList.Clear();
        createTran.gameObject.GetComponent<RectTransform>().anchoredPosition =new Vector2(0,0);
    }

}
