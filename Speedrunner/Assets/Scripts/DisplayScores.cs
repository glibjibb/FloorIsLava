using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DisplayScores : MonoBehaviour {

	public Text level1, level2, level3, level4;
	private GameObject scoreKeeper;

	// Use this for initialization
	void Start () {
		Cursor.visible = true;
		scoreKeeper = GameObject.Find ("ScoreKeeper");
		level1.text = "Level 1: " + scoreKeeper.GetComponent<KeepScore> ().GetLevelScore (0);
		level2.text = "Level 2: " + scoreKeeper.GetComponent<KeepScore> ().GetLevelScore (1);
		level3.text = "Level 3: " + scoreKeeper.GetComponent<KeepScore> ().GetLevelScore (2);
		level4.text = "Level 4: " + scoreKeeper.GetComponent<KeepScore> ().GetLevelScore (3);

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape))
			Application.Quit ();
	}

	public void Replay(int level) {
		scoreKeeper.GetComponent<KeepScore>().isReplaying = true;
		SceneManager.LoadScene (level);
	}
	public void Restart() {
		scoreKeeper.GetComponent<KeepScore>().isReplaying = false;
		SceneManager.LoadScene (1);
	}
}
