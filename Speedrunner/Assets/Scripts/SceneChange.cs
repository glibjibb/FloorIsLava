using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour {

	void Update() {
		if (Input.GetKeyDown (KeyCode.Escape))
			Application.Quit ();
	}

	public void LoadScene(string scene) {
		SceneManager.LoadScene (scene);
	}
}
