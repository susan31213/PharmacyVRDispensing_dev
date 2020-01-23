using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintLine : MonoBehaviour {

    public Transform rightHand;
    public List<Vector3> meds;
    List<LineRenderer> lines;
    public GameObject linePrefab;

    public bool show = false;

    public List<KeyValuePair<string, Vector3>> medIndexPosPair;

    private void Awake()
    {
        GameObject[] allMedGo = GameObject.FindGameObjectsWithTag("MedModel");

        medIndexPosPair = new List<KeyValuePair<string, Vector3>>();
        foreach (GameObject g in allMedGo)
        {
            medIndexPosPair.Add(new KeyValuePair<string, Vector3>(g.name, g.transform.position));
        }
    }

    private void Start()
    {
        meds = new List<Vector3>();
        lines = new List<LineRenderer>();
    }

    private void Update()
    {
        if(show)
        {
            LineRenderer[] lr = GetComponentsInChildren<LineRenderer>();
            foreach (LineRenderer l in lr)
                l.enabled = true;

            if (meds.Count == 0) return;

            for (int i = 0; i < meds.Count; i++)
            {
                lines[i].SetPosition(0, rightHand.position);
                lines[i].SetPosition(1, meds[i]);
            }
        }
        else
        {
            LineRenderer[] lr = GetComponentsInChildren<LineRenderer>();
            foreach (LineRenderer l in lr)
                l.enabled = false;
        }

        
    }

    public void GenrateLine(string medIndex)
    {
        string target = (medIndex.Length > 6)? medIndex.Remove(6): medIndex;
        Debug.Log(target + ", " + target.Length);
        Vector3 tar = medIndexPosPair.Find(x => x.Key == target).Value;
        if(tar == null)
        {
            Debug.Log("No this med.");
        }

        meds.Add(tar);

        GameObject g = Instantiate(linePrefab, transform);
        lines.Add(g.GetComponent<LineRenderer>());
    }

    public void ClearList() {
        for(int i=0; i<transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        meds.Clear();
        lines.Clear();
    }
}
