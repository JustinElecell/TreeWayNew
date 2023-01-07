using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class NoticePanel : MonoBehaviour
{
    public Text mainText;
    public void Notic(string text)
    {
        mainText.text = text;
        this.gameObject.SetActive(true);

    }
}
