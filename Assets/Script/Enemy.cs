using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public SO_Enemy so_data;


    bool startAtkFlag=false;
    Coroutine AtkCoroutine;
    BoxCollider coll;
    float speed = 0;

    int Hp;
    int Hpmax;

    private void OnEnable()
    {
        coll = GetComponent<BoxCollider>();
        var tmpPos = new Vector3(-MainManager.instance.Rect.rect.size.x / 2, 0, 0);
        gameObject.transform.localPosition = tmpPos;
        speed = MainManager.instance.Rect.rect.size.x /so_data.speed;

        var tmpHp = ((float)(20 * 1 + (1 - 1) * 0.1)) * (1 + (GamePlayManager.instance.Wave - 1)  *0.2)*(so_data.hp/100);
        var tmpHpFloat=((float)tmpHp);
        Hpmax = ((int)(Mathf.Round(tmpHpFloat)));

        Hp = Hpmax;
    }


    private void FixedUpdate()
    {
        if (gameObject.transform.localPosition.x < MainManager.instance.Rect.rect.size.x / 2-150)
        {
            // 移動
            if(startAtkFlag)
            {
                StopCoroutine(AtkCoroutine);
            }
            var pos = gameObject.transform.localPosition;
            pos.x += (speed * Time.fixedDeltaTime);
            gameObject.transform.localPosition = pos;
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

    IEnumerator Attack()
    {

        while(GamePlayManager.instance.FindPlayer())
        {            
            // 攻擊

            GamePlayManager.instance.EAttack(so_data.atk);

            yield return new WaitForSeconds(so_data.atk_Interval);
        }
    }
    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("TEST"+other.gameObject.name);


    }

}
