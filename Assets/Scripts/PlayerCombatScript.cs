///<author> Graham Alexander MacDonald 07/02/2015 </author>

using UnityEngine;
using System.Collections;

public class PlayerCombatScript : MonoBehaviour
{
    public int iMaxHealth = 10;
    public int iCurrentHealth = 3;
    public int iScore;
	public int iCharges;

    public bool bDodging;
    public float fDodgeVelocity = 15.0f;

    public PlayerScript ps;

    public static float fOBSTACLE_TIMER = 0.5f;
    public float fInvincibleTimer;
    public bool bInvincible;

    public static float fCHARGE_TIMER = 1.5f;
    public float fChargeTimer;
    public bool bCharging;
    private bool dead;

	public Camera playerCamera;
	public bool bResetCamera;
	public Color rgbaDefaultSky;

    /// <summary> Use this for initialization </summary>
	void Start ()
    {
		LinkComponents ();
		iCharges = ps.lgs.gds.iCharges;
		iCurrentHealth = ps.lgs.gds.iHealth;
		playerCamera = ps.gameObject.transform.Find ("PlayerCamera").GetComponent<Camera> ();
		rgbaDefaultSky = playerCamera.backgroundColor;
	}

    /// <summary> Update is called once per frame </summary>
	void Update ()
    {
        if (bInvincible) Timer();
        if (bCharging) ChargeTimer();
		if (iCurrentHealth <= 0) {
			if (Application.loadedLevelName == "GameScene")
				GameOver ();
			else
				Application.LoadLevel ("TutorialScene");
		} 
		else
		{
			if (iCurrentHealth == 1) 
			{
				if(!bResetCamera) bResetCamera = true;
				UpdateCameraColour();
			}
		}

		if (bResetCamera && iCurrentHealth > 1)
			ResetCameraColour ();
	}

    void LinkComponents()
    {
        ps = this.gameObject.GetComponent<PlayerScript>();
    }

	void UpdateCameraColour()
	{
		float lerp = Mathf.PingPong(Time.time, 3.0f) / 3.0f;
		playerCamera.backgroundColor = Color.Lerp(Color.white, Color.red, lerp);
	}

	void DamageThroughCameraColour()
	{
		Handheld.Vibrate();
		playerCamera.backgroundColor = Color.white;
		Debug.Log ("Camera White - Damaged");
		bResetCamera = true;
	}

	void ResetCameraColour()
	{
		if (playerCamera.backgroundColor != rgbaDefaultSky)
		{
			float lerp = Mathf.PingPong (Time.time, 1.0f) / 1.0f;
			playerCamera.backgroundColor = Color.Lerp (Color.white, rgbaDefaultSky, lerp);
			Debug.Log ("Camera Reset");
			if(lerp > 0.9) playerCamera.backgroundColor = rgbaDefaultSky;
		}
		else bResetCamera = false;
	}

    void Timer()
    {
        if (fInvincibleTimer > 0) fInvincibleTimer -= Time.deltaTime;
        else
        {
            bInvincible = false;
            ps.transform.GetComponent<Collider>().enabled = true;
            ps.transform.GetComponent<Rigidbody>().useGravity = true;            
        }
    }

	public void GameOver()
	{
        if (dead == false)
        {
            dead = true;
		    //if(!this.gameObject.rigidbody.IsSleeping())this.gameObject.rigidbody.Sleep();
    	    Destroy (this.gameObject.transform.Find ("Lady-Yang").gameObject);
    	    ps.lgs.DeleteEnemies();
		    ps.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            ps.pms.bGoForward = false;      
		    ps.lgs.gds.bPromptForName = true;
        }
	}

    public void ChargeTimer()
    {
        if (fChargeTimer > 0) fChargeTimer -= Time.deltaTime;
        else
        {
            bCharging = false;
            ps.pms.iState = (int)PlayerMovementScript.eMovementStates.walking;
            ps.pms.StateUpdate(); //Update state and speed of run in Player movement script
        }
    }

    public void StartCharge(float fSwipeVelocity)
    {
        if (iCharges > 0)
		{
			this.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero; //To stop jumping/'going side-to-side' when charge begins
			ps.pms.iState = (int)PlayerMovementScript.eMovementStates.running;
			ps.pms.StateUpdate (); //Update state and speed of run in Player movement script
			fChargeTimer = fCHARGE_TIMER; //Reset Timer
			bCharging = true;
			iCharges--;

		}
    }

    public void Dodge()
    {
        if(ps.pms.iState == (int)PlayerMovementScript.eMovementStates.jumping
            || ps.pms.iState == (int)PlayerMovementScript.eMovementStates.falling)
        {
            bDodging = true;
            this.gameObject.GetComponent<Rigidbody>().velocity += Vector3.down * fDodgeVelocity;
        }
    }

    void OnCollisionEnter(Collision c)
    {
		if (c.gameObject.tag == "Enemy")
        {
			Vector3 v3 = this.gameObject.transform.position; //Player pos
            Vector3 v3c = c.gameObject.transform.position; //Enemy pos

            if (!bDodging)
            {
                //if above the enemy on collision kill it 'Landing on'
                if (v3.y > v3c.y)
                {
                    iScore += 10;
					iCharges += 1;

                    //Get enemy list "To be Destroyed GameObject"
                    c.gameObject.GetComponent<EnemyScript>().lgs.FindAllEnemies(c.gameObject);

                    //Make player jump
                    if (ps.pms.bOnGround) ps.pms.Jump();
                    else
                    {
                        //Forced jump
                        ps.pms.bOnGround = true;
                        ps.pms.Jump();
                        ps.pms.bOnGround = false;
                    }
                }
                else if(iCurrentHealth > 0) 
				{
					iCurrentHealth--; //else take damage
					DamageThroughCameraColour ();
				}
            }
            else 
            {
                iScore += 10;
				iCharges += 1;
                c.gameObject.GetComponent<EnemyScript>().lgs.FindAllEnemies(c.gameObject);
                Dodge();
            }
        }

        if (c.gameObject.tag == "Obstacle")
        {
			//stop obstacle from moving
            c.rigidbody.velocity = new Vector3(0, 0, 0);
            ps.pms.iState = (int)PlayerMovementScript.eMovementStates.walking;
           	ps.pms.StateUpdate();

            if (ps.pms.bOnGround)
            {
                //Stop the player from flying away
                GetComponent<Rigidbody>().velocity = new Vector3(GetComponent<Rigidbody>().velocity.x, 0, GetComponent<Rigidbody>().velocity.z);

                if(iCurrentHealth > 0) 
				{
					iCurrentHealth--;
					DamageThroughCameraColour ();
				}

				/* Can be changed to make the player invisible
				 * instead of using objects collider use players and set time */
                //bInvincible = true; //used to countdown timer
				c.collider.enabled = false; //stop player can no longer collide
				c.rigidbody.useGravity = false; //so the player goes straight
				c.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
				c.rigidbody.velocity = new Vector3(0, 0, 0);
                //fInvincibleTimer = fOBSTACLE_TIMER; //set timer 
            }
            else 
			{
				ps.pms.Jump();
				iCharges++;
			}
        }
    }
}
