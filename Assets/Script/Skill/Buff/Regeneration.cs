using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Regeneration : MonoBehaviour
{
    Stetas stetas;
    private void OnEnable()
    {
        stetas = this.GetComponent<Stetas>();

    }
    private void OnDisable()
    {

        var tmp =GamePlayManager.instance.SetIteam(stetas.iteam, stetas.roadNo, false);


        Vector3 tmpVec = tmp.transform.localPosition;
        tmpVec.x = this.gameObject.transform.localPosition.x;

        tmp.transform.localPosition = tmpVec;
        Destroy(this);
    }
}
