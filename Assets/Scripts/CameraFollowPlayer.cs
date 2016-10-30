using UnityEngine;
using System.Collections;
/*
 * 
 * follow the player with a smooth rotation
 * 
*/
public class CameraFollowPlayer : MonoBehaviour {

	[SerializeField] private GameObject playerObject; //add the player to follow
	[SerializeField] private float cameraOffset = 5.0f; //offset from player
	[SerializeField] private float cameraHeight = 5.0f; //offset from player
	[SerializeField] float heightDamping = 100.0f;
	[SerializeField] float rotationDamping = 100.0f;
	private float currentHeight;
	private float currentAngle;
	private Quaternion currentRotation;
	private PlayerControl player;

	// Use this for initialization
	void Start () {
		player = playerObject.GetComponent<PlayerControl> ();
	}
	
	// Update is called once per frame
	void Update () {


		//took this from here http://answers.unity3d.com/questions/309481/smooth-follow-c.html
		currentAngle = Mathf.LerpAngle (transform.eulerAngles.y, playerObject.transform.eulerAngles.y, rotationDamping * Time.deltaTime);
		currentHeight = Mathf.Lerp (transform.position.y, playerObject.transform.position.y + cameraHeight, heightDamping * Time.deltaTime);
		currentRotation = Quaternion.Euler (0, currentAngle, 0);



		//adjusted because going up and down on jumps was very jarring
		if (player.PlayerState != PlayerControl.State.Jumping && player.PlayerState != PlayerControl.State.Falling) {
			transform.position = playerObject.transform.position - (currentRotation * Vector3.forward * cameraOffset);
			transform.position = new Vector3 (transform.position.x, currentHeight, transform.position.z);
			transform.LookAt (new Vector3 (playerObject.transform.position.x, playerObject.transform.position.y + 2.0f, playerObject.transform.position.z)); //look at the player
		} else {
			float currentY = transform.position.y;
			transform.position = playerObject.transform.position - (currentRotation * Vector3.forward * cameraOffset);
			transform.position = new Vector3(transform.position.x, currentY, transform.position.z);
		}

	}
}
