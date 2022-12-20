using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShatteringShock : MonoBehaviour
{

    Stetas stetas;
    private void OnEnable()
    {
        stetas = this.gameObject.GetComponent<Stetas>();
    }
    private void OnDisable()
    {
        if(stetas.Hp<=0)
        {
            var tmp = Instantiate(GamePlayManager.instance.skillManager.CheckCircle, this.transform.parent.transform);
            Vector3 tmpVec = tmp.transform.localPosition;
            tmpVec.x = this.gameObject.transform.localPosition.x;

            tmp.transform.localPosition = tmpVec;
            tmp.GetComponent<CircleCheck>().MP = ((int)stetas.iteam.Mp);
            tmp.SetActive(true);
        }

        Destroy(this);
    }


}