using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour {

    private Text text;
    private Queue<float> rates = new Queue<float>();
    private float cumulativeTotal = 0;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        rates.Enqueue(1f / Time.deltaTime);
        cumulativeTotal += 1f / Time.deltaTime;
        if (rates.Count > 60)
        {
            cumulativeTotal -= rates.Dequeue();
        }
        text.text = (cumulativeTotal/rates.Count).ToString("0");
	}
}
