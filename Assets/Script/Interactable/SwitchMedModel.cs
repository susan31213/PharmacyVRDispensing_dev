namespace VRTK.Examples
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class SwitchMedModel : VRTK_InteractableObject {

		// Will only show first object.
		public GameObject[] medObjects;

		private float timer = 0f;
		public bool isDelay = false;

		public override void StartUsing(VRTK_InteractUse currentUsingObject = null)
		{
			if(!isDelay) {
				medObjects[0].SetActive(true);
				for(int i=1; i<medObjects.Length; i++) {
					medObjects[i].SetActive(false);
				}
				isDelay = true;
				timer = 0.5f;
			}
		}

		protected override void Update () {
			base.Update();
			if(timer > 0) {
				timer -= Time.deltaTime;
			}
			else {
				isDelay = false;
			}
		}
	}
}

