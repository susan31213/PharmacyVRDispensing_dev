using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class MatNoNamePair{
	public string matNo;
	public string medName;
	public string position;
}

public class dataProcess : MonoBehaviour {

	public List<MatNoNamePair> medPosList;
	
	void Start(){
		initMedList ();
        Debug.Log(getMedName ("361925"));
		//getPosition ("benzBROMARONE 降尿酸 50mg/tab");
	}

	public void initMedList(){
		medPosList = new List<MatNoNamePair> ();
		string s = File.ReadAllText(Application.dataPath + "/Resources/matNoNameList.txt");
		string[] l = s.Split ("\n"[0]);
		for (int i = 1; i < l.Length - 1; i++) {
			string[] temp = l[i].Split ("\t" [0]);
			MatNoNamePair m = new MatNoNamePair();
			m.matNo = temp [0];
			m.medName = temp [1];
			m.position = temp [2];
			medPosList.Add (m);
		}
	}
	public string getMedName(string no){
        no = no.Replace(" ", "");
        no = no.Replace("\t", "");
        MatNoNamePair t = medPosList.Find (x => x.matNo == no);
		if (t == null) {
			//Debug.Log("Material No. Not Found");
			return "";
		}
		//Debug.Log (t.medName);
		return t.medName;
	}

	public string getPosition(string no){
        no = no.Replace(" ", "");
        no = no.Replace("\t", "");
        MatNoNamePair t = medPosList.Find (x => x.matNo == no);
		if (t == null) {
			//Debug.Log("Medicine "+ no +" Not Found");
			return "";
		}
		//Debug.Log (t.position);
		return t.position;
	}
}
