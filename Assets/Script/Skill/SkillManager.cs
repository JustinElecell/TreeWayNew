using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{

    public ISkill[] skills;

    List<List<string>> skillLists;


    public void SetAllSkill(List<List<string>> data)
    {
        skillLists = data;

        skills = new ISkill[skillLists.Count-1];

        for(int i=0;i< skills.Length;i++)
        {
            SkillData skillData=new SkillData();
            skillData.skillName = skillLists[i + 1][0];
            skillData.skillEffect = skillLists[i + 1][1];
            if(skillLists[i + 1][2]== "-")
            {
                skillData.maxLevel = 999;

            }
            else
            {
                skillData.maxLevel = int.Parse(skillLists[i + 1][2]);

            }

            ISkill iSkill = new ISkill();
            skills[i] = iSkill;
            skills[i].data = skillData;

        }

    }

    public ISkill GetSkill()
    {
        return skills[UnityEngine.Random.Range(0, skills.Length)];
    }

}
