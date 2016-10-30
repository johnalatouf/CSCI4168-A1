using UnityEngine;
using System.Collections;

//keeps track of lives and things through scenes

public class Record : MonoBehaviour {
	[SerializeField] private int totalLives = 5;
	private int savedLives = 5;
	private static Record _instance ;

	public int SavedLives {
		set{ savedLives = value; }
		get{ return savedLives; }
	}

	public int TotalLives {
		get{ return totalLives; }
	}

	void Awake()
	{
		// this checks if the object already exists and destroys
		if (!_instance) {
			_instance = this;
		} else {
			Destroy (this.gameObject);
		}

		// carry this over to level 2
		DontDestroyOnLoad(this.gameObject) ;
	}

	// Use this for initialization
	void Start () {
		SavedLives = totalLives;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
