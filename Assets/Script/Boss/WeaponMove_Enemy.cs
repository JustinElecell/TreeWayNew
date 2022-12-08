using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class WeaponMove_Enemy : MonoBehaviour
{

    public Stetas stetas;

    float speed = 0;
    BoxCollider coll;
    Dictionary<Stetas.ActionType, Action> ActionTypeFunc = new Dictionary<Stetas.ActionType, Action>();

    List<GameObject> InvokedList = new List<GameObject>();
    public List<GameObject> WeaponList = new List<GameObject>();

    public Stetas TargetInvoked;

    float saveTime;
    bool canAttack = true;

    private void OnEnable()
    {

        coll = GetComponent<BoxCollider>();


            speed = MainManager.instance.Rect.rect.size.x / stetas.iteam.Speed;

        switch (stetas.type)
        {
            case Stetas.Type.道具:
                stetas.HpMax = (5 * 1 + (GamePlayManager.instance.Wave - 1) * 0.2f) * (stetas.iteam.Hp / 100);
                stetas.Hp = stetas.HpMax;
                break;
            case Stetas.Type.召喚:
                stetas.HpMax = stetas.iteam.Hp;
                stetas.Hp = stetas.HpMax;
                break;
        }
        
        stetas.actionType = Stetas.ActionType.移動;
        stetas.roadNo = gameObject.transform.parent.transform.GetSiblingIndex() + 1;

        canAttack = true;

    }
    private void OnDisable()
    {
        WeaponList.Clear();
        InvokedList.Clear();
    }
    private void Start()
    {
        FuncInit();
    }

    void FuncInit()
    {
        ActionTypeFunc.Add(Stetas.ActionType.移動, () => {
            if (gameObject.transform.localPosition.x < MainManager.instance.Rect.rect.size.x / 2 - 150)
            {
                // 移動
                var pos = gameObject.transform.localPosition;
                pos.x += (speed * Time.deltaTime);
                gameObject.transform.localPosition = pos;

            }
            else
            {
                if(stetas.type==Stetas.Type.道具)
                {
                    GamePlayManager.instance.player.tmpPlayerData.Hp -= stetas.iteam.Atk;
                    GamePlayManager.instance.player.ResetPlayerHp();

                    ReSet();

                }
                else if(stetas.type == Stetas.Type.召喚)
                {
                    if (canAttack)
                    {
                        GamePlayManager.instance.player.tmpPlayerData.Hp -= stetas.iteam.Atk;
                        GamePlayManager.instance.player.ResetPlayerHp();
                        saveTime = Time.time;

                        canAttack = false;
                    }
                    else
                    {
                        if (Time.time - saveTime >= stetas.enemy.Atk_wait)
                        {
                            canAttack = true;
                        }
                    }

                }
            }
        });

        ActionTypeFunc.Add(Stetas.ActionType.攻擊, () => {


            if (TargetInvoked != null)
            {
                if (!TargetInvoked.CheckIsAlive())
                {
                    if (InvokedList.Count <= 0)
                    {
                        stetas.actionType = Stetas.ActionType.移動;

                    }
                    else
                    {
                        TargetInvoked = null;
                        if (FindFightInvoked())
                        {


                        }
                        else
                        {
                            // 沒有戰鬥中敵人
                            stetas.actionType = Stetas.ActionType.移動;
                        }
                    }
                }

                if (canAttack)
                {
                    TargetInvoked.TakeDamage(((int)stetas.iteam.Atk));
                    saveTime = Time.time;

                    canAttack = false;
                }
                else
                {
                    if (Time.time - saveTime >= stetas.iteam.Atk_wait)
                    {
                        canAttack = true;
                    }
                }

            }

        });


    }

    private void Update()
    {
        if (stetas.Hp <= 0)
        {
            ReSet();
        }

        ActionTypeFunc[stetas.actionType]();


    }

    void ReSet()
    {
        gameObject.SetActive(false);
        var tmp = GamePlayManager.instance.iteamGround_Enemy.transform.Find(stetas.iteam.IteamName + "物件池");

        gameObject.transform.SetParent(tmp.transform);
    }

    private void OnTriggerEnter(Collider other)
    {


        if(other.gameObject.tag == "Player")
        {
            var otherstetas = other.gameObject.GetComponent<Stetas>();
            switch (otherstetas.type)
            {

                case Stetas.Type.召喚:
                    
                    if(stetas.type== Stetas.Type.召喚)
                    {
                        var tmpObj = other.gameObject;
                        tmpObj.transform.SetParent(GamePlayManager.instance.roads[stetas.roadNo - 1].transform.GetChild(1).transform);

                        InvokedList.Add(tmpObj);
                        saveTime = Time.time;

                        FindFightInvoked();
                    }
                    else if(stetas.type == Stetas.Type.道具)
                    {
                        WeaponList.Add(other.gameObject);

                    }


                    break;
            }
        }

        for (int i = 0; i < WeaponList.Count; i++)
        {
            var otherstetas = WeaponList[i].GetComponent<Stetas>();
            switch (otherstetas.type)
            {
                case Stetas.Type.道具:
                    otherstetas.TakeDamage(((int)stetas.iteam.Atk));
                    stetas.Hp -= otherstetas.iteam.Atk;

                    break;

                case Stetas.Type.召喚:
                    otherstetas.TakeDamage(((int)stetas.iteam.Atk));
                    stetas.Hp -= otherstetas.iteam.Atk;
                    break;
            }
            if (stetas.Hp <= 0)
            {
                ReSet();
            }
        }
    }

    bool FindFightInvoked()
    {
        if (GamePlayManager.instance.roads[stetas.roadNo - 1].transform.GetChild(1).transform.childCount > 0)
        {
            
            var tmp  = GamePlayManager.instance.roads[stetas.roadNo - 1].transform.GetChild(0).transform.GetChild(0).GetComponent<Stetas>();

            foreach (var dataObj in InvokedList)
            {
                if (dataObj == tmp.gameObject)
                {
                    TargetInvoked = tmp;
                    return true;
                }
            }
        }
        return false;
    }

    private void OnTriggerExit(Collider other)
    {


        if (other.gameObject.tag == "Player")
        {
            var otherstetas = other.gameObject.GetComponent<Stetas>();
            switch (otherstetas.type)
            {
                case Stetas.Type.召喚:
                    InvokedList.Remove(other.gameObject);
                    break;
                case Stetas.Type.道具:
                    WeaponList.Remove(other.gameObject);

                    break;
            }
        }
    }
}
