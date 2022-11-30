using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    public static MainManager instance;

    public  GamePlayManager gamePlayManager;

    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
            Application.targetFrameRate = 60;

        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public RectTransform Rect;





}
