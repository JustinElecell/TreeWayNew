using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Stetas : MonoBehaviour
{
    public enum Type
    {
        道具,
        敵人,

    }

    public Type type;
    public Image hpBar;

    [Header("道具")]
    public SO_Iteam iteam;

    [Header("敵人")]
    public SO_Enemy enemy;


    public void Damage()
    {

    }

}
