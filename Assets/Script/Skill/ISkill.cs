using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
public class ISkill
{
    public int buffLevel;
    public SkillData data;
    //初始化，獲取對應BuffData,輸入至IBuff子類
    public ISkill()
    {
        buffLevel = 0;
    }
    public Action action;


    ////觸發類buff單次發生
    //public virtual void BuffEffect() { }
    ////狀態類buff開始
    //public virtual void BuffStart() { }
    ////狀態類buff結束
    //public virtual void BuffEnd() { }


    public void BuffLevelUp()
    {
        //不能超過最大等級
        if (buffLevel >= data.maxLevel)
        {
            return;
        }
        buffLevel++;
    }

    ////獲取buff的data數據
    //public void SetBuffData(SkillData inputdata)
    //{
    //    if (this.data != null || inputdata == null) { return; }
    //    this.data = inputdata;
    //}




}
