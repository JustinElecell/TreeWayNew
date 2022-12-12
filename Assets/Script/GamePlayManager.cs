using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public class GamePlayManager : MonoBehaviour
{
    public static GamePlayManager instance;
    public RectTransform Rect;

    public GameObject[] roads;
    public SO_Enemy[] enemyData;
    public GameObject bossPerfab;

    public int Wave;


    public InfoPanel infoPanel;
    public SkillPanel skillPanel;

    int timecount_sec;
    int timecount_min;
    Coroutine timeCount_Coroutine;

    public Player player;

    Dictionary<int, Func<float, float>> createEnemyTimeFunc = new Dictionary<int, Func<float, float>>();
    //public event Action atk;
    public GameObject iteamGround_Player;
    public GameObject iteamGround_Enemy;
    public GameObject iteamGround_Boss;
    
    [Header("測試用")]
    public SO_Iteam Skill;
    public Color White;
    public Color Green;
    public GameObject GameOver;

    public List<List<string>> BuffList;
    

    public Button BuffButton;

    public IteamUI[] SkillButtonLists;

    [Header("Skill管理")]
    public SkillManager skillManager;



    #region 初始化
    private void OnEnable()
    {

        if(instance==null)
        {
            instance = this;
            FuncInit();

            EnemyGroundInit();

            White = Color.white;
            Green = Color.green;
            timeCount_Coroutine = StartCoroutine(TimeCount());
            if (FindPlayer())
            {
                player.Init();

            }
            Time.timeScale = 1;
            BuffInit();

        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void BuffInit()
    {

        BuffList = ReadCsv.MyReadCSV.Read("Csv/Buff");
        skillManager.SetAllSkill(BuffList);

        //for(int i=1;i<BuffList.Count;i++)
        //{
        //    Debug.Log(BuffList[i][0]);
        //}
        BuffButton.onClick.AddListener(() =>
        {
            if(MainManager.instance.TestFlag)
            {
                //for (int i = 0; i < 3; i++)
                //{
                //    skillPanel.SkillSet(skillManager.GetSkills());

                //}
                skillPanel.SkillSet(skillManager.GetSkillLists());

                player.mp -= 80;

                player.ResetPlayerMp();
                skillPanel.gameObject.SetActive(true);
                Time.timeScale = 0;
                return;
            }

            if(player.stetas.Hp<=0)
            {
                return;
            }

            if (player.mp >= 80)
            {

                //for(int i=0;i<3;i++)
                //{
                //    skillPanel.SkillSet(skillManager.GetSkill());

                //}
                player.mp -= 80;

                player.ResetPlayerMp();
                skillPanel.gameObject.SetActive(true);
                Time.timeScale = 0;
            }




        });
    }


    void LotteryBuff()
    {

    }



    void FuncInit()
    {
        createEnemyTimeFunc.Add(0, (x) => {
            return 1;
        });
        createEnemyTimeFunc.Add(1, (x) => {
            return ((float)(0.75 + 0.01 * x));
        });
        createEnemyTimeFunc.Add(2, (x) => {
            return ((float)(0.5 + 0.02 * x));
        });
        createEnemyTimeFunc.Add(3, (x) => {
            return ((float)(0.8 + ((x / 10) % 2) * 0.4));
        });
        createEnemyTimeFunc.Add(4, (x) => {
            return ((float)(0.8 + (((x + 10) / 10) % 2) * 0.4));
        });
        createEnemyTimeFunc.Add(5, (x) => {
            return ((float)(0.5 + (x / 10) % 2));
        });
    }

    void EnemyGroundInit()
    {
        for(int i=0;i<2;i++)
        {
            GameObject tmp = new GameObject(enemyData[i].name + "物件池");


            tmp.transform.SetParent(GamePlayManager.instance.iteamGround_Enemy.transform);

            for (int l = 0; l < 60; l++)
            {
                Instantiate(enemyData[i].enemyPerfab, tmp.transform);

            }
        }
        

    }



    public void StartGameButton_Test()
    {
        timeCount_Coroutine = StartCoroutine(TimeCount());
        if (FindPlayer())
        {
            player.Init();

        }
    }
    private void OnDisable()
    {
        StopCoroutine(timeCount_Coroutine);

        Wave = 1;
    }
    #endregion


    IEnumerator CreateEnemy(int time,int enemyNo)
    {
        float saveTime = Time.time;
        var enemyCountInit = ((int)( 1 + (Wave - 1) * 0.2));

        int tmpRange = UnityEngine.Random.Range(0, 6);

        //func 60S換一次
        if (timecount_sec%60==0)
        {
            tmpRange = UnityEngine.Random.Range(0, 6);
        }

        //  2 * 該怪物出現率調整比 ( 怪物列表設定 ) * 該怪物出現量調整比 ( 關卡比例設定，如A怪70%/B怪30% ) * ( 1 + ( 當前波次編號 - 1  ) * 0.2 )
        float tmpTime =((float)(2* enemyData[enemyNo].generationRate*0.5* enemyCountInit));

        tmpTime = 1 / tmpTime;

        while (Time.time-saveTime < 10)
        {

            //Instantiate(enemyData[enemyNo].enemyPerfab, roads[UnityEngine.Random.Range(0, 3)].transform);
            var tmp = iteamGround_Enemy.transform.Find(enemyData[enemyNo].name + "物件池");

            if (tmp != null)
            {
                if(tmp.transform.childCount>0)
                {
                    var enemyObj = tmp.transform.GetChild(0).gameObject;

                    enemyObj.transform.SetParent(roads[UnityEngine.Random.Range(0, 3)].transform);

                    enemyObj.SetActive(true);
                }


            }


            yield return new WaitForSeconds(tmpTime);
        }

    }


    public void EAttack(int atk)
    {

        player.stetas.Hp -= atk;
        player.ResetPlayerHp();

    }

    public void PAttackInit(SO_Iteam iteam)
    {

        if(player.mp>= iteam.Mp)
        {
            var tmp =iteamGround_Player.transform.Find(iteam.IteamName + "物件池");
            player.mp -= iteam.Mp;
            player.ResetPlayerMp();

            if (tmp!=null)
            {
                var iteamObj = tmp.transform.GetChild(0).gameObject;

                iteamObj.transform.SetParent(player.transform.parent.transform);

                iteamObj.SetActive(true);

            }    
            
        }
    }

    IEnumerator TimeCount()
    {
        timecount_sec = 0;
        timecount_min = 0;
        infoPanel.WaveText.text = "Wave " + Wave.ToString() + " / 3";
        infoPanel.TimeText.text = timecount_min.ToString() + " : " + timecount_sec.ToString("D2");

        // 生怪
        for (int i = 0; i < enemyData.Length; i++)
        {
            StartCoroutine(CreateEnemy(0, i));
        }

        while (gameObject.activeSelf)
        {
            yield return new WaitForSeconds(1);
            timecount_sec++;

            if (timecount_sec >= 60)
            {
                if(Wave<3)
                {
                    WaveUp();

                }
                else
                {
                    bossPerfab.SetActive(true);


                    infoPanel.WaveText.gameObject.SetActive(false);
                    infoPanel.BossHpImage.gameObject.SetActive(true);
                }
                timecount_sec = 0;
                timecount_min++;
            }

            if (timecount_sec%10==0)
            {
                Debug.Log(timecount_sec);
                // 生怪
                for (int i = 0; i < enemyData.Length; i++)
                {
                    StartCoroutine(CreateEnemy(0, i));

                }
            }



            infoPanel.TimeText.text = timecount_min.ToString() + " : " + timecount_sec.ToString("D2");

        }
    }


    void WaveUp()
    {
        Wave++;
        infoPanel.WaveText.text = "Wave " + Wave.ToString() + " / 3";
    }




    public bool FindPlayer()
    {
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.Log("還是找不到");
            return false;

        }
        return true;
    }
    public void SetPlayer(int no)
    {
        if (player == null)
        {
            FindPlayer();
        }

        player.gameObject.transform.SetParent(roads[no].transform);

        var tmpPos = player.gameObject.transform.localPosition;
        tmpPos.y = 0;
        player.gameObject.transform.localPosition = tmpPos;

    }

    public void SetSkill(int no)
    {
        if(Skill!=null)
        {
            SetPlayer(no);
            if (player.mp >= Skill.Mp)
            {
                var tmp = iteamGround_Player.transform.Find(Skill.IteamName + "物件池");
                player.mp -= Skill.Mp;
                player.ResetPlayerMp();

                if (tmp != null)
                {
                    if(tmp.transform.childCount>0)
                    {
                        var iteamObj = tmp.transform.GetChild(0).gameObject;
                        iteamObj.GetComponent<Stetas>().iteam = Skill;
                        iteamObj.transform.SetParent(player.transform.parent.transform);

                        iteamObj.SetActive(true);

                    }

                }

            }
        }
    }
    public void Pause()
    {
        Time.timeScale = 0f;
    }
    public void unPause()
    {
        Time.timeScale = 1f;
    }



}
