using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : BossInit
{
    private void Awake()
    {
        FuncInit();
        for (int r = 0; r < bullot.Length; r++)
        {
            GameObject tmp = new GameObject(bullot[r].IteamName + "物件池");

            tmp.transform.SetParent(GamePlayManager.instance.iteamGround_Enemy.transform);


            for (int i = 0; i < 30; i++)
            {
                Instantiate(bullot[r].IteamPerfab, tmp.transform);

            }
        }
    }

    private void OnEnable()
    {
        StageFunc[Stage.隨機路線]();
    }



    public SO_Iteam[] bullot;
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
        yield return StartCoroutine(IEAttackType1(0.6f, 5, 2, 2, bullot[0]));
        yield return StartCoroutine(IEAttackType3(0.7f, 7, 2.1f, 1, bullot[0], 1));
        yield return StartCoroutine(IEAttackType3(0.7f, 7, 2.1f, 1, bullot[0], 2));
        yield return StartCoroutine(IEAttackType3(0.7f, 7, 2.1f, 1, bullot[0], 3));
        yield return StartCoroutine(IEAttackType4(0.8f, 2, 5.73f, 3, bullot[0], bullot[1]));
        //yield return new WaitForSeconds(1);
        StageFunc[Stage.CD]();

    }

    IEnumerator IEWaitTime(float WaitTime)
    {
        yield return new WaitForSeconds(WaitTime);
        StageFunc[Stage.隨機路線]();

    }


}
