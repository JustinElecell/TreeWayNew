using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadCsv;
using UnityEngine.UI;


public class IteamPanel : MonoBehaviour
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

    private void Start()
    {
        Debug.Log("Init");
        IteamLists = ReadCsv.MyReadCSV.Read("Csv/Iteam");
        Debug.Log(IteamLists.Count);
        

        for(int i=0;i< Buttons.Length;i++)
        {
            int no = i;
            Buttons[i].onClick.AddListener(() => {
                for(int r=0;r< Panels.Length;r++)
                {
                    Panels[r].gameObject.transform.parent.transform.parent.gameObject.SetActive(false);
                    Buttons[r].gameObject.GetComponent<Image>().color = Color.white;
                }
                Panels[no].gameObject.transform.parent.transform.parent.gameObject.SetActive(true);
                Buttons[no].gameObject.GetComponent<Image>().color = color;


            });
        }

        for(int i=0;i<IteamLists.Count;i++)
        {
            switch(IteamLists[i][2])
            {
                case "武具":
                    var tmp = Instantiate(IteamsObjButton, Panels[0]);
                    int no = i;

                    var data = Resources.Load<SO_Iteam>("Iteam/Weapon/" + IteamLists[no][0]);
                    tmp.gameObject.GetComponent<Image>().sprite = data.IteamImage;

                    tmp.onClick.AddListener(() => {
                        


                        Debug.Log(IteamLists[no][0]);
                        SetText(no);



                    });
                    break;
                case "魔法":

                    var tmp_M = Instantiate(IteamsObjButton, Panels[1]);
                    int no_M = i;
                    var data_M = Resources.Load<SO_Iteam>("Iteam/Magic/" + IteamLists[no_M][0]);
                    tmp_M.gameObject.GetComponent<Image>().sprite = data_M.IteamImage;

                    tmp_M.onClick.AddListener(() => {

                        Debug.Log(IteamLists[no_M][0]);

                        SetText(no_M);

                    });
                    break;
                case "召喚":

                    var tmp_I = Instantiate(IteamsObjButton, Panels[2]);
                    int no_I = i;
                    tmp_I.onClick.AddListener(() => {

                        Debug.Log(IteamLists[no_I][0]);
                        SetText(no_I);


                    });
                    break;
            }
        }

        SetText(1);
        Buttons[0].gameObject.GetComponent<Image>().color = color;
        Panels[0].GetChild(0).GetComponent<Image>().color = color;

        ChangeButton.onClick.AddListener(() => {

            MainManager.instance.skillIteams[MainManager.instance.targetSkillNo] = targetIteam;
            TargetSkillButton[MainManager.instance.targetSkillNo].gameObject.GetComponent<Image>().sprite = targetIteam.IteamImage;
        });

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
            MainManager.instance.skillIteams[MainManager.instance.targetSkillNo] = targetIteam;
            TargetSkillButton[MainManager.instance.targetSkillNo].gameObject.GetComponent<Image>().sprite = targetIteam.IteamImage;
        }
    }

    void SetText(int no)
    {
        Name.text = IteamLists[no][0];
        DMG.text = IteamLists[no][4];
        HP.text = IteamLists[no][8];
        MP.text = IteamLists[no][7];
        SPD.text = IteamLists[no][10];
        Special.text = IteamLists[no][12];

        switch (IteamLists[no][2])
        {
            case "武具":
                targetIteam = Resources.Load<SO_Iteam>("Iteam/Weapon/" + IteamLists[no][0]);
                break;
            case "魔法":
                targetIteam = Resources.Load<SO_Iteam>("Iteam/Magic/" + IteamLists[no][0]);

                break;
            case "召喚":
                targetIteam = Resources.Load<SO_Iteam>("Iteam/Invoked/" + IteamLists[no][0]);

                break;
        }


        if (targetIteam != null)
        {
            image.sprite = targetIteam.IteamImage;

        }


    }




}
