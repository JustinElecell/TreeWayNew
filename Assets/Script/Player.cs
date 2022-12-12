using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Player : MonoBehaviour
{

    //public SO_Player playerDate;
    //public SO_Player tmpPlayerData;

    public Stetas stetas;

    public float mp;
    public float mpMax;
    public void Init()
    {
        var tmpPos =new Vector3((GamePlayManager.instance.Rect.rect.size.x / 2), 0, 0);

        gameObject.transform.SetParent(GamePlayManager.instance.Rect.transform.GetChild(1).transform);
        gameObject.transform.localPosition = tmpPos;

        var baseNo = int.Parse(MainManager.instance.TargetCharater[0]);
        
        gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Image/Charater/" + baseNo);
        
        //Hp初始化
        var baseHp = int.Parse(MainManager.instance.TargetCharater[2]);
        var baseHpUpRate = float.Parse(MainManager.instance.TargetCharater[7]);
        stetas.player.Hp = baseHp * (1+stetas.player.Overfulfil* baseHpUpRate) * (1+0);
        
        stetas.HpMax = stetas.player.Hp;
        stetas.Hp = stetas.HpMax;

        //mp初始化
        var baseMpMax = int.Parse(MainManager.instance.TargetCharater[3]);
        var baseMpUpRate = float.Parse(MainManager.instance.TargetCharater[8]);
        var tmpMpMax = baseMpMax * (1 +stetas.player.Overfulfil * baseMpUpRate)*(1+0);
        mpMax = ((int)(Mathf.Round(((float)(tmpMpMax)))));
        var tmpMp = baseMpMax * 0.3 * (1 + 0);
        mp = ((float)tmpMp);
        stetas.player.MpUp = float.Parse(MainManager.instance.TargetCharater[4]);

        //攻擊力初始化
        var baseAtk= int.Parse(MainManager.instance.TargetCharater[5]);
        var baseAtkUp = float.Parse(MainManager.instance.TargetCharater[6]);
        stetas.player.Atk = ((int)(baseAtk * (1 + stetas.player.Overfulfil * baseAtkUp) * (1 + 0)));


        ResetPlayerHp();

        ResetPlayerMp();

        StartCoroutine(MpUpdate());
    }


    public void ResetPlayerHp()
    {
        if(stetas.Hp<0)
        {
            stetas.Hp = 0;
        }
        GamePlayManager.instance.infoPanel.PlayerHpText.text = stetas.Hp.ToString() + " / " + stetas.HpMax.ToString();
        float tmp = stetas.Hp / stetas.HpMax;
        GamePlayManager.instance.infoPanel.PlayerHpImage.fillAmount = tmp;

        if (stetas.Hp <= 0)
        {
            //StopAllCoroutines();
            if(!MainManager.instance.TestFlag)
            {
                GamePlayManager.instance.Pause();

                GamePlayManager.instance.GameOver.SetActive(true);

            }

        }
    }

    public void ResetPlayerMp()
    {
        GamePlayManager.instance.infoPanel.PlayerMpText.text = mp.ToString("0") + " / " + mpMax.ToString();
        float tmp = mp / mpMax;
        GamePlayManager.instance.infoPanel.PlayerMpImage.fillAmount = tmp;

    }
    IEnumerator MpUpdate()
    {

        while(this.gameObject.activeSelf)
        {
            if (mp < mpMax)
            {
                //MP恢復率(每秒回復多少MP)=( 10 * ( 1 + Buff合計 ) ) * ( 1 + ( 當前等級 - 1  ) * 0.2 )
                var tmpMpUp = (stetas.player.MpUp * (1 + 0)) * 1 + ((stetas.player.Level - 1) * 0.2);
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
