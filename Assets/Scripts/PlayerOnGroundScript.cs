///<author> Graham Alexander MacDonald 05/02/2015 </author>

using UnityEngine;
using System.Collections;

public class PlayerOnGroundScript : MonoBehaviour
{
    private PlayerScript ps;
    public bool bOffRoad;
    
    /// <summary> Use this for initialization </summary>
	void Start ()
    {
        LinkComponents();
	}

    /// <summary> Links components </summary>
    void LinkComponents()
    {
        ps = this.gameObject.transform.parent.GetComponent<PlayerScript>();
    }

    /// <summary> Collision with tag "Ground" sets player bool for "On Ground" = true </summary>
    /// <param name="c"> Collider to condition </param>
    void OnTriggerStay(Collider c)
    {
		if (c.transform.name == "Ramp(Clone)" && ps.pcs.bInvincible) 
		{
			ps.pcs.bInvincible = false;
			ps.GetComponent<Collider>().enabled = true;
			ps.GetComponent<Rigidbody>().velocity += Vector3.up;
			ps.GetComponent<Rigidbody>().useGravity = true;
		}

		if (c.transform.name == "MovingLeftRight(Clone)") 
		{
			c.GetComponent<MovingPlatformScript>().bHasPlayer = true;
		}

		if(c.tag == "Ground")
        {
            ps.pms.bOnGround = true;
            if (ps.pms.iState == (int)PlayerMovementScript.eMovementStates.jumping
                || ps.pms.iState == (int)PlayerMovementScript.eMovementStates.falling)
            {
                ps.pms.iState = (int)PlayerMovementScript.eMovementStates.stationary;
            }
            if (ps.pcs.bDodging)
            {
                ps.pcs.bDodging = false;
                ps.pms.Jump();
            }
        }

        if(c.name == "SubEnd(Clone)"
            || c.name == "BigEnd(Clone)")
        {
			if (Application.loadedLevelName == "GameScene")
			{
				ps.lgs.gds.iTotalScore += ps.pcs.iScore;

				foreach(int i in ps.lgs.AiRewardScreenLevels)
				{
					if(ps.lgs.gds.iLoopCount == i) 
					{
						ps.pms.bGoForward = false;
						ps.pgs.bRewardScreen = true;
						Destroy (this.gameObject.transform.parent.Find ("Lady-Yang").gameObject);
					}
				}

				if(!ps.pgs.bRewardScreen) 
				{
					ps.lgs.gds.iLoopCount++;
					ps.lgs.gds.iCharges = ps.pcs.iCharges;
					ps.lgs.gds.iHealth = ps.pcs.iCurrentHealth;
					Application.LoadLevel("GameScene");
				}
			}
            else if (Application.loadedLevelName == "TutorialScene") Application.LoadLevel("GameScene");
        }

        if (c.tag == "OffRoad")
        {
            if (!bOffRoad) ps.pms.fRespawnTimer = ps.pms.fOFF_ROAD_TIMER;
            bOffRoad = true;
        }
    }

    /// <summary> Exiting Collision with tag "Ground" sets player bool for "On Ground" = false </summary>
    /// <param name="c"> Collider to condition </param>
    void OnTriggerExit(Collider c)
    {
        if (c.tag == "Ground")
        {
            ps.pms.bOnGround = false;
        }

		if (c.transform.name == "MovingLeftRight(Clone)") 
		{
			c.GetComponent<MovingPlatformScript>().bHasPlayer = false;
		}
    }
}
