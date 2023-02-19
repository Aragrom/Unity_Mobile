///<author> Graham Alexander MacDonald 10/03/2015 </author>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HouseOnPathScript : MonoBehaviour
{
	public static GameObject goREAL_HOUSE_1;
	public static GameObject goREAL_HOUSE_2;
	public static GameObject goREAL_HOUSE_3;
	public static GameObject goREAL_BASE;

	public Vector3 v3SpawnPointA;
	public Vector3 v3SpawnPointB;

	public int iAmountToBuild;

	public GameObject goParentA;
	public GameObject goParentB;
	public GameObject goChild;

	// Use this for initialization
	void Start ()
	{
		LoadResources ();
		iAmountToBuild = AmountToBuild ();
		SetSpawnPoints ();
		BuildHouses ();
	}

	void BuildHouses()
	{
		if (iAmountToBuild > 0)
		{
			int i = 0;
			while (i <= iAmountToBuild)
			{
				if (i == iAmountToBuild)
				{
					goChild = (GameObject)Instantiate (goREAL_HOUSE_3, v3SpawnPointA, Quaternion.identity);
					goChild.transform.parent = goParentA.transform;
					goChild = (GameObject)Instantiate (goREAL_HOUSE_3, v3SpawnPointB, Quaternion.identity);
					goChild.transform.parent = goParentB.transform;
				} 
				if (i != iAmountToBuild)
				{
					if(goParentA == null) goParentA = (GameObject)Instantiate (goREAL_BASE, v3SpawnPointA, Quaternion.identity);
					else 
					{
						goChild = (GameObject)Instantiate (goREAL_BASE, v3SpawnPointA, Quaternion.identity);
						goChild.transform.parent = goParentA.transform;
					}
					if(goParentB == null) goParentB = (GameObject)Instantiate (goREAL_BASE, v3SpawnPointB, Quaternion.identity);
					else 
					{
						goChild = (GameObject)Instantiate (goREAL_BASE, v3SpawnPointB, Quaternion.identity);
						goChild.transform.parent = goParentB.transform;
					}

					v3SpawnPointA += Vector3.up * 5;
					v3SpawnPointB += Vector3.up * 5;
				}
				i++;
			}
		} 
		else
		{
			List<GameObject> LgoHouses = new List<GameObject>();
			LgoHouses.Add(goREAL_HOUSE_1);
			LgoHouses.Add(goREAL_HOUSE_2);
			LgoHouses.Add(goREAL_HOUSE_3);

			int iRand = (int)Random.Range(0.0f, 3.0f);

			Instantiate(LgoHouses[iRand], v3SpawnPointA, Quaternion.identity);
			Instantiate(LgoHouses[iRand], v3SpawnPointB, Quaternion.identity);			
		}
	}

	void LoadResources()
	{
		goREAL_HOUSE_1 = (GameObject)Resources.Load("Prefabs/RealHouse1");
		goREAL_HOUSE_2 = (GameObject)Resources.Load("Prefabs/RealHouse2");
		goREAL_HOUSE_3 = (GameObject)Resources.Load("Prefabs/RealHouse3");
		goREAL_BASE = (GameObject)Resources.Load("Prefabs/RealHouseBase");
	}

	void SetSpawnPoints()
	{
		float fGap = 0;

		//if name of path = this decide gap.
		if (this.gameObject.name == "Sub1(Clone)"
		    || this.gameObject.name == "Sub1")
		{
			fGap = 1.0f;
		}
		if (this.gameObject.name == "Sub2(Clone)"
		    || this.gameObject.name == "Sub2")
		{
			fGap = 1.5f;
		}
		if (this.gameObject.name == "Sub3(Clone)"
		    || this.gameObject.name == "Sub3"
		    || this.gameObject.name == "BarrelAmbush(Clone)"
		    || this.gameObject.name == "BarrelAmbush"
		    || this.gameObject.name == "PlankDrop(Clone)"
		    || this.gameObject.name == "PlankDrop"
		    || this.gameObject.name == "Back(Clone)"
		    || this.gameObject.name == "Back")
		{
			fGap = 2.0f;
		}

		Vector3 myRotation = this.transform.rotation.eulerAngles; //get Vector3 rotation x, y, z

		if (myRotation == new Vector3(0, 0, 0)
		    || myRotation == new Vector3(0, 180, 0)
		    || myRotation == new Vector3(0, 360, 0))
		{
			v3SpawnPointA = this.transform.position + (new Vector3(0,0,10) * fGap);
			v3SpawnPointB = this.transform.position + (new Vector3(0,0,10) * -fGap);
		}
		else if (myRotation == new Vector3(0, 90, 0)
		         || myRotation == new Vector3(0, 270, 0))
		{
			v3SpawnPointA = this.transform.position + (new Vector3(10,0,0) * fGap);
			v3SpawnPointB = this.transform.position + (new Vector3(10,0,0) * -fGap);
		}

		// zero y-axis
		v3SpawnPointA = new Vector3 (v3SpawnPointA.x, 0, v3SpawnPointA.z);
		v3SpawnPointB = new Vector3 (v3SpawnPointB.x, 0, v3SpawnPointB.z);
	}

	int AmountToBuild()
	{
		int i = (int)this.transform.position.y / 5;
		return i;
	}
}
