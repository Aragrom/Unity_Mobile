///<author> Graham Alexander MacDonald 03/03/2015 </author>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameDataScript : MonoBehaviour
{
	public static int iGAME_LIMIT = 666;
	public int iLoopCount = 0;

	public string strInputChoice = "gyro";

	public int iTotalScore;
	public string strDataFile;

	public float fAnalogDeadZone = 5.0f;

	public int iCharges = 0;
	public int iHealth = 3;

	public int iTop = 0;
	public string strTop = "";
	public int iSecond = 0;
	public string strSecond = "";
	public int iThird = 0;
	public string strThird = "";

	public string strNewName;
	public int iMaxStringLength = 3;
	public bool bPromptForName;

	public LevelGeneratorScript lgs;

	void Awake() 
	{
		DontDestroyOnLoad(this.gameObject);
		//strInputChoice = PlayerPrefs.GetString ("input");
		//if (strInputChoice == null)
			//strInputChoice = "gyro";

		iTop = PlayerPrefs.GetInt ("top");
		strTop = PlayerPrefs.GetString ("strTop");

		iSecond = PlayerPrefs.GetInt ("second");
		strSecond = PlayerPrefs.GetString ("strSecond");

		iThird = PlayerPrefs.GetInt ("third");
		strThird = PlayerPrefs.GetString ("strThird");
	}

	void Start()
	{
		//LoadDataFromFile ();
	}

	void Update()
	{
		if(bPromptForName)
		{
			PlayerGUIScript pgs = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerGUIScript>();
			if(pgs.strKeyPress != "") 
			{
				if(pgs.strKeyPress == "delete")
				{
					if(strNewName.Length > 0) strNewName = strNewName.Remove(strNewName.Length - 1);
				}
				else if(strNewName.Length < iMaxStringLength) strNewName += pgs.strKeyPress;

				pgs.strKeyPress = "";
			}
		}
	}

	public void SaveDataToPrefs()
	{
		//PlayerPrefs.SetString ("input", strInputChoice);
		//PlayerPrefs.SetFloat ("analogDeadZone", fAnalogDeadZone);

		PlayerPrefs.SetInt ("top", iTop);
		PlayerPrefs.SetString ("strTop", strTop);

		PlayerPrefs.SetInt ("second", iSecond);
		PlayerPrefs.SetString ("strSecond", strSecond);

		PlayerPrefs.SetInt ("third", iThird);
		PlayerPrefs.SetString ("strThird", strThird);

		/*
		PlayerPrefs.SetString("dataFile",strDataFile);
		*/
	}

	void LoadDataFromFile()
	{
		//TextAsset taData = (TextAsset)Resources.Load("Text_Files/text", typeof(TextAsset));
		//strDataFile = taData.ToString ();
	}

	public void CheckForTopScore()
	{
		bool bSavedScore = false;
		if (iLoopCount > iTop)
		{
			iThird = iSecond;
			strThird = strSecond;

			iSecond = iTop;
			strSecond = strTop;

			iTop = iLoopCount;
			strTop = strNewName;
			bSavedScore = true;

		} 
		else if (iLoopCount > iSecond && !bSavedScore)
		{
			iThird = iSecond;
			strThird = strSecond;
			
			iSecond = iLoopCount;
			strSecond = strNewName;
			bSavedScore = true;
		}
		else if (iLoopCount > iThird && !bSavedScore)
		{
			iThird = iLoopCount;
			strThird = strNewName;
			bSavedScore = true;
		}
	}
}
