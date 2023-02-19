///<author> Graham Alexander MacDonald 20/02/2015 </author>

using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour
{
    public int iGuiBarLength;
    public int iGuiBarHeight;

	public int iVolumeButtonWidth;
	public int iVolumeButtonHeight;

	public int iAdjustY;
    public Texture2D t2dTitleBackGround;
	public string strScene;
	public GameDataScript gds;
	public bool bGotFont = false;
	public Texture2D t2dEmptyButton;
	
	void Start()
    {
		gds = GameObject.Find("GameData").GetComponent<GameDataScript>();
		t2dTitleBackGround = (Texture2D)Resources.Load("GUI/titleScreen");
		strScene = "TitleScene";
		Screen.sleepTimeout = SleepTimeout.NeverSleep; //so screen doesn't switch off when no touch input found
		Screen.orientation = ScreenOrientation.LandscapeLeft; //force landscape always
		t2dEmptyButton = (Texture2D)Resources.Load ("GUI/emptyButton");
		iGuiBarLength = Screen.width / 4;
		iGuiBarHeight = Screen.height / 8;
		iVolumeButtonWidth = Screen.width / 15;
		iVolumeButtonHeight = Screen.height / 10;
		iAdjustY = Screen.height / 4;
		Debug.Log ("Width: " + Screen.width.ToString () + " Height: " + Screen.height.ToString ());;
    }

    void OnGUI()
    {
		if (!bGotFont) {
			GUI.skin.font = (Font)Resources.Load ("Fonts/Wanted M54");
			if(Screen.width > 1500) 
			{
				GUI.skin.label.fontSize = 30;
				GUI.skin.button.fontSize = 30;
			}
			else
			{
				GUI.skin.label.fontSize = 20;
				GUI.skin.button.fontSize = 20;
			}
			bGotFont = true;
		}
		if (strScene == "TitleScene") TitleGUI ();
		if (strScene == "OptionScene") OptionGUI ();
    }

	void TitleGUI()
	{
		var centeredStyle = GUI.skin.GetStyle("Label");
		centeredStyle.alignment = TextAnchor.MiddleCenter;
		centeredStyle.normal.textColor = Color.white;

		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), t2dTitleBackGround);

		GUI.Label(new Rect((Screen.width / 2) - ((iGuiBarLength * 2) / 2), (Screen.height / 2) - (iGuiBarHeight / 2) - (iGuiBarHeight / 2) - iAdjustY, iGuiBarLength * 2, iGuiBarHeight), "Lady Yang - Team ONE - GCU", centeredStyle);

		if (GUI.Button (new Rect (Screen.width - 10 - (iVolumeButtonWidth), 10 + (iVolumeButtonHeight / 2), iVolumeButtonWidth, iVolumeButtonHeight), "V", centeredStyle)) {
			AudioSource audioSource = GameObject.Find("MusicData").GetComponent<AudioSource>();
			if(audioSource.volume == 0) audioSource.volume = 1;
			else audioSource.volume = 0;
		}

		centeredStyle.normal.textColor = Color.black;

		GUI.DrawTexture (new Rect ((Screen.width / 2) - (iGuiBarLength / 2), (Screen.height / 2) - (iGuiBarHeight / 2) + (iGuiBarHeight / 2) - iAdjustY, iGuiBarLength, iGuiBarHeight), t2dEmptyButton);

		if (GUI.Button(new Rect((Screen.width / 2) - (iGuiBarLength / 2), (Screen.height / 2) - (iGuiBarHeight / 2) + (iGuiBarHeight / 2) - iAdjustY, iGuiBarLength, iGuiBarHeight), "Start", centeredStyle)) Application.LoadLevel("GameScene");

		GUI.DrawTexture (new Rect((Screen.width / 2) - (iGuiBarLength / 2), (Screen.height / 2) - (iGuiBarHeight / 2) + (iGuiBarHeight / 2) + iGuiBarHeight - iAdjustY, iGuiBarLength, iGuiBarHeight), t2dEmptyButton);

		if (GUI.Button(new Rect((Screen.width / 2) - (iGuiBarLength / 2), (Screen.height / 2) - (iGuiBarHeight / 2) + (iGuiBarHeight / 2) + iGuiBarHeight - iAdjustY, iGuiBarLength, iGuiBarHeight), "Tutorial", centeredStyle)) Application.LoadLevel("TutorialScene");

		//if (GUI.Button(new Rect((Screen.width / 2) - (iGuiBarLength / 2), (Screen.height / 2) - (iGuiBarHeight / 2) + (iGuiBarHeight / 2) + iGuiBarHeight * 3 - iAdjustY, iGuiBarLength, iGuiBarHeight), "Options")) strScene = "OptionScene";
	}

	void OptionGUI()
	{
		var centeredStyle = GUI.skin.GetStyle("Label");
		centeredStyle.alignment = TextAnchor.MiddleCenter;

		if (gds.strInputChoice == "analog")
		{
			float f = 0.0f;
			f = GUI.HorizontalSlider (new Rect (25,25,100,30), f, 0.0f, 5.0f);
			gds.fAnalogDeadZone = f;
		}


		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), t2dTitleBackGround);
		GUI.Label(new Rect((Screen.width / 2) - (iGuiBarLength / 2), (Screen.height / 2) - (iGuiBarHeight / 2) - iAdjustY, iGuiBarLength, iGuiBarHeight), "Options - Player Input: " + gds.strInputChoice, centeredStyle);

		if (GUI.Button(new Rect((Screen.width / 2) - (iGuiBarLength / 2) - (iGuiBarLength / 2), (Screen.height / 2) - (iGuiBarHeight / 2) + (iGuiBarHeight / 2) - iAdjustY, iGuiBarLength, iGuiBarHeight), "Analog")) gds.strInputChoice = "analog";
		if (GUI.Button(new Rect((Screen.width / 2) + (iGuiBarLength / 2) - (iGuiBarLength / 2), (Screen.height / 2) - (iGuiBarHeight / 2) + (iGuiBarHeight / 2) - iAdjustY, iGuiBarLength, iGuiBarHeight), "Gyro")) gds.strInputChoice = "gyro";
		//if (GUI.Button(new Rect((Screen.width / 2) - (iGuiBarLength / 2), (Screen.height / 2) - (iGuiBarHeight / 2) + (iGuiBarHeight / 2) + iGuiBarHeight * 2 - iAdjustY, iGuiBarLength, iGuiBarHeight), "Button")) gds.strInputChoice = "button";
		if (GUI.Button (new Rect ((Screen.width / 2) - (iGuiBarLength / 2), (Screen.height / 2) - (iGuiBarHeight / 2) + (iGuiBarHeight / 2) + iGuiBarHeight * 2 - iAdjustY, iGuiBarLength, iGuiBarHeight), "Reset Data")) {
			gds.iTop = 0;
			gds.strTop = "";
			gds.iSecond = 0;
			gds.strSecond = "";
			gds.iThird = 0;
			gds.strThird = "";
			gds.strInputChoice = "gyro";
			gds.SaveDataToPrefs ();
		}
		if (GUI.Button(new Rect((Screen.width / 2) - (iGuiBarLength / 2), (Screen.height / 2) - (iGuiBarHeight / 2) + (iGuiBarHeight / 2) + iGuiBarHeight * 3 - iAdjustY, iGuiBarLength, iGuiBarHeight), "Title Screen")) strScene = "TitleScene";
	}
}
