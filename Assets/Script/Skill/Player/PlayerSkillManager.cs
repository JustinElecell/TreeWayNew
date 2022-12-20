using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PlayerSkillManager : MonoBehaviour
{
    Dictionary<int, Action<int,int>> skillFunc = new Dictionary<int, Action<int,int>>();

    public void Init()
    {
        FuncInit();
    }

    void FuncInit()
    {
        //「x類型」武器攻擊力提升 y %
        //0 = 全部,1 = 武具,2 = 魔法,3 = 召喚
        skillFunc.Add(1, (x,y) => {
            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    float tmpUp = 0;
                    switch (x)
                    {
                        case 0:
                            tmpUp = data.iteamData.Atk * (y / 100);
                            data.iteamData.Atk += (tmpUp);
                            
                            break;
                        case 1:
                            if (data.iteamData.type == SO_Iteam.IteamType.武具)
                            {
                                tmpUp = data.iteamData.Atk * (y / 100);
                                data.iteamData.Atk += ((float)tmpUp);
                            }
                            break;
                        case 2:
                            if (data.iteamData.type == SO_Iteam.IteamType.魔法)
                            {
                                tmpUp = data.iteamData.Atk * (y / 100);
                                data.iteamData.Atk += ((float)tmpUp);
                            }
                            break;
                        case 3:
                            if (data.iteamData.type == SO_Iteam.IteamType.召喚)
                            {
                                tmpUp = data.iteamData.Atk * (y / 100);
                                data.iteamData.Atk += ((float)tmpUp);
                            }
                            break;
                    }
                }
            }
        });
        //「x屬性」武器攻擊力提升 y %
        //1 = 刃,2 = 打,3 = 火,4 = 雷,5 = 風,
        //6 = 水,7 = 火 + 雷,8 = 風 + 水,9 = 雷 + 風,10 = 水 + 火,
        //11 = 雷 + 水,12 = 火 + 風
        skillFunc.Add(2, (x, y) => {
            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    switch (x)
                    {
                        case 1:
                            if (data.iteamData.attributesType == SO_Iteam.AttributesType.刃)
                            {
                                var tmpUp = data.iteamData.Atk * (y / 100);
                                data.iteamData.Atk += ((float)tmpUp);
                            }
                            break;
                        case 2:
                            if (data.iteamData.attributesType == SO_Iteam.AttributesType.打)
                            {
                                var tmpUp = data.iteamData.Atk * (y / 100);
                                data.iteamData.Atk += ((float)tmpUp);
                            }
                            break;
                        case 3:
                            if (data.iteamData.attributesType == SO_Iteam.AttributesType.火)
                            {
                                var tmpUp = data.iteamData.Atk * (y / 100);
                                data.iteamData.Atk += ((float)tmpUp);
                            }
                            break;
                        case 4:
                            if (data.iteamData.attributesType == SO_Iteam.AttributesType.雷)
                            {
                                var tmpUp = data.iteamData.Atk * (y / 100);
                                data.iteamData.Atk += ((float)tmpUp);
                            }
                            break;
                        case 5:
                            if (data.iteamData.attributesType == SO_Iteam.AttributesType.風)
                            {
                                var tmpUp = data.iteamData.Atk * (y / 100);
                                data.iteamData.Atk += ((float)tmpUp);
                            }
                            break;
                        case 6:
                            if (data.iteamData.attributesType == SO_Iteam.AttributesType.水)
                            {
                                var tmpUp = data.iteamData.Atk * (y / 100);
                                data.iteamData.Atk += ((float)tmpUp);
                            }
                            break;
                        case 7:
                            if (data.iteamData.attributesType == SO_Iteam.AttributesType.火|| data.iteamData.attributesType == SO_Iteam.AttributesType.雷)
                            {
                                var tmpUp = data.iteamData.Atk * (y / 100);
                                data.iteamData.Atk += ((float)tmpUp);
                            }
                            break;
                        case 8:
                            if (data.iteamData.attributesType == SO_Iteam.AttributesType.風|| data.iteamData.attributesType == SO_Iteam.AttributesType.水)
                            {
                                var tmpUp = data.iteamData.Atk * (y / 100);
                                data.iteamData.Atk += ((float)tmpUp);
                            }
                            break;
                        case 9:
                            if (data.iteamData.attributesType == SO_Iteam.AttributesType.風|| data.iteamData.attributesType == SO_Iteam.AttributesType.雷)
                            {
                                var tmpUp = data.iteamData.Atk * (y / 100);
                                data.iteamData.Atk += ((float)tmpUp);
                            }
                            break;
                        case 10:
                            if (data.iteamData.attributesType == SO_Iteam.AttributesType.水|| data.iteamData.attributesType == SO_Iteam.AttributesType.火)
                            {
                                var tmpUp = data.iteamData.Atk * (y / 100);
                                data.iteamData.Atk += ((float)tmpUp);
                            }
                            break;
                        case 11:
                            if (data.iteamData.attributesType == SO_Iteam.AttributesType.雷|| data.iteamData.attributesType == SO_Iteam.AttributesType.水)
                            {
                                var tmpUp = data.iteamData.Atk * (y / 100);
                                data.iteamData.Atk += ((float)tmpUp);
                            }
                            break;
                        case 12:
                            if (data.iteamData.attributesType == SO_Iteam.AttributesType.風|| data.iteamData.attributesType == SO_Iteam.AttributesType.火)
                            {
                                var tmpUp = data.iteamData.Atk * (y / 100);
                                data.iteamData.Atk += ((float)tmpUp);
                            }
                            break;
                    }
                }
            }


        });
        // 使「x類型」武器的消耗MP減少 y %
        skillFunc.Add(3, (x, y) => {
            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    float tmpUp = 0;
                    switch (x)
                    {
                        case 0:
                            tmpUp = data.iteamData.Mp * (y / 100);
                            data.iteamData.Mp -= (tmpUp);

                            break;
                        case 1:
                            if (data.iteamData.type == SO_Iteam.IteamType.武具)
                            {
                                tmpUp = data.iteamData.Mp * (y / 100);
                                data.iteamData.Mp -= ((float)tmpUp);
                            }
                            break;
                        case 2:
                            if (data.iteamData.type == SO_Iteam.IteamType.魔法)
                            {
                                tmpUp = data.iteamData.Mp * (y / 100);
                                data.iteamData.Mp -= ((float)tmpUp);
                            }
                            break;
                        case 3:
                            if (data.iteamData.type == SO_Iteam.IteamType.召喚)
                            {
                                tmpUp = data.iteamData.Mp * (y / 100);
                                data.iteamData.Mp -= ((float)tmpUp);
                            }
                            break;
                    }
                }
            }


        });
        // 使「x屬性」武器的消耗MP減少 y %
        skillFunc.Add(4, (x, y) => {
            foreach (var data in GamePlayManager.instance.SkillButtonLists)
            {
                if (data.iteamData != null)
                {
                    switch (x)
                    {
                        case 1:
                            if (data.iteamData.attributesType == SO_Iteam.AttributesType.刃)
                            {
                                var tmpUp = data.iteamData.Mp * (y / 100);
                                data.iteamData.Mp -= ((float)tmpUp);
                            }
                            break;
                        case 2:
                            if (data.iteamData.attributesType == SO_Iteam.AttributesType.打)
                            {
                                var tmpUp = data.iteamData.Mp * (y / 100);
                                data.iteamData.Mp -= ((float)tmpUp);
                            }
                            break;
                        case 3:
                            if (data.iteamData.attributesType == SO_Iteam.AttributesType.火)
                            {
                                var tmpUp = data.iteamData.Mp * (y / 100);
                                data.iteamData.Mp -= ((float)tmpUp);
                            }
                            break;
                        case 4:
                            if (data.iteamData.attributesType == SO_Iteam.AttributesType.雷)
                            {
                                var tmpUp = data.iteamData.Mp * (y / 100);
                                data.iteamData.Mp -= ((float)tmpUp);
                            }
                            break;
                        case 5:
                            if (data.iteamData.attributesType == SO_Iteam.AttributesType.風)
                            {
                                var tmpUp = data.iteamData.Mp * (y / 100);
                                data.iteamData.Mp -= ((float)tmpUp);
                            }
                            break;
                        case 6:
                            if (data.iteamData.attributesType == SO_Iteam.AttributesType.水)
                            {
                                var tmpUp = data.iteamData.Mp * (y / 100);
                                data.iteamData.Mp -= ((float)tmpUp);
                            }
                            break;
                        case 7:
                            if (data.iteamData.attributesType == SO_Iteam.AttributesType.火 || data.iteamData.attributesType == SO_Iteam.AttributesType.雷)
                            {
                                var tmpUp = data.iteamData.Mp * (y / 100);
                                data.iteamData.Mp -= ((float)tmpUp);
                            }
                            break;
                        case 8:
                            if (data.iteamData.attributesType == SO_Iteam.AttributesType.風 || data.iteamData.attributesType == SO_Iteam.AttributesType.水)
                            {
                                var tmpUp = data.iteamData.Mp * (y / 100);
                                data.iteamData.Mp -= ((float)tmpUp);
                            }
                            break;
                        case 9:
                            if (data.iteamData.attributesType == SO_Iteam.AttributesType.風 || data.iteamData.attributesType == SO_Iteam.AttributesType.雷)
                            {
                                var tmpUp = data.iteamData.Mp * (y / 100);
                                data.iteamData.Mp -= ((float)tmpUp);
                            }
                            break;
                        case 10:
                            if (data.iteamData.attributesType == SO_Iteam.AttributesType.水 || data.iteamData.attributesType == SO_Iteam.AttributesType.火)
                            {
                                var tmpUp = data.iteamData.Mp * (y / 100);
                                data.iteamData.Mp -= ((float)tmpUp);
                            }
                            break;
                        case 11:
                            if (data.iteamData.attributesType == SO_Iteam.AttributesType.雷 || data.iteamData.attributesType == SO_Iteam.AttributesType.水)
                            {
                                var tmpUp = data.iteamData.Mp * (y / 100);
                                data.iteamData.Mp -= ((float)tmpUp);
                            }
                            break;
                        case 12:
                            if (data.iteamData.attributesType == SO_Iteam.AttributesType.風 || data.iteamData.attributesType == SO_Iteam.AttributesType.火)
                            {
                                var tmpUp = data.iteamData.Mp * (y / 100);
                                data.iteamData.Mp -= ((float)tmpUp);
                            }
                            break;
                    }
                }
            }


        });
        
        // 使用「x類型」武器時，有 y % 的機率額外發動一次(不消耗MP)
        skillFunc.Add(5, (x, y) => {
            
            Func<Player, SO_Iteam, bool> func = (player, skill) => {         
                var tmpInt = UnityEngine.Random.Range(0, 100);
                switch (x)
                {
                    case 0:
                        if (tmpInt <= y)
                        {
                            GamePlayManager.instance.SetIteam(skill, GamePlayManager.instance.player.transform.parent.transform.GetSiblingIndex() + 1, true);
                        }
                        break;
                    case 1:
                        if (skill.type == SO_Iteam.IteamType.武具)
                        {
                            if (tmpInt <= y)
                            {
                                GamePlayManager.instance.SetIteam(skill, GamePlayManager.instance.player.transform.parent.transform.GetSiblingIndex() + 1, true);
                            }
                        }
                        break;
                    case 2:
                        if (skill.type == SO_Iteam.IteamType.魔法)
                        {
                            if (tmpInt <= y)
                            {
                                GamePlayManager.instance.SetIteam(skill, GamePlayManager.instance.player.transform.parent.transform.GetSiblingIndex() + 1, true);
                            }
                        }
                        break;
                    case 3:
                        if (skill.type == SO_Iteam.IteamType.召喚)
                        {
                            if (tmpInt <= y)
                            {
                                GamePlayManager.instance.SetIteam(skill, GamePlayManager.instance.player.transform.parent.transform.GetSiblingIndex() + 1, true);
                            }
                        }
                        break;
                }
                return true;
            };
            GamePlayManager.instance.testAction += func;
        });
        // 使用「x屬性」武器時，有 y % 的機率額外發動一次(不消耗MP)
        skillFunc.Add(6, (x, y) => {
            Func<Player, SO_Iteam, bool> func = (player, skill) => {
                var tmpInt = UnityEngine.Random.Range(0, 100);
                switch (x)
                {
                    case 1:
                        if (skill.attributesType == SO_Iteam.AttributesType.刃)
                        {
                            if (tmpInt <= y)
                            {
                                GamePlayManager.instance.SetIteam(skill, GamePlayManager.instance.player.transform.parent.transform.GetSiblingIndex() + 1, true);
                            }
                        }
                        break;
                    case 2:
                        if (skill.attributesType == SO_Iteam.AttributesType.打)
                        {
                            if (tmpInt <= y)
                            {
                                GamePlayManager.instance.SetIteam(skill, GamePlayManager.instance.player.transform.parent.transform.GetSiblingIndex() + 1, true);
                            }
                        }
                        break;
                    case 3:
                        if (skill.attributesType == SO_Iteam.AttributesType.火)
                        {
                            if (tmpInt <= y)
                            {
                                GamePlayManager.instance.SetIteam(skill, GamePlayManager.instance.player.transform.parent.transform.GetSiblingIndex() + 1, true);
                            }
                        }
                        break;
                    case 4:
                        if (skill.attributesType == SO_Iteam.AttributesType.雷)
                        {
                            if (tmpInt <= y)
                            {
                                GamePlayManager.instance.SetIteam(skill, GamePlayManager.instance.player.transform.parent.transform.GetSiblingIndex() + 1, true);
                            }
                        }
                        break;
                    case 5:
                        if (skill.attributesType == SO_Iteam.AttributesType.風)
                        {
                            if (tmpInt <= y)
                            {
                                GamePlayManager.instance.SetIteam(skill, GamePlayManager.instance.player.transform.parent.transform.GetSiblingIndex() + 1, true);
                            }
                        }
                        break;
                    case 6:
                        if (skill.attributesType == SO_Iteam.AttributesType.水)
                        {
                            if (tmpInt <= y)
                            {
                                GamePlayManager.instance.SetIteam(skill, GamePlayManager.instance.player.transform.parent.transform.GetSiblingIndex() + 1, true);
                            }
                        }
                        break;
                    case 7:
                        if (skill.attributesType == SO_Iteam.AttributesType.火 || skill.attributesType == SO_Iteam.AttributesType.雷)
                        {
                            if (tmpInt <= y)
                            {
                                GamePlayManager.instance.SetIteam(skill, GamePlayManager.instance.player.transform.parent.transform.GetSiblingIndex() + 1, true);
                            }
                        }
                        break;
                    case 8:
                        if (skill.attributesType == SO_Iteam.AttributesType.風 || skill.attributesType == SO_Iteam.AttributesType.水)
                        {
                            if (tmpInt <= y)
                            {
                                GamePlayManager.instance.SetIteam(skill, GamePlayManager.instance.player.transform.parent.transform.GetSiblingIndex() + 1, true);
                            }
                        }
                        break;
                    case 9:
                        if (skill.attributesType == SO_Iteam.AttributesType.風 || skill.attributesType == SO_Iteam.AttributesType.雷)
                        {
                            if (tmpInt <= y)
                            {
                                GamePlayManager.instance.SetIteam(skill, GamePlayManager.instance.player.transform.parent.transform.GetSiblingIndex() + 1, true);
                            }
                        }
                        break;
                    case 10:
                        if (skill.attributesType == SO_Iteam.AttributesType.水 || skill.attributesType == SO_Iteam.AttributesType.火)
                        {
                            if (tmpInt <= y)
                            {
                                GamePlayManager.instance.SetIteam(skill, GamePlayManager.instance.player.transform.parent.transform.GetSiblingIndex() + 1, true);
                            }
                        }
                        break;
                    case 11:
                        if (skill.attributesType == SO_Iteam.AttributesType.雷 || skill.attributesType == SO_Iteam.AttributesType.水)
                        {
                            if (tmpInt <= y)
                            {
                                GamePlayManager.instance.SetIteam(skill, GamePlayManager.instance.player.transform.parent.transform.GetSiblingIndex() + 1, true);
                            }
                        }
                        break;
                    case 12:
                        if (skill.attributesType == SO_Iteam.AttributesType.風 || skill.attributesType == SO_Iteam.AttributesType.火)
                        {
                            if (tmpInt <= y)
                            {
                                GamePlayManager.instance.SetIteam(skill, GamePlayManager.instance.player.transform.parent.transform.GetSiblingIndex() + 1, true);
                            }
                        }
                        break;
                }
                return true;
            };
            GamePlayManager.instance.testAction += func;


        });
        // 抽取升級Buff時，選項+x
        skillFunc.Add(7, (x, y) => {
            GamePlayManager.instance.GetBuffMax += x;
        });
        // 每攜帶一種類型的裝備，增加 x % 的攻擊力
        skillFunc.Add(8, (x, y) => {
            Dictionary<SO_Iteam.IteamType, int> tmp = new Dictionary<SO_Iteam.IteamType, int>();
            
            foreach (var data in MainManager.instance.skillIteams)
            {
                if(data!=null)
                {
                    Debug.Log(data.type);

                    tmp.Add(data.type, 0);

                }
            }

            GamePlayManager.instance.player.stetas.player.AtkUp += tmp.Count * x;

        });
        //「x類型」武器有y % 的機率使造成傷害翻倍
        skillFunc.Add(9, (x, y) => {
            Action<SO_Iteam, SO_Iteam, Stetas> func = (saveSkill, skill, targetStetas) => {
                var tmpInt = UnityEngine.Random.Range(0, 100);
                switch (x)
                {
                    case 0:
                        if(tmpInt<=y)
                        {
                            targetStetas.BuffAtkUp = 100;
                        }
                        break;
                    case 1:
                        if (skill.type == SO_Iteam.IteamType.武具)
                        {
                            if (tmpInt <= y)
                            {
                                targetStetas.BuffAtkUp = 100;
                            }
                        }
                        break;
                    case 2:
                        if (skill.type == SO_Iteam.IteamType.魔法)
                        {
                            if (tmpInt <= y)
                            {
                                targetStetas.BuffAtkUp = 100;
                            }
                        }
                        break;
                    case 3:
                        if (skill.type == SO_Iteam.IteamType.召喚)
                        {
                            if (tmpInt <= y)
                            {
                                targetStetas.BuffAtkUp = 100;
                            }
                        }
                        break;
                }
            };
            GamePlayManager.instance.IteamATKUp += func;
        });
        //「x屬性」武器有y % 的機率使造成傷害翻倍
        skillFunc.Add(10, (x, y) => {
            Action<SO_Iteam, SO_Iteam, Stetas> func = (saveSkill, skill, targetStetas) => {
                var tmpInt = UnityEngine.Random.Range(0, 100);
                switch (x)
                {
                    case 1:
                        if (skill.attributesType == SO_Iteam.AttributesType.刃)
                        {
                            if (tmpInt <= y)
                            {
                                targetStetas.BuffAtkUp = 100;
                            }
                        }
                        break;
                    case 2:
                        if (skill.attributesType == SO_Iteam.AttributesType.打)
                        {
                            if (tmpInt <= y)
                            {
                                targetStetas.BuffAtkUp = 100;
                            }
                        }
                        break;
                    case 3:
                        if (skill.attributesType == SO_Iteam.AttributesType.火)
                        {
                            if (tmpInt <= y)
                            {
                                targetStetas.BuffAtkUp = 100;
                            }
                        }
                        break;
                    case 4:
                        if (skill.attributesType == SO_Iteam.AttributesType.雷)
                        {
                            if (tmpInt <= y)
                            {
                                targetStetas.BuffAtkUp = 100;
                            }
                        }
                        break;
                    case 5:
                        if (skill.attributesType == SO_Iteam.AttributesType.風)
                        {
                            if (tmpInt <= y)
                            {
                                targetStetas.BuffAtkUp = 100;
                            }
                        }
                        break;
                    case 6:
                        if (skill.attributesType == SO_Iteam.AttributesType.水)
                        {
                            if (tmpInt <= y)
                            {
                                targetStetas.BuffAtkUp = 100;
                            }
                        }
                        break;
                    case 7:
                        if (skill.attributesType == SO_Iteam.AttributesType.火 || skill.attributesType == SO_Iteam.AttributesType.雷)
                        {
                            if (tmpInt <= y)
                            {
                                targetStetas.BuffAtkUp = 100;
                            }
                        }
                        break;
                    case 8:
                        if (skill.attributesType == SO_Iteam.AttributesType.風 || skill.attributesType == SO_Iteam.AttributesType.水)
                        {
                            if (tmpInt <= y)
                            {
                                targetStetas.BuffAtkUp = 100;
                            }
                        }
                        break;
                    case 9:
                        if (skill.attributesType == SO_Iteam.AttributesType.風 || skill.attributesType == SO_Iteam.AttributesType.雷)
                        {
                            if (tmpInt <= y)
                            {
                                targetStetas.BuffAtkUp = 100;
                            }
                        }
                        break;
                    case 10:
                        if (skill.attributesType == SO_Iteam.AttributesType.水 || skill.attributesType == SO_Iteam.AttributesType.火)
                        {
                            if (tmpInt <= y)
                            {
                                targetStetas.BuffAtkUp = 100;
                            }
                        }
                        break;
                    case 11:
                        if (skill.attributesType == SO_Iteam.AttributesType.雷 || skill.attributesType == SO_Iteam.AttributesType.水)
                        {
                            if (tmpInt <= y)
                            {
                                targetStetas.BuffAtkUp = 100;
                            }
                        }
                        break;
                    case 12:
                        if (skill.attributesType == SO_Iteam.AttributesType.風 || skill.attributesType == SO_Iteam.AttributesType.火)
                        {
                            if (tmpInt <= y)
                            {
                                targetStetas.BuffAtkUp = 100;
                            }
                        }
                        break;
                }
            };
            GamePlayManager.instance.IteamATKUp += func;


        });

        // 使用消費MP在「x」以上的武器時，攻擊力提升 y %
        skillFunc.Add(11, (x, y) => {
            Action<SO_Iteam, SO_Iteam, Stetas> func = (saveSkill, skill, targetStetas) => {
                foreach(var data in GamePlayManager.instance.SkillButtonLists)
                {
                    if(data.iteamData!=null&&data.iteamData.IteamName==skill.IteamName)
                    {
                        if(data.tmpIteamData.Mp>=x)
                        {
                            targetStetas.iteam.temporaryAtkUp += y;
                        }
                    }
                }
            };
            GamePlayManager.instance.IteamATKUp += func;

        });
        // 使用消費MP在「x」以下的武器時，攻擊力提升 y %
        skillFunc.Add(12, (x, y) => {
            Action<SO_Iteam, SO_Iteam, Stetas> func = (saveSkill, skill, targetStetas) => {
                foreach (var data in GamePlayManager.instance.SkillButtonLists)
                {
                    if (data.iteamData != null && data.iteamData.IteamName == skill.IteamName)
                    {
                        if (data.tmpIteamData.Mp <= x)
                        {
                            targetStetas.iteam.temporaryAtkUp += y;
                        }
                    }
                }
            };
            GamePlayManager.instance.IteamATKUp += func;
        });

        // 當現有MP在「x%」以上時，攻擊力提升 y %
        skillFunc.Add(13, (x, y) => {
            Action<SO_Iteam, SO_Iteam, Stetas> func = (saveSkill, skill, targetStetas) => {
                if (GamePlayManager.instance.player.mp >= x)
                {
                    targetStetas.iteam.temporaryAtkUp += y;
                }
            };
            GamePlayManager.instance.IteamATKUp += func;
        });

        // 當現有MP在「x%」以下時，攻擊力提升 y %
        skillFunc.Add(14, (x, y) => {
            Action<SO_Iteam, SO_Iteam, Stetas> func = (saveSkill, skill, targetStetas) => {

                if (GamePlayManager.instance.player.mp <= x)
                {
                    targetStetas.iteam.temporaryAtkUp += y;
                }
            };
            GamePlayManager.instance.IteamATKUp += func;
        });

        // 使最大HP提升 x %
        skillFunc.Add(15, (x, y) => {

            var tmpHpUp = GamePlayManager.instance.player.stetas.HpMax * (x / 100);
            GamePlayManager.instance.player.stetas.HpMax += tmpHpUp;
            GamePlayManager.instance.player.stetas.Hp = GamePlayManager.instance.player.stetas.HpMax;

        });
        // 使基礎MP上限提升 x %
        skillFunc.Add(16, (x, y) => {
            var tmpMpUp = GamePlayManager.instance.player.mpMax * (x / 100);
            GamePlayManager.instance.player.mpMax += tmpMpUp;
        });
        // 使Mp恢復速度提升 x %
        skillFunc.Add(17, (x, y) => {
            GamePlayManager.instance.player.mpUp += 12;


        });

        //使受到傷害減少 x %
        skillFunc.Add(18, (x, y) => {



        });
        skillFunc.Add(19, (x, y) => {



        });
        skillFunc.Add(20, (x, y) => {



        });

    }



}
