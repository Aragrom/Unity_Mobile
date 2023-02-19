///<author> Graham Alexander MacDonald 04/02/2015 </author>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerGUIScript : MonoBehaviour
{
	public bool bLoading = true;

	public Font fontWanted;

    //Image holding a single white pixel
    public Texture2D t2dWhitePixel;
    public Texture2D t2dRedPixel;
    public Texture2D t2dGreenPixel;

    //Textures for analog stick
    public Texture2D t2dMovingLeft;
    public Texture2D t2dMovingRight;
	public Texture2D t2dMovingStraight;
    public Texture2D t2dAnalogStick;

	public Sprite spriteMessageImage;
	public Sprite spriteTilt;
	public Sprite spriteSwipe;
	public Sprite spriteTouch;

	public Texture2D t2dLoadScreen;

    //Analog stick texture size variables
    public float fAnalogTextureWidth = 100.0f;
    public float fAnalogTextureHeight = 100.0f;
    public float fTextureStepSize = 10.0f; //for how far to draw icon from origin

    //GUI display variables (Entire debug lists go from these starting values)
    public float fStartPosX = 10.0f;
    public float fStartPosY = 10.0f;
    public float fRectWidth = 200.0f;
    public float fRectHeight = 100.0f;

    //Used in calculating health bar
    public int iGuiBarLength = 100;
    public int iGuiBarHeight = 20;

	public int iVolumeButtonWidth;
	public int iVolumeButtonHeight;

    //Disables 'some' gui features
    public bool bDebugMode = true;
    //holds the most recent "game - error or message" to be displayed on the gui
    public string strDebugMessage;

    private GUIStyle guiStyle;

    //private LineRenderer lr;
    private PlayerScript ps;
    private LevelGeneratorScript lgs;

	public List<string> LstrAlphabet;
	public int iKeyWidth;
	public int iKeyHeight;

	public string strKeyPress;

	public int iMoveButtonWidth = 100;
	public int iMoveButtonHeight = 100;

	public bool bHasMessageToDisplay = true; //starts true as load bar uses this timer
	public string strMessageToPlayer;
	public GameObject goDisplayOn;
	public bool bDisplayOnLeft = false;
	public bool bDisplayOnRight = false;
	public float fMessageTimer = 2.0f;

	public GameObject goTiltAnim;
	public GameObject goTouchAnim;
	private GameObject goSpriteSpawner;

	public bool bChangedFontSize = false;

	public bool bRewardScreen = false;
	public int iRewardButtonWidth;
	public int iRewardButtonHeight;

	public Texture2D t2dHealth;
	public Texture2D t2dCharge;

	public Texture2D t2dSwipe;
	public Texture2D t2dSwipe0;
	public Texture2D t2dSwipe1;

	public float fSwipeAnimTimer;

    /// <summary> Use this for initialization </summary>
    void Start()
    {
    	goSpriteSpawner = GameObject.Find("SpriteSpawner");
		ConfigureAlphaList ();
		LoadResources();
        LinkComponents();
        bDebugMode = false;

		Screen.sleepTimeout = SleepTimeout.NeverSleep; //so screen doesn't black-out when no input found
		Screen.orientation = ScreenOrientation.LandscapeLeft; //force landscape always

		iVolumeButtonWidth = Screen.width / 15;
		iVolumeButtonHeight = Screen.height / 10;

		Invoke ("TriggerMovementMessage", 2.0f);

		iRewardButtonWidth = Screen.width / 6;
		iRewardButtonHeight = Screen.height /15;

		strMessageToPlayer = GetRandomLoadMessage ();
    }

    /// <summary> Update is called once per frame </summary>
    void Update()
    {
        if (bHasMessageToDisplay)
		{
			if(fMessageTimer >= 0) fMessageTimer -= Time.deltaTime;
        	else
        	{
        		fMessageTimer = 10.0f;
				bHasMessageToDisplay = false;
				ps.pms.bGoForward = true;
        	}
        }

		if (ps.mis.bHasAnalogStick) UpdateAnalogTexture();
    }

	public string GetRandomLoadMessage()
	{
		string[] AstrMessages = new string[20];
		AstrMessages[0] = "Every 5 levels you can spend that score!";
		AstrMessages[1] = "Bounce on obstacles to gain a charge!";
		AstrMessages[2] = "Bounce on enemies to gain a charge and score!";
		AstrMessages[3] = "Every Level Lady Yang runs faster and faster!";
		AstrMessages[4] = "Cant complete - Get Stuck - It will regenerate!";
		AstrMessages[5] = "Beat the top score to store your name";
		AstrMessages[6] = "Try charging on to enemies and obstacles";
		AstrMessages[7] = "Fall of the track - Theres check points!";
		AstrMessages[8] = "If you get stuck it wont affect your level";
		AstrMessages[9] = "Volume button is located at the upper right corner";
		AstrMessages[10] = "Swipe up - charge";
		AstrMessages[11] = "Swipe up - charge";
		AstrMessages[12] = "Swipe up - charge";
		AstrMessages[13] = "Swipe up - charge";
		AstrMessages[14] = "Swipe up - charge";
		AstrMessages[15] = "Swipe up - charge";
		AstrMessages[16] = "Swipe up - charge";
		AstrMessages[17] = "Swipe up - charge";
		AstrMessages[18] = "Swipe up - charge";
		AstrMessages[19] = "Swipe up - charge";
		int i = (int)Random.Range (1.0f, (float)AstrMessages.Length) - 1;
		return AstrMessages [i];
	}

	public void CreateTutAnimation(string animType)
	{
		GameObject goTutAnim;
		float fYIncrement;
		switch (animType)
		{
			case "Jump":
				goTutAnim = goTouchAnim;
				fYIncrement = 0;
				break;

			case "Move":
				goTutAnim = goTiltAnim;
				fYIncrement = 90;
				break;

			default :
				goTutAnim = goTiltAnim;
				fYIncrement = 90;
				break;
		}
		//instantiates animated sprite gameobject at sprite spawners position
		Debug.Log(animType);
		GameObject go = Instantiate(goTutAnim,goSpriteSpawner.transform.position, Quaternion.Euler (0,transform.rotation.y+fYIncrement,transform.rotation.z)) as GameObject;
		go.transform.parent = this.transform;
	}
	void TriggerMovementMessage()
	{
		bLoading = false;
		ps.pms.bGoForward = true;
		if(Application.loadedLevelName == "TutorialScene") 
		{
			bHasMessageToDisplay = true;
			if(lgs.gds.strInputChoice == "gyro")
			{
				strMessageToPlayer = "'TILT' the phone left and right to MOVE Lady Yang";
				bDisplayOnLeft = false;
				bDisplayOnRight = false;
			}
			else 
			{
				strMessageToPlayer = "PRESS down on the LEFT side of the screen to create an 'Analog Stick' \n\n EACH press creates a NEW analog stick and holds Lady Yang STEADY";
				bDisplayOnLeft = true;
				bDisplayOnRight = false;
			}
			CreateTutAnimation("Move");
			fMessageTimer = 10.0f;

			GameObject.Find("TutJump(Clone)").transform.Find("TutorialCollider").GetComponent<TutorialObjectScript>().ps = this.ps;
		}
	}

	void ConfigureAlphaList()
	{
		#region Alphabet List
		LstrAlphabet.Add ("A");
		LstrAlphabet.Add ("B");
		LstrAlphabet.Add ("C");
		LstrAlphabet.Add ("D");
		LstrAlphabet.Add ("E");
		LstrAlphabet.Add ("F");
		LstrAlphabet.Add ("G");
		LstrAlphabet.Add ("H");
		LstrAlphabet.Add ("I");
		LstrAlphabet.Add ("J");
		LstrAlphabet.Add ("K");
		LstrAlphabet.Add ("L");
		LstrAlphabet.Add ("M");
		LstrAlphabet.Add ("N");
		LstrAlphabet.Add ("O");
		LstrAlphabet.Add ("P");
		LstrAlphabet.Add ("Q");
		LstrAlphabet.Add ("R");
		LstrAlphabet.Add ("S");
		LstrAlphabet.Add ("T");
		LstrAlphabet.Add ("U");
		LstrAlphabet.Add ("V");
		LstrAlphabet.Add ("W");
		LstrAlphabet.Add ("X");
		LstrAlphabet.Add ("Y");
		LstrAlphabet.Add ("Z");
		#endregion
	}

    /// <summary> For Updating analog texture to be displayed </summary>
    void UpdateAnalogTexture()
    {
        if (ps.mis.v2AnalogAxis.x > 0)
			t2dAnalogStick = t2dMovingRight;
		if (ps.mis.v2AnalogAxis.x < 0)
			t2dAnalogStick = t2dMovingLeft;

		if (ps.mis.v2AnalogAxis.x == 0)
			t2dAnalogStick = t2dMovingStraight;
    }

    /// <summary> Loads reources </summary>
    void LoadResources()
    {
    	//load animated sprites
    	goTiltAnim = (GameObject)Resources.Load("GUI/TiltAnim");
		goTouchAnim = (GameObject)Resources.Load("GUI/TouchAnim");

        t2dMovingLeft = (Texture2D)Resources.Load("GUI/leftArrow");
        t2dMovingRight = (Texture2D)Resources.Load("GUI/rightArrow");
		t2dMovingStraight = (Texture2D)Resources.Load("GUI/straightArrow");

        t2dWhitePixel = (Texture2D)Resources.Load("GUI/whitePixel");
        t2dRedPixel = (Texture2D)Resources.Load("GUI/redPixel");
        t2dGreenPixel = (Texture2D)Resources.Load("GUI/greenPixel");

		t2dLoadScreen = (Texture2D)Resources.Load ("GUI/titleScreen");

		t2dHealth = (Texture2D)Resources.Load ("GUI/Health");
		t2dCharge = (Texture2D)Resources.Load ("GUI/Charge");

		t2dSwipe0 = (Texture2D)Resources.Load ("GUI/Swipe_0");
		t2dSwipe1 = (Texture2D)Resources.Load ("GUI/Swipe_1");

		fontWanted = (Font)Resources.Load ("Fonts/Wanted M54");
    }

    /// <summary> Links components </summary>
    void LinkComponents()
    {
        ps = this.gameObject.GetComponent<PlayerScript>();
		lgs = GameObject.Find("LevelGenerator").GetComponent<LevelGeneratorScript>();
    }

    /// <summary>  Player User interface </summary>
    void OnGUI()
    {
		var centeredStyle = GUI.skin.GetStyle ("Label");
		centeredStyle.alignment = TextAnchor.UpperCenter;
		centeredStyle.normal.textColor = Color.white;

		if (!bChangedFontSize)
		{
			GUI.skin.label.fontSize = 20;
			GUI.skin.button.fontSize = 20;
			bChangedFontSize = true;
		}

		if (!bLoading)
		{
			Rect rect = new Rect (fStartPosX, fStartPosY, fRectWidth, fRectHeight);
			var middleLeftStyle = GUI.skin.GetStyle ("Label");
			middleLeftStyle.alignment = TextAnchor.MiddleLeft;
        	
			//Display
			Rect newRect1 = new Rect (fStartPosX, fStartPosY, fRectWidth, fRectHeight);
			if (Application.loadedLevelName == "GameScene")
				GUI.Label (newRect1, "Level - " + lgs.gds.iLoopCount.ToString ());
			else
				GUI.Label (newRect1, "Level - Tutorial");
			newRect1.y += 20;
			if(Application.loadedLevelName == "GameScene") GUI.Label (newRect1, "Total Score - " + lgs.gds.iTotalScore);
			newRect1.y += 20;
			if(Application.loadedLevelName == "GameScene") GUI.Label (newRect1, "Score - " + ps.pcs.iScore.ToString ());
			newRect1.y += 20;
			GUI.Label (newRect1, "Charges - " + ps.pcs.iCharges.ToString ());
			newRect1.y += 20;
			GUI.Label (newRect1, "Health - " + ps.pcs.iCurrentHealth.ToString ());
			newRect1.y += 30;
			//GUI.Label (newRect1, "Input : " + lgs.gds.strInputChoice);
			//newRect1.y += 40;
			if(Application.loadedLevelName == "GameScene") GUI.Label (newRect1, "[1] - " + lgs.gds.strTop + " - " + lgs.gds.iTop.ToString ());
			newRect1.y += 20;
			if(Application.loadedLevelName == "GameScene") GUI.Label (newRect1, "[2] - " + lgs.gds.strSecond + " - " + lgs.gds.iSecond.ToString ());
			newRect1.y += 20;
			if(Application.loadedLevelName == "GameScene") GUI.Label (newRect1, "[3] - " + lgs.gds.strThird + " - " + lgs.gds.iThird.ToString ());
			newRect1.y += 20;
        	
			if (ps.mis.bUsingButtonInput) {
				if (GUI.Button (new Rect (Screen.width / 2 - Screen.width / 4, Screen.height, iMoveButtonWidth, iMoveButtonHeight), "Left")) {
					ps.pms.AdjustVelocityWithButtons ("left");
				}
				if (GUI.Button (new Rect (Screen.width / 2 + Screen.width / 4, Screen.height, iMoveButtonWidth, iMoveButtonHeight), "Right")) {
					ps.pms.AdjustVelocityWithButtons ("right");
				}
			}

			if (lgs.gds.bPromptForName)
			{
				centeredStyle.alignment = TextAnchor.UpperCenter;

				int iCurRow = 0;
				int iMaxRow = 5;
        	
				int iCurCol = 0;
        	
				int iStartPosX = 0;
				int iStartPosY = Screen.height / 2;
        	
				iKeyWidth = Screen.width / 5;
				iKeyHeight = Screen.height / 12;
        	
				Rect rectAlpha = new Rect (iStartPosX, iStartPosY, iKeyWidth, iKeyHeight);
        	
				Rect rectName = rectAlpha;
				rectName.width = rectName.width * iMaxRow;
				rectName.y -= iKeyHeight * 2;
				GUI.Label (rectName, lgs.gds.strNewName, centeredStyle);
        	
				foreach (string str in LstrAlphabet) {
					if (GUI.Button (rectAlpha, str.ToString (), centeredStyle))
						strKeyPress = str;
					rectAlpha.x += iKeyWidth;
					iCurRow++;
        	
					if (iCurRow == iMaxRow) {
						rectAlpha.y += iKeyHeight;
						iCurCol++;
        	
						rectAlpha.x -= iKeyWidth * iMaxRow;
						iCurRow = 0;
					}
				}
        	
				rectAlpha.x += iKeyWidth * 2;
        	
				if (GUI.Button (rectAlpha, "Ent", centeredStyle)) {
					if (lgs.gds.strNewName.Length <= lgs.gds.iMaxStringLength)
					{
						lgs.gds.bPromptForName = false;
						lgs.gds.CheckForTopScore ();
						lgs.gds.SaveDataToPrefs ();
						Destroy (lgs.gds.gameObject);
						Destroy (GameObject.Find ("MusicData"));
						Application.LoadLevel ("TitleScene");
					}
				}
        	
				rectAlpha.x += iKeyWidth;
        	
				if (GUI.Button (rectAlpha, "Del", centeredStyle))
					strKeyPress = "delete";
			}
        	
			//if analog stick exists
			if (ps.mis.bHasAnalogStick) {
				if (bDebugMode) {
					// "Analog. Stick. I.D 'VAR'"
					GUI.Label (new Rect (rect.xMin + rect.width, rect.yMin + 20, rect.width, rect.height), "AS ID" + ps.mis.iAnalogId.ToString ()); 
					//['Analog Axis'] eg'(x,y)'
					GUI.Label (new Rect (rect.xMin + rect.width, rect.yMin + 30, rect.width, rect.height), ps.mis.v2AnalogAxis.ToString ());
				}
        	
				//Analog Stick Texture
				GUI.DrawTexture (CalculateAnalogTexturePosition (), t2dAnalogStick);
        	
				//Analog stick line
				/*Vector3 v3Start = new Vector3(ps.tis.v2AnalogStart.x, ps.tis.v2AnalogStart.y, 0);
        	    lr.SetPosition(0, v3Start);
        	
        	    Vector3 v3End = new Vector3(
        	        ps.tis.v2AnalogStart.x + (ps.tis.v2AnalogAxis.x * fTextureStepSize),
        	        (Screen.height - ps.tis.v2AnalogStart.y - (ps.tis.v2AnalogAxis.y * fTextureStepSize)),
        	        0);
        	    lr.SetPosition(1, v3End);*/
        	
			}
		}
		else
		{
			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), t2dLoadScreen);
			GUI.DrawTexture(new Rect(Screen.width / 2 - (iGuiBarLength * 2), (Screen.height / 4) * 3.5f, LoadBarLength(), iGuiBarHeight),t2dGreenPixel);

			if(fSwipeAnimTimer > 0) fSwipeAnimTimer -= Time.deltaTime;
			else
			{
				fSwipeAnimTimer = 0.7f;
				if(t2dSwipe == t2dSwipe0) t2dSwipe = t2dSwipe1;
				else t2dSwipe = t2dSwipe0;
			}
			if(strMessageToPlayer == "Swipe up - charge") 
			{
				GUI.DrawTexture(new Rect(Screen.width / 2 - ((Screen.width / 4.5f)/2), Screen.height / 8, Screen.width / 5, Screen.width /5), t2dSwipe);
				Debug.Log ("Drawing Texture!");
			}
		}

		centeredStyle.alignment = TextAnchor.UpperCenter;

		if (bHasMessageToDisplay)
		{
			if (bDisplayOnLeft && !bDisplayOnRight)
			{
				GUI.Label (new Rect (Screen.width / 4 - 150, Screen.height / 2, 300, 200), strMessageToPlayer, centeredStyle);
			}
			if (bDisplayOnRight && !bDisplayOnLeft)
			{
				GUI.Label (new Rect (((Screen.width / 4) * 3) - 150, Screen.height / 2, 300, 200), strMessageToPlayer, centeredStyle);
			}

			if (!bDisplayOnLeft && !bDisplayOnRight)
			{
				if(strMessageToPlayer == "Must enter 3 or less initials") 
				{
					GUI.Label (new Rect (Screen.width / 2 - 150, Screen.height /5 , 300, 200), strMessageToPlayer, centeredStyle);
				}
				else 
				{
					GUI.Label (new Rect (Screen.width / 2 - 150, Screen.height / 2, 300, 200), strMessageToPlayer, centeredStyle);
				}
			}
		}

		if (GUI.Button (new Rect (Screen.width - 10 - (iVolumeButtonWidth), 10 + (iVolumeButtonHeight / 2), iVolumeButtonWidth, iVolumeButtonHeight), "V", centeredStyle)) {
			AudioSource audioSource = GameObject.Find("MusicData").GetComponent<AudioSource>();
			if(audioSource.volume == 0) audioSource.volume = 1;
			else audioSource.volume = 0;
		}

		if (bRewardScreen)
		{
			GUI.DrawTexture(new Rect(Screen.width / 2 - iRewardButtonWidth,
			                         Screen.height / 2,
			                         iRewardButtonWidth,
			                         iRewardButtonHeight)
			                ,t2dHealth);

			GUI.DrawTexture(new Rect(Screen.width / 2 - iRewardButtonWidth,
			                         Screen.height / 2 - iRewardButtonHeight,
			                         iRewardButtonWidth,
			                         iRewardButtonHeight)
			                ,t2dCharge);

			if(GUI.Button (new Rect(Screen.width / 2,
			                     Screen.height / 2,
			                     iRewardButtonWidth,
			                     iRewardButtonHeight),
			            "Health - 100",
			            centeredStyle))
			{
				if(ps.pcs.iCurrentHealth < ps.pcs.iMaxHealth && ps.lgs.gds.iTotalScore >= 100)
				{
					ps.lgs.gds.iTotalScore -= 100;
					ps.pcs.iCurrentHealth++;
				}
			}

			if(GUI.Button (new Rect(Screen.width / 2,
			                     Screen.height / 2 - iRewardButtonHeight,
			                     iRewardButtonWidth,
			                     iRewardButtonHeight),
			            "Charge - 10",
			            centeredStyle))
			{
				if(ps.pcs.iCurrentHealth < ps.pcs.iMaxHealth && ps.lgs.gds.iTotalScore >= 10)
				{
					ps.lgs.gds.iTotalScore -= 10;
					ps.pcs.iCharges++;
				}
			}

			if(GUI.Button (new Rect(Screen.width / 2 - iRewardButtonWidth / 2,
			                        Screen.height / 2 + iRewardButtonHeight * 2,
			                        iRewardButtonWidth,
			                        iRewardButtonHeight),
			               "Continue",
			               centeredStyle))
			{
				ps.lgs.gds.iLoopCount++;
				ps.lgs.gds.iCharges = ps.pcs.iCharges;
				ps.lgs.gds.iHealth = ps.pcs.iCurrentHealth;
				Application.LoadLevel("GameScene");
			}
		}
	}
	
	Rect RectAtGameObjectPosition(GameObject goPos)
    {
        Camera camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
		Vector3 screenPos = camera.WorldToScreenPoint(goPos.transform.position);

		Rect rect = new Rect(screenPos.x, Screen.height - screenPos.y, iGuiBarLength, iGuiBarHeight);
        return rect;
    }

    float HealthBarLength()
    {
        float f;
        f = iGuiBarLength * (ps.pcs.iCurrentHealth / (float)ps.pcs.iMaxHealth);
        return f;
    }

	float LoadBarLength()
	{
		float f;
		f = (iGuiBarLength * 4) * (fMessageTimer / 2.0f);
		return f;
	}

    /// <summary> Using analog stick starting position and the analog stick axis to animate the rect inside the GUI function </summary>
    /// <returns> Rect for positioning the draw of the analog stick texture </returns>
    Rect CalculateAnalogTexturePosition()
    {
        
        Rect rect = new Rect
            (
            (ps.mis.v2AnalogStart.x + (ps.mis.v2AnalogAxis.x * fTextureStepSize)) - (fAnalogTextureHeight / 2),                   // Texture Position x
            //(Screen.height - ps.tis.v2AnalogStart.y - (ps.tis.v2AnalogAxis.y * fTextureStepSize)) - (fAnalogTextureHeight / 2),   // Texture Position y (for moving image up and down by an amount aswell)
            (Screen.height - ps.mis.v2AnalogStart.y) - (fAnalogTextureHeight / 2) - 200.0f, // Texture Position y
            fAnalogTextureWidth,     // Texture Width
            fAnalogTextureHeight     // Texture Height
            );
        Debug.Log(rect.ToString());
        return rect;
    }
}
