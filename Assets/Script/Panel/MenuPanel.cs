using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MenuPanel : MonoBehaviour
{
    public Button[] buttons;
    public GameObject[] panels;


    private void Start()
    {
        for(int i=0;i<buttons.Length;i++)
        {
            int no=i;
            buttons[i].onClick.AddListener(() => { 
                
                for(int r=0;r<panels.Length;r++)
                {
                    panels[r].SetActive(false);
                }

                panels[no].SetActive(true);
            
            });

        }
    }



}
