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
    }
    public enum ActionType
    {
        攻擊,
        移動,
        不能動作,
        技能時
    }


    public Type type;
    public Image hpBar;
    [Header("玩家")]
    public SO_Player player;


    [Header("道具")]
    public SO_Iteam iteam;
    public IteamSkillBase Skill;
    [Header("敵人")]
    public SO_Enemy enemy;
    public bool BossHpBarFlag;

    public float CantMoveCount;
    public float repelMax=1f;

    [Header("共通用")]
    public float Hp;
    public float HpMax;
    public ActionType actionType;
    public int roadNo;
    public float saveTime;

    public float BuffAtkUp;
    public float Weight;
    public float damageDown = 0;
    

    public int WeaponAtkChange(SO_Iteam data)
    {
        //(roundup((玩家基礎攻擊力 * (1 + 突破次數 * 突破時攻擊力加成比例) * (1 + 其餘攻擊力buff %)) * (武器傷害 * (1 + 武器傷害Buff % )))) * ( if (觸發翻倍 = ture , 2 , 1 ) )         
        //突破次數 * 突破時攻擊力加成比例已經在遊戲開始時，在Player.cs初始化算在stetas.player.Atk內了
        return ((int)((GamePlayManager.instance.player.stetas.player.Atk * (1 + (GamePlayManager.instance.player.AtkUp+data.temporaryAtkUp)/100)) * (data.Atk / 100 * (1 + data.atkUp / 100))))*((int)(1+BuffAtkUp/100));
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

    public void TakeDamage(float damage, float damageDown)
    {
        var realDamage = Mathf.Max(damage * Mathf.Max(1 + damageDown / 100,0.01f), 1);

        GamePlayManager.instance.damageManager.Damage(this.gameObject, ((int)realDamage));
        Hp -= realDamage;

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
