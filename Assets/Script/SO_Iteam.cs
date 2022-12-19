using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[CreateAssetMenu(fileName = "New Data", menuName = "Iteam State/Data")]



public class SO_Iteam : ScriptableObject
{
    public enum IteamType
    {
        武具,
        魔法,
        召喚
    }

    public enum AttributesType
    {
        打,
        刃,
        火,
        水,
        風,
        雷,
        No= 100
    }


    public string IteamName = "";
    public string IteamString = "";
    public Sprite IteamImage;
    public IteamType type;
    public AttributesType attributesType;

    public float Hp;
    public float BaseHp;
    public float Mp;
    public float BaseMp;
    public float Atk;
    public float Atk_wait;
    public float Speed;
    public float atkUp;

    public GameObject IteamPerfab;


}
