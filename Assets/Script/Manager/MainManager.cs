using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MainManager : MonoBehaviour
{
    public static MainManager instance;

    public  GamePlayManager gamePlayManager;

    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
            Application.targetFrameRate = 120;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public RectTransform Rect;

    public int TargetCharaterNo=1;

    public SO_Iteam[] Iteams;
}
