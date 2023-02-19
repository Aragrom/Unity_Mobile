///<author> Graham Alexander MacDonald 04/02/2015 </author>
///<author> Calum Brown 04/03/2015 (Updating movement using Acclerometer)</author>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerMovementScript : MonoBehaviour
{
	public float fSpeedMod;
    public float fMoveSpeed = 6.0f; //Used in moving the player 'side-to-side'

    public float fJumpVelocity = 10.0f; //Amount to make player jump in Y-axis

    public float fWALK_SPEED = 8.5f;
    public float fWalkSpeed = 8.5f;
    public float fRUN_SPEED = 13.0f;
    public float fRunSpeed = 13.0f;

    public enum eMovementStates { stationary, walking, running,  jumping, falling };
    public int iState;
    public bool bAutoVelocity = true;
    public float fPositionCheckTimer = 5.0f;
    public Vector3 v3LastPosition;
    public float fVelocitySideToSideLimit = 8.0f;
    public float fVelocityAddedDown = 0.2f;

    public int iDirection; // 0-3 {north, east, south, west}

	public bool bForceJump = false;

    public bool bOnGround; //True when 'collider' on child gameobject "OnGroundController" and attached script "PlayerOnGroundController"

    public bool bGoForward = false; //When true moves the player forward
    public float fForwardSpeed = 8.5f; //Used in moving the player forward

    public bool bIsRotating; //When rotating this is true and disables controls like movement and jump
    public Quaternion qRotation; //Target rotation
    public float fRotateSpeed = 10.0f; //Speed to rotate the player

	public TurnPointScript tpsCheckPoint;

    //Used in respawning the player
    public float fOFF_ROAD_TIMER = 1.5f;
    public float fRespawnTimer;
    public bool bRespawning;

	private float fLeanSensitivity = 5.0f;

    private PlayerScript ps;
    private Rigidbody r;

    /// <summary> Use this for initialization </summary>
	void Start ()
    {
        LinkComponents();
        v3LastPosition = new Vector3(0,-50,0); //supposed to be out so if lagging and trying to check when not moved doesnt return true
		AdjustSpeeds ();
		Invoke ("TriggerPlayerRunning", 2.0f);
	}

	void TriggerPlayerRunning()
	{
		bGoForward = true;
	}

    /// <summary> Update is called once per frame </summary>
	void Update ()
    {
		if (fPositionCheckTimer > 0 && !ps.lgs.gds.bPromptForName) fPositionCheckTimer -= Time.deltaTime;
        else
        {
			if(!ps.lgs.gds.bPromptForName && !ps.pgs.bRewardScreen) CheckPositionForReset();
        }
        
        if (bAutoVelocity) AutoVelocity();

        //If player has activated the analog stick
		if (GetAnalogStickState() && !bIsRotating && !ps.pgs.bRewardScreen && !ps.lgs.gds.bPromptForName)
        {
            AdjustVelocity();
        }

        //If using the gyroscope for input
		if (ps.mis.bUsingGyroInput && bGoForward && !ps.pgs.bRewardScreen && !ps.lgs.gds.bPromptForName) 
        {
            //adjust velocity of player relative to the direction of the gyroscope
            if(!bIsRotating) AdjustVelocityWithGyro();

            //If the direction string is null set the sideways velocity to null
			if (!bIsRotating && ps.mis.strGyroDirection == null)
            {
                if (iDirection == (int)TurnPointScript.eDirection.north) r.velocity = new Vector3(r.velocity.x, r.velocity.y, 0);
                else if (iDirection == (int)TurnPointScript.eDirection.south) r.velocity = new Vector3(r.velocity.x, r.velocity.y, -0);
                else if (iDirection == (int)TurnPointScript.eDirection.east) r.velocity = new Vector3(0, r.velocity.y, r.velocity.z);
                else if (iDirection == (int)TurnPointScript.eDirection.west) r.velocity = new Vector3(0, r.velocity.y, r.velocity.z);
            }
        }

		if (bForceJump) {
			Jump();
			bForceJump = false;
		}

        //If below certain point restart level
		if (transform.position.y > 100.0f 
		    || transform.position.y < -100.0f
		    || transform.position.x > 1000.0f
		    || transform.position.x < -1000.0f
		    || transform.position.z > 1000.0f
		    || transform.position.z < -1000.0f)
            if(Application.loadedLevelName == "GameScene") RespawnPlayer();
            else if (Application.loadedLevelName == "TutorialScene") RespawnPlayer ();

        if (bGoForward && !ps.pgs.bRewardScreen && !ps.lgs.gds.bPromptForName) MoveForward();

        StateUpdate();

        if (bIsRotating) RotatePlayer();

        if (ps.pogs.bOffRoad) RespawnTimer();
	}

    /// <summary> Linking component interaction </summary>
    void LinkComponents()
    {
        ps = this.gameObject.GetComponent<PlayerScript>();
        r = this.gameObject.GetComponent<Rigidbody>();
    }

	void AdjustSpeeds()
	{
		if (ps.lgs.gds.iLoopCount > 0)
			fSpeedMod = (float)ps.lgs.gds.iLoopCount / 2;
		else
			fSpeedMod = 0;

		if (fSpeedMod > 10)	fSpeedMod = 10;

		fForwardSpeed += fSpeedMod;
		fMoveSpeed += fSpeedMod;
		fWALK_SPEED += fSpeedMod;
		fWalkSpeed += fSpeedMod;
		fRUN_SPEED += fSpeedMod;
		fRunSpeed += fSpeedMod;
		fRotateSpeed += fSpeedMod;
	}

    void AutoVelocity()
    {
		if (GetComponent<Rigidbody>().velocity.y < 0) GetComponent<Rigidbody>().velocity += Vector3.down * fVelocityAddedDown;
    }

    void RespawnTimer()
    {
        if (fRespawnTimer > 0) fRespawnTimer -= Time.deltaTime;
        else RespawnPlayer();
    }

    void RespawnPlayer()
    {
		Vector3 v3 = new Vector3 ();

		if (tpsCheckPoint == null)
		{			
			v3 = ps.lgs.LgoLevel[0].gameObject.transform.position;
			iDirection = 0;
		}
		else 
		{
			v3 = tpsCheckPoint.gameObject.transform.position; //Get Turn Point position
			iDirection = tpsCheckPoint.iDirection;
		}

        ReadyRotation(); //Rotate Player

        //Move Player
        this.transform.position = new Vector3(v3.x, v3.y + 3, v3.z);
        r.velocity = Vector3.zero;
		ps.pcs.iScore = 0;        

        ps.pogs.bOffRoad = false;
    }

    void CheckPositionForReset()
    {
        Vector3 v3New = new Vector3((int)this.gameObject.transform.position.x,
            (int)this.gameObject.transform.position.y,
            (int)this.gameObject.transform.position.z);
		if (v3LastPosition == v3New && !ps.lgs.gds.bPromptForName)
		{

			if(Application.loadedLevelName == "GameScene")
			{
				ps.lgs.gds.iTotalScore += ps.pcs.iScore;
				ps.lgs.gds.iCharges = ps.pcs.iCharges;
				ps.lgs.gds.iHealth = ps.pcs.iCurrentHealth;
				Application.LoadLevel ("GameScene");
			}
			if(Application.loadedLevelName == "TutorialScene") RespawnPlayer();
		}

        v3LastPosition = v3New;
        fPositionCheckTimer = 2.0f;
	}
	
    List<Transform> SortTurnPointByDistance(List<Transform> Lt)
    {
        Lt.Sort(delegate(Transform t1, Transform t2)
        {
            return Vector3.Distance(t1.position, this.transform.position)
        .CompareTo(Vector3.Distance(t2.position, this.transform.position));
        });
        return Lt;
    }

    /// <summary> Get/Set analog stick state </summary>
    /// <returns> analog stick being used state </returns>
    bool GetAnalogStickState()
    {
        bool b = ps.mis.bHasAnalogStick;
        return b;
    }

    /// <summary> Uses Touch Input v2AnalogAxis to update the player velocity left and right </summary>
    void AdjustVelocity()
    {
        //Holds normalized Analog Vector multiplied by speed
        Vector2 v2 = -ps.mis.v2AnalogAxis * fMoveSpeed;

        if (iDirection == (int)TurnPointScript.eDirection.north) r.velocity = new Vector3(r.velocity.x, r.velocity.y, v2.x);
        else if (iDirection == (int)TurnPointScript.eDirection.south) r.velocity = new Vector3(r.velocity.x, r.velocity.y, -v2.x);
        else if (iDirection == (int)TurnPointScript.eDirection.east) r.velocity = new Vector3(v2.x, r.velocity.y, r.velocity.z);
        else if (iDirection == (int)TurnPointScript.eDirection.west) r.velocity = new Vector3(-v2.x, r.velocity.y, r.velocity.z);

        if (ps.pms.iState != (int)eMovementStates.running
            && ps.pms.iState != (int)eMovementStates.jumping)
        {
            ps.pms.iState = (int)eMovementStates.walking;
        }
    }

	void AdjustVelocityWithGyro()
	{
		float fGyroAngle = ps.mis.fLeanAmount;
		if (fGyroAngle < 0)
		{
			fGyroAngle = -fGyroAngle;
		}
		//Get the sGyroDirection string from the TouchInputScript
		string direction = ps.mis.strGyroDirection;
		switch (direction)
		{
			//If direction == Left add left velocity taking into account travel direction
		case"Left":
				if (iDirection == (int)TurnPointScript.eDirection.north) r.velocity = new Vector3(r.velocity.x, r.velocity.y, fMoveSpeed * fGyroAngle * fLeanSensitivity);
			else if (iDirection == (int)TurnPointScript.eDirection.south) r.velocity = new Vector3(r.velocity.x, r.velocity.y, -fMoveSpeed * fGyroAngle * fLeanSensitivity);
			else if (iDirection == (int)TurnPointScript.eDirection.east) r.velocity = new Vector3(fMoveSpeed * fGyroAngle * fLeanSensitivity, r.velocity.y, r.velocity.z);
			else if (iDirection == (int)TurnPointScript.eDirection.west) r.velocity = new Vector3(-fMoveSpeed * fGyroAngle * fLeanSensitivity, r.velocity.y, r.velocity.z);
			break;
			//If direction == Right add right velocity taking into account travel direction
		case"Right":
				if (iDirection == (int)TurnPointScript.eDirection.north) r.velocity = new Vector3(r.velocity.x, r.velocity.y, -fMoveSpeed * fGyroAngle * fLeanSensitivity);
			else if (iDirection == (int)TurnPointScript.eDirection.south) r.velocity = new Vector3(r.velocity.x, r.velocity.y, fMoveSpeed * fGyroAngle * fLeanSensitivity);
			else if (iDirection == (int)TurnPointScript.eDirection.east) r.velocity = new Vector3(-fMoveSpeed * fGyroAngle * fLeanSensitivity, r.velocity.y, r.velocity.z);
			else if (iDirection == (int)TurnPointScript.eDirection.west) r.velocity = new Vector3(fMoveSpeed * fGyroAngle * fLeanSensitivity, r.velocity.y, r.velocity.z);
			break;
		}
	}

	public void AdjustVelocityWithButtons(string strButtonDir)
	{
		if (strButtonDir != "jump") {
			float f = 0;
			if (strButtonDir == "left") {
				f = -1 * fMoveSpeed;
			}
			if (strButtonDir == "right")
				f = 1 * fMoveSpeed;
			
			if (iDirection == (int)TurnPointScript.eDirection.north
				|| iDirection == (int)TurnPointScript.eDirection.south)
				r.velocity = new Vector3 (r.velocity.x, r.velocity.y, f);
			else if (iDirection == (int)TurnPointScript.eDirection.east
				|| iDirection == (int)TurnPointScript.eDirection.west)
				r.velocity = new Vector3 (f, r.velocity.y, r.velocity.z);
		} else
			ps.pms.Jump ();
		
	}
	
	/// <summary> Moves the player positively in the x-axis using velocity </summary>
	public void MoveForward()
	{
		if(iDirection == (int)TurnPointScript.eDirection.north) r.velocity = new Vector3(fForwardSpeed, r.velocity.y, r.velocity.z);
        else if (iDirection == (int)TurnPointScript.eDirection.south) r.velocity = new Vector3(-fForwardSpeed, r.velocity.y, r.velocity.z);
        else if (iDirection == (int)TurnPointScript.eDirection.east) r.velocity = new Vector3(r.velocity.x, r.velocity.y, -fForwardSpeed);
        else if (iDirection == (int)TurnPointScript.eDirection.west) r.velocity = new Vector3(r.velocity.x, r.velocity.y, fForwardSpeed);
    }

    /// <summary> Moves the player positively in the Y-axis using velocity (instant velocity) </summary>
    public void Jump()
    {
        //Jump
        r.velocity = new Vector3(r.velocity.x, fJumpVelocity, r.velocity.z);
        //ps.pcs.bSlamming = false;

        iState = (int)eMovementStates.jumping;
    }

    /// <summary> </summary>
    private void CheckRotation()
    {
        Vector3 myRotation = transform.rotation.eulerAngles; //get Vector3 rotation x, y, z

        #region Check Y Rotation near 0-90-180-270-360
        if (myRotation.y > -0.5 && myRotation.y < 0.5)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            bIsRotating = false;
        }
        if (myRotation.y > 89.5 && myRotation.y < 90.5)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
            bIsRotating = false;
        }
        if (myRotation.y > 179.5 && myRotation.y < 180.5)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            bIsRotating = false;
        }
        if (myRotation.y > 269.5 && myRotation.y < 270.5)
        {
            transform.rotation = Quaternion.Euler(0, 270, 0);
            bIsRotating = false;
        }
        if (myRotation.y > 359.0 && myRotation.y < 360.0)
        {
            transform.rotation = Quaternion.Euler(0, 360, 0);
            bIsRotating = false;
        }
        #endregion
    }

    /// <summary> </summary>
    public void ReadyRotation()
    {
        if (iDirection == (int)TurnPointScript.eDirection.north) qRotation = Quaternion.Euler(0, 0, 0);
        else if (iDirection == (int)TurnPointScript.eDirection.east) qRotation = Quaternion.Euler(0, 90, 0);
        else if (iDirection == (int)TurnPointScript.eDirection.south) qRotation = Quaternion.Euler(0, 180, 0);
        else if (iDirection == (int)TurnPointScript.eDirection.west) qRotation = Quaternion.Euler(0, 270, 0);

        bIsRotating = true; //set rotating true in update()
    }

    /// <summary> </summary>
    private void RotatePlayer()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, qRotation, Time.deltaTime * fRotateSpeed);
        CheckRotation();
    }

    /// <summary> </summary>
    public void StateUpdate()
    {
        switch(iState)
        {
            case (int)eMovementStates.walking:
                fForwardSpeed = fWalkSpeed;
                break;

            case (int)eMovementStates.running:
                fForwardSpeed = fRunSpeed;
                break;

            default:
                break;
        }
    }

	void SetCheckPoint(TurnPointScript tps)
	{
		//Set all false;
		GameObject[] aGo = GameObject.FindGameObjectsWithTag("TurnPoint");

		foreach (GameObject go in aGo)
		{
			Debug.Log (go.name);
			if(go.GetComponent<TurnPointScript>().bIsCheckPoint) go.GetComponent<TurnPointScript>().bIsCheckPoint = false;
		}

		//Set this check point true
		tps.bIsCheckPoint = true;
		tpsCheckPoint = tps;
	}

    void OnTriggerEnter(Collider c)
    {
        if(c.tag == "TurnPoint"
		   || c.name == "TurnPoint(Clone)"
		   || c.name == "TurnPoint")
        {
			TurnPointScript tps = c.gameObject.GetComponent<TurnPointScript>();
			Debug.Log ("Passed tps" + tps.gameObject.name);
			SetCheckPoint(tps);
			iDirection = tps.iDirection;
            ReadyRotation();
            //c.GetComponent<BoxCollider>().enabled = false;
            //c.GetComponent<Renderer>().enabled = false;
        }
    }
}
