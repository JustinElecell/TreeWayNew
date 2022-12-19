using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleCheck : MonoBehaviour
{
    public List<GameObject> lists = new List<GameObject>();
    public int MP;
    private void OnEnable()
    {
        StartCoroutine(IEwait());

    }
    IEnumerator IEwait()
    {
        yield return new WaitForSeconds(0.5f);
        float inttmp = (GamePlayManager.instance.player.stetas.player.Atk / (10 + 5 * Mathf.Min(lists.Count, 5)));

        var damage = Mathf.Max(inttmp * MP,1);
        Debug.Log(MP);
        Debug.Log(lists.Count+" / "+ damage);
        foreach (var data in lists)
        {
            data.GetComponent<Stetas>().TakeDamage(damage);
        }

        Destroy(this.gameObject);
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag=="Enemy")
        {
            lists.Add(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            lists.Remove(other.gameObject);
        }
    }
}
