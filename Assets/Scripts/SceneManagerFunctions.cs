using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SceneManagerFunctions : MonoBehaviour {

	//Should the key be working?
	//public static bool keysON = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void setupInitialScreen()
	{
		//Button 
		Debug.Log("Start button");
		GameObject start = GameObject.Find("Start") as GameObject;
		start.SetActive (false);

		Debug.Log("Rand button");
		GameObject rand = GameObject.Find("RandomisationID") as GameObject;
		rand.SetActive (false);

		//Participant Input
		InputField pID = GameObject.Find ("ParticipantID").GetComponent<InputField>();

		InputField.SubmitEvent se = new InputField.SubmitEvent();
		//se.AddListener(submitPID(start));
		se.AddListener((value)=>submitPID(value,start,rand));
		pID.onEndEdit = se;


		//Randomisation Input
		InputField rID = rand.GetComponent<InputField>();

		InputField.SubmitEvent se2 = new InputField.SubmitEvent();
		//se.AddListener(submitPID(start));
		se2.AddListener((value)=>submitRandID(value,start));
		rID.onEndEdit = se2;

		//pID.onSubmit.AddListener((value) => submitPID(value));

	}

	public void submitPID(string pIDs, GameObject start, GameObject rand)
	{
		//Debug.Log (pIDs);

		GameObject pID = GameObject.Find ("ParticipantID");
		pID.SetActive (false);
		//pIDT.SetActive (false);

		//Set Participant ID
		GameManager.participantID=pIDs;

		//Activate Randomisation Listener
		rand.SetActive (true);



		//Activate Start Button and listener
		//GameObject start = GameObject.Find("Start");
		//start.SetActive (true);
		//keysON = true;

	}

	public void submitRandID(string rIDs, GameObject start)
	{
		//Debug.Log (pIDs);

		GameObject rID = GameObject.Find ("RandomisationID");
		GameObject pIDT = GameObject.Find ("Participant ID Text");
		rID.SetActive (false);
		pIDT.SetActive (false);

		//Set Participant ID
		GameManager.randomisationID=rIDs;

		//Activate Start Button and listener
		//GameObject start = GameObject.Find("Start");
		start.SetActive (true);
		BoardManager.keysON = true;

	}


}
