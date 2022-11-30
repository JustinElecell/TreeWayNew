using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMove : MonoBehaviour
{
    public SO_Iteam data;
    float speed = 0;
    BoxCollider coll;

    private void OnEnable()
    {
        coll = GetComponent<BoxCollider>();


        var tmpPos = new Vector3(MainManager.instance.Rect.rect.size.x / 2, 0, 0);
        gameObject.transform.localPosition = tmpPos;
        speed = MainManager.instance.Rect.rect.size.x / data.Speed;
    }


    private void FixedUpdate()
    {
        //gameObject.transform.localPosition = Vector3.zero;

        if (gameObject.transform.localPosition.x > -MainManager.instance.Rect.rect.size.x / 2)
        {
            //移動

            var pos = gameObject.transform.localPosition;
            pos.x -= (speed * Time.fixedDeltaTime);
            gameObject.transform.localPosition = pos;
        }
        else
        {
            gameObject.SetActive(false);
            var tmp = GamePlayManager.instance.iteamGround.transform.Find(data.IteamName + "物件池");

            gameObject.transform.SetParent(tmp.transform);

        }

    }

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("TEST" + other.gameObject.name);

        
    }

}
