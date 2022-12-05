using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : BossInit
{
    private void Start()
    {
        FuncInit();
        GameObject tmp = new GameObject(bullot1.IteamName + "物件池");

        tmp.transform.SetParent(GamePlayManager.instance.iteamGround_Boss.transform);


        for (int i = 0; i < 30; i++)
        {
            Instantiate(bullot1.IteamPerfab, tmp.transform);

        }
        StageFunc[Stage.CD]();

    }

    public SO_Iteam bullot1;
    public void FuncInit()
    {
        StageFunc.Add(Stage.隨機路線, () => {

            RandomRoad(this.gameObject);

            var tmpPos = new Vector3(-MainManager.instance.Rect.rect.size.x / 2+200, 0, 0);

            gameObject.transform.localPosition = tmpPos;

            StageFunc[Stage.攻擊]();
        });

        StageFunc.Add(Stage.攻擊, () => {

            StartCoroutine(IEAttack());
        });
        
        StageFunc.Add(Stage.CD, () => {

            StartCoroutine(IEWaitTime(2f));
        });

    }

    IEnumerator IEAttack()
    {
        yield return StartCoroutine(IEAttackType1(0.6f,5,2,2, bullot1));
        yield return StartCoroutine(IEAttackType3(0.7f,7,2.1f,1, bullot1,1));
        yield return StartCoroutine(IEAttackType3(0.7f,7,2.1f,1, bullot1,2));
        yield return StartCoroutine(IEAttackType3(0.7f,7,2.1f,1, bullot1,3));
        StageFunc[Stage.CD]();

    }

    IEnumerator IEWaitTime(float WaitTime)
    {
        yield return new WaitForSeconds(WaitTime);
        StageFunc[Stage.隨機路線]();

    }


}
