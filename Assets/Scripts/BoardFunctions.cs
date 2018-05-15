using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BoardFunctions : MonoBehaviour {

	//Function which takes coordinates from input files and converts it to coordinates on the unity grid
	//unity origin x=-476, y=-344, which means opposite corner is x=476 and y=344
	//minimum input coordinate is (0,0), maximum input coordinate is (952,688)
	public static List<Vector2> coordinateconvertor(float[] cox, float[] coy)
	{
		List<Vector2> unitycoordinates = new List<Vector2> ();
		for (int i = 0; i < cox.Length; i++) {
			//			unitycoordinates.Add (new Vector2 ((float)((cox [i] / cox.Max ()) * 952 - 476) / 100, (float)((coy [i] / coy.Max ()) * 688 - 344) / 100));
			//			Debug.Log ((float)(cox [i] / cox.Max ()) * 952 - 476);

			//x=0 in coord leads to x=-384 in unity, x=1024 leads to x=599.04 - so a 1 unit increase in coord leads to a 0.96 unit increase in unity
			//y=0 in coord leads to y=-288 in unity, y=768 leads to y=449.28 - so a 1 unit increase in coord leads to a 0.96 unit increase in unity
			//x-1 below leads to x=-384 in unity, x=1024 leads to x=599.04 - so a 1 unit increase in coord leads to a 0.96 unit increase in unity
			//y-1 below leads to 
			unitycoordinates.Add (new Vector2 ((float)((cox[i] / cox.Max()) * 1024-110f)/100,(float)((coy[i] / coy.Max()) * 710-80f)/100));

			//			unitycoordinates.Add (new Vector2 ((float)cox[i], (float)coy[i]));
			//			Debug.Log (((float)(cox [i] / cox.Max ()) * 1024 - 512) / 100);
		}
		return unitycoordinates;

	}




	/// <summary>
	/// Places the Item on the input position
	/// </summary>
	public static void placeItem(BoardManager.Item ItemToLocate, Vector2 position){
		//Setting the position in a separate line is importatant in order to set it according to global coordinates.
		ItemToLocate.gameItem.transform.position = position;
		ItemToLocate.CityButton.onClick.AddListener(delegate{GameManager.gameManager.boardScript.ClickOnItem(ItemToLocate);});
	}	



	//Updates the timer rectangle size accoriding to the remaining time.
	public static void updateTimer()
	{
		// timer = GameObject.Find ("Timer").GetComponent<RectTransform> ();
		// timer.sizeDelta = new Vector2 (timerWidth * (GameManager.tiempo / GameManager.totalTime), timer.rect.height);
		if (GameManager.escena != "SetUp" || GameManager.escena == "InterTrialRest" || GameManager.escena == "End") 
		{
			Image timer = GameObject.Find ("Timer").GetComponent<Image> ();
			timer.fillAmount = GameManager.tiempo / GameManager.totalTime;
		}

	}

	public static void highlightButton(GameObject butt)
	{
		Text texto = butt.GetComponentInChildren<Text> ();
		texto.color = Color.gray;
	}

	//Randomizes YES/NO button positions (left or right) and allocates corresponding script to save the correspondent answer.
	//1: No/Yes 0: Yes/No
	public static void RandomizeButtons()
	{
		Button btnLeft = GameObject.Find("LEFTbutton").GetComponent<Button>();
		Button btnRight = GameObject.Find("RIGHTbutton").GetComponent<Button>();

		BoardManager.randomYes=GameManager.buttonRandomization[GameManager.trial-1];

		if (BoardManager.randomYes == 1) 
		{
			btnLeft.GetComponentInChildren<Text>().text = "No";
			btnRight.GetComponentInChildren<Text>().text = "Yes";
		} 
		else 
		{
			btnLeft.GetComponentInChildren<Text>().text = "Yes";
			btnRight.GetComponentInChildren<Text>().text = "No";
		}
	}
		
	//This Initializes the GridPositions which are the possible places where the Items will be placed.
	public static List <Vector3> InitialiseList ()
	{
		List <Vector3> gridPositions = new List<Vector3> ();
		//gridPositions.Clear ();
		//Simple 9 positions grid. 
		for (int y = 0; y < BoardManager.rows; y++) {
			for (int x = 0; x < BoardManager.columns; x++) {	
				float xUnit = (float)(BoardManager.resolutionWidth / 100) / BoardManager.columns;
				float yUnit = (float)(BoardManager.resolutionHeight / 100) / BoardManager.rows;
				//1 x unit = 320x positions in unity, whilst 1 y unit = 336y grid positions in unity
				//gridPositions.Add (new Vector3 ((x-0.8f) * xUnit, (y-0.7619f) * yUnit, 0f)); //- top left value in the right spot, everything else not quite
				gridPositions.Add (new Vector3 ((x) * xUnit, (y+0.4f) * yUnit, 0f));
				Debug.Log ("x" + x + " y" + y);
			}
		}

		return gridPositions;
	}

	//66
	//Returns a random position from the grid and removes the Item from the list.
	//	public static Tuple<int, Vector3> RandomPosition(List <Vector3> gridPositions)
	//	{
	//		int randomIndex=Random.Range(0,gridPositions.Count);
	//		Vector3 randomPosition = gridPositions[randomIndex];
	//		gridPositions.RemoveAt(randomIndex);
	//		return Tuple.Create(randomPosition,gridPositions);
	//	}

}
