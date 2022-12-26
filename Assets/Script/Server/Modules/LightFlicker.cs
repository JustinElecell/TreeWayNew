using UnityEngine;
using System.Collections;

public class LightFlicker : MonoBehaviour {
	
	public AnimationCurve[] curve = new AnimationCurve[3];
	public float speed;
	
	private Light myLight;
	private float oriIntensity;
	
	private float timer = 0f;
	private int curveIndex;
	
	void Start () {
		myLight = GetComponent<Light>();
		oriIntensity = myLight.intensity;
	}
	
	void Update () {
		
		myLight.intensity = curve[curveIndex].Evaluate(timer) * oriIntensity;
		
		timer += Time.deltaTime * speed;
		if (timer > 1f) {
			timer = 0f;
			curveIndex = Random.Range(0,curve.Length);
		}
	}
}
