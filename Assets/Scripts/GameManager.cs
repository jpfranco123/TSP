using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Linq; 
using Random = UnityEngine.Random;
//using System.Diagnostics;

public class GameManager : MonoBehaviour 
{

	//Game Manager: It is a singleton (i.e. it is always one and the same it is nor destroyed nor duplicated)	
	public static GameManager gameManager=null;

	//The reference to the script managing the board (interface/canvas).
	public BoardManager boardScript;

	//Current Scene
	public static string escena;

	//Time spent so far on this scene
	public static float tiempo;

	//Total time for this scene
	public static float totalTime;

	//Current trial initialization
	public static int trial = 0;

	//The total number of trials across all blocks
	public static int TotalTrial = 0;

	//Current block initialization
	public static int block = 0;

	private static bool showTimer;

	//Intertrial rest time
	public static float timeRest1=5f;

	//The times listed are default settings. They are over-ridden by input files, so you need to change the input files to change times
	//InterBlock rest time
	public static float timeRest2=10;

	//Time given for each trial (The total time the items are shown -With and without the question-)
	public static float timeTrial=10;

	//Time for seeing the TSP question 
	public static float timeQuestion=10;

	//Time given for answering 
	public static float timeAnswer=3;

	//Total number of trials in each block
	private static int numberOfTrials = 30;

	//Total number of blocks
	private static int numberOfBlocks = 3;

	//This is also taken from input files, so in reality 63 instance files are loaded, not 3
	//Number of instance files to be considered. From i1.txt to i_.txt..
	public static int numberOfInstances = 3;

	//The order of the instances to be presented
	public static int[] instanceRandomization;

	//The order of the left/right No/Yes randomization
	public static int[] buttonRandomization;

	//we would need a vector of literals, corresponding to 3 literals for each clause in that instance
	//also need a vector of variables for each instance, to see where the literals are being drawn from
	//changes needed further along due to changes in struct
	//A structure that contains the parameters of each instance
	public struct gameInstance
	{
		public int[] cities;
		public float[] coordinatesx;
		public float[] coordinatesy;
		public int[,] distancematrix;
		public int[] distancevector;

		public int ncities;
		public int maxdistance;

		public string id;
		public string type;

		public int solution;
	}

	//distance travelled
	public static int Distancetravelled;

	//An array of all the instances to be uploaded from .txt files, i.e importing everything using the structure from above 
	public static gameInstance[] game_instances;// = new TSPInstance[numberOfInstances];

	// Use this for initialization
	void Awake () 
	{

		//Makes the Game manager a Singleton
		if (gameManager == null) {
			gameManager = this;
		} else if (gameManager != this) {
			Destroy (gameObject);
		}

		DontDestroyOnLoad (gameObject);

		//Initializes the game
		boardScript = gameManager.GetComponent<BoardManager> ();

		InitGame();
		if (escena != "SetUp") 
		{
			InputOutputManager.saveTimeStamp(escena);
		}

	}

	//Initializes the scene. One scene is setup, other is trial, other is Break....
	void InitGame()
	{
		/*
		Scene Order: escena
		0=setup
		1=trial game
		2=trial game answer
		3= intertrial rest
		4= interblock rest
		5= end
		*/
		//Creates the variable scene? and selects the active scene
		Scene scene = SceneManager.GetActiveScene();
		escena = scene.name;
		//Name the scene "Scene #"
		Debug.Log ("escena" + escena);
		//change numbers to names
		//the loop which runs the game, and drives you from one scene to another
		//If it's the first scene, upload parameters and instances (this happens only once), randomise instances and move incrememntally through >blocks< 1 at a time
		if (escena == "SetUp") 
		{
			block++; 
			//loadParameters ();
			//loadTSPInstance ();

			buttonRandomization = GameFunctions.randomizeButtons (numberOfTrials);
			GameFunctions.setupInitialScreen ();
		} 
		else if (escena == "Trial") 
		{
			trial++;
			TotalTrial++;
			showTimer = true;
			boardScript.SetupScene ("Trial");

			tiempo = timeQuestion;
			totalTime = tiempo;
		} 
		else if (escena == "TrialAnswer") 
		{
			showTimer = true;
			boardScript.SetupScene ("TrialAnswer");
			tiempo = timeAnswer;
			totalTime = tiempo;
		} 
		else if (escena == "InterTrialRest") 
		{
			showTimer = false;
			tiempo = timeRest1;
			totalTime = tiempo;

		} 
		else if (escena == "InterBlockRest") 
		{
			trial = 0;
			block++;
			showTimer = true;
			tiempo = timeRest2;
			totalTime = tiempo;
			//Debug.Log ("TiempoRest=" + tiempo);

			buttonRandomization = GameFunctions.randomizeButtons (numberOfTrials);
		}

	}


	// Update is called once per frame
	void Update () 
	{
		if (escena != "SetUp") 
		{
			startTimer ();
			GameFunctions.pauseManager ();
		}
	}

	//Assigns the parameters in the dictionary to variables
	public static void assignVariables(Dictionary<string,string> dictionary)
	{
		//Assigns Parameters - these are all going to be imported from input files
		string timeRest1S;
		string timeRest2S;
		string timeQuestionS;
		string timeAnswerS;
		string numberOfTrialsS;
		string numberOfBlocksS;
		string numberOfInstancesS;
		string instanceRandomizationS;

		dictionary.TryGetValue ("timeRest1", out timeRest1S);
		dictionary.TryGetValue ("timeRest2", out timeRest2S);

		dictionary.TryGetValue ("timeQuestion", out timeQuestionS);

		dictionary.TryGetValue ("timeAnswer", out timeAnswerS);

		dictionary.TryGetValue ("numberOfTrials", out numberOfTrialsS);

		dictionary.TryGetValue ("numberOfBlocks", out numberOfBlocksS);

		dictionary.TryGetValue ("numberOfInstances", out numberOfInstancesS);

		timeRest1=Convert.ToSingle (timeRest1S);
		timeRest2=Convert.ToSingle (timeRest2S);
		timeQuestion=Int32.Parse(timeQuestionS);
		timeAnswer=Int32.Parse(timeAnswerS);
		numberOfTrials=Int32.Parse(numberOfTrialsS);
		numberOfBlocks=Int32.Parse(numberOfBlocksS);
		numberOfInstances=Int32.Parse(numberOfInstancesS);

		dictionary.TryGetValue ("instanceRandomization", out instanceRandomizationS);

		int[] instanceRandomizationNo0 = Array.ConvertAll(instanceRandomizationS.Substring (1, instanceRandomizationS.Length - 2).Split (','), int.Parse);
		instanceRandomization = new int[instanceRandomizationNo0.Length];

		for (int i = 0; i < instanceRandomizationNo0.Length; i++)
		{
			instanceRandomization[i] = instanceRandomizationNo0 [i] - 1;
		}
			
		////Assigns LayoutParameters to Board Manager

		string columnsS;
		string rowsS;

		dictionary.TryGetValue ("columns", out columnsS);
		dictionary.TryGetValue ("rows", out rowsS);

		BoardManager.columns=Int32.Parse(columnsS);
		BoardManager.rows=Int32.Parse(rowsS);

	}

	//Takes care of changing the Scene to the next one (Except for when in the setup scene)
	public static void changeToNextScene(List <Vector3> itemClicks, int answer, int randomYes)
	{
		BoardManager.keysON = false;
		if (escena == "SetUp") {
			Debug.Log ("SetUp");

			InputOutputManager.loadGame ();
			SceneManager.LoadScene ("Trial");
		} else if (escena == "Trial") {
			//66 Update distanceTravelled Value or delete to previous code...
			Distancetravelled = BoardManager.distanceTravelledValue;
			SceneManager.LoadScene ("TrialAnswer");
		} else if (escena == "TrialAnswer") {
			string itemsSelected= extractItemsSelected (itemClicks);
			if (answer == 2) {
				InputOutputManager.save(itemsSelected, answer, timeQuestion, randomYes, "");
			} else {
				InputOutputManager.save (itemsSelected, answer, timeAnswer - tiempo, randomYes, "");
				InputOutputManager.saveTimeStamp ("ParticipantAnswer");
			}
			InputOutputManager.saveClicks (itemClicks);
			SceneManager.LoadScene ("InterTrialRest");
		} else if (escena == "InterTrialRest") {
			changeToNextTrial ();
		} else if (escena == "InterBlockRest") {
			SceneManager.LoadScene ("Trial");
		}
	}
	//Redirects to the next scene depending if the trials or blocks are over.
	public static void changeToNextTrial()
	{
		//Checks if trials are over
		if (trial < numberOfTrials) {
			SceneManager.LoadScene ("Trial");
		} else if (block < numberOfBlocks) {
			SceneManager.LoadScene ("InterBlockRest");
		}else {
			SceneManager.LoadScene ("End");
		}
	}

	/// <summary>
	/// Extracts the items that were finally selected based on the sequence of clicks.
	/// </summary>
	/// <returns>The items selected.</returns>
	/// <param name="itemClicks"> Sequence of clicks on the items.</param>
	private static string extractItemsSelected (List <Vector3> itemClicks){
		List<int> itemsIn = new List<int>();
		foreach(Vector3 clickito in itemClicks){
			if (clickito.z == 1) {
				itemsIn.Add (Convert.ToInt32 (clickito.x));
			} else if (clickito.z == 0) {
				itemsIn.Remove (Convert.ToInt32 (clickito.x));
			} else if (clickito.z == 3) {
				itemsIn.Clear ();
			}
		}

		string itemsInS = "";
		foreach (int i in itemsIn)
		{
			itemsInS = itemsInS + i + ",";
		}
		if(itemsInS.Length>0)
			itemsInS = itemsInS.Remove (itemsInS.Length - 1);

		return itemsInS;
	}

	//Updates the timer (including the graphical representation)
	//If time runs out in the trial or the break scene. It switches to the next scene.
	void startTimer()
	{
		tiempo -= Time.deltaTime;
		//Debug.Log (tiempo);
		if (showTimer) 
		{
			BoardFunctions.updateTimer();
		}

		//When the time runs out:
		if(tiempo < 0)
		{
			//changeToNextScene(2,BoardManager.randomYes);
			changeToNextScene(BoardManager.itemClicks,BoardManager.answer,BoardManager.randomYes);
		}
	}
		

}