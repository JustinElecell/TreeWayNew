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




    public  void Init(SO_Iteam data)
    {
        tmpIteamData = data;

        if (tmpIteamData!=null)
        {
            iteamData = Instantiate(tmpIteamData);

        }

        IteamImage = IteamObj.GetComponent<Image>();
        IteanButton = IteamObj.GetComponent<Button>();


        if (iteamData != null)
        {
            IteamImage.sprite = iteamData.IteamImage;
            this.gameObject.SetActive(true);
        }
        else
        {
            this.gameObject.SetActive(false);
            return;
        }

        RefreshUI();


        SkillLimitText.text = iteamData.Mp.ToString();

        for (int i = 0; i < 30; i++)
        {
            GamePlayManager.instance.CreateIteamInit(iteamData, GamePlayManager.instance.iteamGround_Player.transform);
        }

        // IteamUI按鍵
        IteanButton.onClick.AddListener(() =>
        {
            //刷出武器
            //GamePlayManager.instance.PAttackInit(iteamData);

            for(int i=0;i<GamePlayManager.instance.SkillButtonLists.Length;i++)
            {
                GamePlayManager.instance.SkillButtonLists[i].gameObject.GetComponent<Image>().color = GamePlayManager.instance.White;
            }

            gameObject.GetComponent<Image>().color = GamePlayManager.instance.Green;
            GamePlayManager.instance.Skill = iteamData;
        });
    }


    public void RefreshUI()
    {

        //iteamData.Mp = Mathf.Round(iteamData.Mp);
        SkillLimitText.text = Mathf.Round(iteamData.Mp).ToString();


    }
}
