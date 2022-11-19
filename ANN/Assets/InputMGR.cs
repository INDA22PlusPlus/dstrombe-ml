using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputMGR : MonoBehaviour {
	InputField f;
	public ANN network;
	// Use this for initialization
	void Start () {
		f = GetComponent<InputField>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void submit()
	{
		network.HumanTest(f.text);
	}
}
