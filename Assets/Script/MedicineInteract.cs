using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicineInteract : MonoBehaviour {

    private Vector3 lastPos;
    
    private float shakeDelayTimer = 0;
    private bool isShaking = false;
    private float shakeLimit = 2f;

    public int type;
    public float shakeDelay = 0.3f;
    public float unit;

	private void Start()
	{
		lastPos = transform.position;
	}
	
	void Update () {

		if (!isShaking && (lastPos.y - transform.position.y) / Time.deltaTime > shakeLimit)
        {
            ShakeDetecter.makeShakedEvent("Medicine", type, unit);
            isShaking = true;
        }
        else if(isShaking)
        {
            if (shakeDelayTimer > shakeDelay)
            {
                shakeDelayTimer = 0;
                isShaking = false;
            }
            else
                shakeDelayTimer += Time.deltaTime;
        }
		lastPos = transform.position;
    }
}
