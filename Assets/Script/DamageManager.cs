using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TweenTools;
public class DamageManager : MonoBehaviour
{
    public Transform damageMonoTran;
    public GameObject textObj;

    public void Close(GameObject obj)
    {
        obj.transform.SetParent(damageMonoTran);
    }
    public void Init()
    {
        for(int i=0;i<120;i++)
        {
            Instantiate(textObj, damageMonoTran);

        }
    }

    //private void Update()
    //{

    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        Debug.Log("000"); 

    //    }
    //}

    public void Damage(GameObject target, int damage)
    {
        var obj = damageMonoTran.GetChild(0).gameObject;
        obj.GetComponent<Text>().text=damage.ToString();
        obj.transform.SetParent(target.transform.parent.transform);
        
        var tnpPos = target.transform.localPosition;
        tnpPos.y += 60;
        obj.transform.localPosition = tnpPos;
        obj.SetActive(true); 
        obj.GetComponent<TweenPosition>().from = tnpPos;
        tnpPos.y += 50;
        obj.GetComponent<TweenPosition>().to = tnpPos;
        obj.SetActive(true);

        obj.GetComponent<TweenPosition>().enabled = true;
        obj.GetComponent<TweenAlpha>().enabled = true;

    }
}
