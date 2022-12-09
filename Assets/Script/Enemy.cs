using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Enemy : MonoBehaviour
{

    public Stetas stetas;

    BoxCollider coll;
    float speed = 0;




    Dictionary<Stetas.ActionType, Action> ActionTypeFunc = new Dictionary<Stetas.ActionType, Action>();

    List<GameObject> InvokedList = new List<GameObject>();
    public InvokedMove TargetInvoked;

    public int roadNo;

    float saveTime;
    bool canAttack = true;
    private void OnEnable()
    {

        coll = GetComponent<BoxCollider>();
        var tmpPos = new Vector3(-GamePlayManager.instance.Rect.rect.size.x / 2, 0, 0);
        gameObject.transform.localPosition = tmpPos;
        speed = GamePlayManager.instance.Rect.rect.size.x / stetas.enemy.speed;

        //var tmpHp = ((float)(20 * 1 + (1 - 1) * 0.1)) * (1 + (GamePlayManager.instance.Wave - 1)  *0.2)*(stetas.enemy.hp/100);

        //roundup( ( 50 * ( 1 + ( ( 關卡編號 - 1 ) *0.1 ) ) ) * 該怪物血量調整比 * ( 1 + ( ( 當前波次編號 - 1 ) * 0.25 ) ) )
        var tmpHp = (50 * (1 + ((1 - 1) * 0.1))) *(stetas.enemy.hp/100)* (1 + ((GamePlayManager.instance.Wave - 1) * 0.25));

        var tmpHpFloat =((float)tmpHp);

        roadNo = gameObject.transform.parent.transform.GetSiblingIndex() + 1;

        stetas.HpMax = ((int)(Mathf.Round(tmpHpFloat)));
        stetas.Hp = stetas.HpMax;
        stetas.actionType = Stetas.ActionType.移動;
        
        stetas.hpBar.gameObject.SetActive(false);
        stetas.hpBar.fillAmount = 1;
        canAttack = true;
    }


    private void Start()
    {
        FuncInit();
    }

    void FuncInit()
    {
        ActionTypeFunc.Add(Stetas.ActionType.移動, () => {
            if (gameObject.transform.localPosition.x < GamePlayManager.instance.Rect.rect.size.x / 2 - 150)
            {
                // 移動
                var pos = gameObject.transform.localPosition;
                pos.x += (speed * Time.deltaTime);
                gameObject.transform.localPosition = pos;
                
            }
            else
            {
                if (canAttack)
                {
                    GamePlayManager.instance.player.stetas.Hp -= stetas.enemy.Atk;
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
        });

        ActionTypeFunc.Add(Stetas.ActionType.攻擊, () => {


            if (TargetInvoked != null)
            {
                if (!TargetInvoked.stetas.CheckIsAlive())
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

                if(canAttack)
                {
                    if(TargetInvoked!=null&&TargetInvoked.gameObject.activeSelf)
                    {
                        TargetInvoked.stetas.TakeDamage(stetas.enemy.Atk);
                        saveTime = Time.time;

                        canAttack = false;
                    }

                }
                else
                {
                    if (Time.time - saveTime >= stetas.enemy.Atk_wait)
                    {
                        canAttack = true;
                    }
                }

            }

        });


    }

    bool FindFightInvoked()
    {
        if (GamePlayManager.instance.roads[roadNo - 1].transform.GetChild(1).transform.childCount > 0)
        {
            Debug.Log("測試"+ GamePlayManager.instance.roads[roadNo - 1].transform.GetChild(1).transform.GetChild(0).GetComponent<InvokedMove>());
            TargetInvoked = GamePlayManager.instance.roads[roadNo - 1].transform.GetChild(1).transform.GetChild(0).GetComponent<InvokedMove>();

            return true;
        }
        return false;


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
        var tmp = GamePlayManager.instance.iteamGround_Enemy.transform.Find(stetas.enemy.name + "物件池");

        gameObject.transform.SetParent(tmp.transform);


    }

    IEnumerator Attack()
    {

        while(GamePlayManager.instance.FindPlayer())
        {
            // 攻擊
            yield return new WaitForSeconds(stetas.enemy.Atk_wait);

            GamePlayManager.instance.EAttack(stetas.enemy.Atk);

        }
    }


    private void OnTriggerEnter(Collider other)
    {

        if(other.gameObject.tag=="Player")
        {
            var otherstetas = other.gameObject.GetComponent<Stetas>();
            switch (otherstetas.type)
            {
                case Stetas.Type.召喚:
                    var tmpObj = other.gameObject;
                    tmpObj.transform.SetParent(GamePlayManager.instance.roads[roadNo - 1].transform.GetChild(1).transform);

                    InvokedList.Add(tmpObj);
                    saveTime = Time.time;

                    FindFightInvoked();

                    break;

            }
        }



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

            }
        }
    }



}
