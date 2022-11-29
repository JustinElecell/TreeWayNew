using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "Enemy State/Data")]

public class SO_Enemy : ScriptableObject
{
    public string name = "";
    public string explanation = "";



    public float generationRate;
    public float mp;
    public float hp;

    public int speed;
    public int atk;
    public float atk_Interval;


    public GameObject enemyPerfab;
}
