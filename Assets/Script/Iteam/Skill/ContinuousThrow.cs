using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousThrow : IteamSkillBase
{
    public int ThrowMax;


    private void OnEnable()
    {
        SkillEffect();
    }
    public override void SkillEffect()
    {
        StartCoroutine(Effect());

    }

    IEnumerator Effect()
    {
        int i = ThrowMax;

        while (i>0)
        {
            yield return new WaitForSeconds(stetas.iteam.Speed / 30);
            
            var tmp = GamePlayManager.instance.iteamGround_Player.transform.Find(stetas.iteam.IteamName + "物件池");

            if (tmp != null)
            {
                if (tmp.transform.childCount > 0)
                {
                    var iteamObj = tmp.transform.GetChild(0).gameObject;
                    iteamObj.GetComponent<Stetas>().iteam = stetas.iteam;
                    iteamObj.transform.SetParent(GamePlayManager.instance.roads[stetas.roadNo-1].transform);

                    iteamObj.SetActive(true);
                }
            }
            i--;

        }
        this.enabled = false;
    }

}
