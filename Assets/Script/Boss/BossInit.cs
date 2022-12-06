using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BossInit : MonoBehaviour
{
    public Stetas stetas;

    public Dictionary<Stage, Action> StageFunc = new Dictionary<Stage, Action>();

    int repeatTimes;

    //public Stage stage;

    public enum Stage
    {
        隨機路線,
        攻擊,
        CD
    }


    public void RandomRoad(GameObject obj)
    {
        Debug.Log(GamePlayManager.instance.roads[UnityEngine.Random.Range(0, 3)].gameObject+"隨機路徑");
        obj.transform.SetParent(GamePlayManager.instance.roads[UnityEngine.Random.Range(0, 3)].gameObject.transform);
    }

    public IEnumerator IEAttackType1(float bullotWait, int bullotCnt,float groundWait,int count, SO_Iteam iteam)
    {
        //向自身所在路線發射主子彈
        for (int r=0;r<count;r++)
        {
            for(int i=0;i<bullotCnt;i++)
            {
                // 發射子彈
                BullotFire(iteam);
                yield return new WaitForSeconds(bullotWait);
            }
            yield return new WaitForSeconds(groundWait);
        }
    }


    public IEnumerator IEAttackType3(float bullotWait, int bullotCnt, float groundWait, int count, SO_Iteam iteam,int parameter)
    {
        //向指定路線發射指定編號子彈
        for (int r = 0; r < count; r++)
        {
            for (int i = 0; i < bullotCnt; i++)
            {
                List<int> list = new List<int>();
                int cnt = 3;
                list.Add(1);
                list.Add(2);
                list.Add(3);
                for (int z=0;z<parameter;z++)
                {
                    int no = UnityEngine.Random.Range(0, cnt);

                    // 發射子彈
                    BullotFire(iteam, list[no]);
                    list.RemoveAt(no);
                    cnt--;
                }
                yield return new WaitForSeconds(bullotWait);
            }
            yield return new WaitForSeconds(groundWait);
        }
    }
    public IEnumerator IEAttackType4(float bullotWait, int bullotCnt, float groundWait, int count, SO_Iteam iteam, SO_Iteam iteam2)
    {
        //向自身所在路線發射主子彈，並向另外兩條路線同時發射指定編號子彈
        for (int r = 0; r < count; r++)
        {
            for (int i = 0; i < bullotCnt; i++)
            {
                BullotFire(iteam);
                
                List<int> list = new List<int>();
                int no = this.gameObject.transform.parent.transform.GetSiblingIndex();
                list.Add(1);
                list.Add(2);
                list.Add(3);
                list.RemoveAt(no);

                for(int z=0;z<list.Count;z++)
                {
                    BullotFire(iteam2,list[z]);

                }


                yield return new WaitForSeconds(bullotWait);
            }
            yield return new WaitForSeconds(groundWait);
        }
    }

    float TimeChange(float timeRate)
    {
        return ((float)((5 * 1 + (GamePlayManager.instance.Wave - 1) * 0.2) * timeRate));
    }

    void BullotFire(SO_Iteam iteam)
    {
        var tmp = GamePlayManager.instance.iteamGround_Enemy.transform.Find(iteam.IteamName + "物件池");
        if (tmp != null)
        {
            var iteamObj = tmp.transform.GetChild(0).gameObject;

            iteamObj.transform.SetParent(this.gameObject.transform.parent.transform);



            iteamObj.SetActive(true);

            var tmpPos = new Vector3(this.gameObject.transform.localPosition.x, 0, 0);
            iteamObj.transform.localPosition = tmpPos;

        }

    }
    void BullotFire(SO_Iteam iteam,int no)
    {
        var tmp = GamePlayManager.instance.iteamGround_Enemy.transform.Find(iteam.IteamName + "物件池");
        if (tmp != null)
        {
            var iteamObj = tmp.transform.GetChild(0).gameObject;

            iteamObj.transform.SetParent(GamePlayManager.instance.roads[no-1].gameObject.transform);



            iteamObj.SetActive(true);

            var tmpPos = new Vector3(this.gameObject.transform.localPosition.x, 0, 0);
            iteamObj.transform.localPosition = tmpPos;

        }

    }


}
