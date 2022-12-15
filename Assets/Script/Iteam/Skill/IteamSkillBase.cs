using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IteamSkillBase : MonoBehaviour
{
    public Stetas stetas;
    public SkillAttackType skillAttackType;
    public enum SkillAttackType
    {
        連擲,
        擊退,
        同時攻擊三條路,
        擊中時額外隨機攻擊
    }
    public virtual void SkillEffect()
    {

    }
    public virtual void SkillEffect(GameObject obj)
    {

    }
}
