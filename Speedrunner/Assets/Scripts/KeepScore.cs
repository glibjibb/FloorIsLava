using UnityEngine;
using System.Collections;
using System;

public class KeepScore : MonoBehaviour {

	static ArrayList scoreList;
	static ArrayList replays;
	public bool isReplaying;

	void Start() {
		isReplaying = false;
		scoreList = new ArrayList {"","","",""};
		replays = new ArrayList {new Queue(), new Queue(), new Queue(), new Queue()};
	}

	void Awake () {
		DontDestroyOnLoad (this);
		if (FindObjectsOfType (GetType ()).Length > 1)
			Destroy (gameObject);
	}
	
	public void AddScore(int index, string f) {
		scoreList [index] = f;
	}
	public string GetLevelScore(int index) {
		return (string)scoreList [index];
	}
	public void AddReplay(int index, Queue replay) {
		replays [index] = replay;
	}
	public Queue GetReplay(int index) {
		Queue copy = new Queue ((Queue)replays [index]);
		return copy;
	}
}
