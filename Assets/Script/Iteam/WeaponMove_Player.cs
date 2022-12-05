using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMove_Player : MonoBehaviour
{
    
    public Stetas stetas;

    float speed = 0;
    BoxCollider coll;

    private void OnEnable()
    {

        coll = GetComponent<BoxCollider>();


            var tmpPos = new Vector3(MainManager.instance.Rect.rect.size.x / 2, 0, 0);
            gameObject.transform.localPosition = tmpPos;
            speed = MainManager.instance.Rect.rect.size.x / stetas.iteam.Speed;
        


        stetas.HpMax = ((int)(stetas.iteam.Hp));

        stetas.Hp = stetas.HpMax;
    }


    private void Update()
    {
        //gameObject.transform.localPosition = Vector3.zero;
        if (stetas.Hp <= 0)
        {
            ReSet();
        }


        if (gameObject.transform.localPosition.x > -MainManager.instance.Rect.rect.size.x / 2)
        {
            //移動

            var pos = gameObject.transform.localPosition;
            pos.x -= (speed * Time.fixedDeltaTime);
            gameObject.transform.localPosition = pos;
        }
        else
        {
            ReSet();

        }
        



    }

    void ReSet()
    {
        gameObject.SetActive(false);
        var tmp = GamePlayManager.instance.iteamGround_Player.transform.Find(stetas.iteam.IteamName + "物件池");

        gameObject.transform.SetParent(tmp.transform);
    }
    private void OnTriggerEnter(Collider other)
    {

        var otherstetas = other.gameObject.GetComponent<Stetas>();
        switch(otherstetas.type)
        {
            case Stetas.Type.道具:


                break;
            case Stetas.Type.敵人:

                otherstetas.TakeDamage(stetas.WeaponAtkChange(stetas.iteam.Atk));
                stetas.Hp -= otherstetas.enemy.Atk;

                

                break;
        }

        
    }

}
