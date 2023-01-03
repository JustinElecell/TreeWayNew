using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using EleCellLogin;
using SimpleJSON;
public class MainManager : MonoBehaviour
{
    public static MainManager instance;

    public enum ServerData
    {
        CardPool,
        Charater,
        Item
    }

    public Dictionary<ServerData, JSONClass> ServerData_Json = new Dictionary<ServerData, JSONClass>();
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

    public List<SO_Iteam> AllItemList = new List<SO_Iteam>();

    public DrawChancePanel DrawChancePanel;

    public void TEST()
    {
        GameServer.instance.Test(Test2);
    }
    void Test2(string x ,string y)
    {

    }
}
