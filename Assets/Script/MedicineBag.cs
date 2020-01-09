using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicineBag : MonoBehaviour {

    private Vector3 lastPos;

    private float shakeDelayTimer = 0;
    private bool isShaking = false;
    private float shakeLimit = 2f;

    public int type;
    public float shakeDelay = 0.3f;
    public float unit;

    public float targetAmount;
    private float amountInBag;

    private void Awake()
    {
        ShakeDetecter.Shaked += onMedicineShake;
    }

    private void OnDestroy()
    {
        ShakeDetecter.Shaked -= onMedicineShake;
    }

	private void Start()
	{
		lastPos = transform.position;
	}

    void Update()
    {
		Debug.Log ((lastPos.y - transform.position.y) / Time.deltaTime);

		if (!isShaking && (lastPos.y - transform.position.y) / Time.deltaTime > shakeLimit)
        {
            ShakeDetecter.makeShakedEvent("Bag", type, -unit);
            isShaking = true;
        }
        else if (isShaking)
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

    private void onMedicineShake(string shakedItemType, int ItemIndex, float unit)
    {
        if(ItemIndex == type)
        {
            amountInBag += unit;
            GetComponentInChildren<TextMesh>().text = (char)('A' + type) + "\n" + amountInBag;
        }
    }
}
