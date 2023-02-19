///<author> Graham Alexander MacDonald 14/03/2015 </author>

using UnityEngine;
using System.Collections;

public class TutorialObjectScript : MonoBehaviour {

	public GameDataScript gds;
	public PlayerScript ps;
	public string strMessageToPlayer;
	public bool bTimeToDestroySelf = false;
	public float fTimer = 10.0f;
	public string strSideToDisplay = "";
	private bool bSendMessage = false;
	// Use this for initialization
	void Start ()
	{

	}
	
	// Update is called once per frame
	void Update ()
	{
		if(ps != null)
		{
			gds = GameObject.Find("GameData").GetComponent<GameDataScript>();
			if (gds.strInputChoice == "gyro" && gameObject.transform.parent.name == "TutJump(Clone)")
			{
				if (bSendMessage == true)
				{
					bSendMessage = false;
					ps.pgs.CreateTutAnimation("Jump");
				}
				strMessageToPlayer = "Press anywhere to JUMP";
				strSideToDisplay = "centered";
			}
			else if(gameObject.transform.parent.name == "TutJump(Clone)") 
			{
				strMessageToPlayer = "Press anywhere on the RIGHT side of the screen to JUMP";
				strSideToDisplay = "right";
			}

			if(gameObject.transform.parent.name == "TutMovingBlocked(Clone)") strMessageToPlayer = "Dodge obstacles to not take damage";
			if(gameObject.transform.parent.name == "TutMovingLeftRight(Clone)") strMessageToPlayer = "Jump the gap or use platform to move across";
		}

		if (bTimeToDestroySelf)
		{
			if(fTimer >= 0) fTimer -= Time.deltaTime;
			else
			{
				ps.pms.bGoForward = true; // allow player to move again
				Destroy(this.gameObject); // delete this collider and script. Keep event
			}
		}
	}

	void OnTriggerEnter(Collider c)
	{
		if (c.tag == "Player") 
		{
			bSendMessage = true;

			// CAN STOP PLAYER BELOW (COMMENTED OUT)

			//ps.pms.bGoForward = false; //Stop player
			//ps.rigidbody.velocity = Vector3.zero;
			//ps.pms.fPositionCheckTimer = 15.0f; //so when stopped you dont respawn

			if(strSideToDisplay == "left")
			{
				ps.pgs.bDisplayOnLeft = true;
				ps.pgs.bDisplayOnRight = false;
			}
			if(strSideToDisplay == "right")
			{
				ps.pgs.bDisplayOnRight = true;
				ps.pgs.bDisplayOnLeft = false;
			}
			if(strSideToDisplay == "centered")
			{
				ps.pgs.bDisplayOnRight = false;
				ps.pgs.bDisplayOnLeft = false;
			}

			ps.pgs.bHasMessageToDisplay = true; // Set bool true in GUI script
			ps.pgs.strMessageToPlayer = strMessageToPlayer; // message to diaplay
			ps.pgs.goDisplayOn = gameObject.transform.parent.gameObject; //Display message above this object

			fTimer = 5.0f; //Timer to destroy this event message
		}
	}
}
