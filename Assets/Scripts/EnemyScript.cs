///<author> Graham Alexander MacDonald 20/02/2015 </author>

using UnityEngine;
using System.Collections;

public class EnemyScript : MonoBehaviour
{
    public GameObject goTarget;
    public PlayerScript psTarget;
    public float fMaxDistanceFromTargetX = 50.0f;
    public float fMaxDistanceFromTargetY = 50.0f;
    public float fMaxDistanceFromTargetZ = 50.0f;

    public float fDistanceToBeginChaseX = 10.0f;
    public float fDistanceToBeginChaseY = 5.0f;
    public float fDistanceToBeginChaseZ = 10.0f;
    
    public enum eEnemyStates { patrolling, chasing, blocking };
    public enum eEnemyBehaviours { walking, running, attacking, jumping };

    public int iEnemyState;
    public int iEnemyBehaviour;

    public Color colorStart;
    public Color colorEnd;
    public float fDuration;

    public float fDecisionTimer;
    public bool bUpdateTimer;

    public Vector3 v3MoveDirection;
    public float fMoveSpeed = 4.0f;

    public int iEnemyId;

    public float fDistanceWhenToJump = 3.0f;
    public float fJumpVelocity = 7.0f;

	public bool bInFrontOfPlayer = true;
	public float fDeathTimer = 1.0f;

    public LevelGeneratorScript lgs;
    private Rigidbody r;

    /// <summary> Creates the flashing effect on the texture </summary>
	void Start ()
    {
        LinkComponents();
        goTarget = GameObject.FindGameObjectWithTag("Player");
        psTarget = goTarget.GetComponent<PlayerScript>();
        iEnemyState = (int)eEnemyStates.patrolling;
        SelectMaterialColour();
	}

    /// <summary> Creates the flashing effect on the texture </summary>
	void Update ()
    {        
        UpdateMaterialColour();

		if (iEnemyState == (int)eEnemyStates.chasing && fMoveSpeed != 8.0f)
			fMoveSpeed = 8.0f;

        CalculateTargetDistance();

        if(bUpdateTimer) DecisionTimer();
        if (fDecisionTimer <= 0)
        {
            CalculateMoveDirection();
            fDecisionTimer = Random.Range(0.1f, 0.6f);
            bUpdateTimer = true;
        }

        if (iEnemyState == (int)eEnemyStates.chasing || iEnemyState == (int)eEnemyStates.blocking)
        {
            fDistanceWhenToJump = Random.Range(2.0f, 2.5f);
            AdjustVelocity();
            if (iEnemyBehaviour != (int)eEnemyBehaviours.jumping) 
            {
                if (CheckConditionsToJump()) Jump();
            }

            RotateToLookAtPlayer();
        }

		if (iEnemyState == (int)eEnemyStates.chasing)
			bInFrontOfPlayer = false;

		if (!bInFrontOfPlayer) 
		{
			if (fDeathTimer > 0)
				fDeathTimer -= Time.deltaTime;
			else
				lgs.FindAllEnemies (this.gameObject);
		}
	}

    void LinkComponents()
    {
        r = this.gameObject.transform.GetComponent<Rigidbody>();
		lgs = GameObject.Find("LevelGenerator").GetComponent<LevelGeneratorScript>();
    }

    void RotateToLookAtPlayer()
    {
        Vector3 v3LookAt = new Vector3(goTarget.transform.position.x,
            this.gameObject.transform.position.y,
            goTarget.transform.position.z);
        this.transform.LookAt(v3LookAt, Vector3.up);
    }

    void DecisionTimer()
    {
        if (fDecisionTimer > 0) fDecisionTimer -= Time.deltaTime;
        else bUpdateTimer = false;
    }

    bool CheckConditionsToJump()
    {
        bool b = false;
        
        //the enemies current position
        Vector3 v3 = this.gameObject.transform.position;
        //Directional vector for distance calculations
		GameObject goGotPlayer = GameObject.Find ("Player(Clone)");
		Vector3 v3Distance = goGotPlayer.transform.position - v3;

        if (v3Distance.x > -fDistanceWhenToJump 
			&& v3Distance.x < fDistanceWhenToJump 
			&& goGotPlayer.GetComponent<PlayerScript> ().pms.bOnGround == false)
		{
			b = true;
			Debug.Log ("Jump: " + b.ToString());
		}

        return b;
    }

    public void Jump()
    {
        r.velocity = new Vector3(r.velocity.x, fJumpVelocity, r.velocity.z);
        iEnemyBehaviour = (int)eEnemyBehaviours.jumping;
    }

    void CalculateMoveDirection()
    {
        //the enemies current position
        Vector3 v3 = this.gameObject.transform.position;
        //Directional vector for distance calculations
        Vector3 v3Distance = goTarget.transform.position - v3;

        if (iEnemyState == (int)eEnemyStates.chasing)
        {
            if (psTarget.pms.iDirection == (int)TurnPointScript.eDirection.north)
            {
                if (v3Distance.x < 0) v3MoveDirection = Vector3.left;
                else v3MoveDirection = Vector3.right;
            }
            else if(psTarget.pms.iDirection == (int)TurnPointScript.eDirection.south)
            {
                if (v3Distance.x > 0) v3MoveDirection = Vector3.right;
                else v3MoveDirection = Vector3.left;
            }

            else if(psTarget.pms.iDirection == (int)TurnPointScript.eDirection.east)
            {
                if (v3Distance.z < 0) v3MoveDirection = Vector3.back;
                else v3MoveDirection = Vector3.forward;
            }
            else if(psTarget.pms.iDirection == (int)TurnPointScript.eDirection.west)
            {
                if (v3Distance.z > 0) v3MoveDirection = Vector3.forward;
                else v3MoveDirection = Vector3.back;
            }
        }
        if (iEnemyState == (int)eEnemyStates.blocking)
        {
            if (psTarget.pms.iDirection == (int)TurnPointScript.eDirection.north)
            {
                if (v3Distance.z < 0) v3MoveDirection = Vector3.left;
                else v3MoveDirection = Vector3.right;
            }

            else if (psTarget.pms.iDirection == (int)TurnPointScript.eDirection.south)
            {
                if (v3Distance.z > 0) v3MoveDirection = Vector3.right;
                else v3MoveDirection = Vector3.left;
            }

            else if (psTarget.pms.iDirection == (int)TurnPointScript.eDirection.east)
            {
                if (v3Distance.x < 0) v3MoveDirection = Vector3.back;
                else v3MoveDirection = Vector3.forward;
            }

            else if (psTarget.pms.iDirection == (int)TurnPointScript.eDirection.west)
            {
                if (v3Distance.x > 0) v3MoveDirection = Vector3.forward;
                else v3MoveDirection = Vector3.back;
            }
        }

        //Destroy "this" enemy if out of range of player or scene
        if (v3Distance.x > fMaxDistanceFromTargetX ||
            v3Distance.x < -fMaxDistanceFromTargetX ||
            v3Distance.y > fMaxDistanceFromTargetY ||
            v3Distance.y < -fMaxDistanceFromTargetY ||
            v3Distance.z > fMaxDistanceFromTargetZ ||
            v3Distance.z < -fMaxDistanceFromTargetZ ||
            v3.y < -100.0f)
        {
            lgs.FindAllEnemies(this.gameObject);
        }
    }

    /// <summary> </summary>
    void AdjustVelocity()
    {
        float f;

        if (psTarget.pms.iDirection == (int)TurnPointScript.eDirection.north
            || psTarget.pms.iDirection == (int)TurnPointScript.eDirection.south)
            f = v3MoveDirection.x * fMoveSpeed;
        else f = v3MoveDirection.z * fMoveSpeed;

        //Set velocity X - axis chasing
        if (iEnemyState == (int)eEnemyStates.blocking)
        {
            if (psTarget.pms.iDirection == (int)TurnPointScript.eDirection.north) r.velocity = new Vector3(r.velocity.x, r.velocity.y, f);
            if (psTarget.pms.iDirection == (int)TurnPointScript.eDirection.south) r.velocity = new Vector3(r.velocity.x, r.velocity.y, f);
            if (psTarget.pms.iDirection == (int)TurnPointScript.eDirection.east) r.velocity = new Vector3(f, r.velocity.y, r.velocity.z);
            if (psTarget.pms.iDirection == (int)TurnPointScript.eDirection.west) r.velocity = new Vector3(f, r.velocity.y, r.velocity.z);
        }

        //Set velocity z - axis blocking
        if (iEnemyState == (int)eEnemyStates.chasing)
        {
            if (psTarget.pms.iDirection == (int)TurnPointScript.eDirection.east) r.velocity = new Vector3(r.velocity.x, r.velocity.y, f);
            if (psTarget.pms.iDirection == (int)TurnPointScript.eDirection.west) r.velocity = new Vector3(r.velocity.x, r.velocity.y, f);
            if (psTarget.pms.iDirection == (int)TurnPointScript.eDirection.north) r.velocity = new Vector3(f, r.velocity.y, r.velocity.z);
            if (psTarget.pms.iDirection == (int)TurnPointScript.eDirection.south) r.velocity = new Vector3(f, r.velocity.y, r.velocity.z);
        }
    }

    /// <summary> Creates the flashing effect on the texture </summary>
    void UpdateMaterialColour()
    {
        float lerp = Mathf.PingPong(Time.time, fDuration) / fDuration;
        GetComponent<Renderer>().material.color = Color.Lerp(colorStart, colorEnd, lerp);
    }

    void SelectMaterialColour()
    {
        switch (iEnemyState)
        {
            case (int)eEnemyStates.patrolling:
                colorStart = Color.white;
                colorEnd = Color.black;
                fDuration = 1.0f;
                break;

            case (int)eEnemyStates.blocking:
                colorStart = Color.red;
                colorEnd = Color.black;
                fDuration = 0.2f;
                break;

            case (int)eEnemyStates.chasing:
                colorStart = Color.green;
                colorEnd = Color.black;
                fDuration = 0.5f;
                break;

            default:
                break;
        }
    }

    void CalculateTargetDistance()
    {
        //the enemies current position
        Vector3 v3 = this.gameObject.transform.position;
        //Directional vector for distance calculations
        Vector3 v3Distance = goTarget.transform.position - v3;

        //Check if close enough along the x axis
        bool bX;
        if (v3Distance.x < fDistanceToBeginChaseX && v3Distance.x > -fDistanceToBeginChaseX) bX = true;
        else bX = false;

        //Check if close enough along the y axis
        bool bY;
        if (v3Distance.y < fDistanceToBeginChaseY && v3Distance.y > -fDistanceToBeginChaseY) bY = true;
        else bY = false;

        //Check if close enough along the z axis
        bool bZ;
        if (v3Distance.z < fDistanceToBeginChaseZ && v3Distance.z > -fDistanceToBeginChaseZ) bZ = true;
        else bZ = false;

        if ((bX || bZ ) && bY)
        {
            if (psTarget.pms.iDirection == (int)TurnPointScript.eDirection.north)
            {
                if (v3Distance.x > 0)
                {
                    iEnemyState = (int)eEnemyStates.chasing;
                }
                else iEnemyState = (int)eEnemyStates.blocking; 
            }

            else if (psTarget.pms.iDirection == (int)TurnPointScript.eDirection.south)
            {
                if (v3Distance.x < 0)
                {
                    iEnemyState = (int)eEnemyStates.chasing;
                }
                else iEnemyState = (int)eEnemyStates.blocking;
            }

            else if (psTarget.pms.iDirection == (int)TurnPointScript.eDirection.east)
            {
                if (v3Distance.z < 0)
                {
                    iEnemyState = (int)eEnemyStates.chasing;
                }
                else iEnemyState = (int)eEnemyStates.blocking;
            }

            else if (psTarget.pms.iDirection == (int)TurnPointScript.eDirection.west)
            {
                if (v3Distance.z > 0)
                {
                    iEnemyState = (int)eEnemyStates.chasing;
                }
                else iEnemyState = (int)eEnemyStates.blocking;
            }
            SelectMaterialColour();
        }

        if (!bX || !bZ)
        {
            iEnemyState = (int)eEnemyStates.patrolling;
            SelectMaterialColour();
        }
    }

    void OnCollisionEnter(Collision c)
    {
        if(c.gameObject.tag == "OffRoad")
        {
            lgs.FindAllEnemies(this.gameObject);
        }

		if (c.gameObject.tag == "Ground" && iEnemyBehaviour == (int)eEnemyBehaviours.jumping)
			iEnemyBehaviour = (int)eEnemyBehaviours.walking;
		
		if (c.gameObject.tag == "Enemy"
		    && c.gameObject.GetComponent<EnemyScript>().iEnemyBehaviour == (int)eEnemyBehaviours.jumping
		    || iEnemyBehaviour == (int)eEnemyBehaviours.jumping)
		{
			Transform tEnemy = c.transform;
			if (tEnemy.position.y > this.transform.position.y) 
			{
				EnemyScript es = c.gameObject.GetComponent<EnemyScript>();
				es.Jump();
			}
			else Jump();
		}
    }
}
