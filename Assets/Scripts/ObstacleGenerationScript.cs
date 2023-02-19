///<Author> Calum Brown </Author>

using UnityEngine;
using System.Collections;

public class ObstacleGenerationScript : MonoBehaviour {

    ///<summary>
    /// This script is attatched to the ground object of the line prefab, it contains three lanes on which obstacles are randomly spawned.
    /// </summary>

    //maximum amount of obstacles we want, editable in the inspector
    public int iMaxObs;
    //minimum amount of obstacles we want, editable in the inspector
    public int iMinObs;
    //array of obstacles we call from, editable in the inspector
    public GameObject[] gObstacles;
    //array of lanes that are children to the gameobject this is attatched too
    public GameObject[] gLanes;
    //array that holds the obstacles to go into the left lane
    private GameObject[] gLeftArray;
    //array that holds the obstacles to go into the middle lane
    private GameObject[] gMidArray;
    //array that holds the obstacles to go into the right lane
    private GameObject[] gRightArray;


    /// <summary>
    ///     Start ()
    ///     Initialises the arrays for each lane that hold the objects to be instantiated in them 
    ///     gets the child lanes of the object and adds them to the array gLanes
    ///     Invokes the GenerateObstacles method that uses the above arrays to create the obstacles in position
    /// </summary>
	void Start () 
    {
        InitialiseArrays();
        GetChildren();
        GenerateObstacles();
	}

    /// <summary>
    ///     InitialiseArrays()
    ///     Initializes the arrays that hold the obstacles to be spawned in any one array
    /// </summary>
    void InitialiseArrays()
    {
        gLeftArray = new GameObject[2];
        gRightArray = new GameObject[2];
        gMidArray = new GameObject[2];
    }

    /// <summary>
    ///     GetChildren()
    ///     Adds each child of the ground object (each lane) to the gLanes array
    /// </summary>
    void GetChildren()
    {
        for (int iter =0; iter<3; iter++)
        {
           gLanes[iter] = transform.GetChild(iter).gameObject;
        }
    }

/// <summary>
///  GenerateObstacles()
///  Randomly decides how many objects to spawn then instantiates them in selected lanes on the ground section
///  Steps: 
///     randomly decide the number of objects to spawn
///     for each obstacle to spawn
///         randomly select the obstacle to spawn
///         randomly decide the lane to spawn it in
///         randomly select the position in lane to spawn it in
///         set position in selected lanes array to selected obstacle
///         check to see if lane is blocked by three obstacles
///         if so set one of them to be a flat obstacle so the player can continue
///         endfor
///     call methods to actually instantiate the obstacles
/// </summary>
    void GenerateObstacles()
    {
        // random integer value to determine the number of obstacles generated on the tile
        int iNoObs = Random.Range(iMaxObs, iMinObs);
        //temporary gameobject to hold the gameobject that is to be inserted into the lane
        GameObject gSelectedObstacle;
        // for the number of obstacles generated 
        for (int iter = 0; iter < iNoObs; iter++)
        {
            //select a random obstacle from the obstacle array and assign it to gSelectedObstacle
            gSelectedObstacle = (GameObject)gObstacles[Random.Range(0, gObstacles.Length)];
            //select a random lane
            int iRandLane = Random.Range(1, 4);
            //initialise an array of gameobjects 
            GameObject[] gSelectedLane = new GameObject[3];
            //choose lane based on value of iRandLane
            switch (iRandLane)
            {
                case (1):
                    gSelectedLane = gLeftArray;
                    break;
                case (2):
                    gSelectedLane = gMidArray;
                    break;
                case (3):
                    gSelectedLane = gRightArray;
                    break;
            }
            //randomly select a position on the lane 
            int iRandLanePos = Random.Range(0, 2);
            //add the randomly selected obstacle to the slected lane at the random position
            gSelectedLane[iRandLanePos] = gSelectedObstacle;
            // if the same index of each array is occupied (if an entire cross section of the lane is occupied by obstacles)
            if (gLeftArray[iRandLanePos]!=null && gMidArray[iRandLanePos]!=null && gRightArray[iRandLanePos]!=null)
            { 
                // and if each one is a tall object (and thus is impassable)
                if (gLeftArray[iRandLanePos].gameObject.name == "Tall" && gMidArray[iRandLanePos].gameObject.name == "Tall" && gRightArray[iRandLanePos].gameObject.name == "Tall")
                {
                    // make the current one a jumpable object
                    gSelectedLane[iRandLanePos] = gObstacles[1];
                }
            }
        }
        //call the create obstacles method for each lane
        CreateObstacles(gLeftArray, 0);
        CreateObstacles(gMidArray, 1);
        CreateObstacles(gRightArray, 2);
    }

    /// <summary>
    ///     GenObstacles()
    ///     loops through each object in the obstacles array and spawns it in the correct position as a child object of the lane
    /// </summary>
    /// <param name="obstacles"> An array of obstacles from the GenerateObstacles method to be spawned on the lane</param>
    /// <param name="lane">The value of the lane in the gLanes array that the object will be spawned in</param>

    void CreateObstacles(GameObject[] obstacles, int lane)
    {
        //iter used to iterate through the array
        int iter = 0;
        //float used to position the obstacle on the z axis
        float fZpos = 0.2f;
        // for each gameobject obstacle in the array obstacles
        while(iter < obstacles.Length - 1)
        {
			// ############################################################################################################################
			/* ###### Calum changed this to a while loop as not using the variable in the foreach loop was returning a minor error ########*/
			// ############################################################################################################################

			// if current index of the obstacles array is not null
            if (obstacles[iter])
            {
                // instantiate new vector3 to use as the spawning position
                Vector3 v3SpawnPosition = new Vector3();
                // switch to change the y value of the spawning position
                switch (obstacles[iter].gameObject.name)
                {
                    case ("tall"):
                        //tall objects need to be at 2.5f on the y axis
                        v3SpawnPosition.y = 2.5f;
                        break;
                    case ("flat"):
                        //flat objects need to be at 0.1f on the y axis
                        v3SpawnPosition.y = 0.1f;
                        break;
                }
                // set the z position to the FZStartPos variable which gets added to outside this if 
                v3SpawnPosition.z = fZpos;
                // instantiate the current obstacle as gObstacle so it can be edited after
                GameObject gObstacle = (GameObject)Instantiate(obstacles[iter], v3SpawnPosition, transform.rotation);
                // make the parent of the obstacle the current lane
                gObstacle.transform.parent = gLanes[lane].transform;
                //position the obstacle relative to the parent lane
                gObstacle.transform.localPosition = v3SpawnPosition;
            }
            //subtract 0.4f from the Z position 
            fZpos -= 0.4f;
            //add to iteration so increment the current obstacle of the obstacles array
            iter++;
        }
    }

}
