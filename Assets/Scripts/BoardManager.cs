using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

// This Script (a component of Game Manager) Initializes the Borad (i.e. screen).
public class BoardManager : MonoBehaviour {

	//Resoultion width and Height
	//CAUTION! Modifying this does not modify the Screen resolution. This is related to the unit grid on Unity.
	public static int resolutionWidth = 1024;
	public static int resolutionHeight = 768;

	//Number of Columns and rows of the grid (the possible positions of the Items) note: these are default values.
	public static int columns = 4;
	public static int rows = 4;

	//A canvas where all the board is going to be placed
	private GameObject canvas;

	//The method to be used to place Items randomly on the grid.
	//1. Choose random positions from full grid. It might happen that no placement is found and the trial will be skipped.
	//2. Choose randomly out of 10 positions. A placement is guaranteed

	//Prefab of the Item interface configuration
	public static GameObject TSPItemPrefab;

	//Prefab of the Item interface configuration
	public static GameObject LineItemPrefab;

	//The possible positions of the Items;
	//private List <Vector3> gridPositions = new List<Vector3> ();

	//Counter
	public Text DistanceText;

	//Coordinate vectors for this trial. CURRENTLY SET UP TO ALLOW ONLY INTEGERS.
	private float[] cox;
	private float[] coy;
	private int[] cities;
	private int[,] distances;

	//The answer Input by the player
	//0:No / 1:Yes / 2:None
	public static int answer;

	private String question;

	//Should the key be working?
	public static bool keysON = false;

	//Reset button
	public Button Reset;

	//necessary?
	//If randomization of buttons:
	//1: No/Yes 0: Yes/No
	public static int randomYes;//=Random.Range(0,2);

	public static int distanceTravelledValue;

	//Structure with the relevant parameters of an Item.
	//gameItem: is the game object
	//66: ItemNumber: a number between 1 and the number of Items. It corresponds to the index in the weight's (and value's) vector.
	public struct Item
	{
		public GameObject gameItem;
		public Vector2 center;
		public int CityNumber;
		public Button CityButton;
	}
//	public Button LineButton;

	//The Items for the scene are stored here.
	private Item[] Items;

	public List<Vector2> unitycoord = new List<Vector2> ();

	//previouscities and addcity based on http://answers.unity3d.com/questions/906057/adding-gameobjects-to-a-list.html
	public List<int> previouscities = new List<int> ();

	// The list of all the button clicks on items. Each event contains the following information:
	// ItemNumber (a number between 1 and the number of items. It corresponds to the index in the weight's (and value's) vector.)
	// Item is being selected In/Out (1/0)
	// Time of the click with respect to the beginning of the trial
	public static List <Vector3> itemClicks =  new List<Vector3> ();

	public GameObject[] lines= new GameObject[100];
	public LineRenderer[] newLine= new LineRenderer[100];

	public int citiesvisited = 0;

	public static void assignVariables(Dictionary<string,string> dictionary)
	{
		////Assigns LayoutParameters to Board Manager

		string columnsS;
		string rowsS;

		dictionary.TryGetValue ("columns", out columnsS);
		dictionary.TryGetValue ("rows", out rowsS);

		columns=Int32.Parse(columnsS);
		rows=Int32.Parse(rowsS);

	}

	//Initializes the instance for this trial:
	//1. Sets the question string using the instance (from the .txt files)
	//2. The weight and value vectors are uploaded
	//3. The instance prefab is uploaded
	void setInstance()
	{
		int randInstance = GameManager.instanceRandomization[GameManager.TotalTrial-1];

		//necessary?
		//question = "Can you pack $" + GameManager.satinstances[randInstance].profit + " if your capacity is " + GameManager.satinstances[randInstance].capacity +"kg?";
		question = "Max: " + GameManager.game_instances[randInstance].maxdistance +"km";
		Text Quest = GameObject.Find("Question").GetComponent<Text>();
		Quest.text = question;
		DistanceText = GameObject.Find ("DistanceText").GetComponent<Text>();
		Reset = GameObject.Find("Reset").GetComponent<Button>();
		Reset.onClick.AddListener(ResetClicked);

		//question = " Max: " + System.Environment.NewLine + GameManager.satinstances[randInstance].capacity +"kg ";

		cox = GameManager.game_instances [randInstance].coordinatesx;
		coy = GameManager.game_instances [randInstance].coordinatesy;
		unitycoord = BoardFunctions.coordinateconvertor(cox,coy);

		cities = GameManager.game_instances [randInstance].cities;
		distances = GameManager.game_instances [randInstance].distancematrix;

		TSPItemPrefab = (GameObject)Resources.Load ("TSPItem");
		LineItemPrefab = (GameObject)Resources.Load ("LineButton");

		int objectCount =coy.Length;
		Items = new Item[objectCount];
		for(int i=0; i < objectCount;i=i+1)
		{
			//int objectPositioned = 0;
			Item ItemToLocate = generateItem (i, unitycoord[i]);//66: Change to different Layer?
			Items[i] = ItemToLocate;
		}
	}

	/// <summary>
	/// Instantiates an Item and places it on the position from the input
	/// </summary>
	/// <returns>The Item structure</returns>
	/// The Item placing here is temporary; The real placing is done by the placeItem() method.
	Item generateItem(int ItemNumber ,Vector2 randomPosition)
	{
		//Instantiates the Item and places it.
		GameObject instance = Instantiate (TSPItemPrefab, randomPosition, Quaternion.identity) as GameObject;

		canvas=GameObject.Find("Canvas");
		instance.transform.SetParent (canvas.GetComponent<Transform> (),false);

		Item ItemInstance = new Item();
		ItemInstance.gameItem = instance;//.gameObject;
		ItemInstance.CityButton = ItemInstance.gameItem.GetComponent<Button> ();
		ItemInstance.CityNumber = cities[ItemNumber];
		ItemInstance.center = randomPosition;

		//Setting the position in a separate line is importatant in order to set it according to global coordinates.
		BoardFunctions.placeItem (ItemInstance,randomPosition);

		//Goes from 1 to numberOfItems
		//note: not sure what this is being used for, so check that's it's ok before using it elsewhere

		return(ItemInstance);

	}

	/// Macro function that initializes the Board
	public void SetupScene(string sceneToSetup)
	{
		if (sceneToSetup == "Trial")
		{
			previouscities.Clear();
			itemClicks.Clear ();
			GameManager.Distancetravelled = 0;
			distanceTravelledValue = 0;
			setInstance ();

			keysON = true;
		} else if(sceneToSetup == "TrialAnswer")
		{
			answer = 2;
			//setQuestion ();
			BoardFunctions.RandomizeButtons ();
			keysON = true;

			//seeGrid();
		}

	}

	//Sets the triggers for pressing the corresponding keys
	//123: Perhaps a good practice thing to do would be to create a "close scene" function that takes as parameter the answer and closes everything (including keysON=false) and then forwards to
	//changeToNextScene(answer) on game manager
	//necessary: this was imported from decision version
	private void setKeyInput(){

		if (GameManager.escena == "Trial") {
			if (Input.GetKeyDown (KeyCode.UpArrow)) {
				InputOutputManager.saveTimeStamp ("ParticipantSkip");
				GameManager.changeToNextScene (itemClicks,0,0,1);
			}
		} else if (GameManager.escena == "TrialAnswer")
		{
			//1: No/Yes 0: Yes/No
			if (randomYes == 1) {
				if (Input.GetKeyDown (KeyCode.LeftArrow)) {
					//Left
					keysON = false;
					answer=0;
					GameObject boto = GameObject.Find("LEFTbutton") as GameObject;
					BoardFunctions.highlightButton(boto);
					GameFunctions.setTimeStamp ();
					GameManager.changeToNextScene (itemClicks,0,1,0);
				} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
					//Right
					keysON = false;
					answer=1;
					GameObject boto = GameObject.Find("RIGHTbutton") as GameObject;
					BoardFunctions.highlightButton(boto);
					GameFunctions.setTimeStamp ();
					GameManager.changeToNextScene (itemClicks,1,1,0);
				}
			} else if (randomYes == 0) {
				if (Input.GetKeyDown (KeyCode.LeftArrow)) {
					//Left
					keysON = false;
					answer=1;
					GameObject boto = GameObject.Find("LEFTbutton") as GameObject;
					BoardFunctions.highlightButton(boto);
					GameFunctions.setTimeStamp ();
					GameManager.changeToNextScene (itemClicks,1,0,0);
				} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
					//Right
					keysON = false;
					answer = 0;
					GameObject boto = GameObject.Find("RIGHTbutton") as GameObject;
					BoardFunctions.highlightButton(boto);
					GameFunctions.setTimeStamp ();
					GameManager.changeToNextScene (itemClicks,0,0,0);
				}
			}
		} else if (GameManager.escena == "SetUp") {
			if (Input.GetKeyDown (KeyCode.Space)) {
				GameFunctions.setTimeStamp ();
				GameManager.changeToNextScene (itemClicks,0,0,0);
			}
		}
	}

	//if clicking on the first city, light it up. after that, clicking on a city will fill the destination city, indicating you've travelled to it, and draw a
	//connecting line between the city of departure and the destination
	public void ClickOnItem(Item ItemToLocate)
	{
		if (!previouscities.Contains (ItemToLocate.CityNumber) || (previouscities.Count () == cities.Length && previouscities.First () == ItemToLocate.CityNumber)) {
			if (CityFirst (previouscities.Count ())) {
				LightFirstCity (ItemToLocate);
			} else {
				DrawLine (ItemToLocate);
			}
			addcity (ItemToLocate);
			itemClicks.Add (new Vector3 (ItemToLocate.CityNumber, GameManager.timeQuestion - GameManager.tiempo,1));
			SetDistanceText ();
		} else if (previouscities.Last () == ItemToLocate.CityNumber) {
			EraseLine (ItemToLocate);
			itemClicks.Add (new Vector3 (ItemToLocate.CityNumber, GameManager.timeQuestion - GameManager.tiempo,0));
		}
	}

	// Use this for initialization
	void Start ()
	{

	}

	// Update is called once per frame
	void Update ()
	{
		if (keysON)
		{
			setKeyInput ();
		}

	}


	///
	/// TSP-Specific Board functions:
	///

	void addcity(Item ItemToLocate)
	{
		if (previouscities.Count () == 0) {
			previouscities.Add (ItemToLocate.CityNumber);
			citiesvisited = previouscities.Count ();
		} else {
			previouscities.Add (ItemToLocate.CityNumber);
			citiesvisited = previouscities.Count ();
		}
	}

	public static List<int> previouscities2f (List <int> previouscitiesP)
	{
		List<int> previouscities2 = previouscitiesP;
		previouscities2.RemoveAt (previouscitiesP.Count ()-1);
		return previouscities2;
	}

	public int distancetravelled()
	{
		int[] individualdistances = new int[previouscities.Count()];
		if (previouscities.Count() < 2) {
		} else {
			for (int i = 0; i < (previouscities.Count ()-1); i++) {
				individualdistances [i] = distances [previouscities[i], previouscities[i+1]];
			}
		}

		int distancetravelled = individualdistances.Sum ();
		distanceTravelledValue = distancetravelled;
		return distancetravelled;
	}

	void SetDistanceText ()
	{
		Debug.Log ("SetDistanceText");
		int distanceT = distancetravelled();
		DistanceText.text = "Distance so far: " + distanceT.ToString () + "km";
	}

	//determining whether the city is the first one to have been clicked in that instance i.e. where is the starting point
	bool CityFirst(int citiesvisited)
	{
		if (citiesvisited == 0)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	//turn the light on around the first city to be clicked on
	private void LightFirstCity(Item ItemToLocate)
	{
		Light myLight = ItemToLocate.gameItem.GetComponent<Light> ();
		myLight.enabled = !myLight.enabled;
		//int cityIn=(myLight.enabled)? 1 : 0 ;
	}

	void DrawLine(Item ItemToLocate)
	{
		int cityofdestination = ItemToLocate.CityNumber;
		int cityofdeparture = previouscities[previouscities.Count()-1];

		Vector2 coordestination = unitycoord [cityofdestination];
		Vector2 coordeparture = unitycoord [cityofdeparture];

		Vector3[] coordinates = new Vector3[2];
		coordinates [0] = coordestination;
		coordinates [1] = coordeparture;

		GameObject instance = Instantiate (LineItemPrefab, new Vector2(0,0), Quaternion.identity) as GameObject;

		canvas=GameObject.Find("Canvas");
		instance.transform.SetParent (canvas.GetComponent<Transform> (),false);

		lines[citiesvisited] = instance;
		newLine[citiesvisited] = lines[citiesvisited].GetComponent<LineRenderer> ();
		newLine[citiesvisited].SetPositions(coordinates);
//		Debug.Log("LineCreated");
//		Debug.Log (citiesvisited);
		}

	//if double click on the previous city then cancel change the destination city back to vacant, and delete the connecting line b/w the two cities
	void EraseLine(Item ItemToLocate)
	{
//		Debug.Log("LineErased");
//		Debug.Log (citiesvisited);
		if (previouscities.Count == 1) {
			ItemToLocate.gameItem.GetComponent<Light> ().enabled = false;
		}
		Destroy (lines[citiesvisited-1]);
		previouscities.RemoveAt (previouscities.Count () - 1);
		citiesvisited --;
		SetDistanceText ();

	}

	private void Lightoff(){
		foreach(Item Item1 in Items){
			if (Item1.CityNumber == previouscities[0]){
				Light myLight = Item1.gameItem.GetComponent<Light> ();
				myLight.enabled = false;
			}
		}
	}

	public void ResetClicked(){
		if (previouscities.Count() != 0)
		{
			for (int i = 0; i < lines.Length; i++) {
				DestroyObject (lines [i]);
			}
			Lightoff();
			previouscities.Clear();
			SetDistanceText ();
			citiesvisited = 0;
			itemClicks.Add (new Vector3 (100, GameManager.timeQuestion - GameManager.tiempo,3));
		}
	}
}
