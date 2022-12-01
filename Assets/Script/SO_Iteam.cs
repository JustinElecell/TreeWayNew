using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "Iteam State/Data")]



public class SO_Iteam : ScriptableObject
{
    public enum IteamType
    {
        武具,
        魔法,
        召喚

    }

    public string IteamName = "";
    public string IteamString = "";
    public Sprite IteamImage;
    public IteamType type;

    public float Hp;
    public float Mp;
    public float Atk;
    public float Atk_wait;
    public float Speed;

    public GameObject IteamPerfab;


}
