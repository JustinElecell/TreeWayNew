using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMove_Player : MonoBehaviour
{
    
    public Stetas stetas;

    float speed = 0;
    BoxCollider coll;

    public List<GameObject> targetList = new List<GameObject>();
    public int roadNo;

    private void OnEnable()
    {

        coll = GetComponent<BoxCollider>();


            var tmpPos = new Vector3(GamePlayManager.instance.Rect.rect.size.x / 2, 0, 0);
            gameObject.transform.localPosition = tmpPos;
            speed = GamePlayManager.instance.Rect.rect.size.x / stetas.iteam.Speed;

        roadNo = gameObject.transform.parent.transform.GetSiblingIndex() + 1;


        stetas.HpMax = ((int)(stetas.iteam.Hp));

        stetas.Hp = stetas.HpMax;
        this.gameObject.GetComponent<SpriteRenderer>().enabled = true;
        this.gameObject.GetComponent<BoxCollider>().enabled = true;
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
        



    }

    void ReSet()
    {
        if(stetas.Skill.enabled==true)
        {
            this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            this.gameObject.GetComponent<BoxCollider>().enabled = false;
        }
        else
        {
            gameObject.SetActive(false);
            var tmp = GamePlayManager.instance.iteamGround_Player.transform.Find(stetas.iteam.IteamName + "物件池");

            gameObject.transform.SetParent(tmp.transform);
            targetList.Clear();
            stetas.Skill.enabled = false;
        }

    }
    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Enemy")
        {
            var otherstetas = other.gameObject.GetComponent<Stetas>();
            targetList.Add(other.gameObject);



        }

        for (int i = targetList.Count-1; i >= 0; i--)
        {
            if(targetList.Count> 0)
            {
                var otherstetas = targetList[i].GetComponent<Stetas>();
                switch (otherstetas.type)
                {
                    case Stetas.Type.道具:
                        otherstetas.TakeDamage(stetas.WeaponAtkChange(stetas.iteam));
                        stetas.Hp -= otherstetas.iteam.Atk;

                        break;
                    case Stetas.Type.敵人:
                        otherstetas.TakeDamage(stetas.WeaponAtkChange(stetas.iteam));
                        stetas.Hp -= otherstetas.enemy.Atk;
                        
                        if(stetas.Skill!=null&&stetas.Skill.enabled==true)
                        {
                            stetas.Skill.SkillEffect(other.gameObject);
                        }

                        break;
                    case Stetas.Type.召喚:
                        otherstetas.TakeDamage(stetas.WeaponAtkChange(stetas.iteam));
                        stetas.Hp -= otherstetas.iteam.Atk;
                        break;
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
