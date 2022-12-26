using UnityEngine;
using System.Collections;

public class DoorStopper : MonoBehaviour {
	
	public bool stop;
	
	void OnBecameInvisible(){
		//Debug.Log("invis");
		stop = false;
	}
	void OnBecameVisible(){
		//Debug.Log("vis");
		stop = true;
	}
}
