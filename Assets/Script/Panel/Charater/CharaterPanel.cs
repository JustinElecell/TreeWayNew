using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadCsv;
using UnityEngine.UI;

using EleCellLogin;
using SimpleJSON;
using ElecellConnection;


public class ItemStetas
{
    public string Name;
    public int Level;
    public int Rank;
    public int HP;
    public int MP;
    public int ATK;
    public float REC;
}
public class CharaterPanel : BasePanel
{
    public Transform createrTran;
    public Button charaterButtonPerfab;
    public Button LevelUpButton;

    List<List<string>> charaterLists;
    Dictionary<int, ItemStetas> itemStetasList = new Dictionary<int, ItemStetas>();

    int charaterNo=1;
    [Header("內容設定")]
    public Text Name;
    public Text Level;
    public Text HP;
    public Text MP;
    public Text ATK;
    public Text REC;
    public Image image;

    public Text[] Lv;

    public int targetNo;
    public int targetLevel;
    public Button TargetCharaterButton;
    public CharaterRankUpCount RankUp;
    private void Start()
    {
        TargetCharaterButton.onClick.AddListener(() => {

            MainManager.instance.TargetCharater = charaterLists[charaterNo];
        });

        LevelUpButton.onClick.AddListener(() => {
            if (itemStetasList[targetNo].Level < 5)
            {
                GameServer.instance.LevelUp_Server(1000, targetNo.ToString(), new EleCellJsonCallback(delegate (string err, JSONClass message) {
                    itemStetasList[targetNo].Level++;
                    SetCharaterText(targetNo);
                }));
            }



        });
    }
    public override void Init()
    {
        charaterLists = ReadCsv.MyReadCSV.Read("Csv/Charater");
        Debug.Log(".......TEST");

        GameServer.instance.LoadCharater_Server(MjSave.instance.playerID.Replace(".", ""), new EleCellJsonCallback(delegate (string err, JSONClass message) {

            if (message.Count == 0)
            {
                return;
            }
            itemStetasList.Clear();
            Debug.Log(message);
            for (int i = 0; i < message[0].Count; i++)
            {

                int no = message[0][i]["type"].AsInt;

                var data = new ItemStetas();


                data.Level = message[0][i]["Level"].AsInt + 1;
                data.Rank = message[0][i]["qty"].AsInt;
                data.Name = charaterLists[no][1];
                data.HP = int.Parse(charaterLists[no][2]);
                data.MP = int.Parse(charaterLists[no][3]);
                data.ATK = int.Parse(charaterLists[no][5]);
                data.REC = float.Parse(charaterLists[no][4]);

                itemStetasList.Add(no, data);

                var tmp = Instantiate(charaterButtonPerfab, createrTran);
                tmp.gameObject.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/Charater/" + no.ToString());

                tmp.onClick.AddListener(() =>
                {

                    SetCharaterText(no);

                    RankUp.SetUpRank(itemStetasList[no].Rank);
                    charaterNo = no;

                });

                if (i == 0)
                {
                    SetCharaterText(no);
                    //charaterNo = 1;
                    //MainManager.instance.TargetCharater = charaterLists[1];
                }

            }


        }));




    }







    public void SetCharaterText(int No)
    {

        targetNo = No;
        Level.text = itemStetasList[No].Level.ToString();
        Name.text = itemStetasList[No].Name;
        HP.text =itemStetasList[No].HP.ToString();
        MP.text = itemStetasList[No].MP.ToString();
        ATK.text = itemStetasList[No].ATK.ToString();
        REC.text = itemStetasList[No].REC.ToString();
        image.sprite = Resources.Load<Sprite>("Image/Charater/" + No.ToString());
        for (int i = 0; i < Lv.Length; i++)
        {
            if (itemStetasList[No].Level >= (i + 1))
            {
                Lv[i].fontStyle = FontStyle.Bold;
                Color color = new Color();
                color.a = 1f;
                Lv[i].color = color;
            }
            else
            {
                Lv[i].fontStyle = FontStyle.Normal;
                Color color = new Color();
                color.a = 0.5f;
                Lv[i].color = color;
            }
            Lv[i].text = "Lv" + (i + 1).ToString() + ": " + charaterLists[No][9 + i * 4];
        }
    }


}
