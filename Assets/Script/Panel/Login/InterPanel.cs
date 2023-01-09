using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InterPanel : MonoBehaviour
{
    public Button interButton;
    public Text ID;


    public void Init(string id)
    {
        interButton.onClick.AddListener(() => {
            LoginManager.instance.Load();
        });

        ID.text = "ID " + id;
        this.gameObject.SetActive(true);



    }
}
