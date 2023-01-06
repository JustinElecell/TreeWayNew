using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadCsv;
using UnityEngine.UI;
using EleCellLogin;
using SimpleJSON;

public class IteamPanel : BasePanel
{
    List<List<string>> IteamLists;
    //List<List<string>> WeaponLists;
    //List<List<string>> MagicLists;
    //List<List<string>> InvokedLists;

    [Header("內容")]
    public Button[] Buttons;
    public Transform[] Panels;

    public Button IteamsObjButton;

    public Text Name;
    public Text Level;

    public Text DMG;
    public Text HP;
    public Text MP;
    public Text SPD;
    public Text Special;
    public Color color;
    public Image image;

    public Button ChangeButton;

    SO_Iteam targetIteam;
    public Button[] TargetSkillButton;



    bool IsLoadFloag = false;

    private void Awake()
    {
        if (!IsLoadFloag)
        {

            IteamLists = ReadCsv.MyReadCSV.Read("Csv/Iteam");

            for (int i = 0; i < Buttons.Length; i++)
            {
                int no = i;
                Buttons[i].onClick.AddListener(() =>
                {
                    for (int r = 0; r < Panels.Length; r++)
                    {
                        Panels[r].gameObject.transform.parent.transform.parent.gameObject.SetActive(false);
                        Buttons[r].gameObject.GetComponent<Image>().color = Color.white;
                    }
                    Panels[no].gameObject.transform.parent.transform.parent.gameObject.SetActive(true);
                    Buttons[no].gameObject.GetComponent<Image>().color = color;


                });
            }

            ChangeButton.onClick.AddListener(() => {

                MainManager.instance.skillIteams[MainManager.instance.targetSkillNo] = targetIteam;
                TargetSkillButton[MainManager.instance.targetSkillNo].gameObject.GetComponent<Image>().sprite = targetIteam.IteamImage;
            });

            IsLoadFloag = true;
        }
    }


    int FindItem(string itemNo)
    {
        for (int i = 0; i < IteamLists.Count; i++)
        {
            if (IteamLists[i][0] == itemNo)
            {

                return i;

            }

        }
        Debug.Log("沒找到對應編號道具");
        return 0;
    }
    private void Start()
    {
        Init();
    }
    public override void Init()
    {
        GameServer.instance.LoadItem_Server(MjSave.instance.playerID.Replace(".", ""), new EleCellJsonCallback(delegate (string err, JSONClass message) {

            if (message.Count == 0)
            {
                return;
            }
            Debug.Log(message);

            for (int i = 0; i < message[0].Count; i++)
            {
                var no = FindItem(message[0][i]["type"]);
                var data = Resources.Load<SO_Iteam>("Iteam/PlayerItem/" + IteamLists[no][1]);
                int level = int.Parse(message[0][i]["qty"]);

                switch (data.type)
                {
                    case SO_Iteam.IteamType.武具:
                        var tmp = Instantiate(IteamsObjButton, Panels[0]);
                        tmp.gameObject.GetComponent<Image>().sprite = data.IteamImage;
                        tmp.onClick.AddListener(() =>
                        {
                            SetText(data, no, level);

                        });
                        tmp.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Lv " + level;
                        break;
                    case SO_Iteam.IteamType.魔法:

                        var tmp_M = Instantiate(IteamsObjButton, Panels[1]);
                        int no_M = i;
                        tmp_M.gameObject.GetComponent<Image>().sprite = data.IteamImage;
                        tmp_M.onClick.AddListener(() =>
                        {


                            SetText(data, no, level);

                        });
                        tmp_M.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Lv " + level;

                        break;
                    case SO_Iteam.IteamType.召喚:

                        var tmp_I = Instantiate(IteamsObjButton, Panels[2]);
                        int no_I = i;
                        tmp_I.onClick.AddListener(() =>
                        {

                            Debug.Log(IteamLists[no_I][0]);
                            SetText(data, no, level);
                        });
                        tmp_I.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Lv " + level;
                        break;
                }

            }

            var no2 = FindItem(message[0][0]["type"]);
            var data2 = Resources.Load<SO_Iteam>("Iteam/PlayerItem/" + IteamLists[no2][1]);
            int level2 = int.Parse(message[0][0]["qty"]);

            SetText(data2, no2, level2);
        }));

        for (int i = 0; i < TargetSkillButton.Length; i++)
        {
            int no = i;
            TargetSkillButton[i].onClick.AddListener(() => {

                for (int r = 0; r < 5; r++)
                {
                    TargetSkillButton[r].gameObject.GetComponent<Image>().color = Color.white;
                }
                TargetSkillButton[no].gameObject.GetComponent<Image>().color = Color.green;
                MainManager.instance.targetSkillNo = no;
            });
        }
    }




    void SetText(SO_Iteam data,int no,int level)
    {
        Name.text = data.IteamName;
        DMG.text = data.Atk.ToString()+"%";
        HP.text = data.Hp.ToString();
        MP.text = data.Mp.ToString();
        SPD.text = data.Speed.ToString();
        Special.text = IteamLists[no][13];

        Level.text = "Lv " + level;
        targetIteam = data;

        if (targetIteam != null)
        {
            image.sprite = targetIteam.IteamImage;

        }


    }




}
