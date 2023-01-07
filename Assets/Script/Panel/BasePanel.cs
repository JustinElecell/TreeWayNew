using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    public virtual void Init() { }
    public virtual void ResetPanel() { }
    public void DerstorChild(Transform tran)
    {
        for (int i = 0; i < tran.childCount; i++)
        {
            Destroy(tran.GetChild(i).gameObject);
        }
    }
}
