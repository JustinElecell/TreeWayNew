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
            Init();
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

    public Button[] TargetSkillButton;

    void Init()
    {
        for(int i=0;i<TargetSkillButton.Length;i++)
        {

            int no = i;
            TargetSkillButton[i].onClick.AddListener(() => {

                for(int r=0;r<5;r++)
                {
                    TargetSkillButton[r].gameObject.GetComponent<Image>().color = Color.white;
                }
                TargetSkillButton[no].gameObject.GetComponent<Image>().color = Color.green;
                targetSkillNo = no;
            
            
            });
        }
    }

}
