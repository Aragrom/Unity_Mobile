///<author> Graham Alexander MacDonald 04/02/2015
/// Calum Brown - "Gyro" Input </author>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MobileInputScript : MonoBehaviour
{
    public bool bUsingGyroInput = true;
	public bool bUsingButtonInput = false;

    public string strGyroDirection;
	public float fLeanAmount; // The amount the device is leaning (used for increasing speed of character moving left to right)
    
    //List holds all touch 'positions' (reset on analog being let go)
    public List<Vector2> Lv2TouchScreenPositions;

    //Analog Properties
	public bool bHasAnalogStick = false;
    public int iAnalogId;
    public Vector2 v2AnalogAxis;
    public Vector2 v2AnalogStart;

    public float iAnalogDeadZone = 5.0f;

    //Lists for tracking swipe ...[0] = 'first' id, 'first' start position, 'first' timer
    public List<int> LiSwipeId; //used to track all swipes
    public List<Vector2> Lv2SwipeStart; //used to track start position of all swipes

	public int iSwipeDeadZone = 10;

    //Jump Modifier
    public bool bJumpModifier;
    public int iJumpId; //for tracking which touch is jumpModifier

    //GameObject to be effected by gyro input
    public GameObject goGyroTest; // = Place holder for gyro testing
    public float fSmooth = 10.0f;

    public string strDeviceOrientation; // string that holds current device orientation. (For use in GUI script (currently))
    Compass compass; //Hand held devices only http://docs.unity3d.com/ScriptReference/Compass.html

    private PlayerScript ps; 

    /// <summary> Use this for initialization </summary>  
    void Start()
    {
        LinkComponents();
        CheckDeviceOrientation();

		//ManageInputType ();
    }

    /// <summary> Update is called once per frame </summary>
    void Update()
    {
        if (!ps.lgs.gds.bPromptForName)
		{
			CheckForInput (); //from the player
			//CheckDeviceOrientation ();
        	
			if (goGyroTest != null)
				GyroRotate ();
        	
			//if a finger has been selected as "Analog Stick"
			if (bHasAnalogStick)
				CalculateAnalogDirection ();
        	
			//Work-around for infinite jump. Forces to be always "On the ground"
			//if (bJumpModifier)
				//JumpModifier ();
        	
			if (bUsingGyroInput)
				GetGyroDirection ();
		}
    }

	private void ManageInputType()
	{

		if (ps.lgs.gds.strInputChoice == "analog")
		{
			bUsingGyroInput = false;
			bUsingButtonInput = false;
		}
		if (ps.lgs.gds.strInputChoice == "gyro"
		    || ps.lgs.gds.strInputChoice == "") 
		{
			bUsingGyroInput = true;
			bUsingButtonInput = false;
		}
		if (ps.lgs.gds.strInputChoice == "button") 
		{
			bUsingButtonInput = true;
			bUsingGyroInput = false;
		}

		if (ps.lgs.gds.strInputChoice == null)
		{
			bUsingGyroInput = true;
			bUsingButtonInput = false;
		}
	}

    /// <summary> Links components </summary>
    void LinkComponents()
    {
        ps = this.gameObject.GetComponent<PlayerScript>();
        //goGyroTest = GameObject.Find("Player").transform.FindChild("Gyro").gameObject;
        goGyroTest = GameObject.Find("Gyro");
    }

	void GetGyroDirection()
	{
		if (Input.acceleration.x < -0.05f)
		{
			strGyroDirection = "Left";
		}
		else if (Input.acceleration.x > 0.05f)
		{
			strGyroDirection = "Right";
		}
		else
		{
			strGyroDirection = null;
		}
		fLeanAmount = Input.acceleration.x;
	}

    /// <summary> Checks for input from the user </summary>
    void CheckForInput()
    {
        //If touch count is greater than zero
        int i = 0;
        while (i < Input.touchCount)
        {
            Touch touch = Input.GetTouch(i);

            if (touch.phase == TouchPhase.Began)
            {
                //Store "touch" position at the end of the list
                Lv2TouchScreenPositions.Add(touch.position);

                if (ps.lgs.gds.strInputChoice == "analog")
				{
                	//if analog stick DOES NOT exists
                	if (!bHasAnalogStick && touch.position.x < Screen.width / 2)
                	{
                	    SetAnalogTouch(touch);
                	}
                	else if(ps.pms.bOnGround && !ps.pcs.bInvincible) ps.pms.Jump(); //JUMP
                }
				else if(ps.pms.bOnGround && !ps.pcs.bInvincible) ps.pms.Jump();
            }

            //Store position on "first" movement
            if (touch.phase == TouchPhase.Moved)
            {
				if(ps.lgs.gds.strInputChoice == "analog" && touch.fingerId != iAnalogId) AddToStartSwipeLists(touch);
				else AddToStartSwipeLists(touch);
            }

            if (touch.phase == TouchPhase.Ended)
            {
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // here we need to build what swipes do under each condition
                /////////////////////////////////////////////////////////////////////////////////////////////////////////////
				Vector2 v2Dir = new Vector2();
				if(ps.lgs.gds.strInputChoice == "analog" && touch.fingerId != iAnalogId) v2Dir = CalculateSwipeDirection(touch);
				else v2Dir = CalculateSwipeDirection(touch);
				//Uses id to track start position then use this "touch"

                //Get absolute values for x and y
                float fX = Mathf.Abs(v2Dir.x);
                float fY = Mathf.Abs(v2Dir.y);

                //Attack condition "Slam"
                //if Axis is "more" 'Y' than 'X' 
                //and 'Y-axis direction is equal to down' 
                //and 'the absolute value you of Y is greater than Swipe Dead Zone'
                
                if (fY > fX
				    && fY > iSwipeDeadZone)
				{
					if(v2Dir.y > 0) ps.pcs.StartCharge(fY);
				    //else if(v2Dir.y < 0) ps.pcs.Dodge();
				}
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////
            }
            ++i;
        }

        //if NOT touching the screen
        if (Input.touchCount == 0)
        {
            // clear touch position list
            Lv2TouchScreenPositions.Clear();
        }

        // !!! - Return player to start - !!!
        if(Input.touchCount > 7)
        {
            this.gameObject.transform.position = new Vector3(0, 10, 0);
        }
    }

    /// <summary> Used to get the gyro input and manipulate an object </summary>
    void GyroRotate() // by Graham
    {
        Gyroscope gyro = Input.gyro;
        gyro.enabled = true;

        //if enabled
        if (Input.gyro.enabled)
        {
            /*ps.pgs.strDebugMessage = "X: " + gyro.attitude.x.ToString()
                + " Y: " + gyro.attitude.y.ToString()
                + " Z: " + (-gyro.attitude.z).ToString()
                + " W: " + (-gyro.attitude.w).ToString();*/

            //Rotation to be added
            Quaternion quart = new Quaternion
                (
                gyro.attitude.x,
                gyro.attitude.y,
                gyro.attitude.z,
                gyro.attitude.w
                );

            //Updating rotation
            goGyroTest.transform.rotation = Quaternion.Slerp
                (
                goGyroTest.transform.rotation,
                quart,
                Time.deltaTime * fSmooth
                );
        }
        //else ps.pgs.strDebugMessage = "No gyroscope detected";
    }

    /// <summary> Device orientations http://docs.unity3d.com/ScriptReference/DeviceOrientation.html </summary>
    void CheckDeviceOrientation()
    {
        #region Device Orientation
        switch (Input.deviceOrientation)
        {
            case DeviceOrientation.FaceDown:
                strDeviceOrientation = "FaceDown";
                break;

            case DeviceOrientation.FaceUp:
                strDeviceOrientation = "FaceUP";
                break;

            case DeviceOrientation.LandscapeLeft:
                strDeviceOrientation = "LandscapeLeft";
                break;

            case DeviceOrientation.LandscapeRight:
                strDeviceOrientation = "LandscapeRight";
                break;

            case DeviceOrientation.Portrait:
                strDeviceOrientation = "Portrait";
                break;

            case DeviceOrientation.PortraitUpsideDown:
                strDeviceOrientation = "PortraitUpsideDown";
                break;

            case DeviceOrientation.Unknown:
                strDeviceOrientation = "Unknown";
                break;

            default:
                strDeviceOrientation = "Unknown";
                break;
        }
        #endregion
    }

    /// <summary> Set the touch to be tracked as "Jump Modifier" 
    /// stores id </summary>
    /// <param name="t"> stores "touch id" </param>
    void SetJumpModifier(Touch t)
    {
        iJumpId = t.fingerId;
        bJumpModifier = true;
        ps.pms.bOnGround = true;
    }

    /// <summary> Set touch as Analog stick touch, store id and position </summary>
    /// <param name="t"> Touch to be set </param>
    void SetAnalogTouch(Touch t)
    {
        v2AnalogStart = t.position;
        iAnalogId = t.fingerId;
        bHasAnalogStick = true;
    }

    /// <summary> Managing the adding of a swipe </summary>
    /// <param name="t"> touch to be added </param>
    void AddToStartSwipeLists(Touch t)
    {
        //Check to see if touch is already being tracked
        bool b = false;
        foreach (int _i in LiSwipeId)
        {
            //Check to see if id is in list (if already stored)
            if (_i == t.fingerId) b = true;
        }
        if (!b) //if not stored
        {
            //Start position of swipe and id
            LiSwipeId.Add(t.fingerId);
            Lv2SwipeStart.Add(t.position);
        }
    }

    /// <summary> Calculates "directional" displacement vector for a swipe </summary>
    /// <param name="t"> touch to be managed </param>
    /// <returns> Displacement Vector </returns>
    Vector2 CalculateSwipeDirection(Touch t)
    {
        Vector2 v2 = new Vector2();
        int iElement = 0;

        //Find Start vector using id for the current ending swipe touch id
        foreach (int i__ in LiSwipeId)
        {
            //Checking id's
            if (i__ == t.fingerId)
            {
                //Found correct id use the position vector
                v2 = Lv2SwipeStart[iElement];
            }
            else iElement++;
        }

        //Clear all swipes being tracked
        //Lv2SwipeStart.Clear();
        //LiSwipeId.Clear();
        //LfSwipeTimer.Clear();

        Lv2SwipeStart.Remove(Lv2SwipeStart[iElement]);
        LiSwipeId.Remove(LiSwipeId[iElement]);

        v2 = t.position - v2;

        return v2;
    }

    /// <summary> Calculates the normalized direction of 
    /// the touch for the analog stick </summary>
    void CalculateAnalogDirection()
    {
        Touch tAnalogStick = GetTouch(iAnalogId);

        if (tAnalogStick.phase == TouchPhase.Moved)
        {
            Vector2 v2Dir;
            v2Dir = (tAnalogStick.position - v2AnalogStart);
            if (Mathf.Abs(v2Dir.x) > iAnalogDeadZone)
            {
                v2Dir.Normalize();
                v2AnalogAxis = v2Dir;
            }
            else v2AnalogAxis = new Vector2();
        }

        if (tAnalogStick.phase == TouchPhase.Ended)
        {
            bHasAnalogStick = false;
        }
    }

    /// <summary> Could be used for unlimited jumps/run modifier
    /// currently set up for a "flappy bird jump" </summary>
    void JumpModifier()
    {
        Touch tJumpModifier = GetTouch(iJumpId);

        if (tJumpModifier.phase == TouchPhase.Ended)
        {
            bJumpModifier = false;
            ps.pms.bOnGround = false;
        }
    }

    /// <summary> Used to get touch based on ID </summary>
    /// <param name="iId"> touch ID </param>
    /// <returns> the "touch" </returns>
    Touch GetTouch(int iId)
    {
		Touch t = new Touch();
        foreach (Touch t1 in Input.touches)
        {
            if (t1.fingerId == iId)
            {
                return t = t1;
            }
        }
		return t;
    }
}

