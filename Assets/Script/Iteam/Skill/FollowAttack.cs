using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowAttack : IteamSkillBase
{
    public GameObject targetobj;
    public override void SkillEffect(GameObject obj)
    {
        targetobj = obj;
        StartCoroutine(Effect(obj));
        StartCoroutine(Target(obj));
        this.gameObject.GetComponent<BoxCollider>().enabled = false;
        stetas.actionType = Stetas.ActionType.技能時;
    }

    IEnumerator Effect(GameObject obj)
    {
        var tmp = obj.GetComponent<Stetas>();

        while (tmp.Hp>0)
        {

            tmp.TakeDamage(stetas.WeaponAtkChange(stetas.iteam));

            yield return new WaitForSeconds(1f);

        }
        stetas.actionType = Stetas.ActionType.移動;
        this.gameObject.GetComponent<BoxCollider>().enabled = true;


    }

    IEnumerator Target(GameObject obj)
    {
        var tmp = obj.GetComponent<Stetas>();
        while (tmp.Hp > 0)
        {
            this.gameObject.transform.localPosition=obj.transform.localPosition;
            yield return new WaitForSeconds(0.02f);

        }

        stetas.actionType = Stetas.ActionType.移動;
        this.gameObject.GetComponent<BoxCollider>().enabled = true;
    }
}
