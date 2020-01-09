namespace VRTK.Examples
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	public class MedBag : MonoBehaviour {

		GameMaster gameMaster;
		VRTK_InteractGrab leftHand, rightHand;
		Transform medInBag = null;
		Text amtTxt;

		public string index = "-1", amount = "0";

		void Start() {
			gameMaster = GameObject.Find("GameMaster").GetComponent<GameMaster>();
			leftHand = GameObject.Find("LeftController").GetComponent<VRTK_InteractGrab>();
			rightHand = GameObject.Find("RightController").GetComponent<VRTK_InteractGrab>();
			amtTxt = transform.Find("Amount").GetComponentInChildren<Text>();
		}

		void Update() {
			amtTxt.text = amount;
		}

		void OnCollisionEnter(Collision col) {
			if(col.gameObject.tag == "MedModel" && col.gameObject.name.Length == 6) {
				index = col.gameObject.name;
				amount = gameMaster.drugAmount();

				GameObject go = Instantiate(col.gameObject, transform.position, gameMaster.grabObjTransform.rotation);
				Destroy(go.GetComponent<VRTK_InteractControllerAppearance>());
				Destroy(go.GetComponent<VRTK_InteractableObject>());
				Destroy(go.GetComponent<FixedJoint>());
				Destroy(go.GetComponent<Rigidbody>());
				BoxCollider[] bc = go.GetComponentsInChildren<BoxCollider>();
				foreach (BoxCollider b in bc) {
					Destroy(b);
				}
				if(medInBag != null) {
					Destroy(medInBag.gameObject);
				}
				medInBag = go.transform;
				go.transform.position += new Vector3(0, 0.05f, 0);
				go.transform.localScale = 0.8f * gameMaster.grabObjTransform.localScale;
				go.transform.parent = transform;
				leftHand.ForceRelease();
				rightHand.ForceRelease();

				Destroy(col.gameObject);
			}
		}
	}
}
