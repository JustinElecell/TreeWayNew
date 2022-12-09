using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MainManager : MonoBehaviour
{
    public static MainManager instance;


    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
            Application.targetFrameRate = 120;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public List<string> TargetCharater;

    public SO_Iteam[] Iteams;
}
