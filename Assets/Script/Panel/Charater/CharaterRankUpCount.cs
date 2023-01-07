using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharaterRankUpCount : MonoBehaviour
{
    public Text UpcountText;
    int UpRank;


    public void SetUpRank(int Rank)
    {
        if(Rank>=1)
        {
            UpcountText.text = "+" + (Rank).ToString();
            this.gameObject.SetActive(true);
        }
        else
        {
            this.gameObject.SetActive(false);

        }
    }
}
