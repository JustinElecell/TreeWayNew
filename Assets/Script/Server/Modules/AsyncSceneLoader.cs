using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class AsyncSceneLoader : MonoBehaviour {

	[System.Serializable]
	public struct SceneData	{
		public string name;
		public bool additive;
	}

	public SceneData[] scenes;

	IEnumerator Start () {
		MjInit.audioLoaded = true;
		for (int i = 0; i < scenes.Length; i++) {
			if (scenes[i].additive)
				yield return SceneManager.LoadSceneAsync (scenes[i].name,LoadSceneMode.Additive);
			else
				yield return SceneManager.LoadSceneAsync (scenes[i].name);	
		}
	}

}

