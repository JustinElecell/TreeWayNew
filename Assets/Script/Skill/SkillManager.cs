using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;




public class SkillManager : MonoBehaviour
{

    public ISkill[] skills;

    List<List<string>> skillLists;

    Dictionary<string, Action> SkillFunc = new Dictionary<string, Action>();



    private void Awake()
    {
        SkillInit();

        weightDict.Add(1, 60);
        weightDict.Add(2, 20);
        weightDict.Add(3, 20);
    }
    void SkillInit()
    {
        //等等修改為搜索玩家跟身上裝備，直接提升效果
        SkillFunc.Add("攻擊力提升", () =>
        {
            Debug.Log("攻擊力提升");
            GamePlayManager.instance.player.stetas.player.AtkUp += 10;
        });

        SkillFunc.Add("耐久度提升", () =>
        {
            Debug.Log("耐久度提升");
            foreach(var data in GamePlayManager.instance.SkillButtonLists)
            {
                if(data.iteamData!=null)
                {
                    var tmpUp = data.iteamData.Hp * 0.1;
                    data.iteamData.Hp += ((float)tmpUp);
                }
            }


        });
        SkillFunc.Add("堅硬武具", () =>
        {
            Debug.Log("堅硬武具");
            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    if(data.iteamData.type== SO_Iteam.IteamType.武具)
                    {
                        var tmpUp = data.iteamData.Hp * 0.12;
                        data.iteamData.Hp += ((float)tmpUp);
                    }

                }
            }
        });
        SkillFunc.Add("穩固魔力", () =>
        {
            Debug.Log("穩固魔力");
            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    if (data.iteamData.type == SO_Iteam.IteamType.魔法)
                    {
                        var tmpUp = data.iteamData.Hp * 0.12;
                        data.iteamData.Hp += ((float)tmpUp);
                    }

                }
            }
        });
        SkillFunc.Add("強健生物", () =>
        {
            Debug.Log("強健生物");
            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    if (data.iteamData.type == SO_Iteam.IteamType.召喚)
                    {
                        var tmpUp = data.iteamData.Hp * 0.12;
                        data.iteamData.Hp += ((float)tmpUp);
                    }

                }
            }
        });
        SkillFunc.Add("武具強化", () =>
        {
            Debug.Log("武具強化");

            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    if (data.iteamData.type == SO_Iteam.IteamType.武具)
                    {
                        data.iteamData.atkUp += 12;
                    }

                }
            }
        });
        SkillFunc.Add("魔力強化", () =>
        {
            Debug.Log("魔力強化");
            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    if (data.iteamData.type == SO_Iteam.IteamType.魔法)
                    {
                        data.iteamData.atkUp += 12;
                    }

                }
            }
        });
        SkillFunc.Add("召靈強化", () =>
        {
            Debug.Log("召靈強化");
            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    if (data.iteamData.type == SO_Iteam.IteamType.召喚)
                    {
                        data.iteamData.atkUp += 12;
                    }

                }
            }

        });
        SkillFunc.Add("銳化刀刃", () =>
        {
            Debug.Log("銳化刀刃");
            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    if (data.iteamData.attributesType == SO_Iteam.AttributesType.刃)
                    {
                        data.iteamData.atkUp += 15;
                    }

                }
            }

        });
        SkillFunc.Add("強化衝擊", () =>
        {
            Debug.Log("強化衝擊");
            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    if (data.iteamData.attributesType == SO_Iteam.AttributesType.打)
                    {
                        data.iteamData.atkUp += 15;
                    }

                }
            }

        });
        SkillFunc.Add("烈炎", () =>
        {
            Debug.Log("烈炎");
            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    if (data.iteamData.attributesType == SO_Iteam.AttributesType.火)
                    {
                        data.iteamData.atkUp += 15;
                    }

                }
            }

        });
        SkillFunc.Add("止水", () =>
        {
            Debug.Log("止水");
            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    if (data.iteamData.attributesType == SO_Iteam.AttributesType.水)
                    {
                        data.iteamData.atkUp += 15;
                    }

                }
            }
        });
        SkillFunc.Add("狂風", () =>
        {
            Debug.Log("狂風");
            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    if (data.iteamData.attributesType == SO_Iteam.AttributesType.風)
                    {
                        data.iteamData.atkUp += 15;
                    }

                }
            }

        });
        SkillFunc.Add("響雷", () =>
        {
            Debug.Log("響雷");
            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    if (data.iteamData.attributesType == SO_Iteam.AttributesType.雷)
                    {
                        data.iteamData.atkUp += 15;
                    }

                }
            }

        });
        SkillFunc.Add("雙重施法", () =>
        {
            Debug.Log("雙重施法");

        });
        SkillFunc.Add("雙重投擲", () =>
        {
            Debug.Log("雙重投擲");

        });
        SkillFunc.Add("雙重召喚", () =>
        {
            Debug.Log("雙重召喚");

        });
        SkillFunc.Add("千變", () =>
        {
            Debug.Log("千變");

        });
        SkillFunc.Add("魔力回收", () =>
        {
            Debug.Log("魔力回收");

        });
        SkillFunc.Add("魔力節約", () =>
        {
            Debug.Log("魔力節約");

        });
        SkillFunc.Add("破滅衝擊", () =>
        {
            Debug.Log("破滅衝擊");

        });
    }
    

    public void SetAllSkill(List<List<string>> data)
    {
        skillLists = data;

        skills = new ISkill[skillLists.Count-1];

        for (int i=0;i< skills.Length;i++)
        {
            SkillData skillData=new SkillData();
            skillData.skillName = skillLists[i + 1][0];
            skillData.skillEffect = skillLists[i + 1][1];
            skillData.Weight = int.Parse(skillLists[i + 1][4]);

            if (skillLists[i + 1][2]== "-")
            {
                skillData.maxLevel = 999;
            }
            else
            {
                skillData.maxLevel = int.Parse(skillLists[i + 1][2]);
            }
            ISkill iSkill = new ISkill();
            iSkill.action = SkillFunc[skillData.skillName];
            skills[i] = iSkill;
            skills[i].data = skillData;
        }
    }

    public ISkill GetSkill()
    {
        return skills[UnityEngine.Random.Range(0, skills.Length)];
    }

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
            foreach(var tmpdata in skills)
            {
                if (tmpdata.data.Weight== tmpID)
                {
                    tmpskillLists.Add(tmpdata);
                    // 檢查已抽過技能有抽過就從待選名單刪除，確保可抽選清單內都是沒有已抽過內容
                    foreach (var data in skillLists)
                    {
                        if (data.data.skillName == tmpdata.data.skillName)
                        {
                            tmpskillLists.Remove(tmpdata);
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
