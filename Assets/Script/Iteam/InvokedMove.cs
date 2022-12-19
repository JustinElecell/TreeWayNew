using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class InvokedMove : MonoBehaviour
{
    public Stetas stetas;

    float speed, speedMax = 0;
    float speedUp;
    BoxCollider coll;
    int Hp;
    int Hpmax;


    public Dictionary<Stetas.ActionType, Action> ActionTypeFunc = new Dictionary<Stetas.ActionType, Action>();

    public Stetas TargetStetas;
    public List<GameObject> enemyList = new List<GameObject>();
    float saveTime;
    bool canAttack = true;

    float speedSaveTime;
    public void ResetSpeedSaveTime()
    {
        speedSaveTime = Time.time;
    }
    public void ReSetSpeed()
    {
        speed = 0;
    }
    private void OnEnable()
    {

        coll = GetComponent<BoxCollider>();

        var tmpPos = new Vector3(GamePlayManager.instance.Rect.rect.size.x / 2, 0+UnityEngine.Random.Range(-20,20), 0);
        gameObject.transform.localPosition = tmpPos;

        speedMax = GamePlayManager.instance.Rect.rect.size.x / stetas.iteam.Speed;

        speed = 0;
        speedUp = speedMax - speed;
        ResetSpeedSaveTime();

        stetas.roadNo = gameObject.transform.parent.transform.GetSiblingIndex() + 1;
        stetas.HpMax = ((int)(stetas.iteam.Hp));

        stetas.Hp = stetas.HpMax;
        stetas.actionType = Stetas.ActionType.移動;
        canAttack = true;

        stetas.HpBarInit();
    }

    private void OnDisable()
    {
        enemyList.Clear();


    }
    private void Start()
    {

        FuncInit();
    }

    void FuncInit()
    {
        ActionTypeFunc.Add(Stetas.ActionType.移動, () => {



            if (gameObject.transform.localPosition.x > -GamePlayManager.instance.Rect.rect.size.x / 2)
            {
                if(speed < speedMax)
                {
                    if(Time.time-speedSaveTime>1f/5)
                    {
                        speed += speedUp / 5;
                        ResetSpeedSaveTime();
                    }
                }
                else
                {
                    speed = speedMax;
                }


                //移動
                var pos = gameObject.transform.localPosition;
                pos.x -= (speed * Time.deltaTime);
                gameObject.transform.localPosition = pos;
                canAttack = true;

            }
            else
            {
                ReSet();

            }


        });

        ActionTypeFunc.Add(Stetas.ActionType.攻擊, () => {


            if (TargetStetas != null)
            {
                if (!TargetStetas.CheckIsAlive())
                {
                    if (enemyList.Count <= 0)
                    {
                        stetas.actionType = Stetas.ActionType.移動;
                        ResetSpeedSaveTime();

                    }
                    else
                    {
                        TargetStetas = null;
                        if (FindFightEnemy())
                        {


                        }
                        else
                        {
                            // 沒有戰鬥中敵人
                            stetas.actionType = Stetas.ActionType.移動;
                            ResetSpeedSaveTime();
                        }
                    }
                }

                if (canAttack)
                {


                    if (TargetStetas!=null&&TargetStetas.gameObject.activeSelf)
                    {
                        if (stetas.Skill != null && stetas.Skill.enabled == true&& TargetStetas.type != Stetas.Type.Boss)
                        {
                            stetas.Skill.SkillEffect(TargetStetas.gameObject);
                            return;
                        }

                        TargetStetas.TakeDamage(stetas.WeaponAtkChange(stetas.iteam));
                    }
                    saveTime = Time.time;
                    canAttack = false;
                    if(TargetStetas!=null&&TargetStetas.Hp<=0)
                    {
                        enemyList.Remove(TargetStetas.gameObject);
                    }
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

        ActionTypeFunc.Add(Stetas.ActionType.技能時, () => {

        });
    }    

    bool FindFightEnemy()
    {
        if (GamePlayManager.instance.roads[stetas.roadNo - 1].transform.GetChild(0).transform.childCount > 0)
        {
            var tmp = GamePlayManager.instance.roads[stetas.roadNo - 1].transform.GetChild(0).transform.GetChild(0).GetComponent<Stetas>();
            foreach(var dataObj in enemyList)
            {

                if(dataObj==tmp.gameObject)
                {
                    TargetStetas = tmp;
                    return true;
                }
            }

        }
        return false;

        
    }

    private void Update()
    {
        if (stetas.Hp <= 0)
        {
            ReSet();
        }
        else
        {
            ActionTypeFunc[stetas.actionType]();

        }
    }

    void ReSet()
    {
        gameObject.SetActive(false);
        var tmp = GamePlayManager.instance.iteamGround_Player.transform.Find(stetas.iteam.IteamName + "物件池");

        gameObject.transform.SetParent(tmp.transform);
        TargetStetas = null;
        enemyList.Clear();
        stetas.BuffAtkUp = 0;

    }

    IEnumerator Attack(Collider other)
    {
        var otherstetas = other.gameObject.GetComponent<Stetas>();

        while (other.gameObject!=null&& otherstetas.enemy == null)
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
            var tmpStetas = other.gameObject.GetComponent<Stetas>();

            if (tmpStetas.type == Stetas.Type.敵人|| tmpStetas.type == Stetas.Type.召喚)
            {
                if (tmpStetas.actionType == Stetas.ActionType.移動)
                {
                    var tmpObj = other.gameObject;
                    tmpObj.transform.SetParent(GamePlayManager.instance.roads[stetas.roadNo - 1].transform.GetChild(0).transform);


                    var tmp = tmpObj.GetComponent<Stetas>();
                    tmp.actionType = Stetas.ActionType.攻擊;
                }
            }



            enemyList.Add(other.gameObject);
            FindFightEnemy();


            stetas.actionType = Stetas.ActionType.攻擊;
            saveTime = Time.time;



        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            enemyList.Remove(other.gameObject);

        }


    }

}
