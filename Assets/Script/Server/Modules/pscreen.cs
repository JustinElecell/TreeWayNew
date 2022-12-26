using UnityEngine;
using System.Collections;

public class pscreen : MonoBehaviour {
	#if UNITY_EDITOR
	
	public new Camera camera;
	
	public static string ScreenShotName() {
		return string.Format("screen_{0}.png", 
		                     System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
	}
	
	void LateUpdate() {
		if (Input.GetKeyUp(KeyCode.P)) {
			StartCoroutine(screenshot());
		}
	}
	
	IEnumerator screenshot(){
		float origD = camera.depth;
		camera.depth = 100f;
		yield return null;
		ScreenCapture.CaptureScreenshot(ScreenShotName(), 4);
		yield return null;
		camera.depth = origD;
	}
	#endif
}
