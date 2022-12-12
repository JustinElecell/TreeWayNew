using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IteamUI : MonoBehaviour
{
    public SO_Iteam iteamData;
    public SO_Iteam tmpIteamData;
    public GameObject IteamObj;

    Image IteamImage;
    Button IteanButton;
    public Text SkillLimitText;




    private void Start()
    {
        if(tmpIteamData!=null)
        {
            iteamData = Instantiate(tmpIteamData);

        }
        RefreshUI();
        IteamImage = IteamObj.GetComponent<Image>();
        IteanButton = IteamObj.GetComponent<Button>();

        IteamImage.sprite = iteamData.IteamImage;
        SkillLimitText.text = iteamData.Mp.ToString();
        GameObject tmp = new GameObject(iteamData.IteamName + "物件池");
        tmp.transform.SetParent(GamePlayManager.instance.iteamGround_Player.transform);

        
        for(int i=0;i<30;i++)
        {
            Instantiate(iteamData.IteamPerfab, tmp.transform);

        }

        // IteamUI按鍵
        IteanButton.onClick.AddListener(() =>
        {
            //刷出武器
            //GamePlayManager.instance.PAttackInit(iteamData);

            for(int i=0;i<GamePlayManager.instance.UI.Length;i++)
            {
                GamePlayManager.instance.UI[i].color = GamePlayManager.instance.White;
            }

            gameObject.GetComponent<Image>().color = GamePlayManager.instance.Green;
            GamePlayManager.instance.Skill = iteamData;
        });
    }


    public void RefreshUI()
    {
        //if (nowIteamData == null)
        //{
        //    IteamObj.SetActive(false);
        //    TargetObj.SetActive(false);
        //    CountText.gameObject.SetActive(false);

        //    return;
        //}


        //TargetObj.SetActive(false);
        //CountText.gameObject.SetActive(true);
        //CountText.text = "X" + nowIteamData.count.ToString();

        //IteamObj.SetActive(true);
        //IteamImage.sprite = nowIteamData.image;

    }
}
