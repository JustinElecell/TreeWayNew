using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InvokedMove_Boss : MonoBehaviour
{
    public Stetas stetas;

    float speed = 0;
    BoxCollider coll;
    int Hp;
    int Hpmax;

    public int roadNo;

    Dictionary<Stetas.ActionType, Action> ActionTypeFunc = new Dictionary<Stetas.ActionType, Action>();

    public Enemy TargetEnemy;
    List<GameObject> enemyList = new List<GameObject>();
    float saveTime;
    bool canAttack = true;
    private void OnEnable()
    {

        coll = GetComponent<BoxCollider>();


        var tmpPos = new Vector3(-GamePlayManager.instance.Rect.rect.size.x / 2, 0 + UnityEngine.Random.Range(-20, 20), 0);
        gameObject.transform.localPosition = tmpPos;
        speed = GamePlayManager.instance.Rect.rect.size.x / stetas.iteam.Speed;

        roadNo = gameObject.transform.parent.transform.GetSiblingIndex() + 1;
        stetas.HpMax = ((int)(stetas.iteam.Hp));

        stetas.Hp = stetas.HpMax;
        stetas.actionType = Stetas.ActionType.移動;
        canAttack = true;
    }

    private void Start()
    {
        FuncInit();
    }

    void FuncInit()
    {
        ActionTypeFunc.Add(Stetas.ActionType.移動, () => {
            if (gameObject.transform.localPosition.x < GamePlayManager.instance.Rect.rect.size.x / 2)
            {
                //移動
                var pos = gameObject.transform.localPosition;
                pos.x += (speed * Time.deltaTime);
                gameObject.transform.localPosition = pos;
            }
            else
            {
                ReSet();

            }


        });

        ActionTypeFunc.Add(Stetas.ActionType.攻擊, () => {



            if (TargetEnemy != null)
            {
                if (!TargetEnemy.stetas.CheckIsAlive())
                {
                    if (enemyList.Count <= 0)
                    {
                        stetas.actionType = Stetas.ActionType.移動;

                    }
                    else
                    {
                        TargetEnemy = null;
                        if (FindFightEnemy())
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
                    TargetEnemy.stetas.TakeDamage(stetas.WeaponAtkChange(stetas.iteam.Atk));
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
            else
            {
                stetas.actionType = Stetas.ActionType.移動;

            }
        });


    }

    bool FindFightEnemy()
    {
        if (GamePlayManager.instance.roads[roadNo - 1].transform.GetChild(0).transform.childCount > 0)
        {
            TargetEnemy = GamePlayManager.instance.roads[roadNo - 1].transform.GetChild(0).transform.GetChild(0).GetComponent<Enemy>();

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
        var tmp = GamePlayManager.instance.iteamGround_Enemy.transform.Find(stetas.iteam.IteamName + "物件池");

        gameObject.transform.SetParent(tmp.transform);
    }

    IEnumerator Attack(Collider other)
    {
        var otherstetas = other.gameObject.GetComponent<Stetas>();

        while (other.gameObject != null && otherstetas.enemy == null)
        {
            yield return new WaitForSeconds(otherstetas.enemy.Atk_wait);

            Hp -= otherstetas.enemy.Atk;

            Debug.Log(other.gameObject.name);
        }


    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Enemy")
        {
            if (other.gameObject.GetComponent<Enemy>().stetas.actionType == Stetas.ActionType.移動)
            {
                var tmpObj = other.gameObject;
                tmpObj.transform.SetParent(GamePlayManager.instance.roads[roadNo - 1].transform.GetChild(0).transform);


                var tmp = tmpObj.GetComponent<Enemy>();
                tmp.stetas.actionType = Stetas.ActionType.攻擊;




            }

            enemyList.Add(other.gameObject);
            stetas.actionType = Stetas.ActionType.攻擊;

            saveTime = Time.time;
            FindFightEnemy();


        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {

            enemyList.Remove(other.gameObject);

        }

        if (enemyList.Count <= 0)
        {
            stetas.actionType = Stetas.ActionType.移動;

        }
    }

}
