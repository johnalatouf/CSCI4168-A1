using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/*
 * 
 * basic life tracking
 * restarts game on gameover or win
 * 
*/
public class Lives : MonoBehaviour {

	//[SerializeField] private int TotalLives = 5;
	[SerializeField] private Text lifeText;					// text that displays lives
	[SerializeField] private GameObject gameOverGraphic;	// UI panel for game over
	private GameObject scripts;
	private int lives;

	public int RemainingLives {
		set{ lives = value; }
		get{ return lives; }
	}

	// Use this for initialization
	void Start () {
		//RemainingLives = TotalLives;
		scripts = GameObject.FindWithTag("NonDestruct");				//get life info from the non-dustructable
		RemainingLives = scripts.GetComponent<Record>().SavedLives;
		lifeText.text = RemainingLives.ToString();
		gameOverGraphic.SetActive(false);
	}

	// resets lives and sets back to scene L01
	public void RestartGame(){
		Debug.Log ("Restart");
		scripts.GetComponent<Record>().SavedLives = scripts.GetComponent<Record>().TotalLives;
		SceneManager.LoadScene("L01");


	}
	
	// Update is called once per frame
	void Update () {

		lifeText.text = RemainingLives.ToString();

		// when lives run out, go to game-over graphic
		if (RemainingLives == 0) {
			gameOverGraphic.SetActive(true);
		}
	}
}
