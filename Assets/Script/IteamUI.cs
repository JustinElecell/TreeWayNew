using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IteamUI : MonoBehaviour
{
    public GameObject IteamObj;
    public GameObject TargetObj;

    public int Count = 0;
    Image IteamImage;
    Button IteanButton;




    private void Awake()
    {
        RefreshUI();
        IteamImage = IteamObj.GetComponent<Image>();
        IteanButton = IteamObj.GetComponent<Button>();

        // IteamUI按鍵
        IteanButton.onClick.AddListener(() =>
        {



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
