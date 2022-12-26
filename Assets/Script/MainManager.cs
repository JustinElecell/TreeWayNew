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
            DontDestroyOnLoad(this.gameObject);
            Debug.Log(MjSave.instance.playerID);
            if(MjSave.instance.playerID!="")
            {
                ID.text = "ID : " + MjSave.instance.playerID.ToString();
            }
            else
            {
                ID.text = "ID : Null";
            }
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

    public Text ID;

}
