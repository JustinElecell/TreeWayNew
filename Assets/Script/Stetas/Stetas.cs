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

    }
    public enum ActionType
    {
        攻擊,
        移動
    }

    public Type type;
    public Image hpBar;

    [Header("道具")]
    public SO_Iteam iteam;

    [Header("敵人")]
    public SO_Enemy enemy;

    [Header("共通用")]
    public float Hp;
    public float HpMax;
    public ActionType actionType;

    public int WeaponAtkChange(float atk)
    {
        return ((int)((GamePlayManager.instance.player.tmpPlayerData.Atk * (1 + 0) * (1 + 0)) * (atk / 100 * (1 + 0))));
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

    }

}
