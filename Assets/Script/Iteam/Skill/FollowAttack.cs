using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class FollowAttack : IteamSkillBase
{
    public GameObject targetobj;


    WeaponMove_Player player;
    float saveTime;
    public override void SkillEffect(GameObject obj)
    {
        player = this.gameObject.GetComponent<WeaponMove_Player>();
        targetobj = obj;

        stetas.actionType = Stetas.ActionType.技能時;
        saveTime = Time.time;
        Action action = () => {

            if(player.targetList.Count>0)
            {

                if (player.targetList[0] != null)
                {
                    var tmp = player.targetList[0].GetComponent<Stetas>();

                    if (tmp.Hp <= 0)
                    {
                        player.targetList.RemoveAt(0);
                        return;
                    }
                    this.gameObject.transform.localPosition = player.targetList[0].transform.localPosition;
                    if(Time.time-saveTime>=0.1)
                    {
                        Debug.Log("攻擊");
                        saveTime = Time.time;
                        tmp.TakeDamage(stetas.WeaponAtkChange(stetas.iteam)/10,tmp.damageDown);
                        stetas.Hp -= tmp.enemy.Atk/10;


                    }

                }
            }
            else
            {

                stetas.actionType = Stetas.ActionType.移動;

            }
        };

        player.ActionTypeFunc[Stetas.ActionType.技能時] = action;
    }

}
