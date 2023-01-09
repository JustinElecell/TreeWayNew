using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NoticePanel : MonoBehaviour
{


    public static NoticePanel instance;
    public Text mainText;
    public GameObject Panel;

    private void Awake()
    {

        DontDestroyOnLoad(this.gameObject);
        instance = this;
    }
    public void Notic(string text)
    {
        mainText.text = text;
        Panel.SetActive(true);

    }
}
