using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReadCsv;
using UnityEngine.UI;
public class CharaterPanel : MonoBehaviour
{
    public Transform createrTran;
    public Button charaterButtonPerfab;
    List<List<string>> charaterLists;

    [Header("內容設定")]
    public Text Name;
    public Text Level;
    public Text HP;
    public Text MP;
    public Text ATK;
    public Text REC;
    
    public Text Lv1;
    public Text Lv2;
    public Text Lv3;
    public Text Lv4;
    public Text Lv5;




    private void Start()
    {
        charaterLists = ReadCsv.MyReadCSV.Read("Csv/Test");
        Debug.Log(charaterLists.Count);  
        
        for(int i=1;i<charaterLists.Count;i++)
        {
            int no = i;

            var tmp = Instantiate(charaterButtonPerfab, createrTran);
            tmp.onClick.AddListener(() => {

                SetCharaterText(no);


            });
        }
        SetCharaterText(1);



    }


    public void SetCharaterText(int No)
    {
        Name.text = charaterLists[No][1];
        Level.text = "1";
        HP.text = charaterLists[No][2];
        MP.text = charaterLists[No][3];
        ATK.text = charaterLists[No][5];
        REC.text = charaterLists[No][4];

        Lv1.text = "Lv1: " + charaterLists[No][9];
        Lv2.text = "Lv2: " + charaterLists[No][13];
        Lv3.text = "Lv3: " + charaterLists[No][17];
        Lv4.text = "Lv4: " + charaterLists[No][21];
        Lv5.text = "Lv5: " + charaterLists[No][25];



    }
}
