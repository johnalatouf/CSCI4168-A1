using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Any type of UI related functions
/// </summary>

public class MiscUI : MonoBehaviour {
	
	[SerializeField] private Text helpText;
	[SerializeField] private GameObject helpMenu;


	// Use this for initialization
	void Start () {
		helpText.text = "?";
		helpMenu.SetActive (false);
	}


	/// <summary>
	/// make the help menu appear
	/// </summary>
	public void ToggleHelpMenu(){
		if (helpText.text == "?") {
			helpText.text = "X";
			helpMenu.SetActive (true);
		} else {
			helpText.text = "?";
			helpMenu.SetActive (false);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
