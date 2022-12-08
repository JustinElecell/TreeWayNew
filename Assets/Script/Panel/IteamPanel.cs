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
                    Buttons[r].gameObject.GetComponent<Image>().color = color;
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
                    tmp.onClick.AddListener(() => {
                        
                        for(int r=0;r< Panels[0].childCount;r++)
                        {
                            Panels[0].GetChild(r).gameObject.GetComponent<Image>().color = Color.white;
                        }

                        Debug.Log(IteamLists[no][0]);
                        tmp.gameObject.GetComponent<Image>().color = color;
                        SetText(no);


                    });
                    break;
                case "魔法":

                    var tmp_M = Instantiate(IteamsObjButton, Panels[1]);
                    int no_M = i;
                    tmp_M.onClick.AddListener(() => {
                        for (int r = 0; r < Panels[1].childCount; r++)
                        {
                            Panels[1].GetChild(r).gameObject.GetComponent<Image>().color = Color.white;
                        }
                        Debug.Log(IteamLists[no_M][0]);
                        tmp_M.gameObject.GetComponent<Image>().color = color;

                        SetText(no_M);

                    });
                    break;
                case "召喚":

                    var tmp_I = Instantiate(IteamsObjButton, Panels[2]);
                    int no_I = i;
                    tmp_I.onClick.AddListener(() => {
                        for (int r = 0; r < Panels[2].childCount; r++)
                        {
                            Panels[2].GetChild(r).gameObject.GetComponent<Image>().color = Color.white;
                        }
                        Debug.Log(IteamLists[no_I][0]);
                        SetText(no_I);
                        tmp_I.gameObject.GetComponent<Image>().color = color;


                    });
                    break;
            }
        }

        SetText(1);
        Buttons[0].gameObject.GetComponent<Image>().color = color;
        Panels[0].GetChild(0).GetComponent<Image>().color = color;
    }

    void SetText(int no)
    {
        Name.text = IteamLists[no][0];
        DMG.text = IteamLists[no][4];
        HP.text = IteamLists[no][8];
        MP.text = IteamLists[no][7];
        SPD.text = IteamLists[no][10];
        Special.text = IteamLists[no][12];




    }




}
