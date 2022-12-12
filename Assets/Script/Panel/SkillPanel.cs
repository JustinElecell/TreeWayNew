using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPanel : MonoBehaviour
{
    public Button BuffButtonPerfab;
    public void SkillSet(ISkill skill)
    {
        var tmp=Instantiate(BuffButtonPerfab, this.transform);
        tmp.transform.GetChild(0).GetComponent<Text>().text = skill.data.skillName;
        tmp.transform.GetChild(2).GetComponent<Text>().text = skill.data.skillEffect;

        tmp.onClick.AddListener(() => {
            Time.timeScale = 1;
            GamePlayManager.instance.skillPanel.gameObject.SetActive(false);
            skill.action();

            for(int i=0;i<GamePlayManager.instance.skillPanel.transform.childCount;i++)
            {
                Destroy(GamePlayManager.instance.skillPanel.transform.GetChild(i).gameObject);
            }
        
        });
    }
}
