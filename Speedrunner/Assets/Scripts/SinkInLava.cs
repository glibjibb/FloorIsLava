using UnityEngine;
using System.Collections;

public class SinkInLava : MonoBehaviour {

	public float speed;
	public float minY;
	private bool isSinking = false;

	public void TriggerSink() {
		isSinking = true;
	}

	void Update() {
		if (isSinking && transform.position.y > minY) {
			transform.position = new Vector3(transform.position.x, transform.position.y - speed*Time.deltaTime, transform.position.z);
		}
	}
}
