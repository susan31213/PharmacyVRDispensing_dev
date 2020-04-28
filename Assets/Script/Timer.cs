using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {

    private float time;
    private bool counting = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (counting)
            time += Time.deltaTime;

        Debug.Log(time);
	}

    public void StartCount()
    {
        time = 0;
        counting = true;
        Debug.Log("start");
    }

    public float EndCount()
    {
        counting = false;
        Debug.Log("end");
        return time;
    }

    public float getTime()
    {
        return time;
    }
}
