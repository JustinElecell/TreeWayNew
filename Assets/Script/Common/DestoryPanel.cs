using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryPanel : MonoBehaviour
{
    public Transform[] tran;

    private void OnDisable()
    {
        for(int x=0;x< tran.Length;x++)
        {
            for(int i=0;i<tran[x].childCount;i++)
            {
                Destroy(tran[x].GetChild(i).gameObject);
            }
        }

    }
}
