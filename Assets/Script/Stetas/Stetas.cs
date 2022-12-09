using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Stetas : MonoBehaviour
{
    public enum Type
    {
        道具,
        敵人,
        召喚,
        Boss
    }
    public enum ActionType
    {
        攻擊,
        移動
    }

    public Type type;
    public Image hpBar;
    [Header("玩家")]
    public SO_Player player;


    [Header("道具")]
    public SO_Iteam iteam;

    [Header("敵人")]
    public SO_Enemy enemy;
    public bool BossHpBarFlag;
    [Header("共通用")]
    public float Hp;
    public float HpMax;
    public ActionType actionType;
    public int roadNo;

    public int WeaponAtkChange(float atk)
    {
        return ((int)((GamePlayManager.instance.player.stetas.player.Atk * (1 + 0) * (1 + 0)) * (atk / 100 * (1 + 0))));
    }

    //false:死    true:還活著

    public bool CheckIsAlive()
    {
        if (Hp > 0)
        {
            return true;
        }
        return false;
    }

    public void TakeDamage(int damage)
    {
        
        Hp -= damage;

        if(hpBar!=null)
        {
            if ( Hp != HpMax)
            {
                hpBar.gameObject.SetActive(true);
                float tmp = Hp / HpMax;

                hpBar.fillAmount = tmp;
            }
        }

        if(GamePlayManager.instance.infoPanel.BossHpImage.gameObject.activeSelf&&BossHpBarFlag)
        {
            if (Hp != HpMax)
            {
                if(Hp<0)
                {
                    Hp = 0;
                }
                float tmp = Hp / HpMax;

                GamePlayManager.instance.infoPanel.BossHpImage.fillAmount = tmp;

                GamePlayManager.instance.infoPanel.BossHpText.text = Hp.ToString()+" / " + HpMax.ToString();
            }
        }

    }
    public void HpBarInit()
    {
        float tmp = Hp / HpMax;

        hpBar.fillAmount = tmp;

        hpBar.gameObject.SetActive(false);




    }

}
