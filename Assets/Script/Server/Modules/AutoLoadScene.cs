using UnityEngine;
using System.Collections;

public class AutoLoadScene : MonoBehaviour {
	
	public float delay = 1.5f;
	public float delay2 = 1.5f;
	public int scene;
	private float startTime;
	
	void Start () {
		//startTime = Time.time + delay;
		StartCoroutine(load());
		
	}
	
	IEnumerator load(){
		yield return new WaitForSeconds(delay);
		startTime = Time.time + delay2;
		AsyncOperation lvl = Application.LoadLevelAdditiveAsync(scene);
		yield return lvl;
		
		while (Time.time < startTime) yield return null;
		Destroy(gameObject);
	}

}
