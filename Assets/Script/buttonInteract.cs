using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class buttonInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public Button b;
	Text[] textlist;

	public void OnPointerEnter(PointerEventData eventData){
		textlist = b.GetComponentsInChildren<Text>();
		foreach(Text t in textlist)
			t.color = Color.red;
	}
	public void OnPointerExit(PointerEventData eventData){
		textlist = b.GetComponentsInChildren<Text>();
		foreach(Text t in textlist)
			t.color = Color.black;
	}

	public void TaskOnClick(){
		textlist = b.GetComponentsInChildren<Text>();
		foreach(Text t in textlist)
			t.color = Color.black;
	}

	public void RestartApplication() {
		SceneManager.LoadScene("Main", LoadSceneMode.Single);
	}

	public void ExitApplication() {
		Application.Quit();
	}

}
