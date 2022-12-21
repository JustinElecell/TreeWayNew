using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerSkill
{
    Action<int,int> action;
    public int skillLevel;
    //public int skill
    public PlayerSkill()
    {
        skillLevel = 0;
    }
    //public void BuffLevelUp()
    //{
    //    //不能超過最大等級
    //    if (skillLevel >= data.maxLevel)
    //    {
    //        return;
    //    }
    //    skillLevel++;
    //}
}
