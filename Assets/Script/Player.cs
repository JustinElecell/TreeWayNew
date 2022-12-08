using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public SO_Player playerDate;
    public SO_Player tmpPlayerData;

    public float mp;
    public float mpMax;
    public void Init()
    {
        var tmpPos =new Vector3((MainManager.instance.Rect.rect.size.x / 2), 0, 0);

        gameObject.transform.SetParent(MainManager.instance.Rect.transform.GetChild(1).transform);
        gameObject.transform.localPosition = tmpPos;

        if (tmpPlayerData == null)
        {
            tmpPlayerData = Instantiate(playerDate);
        }

        ResetPlayerHp();


        var tmpMp = playerDate.Mp * (1 + (tmpPlayerData.Level - 1) * 0.5);
        //mpMax=
        mpMax = ((int)(Mathf.Round(((float)(tmpMp)))));
        mp = mpMax;
        ResetPlayerMp();

        StartCoroutine(MpUpdate());
    }


    public void ResetPlayerHp()
    {
        if(tmpPlayerData.Hp<0)
        {
            tmpPlayerData.Hp = 0;
        }
        GamePlayManager.instance.infoPanel.PlayerHpText.text = tmpPlayerData.Hp.ToString() + " / " + playerDate.Hp.ToString();
        float tmp = tmpPlayerData.Hp / playerDate.Hp;
        GamePlayManager.instance.infoPanel.PlayerHpImage.fillAmount = tmp;

        if (tmpPlayerData.Hp <= 0)
        {
            GamePlayManager.instance.Pause();
            //StopAllCoroutines();
            GamePlayManager.instance.GameOver.SetActive(true);

        }
    }

    public void ResetPlayerMp()
    {
        GamePlayManager.instance.infoPanel.PlayerMpText.text = mp.ToString() + " / " + mpMax.ToString();
        float tmp = mp / mpMax;
        GamePlayManager.instance.infoPanel.PlayerMpImage.fillAmount = tmp;

    }
    IEnumerator MpUpdate()
    {

        while(this.gameObject.activeSelf)
        {
            if (mp < mpMax)
            {
                var tmpMpUp = (10 * (1 + 0)) * 1 + ((tmpPlayerData.Level - 1) * 0.2);
                mp += ((float)(tmpMpUp));


                if (mp > mpMax)
                {
                    mp = mpMax;
                }

                ResetPlayerMp();
            }
            yield return new WaitForSeconds(1);
            


        }
    }
}
