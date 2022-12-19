using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;




public class SkillManager : MonoBehaviour
{

    //public ISkill[] skills;
    public Dictionary<string, ISkill> skillsDic = new Dictionary<string, ISkill>();

    List<List<string>> skillLists;

    Dictionary<string, Action> SkillFunc = new Dictionary<string, Action>();

    public GameObject CheckCircle;

    private void Awake()
    {
        SkillInit();

        weightDict.Add(1, 60);
        weightDict.Add(2, 20);
        weightDict.Add(3, 20);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            SkillFunc["破滅衝擊"]();

        }
    }
    void SkillInit()
    {
        //等等修改為搜索玩家跟身上裝備，直接提升效果
        SkillFunc.Add("攻擊力提升", () =>
        {
            Debug.Log("攻擊力提升");
            //tmp=data.iteamData.Hp*
            skillsDic["攻擊力提升"].BuffLevelUp();
            GamePlayManager.instance.player.stetas.player.AtkUp += 10* skillsDic["攻擊力提升"].buffLevel;
        });

        SkillFunc.Add("耐久度提升", () =>
        {
            skillsDic["耐久度提升"].BuffLevelUp();

            Debug.Log("耐久度提升");
            foreach(var data in GamePlayManager.instance.SkillButtonLists)
            {
                if(data.iteamData!=null)
                {
                    var tmpUp = data.iteamData.Hp * (0.1* skillsDic["耐久度提升"].buffLevel);
                    data.iteamData.Hp += ((float)tmpUp);

                }
            }


        });
        SkillFunc.Add("堅硬武具", () =>
        {
            skillsDic["堅硬武具"].BuffLevelUp();


            Debug.Log("堅硬武具");
            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    if(data.iteamData.type== SO_Iteam.IteamType.武具)
                    {

                        var tmpUp = data.iteamData.Hp * (0.12* skillsDic["堅硬武具"].buffLevel);
                        data.iteamData.Hp += ((float)tmpUp);

                    }

                }
            }
        });
        SkillFunc.Add("穩固魔力", () =>
        {
            Debug.Log("穩固魔力");
            skillsDic["穩固魔力"].BuffLevelUp();

            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    if (data.iteamData.type == SO_Iteam.IteamType.魔法)
                    {
                        var tmpUp = data.iteamData.Hp * (0.12 * skillsDic["穩固魔力"].buffLevel);
                        data.iteamData.Hp += ((float)tmpUp);

                    }

                }
            }
        });
        SkillFunc.Add("強健生物", () =>
        {
            Debug.Log("強健生物");
            skillsDic["強健生物"].BuffLevelUp();

            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    if (data.iteamData.type == SO_Iteam.IteamType.召喚)
                    {
                        var tmpUp = data.iteamData.Hp * (0.12 * skillsDic["強健生物"].buffLevel);
                        data.iteamData.Hp += ((float)tmpUp);

                    }
                }
            }
        });


        SkillFunc.Add("武具強化", () =>
        {
            Debug.Log("武具強化");
            skillsDic["武具強化"].BuffLevelUp();

            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    if (data.iteamData.type == SO_Iteam.IteamType.武具)
                    {
                        data.iteamData.atkUp += (12 * skillsDic["武具強化"].buffLevel);

                    }
                }
            }
        });

        SkillFunc.Add("魔力強化", () =>
        {
            Debug.Log("魔力強化");
            skillsDic["魔力強化"].BuffLevelUp();

            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    if (data.iteamData.type == SO_Iteam.IteamType.魔法)
                    {
                        data.iteamData.atkUp += (12* skillsDic["魔力強化"].buffLevel);

                    }

                }
            }
        });
        SkillFunc.Add("召靈強化", () =>
        {
            Debug.Log("召靈強化");
            skillsDic["召靈強化"].BuffLevelUp();

            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    if (data.iteamData.type == SO_Iteam.IteamType.召喚)
                    {
                        data.iteamData.atkUp += (12* skillsDic["召靈強化"].buffLevel);

                    }

                }
            }

        });
        SkillFunc.Add("銳化刀刃", () =>
        {
            Debug.Log("銳化刀刃");
            skillsDic["銳化刀刃"].BuffLevelUp();

            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    if (data.iteamData.attributesType == SO_Iteam.AttributesType.刃)
                    {
                        data.iteamData.atkUp += (15* skillsDic["銳化刀刃"].buffLevel);
                    }

                }
            }

        });

        SkillFunc.Add("堅刃", () =>
        {
            Debug.Log("堅刃");
            skillsDic["堅刃"].BuffLevelUp();

            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    if (data.iteamData.attributesType == SO_Iteam.AttributesType.刃)
                    {
                        var tmpUp = data.iteamData.Hp * (0.15 * skillsDic["堅刃"].buffLevel);
                        data.iteamData.Hp += ((float)tmpUp);

                    }

                }
            }

        });
        SkillFunc.Add("強化衝擊", () =>
        {
            Debug.Log("強化衝擊");
            skillsDic["強化衝擊"].BuffLevelUp();

            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    if (data.iteamData.attributesType == SO_Iteam.AttributesType.打)
                    {
                        data.iteamData.atkUp += (15 * skillsDic["強化衝擊"].buffLevel);
                    }

                }
            }

        });

        SkillFunc.Add("硬化", () =>
        {
            Debug.Log("硬化");
            skillsDic["硬化"].BuffLevelUp();

            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    if (data.iteamData.attributesType == SO_Iteam.AttributesType.打)
                    {
                        var tmpUp = data.iteamData.Hp * (0.15 * skillsDic["硬化"].buffLevel);
                        data.iteamData.Hp += ((float)tmpUp);
                    }

                }
            }

        });
        SkillFunc.Add("烈炎", () =>
        {
            Debug.Log("烈炎");
            skillsDic["烈炎"].BuffLevelUp();

            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    if (data.iteamData.attributesType == SO_Iteam.AttributesType.火)
                    {
                        data.iteamData.atkUp += (18 * skillsDic["烈炎"].buffLevel);
                    }

                }
            }
        });
        
        SkillFunc.Add("召炎", () =>
        {
            Debug.Log("召炎");
            skillsDic["召炎"].BuffLevelUp();

            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    if (data.iteamData.attributesType == SO_Iteam.AttributesType.火)
                    {

                        var tmpMPDown = data.iteamData.Mp * (0.18 * skillsDic["召炎"].buffLevel);
                        data.iteamData.Mp -= ((float)tmpMPDown);
                        data.RefreshUI();

                    }

                }
            }
        });



        SkillFunc.Add("止水", () =>
        {
            Debug.Log("止水");
            skillsDic["止水"].BuffLevelUp();

            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    if (data.iteamData.attributesType == SO_Iteam.AttributesType.水)
                    {
                        data.iteamData.atkUp += (18 * skillsDic["止水"].buffLevel);

                    }

                }
            }
        });

        SkillFunc.Add("引水", () =>
        {
            Debug.Log("引水");
            skillsDic["引水"].BuffLevelUp();

            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    if (data.iteamData.attributesType == SO_Iteam.AttributesType.水)
                    {

                        var tmpMPDown = data.iteamData.Mp * (0.18 * skillsDic["引水"].buffLevel);
                        data.iteamData.Mp -= ((float)tmpMPDown);
                        data.RefreshUI();
                    }

                }
            }
        });
        SkillFunc.Add("狂風", () =>
        {
            Debug.Log("狂風");
            skillsDic["狂風"].BuffLevelUp();

            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    if (data.iteamData.attributesType == SO_Iteam.AttributesType.風)
                    {
                        data.iteamData.atkUp += (18* skillsDic["狂風"].buffLevel);
                    }

                }
            }
        });
        SkillFunc.Add("呼風", () =>
        {
            Debug.Log("呼風");
            skillsDic["呼風"].BuffLevelUp();

            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    if (data.iteamData.attributesType == SO_Iteam.AttributesType.風)
                    {

                        var tmpMPDown = data.iteamData.Mp * (0.18 * skillsDic["呼風"].buffLevel);
                        data.iteamData.Mp -= ((float)tmpMPDown);
                        data.RefreshUI();

                    }

                }
            }
        });


        SkillFunc.Add("響雷", () =>
        {
            Debug.Log("響雷");
            skillsDic["響雷"].BuffLevelUp();

            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    if (data.iteamData.attributesType == SO_Iteam.AttributesType.雷)
                    {
                        data.iteamData.atkUp += (18 * skillsDic["響雷"].buffLevel);
                    }
                }
            }

        });
        SkillFunc.Add("喚雷", () =>
        {
            Debug.Log("喚雷");
            skillsDic["喚雷"].BuffLevelUp();

            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    if (data.iteamData.attributesType == SO_Iteam.AttributesType.雷)
                    {

                        var tmpMPDown = data.iteamData.Mp * (0.18 * skillsDic["喚雷"].buffLevel);
                        data.iteamData.Mp -= ((float)tmpMPDown);
                        data.RefreshUI();

                    }

                }
            }
        });

        SkillFunc.Add("雙重施法", () =>
        {
            Debug.Log("雙重施法");
            skillsDic["雙重施法"].BuffLevelUp();
            
            Func<Player,SO_Iteam,bool> func = (player,skill) => {
                if(skill.type==SO_Iteam.IteamType.魔法)
                {
                    var tmpInt = UnityEngine.Random.Range(0, 100);
                    if(tmpInt<=10* skillsDic["雙重施法"].buffLevel)
                    {
                        Debug.Log("不消耗MP");
                        Debug.Log(GamePlayManager.instance.player.transform.parent.transform.GetSiblingIndex() + 1);
                        GamePlayManager.instance.SetIteam(skill, GamePlayManager.instance.player.transform.parent.transform.GetSiblingIndex()+1, false);
                        return true;
                    }
                }
                return false;
            };
            GamePlayManager.instance.testAction += func;

        });
        SkillFunc.Add("雙重投擲", () =>
        {
            Debug.Log("雙重投擲");
            skillsDic["雙重投擲"].BuffLevelUp();
            Func<Player, SO_Iteam, bool> func = (player, skill) => {
                if (skill.type == SO_Iteam.IteamType.武具)
                {
                    var tmpInt = UnityEngine.Random.Range(0, 100);
                    if (tmpInt <= 10 * skillsDic["雙重投擲"].buffLevel)
                    {
                        Debug.Log("不消耗MP");
                        GamePlayManager.instance.SetIteam(skill, GamePlayManager.instance.player.transform.parent.transform.GetSiblingIndex()+1, false);

                        return true;
                    }
                }
                return false;
            };
            GamePlayManager.instance.testAction += func;

        });
        SkillFunc.Add("雙重召喚", () =>
        {
            Debug.Log("雙重召喚");
            skillsDic["雙重召喚"].BuffLevelUp();
            Func<Player, SO_Iteam, bool> func = (player, skill) => {
                if (skill.type == SO_Iteam.IteamType.召喚)
                {
                    var tmpInt = UnityEngine.Random.Range(0, 100);
                    if (tmpInt <= 10 * skillsDic["雙重召喚"].buffLevel)
                    {
                        Debug.Log("不消耗MP");
                        GamePlayManager.instance.SetIteam(skill, GamePlayManager.instance.player.transform.parent.transform.GetSiblingIndex()+1, false);

                        return true;
                    }
                }
                return false;
            };
            GamePlayManager.instance.testAction += func;

        });
        SkillFunc.Add("千變", () =>
        {
            Debug.Log("千變");
            skillsDic["千變"].BuffLevelUp();
            Action<SO_Iteam, SO_Iteam,Stetas> func = (saveSkill, skill,targetStetas) => {
                if(saveSkill!=skill)
                {
                    Debug.Log("增傷: " + skillsDic["千變"].buffLevel * 12 + "%");
                    targetStetas.BuffAtkUp = skillsDic["千變"].buffLevel * 12;
                }
            };

            GamePlayManager.instance.IteamATKUp += func;
        });
        SkillFunc.Add("魔力回收", () =>
        {
            Debug.Log("魔力回收");
            skillsDic["魔力回收"].BuffLevelUp();
            
            Func<Player, SO_Iteam, bool> func = (player, skill) => {

                player.mp -= skill.Mp;

                var tmpInt = UnityEngine.Random.Range(0, 100);
                if (tmpInt <= 20)
                {

                    Debug.Log(player.mp+" /回復一半/ "+ (player.mp + (skill.Mp / 2)));
                    player.mp += skill.Mp/2;

                }

                player.ResetPlayerMp();
                return true;

            };
            GamePlayManager.instance.testAction += func;

        });
        SkillFunc.Add("魔力節約", () =>
        {
            Debug.Log("魔力節約");
            skillsDic["魔力節約"].BuffLevelUp();

            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {

                    var tmpMPDown = data.iteamData.Mp * (0.1 * skillsDic["魔力節約"].buffLevel);
                    data.iteamData.Mp -= ((float)tmpMPDown);
                    data.RefreshUI();

                }
            }

        });
        SkillFunc.Add("破滅衝擊", () =>
        {
            Debug.Log("破滅衝擊");
            skillsDic["破滅衝擊"].BuffLevelUp();

            Action<Stetas> func = (objStetas) => {
                if (objStetas.type == Stetas.Type.道具&&objStetas.iteam.type==SO_Iteam.IteamType.武具)
                {
                    objStetas.gameObject.AddComponent<ShatteringShock>();
                    //Instantiate(CheckCircle, objStetas.gameObject.transform);

                }
            };
            GamePlayManager.instance.CreatIteam_AddDeath += func;



        });
        SkillFunc.Add("再生", () =>
        {
            Debug.Log("再生");
            skillsDic["再生"].BuffLevelUp();

            Action<Stetas> func = (objStetas) => {
               if(objStetas.type==Stetas.Type.召喚)
               {
                    if(objStetas.iteam.IteamName!= "巨大史萊姆"|| objStetas.iteam.IteamName != "貓又"|| objStetas.iteam.IteamName != "鳳凰")
                    {
                        var tmp=UnityEngine.Random.Range(0, 100);
                        if(tmp<=10)
                        {
                            Debug.Log("追加再生狀態");
                            objStetas.gameObject.AddComponent<Regeneration>();
                        }
                    }
               }
            };
            GamePlayManager.instance.CreatIteam_AddDeath += func;
        });
    }
    

    public void SetAllSkill(List<List<string>> data)
    {
        skillLists = data;


        for(int i=0;i< skillLists.Count-1; i++)
        {
            SkillData skillData = new SkillData();
            skillData.skillName = skillLists[i + 1][0];
            skillData.skillEffect = skillLists[i + 1][1];
            skillData.Weight = int.Parse(skillLists[i + 1][4]);

            if (skillLists[i + 1][2] == "-")
            {
                skillData.maxLevel = 999;
            }
            else
            {
                skillData.maxLevel = int.Parse(skillLists[i + 1][2]);
            }
            ISkill iSkill = new ISkill();
            iSkill.action = SkillFunc[skillData.skillName];
            
            iSkill.data = skillData;

            skillsDic.Add(skillData.skillName, iSkill);


        }

        //skills = new ISkill[skillLists.Count - 1];


        //for (int i=0;i< skills.Length;i++)
        //{
        //    SkillData skillData=new SkillData();
        //    skillData.skillName = skillLists[i + 1][0];
        //    skillData.skillEffect = skillLists[i + 1][1];
        //    skillData.Weight = int.Parse(skillLists[i + 1][4]);

        //    if (skillLists[i + 1][2]== "-")
        //    {
        //        skillData.maxLevel = 999;
        //    }
        //    else
        //    {
        //        skillData.maxLevel = int.Parse(skillLists[i + 1][2]);
        //    }
        //    ISkill iSkill = new ISkill();
        //    iSkill.action = SkillFunc[skillData.skillName];
        //    skills[i] = iSkill;
        //    skills[i].data = skillData;
        //}
    }

    //public ISkill GetSkill()
    //{
    //    return skills[UnityEngine.Random.Range(0, skills.Length)];
    //}

    //權重
    private Dictionary<int, int> weightDict = new Dictionary<int, int>();

    private int GetTotalWeight()
    {
        int totalWeight = 0;
        foreach (var weight in weightDict.Values)
        {
            totalWeight += weight;
        }
        return totalWeight;
    }

    private int GetRanId()
    {
        int ranNum = UnityEngine.Random.Range(0, GetTotalWeight() + 1);
        int counter = 0;
        foreach (var temp in weightDict)
        {
            counter += temp.Value;
            if (ranNum <= counter)
            {
                Debug.Log("隨機數：" + ranNum + ",隨機id：" + temp.Key);
                return temp.Key;
            }
        }
        Debug.LogError("沒有隨機到，隨機數為1：" + ranNum);
        return 1;
    }

    public List<ISkill> GetSkillLists()
    {
        List<ISkill> tmpskillLists=new List<ISkill>();

        List<ISkill> skillLists=new List<ISkill>();

        skillLists.Clear();

        // 技能數量小於3時，無限抽取
        while (skillLists.Count < 3)
        {
            tmpskillLists.Clear();
            // 抽取權重值
            var tmpID = GetRanId();
            // 將符合權重值的技能追加進暫時清單
            foreach(var tmpdata in skillsDic)
            {
                if (tmpdata.Value.data.Weight== tmpID)
                {
                    tmpskillLists.Add(tmpdata.Value);
                    // 檢查已抽過技能有抽過就從待選名單刪除，確保可抽選清單內都是沒有已抽過內容
                    
                    if (tmpdata.Value.buffLevel >= tmpdata.Value.data.maxLevel)
                    {
                        Debug.Log("刪除已到達上限技能");
                        tmpskillLists.Remove(tmpdata.Value);
                    }

                    foreach (var data in skillLists)
                    {
                        if (data.data.skillName == tmpdata.Value.data.skillName)
                        {
                            Debug.Log("刪除以抽過技能");
                            tmpskillLists.Remove(tmpdata.Value);
                        }
                        

                    }
                }
            }

            // 暫時清單不為零時，隨機抽取清單中一份技能加進真正抽取內容
            if (tmpskillLists.Count>0)
            {
                skillLists.Add(tmpskillLists[UnityEngine.Random.Range(0, tmpskillLists.Count-1)]);
            }
        }
        return skillLists;
    }
}
