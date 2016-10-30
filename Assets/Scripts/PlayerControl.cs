using UnityEngine;
using System.Collections;
/*
 * 
 * Controls movement of player using states
 * 
 * 
 * 
*/

public class PlayerControl : MonoBehaviour {

	[SerializeField] private float rotationSpeed = 3.0f; 		//how fast to rotate
	[SerializeField] private float walkingSpeed = 2.0f; 		//the walking speed
	[SerializeField] private float maxRunningSpeed = 10.0f; 	//get up to this running speed
	[SerializeField] private float dropSpeed = 10.0f; 			//the speed to fall
	[SerializeField] private float jumpSpeed = 10.0f; 			//the speed to fall
	[SerializeField] private float maxJumpHeight = 7.0f;		//the speed to fall
	[SerializeField] private float flightSpeed = 2.0f; 			//flight speed
	[SerializeField] private float staminaChange = 7.5f;
	private float jumping = 0.0f; 								//keeps track of jumping
	private float running = 1.0f;
	private bool grounded = true;								//checks if on ground
	private Vector3 bumpDir;									//finds walls
	private bool bump = false;
	//private bool falling = true;
	private float physicsDampening = 2.0f;
	float moveSpeed = 0.0f;

	// checking floors and walls
	private Collider wall;
	private RaycastHit floor;
	private Vector3 platform = Vector3.zero;
	private bool onMovingPlatform = false;
	private bool bumpRight = false, bumpLeft = false, bumpForward = false, bumpBackward = false;

	//animators
	private Animator animator;

	private State state;

	// communicating with sinking platforms
	private GameObject sinker;
	public GameObject Sinker {
		get{ return sinker; }
	}

	//player states
	public enum State
	{
		Jumping,
		Standing,
		Falling,
		Running,
		Moving,
		Flying,
		Spawn
	}

	// property for other scripts
	public State PlayerState {
		get{ return state; }
	}

	//catches wall bumps
	void OnTriggerEnter(Collider target) {
		bump = true;
		wall = target;
	}

	// Use this for initialization
	void Start () {
		animator = GetComponent <Animator>();
		//state = State.Standing;
		state = State.Spawn;
	}

	//spawn on deaths and restarts
	void SpawnState(){
		if (!grounded) {
			transform.Translate (-transform.up * Time.deltaTime * dropSpeed*0.1f, Space.World);
		} else {
			jumping = 0.0f;
			state = State.Standing;
		}
		if (GetComponent<PlayerCharacter> ().Stamina < GetComponent<PlayerCharacter> ().MaxStamina) {
			Debug.Log ("Stamina not full");
			GetComponent<PlayerCharacter> ().Stamina += Time.deltaTime*staminaChange;
		}
	}

	//on ground and not moving, checks for key presses and changes states
	void standingState() {
		if (Input.GetKey (KeyCode.Space)) {
			state = State.Jumping;
		}
		if (Input.GetKey (KeyCode.UpArrow)) {
			state = State.Moving;
		}
		if (Input.GetKey (KeyCode.LeftArrow)) {
			state = State.Moving;
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			state = State.Moving;
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			state = State.Moving;
		}

		if (Input.GetKey (KeyCode.C)) {
			state = State.Flying;
		}

		if (GetComponent<PlayerCharacter> ().Stamina < GetComponent<PlayerCharacter> ().MaxStamina) {
			GetComponent<PlayerCharacter> ().Stamina += Time.deltaTime*staminaChange;
		}

	}

	////<summary>
	/// makes adjustments for terrain and moving platforms to follow smoothly
	/// </summary>
	void adjustMovement(){
		if (grounded) {
			//Debug.Log ("floor: " + floor.point);
			transform.position = Vector3.MoveTowards (transform.position, new Vector3 (transform.position.x, floor.point.y + 0.3f, transform.position.z), Time.deltaTime * dropSpeed);
		}
		if (onMovingPlatform) {
			transform.position = Vector3.MoveTowards (transform.position, new Vector3 (transform.position.x, floor.point.y + 0.3f, transform.position.z), Time.deltaTime * dropSpeed);
			transform.position += platform;
		}
	}
	////<summary>
	/// jumps to max height then falls
	/// </summary>
	void jumpingState() {
		if (Input.GetKey (KeyCode.Space) && jumping < maxJumpHeight) {
			grounded = false;
			transform.Translate (transform.up * Time.deltaTime * jumpSpeed, Space.World);
			jumping += Time.deltaTime * jumpSpeed;
			if (Input.GetKey (KeyCode.X)) {
				running = maxRunningSpeed;
			} else {
				running = 1.0f;
			}
			if (Input.GetKey (KeyCode.UpArrow) && !bumpForward) {
				transform.Translate (transform.forward * Time.deltaTime * walkingSpeed * running * 3.0f, Space.World);
			} else if (Input.GetKey (KeyCode.UpArrow) && bumpForward) {
				transform.Translate (-transform.forward * Time.deltaTime * walkingSpeed * running * 3.0f, Space.World);
			}

		} else {
			state = State.Falling;
		}
	}

	/// <summary>
	/// flies until up or floats down stamina bar runs out, then falls.
	/// </summary>
	void flyingState() {
		Debug.Log ("stamina: " + GetComponent<PlayerCharacter> ().Stamina);
		if (Input.GetKey (KeyCode.C) && GetComponent<PlayerCharacter> ().Stamina >= 0.01f) {
			grounded = false;
			transform.Translate (transform.up * Time.deltaTime * flightSpeed, Space.World);
			GetComponent<PlayerCharacter> ().Stamina -= Time.deltaTime*staminaChange;

		} else if(!grounded && GetComponent<PlayerCharacter> ().Stamina >= 0.01f) {
			GetComponent<PlayerCharacter> ().Stamina -= 0.5f*Time.deltaTime*staminaChange;
			transform.Translate (-transform.up * Time.deltaTime * flightSpeed, Space.World);
		} else {
			state = State.Falling;
		}

		if (Input.GetKey (KeyCode.X)) {
			running = maxRunningSpeed;
		} else {
			running = 1.0f;
		}
		if (Input.GetKey (KeyCode.UpArrow) && !bumpForward) {
			transform.Translate (transform.forward * Time.deltaTime * walkingSpeed * running, Space.World);
		} else if (Input.GetKey (KeyCode.UpArrow) && bumpForward) {
			transform.Translate (-transform.forward * Time.deltaTime * walkingSpeed * running, Space.World);
		}
		if (Input.GetKey (KeyCode.LeftArrow)) {
			transform.Rotate (0.0f, -rotationSpeed, 0.0f);
		}

		if (Input.GetKey (KeyCode.RightArrow)) {
			transform.Rotate (0.0f, rotationSpeed, 0.0f);
		}

		if (Input.GetKey (KeyCode.DownArrow) && !bumpBackward) {
			transform.Translate (-transform.forward * Time.deltaTime * walkingSpeed * running, Space.World);
		} else if (Input.GetKey (KeyCode.DownArrow) && bumpBackward) {
			transform.Translate (transform.forward * Time.deltaTime * walkingSpeed * running, Space.World);
		}
	}

	/// <summary>
	/// Falls toward ground/water, checks for walls, falls forward if up is pressed
	/// </summary>
	void fallingState() {
		if (!grounded) {
			transform.Translate (-transform.up * Time.deltaTime * dropSpeed, Space.World);
		} else {
			jumping = 0.0f;
			state = State.Standing;
		}
		if (GetComponent<PlayerCharacter> ().Stamina < GetComponent<PlayerCharacter> ().MaxStamina) {
			GetComponent<PlayerCharacter> ().Stamina += Time.deltaTime*staminaChange;
		}
		if (Input.GetKey (KeyCode.C)) {
			state = State.Flying;
		}
		if (Input.GetKey (KeyCode.X)) {
			running = maxRunningSpeed;
		} else {
			running = 1.0f;
		}
		if (Input.GetKey (KeyCode.UpArrow) && !bumpForward) {
			transform.Translate (transform.forward * Time.deltaTime * walkingSpeed * running * 3.0f, Space.World);
		} else if (Input.GetKey (KeyCode.UpArrow) && bumpForward) {
			transform.Translate (-transform.forward * Time.deltaTime * walkingSpeed * running * 3.0f, Space.World);
		}
	}


	/// <summary>
	/// moves or turns based on arrow keys. gains stamina because grounded
	/// </summary>
	void movingState(){
		//adjustMovement ();

		if (GetComponent<PlayerCharacter> ().Stamina < GetComponent<PlayerCharacter> ().MaxStamina) {
			GetComponent<PlayerCharacter> ().Stamina += 0.5f*Time.deltaTime*staminaChange;
		}

		bool didmove = false;
		if (Input.GetKey (KeyCode.X)) {
			running = maxRunningSpeed;
		} else {
			running = 1.0f;
		}

		if (Input.GetKey (KeyCode.UpArrow) && !bumpForward) {
			transform.Translate (transform.forward * Time.deltaTime * walkingSpeed * running, Space.World);
			didmove = true;
		} else if (Input.GetKey (KeyCode.UpArrow) && bumpForward) {
			transform.Translate (-transform.forward * Time.deltaTime * walkingSpeed * running, Space.World);
			didmove = true;
		}

		if (Input.GetKey (KeyCode.LeftArrow)) {
			transform.Rotate (0.0f, -rotationSpeed, 0.0f);
			didmove = true;
		}

		if (Input.GetKey (KeyCode.RightArrow)) {
			transform.Rotate (0.0f, rotationSpeed, 0.0f);
			didmove = true;
		}

		if (Input.GetKey (KeyCode.DownArrow) && !bumpBackward) {
			transform.Translate (-transform.forward * Time.deltaTime * walkingSpeed * running, Space.World);
			didmove = true;
		} else if (Input.GetKey (KeyCode.DownArrow) && bumpBackward) {
			transform.Translate (transform.forward * Time.deltaTime * walkingSpeed * running, Space.World);
			didmove = true;
		}

		if (Input.GetKey (KeyCode.Space)) {
			state = State.Jumping;
			didmove = true;
		}

		if (Input.GetKey (KeyCode.C)) {
			state = State.Flying;
			didmove = true;
		}

		if (!grounded) {
			state = State.Falling;
		} else if (!didmove) {
			state = State.Standing;
			jumping = 0.0f;
		}
	}


	
	// Update is called once per frame
	void Update() {
		if (transform.position.y < -1.0f) {
			//die
			//died
			state = State.Falling;
			GetComponent<PlayerCharacter>().Health = 0.0f;
		}

		//calls functions based on states
		switch (state) {
		case State.Jumping:
			jumpingState ();
			animator.SetFloat ("Forward", 0.0f);
			break;
		case State.Standing:
			standingState ();
			animator.SetFloat ("Forward", 0.0f);
			break;
		case State.Moving:
			movingState ();
			animator.SetFloat ("Forward", 0.25f* walkingSpeed * running);
			break;
		case State.Falling:
			fallingState ();
			animator.SetFloat ("Forward", 0.0f);
			break;
		case State.Flying:
			flyingState (); 
			animator.SetFloat ("Forward", 0.0f);
			break;
		case State.Spawn:
			SpawnState ();
			break;
		}

		// turn on the grounded animations
		if (grounded) {
			animator.SetBool ("OnGround", true);
		} else {
			animator.SetBool("OnGround", false);
		}

		// debugging rays for testing
		Debug.DrawRay (transform.position, transform.forward*10.0f);
		Debug.DrawRay (transform.position, floor.normal*10.0f, Color.green);


	}
	void FixedUpdate () {

		if (grounded){
			adjustMovement (); //call this to handle moving platforms and terrain
		}

		onMovingPlatform = false;
		//use a raycast to detect floors because gravity makes things floppy
		RaycastHit hit;
		if (Physics.Raycast (transform.position, -Vector3.up, out hit)) {
			if (hit.distance < 0.4f && hit.collider.gameObject.tag != "Fallen" 
				&& hit.collider.gameObject.tag != "MidPoint" && hit.collider.gameObject.tag != "Enemy") {
				floor = hit;
				sinker = null;
				if (hit.collider.gameObject.tag == "MovingPlatform") {
					grounded = true;
					onMovingPlatform = true;
					platform = hit.collider.gameObject.GetComponent<MovingPlatform> ().Travel;
					moveSpeed = hit.collider.gameObject.GetComponent<MovingPlatform> ().Speed;
					//need to know if it's a sinking platform
					sinker = hit.collider.gameObject;
				} else if (hit.collider.gameObject.tag != "NotPlatform") {
					grounded = true;
				}
			} else if (hit.distance < 0.2f && hit.collider.gameObject.tag == "Fallen") {
				//died
				state = State.Falling;
				GetComponent<PlayerCharacter>().Health = 0.0f;
		} else {
				grounded = false;
			}
		} else {
			grounded = false;
		}

		if (Physics.Raycast (transform.position, Vector3.forward, out hit)) {
			if (hit.distance < 0.4f) {
				bumpForward = true;
			} else {
				bumpForward = false;
			}
		} else {
			bumpForward = false;
		}

		if (Physics.Raycast (transform.position, -Vector3.forward, out hit)) {
			if (hit.distance < 0.4f && hit.point.y > 0.3) {
				bumpBackward = true; 
			} else {
				bumpBackward = false;
			}
		} else {
			bumpForward = false;
		}

		if (Physics.Raycast (transform.position, -Vector3.right, out hit)) {
			if (hit.distance < 0.4f) {
				bumpLeft = true;
			} else {
				bumpLeft = false;
			}
		} else {
			bumpForward = false;
		}

		if (Physics.Raycast (transform.position, Vector3.right, out hit)) {
			if (hit.distance < 0.4f) {
				bumpRight = true;
			} else {
				bumpRight = false;
			}
		} else {
			bumpForward = false;
		}


	}
}
