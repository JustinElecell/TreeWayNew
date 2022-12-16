using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathGeneration : IteamSkillBase
{
    public SO_Iteam SO_Generation;
    public int countMax;

    
    private void Awake()
    {
        Debug.Log("生成");
        if(SO_Generation.IteamName!=stetas.iteam.IteamName)
        {
            for(int i=0;i<countMax;i++)
            {
                GamePlayManager.instance.CreateIteamInit(SO_Generation, GamePlayManager.instance.iteamGround_Player.transform);

            }
        }

    }
    private void OnDisable()
    {
        Debug.Log("死亡");
        for(int i=0;i<countMax;i++)
        {
            var tmp=GamePlayManager.instance.SetIteam(SO_Generation, stetas.roadNo,false);

            Vector3 tmpVec = tmp.transform.localPosition;
            tmpVec.x = this.gameObject.transform.localPosition.x;

            tmp.transform.localPosition = tmpVec;

        }
    }

}
