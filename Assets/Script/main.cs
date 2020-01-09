using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;

public class Medicine {
	/*	p1	未依肝腎功能調整劑量 
		p2	劑量過高
		p3	劑量過低
		p4 	使用頻次錯誤
		p5	交互作用
		p6	使用禁忌
		p7	重複用藥
		p8	有過敏史
		p9	與適應症不符
		p10	治療天數不適當
		p11	數量錯誤
	*/
	public string index;
	public string medName;
	public string usage;
	public string amt_per_time;
	public string unit;
	public string freq;
	public string day;
	public string amt;
	public bool[] problem = new bool[] {false,false,false,false,false,false,false,false,false,false,false};
	public bool[] reply = new bool[] {false,false,false,false,false,false,false,false,false,false,false};
	public bool replyIndex = false;
	public bool replyAmount = false;
	public string comment; //修正資訊
	public string position;

	public void answerUpdate(List<Toggle> toggleList){
		for (int i = 0; i < 11; i++) {
			if (toggleList [i].isOn) {
				reply [i] = true;
			}else{
				reply [i] = false;
			}
		}
		// for (int i = 0; i < 11; i++) {
		// 	Debug.Log (i + ":" + reply [i]);
		// }
	}
}

class Patient {
	public string name;
	public string date;
	public string age;
	public string no;
	public string bagNum;
	public List<string> diagnosis = new List<string>();
	public string weight;
	public string department;
	public List<string> kidney = new List<string>(); 
	public string allergy;
	public List<string> otherPrescription = new List<string>();
}

public class TestCase {
	public int index;
	public List<string> medIndex = new List<string>();
	public List<Medicine> medList = new List<Medicine>();
    public bool isBad = false;
	public float timer;
    public bool correct;
}


public class main : MonoBehaviour {

	public int testPeriod = 5;

	int medProfileNum = 9;
	int problemNum = 11;
    int maxMedAmt = 12;
	int selectedMedIndex;
	List<TestCase> testedRecord;
	TestCase tcase;
	private Button med1, med2, med3, med4, med5, med6, med7, med8, med9, med10, med11, med12;
	private Toggle t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11;
	public Text diagnose;
	List<Button> buttonList;
	List<Toggle> toggleList;
	List<Patient> plist;//start from 0
	List<String> resultList;
	int resultShowingIndex = -1;
	public GameObject mainP;
	public GameObject resultP;
	public GameObject checkpointP;
    public GameObject chooseP;
	public dataProcess dp;
	public Text resultTxt;
	public Text checkpointTxt;
	private string difficulty;
	private float timer = 0;
	private bool startTimer = false;

    //medPanel
    private RectTransform canvTrans;
    private Transform top;
    private Transform bottom;
    private float altSize;




    void Start(){
		testedRecord = new List<TestCase>();
		plist = new List<Patient>();
		resultList = new List<String>();
        canvTrans = this.transform.parent.GetComponent<RectTransform>();
        //top = this.transform.parent.Find("Main Panel").Find("panelTop");
        //bottom = this.transform.parent.Find("Main Panel").Find("panelBottom");
        top = mainP.transform.Find("panelTop");
        bottom = mainP.transform.Find("panelBottom");
        top.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        bottom.GetComponent<RectTransform>().localPosition = new Vector3(190, -200, 0);
        buttonList = new List<Button> { med1, med2, med3, med4, med5, med6, med7, med8, med9, med10, med11, med12 };
        toggleList = new List<Toggle> { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11 };
        for (int i = 0; i < maxMedAmt; i++){
            buttonList[i]= top.Find(i.ToString()).GetComponent<Button>();
        };
        for(int i = 0; i<problemNum; i++) {
            toggleList[i]=chooseP.transform.Find("p" + (i+1).ToString()).GetComponent<Toggle>();
        };
        
    }

	void updateMed(List<Medicine> list){
        if (list.Count > 6) {
            int amt = list.Count - 6;
            altSize = amt * 20f;
            canvTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 440+altSize);
            top.GetComponent<RectTransform>().localPosition = new Vector3(0,altSize / 2,0);
            bottom.GetComponent<RectTransform>().localPosition = new Vector3(190, -210-altSize / 2, 0);
        };
		for (int i = 0; i < 12; i++) {
			Text[] textList = buttonList[i].GetComponentsInChildren<Text>();
			if (i >= list.Count) {
				foreach(Text t in textList)
					t.text = "";
				buttonList [i].interactable = false;
			}else{
				textList[0].text = list[i].medName; 
				textList[1].text = list[i].usage;
				textList[2].text = list[i].amt_per_time + " " + list[i].unit;
				textList[3].text = list[i].freq;
				textList[4].text = "x"+list[i].day;
				textList[5].text = list[i].amt + " " + list[i].unit;
				buttonList [i].interactable = true;
			}
		}
	}

    public void medPanelReset()
    {
        canvTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 440);
        top.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
        bottom.GetComponent<RectTransform>().localPosition = new Vector3(190, -200, 0);
    } 

	void loadPatient(){
		string s = File.ReadAllText(Application.dataPath + "/Resources/" + difficulty + "/patient.txt", Encoding.UTF8);
		string[] l = s.Split ("\n"[0]);
		for (int i = 1; i < l.Length-1; i++){
			string[] data = l [i].Split ("	"[0]);
			Patient p = new Patient();
			p.name = data[1];
			p.date = data[2];
			p.age = data[3];
			p.no = data[4];
			p.bagNum = data[5];
			for(int j = 6; j < 16; j++){
				if(data[j] != "no"){
					p.diagnosis.Add(data[j]);
				}
			}
			p.weight = data[16];
			p.department = data[17];
			for(int j = 18; j < 21; j++){
				if(data[j] != "no"){
					p.kidney.Add(data[j]);
				}
			}
			p.allergy = data[21];
			string otherPres = File.ReadAllText(Application.dataPath + "/Resources/" + difficulty + "/other/otherprescription_"+data[0]+".txt", Encoding.UTF8);
			string[] ops = otherPres.Split ("\n"[0]);
			for(int n = 1; n < ops.Length-1; n++){
				string[] temp = ops[n].Split ("	"[0]);
				string op = "";
				for(int k = 2; k<temp.Length; k++)
					op = op+temp[k]+"	";
				p.otherPrescription.Add(op);
			}
			plist.Add(p);
		}
	}

	void loadcase(){
		int caseNum = plist.Count;
		if(testedRecord.Count == testPeriod){
			Debug.Log("All Tested!");
			answerCheck("all");
			return;
		}
		string fname = "/testcase/testcase_";
		int ranIndex = casePicker(caseNum);
		string ran = (ranIndex+1).ToString();
		string fileName = fname + ran;
		string s = File.ReadAllText(Application.dataPath + "/Resources/" + difficulty + fileName + ".txt", Encoding.UTF8);
		string[] l = s.Split ("\n"[0]);
		patientProfileUpdate(plist[ranIndex]);
		tcase = new TestCase ();
		tcase.index = ranIndex;
        tcase.isBad = false;
        tcase.correct = false;
		for (int i = 1; i < l.Length-1; i++) {//-1 because last line is empty
			string[] data = l [i].Split ("\t"[0]);
			Medicine m = new Medicine ();
			m.index = data[1];
			m.medName = data [2];
			m.usage = data[3];
			m.amt_per_time = data[4];
			m.unit = data[5];
			m.freq = data[6];
			m.day = data[7];
			m.amt = data[8];
			m.comment = data [medProfileNum + problemNum];
			m.position = dp.getPosition(m.index);
			for (int j = 0; j < problemNum; j++) {
				if (data [j + medProfileNum] == "0")
					m.problem [j] = false;
				else
					m.problem [j] = true;	
			}
			tcase.medList.Add (m);
		}
		updateMed (tcase.medList);
		testedRecord.Add(tcase);
	}
	
	void testMedPos() {
		Debug.Log("testMedPos");
		string info = "";
		for(int c=1; c<=70; c++) {
			string ran = c.ToString();
			string fileName = "/testcase/testcase_" + ran;
			string s = File.ReadAllText(Application.dataPath + "/Resources/" + difficulty + fileName + ".txt", Encoding.UTF8);
			string[] l = s.Split ("\n"[0]);
			patientProfileUpdate(plist[c-1]);
			tcase = new TestCase ();
			tcase.index = c-1;
			for (int i = 1; i < l.Length-1; i++) {//-1 because last line is empty
				string[] data = l [i].Split ("\t"[0]);
				Medicine m = new Medicine ();
				m.index = data[1];
				m.medName = data [2];
				m.usage = data[3];
				m.amt_per_time = data[4];
				m.unit = data[5];
				m.freq = data[6];
				m.day = data[7];
				m.amt = data[8];
				m.comment = data [medProfileNum + problemNum];
				m.position = dp.getPosition(m.index);
				if(m.position == "") {
					info += m.index + "\t" + m.medName + "\n";
				}
			}
		}
		StreamWriter sw;
		FileInfo fi = new FileInfo("D://PharmacyVRTest_1211//MedErrorPos.txt");
		sw = fi.CreateText ();
		sw.WriteLine(info);
		sw.Close();
 		sw.Dispose ();
	}
	int casePicker(int caseNum){
		System.Random random = new System.Random();
        int ranIndex = random.Next(caseNum);
        while (testedRecord.Exists(x => x.index==ranIndex))
			ranIndex = random.Next(caseNum);
        
        return ranIndex;
    }

	void patientProfileUpdate(Patient p){
		GameObject.Find("caseNo").GetComponent<Text>().text = "Case " + (testedRecord.Count+1);
		GameObject.Find("pName").GetComponent<Text>().text = p.name;
		GameObject.Find("date").GetComponent<Text>().text = "日期： " + p.date;
		GameObject.Find("age").GetComponent<Text>().text = "年齡： " + p.age;
		GameObject.Find("no.").GetComponent<Text>().text = "病歷號： " + p.no;
		GameObject.Find("bagNum").GetComponent<Text>().text = "藥袋數： " + p.bagNum;
		String s = "";
		for(int i = 0; i<p.diagnosis.Count; i++)
			s = s + "診斷" + (i+1) + ": " + p.diagnosis[i]+ "\n";
		s = s + "\n體重：" + p.weight + "    " + "看診科別：" + p.department + "\n";

		foreach(string temp in p.kidney)
			s = s + "[腎功能] " + temp + "\n";
		
		s = s + "[過敏藥] " + p.allergy + "\n";
		s = s + "\n其他處方：\n";
		foreach(string temp in p.otherPrescription)
			s = s + temp + "\n";
		GameObject.Find("diagnose").GetComponent<Text>().text = s;
	}

	public void medSelected(){//配藥單上選取後，根據記錄更新疑義介面選項狀態
		selectedMedIndex = Int32.Parse(EventSystem.current.currentSelectedGameObject.name);

		for (int i = 0; i < toggleList.Count; i++) {
			if (tcase.medList [selectedMedIndex].reply [i] == true)
				toggleList [i].isOn = true;
			else
				toggleList [i].isOn = false;
		}
	}

	public void answerSelected(){
		tcase.medList [selectedMedIndex].answerUpdate (toggleList);
	}

	void answerCheck(string mode){

        if (mode == "all") // check all testcase
        {
            string s = "";
            int percentage = 0;
            for (int i = 0; i < testedRecord.Count; i++)
            {
                s += "Case " + (i + 1) + (testedRecord[i].isBad? ":  疑義處方, ": ":  正確處方, ") + (testedRecord[i].correct ? " 回答: 正確, " : " 回答: 錯誤, ") + "時間: " + testedRecord[i].timer + "sec, 處方編號：" + (testedRecord[i].index+1) + "\n\t";
                if (testedRecord[i].correct) percentage++;
            }
            s += "正確率: " + (percentage * 20) + "%\n";
            resultList.Insert(0, s);
            mainP.SetActive(false);
            resultP.SetActive(true);
            resultTxt.text = "Overview\n\t" + resultList[0];
            resultShowingIndex = 0;

            // log file
            StreamWriter sw;
            string fname = "log";
            int num = 1;
            if(File.Exists(Application.dataPath + "/Record/" + fname + ".txt"))
            {
                while (File.Exists(Application.dataPath + "/Record/" + fname + "_" + num.ToString() + ".txt")) num++;
                fname += "_" + num.ToString();
            }
            FileInfo fi = new FileInfo(Application.dataPath + "/Record/"+fname+".txt");
            resultList[0] = resultList[0].Replace("\t", "");
            sw = fi.CreateText();
            sw.WriteLine(resultList[0]);
            sw.Close();
            sw.Dispose();

        }
        else if (mode == "single")      // check the lase testcase
        {
            bool wrongFlag = false, noProblem = true, noQuestion = true, noDisWrong = true;
			TestCase t = testedRecord[testedRecord.Count-1];
            t.isBad = false;
            foreach (Medicine med in t.medList)
            {
                for (int i = 0; i < problemNum; i++)
                    if (med.problem[i]) t.isBad = true;
            }
            foreach (Medicine med in t.medList) {
                bool medWrong = false;
                string s = "\t\t" + med.medName + ": \n";
                // 疑義
                for (int i = 0; i < problemNum; i++) {
                    if(med.problem[i] || med.reply[i])
					    noProblem = false;
                    
					if (med.reply[i] != med.problem[i]){
                        noQuestion = false;
                        medWrong = true;
                        //s = s + ("\t\t" + med.comment + "\n");
                        s = s + "\t\t\t正確答案: ";
						string ansString = getAnswerString (med);
						if (ansString == "") {
							s = s + "無疑義藥品\n";
						}
						else
							s = s + ansString;
						resultList.Add(s);
						wrongFlag = true;
						break;
					}
				}

				// 種類
				if(!med.replyIndex && noProblem && !t.isBad) {
                    noDisWrong = false;
					s += "\t\t\t取用種類錯誤\n";
					if(!wrongFlag)
						resultList.Add(s);
					else {
						resultList[resultList.Count-1] +=  (medWrong?"\t\t\t取用種類錯誤\n" : s);
					}
					wrongFlag = true;
                    medWrong = true;
				}
				// 數量
				if(!med.replyAmount && noProblem && !t.isBad) {
                    noDisWrong = false;
					//Debug.Log(resultList.Count-1);
					s += "\t\t\t取用數量錯誤\n";
					if(!wrongFlag)
						resultList.Add(s);
					else {
						resultList[resultList.Count-1] += (medWrong ? "\t\t\t取用數量錯誤\n" : s);
					}
					wrongFlag = true;
                    medWrong = true;
				}
			}
			if(!wrongFlag){
				string s = "\t\t無\n";
				resultList.Add(s);
			}
            t.correct = !wrongFlag;

            if(!noProblem)
                resultList[resultList.Count - 1] = "處方編號：" + (testedRecord[testedRecord.Count - 1].index+1) + "\n\t評估時間: " + t.timer + "sec\n\t處方評估: " + (noQuestion ? "正確" : ("錯誤" + "\n\t錯誤說明: \n" + resultList[resultList.Count - 1]));
            else
                resultList[resultList.Count - 1] = "處方編號：" + (testedRecord[testedRecord.Count - 1].index+1) + "\n\t調劑時間: " + t.timer + "sec\n\t調劑: " + (noDisWrong ? "正確" : ("錯誤" + "\n\t錯誤說明: \n" + resultList[resultList.Count - 1]));
            mainP.SetActive(false);
			checkpointP.SetActive(true);
			checkpointTxt.text = "Case " + testedRecord.Count + ": \n\t" + resultList[resultList.Count-1];

        }
		
	}

	string getAnswerString(Medicine med){
		string s = "";
		for (int i = 0; i < problemNum; i++) {
			if (med.problem [i] == true) {
				switch (i) {
				case 0:
					s = s+"未依肝腎功能調整劑量\t";
					break;
				case 1:
					s = s+"劑量過高\t";
					break;
				case 2:
					s = s+"劑量過低\t";
					break;
				case 3:
					s = s+"使用頻次錯誤\t";
					break;
				case 4:
					s = s+"交互作用\t";
					break;
				case 5:
					s = s+"使用禁忌\t";
					break;
				case 6:
					s = s+"重複用藥\t";
					break;
				case 7:
					s = s+"有過敏史\t";
					break;
				case 8:
					s = s+"與適應症不符\t";
					break;
				case 9:
					s = s+"治療天數不適當\t";
					break;
				case 10:
					s = s+"數量錯誤\t";
					break;
				}
			}
		}
		return s;
	}

    public void showPrevResult() {
        if (resultShowingIndex == 0)
            return;
        resultShowingIndex--;
        string s;
        if (resultShowingIndex == 0)
            s = "Overview";

        else
            s = "Case " + (resultShowingIndex); ;
		resultTxt.text = s + "\n\t" + resultList[resultShowingIndex];
	}

	public void showNextResult(){
		if(resultShowingIndex==testPeriod)
			return;
		resultShowingIndex++;
        string s;
        if (resultShowingIndex == 0)
            s = "Overview";

        else
            s = "Case " + (resultShowingIndex); ;
        resultTxt.text = s + "\n\t" + resultList[resultShowingIndex];
    }

	public void showCheckpoint() {	// old reload()
		tcase.timer = timer;
        Debug.Log(timer);
		startTimer = false;
		answerCheck("single");
	}

	public void reload(){		
		loadcase();
	}

	public void testAgain(){
		testedRecord = new List<TestCase>();
		resultList = new List<String>();
		resultP.SetActive(false);
		mainP.SetActive(true);
	}

	public void setDifficulty(string level) 
	{
		difficulty = level;
		loadPatient();
		dp.initMedList();
		loadcase();
		setTimer();
	}

	public TestCase NowTestCase() {return tcase;}

	public void setTimer()
    { timer = 0; startTimer = true; Debug.Log("timer start"); }

	private void Update()
	{
		if(startTimer) timer += Time.deltaTime;
	}
}

