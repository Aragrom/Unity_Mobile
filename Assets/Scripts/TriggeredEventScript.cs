///<author> Graham Alexander MacDonald 18/03/2015 </author>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TriggeredEventScript : MonoBehaviour
{
	public bool bArmed = true;
	public List<GameObject> LgoBarrels;
	public GameObject goPlayer;

	public List<GameObject> LgoPlanks;
	public GameObject goLeftPlank;
	public GameObject goMiddlePlank;
	public GameObject goRightPlank;

	public Material matLightBright;
	public Material matLightDark;

	// Use this for initialization
	void Start ()
	{
		if (goLeftPlank != null)
			LgoPlanks.Add (goLeftPlank);
		if (goMiddlePlank != null)
			LgoPlanks.Add (goMiddlePlank);
		if (goRightPlank != null)
			LgoPlanks.Add (goRightPlank);

		LoadResources ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!bArmed) {
			if (this.gameObject.transform.parent.name == "BarrelAmbush(Clone)"
				|| this.gameObject.transform.parent.name == "BarrelAmbush")
				ReleaseBarrels ();

			if (this.gameObject.transform.parent.name == "PlankDrop(Clone)"
				|| this.gameObject.transform.parent.name == "PlankDrop")
			{
				ReleasePlank ();
				bArmed = true;
			}
		}
	}

	void LoadResources()
	{
		matLightBright = (Material)Resources.Load ("Material/Palace_PaperLightLP002_Diffusev1_Bright");
		matLightDark = (Material)Resources.Load ("Material/Palace_PaperLightLP002_Diffusev1_Dark");
	}

	void ReleaseBarrels()
	{
		foreach (GameObject go in LgoBarrels)
		{
			go.transform.GetComponent<Rigidbody>().useGravity = true;
		}
	}

	void ReleasePlank()
	{
		float fRand = Random.Range (1.6f, 3.0f) - 1;
		GameObject goDroppedPlank = LgoPlanks [(int)fRand];
		//goLeftPlank.transform.rigidbody.isKinematic = false;
		//goRightPlank.transform.rigidbody.isKinematic = false;
		//goMiddlePlank.transform.rigidbody.isKinematic = false;
		
		GameObject goStart = goDroppedPlank.transform.parent.Find ("Lantern").gameObject;
		GameObject goEnd = goDroppedPlank.transform.parent.Find("Light").Find("Lantern").gameObject;

		goDroppedPlank.transform.GetComponent<Rigidbody>().useGravity = true;
		goStart.GetComponent<Light> ().enabled = true;
		goEnd.GetComponent<Light>().enabled = true;

		//Palace_PaperLightLP002

		GameObject goLightStart = goDroppedPlank.transform.parent.Find ("Palace_PaperLightLP002").gameObject;
		GameObject goLightEnd = goDroppedPlank.transform.parent.Find ("Light").Find ("Palace_PaperLightLP002").gameObject;

		goLightStart.GetComponent<Renderer>().material = matLightBright;
		goLightEnd.GetComponent<Renderer>().material = matLightBright;

		//change material ############################################################################################################################
	}

	void FindCloseBarrels()
	{
		GameObject[] aGo = GameObject.FindGameObjectsWithTag ("Obstacle");
		Vector3 v3DistanceToTrigger = new Vector3 (30,0,30);

		foreach (GameObject go in aGo)
		{
			if (go.name == "Barrel")
			{
				Vector3 v3Result = go.transform.position - goPlayer.transform.position;
				
				v3Result = new Vector3 (Mathf.Abs (v3Result.x),Mathf.Abs (v3Result.y),Mathf.Abs (v3Result.z));
				
				if(v3Result.x < v3DistanceToTrigger.x && v3Result.z < v3DistanceToTrigger.z)
					LgoBarrels.Add(go);
			}
		}
	}

	void OnTriggerEnter(Collider c)
	{
		if (c.tag == "Player")
		{
			goPlayer = c.gameObject;
			if(this.gameObject.transform.parent.name == "BarrelAmbush(Clone)"
			   ||this.gameObject.transform.parent.name == "BarrelAmbush") FindCloseBarrels();
			bArmed = false;
		}
	}
}
