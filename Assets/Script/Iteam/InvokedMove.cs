using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvokedMove : MonoBehaviour
{
    public Stetas stetas;

    float speed = 0;
    BoxCollider coll;
    int Hp;
    int Hpmax;
    bool canWalk = false;

    Coroutine IEAttatk;

    private void OnEnable()
    {

        coll = GetComponent<BoxCollider>();


        var tmpPos = new Vector3(MainManager.instance.Rect.rect.size.x / 2, 0, 0);
        gameObject.transform.localPosition = tmpPos;
        speed = MainManager.instance.Rect.rect.size.x / stetas.iteam.Speed;


        Hpmax = ((int)(stetas.iteam.Hp));

        Hp = Hpmax;
        canWalk = true;
    }


    private void Update()
    {
        if (Hp <= 0)
        {
            ReSet();
        }

        if (gameObject.transform.localPosition.x > -MainManager.instance.Rect.rect.size.x / 2)
        {
            //移動
            if(canWalk)
            {
                var pos = gameObject.transform.localPosition;
                pos.x -= (speed * Time.deltaTime);
                gameObject.transform.localPosition = pos;
            }

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
        canWalk = true;
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
        var otherstetas = other.gameObject.GetComponent<Stetas>();
        switch (otherstetas.type)
        {
            case Stetas.Type.道具:
                break;
            case Stetas.Type.敵人:
                canWalk = false;
                IEAttatk = StartCoroutine(Attack(other));
                //Hp -= otherstetas.enemy.atk;
                break;
        }


    }

    private void OnTriggerStay(Collider other)
    {
        var otherstetas = other.gameObject.GetComponent<Stetas>();
        if (other == null || otherstetas.enemy == null)
        {
            canWalk = true;
            return;
        }

    }

}
