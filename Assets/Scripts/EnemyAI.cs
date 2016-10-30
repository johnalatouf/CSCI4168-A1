using UnityEngine;
using System.Collections;
/*
 * 
 * enemies fly back at forth at random within a set radius
 * they inflict damage on intersection
 * lose life points and are destroyed by shooting
 * 
*/
public class EnemyAI : MonoBehaviour {
	//speed to move
	[SerializeField] private float speed = 5.0f;				// flight speed
	[SerializeField] private float hitPoints = 15.0f;			// damage to player
	// the space in which it can travel
	[SerializeField] private Vector3 center = Vector3.zero;		// center of the radius
	[SerializeField] private float radius = 3.0f;

	// for when it is shot
	[SerializeField] private float lifePoints = 30.0f;
	[SerializeField] private float damage = 10.0f;
	private float life;

	[SerializeField] private GameObject player;
	private Collider col;
	private bool wallHit = false;
	private bool playerChase = false;							// if the player is nearby
	private Animator animator;									// for animating

	//positions
	private float x;
	private float z;
	private float change = 0.0f;
	private float angle = 0.0f;
	private float current;

	// property to get hitpoints
	public float HitPoints {
		get{ return hitPoints; }
	}

	// Use this for initialization
	void Start () {
		animator = GetComponent <Animator>();
		transform.position = center;
		transform.Rotate(0, Random.Range(-180.0f, 180.0f), 0);
		life = lifePoints;
	}

	/// <summary>
	/// accept damage when hit by glows
	/// </summary>
	/// <param name="col">Col.</param>
	void  OnTriggerEnter (Collider col)
	{
		print ("BEE COLLISION");
		if(col.gameObject.tag == "Glow")
		{
			print ("ENEMY HIT");
			life -= damage;
			print ("bee health: " + life);
		}
	}
		
	// Update is called once per frame
	void Update () {

		if (life <= 0) {
			Destroy (gameObject);
		}
		
		if ((player.transform.position.x - center.x)*(player.transform.position.x - center.x) 
			+ (player.transform.position.z - center.z)*(player.transform.position.z - center.z) <= radius*radius) {
			// the player is within the radius, attack
			wallHit = false;
			animator.SetBool ("Attack", true);
			playerChase = true;
			Vector3 dir = (new Vector3 (player.transform.position.x, player.transform.position.y + 1.0f, player.transform.position.z) - transform.position).normalized;
			Quaternion rot = Quaternion.LookRotation (dir);
			transform.rotation = Quaternion.Slerp (transform.rotation, rot, Time.deltaTime * speed);
		} else if((transform.position.x - center.x)*(transform.position.x - center.x) 
			+ (transform.position.z - center.z)*(transform.position.z - center.z) >= radius*radius && !wallHit) {
			// hit the circumference, so turn around
			transform.Translate (-2.0f * transform.forward * Time.deltaTime * speed, Space.World);
			current = transform.rotation.eulerAngles.y;
			if (current > 180.0f) {
				change = Random.Range (-180.0f, -120.0f);
			} else {
				change = Random.Range (120.0f, 180.0f);
			}
			playerChase = false;
			wallHit = true;
		}

		// the rotation is slerped based on turning direction
		if (wallHit && current > 180.0f && transform.rotation.eulerAngles.y > current + change) {
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + change, transform.rotation.eulerAngles.z), Time.deltaTime * speed);
		} else if (wallHit && current < 180.0f && transform.rotation.eulerAngles.y < current + change) {
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y + change, transform.rotation.eulerAngles.z), Time.deltaTime * speed);
		} else {
			wallHit = false;
			transform.Translate (transform.forward * Time.deltaTime * speed, Space.World);
		}

		// otherwise just fly forward
		if (!playerChase) {
			transform.position = Vector3.MoveTowards (transform.position, new Vector3 (transform.position.x, center.y, transform.position.z), Time.deltaTime * speed);
		}
	}
}
