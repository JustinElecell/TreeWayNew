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
        var tmpHp = ((float)(20 * 1 + (1 - 1) * 0.1)) * (1 + (GamePlayManager.instance.Wave - 1) * 0.2) * (stetas.enemy.hp / 100);
        var tmpHpFloat = ((float)tmpHp);
        stetas.HpMax = ((int)(Mathf.Round(tmpHpFloat)));
        stetas.Hp = stetas.HpMax;
    }



    public SO_Iteam[] bullot;

    private void Update()
    {

        if (stetas.Hp <= 0)
        {
            GamePlayManager.instance.Pause();
            //StopAllCoroutines();

            GamePlayManager.instance.GameOver.SetActive(true);
        }
    }
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
