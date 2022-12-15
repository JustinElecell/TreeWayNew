using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeLinesofAttack : IteamSkillBase
{


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

        for(int i=0;i<3;i++)
        {
            if(i!= stetas.roadNo - 1)
            {
                var tmp = GamePlayManager.instance.iteamGround_Player.transform.Find(stetas.iteam.IteamName + "物件池");

                if (tmp != null)
                {
                    if (tmp.transform.childCount > 0)
                    {
                        var iteamObj = tmp.transform.GetChild(0).gameObject;
                        iteamObj.GetComponent<Stetas>().iteam = stetas.iteam;
                        iteamObj.transform.SetParent(GamePlayManager.instance.roads[i].transform);

                        iteamObj.SetActive(true);
                    }
                }
            }
        }
        this.enabled = false;
        yield return null;


    }


}
