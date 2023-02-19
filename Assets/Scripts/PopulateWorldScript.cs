///<author> Graham Alexander MacDonald 28/03/2015 </author>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PopulateWorldScript : MonoBehaviour
{
	public List<GameObject> LgoMountains;
	public GameObject goSingleMountain;

	public List<GameObject> LgoVillages;
	public GameObject goDistantVillageA;
	public GameObject goDistantVillageB;
	public GameObject goDistantVillageC;

	public int iBuildAmount = 50;
	public int iBuildWidth = 1000;
	public int iBuildHeight;

	public Vector3 v3SpawnPoint;

	// Use this for initialization
	void Start ()
	{
		if (Application.loadedLevelName == "TutorialScene")
			iBuildHeight = -100;
		else
			iBuildHeight = 0;
		
		LoadResources ();
		GenerateListMountains ();
		GenerateListVillages ();
		while(iBuildAmount > 0) Build();
	}

	public void GenerateListMountains()
	{
		LgoMountains.Add (goSingleMountain);
	}

	public void GenerateListVillages()
	{
		LgoVillages.Add (goDistantVillageA);
		LgoVillages.Add (goDistantVillageB);
		LgoVillages.Add (goDistantVillageC);
	}

	public void LoadResources()
	{
		goSingleMountain = (GameObject)Resources.Load ("Prefabs/SingleMountain");
		goDistantVillageA = (GameObject)Resources.Load ("Prefabs/DistantVillageA");
		goDistantVillageB = (GameObject)Resources.Load ("Prefabs/DistantVillageB");
		goDistantVillageC = (GameObject)Resources.Load ("Prefabs/DistantVillageC");
		Debug.Log ("Got Resources");
	}

	public void Build()
	{
		//Get Random position
		float fX = Random.Range ((float)-iBuildWidth, (float)iBuildWidth);
		float fZ = Random.Range ((float)-iBuildWidth, (float)iBuildWidth);

		v3SpawnPoint = new Vector3(fX, iBuildHeight , fZ);

		float fRandMount = Random.Range (-1.0f, 1.0f);
		float fRandVillage = Random.Range (-1.0f, 1.0f);

		int i;

		if (fRandVillage > fRandMount)
		{
			i = (int)Random.Range (0.0f, (float)LgoVillages.Count);
			Instantiate (LgoVillages [i], v3SpawnPoint, Quaternion.identity);
		} 
		else
		{
			i = (int)Random.Range (0.0f, (float)LgoMountains.Count);
			Instantiate (LgoMountains [i].gameObject, v3SpawnPoint, Quaternion.identity);;
		}
		
		iBuildAmount--;
	}
}
