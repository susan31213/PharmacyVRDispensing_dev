namespace VRTK.Examples
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;

	public class OKButton : VRTK_InteractableObject {

		public GameMaster GM;
		public GameObject UICanvas;

		private float timer = 0f;
		public bool isDelay = false;

		public override void StartUsing(VRTK_InteractUse currentUsingObject = null)
		{
			if(!isDelay) {
				base.StartUsing(usingObject);
				UICanvas.SetActive(true);
				GM.SetPointerActive(true);
				GM.EndDispense();
				gameObject.SetActive(false);
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

