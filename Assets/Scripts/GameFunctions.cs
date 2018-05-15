using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameFunctions : MonoBehaviour {

	//Can copy this code if time stamps are needed (likely) Stopwatch to calculate time of events.
	private static System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
	// Time at which the stopwatch started. Time of each event is calculated according to this moment.
	public static string initialTimeStamp;

	public static void setupInitialScreen()
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

	private static void submitPID(string pIDs, GameObject start, GameObject rand)
	{
		//Debug.Log (pIDs);

		GameObject pID = GameObject.Find ("ParticipantID");
		pID.SetActive (false);
		//pIDT.SetActive (false);

		//Set Participant ID
		InputOutputManager.participantID=pIDs;

		//Activate Randomisation Listener
		rand.SetActive (true);



		//Activate Start Button and listener
		//GameObject start = GameObject.Find("Start");
		//start.SetActive (true);
		//keysON = true;

	}

	private static void submitRandID(string rIDs, GameObject start)
	{
		//Debug.Log (pIDs);

		GameObject rID = GameObject.Find ("RandomisationID");
		GameObject pIDT = GameObject.Find ("Participant ID Text");
		rID.SetActive (false);
		pIDT.SetActive (false);

		//Set Participant ID
		InputOutputManager.randomisationID=rIDs;

		//Activate Start Button and listener
		//GameObject start = GameObject.Find("Start");
		start.SetActive (true);
		BoardManager.keysON = true;

	}

	//To pause press alt+p
	//Pauses/Unpauses the game. Unpausing take syou directly to next trial
	//Warning! When Unpausing the following happens:
	//If paused/unpaused in scene 1 or 2 (while items are shown or during answer time) then saves the trialInfo with an error: "pause" without information on the items selected.
	//If paused/unpaused on ITI or IBI then it generates a new row in trial Info with an error ("pause"). i.e. there are now 2 rows for the trial.
	public static void pauseManager(){
		if (( Input.GetKey (KeyCode.LeftAlt) || Input.GetKey (KeyCode.RightAlt)) && Input.GetKeyDown (KeyCode.P) ){
			Time.timeScale = (Time.timeScale == 1) ? 0 : 1;
			if(Time.timeScale==1){
				InputOutputManager.errorInScene("Pause");
			}
		}
	}


	/// <summary>
	/// Starts the stopwatch. Time of each event is calculated according to this moment.
	/// Sets "initialTimeStamp" to the time at which the stopwatch started.
	/// </summary>
	public static void setTimeStamp()
	{
		initialTimeStamp=@System.DateTime.Now.ToString("HH-mm-ss-fff");
		stopWatch.Start ();
		Debug.Log (initialTimeStamp);
	}

	/// <summary>
	/// Calculates time elapsed
	/// </summary>
	/// <returns>The time elapsed in milliseconds since the "setTimeStamp()".</returns>
	public static string timeStamp()
	{
		long milliSec = stopWatch.ElapsedMilliseconds;
		string stamp = milliSec.ToString();
		return stamp;
	}

	//Randomizes The Location of the Yes/No button for a whole block.
	public static int[] randomizeButtons(int numberOfTrials)
	{
		
		int[] buttonRandomization = new int[numberOfTrials];
		List<int> buttonRandomizationTemp = new List<int> ();
		for (int i = 0; i < numberOfTrials; i++){
			if (i % 2 == 0) {
				buttonRandomizationTemp.Add(0);
			} else {
				buttonRandomizationTemp.Add(1);
			}
		}
		for (int i = 0; i < numberOfTrials; i++) {
			int randomIndex = Random.Range (0, buttonRandomizationTemp.Count);
			buttonRandomization [i] = buttonRandomizationTemp [randomIndex];
			buttonRandomizationTemp.RemoveAt (randomIndex);
		}

		return buttonRandomization;
	}
		


}
