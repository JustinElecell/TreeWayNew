using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class WeaponMove_Player : MonoBehaviour
{
    
    public Stetas stetas;

    float speed = 0;
    BoxCollider coll;

    public List<GameObject> targetList = new List<GameObject>();
    public int roadNo;
    public Dictionary<Stetas.ActionType, Action> ActionTypeFunc = new Dictionary<Stetas.ActionType, Action>();

    private void Start()
    {
        FuncInit();
    }
    void FuncInit()
    {
        ActionTypeFunc.Add(Stetas.ActionType.移動, () => {
            if (gameObject.transform.localPosition.x > -GamePlayManager.instance.Rect.rect.size.x / 2)
            {


                //移動

                var pos = gameObject.transform.localPosition;
                pos.x -= (speed * Time.deltaTime);
                gameObject.transform.localPosition = pos;
            }
            else
            {
                ReSet();

            }
        });


        ActionTypeFunc.Add(Stetas.ActionType.技能時, () => {


           
        });

    }
    private void OnEnable()
    {

        coll = GetComponent<BoxCollider>();


        var tmpPos = new Vector3(GamePlayManager.instance.Rect.rect.size.x / 2, 0 + UnityEngine.Random.Range(-20, 20), 0);
            gameObject.transform.localPosition = tmpPos;
            speed = GamePlayManager.instance.Rect.rect.size.x / stetas.iteam.Speed;

        roadNo = gameObject.transform.parent.transform.GetSiblingIndex() + 1;


        stetas.HpMax = ((int)(stetas.iteam.Hp));

        stetas.Hp = stetas.HpMax;
        this.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        this.gameObject.GetComponent<BoxCollider>().enabled = true;
        stetas.actionType = Stetas.ActionType.移動;
        targetList.Clear();

    }

    private void OnDisable()
    {
        targetList.Clear();
    }

    private void Update()
    {
        //gameObject.transform.localPosition = Vector3.zero;
        if (stetas.Hp <= 0)
        {
            ReSet();
        }

        ActionTypeFunc[stetas.actionType]();

        



    }

    void ReSet()
    {

        if(stetas.Skill!=null&&stetas.Skill.enabled==true&&stetas.Skill.needWait)
        {
            this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            this.gameObject.GetComponent<BoxCollider>().enabled = false;
        }
        else
        {
            gameObject.SetActive(false);
            var tmp = GamePlayManager.instance.iteamGround_Player.transform.Find(stetas.iteam.IteamName + "物件池");

            gameObject.transform.SetParent(tmp.transform);
            stetas.BuffAtkUp = 0;
            stetas.iteam.temporaryAtkUp = 0;
            stetas.damageDown = 0;
            if (stetas.Skill != null)
            {
                stetas.Skill.enabled = false;

            }

        }

    }
    private void OnTriggerEnter(Collider other)
    {


        if (other.gameObject.tag == "Enemy")
        {
            var otherstetas = other.gameObject.GetComponent<Stetas>();
            targetList.Add(other.gameObject);

        }

        if (stetas.actionType == Stetas.ActionType.技能時)
        {
            return;
        }

        for (int i = targetList.Count-1; i >= 0; i--)
        {
            if(targetList.Count> 0)
            {
                var otherstetas = targetList[i].GetComponent<Stetas>();
                switch (otherstetas.type)
                {
                    case Stetas.Type.道具:
                        otherstetas.TakeDamage(stetas.WeaponAtkChange(stetas.iteam),otherstetas.damageDown);
                        stetas.Hp -= otherstetas.iteam.Atk;

                        break;
                    case Stetas.Type.敵人:
                        otherstetas.TakeDamage(stetas.WeaponAtkChange(stetas.iteam),otherstetas.damageDown);
                        stetas.Hp -= otherstetas.enemy.Atk;
                        
                        if(stetas.Skill!=null&&stetas.Skill.enabled==true)
                        {
                            Debug.Log(other.gameObject);

                            stetas.Skill.SkillEffect(other.gameObject);

                            
                        }

                        break;
                    case Stetas.Type.召喚:
                        otherstetas.TakeDamage(stetas.WeaponAtkChange(stetas.iteam),otherstetas.damageDown);
                        stetas.Hp -= otherstetas.iteam.Atk;
                        break;
                }
                
                if(otherstetas.Hp<=0)
                {
                    targetList.RemoveAt(i);
                }

                if (stetas.Hp <= 0)
                {
                    ReSet();
                }
            }
            

        }




    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            targetList.Remove(other.gameObject);

        }
    }

}
