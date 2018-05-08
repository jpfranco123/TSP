using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Linq; 
using Random = UnityEngine.Random;

public class InputOutputManager : MonoBehaviour {

	//Setting up the variable participantID
	//This is the string that will be used as the file name where the data is stored. DeCurrently the date-time is used.
	public static string participantID = "Empty";

	//This is the randomisation number (#_param2.txt that is to be used for oder of instances for this participant)
	public static string randomisationID = "Empty";

	public static string dateID = @System.DateTime.Now.ToString("dd MMMM, yyyy, HH-mm");

	//Starting strig of the output file names
	private static string identifierName;

	//Input and Outout Folders with respect to the Application.dataPath;
	private static string inputFolder = "/DATAinf/Input/";
	private static string inputFolderInstances = "/DATAinf/Input/Instances/";
	private static string outputFolder = "/DATAinf/Output/";

	private static string folderPathLoad;
	private static string folderPathLoadInstances;
	private static string folderPathSave;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public static void loadGame(){

		identifierName = participantID + "_" + dateID + "_" + "TSP" + "_";

		folderPathLoad = Application.dataPath + inputFolder;
		folderPathLoadInstances = Application.dataPath + inputFolderInstances;
		folderPathSave = Application.dataPath + outputFolder;
		
		loadParameters ();
		GameManager.game_instances = loadInstances (GameManager.numberOfInstances);
		saveInstanceInfo (GameManager.numberOfInstances, GameManager.game_instances);
		saveHeaders ();
	}


	private static int[,] StringToMatrix(string matrixS)
	{
		int[] convertor = Array.ConvertAll (matrixS.Substring (1, matrixS.Length - 2).Split (','), int.Parse);
		try {
			int vectorheight = Convert.ToInt32(Math.Sqrt (convertor.Length));
			int[,] arr = new int[vectorheight,vectorheight]; // note the swap
			int x = 0;
			int y = 0;
			for (int i = 0; i < convertor.Length; ++i)
			{
				arr[y, x] = convertor[i]; // note the swap
				x++;
				if (x == vectorheight)
				{
					x = 0;
					y++;
				}
			}
			return arr;
		}
		catch (Exception e) {
			Debug.Log ("We don't have an int");
			Debug.Log (e.Message);
			return new int[1, 1]; 
		}
	}

	//Loads the parameters from the text files: param.txt and layoutParam.txt
	private static void loadParameters()
	{

		var dict = new Dictionary<string, string>();

		try 
		{   // Open the text file using a stream reader.
			using (StreamReader sr = new StreamReader (folderPathLoad + "layoutParam.txt")) 
			{

				// (This loop reads every line until EOF or the first blank line.)
				string line;
				while (!string.IsNullOrEmpty((line = sr.ReadLine())))
				{
					// Split each line around ':'
					string[] tmp = line.Split(new char[] {':'}, StringSplitOptions.RemoveEmptyEntries);
					// Add the key-value pair to the dictionary:
					dict.Add(tmp[0], tmp[1]);//int.Parse(dict[tmp[1]]);
				}
			}


			using (StreamReader sr1 = new StreamReader (folderPathLoad + "param.txt")) 
			{

				// (This loop reads every line until EOF or the first blank line.)
				string line1;
				while (!string.IsNullOrEmpty((line1 = sr1.ReadLine())))
				{
					//Debug.Log (1);
					// Split each line around ':'
					string[] tmp = line1.Split(new char[] {':'}, StringSplitOptions.RemoveEmptyEntries);
					// Add the key-value pair to the dictionary:
					dict.Add(tmp[0], tmp[1]);//int.Parse(dict[tmp[1]]);
				}
			}

			using (StreamReader sr2 = new StreamReader (folderPathLoadInstances + randomisationID + "_param2.txt"))
			{

				// (This loop reads every line until EOF or the first blank line.)
				string line2;
				while (!string.IsNullOrEmpty((line2 = sr2.ReadLine())))
				{
					//Debug.Log (1);
					// Split each line around ':'
					string[] tmp = line2.Split(new char[] {':'}, StringSplitOptions.RemoveEmptyEntries);
					// Add the key-value pair to the dictionary:
					dict.Add(tmp[0], tmp[1]);//int.Parse(dict[tmp[1]]);
				}
			}

		} 
		catch (Exception e) 
		{
			Debug.Log ("The file could not be read:");
			Debug.Log (e.Message);
		}

		GameManager.assignVariables(dict);

	}


	/*
	 * Loads all of the instances to be uploaded form .txt files. Example of input file:
	 * Name of the file: i3.txt
	 * Structure of each file is the following:
	 * variables:[1,2,3,4,5]
	 * literals:[X,Y,D,G,W]
	 *
	 * The instances are stored as tspinstances structures in the array of structures: tspinstances
	 * */
	private static GameManager.gameInstance[] loadInstances(int numberOfInstances)
	{
		//string folderPathLoad = Application.dataPath + inputFolderInstances;
		GameManager.gameInstance[] game_instances = new GameManager.gameInstance[numberOfInstances];

		for (int k = 1; k <= numberOfInstances; k++) {
			//create a dictionary where all the variables and definitions are strings
			var dict = new Dictionary<string, string> ();
			//			//Use streamreader to read the input files if there are lines to read
			try {   // Open the text file using a stream reader.
				using (StreamReader sr = new StreamReader (folderPathLoadInstances + "i"+ k + ".txt")) {
					string line;
					while (!string.IsNullOrEmpty ((line = sr.ReadLine ()))) {
						string[] tmp = line.Split (new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
						// Add the key-value pair to the dictionary:
						dict.Add (tmp [0], tmp [1]);//int.Parse(dict[tmp[1]]);
					}
					// Read the stream to a string, and write the string to the console.
					//String line = sr.ReadToEnd();
				}
				//if there is a problem, then report the following error message
			} catch (Exception e) {
				Debug.Log ("The file could not be read:");
				Debug.Log (e.Message);
			}

			//the following are all recorded as string (hence the S at the end) 
			string citiesS;
			string coordinatesxS;
			string coordinatesyS;
			string distancevectorS;
			string ncitiesS;
			string maxdistanceS;
			string solutionS;

			//grab all of those parameters as strings
			dict.TryGetValue ("cities", out citiesS);
			dict.TryGetValue ("coordinatesx", out coordinatesxS);
			dict.TryGetValue ("coordinatesy", out coordinatesyS);
			dict.TryGetValue ("distancevector", out distancevectorS);
			dict.TryGetValue ("ncities", out ncitiesS);
			dict.TryGetValue ("maxdistance", out maxdistanceS);
			dict.TryGetValue ("solution", out solutionS);


			//convert (most of them) to integers, with variables and literals being arrays and the others single literals
			game_instances [k-1].cities = Array.ConvertAll (citiesS.Substring (1, citiesS.Length - 2).Split (','), int.Parse);
			game_instances [k-1].coordinatesx = Array.ConvertAll (coordinatesxS.Substring (1, coordinatesxS.Length - 2).Split (','), float.Parse);
			game_instances [k-1].coordinatesy = Array.ConvertAll (coordinatesyS.Substring (1, coordinatesyS.Length - 2).Split (','), float.Parse);

			game_instances [k-1].distancevector = Array.ConvertAll (distancevectorS.Substring (1, distancevectorS.Length - 2).Split (','), int.Parse);
			game_instances [k - 1].distancematrix = StringToMatrix(distancevectorS);

			game_instances [k-1].ncities = int.Parse (ncitiesS);
			game_instances [k-1].maxdistance = int.Parse (maxdistanceS);
			game_instances [k-1].solution = int.Parse (solutionS);

			dict.TryGetValue ("problemID", out game_instances [k - 1].id);
			dict.TryGetValue ("param", out game_instances [k - 1].type);
		}

		return(game_instances);
	}


	/// <summary>
	/// Saves the Instance Information
	/// </summary>
	private static void saveInstanceInfo(int numberOfInstances, GameManager.gameInstance[] instances)
	{
		//Saves instance info
		//an array of string, a new variable called lines3
		string[] lines3 = new string[numberOfInstances+2];
		//the first two lines will show the following - "string": "parameter/input"
		lines3[0]="PartcipantID:" + participantID;
		lines3 [1] = "instanceNumber" + ";cy"  + ";co"   + ";distances" + ";id" + ";type" + ";sol";

		int l = 2;
		int tspn = 1;
		foreach (GameManager.gameInstance tsp in instances) 
		{
			//Without instance type and problem ID:
			//lines [l] = "Instance:" + tspn + ";c=" + tsp.capacity + ";p=" + tsp.profit + ";w=" + string.Join (",", tsp.variables.Select (p => p.ToString ()).ToArray ()) + ";v=" + string.Join (",", tsp.literals.Select (p => p.ToString ()).ToArray ());
			//With instance type and problem ID
			lines3 [l] = tspn + ";" + string.Join (",", tsp.coordinatesx.Select (p => p.ToString ()).ToArray ())  + ";" + string.Join (",", tsp.coordinatesy.Select (p => p.ToString ()).ToArray ()) 
				+ ";" + string.Join (",", tsp.distancevector.Select (p => p.ToString ()).ToArray ()) 
				+ ";" + tsp.id + ";" + tsp.type + ";" + tsp.solution;

			l++;
			tspn++;
		}
		//using StreamWriter to write the above into an output file
		using (StreamWriter outputFile = new StreamWriter(folderPathSave + identifierName + "InstancesInfo.txt",true)) 
		{
			foreach (string line in lines3)
				outputFile.WriteLine(line);
		}

	}


	/// <summary>
	/// Saves the headers for both files (Trial Info and Time Stamps)
	/// In the trial file it saves:  1. The participant ID. 2. Instance details.
	/// In the TimeStamp file it saves: 1. The participant ID. 2.The time onset of the stopwatch from which the time stamps are measured. 3. the event types description.
	/// </summary>
	private static void saveHeaders()
	{
		// Trial Info file headers
		string[] lines = new string[2];
		lines[0]="PartcipantID:" + participantID;
		lines [1] = "block;trial;answer;correct;timeSpent;itemsSelected;finaldistance;randomYes(1=Left:No/Right:Yes);instanceNumber;error";
		using (StreamWriter outputFile = new StreamWriter(folderPathSave + identifierName + "TrialInfo.txt",true)) 
		{
			foreach (string line in lines)
				outputFile.WriteLine(line);
		}


		// Time Stamps file headers
		string[] lines1 = new string[3];
		lines1[0]="PartcipantID:" + participantID;
		lines1[1] = "InitialTimeStamp:" + SceneManagerFunctions.initialTimeStamp;
		lines1[2]="block;trial;eventType;elapsedTime";
		using (StreamWriter outputFile = new StreamWriter(folderPathSave + identifierName + "TimeStamps.txt",true)) 
		{
			foreach (string line in lines1)
				outputFile.WriteLine(line);
		}

		//Headerds for Clicks file
		string[] lines2 = new string[3];
		lines2[0]="PartcipantID:" + participantID;
		lines2[1] = "InitialTimeStamp:" + SceneManagerFunctions.initialTimeStamp;
		lines2[2]="block;trial;citynumber(100=Reset);In(1)/Out(0)/Reset(3);time"; 
		using (StreamWriter outputFile = new StreamWriter(folderPathSave + identifierName + "Clicks.txt",true)) {
			foreach (string line in lines2)
				outputFile.WriteLine(line);
		}

	}


	//Saves the data of a trial to a .txt file with the participants ID as filename using StreamWriter.
	//If the file doesn't exist it creates it. Otherwise it adds on lines to the existing file.
	//Each line in the File has the following structure: "trial;answer;timeSpent".
	public static void save(string itemsSelected, int answer, float timeSpent, int randomYes, string error) 
	{
		//disregard this...string xyCoordinates = instance.boardScript.getItemCoordinates ();//BoardManager.getItemCoordinates ();

		//Get the instance number for this trial (take the block number, subtract 1 because indexing starts at 0. Then multiply it
		//by numberOfTrials (i.e. 10, 10 per block) and add the trial number of this block. Thus, the 2nd trial of block 2 will be
		//instance number 12 overall) and add 1 because the instanceRandomization is linked to array numbering in C#, which starts at 0;
		int instanceNum = GameManager.instanceRandomization [GameManager.TotalTrial - 1] + 1;
		int finaldistance = GameManager.Distancetravelled;

		//looks at the solution, it is either correct or incorrect
		int solutionQ = GameManager.game_instances [instanceNum - 1].solution;
		int correct = (solutionQ == answer) ? 1 : 0;

		//what to save and the order in which to do so
		//		string dataTrialText = /* block + ";" + */trial + ";" + answer + ";" + correct + ";" + timeSpent + ";" + randomYes +";" + instanceNum + ";" + error;
		string dataTrialText = GameManager.block + ";" + GameManager.trial + ";" + answer + ";" + correct + ";" + timeSpent + ";" + itemsSelected + ";" + finaldistance + ";" + randomYes +";" 
			+ instanceNum + ";" + error;


		//where to save
		string[] lines = {dataTrialText};
		string folderPathSave = Application.dataPath + outputFolder;

		//This location can be used by unity to save a file if u open the game in any platform/computer:      Application.persistentDataPath;

		using (StreamWriter outputFile = new StreamWriter(folderPathSave + identifierName +"TrialInfo.txt",true)) 
		{
			foreach (string line in lines)
				outputFile.WriteLine(line);
		}

		//Options of streamwriter include: Write, WriteLine, WriteAsync, WriteLineAsync
	}


	/// <summary>
	/// Saves the time stamp for a particular event type to the "TimeStamps" File
	/// All these saves take place in the Data folder, where you can create an output folder
	/// </summary>
	/// Event type: 1=ItemsNoQuestion;11=ItemsWithQuestion;2=AnswerScreen;21=ParticipantsAnswer;3=InterTrialScreen;4=InterBlockScreen;5=EndScreen
	public static void saveTimeStamp(string eventType) 
	{
		string dataTrialText = GameManager.block + ";" + GameManager.trial + ";" + eventType + ";" + SceneManagerFunctions.timeStamp();

		string[] lines = {dataTrialText};
		string folderPathSave = Application.dataPath + outputFolder;

		//This location can be used by unity to save a file if u open the game in any platform/computer:      Application.persistentDataPath;
		using (StreamWriter outputFile = new StreamWriter(folderPathSave + identifierName + "TimeStamps.txt",true)) 
		{
			foreach (string line in lines)
				outputFile.WriteLine(line);
		}

	}




	/// <summary>
	/// Saves the time stamp of every click made on the items 
	/// </summary>
	/// block ; trial ; clicklist (i.e. item number ; itemIn? (1: selcting; 0:deselecting) ; time of the click with respect to the begining of the trial)
	public static void saveClicks(List<Vector3> clicksList) {

		string folderPathSave = Application.dataPath + outputFolder;


		string[] lines = new string[clicksList.Count];
		int i = 0;
		foreach (Vector3 clickito in clicksList) {
			lines[i]= GameManager.block + ";" + GameManager.trial + ";" + clickito.x + ";" + clickito.z + ";" + clickito.y ; 
			i++;
		}
		//This location can be used by unity to save a file if u open the game in any platform/computer:      Application.persistentDataPath;
		using (StreamWriter outputFile = new StreamWriter(folderPathSave + identifierName + "Clicks.txt",true)) {
			foreach (string line in lines)
				outputFile.WriteLine(line);
		} 

	}


	/// <summary>
	/// In case of an error: Skip trial and go to next one.
	/// Example of error: Not enough space to place all items
	/// </summary>
	/// Receives as input a string with the errorDetails which is saved in the output file.
	public static void errorInScene(string errorDetails){
		Debug.Log (errorDetails);
		BoardManager.keysON = false;
		int answer = 3;
		int randomYes = -1;
		InputOutputManager.save ("", answer, GameManager.timeTrial, randomYes, errorDetails);
		GameManager.changeToNextTrial ();
	}





}
