using UnityEngine;
using System.Collections;
/*
 * 
 * moves platform
 * planned to make different kinds of platforms, but for now only linear with turning works
 * 
 * 
*/
public class MovingPlatform : MonoBehaviour {

	//type of movement
	private enum typeEnum{random, linear, LinearWithTurning};
	[SerializeField] private typeEnum movementType;
	[SerializeField] private float speed = 3.0f;
	[SerializeField] private float turnSpeed = 1.0f;
	[SerializeField] private float radius = 5.0f;
	[SerializeField] private Vector3 start = new Vector3(0.0f, 0.0f, 0.0f);
	[SerializeField] private Vector3 end = new Vector3(1.0f, 1.0f, 1.0f);
	// variables for direction
	private Vector3 direction;
	private bool forward;

	// variables for communicating direction
	private Vector3 travel;
	private Vector3 lastposition;

	// states
	private enum MoveState{moving, rotating};
	private MoveState state;

	//positions
	private float x;
	private float z;
	private float change = 0.0f;
	private float angle = 0.0f;
	private float current;
	private bool wallHit = false;

	//keep track of the travel of the thing
	public Vector3 Travel {
		set{ travel = value; }
		get{ return travel; }
	}

	public float Speed {
		set{ speed = value; }
		get{ return speed; }
	}
	private Vector3 currentTarget;
	public Vector3 CurrentTarget{
		get{ return currentTarget; }
	}

	// Use this for initialization
	void Start () {
		if (movementType == typeEnum.random) {
			transform.Rotate (0, Random.Range (-180.0f, 180.0f), 0);
		} else if (movementType == typeEnum.linear) {
			transform.position = start;
		} else if (movementType == typeEnum.LinearWithTurning) {
			forward = true;
			state = MoveState.moving;
			transform.position = start;
			transform.LookAt (end);
		} 


		lastposition = transform.position;
	}

	private void MovingState(){
		if (forward) {
			//Debug.Log ("forward");
			transform.position = Vector3.MoveTowards (transform.position, end, speed * Time.deltaTime);
			if (Mathf.Abs(transform.position.x - end.x) < 0.3f
				&& Mathf.Abs(transform.position.y - end.y) < 0.3f && Mathf.Abs(transform.position.z - end.z) < 0.3f) {
				state = MoveState.rotating;
			}
		} else {
			//Debug.Log ("backward");
			transform.position = Vector3.MoveTowards (transform.position, start, speed * Time.deltaTime);
			if (Mathf.Abs(transform.position.x - start.x) < 0.3f
				&& Mathf.Abs(transform.position.y - start.y) < 0.3f && Mathf.Abs(transform.position.z - start.z) < 0.3f) {
				//state = MoveState.rotating;
				state = MoveState.rotating;

			}
		}

	}

	private void RotatingState(){
		Vector3 target;
		//target = end - transform.position;
		Debug.DrawRay (transform.position, 10.0f * transform.forward, Color.magenta);


		if (forward) {
			target = start - transform.position;
			Debug.DrawRay (transform.position, 10.0f * target, Color.red);
		} else {
			target = end - transform.position;
			Debug.DrawRay (transform.position, 10.0f * target, Color.red);
		}

		//transform.rotation = Quaternion.Euler (target);


		//transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(target), turnSpeed*Time.deltaTime);
		Vector3 rot = Vector3.RotateTowards(transform.forward, target, turnSpeed*Time.deltaTime, 1.0f);
		transform.rotation = Quaternion.LookRotation (rot);

		float angle = Vector3.Angle (target, transform.forward);
		if (angle < 1.0f) {
			transform.LookAt(target + transform.position);
			forward = !forward;
			state = MoveState.moving;
		}
	}

	// Update is called once per frame
	void FixedUpdate () {

		if (forward) {
			currentTarget = end;
		} else {
			currentTarget = start;
		}

		switch (state) {
		case MoveState.moving:
			MovingState ();
			break;
		case MoveState.rotating:
			RotatingState ();
			break;
		}

		//keep track of travel
		travel = transform.position - lastposition;
		lastposition = transform.position;
	}
}
