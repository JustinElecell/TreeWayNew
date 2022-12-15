using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repel : IteamSkillBase
{

    public float Max;
    public override void SkillEffect(GameObject obj)
    {
        StartCoroutine(Effect(obj));
    }
    List<GameObject> objList = new List<GameObject>();
    


    IEnumerator Effect(GameObject obj)
    {
        var speed = GamePlayManager.instance.Rect.rect.size.x*(Max/100);

        foreach(var data in objList)
        {
            if(data.gameObject==obj)
            {
                goto Test;
            }
        }



        obj.GetComponent<Stetas>().CantMoveCount = 0.05f;
        obj.GetComponent<Stetas>().saveTime = Time.time;
        obj.GetComponent<Stetas>().actionType = Stetas.ActionType.不能動作;
        objList.Add(obj);
        for (int i = 0; i < 5; i++)
        {
            var pos = obj.transform.localPosition;

            pos.x -= ((float)speed / 5);
            obj.transform.localPosition = pos;

            yield return new WaitForSeconds(0.01f);
        }

    Test: Debug.Log("跳過");



    }


}
