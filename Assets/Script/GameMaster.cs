namespace VRTK.Examples
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	public class GameMaster : MonoBehaviour {
		enum InteractionMode {
			none,
			magnifier,
			platform
		}

		public Parser testCase;
		TestCase tcase;
		private bool dispensing = false;
		private GameObject[] medBags;
		public GameObject OKButton;
		public GameObject leftHand, rightHand;
		public GameObject drugBagPrefab; 
		public Transform[] bagGenratePos;
		VRTK.SDK_BaseController.ControllerHand grabingHand = VRTK.SDK_BaseController.ControllerHand.None;
		InteractionMode mode = InteractionMode.none;
		GameObject grabObject = null;
		public Transform grabObjTransform = null;
		public GameObject readme;
		
		// Magnifier variable
		bool isTouchpadTouching = false;
		Vector2 padLastPos = Vector2.zero;
		public Image[] modeIcons;
		public Sprite searchIcon;

		// Platform variable
		int amount = 0;
		Vector2 padStartPoint;
		int sliderValue = 0;
		GameObject amountPanel;
		Text drugNameTxt;
		Text drugAmountTxt;

        // For debug, HintLine
        HintLine hintLine;

		void Start () {

            // Check controllers has InteractTouch.cs & InteractGrab.cs
			if (leftHand.GetComponent<VRTK_InteractTouch>() == null || leftHand.GetComponent<VRTK_InteractGrab>() == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "GameMaster", "VRTK_InteractTouch and VRTK_InteractGrab", "the Left Controller Alias"));
                return;
            }
			if (rightHand.GetComponent<VRTK_ControllerEvents>() == null && rightHand.GetComponent<VRTK_InteractTouch>() == null || rightHand.GetComponent<VRTK_InteractGrab>() == null)
            {
                VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "GameMaster", "VRTK_InteractTouch and VRTK_InteractGrab", "the Right Controller Alias"));
                return;
            }

            //Setup controller event listeners
            rightHand.GetComponent<VRTK_InteractGrab>().ControllerGrabInteractableObject += new ObjectInteractEventHandler(DoInteractGrab);
            rightHand.GetComponent<VRTK_InteractGrab>().ControllerUngrabInteractableObject += new ObjectInteractEventHandler(DoInteractUngrab);
			rightHand.GetComponent<VRTK_InteractTouch>().ControllerTouchInteractableObject += new ObjectInteractEventHandler(DoInteractTouch);
			rightHand.GetComponent<VRTK_ControllerEvents>().TouchpadTouchStart += new ControllerInteractionEventHandler(DoTouchpadTouchStart);
            rightHand.GetComponent<VRTK_ControllerEvents>().TouchpadTouchEnd += new ControllerInteractionEventHandler(DoTouchpadTouchEnd);
			rightHand.GetComponent<VRTK_ControllerEvents>().TouchpadAxisChanged += new ControllerInteractionEventHandler(DoTouchpadAxisChanged);
            leftHand.GetComponent<VRTK_InteractGrab>().ControllerGrabInteractableObject += new ObjectInteractEventHandler(DoInteractGrab);
            leftHand.GetComponent<VRTK_InteractGrab>().ControllerUngrabInteractableObject += new ObjectInteractEventHandler(DoInteractUngrab);
			leftHand.GetComponent<VRTK_ControllerEvents>().TouchpadTouchStart += new ControllerInteractionEventHandler(DoTouchpadTouchStart);
            leftHand.GetComponent<VRTK_ControllerEvents>().TouchpadTouchEnd += new ControllerInteractionEventHandler(DoTouchpadTouchEnd);
			leftHand.GetComponent<VRTK_ControllerEvents>().TouchpadAxisChanged += new ControllerInteractionEventHandler(DoTouchpadAxisChanged);

            // For debug
            hintLine = GetComponentInChildren<HintLine>();
            hintLine.show = false;

        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.F1))
            {
                hintLine.show = !hintLine.show;
            }

        }

        private void DebugLogger(uint index, string action, GameObject target)
        {
            VRTK_Logger.Info("Controller on index '" + index + "' is " + action + " an object named " + target.name);
        }

		private void DoInteractTouch(object sender, ObjectInteractEventArgs e)
        {
            // if (e.target && grabObject == null)	// need to check is med or not...
            // {
			// 	Debug.Log(e.target.gameObject.transform.parent);
			// 	grabObjOriginTransform = e.target.transform;
            // }
        }

        private void DoInteractGrab(object sender, ObjectInteractEventArgs e)
        {
            if (e.target && grabingHand == VRTK.SDK_BaseController.ControllerHand.None && e.target.tag == "MedModel")   // check e is a MedModel and nothing in hands
            {
                // Hide hand model
				ShowHandModel(false, leftHand);
				ShowHandModel(false, rightHand);

                // Duplicate the grabbed med model to controller transform
				GameObject go = Instantiate(e.target, e.target.transform, true);
				go.transform.parent = e.target.transform.parent;
				go.transform.position = e.target.transform.position;
				go.transform.rotation = e.target.transform.rotation;
				go.transform.localScale = e.target.transform.localScale;
				grabObjTransform = go.transform;
				go.GetComponent<VRTK_InteractableObject>().isGrabbable = true;
				go.name = go.name.Substring(0,6);

                // Set grabing hand and switch to magnifier mode
                grabingHand = e.controllerReference.hand;
				mode = InteractionMode.magnifier;
				EnableMenu(grabingHand.ToString());
                grabObject = e.target;
				grabObject.transform.parent = null;
				e.target.transform.parent = null;
				drugNameTxt.text = "請輸入需要的數量:";
            }
			else if(e.target.tag == "MedModel")     // if grab a med model but something in hands, call DoInteractUngrab
			{
				if(e.controllerReference.hand == VRTK.SDK_BaseController.ControllerHand.Left)
					leftHand.GetComponent<VRTK_InteractGrab>().ForceRelease();
				else
					rightHand.GetComponent<VRTK_InteractGrab>().ForceRelease();
			}		
        }

        private void DoInteractUngrab(object sender, ObjectInteractEventArgs e)
        {
            if (e.target && e.target.tag == "MedModel")
            {
				// show hand if is dispensing
				ShowHandModel(dispensing, leftHand);
				ShowHandModel(dispensing, rightHand);
				ShowControllerModel(!dispensing, leftHand);
				ShowControllerModel(!dispensing, rightHand);

				grabingHand = VRTK.SDK_BaseController.ControllerHand.None;
				grabObjTransform = null;
				mode = InteractionMode.none;
				Destroy(grabObject);
				UnableMenu();
                grabObject = null;
				drugNameTxt.text = "";
				
            }
        }

		private void DoTouchpadTouchStart(object sender, ControllerInteractionEventArgs e)
        {
			if(e.controllerReference.hand == grabingHand) {
				isTouchpadTouching = true;
				if(mode == InteractionMode.magnifier) padLastPos = Vector2.zero - e.touchpadAxis;
				else if(mode == InteractionMode.platform) {padStartPoint = e.touchpadAxis; sliderValue = 0;}
			}
        }

        private void DoTouchpadTouchEnd(object sender, ControllerInteractionEventArgs e)
        {
			if(e.controllerReference.hand == grabingHand) {
				isTouchpadTouching = false;
				if(mode == InteractionMode.platform) {
					amount += sliderValue;
				}
			}
        }

		private void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
			if(e.controllerReference.hand == grabingHand) {
				if(mode == InteractionMode.magnifier) Magnifier(e);
				else if(mode == InteractionMode.platform) Platform(e);
			}
        }

		public void SwitchMode(int m) {
			if(m == 1) {
				mode = InteractionMode.magnifier;
				amountPanel.SetActive(false);
			}
			else if(m == 2) {
				mode = InteractionMode.platform;
				amount = 0;
				sliderValue = 0;
				amountPanel.SetActive(true);
				drugAmountTxt.text = "0";
			}
		}

		public void Reset() 
		{
			if(mode == InteractionMode.magnifier)
			{
				grabObject.transform.localScale = grabObjTransform.localScale;
			}
			else if(mode == InteractionMode.platform)
			{
				drugAmountTxt.text = "0";
				amount = 0;
				sliderValue = 0;
			}
		}

		public void SetPointerActive(bool b) {
			rightHand.GetComponent<VRTK_UIPointer>().enabled = b;
			rightHand.GetComponent<VRTK_Pointer>().enabled = b;
			rightHand.GetComponent<VRTK_StraightPointerRenderer>().enabled = b;
			leftHand.GetComponent<VRTK_UIPointer>().enabled = b;
			leftHand.GetComponent<VRTK_Pointer>().enabled = b;
			leftHand.GetComponent<VRTK_StraightPointerRenderer>().enabled = b;
		}

		void ShowHandModel(bool b, GameObject hand) {
			hand.transform.Find("model").gameObject.SetActive(b);
		}

		void ShowControllerModel(bool b, GameObject hand) {
			GameObject g = hand.transform.parent.Find("Model").gameObject;
			g.SetActive(b);
			MeshRenderer[] meshes = g.GetComponentsInChildren<MeshRenderer>();
			foreach(MeshRenderer m in meshes)
				m.enabled = true;
		}

		private void EnableMenu(string grabHand) {
			if(grabHand == "Right") {
				rightHand.transform.Find("RadialMenu").gameObject.SetActive(false);
				amountPanel = rightHand.transform.Find("med_amount").gameObject;
				ShowControllerModel(true, leftHand);
				leftHand.transform.Find("RadialMenu").gameObject.SetActive(true);
				leftHand.transform.Find("med_amount").gameObject.SetActive(false);
			}
			else if(grabHand == "Left") {
				leftHand.transform.Find("RadialMenu").gameObject.SetActive(false);
				amountPanel = leftHand.transform.Find("med_amount").gameObject;
				ShowControllerModel(true, rightHand);
				rightHand.transform.Find("RadialMenu").gameObject.SetActive(true);
				rightHand.transform.Find("med_amount").gameObject.SetActive(false);
			}

			Text[] texts = amountPanel.GetComponentsInChildren<Text>();
			if(texts[0].name == "name") {
				drugNameTxt = texts[0];
				drugAmountTxt = texts[1];
			}
			else if(texts[0].name == "amount") {
				drugAmountTxt = texts[0];
				drugNameTxt = texts[1];
			}
		}

		private void UnableMenu() {
			rightHand.transform.Find("RadialMenu").gameObject.SetActive(false);
			rightHand.transform.Find("med_amount").gameObject.SetActive(false);
			leftHand.transform.Find("RadialMenu").gameObject.SetActive(false);
			leftHand.transform.Find("med_amount").gameObject.SetActive(false);
			foreach(Image img in modeIcons) {
				img.sprite = searchIcon;
			}
		}
		
		private void Magnifier(ControllerInteractionEventArgs e) {
			if(isTouchpadTouching && grabObject != null) {
				Vector2 nowPos = Vector2.zero - e.touchpadAxis;
				float scaleFator = SignedAngleTo(padLastPos, nowPos);
				if(grabObject.transform.localScale.x > 0)
					grabObject.transform.localScale -= new Vector3(scaleFator, scaleFator, scaleFator) * 0.01f * grabObject.transform.localScale.x;
				padLastPos = nowPos;

			}
		}
		private float SignedAngleTo(Vector2 a, Vector2 b)
		{
			return Mathf.Atan2(a.x * b.y - a.y * b.x, a.x * b.x + a.y * b.y) * Mathf.Rad2Deg;
		}

		private void Platform(ControllerInteractionEventArgs e) {
			Vector2 nowPos = e.touchpadAxis - padStartPoint;
            sliderValue = (int)(nowPos.x * 10);
            if (amount + sliderValue < 0) { amount = 0; sliderValue = 0; }
            drugAmountTxt.text = (amount + sliderValue).ToString();
		}

		public void StartDispense() {

			dispensing = true;
			readme.SetActive(true);

			// 確認user answer有無疑義
			tcase = testCase.NowTestCase();
			foreach(Medicine m in tcase.medList) {
				foreach(bool b in m.reply)
					if(b) {
						Debug.Log("有疑義");
						testCase.showCheckpoint();
						return;
					}
			}

			// Set controller & hand model
			SetPointerActive(false);
			ShowHandModel(true, leftHand);
			ShowHandModel(true, rightHand);
			ShowControllerModel(false, leftHand);
			ShowControllerModel(false, rightHand);

			// generate drug bag
			OKButton.SetActive(true);
			Vector3 genVec = bagGenratePos[1].position - bagGenratePos[0].position;
			genVec /= tcase.medList.Count;
			medBags = new GameObject[tcase.medList.Count];
			for(int i=0; i<tcase.medList.Count; ++i) {
				//Debug.Log(tcase.medList[i].index);
				medBags[i] = Instantiate(drugBagPrefab, bagGenratePos[0].position+genVec*i, Quaternion.identity);
				medBags[i].transform.Find("Info").GetComponentInChildren<Text>().text = tcase.medList[i].medName + "\n數量: " + tcase.medList[i].amt + "\n位置: " + tcase.medList[i].position;
                medBags[i].transform.localScale = Vector3.one * 0.75f;

                // For debug, generate hitLine
                if (hintLine != null) hintLine.GenrateLine(tcase.medList[i].index);
			} 
			
			testCase.transform.parent.gameObject.SetActive(false);
		}

		public void EndDispense() {

			dispensing = false;
			readme.SetActive(false);

			// set controller & hand model
			ShowHandModel(false, leftHand);
			ShowHandModel(false, rightHand);
			ShowControllerModel(true, leftHand);
			ShowControllerModel(true, rightHand);

			// pass user Dispense result
			for(int i=0; i<medBags.Length; i++) {
				MedBag m = medBags[i].GetComponent<MedBag>();
				
				if(tcase.medList[i].index.Contains(m.index))
					tcase.medList[i].replyIndex = true;
				if(tcase.medList[i].amt.Contains(m.amount))
					tcase.medList[i].replyAmount = true;
				//Debug.Log("INDEX: " + tcase.medList[i].replyIndex + "AMT: " + tcase.medList[i].replyAmount);
			}

			// destroy med bags of this testcase
			foreach (GameObject g in medBags) {
				Destroy(g);
			}

            // For debug, clear hint lines
            if (hintLine != null) hintLine.ClearList();


			testCase.showCheckpoint();
		}
		public string drugAmount() {
			return drugAmountTxt.text;
		}
	}
}

