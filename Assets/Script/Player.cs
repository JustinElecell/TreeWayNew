using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Player : MonoBehaviour
{

    //public SO_Player playerDate;
    //public SO_Player tmpPlayerData;

    public Stetas stetas;

    public float AtkUp;

    public float mp;
    public float mpMax;
    public float mpUp;
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
        stetas.player.REC = float.Parse(MainManager.instance.TargetCharater[4]);
        mpUp = 0;

        //攻擊力初始化
        var baseAtk = int.Parse(MainManager.instance.TargetCharater[5]);
        var baseAtkUp = float.Parse(MainManager.instance.TargetCharater[6]);
        stetas.player.Atk =baseAtk;

        for(int i=0;i<5/*stetas.player.Level*/;i++)
        {
            var skillNo = int.Parse(MainManager.instance.TargetCharater[10+i*4]);
            Debug.Log(skillNo);

            var x = int.Parse(MainManager.instance.TargetCharater[11+i*4]);
            Debug.Log(x);


            var y = 0;
            int.TryParse(MainManager.instance.TargetCharater[12+i*4],out y);
            Debug.Log(y);

            GamePlayManager.instance.playerSkillManager.AddPlayerSkill(skillNo,x,y);

        }

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

        if(stetas.Hp>stetas.HpMax)
        {
            stetas.Hp = stetas.HpMax;
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
                //(角色REC(魔力回復速度) * (1 + Buff合計)) * (1 + (當前等級 - 1) * 0.2)              ※此處當前等級意旨「關卡內透過消耗MP抽取Buff的次數，初始值為0」

                var tmpMpUp = (stetas.player.REC * (1 + mpUp/100)) * (1 + (GamePlayManager.instance.buffCount) * 0.2);
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
