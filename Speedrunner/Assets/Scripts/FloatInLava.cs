using UnityEngine;
using System.Collections;

public class FloatInLava : MonoBehaviour {

	public float horizontal;
	public float vertical;
	public float speed;
	private bool up;
	private bool left;
	private float leftBound;
	private float upBound;
	private float rightBound;
	private float lowBound;

	// Use this for initialization
	void Start () {
		leftBound = transform.position.x - horizontal;
		rightBound = transform.position.x + horizontal;
		upBound = transform.position.y + vertical;
		lowBound = transform.position.y - vertical;
	}
	
	// Update is called once per frame
	void Update () {
		if(left) {
			if(transform.position.x <= leftBound) {
				left = false;
			}
			else {
				transform.position = new Vector3(transform.position.x - speed*Time.deltaTime, transform.position.y, transform.position.z);
			}
		}
		else {
			if(transform.position.x >= rightBound) {
				left = true;
			}
			else {
				transform.position = new Vector3(transform.position.x + speed*Time.deltaTime, transform.position.y, transform.position.z);
			}
		}

		if(up) {
			if(transform.position.y >= upBound) {
				up = false;
			}
			else {
				transform.position = new Vector3(transform.position.x, transform.position.y + speed*Time.deltaTime, transform.position.z);
			}
		}
		else {
			if(transform.position.y <= lowBound) {
				up = true;
			}
			else {
				transform.position = new Vector3(transform.position.x, transform.position.y - speed*Time.deltaTime, transform.position.z);
			}
		}
		
	}
}
