using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "Player State/Data")]

public class SO_Player : ScriptableObject
{
    public string PlayerName = "";
    public int Level;
    public float Hp;
    public float Mp;
    public float REC;
    public float Atk;
    public int Def;

    public int Overfulfil;
    public float AtkUp;
    //public float temporaryAtkUp;
}
