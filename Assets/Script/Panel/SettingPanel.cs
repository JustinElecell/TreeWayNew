using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SettingPanel : MonoBehaviour
{
    public Button mailBoxButton;
    public GameObject MailPanel;
    private void Start()
    {
        mailBoxButton.onClick.AddListener(()=> {
            MailPanel.gameObject.SetActive(true);


        });
    }


}
