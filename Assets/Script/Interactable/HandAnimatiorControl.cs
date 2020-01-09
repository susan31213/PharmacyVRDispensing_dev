using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandAnimatiorControl : MonoBehaviour {

	Animator animator;
	bool isClose = false;
	float nowWeight = 0;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
	}

	public void OpenHand() {
		isClose = false;
	}

	public void CloseHand() {
		isClose = true;
	}
	
	// Update is called once per frame
	void Update () {
		if(isClose && nowWeight < 1) {
			animator.SetLayerWeight(animator.GetLayerIndex("ThumbDown"), nowWeight);
			animator.SetLayerWeight(animator.GetLayerIndex("IndexDown"), nowWeight);
			animator.SetLayerWeight(animator.GetLayerIndex("MiddleDown"), nowWeight);
			animator.SetLayerWeight(animator.GetLayerIndex("RingDown"), nowWeight);
			animator.SetLayerWeight(animator.GetLayerIndex("PinkyDown"), nowWeight);
			nowWeight += Time.deltaTime * 4;
		}
		else if(!isClose && nowWeight > 0) {
			animator.SetLayerWeight(animator.GetLayerIndex("ThumbDown"), nowWeight);
			animator.SetLayerWeight(animator.GetLayerIndex("IndexDown"), nowWeight);
			animator.SetLayerWeight(animator.GetLayerIndex("MiddleDown"), nowWeight);
			animator.SetLayerWeight(animator.GetLayerIndex("RingDown"), nowWeight);
			animator.SetLayerWeight(animator.GetLayerIndex("PinkyDown"), nowWeight);
			nowWeight -= Time.deltaTime * 4;
		}
	}
}
