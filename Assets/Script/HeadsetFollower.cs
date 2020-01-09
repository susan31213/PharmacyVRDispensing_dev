using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadsetFollower : MonoBehaviour {

	public Transform target;
	[SerializeField]
	private float delta;
	
	// Update is called once per frame
	void Update () {
		transform.rotation = Quaternion.Lerp (transform.rotation, target.rotation, delta);
	}
}
