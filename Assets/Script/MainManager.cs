using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
public class MainManager : MonoBehaviour
{
    public static MainManager instance;


    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
            Application.targetFrameRate = 120;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public bool TestFlag;
    public List<string> TargetCharater;

    public int targetSkillNo = 1;
    public SO_Iteam[] skillIteams;



}
