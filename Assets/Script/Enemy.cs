using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public Stetas stetas;

    bool startAtkFlag =false;
    Coroutine AtkCoroutine;
    BoxCollider coll;
    float speed = 0;

    public float Hp;
    float Hpmax;

    bool canWalk = false;

    private void OnEnable()
    {

        coll = GetComponent<BoxCollider>();
        var tmpPos = new Vector3(-MainManager.instance.Rect.rect.size.x / 2, 0, 0);
        gameObject.transform.localPosition = tmpPos;
        speed = MainManager.instance.Rect.rect.size.x / stetas.enemy.speed;

        var tmpHp = ((float)(20 * 1 + (1 - 1) * 0.1)) * (1 + (GamePlayManager.instance.Wave - 1)  *0.2)*(stetas.enemy.hp/100);
        var tmpHpFloat=((float)tmpHp);
        Hpmax = ((int)(Mathf.Round(tmpHpFloat)));

        Hp = Hpmax;
        canWalk = true;
    }


    private void Update()
    {
        if (Hp <= 0)
        {
            ReSet();
        }

        if (gameObject.transform.localPosition.x < MainManager.instance.Rect.rect.size.x / 2-150)
        {
            // 移動
            if(startAtkFlag)
            {
                StopCoroutine(AtkCoroutine);
            }
            if(canWalk)
            {
                var pos = gameObject.transform.localPosition;
                pos.x += (speed * Time.fixedDeltaTime);
                gameObject.transform.localPosition = pos;
            }

        }
        else
        {
            
            if(!startAtkFlag)
            {
                AtkCoroutine=StartCoroutine(Attack());
                startAtkFlag = true;
            }

        }
    }
    
    void ReSet()
    {
        gameObject.SetActive(false);
        var tmp = GamePlayManager.instance.iteamGround_Enemy.transform.Find(stetas.enemy.name + "物件池");

        gameObject.transform.SetParent(tmp.transform);
        stetas.hpBar.enabled = false;
        stetas.hpBar.fillAmount = 1;
        canWalk = true;

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

    int WeaponAtkChange(Stetas otherstetas)
    {
        return ((int)((GamePlayManager.instance.player.tmpPlayerData.Atk * (1 + 0) * (1 + 0)) * (otherstetas.iteam.Atk/100 * (1 + 0))));
    }
    private void OnTriggerEnter(Collider other)
    {
        
        var otherstetas = other.gameObject.GetComponent<Stetas>();
        switch (otherstetas.type)
        {
            case Stetas.Type.道具:
                if(otherstetas.iteam.type==SO_Iteam.IteamType.召喚)
                {
                    canWalk = false;
                }

                Debug.Log(WeaponAtkChange(otherstetas));
                Hp -= WeaponAtkChange(otherstetas);
  

                break;
            case Stetas.Type.敵人:
                break;
        }

        if(stetas.hpBar!=null&&Hp!=Hpmax)
        {
            stetas.hpBar.gameObject.SetActive(true);
            float tmp = Hp / Hpmax;

            stetas.hpBar.fillAmount = tmp;

        }

    }


    private void OnTriggerStay(Collider other)
    {
        var otherstetas = other.gameObject.GetComponent<Stetas>();

        if (other==null|| otherstetas.iteam==null)
        {
            canWalk = true;

            return;
        }    


    }



}
