using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Repel : IteamSkillBase
{

    public float Max;
    [Header("true:可持續擊退 false:不可持續擊退")]
    public bool alwaysRepel;
    InvokedMove player;
    Action action;
    bool canAtk;
    private void OnEnable()
    {
        canAtk = true;
    }
    public override void SkillEffect(GameObject obj)
    {


        if(!alwaysRepel)
        {
            Debug.Log(obj.name);
            //if(obj!=null)
            {
                if (obj.GetComponent<Stetas>().Weight > 10)
                {
                    return;
                }
                StartCoroutine(Effect(obj));

            }
        }
        else
        {
            if (stetas.actionType != Stetas.ActionType.技能時)
            {
                player = this.gameObject.GetComponent<InvokedMove>();

                stetas.actionType = Stetas.ActionType.技能時;

                Action action = () =>
                {
                    if (player.enemyList.Count == 0)
                    {
                        player.ResetSpeedSaveTime();
                        player.ReSetSpeed();
                        stetas.actionType = Stetas.ActionType.移動;
                        return;
                    }

                    if (player.enemyList[0].GetComponent<Stetas>().Hp<=0)
                    {
                        player.enemyList.RemoveAt(0);
                    }

                    if (canAtk&& player.enemyList.Count>0)
                    {
                        canAtk = false;
                        player.enemyList[0].GetComponent<Stetas>().CantMoveCount = 0.05f;
                        player.enemyList[0].GetComponent<Stetas>().saveTime = Time.time;
                        player.enemyList[0].GetComponent<Stetas>().repelMax = Max;
                        player.enemyList[0].GetComponent<Stetas>().actionType = Stetas.ActionType.不能動作;
                        
                        player.enemyList[0].GetComponent<Stetas>().TakeDamage(stetas.WeaponAtkChange(stetas.iteam));

                        StartCoroutine(IERepelCD());

                        if (obj.GetComponent<Stetas>().Weight > 10)
                        {
                            return;
                        }
                        StartCoroutine(IERepel(player.enemyList[0]));

                    }




                };

                player.ActionTypeFunc[Stetas.ActionType.技能時] = action;
            }
        }
    }


    IEnumerator IERepelCD()
    {
        yield return new WaitForSeconds(stetas.iteam.Atk_wait);
        canAtk = true;
    }


    List<GameObject> objList = new List<GameObject>();

    IEnumerator IERepel(GameObject obj)
    {
        var speed = GamePlayManager.instance.Rect.rect.size.x * (Max / 100);

        for (int i = 0; i < 5; i++)
        {
            var pos = obj.transform.localPosition;

            pos.x -= ((float)speed / 5);
            obj.transform.localPosition = pos;

            yield return new WaitForSeconds(0.01f);
        }
    }

    IEnumerator Effect(GameObject obj)
    {

        if(!alwaysRepel)
        {
            foreach(var data in objList)
            {
                if(data.gameObject==obj)
                {
                    goto Test;
                }
            }
        }

 
        obj.GetComponent<Stetas>().CantMoveCount = 0.05f;
        obj.GetComponent<Stetas>().saveTime = Time.time;
        obj.GetComponent<Stetas>().repelMax = Max;
        obj.GetComponent<Stetas>().actionType = Stetas.ActionType.不能動作;
        if (!alwaysRepel)
        {
            objList.Add(obj);
        }
        StartCoroutine(IERepel(obj));

    Test: Debug.Log("跳過");
        yield return null;

    }


}
