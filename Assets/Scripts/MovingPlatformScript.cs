///<author> Graham Alexander MacDonald 28/02/2015 </author>

using UnityEngine;
using System.Collections;

public class MovingPlatformScript : MonoBehaviour
{
	public int iDir = 0;
	public GameObject goPlayer;
	public Vector3[] Av3WayPoints = new Vector3[2];
	public Vector3 v3Target;
	public float fSpeed = 5.0f;
	public bool bHasPlayer = false;

	public float fTimer = 2.0f;

	void Awake()
	{
		iDir = GameObject.Find ("LevelGenerator").GetComponent<LevelGeneratorScript> ().iBuildDirection;
	}

	// Use this for initialization
	void Start ()
	{
		CalculateWayPoints ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (fTimer > 0)
			fTimer -= Time.deltaTime;
		else if(goPlayer == null)
			goPlayer = GameObject.Find("Player(Clone)");

		MovePlatform ();
		CheckPosition ();
	}

	void CalculateWayPoints()
	{
		Vector3 v3 = new Vector3();
		if (iDir == (int)TurnPointScript.eDirection.north
		    || iDir == (int)TurnPointScript.eDirection.south) v3 = new Vector3(0,0,10);
		if (iDir == (int)TurnPointScript.eDirection.east
		    || iDir == (int)TurnPointScript.eDirection.west) v3 = new Vector3(10,0,0);

		Av3WayPoints[0] = this.transform.position + v3;
		Av3WayPoints[1] = this.transform.position - v3;

		v3Target = Av3WayPoints [1];
	}

	void CheckPosition()
	{
		Vector3 v3Integer = new Vector3 (this.transform.position.x,
		                         this.transform.position.y,
		                         this.transform.position.z);

		if (v3Integer == Av3WayPoints [0]) v3Target = Av3WayPoints [1];
		if (v3Integer == Av3WayPoints [1]) v3Target = Av3WayPoints [0];
	}

	void MovePlatform()
	{
		float fStep = fSpeed * Time.deltaTime;
		this.transform.position = Vector3.MoveTowards(this.transform.position, v3Target, fStep);

		if (bHasPlayer && goPlayer != null)
		{
			Vector3 v3PlayerTarget = new Vector3(v3Target.x, goPlayer.transform.position.y, v3Target.z);
			goPlayer.transform.position = Vector3.MoveTowards (goPlayer.transform.position, v3PlayerTarget, fStep);
		}
	}
}
