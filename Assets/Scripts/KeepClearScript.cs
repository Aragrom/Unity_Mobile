///<author> Graham Alexander MacDonald 15/03/2015 </author>

using UnityEngine;
using System.Collections;

public class KeepClearScript : MonoBehaviour
{

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnCollisionStay(Collision c)
	{
		if (c.gameObject.name == "New Game Object"
		    || c.gameObject.name == "New Game Object(Clone)"
		    || c.gameObject.name == "RealHouse1(Clone)"
		    || c.gameObject.name == "RealHouse2(Clone)"
		    || c.gameObject.name == "RealHouse3(Clone)"
		    || c.gameObject.name == "RealHouseBase(Clone)"
		    || c.gameObject.name == "House1(Clone)"
		    || c.gameObject.name == "House2(Clone)"
		    || c.gameObject.name == "House3(Clone))"
		    || c.gameObject.name == "Box001"
		    || c.gameObject.name == "Box002"
		    || c.gameObject.name == "House_walls1:Object001"
		    || c.gameObject.tag == "House")
		{
			Debug.Log ("Keep Clear Script: Deleted - " + c.gameObject.name);
			if(c.gameObject.transform.parent != null) Destroy (c.gameObject.transform.parent.gameObject);
			Destroy (c.gameObject);
		}
	}
}
