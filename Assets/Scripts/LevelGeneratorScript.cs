///<author> Graham Alexander MacDonald 06/02/2015 </author>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGeneratorScript : MonoBehaviour
{
    public Transform tPlayer;
    public Vector3 v3SpawnPoint;
    public float fSpawnEnemyTimer;
    public Vector3 v3EnemySpawnDistance;

    public List<GameObject> LgoLevel; //Actual created game objects in scene
    public List<GameObject> LgoEnemies; //Actual Enemies "in game" at runtime
    public int iLevelSize; // Amount of sub levels to be generated
    public int iDeadEndLimit = 7;

    public int iBuildDirection = 0; // 0-3 {north, east, south, west}
    public float fX = 0;
    public float fY = 0;
    public float fZ = 0;
    public float fBuildHeightLimit; //max height ramps can climb to
    public int iRampDir; //Holds positive or negative value representing ramp up or down direction
    public bool bPreviousWasRamp; // when true lowering next node
    public bool bPreviousWasTurnPoint; //When true add an extra small node after turn direction

    public GameObject goActivePlayer; //Instantiated player

    // Resources
    public GameObject goPlayer; //Loaded prefab

    public List<GameObject> LgoScene; //The "to be" generated list of prefabs

	public List<GameObject> LgoHouses;

    public GameObject goSubStart; //Start 'node'
    public List<GameObject> LgoSubLevelTypes; //List of ALL 'sub nodes'
    public GameObject goSub1;
    public GameObject goSub2;
    public GameObject goSub3;
    public GameObject goTurnPoint;
    public GameObject goRamp; //nothing unique atm so rather redundant
    public GameObject goSubEnd; //End 'node'

	public List<GameObject> LgoEvent;
	public GameObject goJump;
	public GameObject goMovingLeftRight;
	public GameObject goMovingBlocked;
	public GameObject goBarrelAmbush;
	public GameObject goPlankDrop;

    public GameObject goEndWall;

    public GameObject goNew;
    public GameObject goLastCreated; //Last placed 'node'

    public List<GameObject> LgoEnemyTypes; //List of ALL possible enemies
    public GameObject goEnemy1;
    public GameObject goEnemy2;
    public GameObject goEnemy3;

    public GameObject goObstacle;

    // for generating below path when path is "raised in the air"
    public GameObject goHouse1;
    public GameObject goHouse2;
    public GameObject goHouse3;
    //

	public int[] AiRewardScreenLevels;

	public GameObject goFloor;

	public GameObject goHouseType;

    public bool bResourcesLoaded = false; //Instead of Application.LoadLevel

	public GameDataScript gds; //Used for carrying variables and data across level loads

	void Awake()
	{
		goPlayer = (GameObject)Resources.Load("Prefabs/Player");
		v3SpawnPoint = new Vector3(10.0f, 5.0f, 0);
		SpawnPlayer();
	}

    /// <summary> Use this for initialization </summary>
    void Start()
    {
		gds = GameObject.Find("GameData").GetComponent<GameDataScript>();
		gds.lgs = this.transform.GetComponent<LevelGeneratorScript>(); //this script

		if(Application.loadedLevelName == "GameScene")
        {
            iLevelSize = 50;
            fBuildHeightLimit = 25.0f;
        }
        else if (Application.loadedLevelName == "TutorialScene")
        {
            iLevelSize = 20;
            fBuildHeightLimit = 0; //There is no ramps so it wont go up or down anyway.
        }
        
        fSpawnEnemyTimer = 10.0f;
        v3EnemySpawnDistance = new Vector3(0.0f, 2.0f, 10.0f);

        LoadResources();
	
        GenerateListSubLevels();
        GenerateListEnemyTypes();
		if (Application.loadedLevelName == "GameScene")	GenerateListEventTypes();
        GenerateListScene();
		
		//Build
        BuildPath();
        if (Application.loadedLevelName == "GameScene") BuildBelowPath ();
        if (Application.loadedLevelName == "TutorialScene") BuildWallDrop();

		goFloor = GameObject.Find ("Floor").gameObject;

		AiRewardScreenLevels = new int[20];
		int i = 0;
		int iLevel = 0;
		while(i < 20)
		{
			iLevel += 5;
			AiRewardScreenLevels[i] = iLevel;
			i++;
		}
    }

    /// <summary> Update is called once per frame </summary>
    void Update()
    {
        SpawnTimer();
    }

    void LoadResources()
    {
        //Load 'Nodes' for track placement
        goSubStart = (GameObject)Resources.Load("Prefabs/SubStart");
        goSubEnd = (GameObject)Resources.Load("Prefabs/SubEnd");
        goSub1 = (GameObject)Resources.Load("Prefabs/Sub1");
        goSub2 = (GameObject)Resources.Load("Prefabs/Sub2");
        goSub3 = (GameObject)Resources.Load("Prefabs/Sub3");

        if (Application.loadedLevelName == "GameScene")
        {
            goTurnPoint = (GameObject)Resources.Load("Prefabs/TurnPoint");

            goRamp = (GameObject)Resources.Load("Prefabs/Ramp");

            //Load 'Obstacle' prefabs
            goObstacle = (GameObject)Resources.Load("Prefabs/Obstacle");

            //Load 
            goHouse1 = (GameObject)Resources.Load("Prefabs/House1");
            goHouse2 = (GameObject)Resources.Load("Prefabs/House2");
            goHouse3 = (GameObject)Resources.Load("Prefabs/House3");

			goMovingLeftRight = (GameObject)Resources.Load ("Prefabs/MovingLeftRight");
			goMovingBlocked = (GameObject)Resources.Load ("Prefabs/MovingBlocked");
			goBarrelAmbush = (GameObject)Resources.Load ("Prefabs/BarrelAmbush");
			goPlankDrop = (GameObject)Resources.Load ("Prefabs/PlankDrop");
			goJump = (GameObject)Resources.Load ("Prefabs/Jump");
        }

        if (Application.loadedLevelName == "TutorialScene")
        {
            goTurnPoint = (GameObject)Resources.Load("Prefabs/BigTurnPoint");
            goSubEnd = (GameObject)Resources.Load("Prefabs/BigEnd");
            goSub3 = (GameObject)Resources.Load("Prefabs/SubTutorial");
            goSub1 = goSub3;
            goSub2 = goSub3;
            goEndWall = (GameObject)Resources.Load("Prefabs/EndWall");

			goJump = (GameObject)Resources.Load ("Prefabs/TutJump");

        }

        //Load 'Enemy' prefabs
        goEnemy1 = (GameObject)Resources.Load("Prefabs/Enemy1");
        goEnemy2 = (GameObject)Resources.Load("Prefabs/Enemy2");
        goEnemy3 = (GameObject)Resources.Load("Prefabs/Enemy3");
    }

    void SpawnTimer()
    {
        if (fSpawnEnemyTimer > 0) fSpawnEnemyTimer -= Time.deltaTime;
        else
        {
            SpawnEnemy();
            fSpawnEnemyTimer = Random.Range(0.0f, 3.0f);
        }
    }

    public void FindAllEnemies(GameObject goToDestroy)
    {
        LgoEnemies.Clear();
        goToDestroy.SetActive(false);
        Destroy(goToDestroy);
        GameObject[] aGo;
        aGo = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject go in aGo)
        {
            LgoEnemies.Add(go);
        }
    }

    void SpawnEnemy()
    {
        float f = Random.Range(0.0f, 2.0f); // Random Enemy type
		Vector3 v3SpawnPoint = GameObject.FindGameObjectWithTag("Player").transform.Find ("EnemySpawnPoint").transform.position;

		GameObject go = (GameObject)Instantiate(LgoEnemyTypes[(int)f], v3SpawnPoint, Quaternion.identity);

        go.GetComponent<EnemyScript>().iEnemyId = LgoEnemies.Capacity;
        LgoEnemies.Add(go);
    }

    void GenerateListSubLevels()
    {
        LgoSubLevelTypes.Add(goSub1);
        LgoSubLevelTypes.Add(goSub2);
        LgoSubLevelTypes.Add(goSub3);
    }

    void GenerateListEnemyTypes()
    {
        LgoEnemyTypes.Add(goEnemy1);
        LgoEnemyTypes.Add(goEnemy2);
        LgoEnemyTypes.Add(goEnemy3);
    }

	void GenerateListEventTypes()
	{
		LgoEvent.Add(goJump);
		LgoEvent.Add(goMovingLeftRight);
		LgoEvent.Add(goBarrelAmbush);
		LgoEvent.Add (goPlankDrop);
		LgoEvent.Add(goMovingBlocked);
		LgoEvent.Add(goMovingBlocked);
	}
	
	void GenerateListScene()
    {
        //'Start platform' at start of list
        LgoScene.Add(goSubStart);
		LgoScene.Add (goSub3);
		LgoScene.Add (goSub3);
		LgoScene.Add (goSub3);
		LgoScene.Add (goSub3);

        //Decrment to track when to place a turning point
        int iNegTurnPoint = 0;
        int iNegRamp = 0;
		int iNegEvent = 0;
		float fScalar;

		if (iLevelSize > 100)
			fScalar = iLevelSize / 100;
		else fScalar = 1.0f;

        if (Application.loadedLevelName == "GameScene") 
        {
            iNegRamp = (int)Random.Range((iLevelSize / 25) / fScalar, (iLevelSize / 20) / fScalar);
			iNegTurnPoint = (int)Random.Range((iLevelSize / 15) / fScalar, (iLevelSize / 10) / fScalar);
			iNegEvent = (int)Random.Range((iLevelSize / 20) / fScalar, (iLevelSize / 15) / fScalar);
        }
        else if (Application.loadedLevelName == "TutorialScene") 
        {
            
			iNegEvent = 7777;
			iNegRamp = 7777; //Never use ramps in tutorial scene
            iNegTurnPoint = 4; 
        }        

        //Generate 'Sub levels' based on the int defining 'Level size'
        int iPos = 1;
        while (iPos < iLevelSize)
        {
            int iRandom = (int)Random.Range(0.0f, LgoSubLevelTypes.Capacity - 1);
            LgoScene.Add(LgoSubLevelTypes[iRandom]);

            iPos++;
            iNegTurnPoint--;
            iNegRamp--;
			iNegEvent--;

            //When decrement reaches 0 add a "Turn Point" prefab
            if (iNegTurnPoint <= 0)
            {
				if (Application.loadedLevelName == "GameScene") 
					iNegTurnPoint = (int)Random.Range((iLevelSize / 15) / fScalar, (iLevelSize / 10) / fScalar);
				else 
					iNegTurnPoint = 7777; //Only one turn in tutorial scene
                LgoScene.Add(goTurnPoint);

                iPos++;
            }

            //When decrement reaches 0 add a "Ramp" prefab
            if (iNegRamp <= 0)
            {
				iNegRamp = (int)Random.Range((iLevelSize / 25) / fScalar, (iLevelSize / 20) / fScalar);
				LgoScene.Add(goRamp);

                iPos++;
            }

			if (iNegEvent <= 0)
			{
				iNegEvent = (int)Random.Range((iLevelSize / 20) / fScalar, (iLevelSize / 15) / fScalar);
				LgoScene.Add(LgoEvent[(int)(Random.Range(0.0f, 6.0f))]);
				
				iPos++;
			}

			if(Application.loadedLevelName == "TutorialScene")
			{
				//Custom sequence is in tutorial level
				switch (iPos)
				{
				case 12:
					LgoScene.Add(goJump);
					iPos++;
					break;

				default:
					break;
				}
			}
        }

        //'End platform' at End of list
        LgoScene.Add(goSubEnd);
    }

    void BuildPath()
    {
        iBuildDirection = 0;
        bool bPreviousWasTurnPoint = false;
        bPreviousWasRamp = false;
        bool bBuild = true;

        foreach (GameObject go in LgoScene)
        {
            if (bBuild)
            {
				//Limit Path check
				iDeadEndLimit = 3;
				while (!CheckForEmptyPath() && iDeadEndLimit >= 0)
				{
					Debug.Log("Dead End!");
					
					//Create new turn point because path is "BLOCKED"
					LgoLevel.Remove(goLastCreated);
					Destroy(goLastCreated);
					goLastCreated = (GameObject)Instantiate(goTurnPoint, v3GetBuildLocation(), qGetSubRotation());
					LgoLevel.Add(goLastCreated);
					goLastCreated.GetComponent<TurnPointScript>().iDirection = iBuildDirection;
					bPreviousWasTurnPoint = true;
					
					//Reverse any previous movement on the y-axis from previous positioning
					//if (goLastCreated.name == "Ramp(Clone)" || bPreviousWasRamp)
					//fY += -iRampDir * 2.5f;
					
					//Limit the amount of times into this cycle
					if (iDeadEndLimit >= 0)
					{
						//Adjust paths
						AdjustBuildDirection();
						iDeadEndLimit--;
					}
					else
					{
						bBuild = false;
						Debug.Log("FATAL ERROR : (DEAD END LIMIT REACHED) LAST CREATED (" + goLastCreated.name.ToString()
						          + ") POS (" + goLastCreated.transform.position.ToString()
						          + ") LAST WAS TURN POINT : " + bPreviousWasTurnPoint.ToString()
						          + " LAST WAS RAMP : " + bPreviousWasRamp.ToString());
						LgoLevel.Remove(goLastCreated);
						Destroy(goLastCreated);
						goLastCreated = (GameObject)Instantiate(goSubEnd, v3GetBuildLocation(), qGetSubRotation());
						LgoLevel.Add(goLastCreated);
					}
				}

				//For an additional piece of track after turn point
				if (bPreviousWasTurnPoint)
				{
					if (Application.loadedLevelName == "TutorialScene")
						for (int i = 0; i < 1; i++) 
					{
						IncrementBuildPosition();
					}
					IncrementBuildPosition();
					goLastCreated = (GameObject)Instantiate(goSub1, v3GetBuildLocation(), qGetSubRotation());
					LgoLevel.Add(goLastCreated);
					
					bPreviousWasTurnPoint = false;
				}
				
				//For an additional piece of track before turn point
				if (Application.loadedLevelName == "GameScene" && go.tag == "TurnPoint")
				{
					IncrementBuildPosition();
					goLastCreated = (GameObject)Instantiate(goSub1, v3GetBuildLocation(), qGetSubRotation());
					LgoLevel.Add(goLastCreated);
				}

                if (go.name == "Ramp")
                {
                    CalibrateForRamp();
                }

                if (bPreviousWasRamp)
                {
                    fY += iRampDir * 2.5f;
                    bPreviousWasRamp = false;
                }

                if (Application.loadedLevelName == "TutorialScene")
                {
                    int iSteps = 0;
                    if (go.name == "BigEnd")
                    {
                        fY -= (int)Random.Range(0,3) * 20;
                        iSteps = 1;
                    }
                    else if (go.tag == "TurnPoint")
					{
						iSteps = 2;
					}

                    while (iSteps > 0)
                    {
                        IncrementBuildPosition();
                        iSteps--;
                    }

					if (go.tag == "TurnPoint")
					{
						AdjustBuildDirection();
						bPreviousWasTurnPoint = true;
					}
                }

				if(go.name == "PlankDrop") 
				{
					int iLimit = 1;
					int i = 0;
					while (i < iLimit)
					{
						IncrementBuildPosition();
						i++;
					}
				}

                if(go.name != "BigTurnPoint") IncrementBuildPosition();
                goLastCreated = (GameObject)Instantiate(go, v3GetBuildLocation(), qGetSubRotation());
                LgoLevel.Add(goLastCreated);

				if(goLastCreated.name == "PlankDrop(Clone)") 
				{
					int iLimit = 1;
					int i = 0;
					while (i < iLimit)
					{
						IncrementBuildPosition();
						i++;
					}

					if(iBuildDirection == (int)TurnPointScript.eDirection.west
					   ||iBuildDirection == (int)TurnPointScript.eDirection.east)
					{
						Vector3 v3Rot = goLastCreated.transform.rotation.eulerAngles;
						Debug.Log ("Event Flipped - BEFORE = " + goLastCreated.transform.rotation.eulerAngles.ToString());
						goLastCreated.transform.rotation = Quaternion.Euler(v3Rot.x, v3Rot.y + 180, v3Rot.z);
						Debug.Log ("Event Flipped  = " + goLastCreated.transform.rotation.eulerAngles.ToString());
					}
				}

				if(goLastCreated.name == "MovingLeftRight(Clone)")
				{
					goLastCreated.GetComponent<MovingPlatformScript> ().iDir = iBuildDirection;
				}

				if(goLastCreated.name == "MovingBlocked(Clone)")
					goLastCreated.transform.Find("Obstacle").GetComponent<MovingPlatformScript> ().iDir = iBuildDirection;

                //Get build direction from turn point
				if(go.tag == "TurnPoint")
				{
					bPreviousWasTurnPoint = true;
					if (Application.loadedLevelName == "GameScene") AdjustBuildDirection();
					if (Application.loadedLevelName == "TutorialScene") goLastCreated.GetComponent<TurnPointScript>().iDirection = iBuildDirection;
				}

                //if the object was a "Ramp" then orientate it correctly
                if (goLastCreated.name == "Ramp(Clone)")
                {
                    GiveRampProperties(goLastCreated);
                }
            }
        }
    }

    void BuildBelowPath()
    {
        foreach (GameObject go in LgoLevel)
        {
			//goHouseType = goHouse1;

            if (go.transform.position.y > 0)
            {
				if(go.name == "Jump(Clone)"
				   || go.name == "Ramp(Clone)"
				   || go.name == "MovingLeftRight(Clone)") goHouseType = new GameObject();

				if (go.name == "Sub1(Clone)") goHouseType = goHouse1;

                if (go.name == "Sub2(Clone)") goHouseType = goHouse2;

                if (go.name == "Sub3(Clone)"
				    || go.name == "MovingBlocked(Clone)") goHouseType = goHouse3;

                if (go.name == "TurnPoint(Clone)"
                    || go.name == "SubStart(Clone)"
                    || go.name == "SubEnd(Clone)") goHouseType = goHouse1;

                Vector3 v3HouseSpawnPosition = go.transform.position + Vector3.down / 10; // To give a small gap to textures done over lap
				v3HouseSpawnPosition += (Vector3.down * 2.5f);
                int iHouseHeight = (int)go.transform.position.y / 5;
                while (iHouseHeight > 0)
                {
                    Instantiate(goHouseType, v3HouseSpawnPosition, go.transform.rotation);
                    iHouseHeight--;
					v3HouseSpawnPosition += (Vector3.down * 5.0f);
				}
			}
        }
    }

    void BuildWallDrop()
    {
        int i = ((int)-fY / 25) + 2;
        while(i > 0)
        {
            fY += 22;
            Instantiate(goEndWall, v3GetBuildLocation(), qGetSubRotation());
            i--;
        }
    }

    void SpawnPlayer()
    {
        goActivePlayer = (GameObject)Instantiate(goPlayer, v3SpawnPoint, Quaternion.identity);
    }

    bool CheckForEmptyPath()
    {
		bool bResult = true;
		Vector3 v3 = v3GetBuildLocation ();

		int i = 0;
        while(i <= 2)
		{
			if(i == 0)
			{
				if (iBuildDirection == (int)TurnPointScript.eDirection.north) 
					v3 += new Vector3 (0, 0, 10);
				if (iBuildDirection == (int)TurnPointScript.eDirection.east)
					v3 += new Vector3 (10, 0, 0);
				if (iBuildDirection == (int)TurnPointScript.eDirection.south)
					v3 += new Vector3 (0, 0, -10);
				if (iBuildDirection == (int)TurnPointScript.eDirection.west)
					v3 += new Vector3 (-10, 0, 0);
			}

			if(i == 2)
			{
				if (iBuildDirection == (int)TurnPointScript.eDirection.north) 
					v3 += new Vector3 (0, 0, -10);
				if (iBuildDirection == (int)TurnPointScript.eDirection.east)
					v3 += new Vector3 (-10, 0, 0);
				if (iBuildDirection == (int)TurnPointScript.eDirection.south)
					v3 += new Vector3 (0, 0, 10);
				if (iBuildDirection == (int)TurnPointScript.eDirection.west)
					v3 += new Vector3 (10, 0, 0);
			}
        	
			Vector3 v3MediumStep = v3;
			Vector3 v3BiggerStep = v3;
			Vector3 v3BiggerStep1 = v3;
			Vector3 v3BiggerStep2 = v3;
        	
			Vector3 v3ToAdd = new Vector3 ();
			//Based on direction check in front of the current build position
			if (iBuildDirection == (int)TurnPointScript.eDirection.north) v3ToAdd = new Vector3 (10, 0, 0);
			if (iBuildDirection == (int)TurnPointScript.eDirection.east) v3ToAdd = new Vector3 (0, 0, -10);
			if (iBuildDirection == (int)TurnPointScript.eDirection.south) v3ToAdd = new Vector3 (-10, 0, 0);
			if (iBuildDirection == (int)TurnPointScript.eDirection.west) v3ToAdd = new Vector3 (0, 0, 10);
        	
			v3 += v3ToAdd;
			v3MediumStep += v3ToAdd * 2;
			v3BiggerStep += v3ToAdd * 3;
			v3BiggerStep1 += v3ToAdd * 4;
			v3BiggerStep2 += v3ToAdd * 5;

			//Check all existing objects
			foreach (GameObject go_ in LgoLevel) {
				Vector3 v3GoPosition = go_.transform.position;
				if ((v3GoPosition.x == v3.x
					&& v3GoPosition.z == v3.z)
					|| (v3GoPosition.x == v3MediumStep.x
					&& v3GoPosition.z == v3MediumStep.z)
					|| (v3GoPosition.x == v3BiggerStep.x
					&& v3GoPosition.z == v3BiggerStep.z)
					|| (v3GoPosition.x == v3BiggerStep1.x
					&& v3GoPosition.z == v3BiggerStep1.z)
					|| (v3GoPosition.x == v3BiggerStep2.x
					&& v3GoPosition.z == v3BiggerStep2.z)) {
					bResult = false;
				}
			}
			i++;
		}

        return bResult;
    }

    Quaternion qGetSubRotation()
    {
        Quaternion q = new Quaternion();
		if (iBuildDirection == (int)TurnPointScript.eDirection.north) q = Quaternion.Euler(0, 0, 0);
        if (iBuildDirection == (int)TurnPointScript.eDirection.east) q = Quaternion.Euler(0, 270, 0);
        if (iBuildDirection == (int)TurnPointScript.eDirection.south) q = Quaternion.Euler(0, 180, 0);
        if (iBuildDirection == (int)TurnPointScript.eDirection.west) q = Quaternion.Euler(0, 90, 0);
        return q;
    }

    void IncrementBuildPosition()
    {
        if (iBuildDirection == (int)TurnPointScript.eDirection.north) fX += 10;
        if (iBuildDirection == (int)TurnPointScript.eDirection.east) fZ -= 10;
        if (iBuildDirection == (int)TurnPointScript.eDirection.south) fX -= 10;
        if (iBuildDirection == (int)TurnPointScript.eDirection.west) fZ += 10;
    }

    void AdjustBuildDirection()
    {
        //Decide if to turn "left" or "right"   
        int i = (int)Random.Range(-6, 6);//6 

        if (i > 0) iBuildDirection++;
        else iBuildDirection--;

        //Limiter
        if (iBuildDirection > 3) iBuildDirection = 0;
        else if (iBuildDirection < 0) iBuildDirection = 3;

        if (goLastCreated != null)
		{
			if (goLastCreated.name == "TurnPoint(Clone)" 
				|| goLastCreated.name == "BigTurnPoint(Clone)") 
				goLastCreated.GetComponent<TurnPointScript> ().iDirection = iBuildDirection;
		}
    }

    Vector3 v3GetBuildLocation()
    {
        Vector3 v3 = new Vector3(fX, fY, fZ);
        return v3;
    }

    void GiveRampProperties(GameObject go)
    {
        Debug.Log("BEFORE RAMP: (" + goLastCreated.transform.rotation.eulerAngles.ToString());

        Vector3 v3CurRot = goLastCreated.transform.rotation.eulerAngles;

        if (iBuildDirection == (int)TurnPointScript.eDirection.north
            || iBuildDirection == (int)TurnPointScript.eDirection.south)
        {
            //if going down flip ramp
            if (iRampDir > 0) goLastCreated.transform.rotation = Quaternion.Euler(v3CurRot.x, v3CurRot.y, v3CurRot.z + 26.5650511771f);
            else goLastCreated.transform.rotation = Quaternion.Euler(v3CurRot.x, v3CurRot.y + 180.0f, v3CurRot.z + 26.5650511771f);
        }

        if (iBuildDirection == (int)TurnPointScript.eDirection.east
            || iBuildDirection == (int)TurnPointScript.eDirection.west)
        {
            //if going down flip ramp
            if (iRampDir < 0) goLastCreated.transform.rotation = Quaternion.Euler(v3CurRot.x, v3CurRot.y, v3CurRot.z + 26.5650511771f);
            else goLastCreated.transform.rotation = Quaternion.Euler(v3CurRot.x, v3CurRot.y + 180.0f, v3CurRot.z + 26.5650511771f); ;
        }

        goLastCreated.transform.localScale = new Vector3(1.11803398875f, 1.0f, 1.0f);

        bPreviousWasRamp = true; //for raising the next platform

        Debug.Log("AFTER RAMP: (" + goLastCreated.transform.rotation.eulerAngles.ToString());
    }

    void CalibrateForRamp()
    {
        if ((Random.Range(-1, 1) < 0 && fY > 0) || fY >= fBuildHeightLimit)
        {
            fY -= 2.5f;
            iRampDir = -1;
        }
        else
        {
            fY += 2.5f;
            iRampDir = 1;
        }
    }

    /// <summary> Manages GameObject deletion and clearing of list for
    /// - List of Prefab GameObjects (LgoScene))
    /// - List of Instantiated GameObjects (LgoLevel)
    /// - List of Instantiated GameObjects (LgoEnemies)</summary>
    public void DeleteScene()
    {
        foreach (GameObject goSceneToDelete in LgoScene)
        {
            Destroy(goSceneToDelete);
        }

        foreach (GameObject goLevelToDelete in LgoLevel)
        {
            Destroy(goLevelToDelete);
        }

        /*foreach (GameObject goEnemyToDelete in LgoEnemies)
        {
            Destroy(goEnemyToDelete);
        }*/

        LgoScene.Clear();
        LgoLevel.Clear();
        //LgoEnemies.Clear();
    }

	public void DeleteEnemies()
	{
		foreach (GameObject goEnemyToDelete in LgoEnemies)
        {
            Destroy(goEnemyToDelete);
        }
		LgoEnemies.Clear();
	}
}