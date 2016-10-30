using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/*
 * 
 * Controls health and items
 * 
 * 
 * 
*/
public class PlayerCharacter : MonoBehaviour {

	// health and stamina variables
	[SerializeField] private float totalHealth = 100.0f;
	[SerializeField] private float totalStamina = 30.0f;
	[SerializeField] private Vector3 spawnPoint = Vector3.zero;

	// the sliders for the UI
	[SerializeField] private Slider staminaSlider;
	[SerializeField] private Slider healthSlider;
	[SerializeField] private GameObject scripts;

	//used for end game popup
	private GameObject nondestruct;
	[SerializeField] private GameObject winGraphic;
	private bool gameFinished = false;

	// projectile variables
	[SerializeField] private float glowSpeed = 1000.0f;
	[SerializeField] private float glowInterval = 0.5f;
	private float glowTime = 0.0f;

	//for shooting
	[SerializeField] private Rigidbody glow;

	//keep track of health and stamina
	private float health;
	private float stamina;

	//access the health if necessary
	public float Health {
		set{ health = value; }
		get{ return health; }
	}

	public float Stamina {
		set{ stamina = value; }
		get{ return stamina; }
	}

	public float MaxStamina {
		set{ totalStamina = value; }
		get{ return totalStamina; }
	}

	void  OnTriggerEnter (Collider col)
	{
		print ("COLLISION");
		if(col.gameObject.tag == "Enemy" && !gameFinished)
		{
			print ("ENEMY COLLISION");
			//Health -= 10.0f;
			Health -= col.gameObject.GetComponent<EnemyAI>().HitPoints;
			print ("health: " + health);
		}
		if(col.gameObject.tag == "HealthBoost")
		{
			print ("Health COLLISION");
			Health += 30.0f;
			print ("health: " + health);
			Destroy (col.gameObject);
		}
		if(col.gameObject.tag == "StaminaBoost")
		{
			print ("STAMINA COLLISION");
			Stamina += 10.0f;
			print ("stamina: " + stamina);
			Destroy (col.gameObject);
		}
		if(col.gameObject.tag == "MidPoint")
		{
			print ("Middle point");
			spawnPoint = new Vector3 (col.gameObject.transform.position.x, col.gameObject.transform.position.y + 1.0f, col.gameObject.transform.position.z);

		}
//		if (col.gameObject.tag == "Fallen") {
//			Health = 0.0f;
//			print ("DYING");
//		}
		if (col.gameObject.tag == "Finish") {
			Stamina = totalStamina;
			print ("Finish!");
			//go to next level
//			Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
//			if (scene.name == "L01") {
//				SceneManager.LoadScene ("L02", LoadSceneMode.Additive);
//			} else {
//				//game done!
//			}
			int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
			if (SceneManager.sceneCountInBuildSettings > nextSceneIndex) {
				SceneManager.LoadScene (nextSceneIndex);
			} else {
				// show the win graphic
				winGraphic.SetActive(true);
			}
		}
	}

	/// <summary>
	/// shoot a glow that is destroyed after time
	/// </summary>
	void shoot(){
		
		if (Input.GetKey(KeyCode.S))
		{
			if (Time.time >= glowTime) {
				Rigidbody glow_shoot = Instantiate (glow, new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), transform.rotation) as Rigidbody;
				//send forward
				glow_shoot.AddForce (transform.forward * glowSpeed);
				Destroy (glow_shoot.gameObject, 2.0f);


				glowTime = Time.time + glowInterval;
			}
		}
	}

	// Use this for initialization
	void Start () {
		Health = totalHealth;
		Stamina = totalStamina;
		winGraphic.SetActive(false);
	}

	// Update is called once per frame
	void Update () {

		shoot ();

		staminaSlider.value = Stamina;
		healthSlider.value = Health;

		// check for 0 health
		if (Health <= 0.0f) {
			transform.position = spawnPoint;
			Health = totalHealth; 	// reset bars
			Stamina = totalStamina;
			scripts.GetComponent<Lives> ().RemainingLives--;	//take away life
			nondestruct = GameObject.FindWithTag("NonDestruct");	//send life # to the next scene
			nondestruct.GetComponent<Record>().SavedLives = scripts.GetComponent<Lives> ().RemainingLives;
		}
	}
}
