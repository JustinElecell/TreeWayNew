using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraRandomAttack : IteamSkillBase
{
    private void OnEnable()
    {
        
    }
    public override void SkillEffect(GameObject obj)
    {
        StartCoroutine(Effect(obj));
    }

    IEnumerator Effect(GameObject obj)
    {
        

        var tmp = GamePlayManager.instance.iteamGround_Player.transform.Find(stetas.iteam.IteamName + "物件池");

        if (tmp != null)
        {
            if (tmp.transform.childCount > 0)
            {
                var iteamObj = tmp.transform.GetChild(0).gameObject;
                iteamObj.GetComponent<Stetas>().iteam = stetas.iteam;
                iteamObj.transform.SetParent(GamePlayManager.instance.roads[UnityEngine.Random.Range(0,3)].transform);

                iteamObj.SetActive(true);
            }
        }
        
        
        this.enabled = false;
        yield return null;



    }

}
