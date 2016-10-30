using UnityEngine;
using System.Collections;
/*
 * 
 * yellow turtles sink when stepped on
 * move back up when stepped off
 * 
*/
public class SinkingPlatform : MonoBehaviour {

	[SerializeField] private GameObject playerObject; //add the player to sink
	[SerializeField] private float startingY = 0.0f;
	[SerializeField] private float sinkSpeed = 0.9f;
	private PlayerControl player;
	private bool onHere = false;

	// Use this for initialization
	void Start () {
		player = playerObject.GetComponent<PlayerControl> ();
	}
	
	// Update is called once per frame
	void Update () {
		onHere = (player.Sinker != null && player.Sinker.name == gameObject.name);
	}
	void FixedUpdate () {
	
		//sinks the platform when the player lands on it
		Vector3 target = GetComponent<MovingPlatform> ().CurrentTarget;
		if (onHere && (player.PlayerState == PlayerControl.State.Standing || player.PlayerState == PlayerControl.State.Moving)) {
			transform.position = new Vector3(transform.position.x, transform.position.y - Time.deltaTime * sinkSpeed, transform.position.z);
			transform.rotation = Quaternion.Euler( new Vector3(transform.rotation.x - Time.deltaTime * sinkSpeed, transform.rotation.y, transform.rotation.z));

		} else if (!onHere && transform.position.y < startingY) {
			transform.position = new Vector3(transform.position.x, transform.position.y + Time.deltaTime * sinkSpeed, transform.position.z);
			transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.x+ Time.deltaTime * sinkSpeed, transform.rotation.y, transform.rotation.z));
		}
	}
}
