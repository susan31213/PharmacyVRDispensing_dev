using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerMedShow : MonoBehaviour {
	
	public float showThreshold;
	GameObject med;

	void Start() {
		med = transform.Find("Med").gameObject;
	}
	void Update () {
		if(transform.localPosition.y < showThreshold)
			med.SetActive(true);
		else
			med.SetActive(false);
	}
}
