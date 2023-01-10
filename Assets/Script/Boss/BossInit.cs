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

    // 子彈時間間隔, 單組發射子彈次數, 重複時間間隔(調整比), 攻擊重複次數, 子彈編號
    public IEnumerator IEAttackType1(float bullotWait, int bullotCnt,float groundWait,int count, SO_Iteam iteam)
    {
        //向自身所在路線發射主子彈
        for (int r=0;r<count;r++)
        {
            for(int i=0;i<bullotCnt;i++)
            {
                // 發射子彈
                BullotFire(iteam);
                if (i < bullotCnt - 1)
                {
                    yield return new WaitForSeconds(bullotWait);

                }
            }
            yield return new WaitForSeconds(groundWait);
        }
    }

    // 子彈時間間隔, 單組發射子彈次數, 重複時間間隔(調整比), 攻擊重複次數, 子彈編號, 方式參數
    public IEnumerator IEAttackType2(float bullotWait, int bullotCnt, float groundWait, int count, SO_Iteam iteam, int parameter)
    {
        //向自身所在路線發射主子彈
        for (int r = 0; r < count; r++)
        {
            for (int i = 0; i < bullotCnt; i++)
            {
                // 發射子彈
                if(parameter <= 3)
                {
                    BullotFire(iteam,count);

                }
                else
                {
                    switch(parameter)
                    {
                        case 4:
                            BullotFire(iteam, 1);
                            BullotFire(iteam, 2);
                            break;
                        case 5:
                            BullotFire(iteam, 1);
                            BullotFire(iteam, 3);
                            break;
                        case 6:
                            BullotFire(iteam, 2);
                            BullotFire(iteam, 3);
                            break;
                        case 7:
                            BullotFire(iteam, 1);
                            BullotFire(iteam, 2);
                            BullotFire(iteam, 3);

                            break;
                    }
                }
                if (i < bullotCnt - 1)
                {
                    yield return new WaitForSeconds(bullotWait);

                }
            }
            yield return new WaitForSeconds(groundWait);
        }
    }


    // 子彈時間間隔, 單組發射子彈次數, 重複時間間隔(調整比), 攻擊重複次數, 子彈編號, 方式參數
    public IEnumerator IEAttackType3(float bullotWait, int bullotCnt, float groundWait, int count, SO_Iteam iteam,int parameter)
    {
        //向隨機 x 條路線同時發射一次主子彈
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
                if (i < bullotCnt - 1)
                {
                    yield return new WaitForSeconds(bullotWait);

                }
            }
            yield return new WaitForSeconds(groundWait);
        }
    }

    // 子彈時間間隔, 單組發射子彈次數, 重複時間間隔(調整比), 攻擊重複次數, 主子彈編號, 指定子彈編號
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


                if (i < bullotCnt - 1)
                {
                    yield return new WaitForSeconds(bullotWait);

                }
            }
            yield return new WaitForSeconds(groundWait);
        }
    }

    // 子彈時間間隔, 單組發射子彈次數, 重複時間間隔(調整比), 攻擊重複次數, 主子彈編號, 指定子彈編號
    public IEnumerator IEAttackType5(float bullotWait, int bullotCnt, float groundWait, int count, SO_Iteam iteam)
    {
        //向自身所在路線發射主子彈，並於經過指定秒數後，向另外兩條路線同時發射主子彈 (此計為一批攻擊)
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

                yield return new WaitForSeconds(bullotWait);

                for (int z = 0; z < list.Count; z++)
                {
                    BullotFire(iteam, list[z]);

                }


                if (i < bullotCnt - 1)
                {
                    yield return new WaitForSeconds(bullotWait);

                }
            }
            yield return new WaitForSeconds(groundWait);
        }
    }


    // 子彈時間間隔, 單組發射子彈次數, 重複時間間隔(調整比), 攻擊重複次數, 主子彈編號, 方式參數
    public IEnumerator IEAttackType6(float bullotWait, int bullotCnt, float groundWait, int count, SO_Iteam iteam, int parameter)
    {
        //向自身所在路線發射主子彈，並於最後x次向另外兩條路線同時發射主子彈 ( 此計為一組攻擊 )
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
                
                if (parameter >= count - r)
                {
                    for (int z = 0; z < list.Count; z++)
                    {
                        BullotFire(iteam, list[z]);

                    }
                }

                if (i < bullotCnt - 1)
                {
                    yield return new WaitForSeconds(bullotWait);

                }
            }
            yield return new WaitForSeconds(groundWait);
        }
    }


    // 子彈時間間隔, 單組發射子彈次數, 重複時間間隔(調整比), 攻擊重複次數, 主子彈編號, 方式參數
    public IEnumerator IEAttackType7(float bullotWait, int bullotCnt, float groundWait, int count, SO_Iteam iteam, int parameter)
    {
        List<int> list = new List<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);

        //首次向自身所在路線發射主子彈，下一次子彈發射於下一條編號的路線，重複直至一組攻擊的次數用盡
        for (int r = 0; r < count; r++)
        {
            for (int i = 0; i < bullotCnt; i++)
            {
                int no = this.gameObject.transform.parent.transform.GetSiblingIndex();

                if(i>0)
                {
                    if(parameter==1)
                    {
                        no += i;
                    }
                    else
                    {
                        no -= i;
                    }

                    no = Math.Abs(no);
                }

                BullotFire(iteam, list[no-1]);
                if(i<bullotCnt-1)
                {
                    yield return new WaitForSeconds(bullotWait);

                }
            }
            yield return new WaitForSeconds(groundWait);
        }
    }

    // 子彈時間間隔, 單組發射子彈次數, 重複時間間隔(調整比), 攻擊重複次數, 主子彈編號, 方式參數
    public IEnumerator IEAttackType8(float bullotWait, int bullotCnt, float groundWait, int count, SO_Iteam iteam, int parameter)
    {
        List<int> list = new List<int>();
        list.Add(1);
        list.Add(2);
        list.Add(3);

        //首次向自身所在路線發射主子彈，下一次子彈發射於下一條編號的路線，若於1或3號路線時，則折返
        for (int r = 0; r < count; r++)
        {
            for (int i = 0; i < bullotCnt; i++)
            {
                int no = this.gameObject.transform.parent.transform.GetSiblingIndex();

                BullotFire(iteam, no);

                if (i < bullotCnt - 1)
                {
                    yield return new WaitForSeconds(bullotWait);

                }
            }
            yield return new WaitForSeconds(groundWait);
        }
    }

    // 子彈時間間隔, 單組發射子彈次數, 重複時間間隔(調整比), 攻擊重複次數, 主子彈編號, 方式參數
    public IEnumerator IEAttackType9(float bullotWait, int bullotCnt, float groundWait, int count, SO_Iteam iteam, int parameter)
    {


        //將總數量為「攻擊方式參數」的子彈，隨機分配給「單組子彈發射次數」組射擊，每個分配的數量為0~3，射擊方式同"3"的隨機路線選取
        for (int r = 0; r < count; r++)
        {
            for (int i = 0; i < bullotCnt; i++)
            {
                
                Dictionary<int, int> keyValues = new Dictionary<int, int>();
                for(int t=1;t< bullotCnt;t++)
                {
                    keyValues.Add(t, 0);
                }
                int cnt = 0;
                while(cnt < parameter)
                {
                    if(keyValues[UnityEngine.Random.Range(1, keyValues.Count)]< 3)
                    {
                        keyValues[UnityEngine.Random.Range(1, keyValues.Count)]++;
                    }
                }

                for(int q=0;q<= keyValues.Count;q++)
                {
                    List<int> list = new List<int>();
                    list.Add(1);
                    list.Add(2);
                    list.Add(3);
                    for (int j=0;j< keyValues[q + 1];j++)
                    {
                        var tmp = list[UnityEngine.Random.Range(0, list.Count)];

                        BullotFire(iteam, tmp);

                    }
                    if (q < bullotCnt - 1)
                    {
                        yield return new WaitForSeconds(bullotWait);

                    }
                }
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
