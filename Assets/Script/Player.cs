using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public SO_Player playerDate;
    public SO_Player tmpPlayerData;

    public void Init()
    {
        var tmpPos =new Vector3(MainManager.instance.Rect.rect.size.x / 2, 0, 0);

        gameObject.transform.SetParent(MainManager.instance.Rect.transform.GetChild(1).gameObject.transform);
        gameObject.transform.localPosition = tmpPos;

        if (tmpPlayerData == null)
        {
            tmpPlayerData = Instantiate(playerDate);
        }

        ResetPlayerHp();
        ResetPlayerMp();
    }


    public void ResetPlayerHp()
    {
        GamePlayManager.instance.infoPanel.PlayerHpText.text = tmpPlayerData.Hp.ToString() + " / " + playerDate.Hp.ToString();
        float tmp = tmpPlayerData.Hp / playerDate.Hp;
        Debug.Log(tmp);
        GamePlayManager.instance.infoPanel.PlayerHpImage.fillAmount = tmp;
    }

    public void ResetPlayerMp()
    {
        GamePlayManager.instance.infoPanel.PlayerMpText.text = tmpPlayerData.Mp.ToString() + " / " + playerDate.Mp.ToString();
        float tmp = tmpPlayerData.Mp / playerDate.Mp;
        Debug.Log(tmp);
        GamePlayManager.instance.infoPanel.PlayerMpImage.fillAmount = tmp;

    }
}
