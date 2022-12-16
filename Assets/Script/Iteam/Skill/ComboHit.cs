using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ComboHit : IteamSkillBase
{
    public int comboHitMax;

    float saveTime;
    InvokedMove player;

    public override void SkillEffect(GameObject obj)
    {
        if (stetas.actionType != Stetas.ActionType.技能時)
        {
            player = this.gameObject.GetComponent<InvokedMove>();

            stetas.actionType = Stetas.ActionType.技能時;
            saveTime = Time.time;
            StartCoroutine(IEComboHit());

            Action action = () => {
                if (player.enemyList.Count > 0)
                {

                    if (player.enemyList[0] != null)
                    {
                        var tmp = player.enemyList[0].GetComponent<Stetas>();

                        if (tmp.Hp <= 0)
                        {
                            player.enemyList.RemoveAt(0);
                            return;
                        }

                        if (Time.time - saveTime >= stetas.iteam.Atk_wait)
                        {
                            StartCoroutine(IEComboHit());

                            saveTime = Time.time;
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

    IEnumerator IEComboHit()
    {

        for (int i=0;i<comboHitMax;i++)
        {
            int EnemyNo = 0;
        test: Debug.Log("開始檢測");
            if(EnemyNo>= player.enemyList.Count || stetas.actionType == Stetas.ActionType.移動)
            {
                goto over;
            }

            if(player.enemyList[EnemyNo].GetComponent<Stetas>().Hp<=0)
            {
                EnemyNo++;
                Debug.Log("連擊敵人血歸0，打下一隻");

                goto test;
            }
            else
            {
                Debug.Log("第" + (i+1).ToString() + "下");
                player.enemyList[EnemyNo].GetComponent<Stetas>().TakeDamage(stetas.WeaponAtkChange(stetas.iteam));
            }
            yield return new WaitForSeconds(0.1f);

        }

    over: Debug.Log("連擊結束");

    }

}
